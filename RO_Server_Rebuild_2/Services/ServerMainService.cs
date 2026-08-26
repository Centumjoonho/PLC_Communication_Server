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
      
        private readonly ApiServer apiServer;
        private readonly ApiSettingsService apiSettingsService;
        private readonly PlcCollectionService plcCollectionService;
        
        public event Action<bool> CollectRunningChanged;
        public event Action<bool> ServerRunningChanged;
        public event Action<IList<PlcData>> CollectDataChanged;

        // 서버와 PLC의 시작·정지 작업이 동시에 실행되지 않도록 보호
        private readonly SemaphoreSlim operationSemaphore = new SemaphoreSlim(1, 1);

        public ServerMainService( ApiServer apiServer,  ApiSettingsService apiSettingsService, PlcCollectionService plcCollectionService)
        {
            if (apiServer == null)
            {
                throw new ArgumentNullException(nameof(apiServer));
            }

            if (apiSettingsService == null)
            {
                throw new ArgumentNullException(nameof(apiSettingsService));
            }

            if (plcCollectionService == null)
            {
                throw new ArgumentNullException(nameof(plcCollectionService));
            }

            this.apiServer = apiServer;
            this.apiSettingsService = apiSettingsService;
            this.plcCollectionService = plcCollectionService;

            // PlcCollectionService의 상태와 데이터를 Presenter에 전달
            this.plcCollectionService.RunningChanged += OnCollectRunningChanged;
            this.plcCollectionService.DataChanged += OnCollectDataChanged;
        }
        // Presenter가 시작 버튼을 눌렀을 때 현재 상태를 확인용
        public bool IsCollectRunning()
        {
            return plcCollectionService.IsRunning();
        }
        public bool IsServerRunning()
        {
            return apiServer.IsRunning;
        }

        // PLC 수집 상태를 Presenter에 전달
        private void OnCollectRunningChanged(bool running)
        {
            CollectRunningChanged?.Invoke(running);
        }

        // PLC 수집 결과를 Presenter에 전달
        private void OnCollectDataChanged(IList<PlcData> plcDataList)
        {
            CollectDataChanged?.Invoke(plcDataList);
        }


        public bool StartServer(out string errorMessage)
        {
            errorMessage = string.Empty;

            // 다른 시작·정지 작업이 진행 중이면 새 작업을 시작하지 않음
            if (!operationSemaphore.Wait(0))
            {
                errorMessage = "서버 또는 PLC 시작·정지 처리가 진행 중입니다.";
                return false;
            }

            try
            {
                if (apiServer.IsRunning) 
                { 
                    return true;
                }

                ApiSettings apiSettings;

                if (!apiSettingsService.TryGetCurrentSettings(out apiSettings))
                {
                    errorMessage = "API 설정을 먼저 적용하세요.";

                    return false;
                }

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
            finally
            {
                operationSemaphore.Release();
            }

        }

        //API 서버가 실행중일때만 PLC 수집 시작
        public bool StartCollect(out string errorMessage)
        {
            errorMessage = string.Empty;

            // 서버 또는 PLC 정지 작업과 수집 시작이 겹치지 않도록 처리
            if (!operationSemaphore.Wait(0))
            {
                errorMessage = "서버 또는 PLC 시작·정지 처리가 진행 중입니다.";
                return false;
            }
            try
            {
                if (!apiServer.IsRunning)
                {
                    errorMessage = "API 서버가 실행 중일 때만 PLC 수집을 시작할 수 있습니다.";

                    return false;
                }

                return plcCollectionService.StartCollect(out errorMessage);
            }
            finally
            {
                operationSemaphore.Release();
            }

        }

        //PLC 수집 정지
        public async Task<bool> StopCollectAsync()
        {
            // 다른 시작·정지 작업이 끝날 때까지 비동기로 대기
            await operationSemaphore.WaitAsync();

            try
            {
                return await plcCollectionService.StopCollectAsync();

            }
            finally
            {
                operationSemaphore.Release();
            }
        }

        // API 서버와 PLC 수집을 모두 정지시키는 메서드
        public async Task<bool> StopServerAsync()
        {
            await operationSemaphore.WaitAsync();

            try
            {
                bool collectStopped = true;
                bool serverStopped = true;

                try
                {
                    // 수집 상태와 관계없이 남아 있는 Task와 DB Worker까지 정리
                    collectStopped = await plcCollectionService.StopCollectAsync();

                    if (!collectStopped)
                    {
                        LogService.Error("API 서버 정지 과정에서 PLC 통신 정리가 정상적으로 완료되지 않았습니다.");
                    }

                    // 서버 통신 정지 : 비정상 종료시에도 남은 Listener와 Client까지 정리 
                    serverStopped = await apiServer.StopListeningAsync();

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
            finally
            {
                operationSemaphore.Release();
            }

        }
    }
}
