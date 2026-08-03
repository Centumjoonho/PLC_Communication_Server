using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Plc;
using RO_Server_Rebuild_2.Store;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class ServerMainService
    {
        private readonly PlcDb plcDb;
        private readonly PlcReader plcReader;
        private readonly ApiServer apiServer;
        private readonly PlcDataStore plcDataStore;
        private readonly ApiSettingsService apiSettingsService;

        private const int MaxPlcParallelCount = 15;
       
        private CancellationTokenSource cancellationTokenSource;
        
        private Task collectTask;
        
        private volatile bool collectRunning;
        
        public event Action<bool> CollectRunningChanged;
        public event Action<bool> ServerRunningChanged;
        public event Action<IList<PlcData>> CollectDataChanged;

        public ServerMainService(PlcDb plcDb , PlcReader plcReader, ApiServer apiServer, PlcDataStore plcDataStore, ApiSettingsService apiSettingsService)
        {
            this.plcDb = plcDb;
            this.plcReader = plcReader;
            this.apiServer = apiServer;
            this.plcDataStore = plcDataStore;
            this.apiSettingsService = apiSettingsService;
        }
        // Presenter가 시작 버튼을 눌렀을 때 현재 상태를 확인용
        public bool IsCollectRunning()
        {
            return collectRunning;
        }
        public bool IsServerRunning()
        {
            return apiServer.IsRunning;
        }

        private void SetCollectRunning(bool running)
        {
            if(collectRunning == running) { return; }
            collectRunning = running;
            CollectRunningChanged?.Invoke(running);

        }
        // PLC 목록 전체를 한 차례 수집
        private async Task<IList<PlcData>> ReadAllPlcDataAsync(IList<PlcMaster> plcMasterList , CancellationToken cancellationToken)
        {
            
            if(plcMasterList == null || plcMasterList.Count ==0 )
            {
                return new List<PlcData>();
            }
            // 동시에 병렬 처리 하는 갯수
            int parallelCount = Math.Min(MaxPlcParallelCount, plcMasterList.Count);

            using (SemaphoreSlim plcSemaphore = new SemaphoreSlim(parallelCount))
            {
                List<Task<PlcData>> readTaskList = new List<Task<PlcData>>();

                foreach (PlcMaster plcMaster in plcMasterList)
                {

                    Task<PlcData> plcWork = ReadPlcWorker(plcMaster,plcSemaphore , cancellationToken);

                    readTaskList.Add(plcWork);
                }

                PlcData[] resultArray = await Task.WhenAll(readTaskList);

                return new List<PlcData>(resultArray);

            }
           
        }
        // PLC 한 대의 오류 처리와 Semaphore 관리
        private async Task<PlcData> ReadPlcWorker(PlcMaster plcMaster, SemaphoreSlim plcSemaphore , CancellationToken cancellationToken)
        {
            await plcSemaphore.WaitAsync(cancellationToken);

            try
            {
                return await plcReader.ReadPlcData(plcMaster, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 수집 정지 요청은 통신 오류 데이터로 바꾸지 않고 상위 반복문으로 전달
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

        public bool StartCollect(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsServerRunning())
            {
                errorMessage = " 서버를 먼저 시작하세요";
                
                return false;
            }

            if (collectTask != null && !collectTask.IsCompleted)
            {
                return true;
            }
            try
            {
                List<PlcMaster> plcMasterList = plcDb.ReadUsePlcs();

                if(plcMasterList.Count == 0)
                {
                    errorMessage = "수집 대상 PLC 가 없습니다.";
                    
                    return false;
                }

                if(cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                }
                cancellationTokenSource = new CancellationTokenSource();

                CancellationToken cancellationToken = cancellationTokenSource.Token;
                
                SetCollectRunning(true);

                // 실제 루프 수집 함수 
                collectTask = Task.Run(() => CollectLoopAsync(plcMasterList, cancellationToken));

                return true;

            }
            catch (Exception ex)
            {
                SetCollectRunning(false);

                errorMessage ="PLC 반복 수집 시작 실패 :" + ex.Message;

                return false;
            }
        }
        public async Task<bool> StopCollectAsync()
        {
            if(cancellationTokenSource == null)
            {
                return true;
            }
            try
            {
                cancellationTokenSource.Cancel();
                
                if(collectTask != null && !collectTask.IsCompleted)
                {
                    await collectTask;
                }

                return true;
            }
            catch (Exception ex)
            {

                LogService.Error("PLC 반복 수집 정지 실패 : " + ex.Message);
                
                return false;
            }
            finally
            {
                SetCollectRunning(false );
                cancellationTokenSource.Dispose();
                cancellationTokenSource=null;
                collectTask = null;
            }
        }
        //1초마다 전체 수집 실행
        private async Task CollectLoopAsync(List<PlcMaster> plcMasterList, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    DateTime loopStartTime = DateTime.Now;

                    IList<PlcData> plcDataList = await ReadAllPlcDataAsync(plcMasterList ,cancellationToken);
                    // Plc 통신 값을 메모리에 저장 하여 필요할 때 사용
                    plcDataStore.SetCollectList(plcDataList);

                    CollectDataChanged?.Invoke(plcDataList);

                    int elapseMilliseconds = (int) (DateTime.Now - loopStartTime).TotalMilliseconds;
                    int delayMilliseconds = 1000 - elapseMilliseconds;

                    if (delayMilliseconds > 0) {

                        await Task.Delay(delayMilliseconds, cancellationToken);
                    }

                }
            }
            catch (OperationCanceledException)
            {
                // 정상적인 수집 정지
            }
            catch (Exception ex)
            {

                LogService.Error("PLC 반복 수집 중 오류 발생 : " + ex.Message);
            }
            finally
            {
                SetCollectRunning(false);
            }
        }

        public bool StartServer(out string errorMessage)
        {
            ApiSettings apiSettings;
            errorMessage = string.Empty;

            if (apiServer.IsRunning)
            {
                return true;
            }
            

            if (!apiSettingsService.TryGetCurrentSettings(out apiSettings))
            {
                errorMessage = "API 설정을 먼저 적용하세요.";

                return false;
            }

            try
            {
                bool started = apiServer.StartListening(apiSettings.Port, out errorMessage);

                if (!started)
                {
                    return false;
                }

                ServerRunningChanged?.Invoke(true);

                LogService.Log("API 서버가 시작 되었습니다. Port : " + apiSettings.Port);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "API 서버 시작 실패 : " + ex.Message;  
                return false;
            }
        }

        public async Task<bool> StopServerAsync()
        {
            bool collectStopped = true;
            bool serverStopped = true;

            try
            {
                if (IsCollectRunning())
                {
                    collectStopped = await StopCollectAsync();

                    if (!collectStopped)
                    {
                        LogService.Error("API 서버 정지 과정에서 PLC 통신 정지 처리가 정상적으로 완료되지 않았습니다.");
                    }
                }
                // PLC 정지 결과와 관계없이 API 서버 정지 진행
                if (apiServer.IsRunning)
                {
                    serverStopped = await apiServer.StopListeningAsync();
                }

                ServerRunningChanged?.Invoke(apiServer.IsRunning);

                if (!serverStopped)
                {
                    LogService.Error("API 서버 정지에 실패했습니다.");
                }

                if (collectStopped && serverStopped)
                {
                    LogService.Log("PLC 통신과 API 서버가 모두 정지되었습니다.");
                }

                return collectStopped && serverStopped;

            }
            catch (Exception ex)
            {

                LogService.Error("API 서버 전체 정지 처리 실패 : " + ex.Message);

                ServerRunningChanged?.Invoke(apiServer.IsRunning);

                return false;
            }
        }
    }
}
