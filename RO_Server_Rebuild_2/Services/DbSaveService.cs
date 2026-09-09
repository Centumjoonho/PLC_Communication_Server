using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RO_Server_Rebuild_2.Services
{
    public class DbSaveService : IDisposable
    {
        // 큐는 처리 순서를 전달하는 버퍼. 미저장 데이터의 원본은 아래 Dictionary/파일.
        private const int MaxSaveQueueCount = 500;
        private const int RetryDelaySeconds = 5;
        private readonly PlcDb plcDb;
        private readonly object stateLock = new object();
        private BlockingCollection<DbSaveItem> saveQueue;
        private readonly Dictionary<string, DbSaveItem> pendingSaveItems =
            new Dictionary<string, DbSaveItem>(StringComparer.Ordinal);
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        private readonly string pendingRoot;
        private Task saveWorkerTask;
        private bool accepting;
        private bool disposed;
        private long nextVersion;
        private DateTime stopDeadlineUtc;
        private DateTime nextDbErrorLogUtc;
        private DateTime nextLocalErrorLogUtc;
        private DateTime nextStatusLogUtc;
        private string storeDirectory;
        private FileStream storeLease;

        // public은 JSON 복원에 필요. 외부에서는 이 객체를 직접 변경하지 않는다.
        public class DbSaveItem
        {
            public PlcData PlcData { get; set; }
            [ScriptIgnore] public DateTime WorkDate { get; set; }
            // 날짜는 UTC 변환하지 않는다. JavaScriptSerializer의 DateTime 변환 방지.
            public string WorkDateText
            {
                get { return WorkDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
                set { WorkDate = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture); }
            }
            public bool SaveDaily { get; set; }
            public bool SaveHistory { get; set; }
            public string PendingKey { get; set; }
            public long Version { get; set; }
            public string MessageId { get; set; }
            public DateTime EnqueuedAtUtc { get; set; }
            [ScriptIgnore] public bool Queued { get; set; }
            [ScriptIgnore] public bool InFlight { get; set; }
            [ScriptIgnore] public bool Persisted { get; set; }
            [ScriptIgnore] public DateTime RetryAtUtc { get; set; }
        }

        public DbSaveService(PlcDb plcDb, string pendingRoot = null)
        {
            if (plcDb == null) throw new ArgumentNullException(nameof(plcDb));
            this.plcDb = plcDb;
            this.pendingRoot = pendingRoot ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RO_Server_Rebuild_2", "DbPending");
        }

        public bool IsRunning()
        {
            lock (stateLock)
                return accepting && saveWorkerTask != null && !saveWorkerTask.IsCompleted;
        }

        public bool HasUnpersistedData()
        {
            lock (stateLock) return pendingSaveItems.Values.Any(x => !x.Persisted);
        }

        public bool DBSaveWorkStart()
        {
            lock (stateLock)
            {
                if (disposed) return false;
                if (saveWorkerTask != null && !saveWorkerTask.IsCompleted) return accepting;
                try
                {
                    EnsureStoreLoaded();
                    if (saveQueue != null) saveQueue.Dispose();
                    saveQueue = new BlockingCollection<DbSaveItem>(MaxSaveQueueCount);
                    foreach (DbSaveItem item in pendingSaveItems.Values)
                    {
                        item.Queued = false;
                        item.InFlight = false;
                        item.RetryAtUtc = DateTime.MinValue;
                    }
                    accepting = true;
                    nextStatusLogUtc = DateTime.UtcNow.AddSeconds(10);
                    BlockingCollection<DbSaveItem> runningQueue = saveQueue;
                    SchedulePending(runningQueue);
                    saveWorkerTask = Task.Run(() => ProcessQueue(runningQueue));
                    return true;
                }
                catch (Exception ex)
                {
                    accepting = false;
                    LogService.Error("[DB][SAVE_WORKER][START][FAIL] " + ex.Message);
                    return false;
                }
            }
        }

        // 기존 호출 방식 유지. true=저장 서비스가 인수했음, MySQL 완료와는 다름.
        public bool EnqueueSave(PlcData plcData, DateTime workDate, bool saveDaily, bool saveHistory)
        {
            if (!ValidData(plcData)) return false;
            PlcData copiedData = CopyPlcData(plcData);
            lock (stateLock)
            {
                if (!accepting || saveWorkerTask == null || saveWorkerTask.IsCompleted) return false;

                // History 요청에도 현재 상태는 일반 병합 항목으로 보낸다.
                // 오래된 History 항목이 나중에 Latest/Daily를 덮어쓰는 것을 방지.
                DbSaveItem state = SetPendingState(copiedData, workDate, saveDaily);
                var changedItems = new List<DbSaveItem> { state };
                if (saveHistory)
                {
                    string id = Guid.NewGuid().ToString("N");
                    var history = new DbSaveItem
                    {
                        PlcData = copiedData, WorkDate = workDate.Date,
                        SaveDaily = false, SaveHistory = true,
                        PendingKey = "H|" + id, MessageId = id,
                        Version = ++nextVersion, EnqueuedAtUtc = DateTime.UtcNow
                    };
                    pendingSaveItems.Add(history.PendingKey, history);
                    changedItems.Add(history);
                }
                // 메모리 인수 후 로컬 보관. 실패 시 메모리에 유지하며 재시도/오류 알림.
                // DB 응답을 기다리지는 않지만 로컬 디스크 기록 시간은 소요된다.
                foreach (DbSaveItem item in changedItems) Persist(item);
                SchedulePending(saveQueue);
                return true;
            }
        }

        // 영업일 전환/정지 시 최종 누적값. Latest에는 영향을 주지 않는 별도 키 사용.
        public bool EnqueueDaily(PlcData plcData, DateTime workDate)
        {
            if (!ValidData(plcData)) return false;
            lock (stateLock)
            {
                if (!accepting) return false;
                PlcData copy = CopyPlcData(plcData);
                string key = "D|" + StateKey(copy.PlcCode, workDate);
                DbSaveItem item;
                if (!pendingSaveItems.TryGetValue(key, out item))
                {
                    item = new DbSaveItem { PendingKey = key, EnqueuedAtUtc = DateTime.UtcNow };
                    pendingSaveItems.Add(key, item);
                }
                item.PlcData = copy;
                item.WorkDate = workDate.Date;
                item.SaveDaily = true;
                item.Version = ++nextVersion;
                item.Persisted = false;
                Persist(item);
                SchedulePending(saveQueue);
                return true;
            }
        }

        private DbSaveItem SetPendingState(PlcData data, DateTime date, bool saveDaily)
        {
            string key = "S|" + StateKey(data.PlcCode, date);
            DbSaveItem item;
            if (!pendingSaveItems.TryGetValue(key, out item))
            {
                item = new DbSaveItem { PendingKey = key, EnqueuedAtUtc = DateTime.UtcNow };
                pendingSaveItems.Add(key, item);
            }
            item.PlcData = data;
            item.WorkDate = date.Date;
            item.SaveDaily |= saveDaily;
            item.Version = ++nextVersion;
            item.Persisted = false;
            return item;
        }

        // stateLock 내부에서만 호출. 큐가 가득 차도 Dictionary/파일에서 유실되지 않음.
        private void SchedulePending(BlockingCollection<DbSaveItem> runningQueue)
        {
            if (runningQueue.IsAddingCompleted) return;
            DateTime now = DateTime.UtcNow;
            foreach (DbSaveItem item in pendingSaveItems.Values
                .Where(x => !x.Queued && !x.InFlight && x.RetryAtUtc <= now)
                .OrderBy(x => x.EnqueuedAtUtc))
            {
                if (!runningQueue.TryAdd(item)) break;
                item.Queued = true;
            }
        }

        private void ProcessQueue(BlockingCollection<DbSaveItem> runningQueue)
        {
            while (true)
            {
                lock (stateLock)
                {
                    SchedulePending(runningQueue);
                    if (!accepting && (pendingSaveItems.Count == 0 || DateTime.UtcNow >= stopDeadlineUtc)) return;
                }
                DbSaveItem item;
                if (!runningQueue.TryTake(out item, 100))
                {
                    lock (stateLock)
                    {
                        if (accepting) continue;
                        // CompleteAdding 이후에는 큐에 재등록하지 않고 남은 항목을 직접 처리.
                        item = pendingSaveItems.Values.OrderBy(x => x.EnqueuedAtUtc).FirstOrDefault();
                        if (item == null) return;
                    }
                }

                DbSaveItem snapshot;
                lock (stateLock)
                {
                    item.Queued = false;
                    item.InFlight = true;
                    snapshot = Snapshot(item);
                    if (!item.Persisted) Persist(item);
                }

                Stopwatch watch = Stopwatch.StartNew();
                Exception error = null;
                try
                {
                    // DB 호출 중에는 stateLock을 잡지 않는다.
                    if (snapshot.SaveHistory)
                        plcDb.SaveHistory(snapshot.PlcData, snapshot.MessageId);
                    else
                    {
                        if (!snapshot.PendingKey.StartsWith("D|", StringComparison.Ordinal))
                            plcDb.SaveLatest(snapshot.PlcData);
                        if (snapshot.SaveDaily)
                            plcDb.SaveDaily(snapshot.PlcData, snapshot.WorkDate);
                    }
                    lock (stateLock)
                    {
                        if (item.Version == snapshot.Version)
                        {
                            File.Delete(ItemPath(item.PendingKey));
                            pendingSaveItems.Remove(item.PendingKey);
                        }
                        // Version이 바뀌었으면 최신 항목을 유지하여 다음 차례에 저장.
                    }
                }
                catch (Exception ex) { error = ex; }
                finally
                {
                    lock (stateLock)
                    {
                        item.InFlight = false;
                        if (error != null) item.RetryAtUtc = DateTime.UtcNow.AddSeconds(RetryDelaySeconds);
                    }
                }

                lock (stateLock)
                {
                    if (error != null && DateTime.UtcNow >= nextDbErrorLogUtc)
                    {
                        nextDbErrorLogUtc = DateTime.UtcNow.AddSeconds(10);
                        LogService.Error("[DB][SAVE][RETRY] [PLC:" + snapshot.PlcData.PlcCode +
                            "] [Elapsed:" + watch.ElapsedMilliseconds + "ms] " + error.Message);
                    }
                    if (DateTime.UtcNow >= nextStatusLogUtc)
                    {
                        nextStatusLogUtc = DateTime.UtcNow.AddSeconds(10);
                        if (error != null || pendingSaveItems.Count >= 50 || watch.ElapsedMilliseconds >= 1000)
                        {
                            int historyCount = pendingSaveItems.Values.Count(x => x.SaveHistory);
                            double oldest = pendingSaveItems.Count == 0 ? 0 :
                                (DateTime.UtcNow - pendingSaveItems.Values.Min(x => x.EnqueuedAtUtc)).TotalSeconds;
                            LogService.Log("[DB][SAVE_QUEUE][STATUS] [State:" +
                                (pendingSaveItems.Count - historyCount) + "] [History:" + historyCount +
                                "] [Oldest:" + oldest.ToString("F1") + "s] [SaveElapsed:" +
                                watch.ElapsedMilliseconds + "ms]");
                        }
                    }
                    // DB가 계속 실패할 때 정지 요청을 무한 대기시키지 않는다.
                    if (!accepting && error != null) return;
                }
            }
        }

        public async Task<bool> DBSaveWorkStopAsync()
        {
            Task worker;
            lock (stateLock)
            {
                accepting = false;
                stopDeadlineUtc = DateTime.UtcNow.AddSeconds(30);
                worker = saveWorkerTask;
                if (saveQueue != null && !saveQueue.IsAddingCompleted) saveQueue.CompleteAdding();
            }
            try { if (worker != null) await worker.ConfigureAwait(false); }
            catch (Exception ex) { LogService.Error("[DB][SAVE_WORKER][STOP][FAIL] " + ex.Message); }
            lock (stateLock)
            {
                foreach (DbSaveItem item in pendingSaveItems.Values)
                {
                    if (!item.Persisted) Persist(item);
                    item.Queued = false;
                    item.InFlight = false;
                }
                if (saveQueue != null) { saveQueue.Dispose(); saveQueue = null; }
                saveWorkerTask = null;
                bool safe = pendingSaveItems.Values.All(x => x.Persisted);
                if (pendingSaveItems.Count > 0)
                    LogService.Log("[DB][STOP][DEFERRED] 미전송: " + pendingSaveItems.Count +
                        "건, 로컬 보관 완료: " + safe + ". 다음 수집 시작 시 재전송합니다.");
                // Clear하지 않는다. DB 미전송이라도 로컬 보관되면 안전한 정지로 취급.
                return safe;
            }
        }

        public int[] RestorePendingDaily(string plcCode, DateTime workDate, int[] dbHours)
        {
            lock (stateLock)
            {
                EnsureStoreLoaded();
                var result = new int[24];
                if (dbHours != null) Array.Copy(dbHours, result, Math.Min(24, dbHours.Length));
                foreach (DbSaveItem item in pendingSaveItems.Values.Where(x => x.SaveDaily &&
                    x.WorkDate == workDate.Date && string.Equals(x.PlcData.PlcCode, plcCode, StringComparison.OrdinalIgnoreCase)))
                    for (int i = 0; i < 24; i++) result[i] = Math.Max(result[i], item.PlcData.HourSeconds[i]);
                return result;
            }
        }

        private void EnsureStoreLoaded()
        {
            if (disposed) throw new ObjectDisposedException(nameof(DbSaveService));
            string directory = Path.Combine(pendingRoot, Hash(plcDb.GetSaveStoreIdentity()));
            if (directory == storeDirectory && storeLease != null) return;
            if (accepting) throw new InvalidOperationException("수집 중 DB 저장 대상을 바꿀 수 없습니다.");
            foreach (DbSaveItem item in pendingSaveItems.Values.Where(x => !x.Persisted)) Persist(item);
            if (pendingSaveItems.Values.Any(x => !x.Persisted))
                throw new IOException("미저장 데이터를 로컬 파일에 보관하지 못했습니다.");
            Directory.CreateDirectory(directory);
            var lease = new FileStream(Path.Combine(directory, "writer.lock"),
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            var loaded = new Dictionary<string, DbSaveItem>(StringComparer.Ordinal);
            try
            {
                foreach (string file in Directory.EnumerateFiles(directory, "*.json"))
                {
                    DbSaveItem item = serializer.Deserialize<DbSaveItem>(File.ReadAllText(file, Encoding.UTF8));
                    if (item != null && item.PlcData != null && item.PlcData.ReceiveTime.Kind == DateTimeKind.Utc)
                        item.PlcData.ReceiveTime = item.PlcData.ReceiveTime.ToLocalTime();
                    if (item == null || !ValidData(item.PlcData) || item.PendingKey == null ||
                        item.PlcData.HourSeconds == null || item.PlcData.HourSeconds.Length != 24 ||
                        !ValidKey(item) || Hash(item.PendingKey) != Path.GetFileNameWithoutExtension(file))
                        throw new InvalidDataException("로컬 미전송 파일 확인 필요: " + file);
                    item.Persisted = true;
                    loaded.Add(item.PendingKey, item);
                }
            }
            catch { lease.Dispose(); throw; }
            if (storeLease != null) storeLease.Dispose();
            storeLease = lease;
            storeDirectory = directory;
            pendingSaveItems.Clear();
            foreach (var pair in loaded) pendingSaveItems.Add(pair.Key, pair.Value);
            nextVersion = loaded.Count == 0 ? 0 : loaded.Values.Max(x => x.Version);
        }

        private bool Persist(DbSaveItem item)
        {
            try
            {
                string target = ItemPath(item.PendingKey);
                string temp = target + ".tmp";
                byte[] bytes = Encoding.UTF8.GetBytes(serializer.Serialize(item));
                using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(target)) File.Replace(temp, target, null);
                else File.Move(temp, target);
                item.Persisted = true;
                return true;
            }
            catch (Exception ex)
            {
                item.Persisted = false;
                if (DateTime.UtcNow >= nextLocalErrorLogUtc)
                {
                    nextLocalErrorLogUtc = DateTime.UtcNow.AddSeconds(10);
                    LogService.Error("[DB][LOCAL_PENDING][FAIL] 메모리에만 보관 중; 종료 전 복구 필요. " + ex.Message);
                }
                return false;
            }
        }

        private static bool ValidKey(DbSaveItem item)
        {
            if (item.SaveHistory)
            {
                Guid id;
                return Guid.TryParseExact(item.MessageId, "N", out id) &&
                    item.PendingKey == "H|" + item.MessageId && !item.SaveDaily;
            }
            string key = StateKey(item.PlcData.PlcCode, item.WorkDate);
            return item.PendingKey == "S|" + key || (item.SaveDaily && item.PendingKey == "D|" + key);
        }
        private string ItemPath(string key) { return Path.Combine(storeDirectory, Hash(key) + ".json"); }
        private static string StateKey(string code, DateTime date)
        { return code.Trim().ToUpperInvariant() + "|" + date.ToString("yyyyMMdd"); }
        private static string Hash(string value)
        {
            using (SHA256 sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "");
        }
        private static bool ValidData(PlcData data) { return data != null && !string.IsNullOrWhiteSpace(data.PlcCode); }
        private DbSaveItem Snapshot(DbSaveItem item)
        {
            return new DbSaveItem
            {
                PlcData = CopyPlcData(item.PlcData), WorkDate = item.WorkDate,
                SaveDaily = item.SaveDaily, SaveHistory = item.SaveHistory,
                PendingKey = item.PendingKey, Version = item.Version, MessageId = item.MessageId,
                EnqueuedAtUtc = item.EnqueuedAtUtc
            };
        }
        private PlcData CopyPlcData(PlcData data)
        {
            var hours = new int[24];
            if (data.HourSeconds != null) Array.Copy(data.HourSeconds, hours, Math.Min(24, data.HourSeconds.Length));
            return new PlcData
            {
                PlcCode = data.PlcCode.Trim().ToUpperInvariant(), PlcName = data.PlcName,
                PlcIp = data.PlcIp, PlcPort = data.PlcPort, MemoryAddress = data.MemoryAddress,
                ReceiveData = data.ReceiveData, Status = data.Status, TotalSeconds = data.TotalSeconds,
                Rate = data.Rate, ReceiveTime = data.ReceiveTime, HourSeconds = hours, ErrorMessage = data.ErrorMessage
            };
        }

        public void Dispose()
        {
            lock (stateLock)
            {
                if (disposed) return;
                if (accepting || (saveWorkerTask != null && !saveWorkerTask.IsCompleted))
                    throw new InvalidOperationException("DBSaveWorkStopAsync 완료 후 Dispose해야 합니다.");
                if (pendingSaveItems.Values.Any(x => !x.Persisted))
                    throw new IOException("로컬 보관에 실패한 데이터가 남아 있습니다.");
                if (saveQueue != null) saveQueue.Dispose();
                if (storeLease != null) storeLease.Dispose();
                disposed = true;
            }
        }
    }
}
