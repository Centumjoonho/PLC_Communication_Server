using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Plc;
using RO_Server_Rebuild_2.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class PlcCollectionService
    {
        private const int MaxPlcParallelCount = 15;
        private const int FailureLimit = 2;
        private const int SkipSeconds = 1;
        private const int LoopIntervalMs = 1000;
        private const string SkipErrorMessage = "연속 실패로 임시 통신 정지";

        private readonly PlcDb plcDb;
        private readonly PlcReader plcReader;
        private readonly RunRateService runRateService;
        private readonly DbSaveService dbSaveService;
        private readonly PlcDataStore plcDataStore;

        // 시작·정지 상태 보호
        private readonly object stateLock = new object();

        // PLC별 실패 횟수와 스킵 상태 보호
        private readonly object failureStateLock = new object();

        private readonly Dictionary<string, PlcFailureState> failureStates = new Dictionary<string, PlcFailureState>();

        private CancellationTokenSource cancellationTokenSource;
        private Task collectTask;
        private Task runRateTask;

        // 동시에 여러 정지 요청이 실행되지 않도록 보호
        private readonly SemaphoreSlim stopSemaphore = new SemaphoreSlim(1, 1);
        private volatile bool collectRunning;
        private volatile bool stopRunning;

        public event Action<bool> RunningChanged;
        public event Action<IList<PlcData>> DataChanged;

        private class PlcFailureState
        {
            public int FailureCount { get; set; }
            public DateTime SkipUntil { get; set; }
            public bool FailureConfirmed { get; set; }
        }

        public PlcCollectionService(PlcDb plcDb, PlcReader plcReader, RunRateService runRateService, DbSaveService dbSaveService, PlcDataStore plcDataStore)
        {
            if (plcDb == null)
            {
                throw new ArgumentNullException(nameof(plcDb));
            }

            if (plcReader == null)
            {
                throw new ArgumentNullException(nameof(plcReader));
            }

            if (runRateService == null)
            {
                throw new ArgumentNullException(nameof(runRateService));
            }

            if (dbSaveService == null)
            {
                throw new ArgumentNullException(nameof(dbSaveService));
            }

            if (plcDataStore == null)
            {
                throw new ArgumentNullException(nameof(plcDataStore));
            }

            this.plcDb = plcDb;
            this.plcReader = plcReader;
            this.runRateService = runRateService;
            this.dbSaveService = dbSaveService;
            this.plcDataStore = plcDataStore;
        }
        public bool StartCollect(out string errorMessage)
        {/*
            PrepareCollection()
            → DbSaveService.Start()
            → CancellationTokenSource 생성
            → SetRunning(true)
            → collectTask 시작
            → runRateTask 시작*/

            errorMessage = string.Empty;

            lock (stateLock)
            {
                // 이미 정상적으로 수집 중이면 중복으로 시작하지 않음
                if (collectRunning)
                {
                    return true;
                }
                if (stopRunning)
                {
                    errorMessage = "PLC 통신 정지 처리가 진행 중입니다.";

                    return false;
                }
                // 이전 Task가 아직 끝나지 않았다면 새 Task를 만들지 않음
                if (collectTask != null && !collectTask.IsCompleted)
                {
                    errorMessage = "이전 PLC 수집 작업이 아직 종료되지 않았습니다.";

                    return false;
                }

                if (runRateTask != null && !runRateTask.IsCompleted)
                {
                    errorMessage = "이전 가동시간 계산 작업이 아직 종료되지 않았습니다.";

                    return false;
                }

                // 이전에 종료된 CancellationTokenSource 정리
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }

                collectTask = null;
                runRateTask = null;

                List<PlcMaster> plcMasterList;

                // PLC 목록 조회와 DB 가동초 초기화
                bool prepareSuccess = PrepareCollection(out plcMasterList, out errorMessage);

                if (!prepareSuccess)
                {
                    return false;
                }

                bool dbSaveStarted = dbSaveService.DBSaveWorkStart();

                if(!dbSaveStarted)
                {
                    runRateService.ResetAll();
                    errorMessage = "PLC 수집을 위한 DB 저장 서비스 시작에 실패했습니다.";
                    return false;
                }
                cancellationTokenSource = new CancellationTokenSource();

                CancellationToken cancellationToken = cancellationTokenSource.Token;

                //Task 실행하기 전에 실행 상태로 변경
                SetRunning(true);

                // PLC 반복 수집 Task 시작
                collectTask = Task.Run(() => CollectLoopAsync(plcMasterList, cancellationToken));

                // 가동시간 계산 Task 시작
                runRateTask = Task.Run(() => RunRateLoopAsync(cancellationToken));

                LogService.Log("PLC 반복 수집이 시작되었습니다. 대상 : " + plcMasterList.Count + "대");

                return true;

            }

        }
        public async Task<bool> StopCollectAsync()
        {
            /* CancellationTokenSource.Cancel()
             → collectTask 종료 대기
             → runRateTask 종료 대기
             → DbSaveService.StopAsync()
             → CancellationTokenSource Dispose
             → RunRateService.ResetAll()
             → SetRunning(false)*/
            // 이미 다른 정지 요청이 실행 중이면 완료될 때까지 대기
            await stopSemaphore.WaitAsync();

            try
            {
                CancellationTokenSource runningTokenSource;
                Task runningCollectTask;
                Task runningRunRateTask;

                lock (stateLock)
                {
                    bool collectTaskCompleted = collectTask == null || collectTask.IsCompleted;
                    bool runRateTaskCompleted = runRateTask == null || runRateTask.IsCompleted;

                    // 모든 작업이 이미 정지된 상태
                    if (cancellationTokenSource == null &&
                        collectTaskCompleted &&
                        runRateTaskCompleted &&
                        !dbSaveService.IsRunning())
                    {
                        SetRunning(false);

                        return true;
                    }

                    stopRunning = true;

                    // await 이후에도 같은 실행 객체를 정리하도록 지역 변수에 보관
                    runningTokenSource = cancellationTokenSource;
                    runningCollectTask = collectTask;
                    runningRunRateTask = runRateTask;
                }

                bool taskStopSuccess = true;
                bool dbStopSuccess = true;

                try
                {
                    // 수집 Task와 가동초 Task에 정지 요청
                    if (runningTokenSource != null)
                    {
                        runningTokenSource.Cancel();
                    }

                    List<Task> runningTaskList = new List<Task>();

                    if (runningCollectTask != null)
                    {
                        runningTaskList.Add(runningCollectTask);
                    }

                    if (runningRunRateTask != null)
                    {
                        runningTaskList.Add(runningRunRateTask);
                    }

                    // 두 Task가 완전히 끝날 때까지 대기
                    if (runningTaskList.Count > 0)
                    {
                        await Task.WhenAll(runningTaskList);
                    }
                }
                catch (OperationCanceledException)
                {
                    // CancellationToken으로 종료된 정상 상황
                }
                catch (Exception ex)
                {
                    taskStopSuccess = false;

                    LogService.Error("PLC 수집 Task 정지 실패 : " + ex.Message);
                }

                try
                {
                    // 신규 DB 저장 요청을 막고 남아 있는 저장 작업 완료 대기
                    dbStopSuccess = await dbSaveService.DBSaveWorkStopAsync();
                }
                catch (Exception ex)
                {
                    dbStopSuccess = false;

                    LogService.Error("PLC DB 저장 작업 정지 실패 : " + ex.Message);
                }

                lock (stateLock)
                {
                    // 현재 정리한 실행 객체가 맞을 때만 필드 초기화
                    if (runningTokenSource == null || ReferenceEquals(cancellationTokenSource, runningTokenSource))
                    {
                        cancellationTokenSource = null;
                        collectTask = null;
                        runRateTask = null;
                    }
                }

                if (runningTokenSource != null)
                {
                    runningTokenSource.Dispose();
                }

                // 다음 통신 시작 시 DB 값으로 다시 초기화하도록 메모리 상태 제거
                runRateService.ResetAll();

                lock (failureStateLock)
                {
                    failureStates.Clear();
                }

                SetRunning(false);

                if (taskStopSuccess && dbStopSuccess)
                {
                    LogService.Log("PLC 반복 수집이 정지되었습니다.");
                }

                return taskStopSuccess && dbStopSuccess;
            }
            finally
            {
                stopRunning = false;

                stopSemaphore.Release();
            }
        }

        private bool PrepareCollection(out List<PlcMaster> plcMasterList, out string errorMessage)
        {
            plcMasterList = new List<PlcMaster>();
            errorMessage = string.Empty;
            try
            {
                // DB에서 사용하도록 설정된 PLC만 조회
                plcMasterList = plcDb.ReadUsePlcs();

                if (plcMasterList == null || plcMasterList.Count == 0)
                {
                    errorMessage = "PLC 정보가 없습니다.";
                    return false;
                }
                // 이전 통신에서 사용한 가동시간 계산 상태 제거
                runRateService.ResetAll();
                // 이전 통신에서 기록된 실패 횟수와 스킵상태 제거
                lock (failureStateLock)
                {
                    failureStates.Clear();
                }
                DateTime now = DateTime.Now;
                DateTime workDate = runRateService.GetWorkDate(now);

                foreach (PlcMaster plcMaster in plcMasterList)
                {
                    if (string.IsNullOrWhiteSpace(plcMaster.PlcCode))
                    {
                        errorMessage = "PLC Code가 없는 통신 대상이 존재합니다.";

                        runRateService.ResetAll();

                        return false;
                    }

                    // DB에 저장된 해당 PLC의 시간대별 가동초 조회
                    int[] savedHourSeconds = plcDb.ReadTodaySeconds(plcMaster.PlcCode, workDate);

                    // 통신 시작 시 DB 가동초를 기준으로 계산 상태 설정
                    runRateService.ResetPlcRunTime(plcMaster.PlcCode, now, savedHourSeconds);
                }

                    return true;

            }
            catch (Exception ex)
            {

                runRateService.ResetAll();

                errorMessage = "PLC 수집 준비 실패 : " + ex.Message;

                return false;
            }
            
        }
        // 등록된 PLC를 최대 15대씩 병렬로 한 번 읽음
        private async Task<IList<PlcData>> ReadAllPlcDataAsync(IList<PlcMaster> plcMasterList, CancellationToken cancellationToken)
        {
            if (plcMasterList == null || plcMasterList.Count == 0) { return new List<PlcData>(); } 

            int parallelCount = Math.Min(MaxPlcParallelCount, plcMasterList.Count);

            using(SemaphoreSlim plcSemaphore = new SemaphoreSlim(parallelCount))
            {
                List<Task<PlcData>> readTaskList = new List<Task<PlcData>>();

                foreach (PlcMaster plcMaster in plcMasterList)
                {
                    PlcData skipData;

                    if (TryCreateSkipData(plcMaster, out skipData))
                    {
                        readTaskList.Add(Task.FromResult(skipData));

                        continue;
                    }

                    Task<PlcData> plcWork = ReadPlcWorker(plcMaster, plcSemaphore, cancellationToken);

                    readTaskList.Add(plcWork);
                }
                PlcData[] resultArray = await Task.WhenAll(readTaskList);

                return new List<PlcData>(resultArray);
            }

        }

        // PLC 한 대를 읽고 통신 오류를 ERROR 데이터로 변환
        private async Task<PlcData> ReadPlcWorker(PlcMaster plcMaster, SemaphoreSlim plcSemaphore, CancellationToken cancellationToken)
        {
            await plcSemaphore.WaitAsync(cancellationToken);

            try
            {
                return await plcReader.ReadPlcData(plcMaster, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 사용자가 요청한 정상적인 통신 정지는 ERROR로 변환하지 않음
                throw;
            }
            catch (Exception ex)
            {
                return new PlcData
                {
                    PlcCode = plcMaster.PlcCode,
                    PlcName = plcMaster.PlcName,
                    PlcIp = plcMaster.PlcIp,
                    PlcPort = plcMaster.PlcPort,
                    MemoryAddress = plcMaster.MemoryAddress,
                    ReceiveData = "90000000",
                    Status = "ERROR",
                    ReceiveTime = DateTime.Now,
                    ErrorMessage = ex.Message
                };
            }
            finally
            {
                plcSemaphore.Release();
            }
        }

        // 현재 1초 스킵 기간에 해당하는 PLC인지 확인
        private bool TryCreateSkipData(PlcMaster plcMaster, out PlcData skipData)
        {
            skipData = null;

            if (plcMaster == null || string.IsNullOrWhiteSpace(plcMaster.PlcCode))
            {
                return false;
            }

            DateTime now = DateTime.Now;

            lock (failureStateLock)
            {
                PlcFailureState failureState;

                if (!failureStates.TryGetValue(plcMaster.PlcCode, out failureState))
                {
                    return false;
                }

                if (failureState.SkipUntil <= now)
                {
                    return false;
                }
            }

            skipData = new PlcData
            {
                PlcCode = plcMaster.PlcCode,
                PlcName = plcMaster.PlcName,
                PlcIp = plcMaster.PlcIp,
                PlcPort = plcMaster.PlcPort,
                MemoryAddress = plcMaster.MemoryAddress,
                ReceiveData = "90000000",
                Status = "ERROR",
                ReceiveTime = now,
                ErrorMessage = SkipErrorMessage
            };

            return true;
        }
        // PLC 통신 오류 횟수를 기록하고 연속 2회 실패 시 1초 스킵 설정
        // 반환값 true는 이번 오류를 장애 이력으로 저장한다는 의미
        private bool ApplyFailurePolicy(PlcData plcData)
        {
            if (plcData == null || string.IsNullOrWhiteSpace(plcData.PlcCode))
            {
                return false;
            }

            // 스킵 중에 생성된 ERROR 데이터는 새로운 실패로 계산하지 않음
            if (string.Equals(plcData.ErrorMessage, SkipErrorMessage, StringComparison.Ordinal))
            {
                return false;
            }

            int failureCount;
            bool saveHistory = false;

            lock (failureStateLock)
            {
                PlcFailureState failureState;

                if (!failureStates.TryGetValue(plcData.PlcCode, out failureState))
                {
                    failureState = new PlcFailureState();

                    failureStates[plcData.PlcCode] = failureState;
                }

                failureState.FailureCount++;

                failureCount = failureState.FailureCount;

                if (failureCount >= FailureLimit)
                {
                    // 다음 수집부터 설정된 시간 동안 실제 PLC 통신을 건너뜀
                    failureState.SkipUntil = DateTime.Now.AddSeconds(SkipSeconds);

                    // 스킵 종료 후 다시 실패 횟수를 계산하기 위해 초기화
                    failureState.FailureCount = 0;

                    // 같은 장애가 계속되는 동안 History는 한 번만 저장
                    if (!failureState.FailureConfirmed)
                    {
                        failureState.FailureConfirmed = true;
                        saveHistory = true;
                    }
                }
            }

            if (failureCount < FailureLimit)
            {
                LogService.Error(
                    "PLC 통신 실패 : " +
                    plcData.PlcCode + " / " +
                    failureCount + "회 / " +
                    plcData.ErrorMessage);
            }
            else
            {
                LogService.Error(
                    "PLC 연속 통신 실패 : " +
                    plcData.PlcCode + " / " +
                    SkipSeconds + "초 동안 통신을 건너뜁니다.");
            }

            return saveHistory;
        }

        //  정상 통신 복구 처리
        // 정상 응답이 들어오면 해당 PLC의 실패 상태 초기화
        private void ResetFailureState(string plcCode)
        {
            if (string.IsNullOrWhiteSpace(plcCode))
            {
                return;
            }

            bool recovered = false;

            lock (failureStateLock)
            {
                PlcFailureState failureState;

                if (!failureStates.TryGetValue(plcCode, out failureState))
                {
                    return;
                }

                recovered =
                    failureState.FailureCount > 0 ||
                    failureState.SkipUntil > DateTime.MinValue ||
                    failureState.FailureConfirmed;

                failureStates.Remove(plcCode);
            }

            if (recovered)
            {
                LogService.Log("PLC 통신 복구 : " + plcCode);
            }
        }

        // 한 차례 읽은 PLC 결과에 가동률과 DB 저장 조건 적용
        private IList<PlcData> ProcessCollectedData(IList<PlcData> readDataList)
        {
            List<PlcData> resultList = new List<PlcData>();

            if (readDataList == null)
            {
                return resultList;
            }

            foreach (PlcData plcData in readDataList)
            {
                if (plcData == null || string.IsNullOrWhiteSpace(plcData.PlcCode))
                {
                    continue;
                }

                // 현재 RUN, STOP, ERROR 상태 갱신
                bool statusUpdated = runRateService.UpdatePlcStatus(plcData);

                if (!statusUpdated)
                {
                    LogService.Error(plcData.PlcCode + " PLC 가동시간 상태가 초기화되지 않았습니다.");
                }

                bool saveHistory = false;

                if (string.Equals(plcData.Status, "ERROR", StringComparison.OrdinalIgnoreCase))
                {
                    saveHistory = ApplyFailurePolicy(plcData);
                }
                else
                {
                    ResetFailureState(plcData.PlcCode);
                }

                // 현재 시간대별 가동초, 전체 가동초, 가동률 적용
                runRateService.ApplyRunRate(plcData);

                DateTime workDate = runRateService.GetWorkDate(plcData.ReceiveTime);

                // Latest와 Daily는 항상 저장
                // History는 장애가 최초 확정된 경우에만 저장
                bool enqueueSuccess = dbSaveService.EnqueueSave(
                    plcData,
                    workDate,
                    true,
                    saveHistory);

                if (!enqueueSuccess)
                {
                    LogService.Error(plcData.PlcCode + " PLC DB 저장 요청을 등록하지 못했습니다.");
                }

                resultList.Add(plcData);
            }

            return resultList;
        }

        public bool IsRunning()
        {
            return collectRunning;
        }

        private void SetRunning(bool running)
        {
            if (collectRunning == running)
            {
                return;
            }

            collectRunning = running;

            RunningChanged?.Invoke(running);
        }

        private async Task CollectLoopAsync(IList<PlcMaster> plcMasterList , CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && collectRunning)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    // 등록된 PLC 목록을 최대 15대씩 병렬로 읽음
                    IList<PlcData> readDataList = await ReadAllPlcDataAsync(plcMasterList, cancellationToken);

                    // 상태, 가동률, 장애 정책, DB 저장 조건 적용
                    IList<PlcData> processedDataList = ProcessCollectedData(readDataList);

                    // API에서 사용할 최신 수집 결과 저장
                    plcDataStore.SetCollectList(processedDataList);

                    // 화면에 결과 전달 : Presenter
                    try
                    {
                        DataChanged?.Invoke(processedDataList);
                    }
                    catch (Exception ex)
                    {
                        // 화면 갱신 실패가 PLC 반복 수집을 중단시키지 않도록 처리
                        LogService.Error("PLC 수집 결과 화면 전달 실패 : " + ex.Message);
                    }

                    stopwatch.Stop();

                    int elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
                    int delayMilliseconds = LoopIntervalMs - elapsedMilliseconds;

                    // 수집 소요시간을 제외한 나머지만 기다려 전체 주기를 약 1초로 유지
                    if (delayMilliseconds > 0)
                    {
                        await Task.Delay(delayMilliseconds, cancellationToken);
                    }
                }

            }
            catch (OperationCanceledException)
            {
                // 사용자가 요청한 정상적인 PLC 통신 정지
            }
            catch (Exception ex)
            {
                LogService.Error("PLC 반복 수집 중 오류 발생 : " + ex.Message);
            }
            finally
            {
                // 수집 루프가 끝나면 가동초 루프도 종료할 수 있도록 상태 변경
                SetRunning(false);
            }
        }
        // PLC 통신 시간과 별개로 실제 시계 기준 1초마다 가동초 계산
        private async Task RunRateLoopAsync(CancellationToken cancellationToken)
        {
            //첫번째 가동초 계산 예정 시각
            DateTime nextCountTime = DateTime.Now.AddSeconds(1);

            try
            {
                while (!cancellationToken.IsCancellationRequested && collectRunning)
                {
                    TimeSpan waitTime = nextCountTime - DateTime.Now;

                    if(waitTime.TotalMilliseconds > 0)
                    {
                        int delayMilliseconds = (int)Math.Ceiling(waitTime.TotalMilliseconds);

                        await Task.Delay(delayMilliseconds, cancellationToken);
                    }

                    // 대기 중 수집이 정지됐다면 가동초를 증가시키지 않음
                    if (cancellationToken.IsCancellationRequested || !collectRunning)
                    {
                        break;
                    }

                    DateTime currentTime =DateTime.Now;

                    // 시스템 지연으로 여러 초가 지난 경우 누락된 초만큼 처리
                    while (nextCountTime <= currentTime)
                    {
                        runRateService.CountOneSecond(nextCountTime);
                        nextCountTime = nextCountTime.AddSeconds(1);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 사용자가 요청한 정상적인 PLC 통신 정지
            }
            catch (Exception ex)
            {
                LogService.Error("PLC 가동시간 계산 중 오류 발생 : " + ex.Message);
            }
            finally
            {
                // 가동초 루프에 문제가 생기면 수집 루프도 다음 반복에서 종료
                SetRunning(false);
            }
        }
    }
}
