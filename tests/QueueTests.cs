using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;

namespace RO_Server_Rebuild_2.Services
{
    public static class LogService
    {
        public static readonly List<string> Lines = new List<string>();
        public static void Log(string s) { lock (Lines) Lines.Add(s); }
        public static void Error(string s) { Log(s); }
    }
}

namespace RO_Server_Rebuild_2.Database
{
    public class PlcDb
    {
        public volatile bool Offline;
        public volatile bool AmbiguousHistory;
        public bool BlockLatest;
        public readonly ManualResetEvent Entered = new ManualResetEvent(false);
        public readonly ManualResetEvent Release = new ManualResetEvent(false);
        public readonly object Gate = new object();
        public readonly Dictionary<string, int> Latest = new Dictionary<string, int>();
        public readonly Dictionary<string, int> Daily = new Dictionary<string, int>();
        public readonly HashSet<string> History = new HashSet<string>();
        public int HistoryCalls;
        public string GetSaveStoreIdentity() { return "test|3306|queue|test"; }
        public void SaveLatest(PlcData data)
        {
            if (BlockLatest)
            {
                BlockLatest = false;
                Entered.Set();
                if (!Release.WaitOne(120000)) throw new TimeoutException("test release timeout");
            }
            if (Offline) throw new IOException("simulated DB outage");
            lock (Gate) Latest[data.PlcCode] = data.TotalSeconds;
        }
        public void SaveDaily(PlcData data, DateTime date)
        {
            if (Offline) throw new IOException("simulated DB outage");
            lock (Gate)
            {
                string key = data.PlcCode + "|" + date.ToString("yyyyMMdd");
                int old;
                Daily.TryGetValue(key, out old);
                Daily[key] = Math.Max(old, data.TotalSeconds);
            }
        }
        public void SaveHistory(PlcData data, string id)
        {
            if (Offline) throw new IOException("simulated DB outage");
            lock (Gate)
            {
                HistoryCalls++;
                History.Add(id);
                if (AmbiguousHistory)
                {
                    AmbiguousHistory = false;
                    throw new IOException("commit succeeded but response lost");
                }
            }
        }
    }
}

public static class QueueTests
{
    static readonly DateTime Day = new DateTime(2026, 9, 9);
    static int passed;
    static string Root;
    static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAIL: " + label);
        passed++;
        Console.WriteLine("PASS: " + label);
    }
    static PlcData Data(string code, int value)
    {
        var hours = new int[24]; hours[9] = value;
        return new PlcData { PlcCode = code, PlcName = code, Status = "RUN",
            ReceiveData = "1", TotalSeconds = value, HourSeconds = hours,
            ReceiveTime = Day.AddHours(9).AddSeconds(value) };
    }
    static int Pending(DbSaveService service)
    {
        object gate = typeof(DbSaveService).GetField("stateLock", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service);
        lock (gate)
            return ((IDictionary)typeof(DbSaveService).GetField("pendingSaveItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service)).Count;
    }
    static bool Until(Func<bool> check, int timeout = 15000)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeout) { if (check()) return true; Thread.Sleep(20); }
        return check();
    }
    static void CoalesceDuringSave()
    {
        var db = new RO_Server_Rebuild_2.Database.PlcDb { BlockLatest = true };
        using (var service = new DbSaveService(db, Path.Combine(Root, "coalesce")))
        {
            Check(service.DBSaveWorkStart(), "worker start");
            Check(service.EnqueueSave(Data("P00", 0), Day, true, false), "first sample accepted");
            Check(db.Entered.WaitOne(5000), "DB held mid-save");
            var watch = Stopwatch.StartNew();
            bool allAccepted = true;
            for (int round = 1; round <= 31; round++)
                for (int plc = 0; plc < 20; plc++)
                    allAccepted &= service.EnqueueSave(Data("P" + plc.ToString("00"), round), Day, true, false);
            Console.WriteLine("620 durable coalescing submissions: " + watch.ElapsedMilliseconds + "ms");
            Check(allAccepted, "620 updates accepted while DB blocked");
            Check(Pending(service) == 20, "only 20 pending state items, not 620");
            service.EnqueueSave(Data("P00", 32), Day, true, true);
            service.EnqueueSave(Data("P00", 33), Day, true, true);
            Check(Pending(service) == 22, "two histories retained independently");
            db.Release.Set();
            Check(Until(() => Pending(service) == 0), "all versions eventually saved");
            lock (db.Gate)
            {
                Check(db.Latest["P00"] == 33, "old in-flight version cannot erase newest value");
                Check(db.Latest.Count == 20 && db.Daily.Count == 20, "other PLCs are not starved");
                Check(db.History.Count == 2, "both history events saved");
            }
            Check(service.DBSaveWorkStopAsync().GetAwaiter().GetResult(), "clean shutdown");
        }
    }
    static void RestoreAndRetry()
    {
        string root = Path.Combine(Root, "restore");
        using (var service = new DbSaveService(new RO_Server_Rebuild_2.Database.PlcDb { Offline = true }, root))
        {
            Check(service.DBSaveWorkStart(), "offline worker start");
            service.EnqueueSave(Data("EB", 77), Day, true, true);
            Thread.Sleep(200);
            Check(service.DBSaveWorkStopAsync().GetAwaiter().GetResult(), "DB outage stop defers durably");
            Check(Pending(service) == 2, "failed state and history not discarded");
        }
        Check(Directory.GetFiles(root, "*.json", SearchOption.AllDirectories).Length == 2, "both unsent files survive service disposal");
        var db = new RO_Server_Rebuild_2.Database.PlcDb { AmbiguousHistory = true };
        using (var service = new DbSaveService(db, root))
        {
            Check(service.RestorePendingDaily("EB", Day, new int[24])[9] == 77, "startup restores uncommitted daily aggregate");
            Check(service.DBSaveWorkStart(), "restored worker start");
            Check(Until(() => Pending(service) == 0, 15000), "ambiguous DB write retries to completion");
            lock (db.Gate) Check(db.HistoryCalls == 2 && db.History.Count == 1, "same history ID reused on retry");
            Check(service.DBSaveWorkStopAsync().GetAwaiter().GetResult(), "restored worker stops cleanly");
        }
        Check(Directory.GetFiles(root, "*.json", SearchOption.AllDirectories).Length == 0, "acknowledged files removed");
    }
    static void WorkdayBoundary()
    {
        var rate = new RunRateService();
        var initial = new int[24]; initial[7] = 12;
        DateTime morning = Day.AddHours(8);
        rate.ResetPlcRunTime("EB", morning.AddSeconds(-1), initial);
        rate.UpdatePlcStatus(Data("EB", 0));
        PlcData closed = null; DateTime closedDate = DateTime.MinValue;
        rate.WorkDateClosing += (data, date) => { closed = data; closedDate = date; };
        var before = Data("EB", 0); before.ReceiveTime = morning;
        Check(rate.ApplyRunRateAndGetWorkDate(before) == Day.AddDays(-1) && before.TotalSeconds == 12,
            "workdate and aggregate captured atomically before reset");
        rate.CountOneSecond(morning);
        Check(closed != null && closed.TotalSeconds == 12 && closedDate == Day.AddDays(-1), "prior workday final aggregate captured");
        var after = Data("EB", 0);
        Check(rate.ApplyRunRateAndGetWorkDate(after) == Day && after.HourSeconds[8] == 1, "new workday starts separately");
    }
    static void FullNotificationQueue()
    {
        var db = new RO_Server_Rebuild_2.Database.PlcDb { BlockLatest = true };
        using (var service = new DbSaveService(db, Path.Combine(Root, "capacity")))
        {
            service.DBSaveWorkStart();
            service.EnqueueSave(Data("CC", 0), Day, true, false);
            Check(db.Entered.WaitOne(5000), "DB held for full-buffer test");
            bool accepted = true;
            for (int i = 1; i <= 510; i++) accepted &= service.EnqueueSave(Data("CC", i), Day, true, true);
            Check(accepted && Pending(service) == 511, "history beyond 500-item notification buffer retained");
            object queue = typeof(DbSaveService).GetField("saveQueue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service);
            Check((int)queue.GetType().GetProperty("Count").GetValue(queue) == 500, "notification queue stays bounded at 500");
            db.Release.Set();
            Check(Until(() => Pending(service) == 0, 20000), "unscheduled overflow is eventually processed");
            lock (db.Gate) Check(db.History.Count == 510 && db.Latest["CC"] == 510, "all overflow events and latest state saved");
            service.DBSaveWorkStopAsync().GetAwaiter().GetResult();
        }
    }
    static void CorruptStore()
    {
        string root = Path.Combine(Root, "corrupt");
        using (var service = new DbSaveService(new RO_Server_Rebuild_2.Database.PlcDb { Offline = true }, root))
        {
            service.DBSaveWorkStart();
            service.EnqueueSave(Data("CC", 8), Day, true, false);
            service.DBSaveWorkStopAsync().GetAwaiter().GetResult();
        }
        string file = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories).Single();
        File.WriteAllText(file, "{broken-json");
        using (var service = new DbSaveService(new RO_Server_Rebuild_2.Database.PlcDb(), root))
            Check(!service.DBSaveWorkStart(), "corrupt outbox refuses startup instead of dropping data");
        Check(File.Exists(file), "corrupt file retained for recovery");
    }
    public static int Main(string[] args)
    {
        Root = Path.GetFullPath(args[0]);
        Directory.CreateDirectory(Root);
        try
        {
            CoalesceDuringSave(); RestoreAndRetry(); WorkdayBoundary(); CorruptStore(); FullNotificationQueue();
            Console.WriteLine("ALL " + passed + " ASSERTIONS PASSED");
            return 0;
        }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
    }
}
