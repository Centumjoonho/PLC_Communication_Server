using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Presenters
{
    public class ServerMainPresenter
    {
        private readonly IServerMainView smView;
        private readonly ServerMainService smService;

        //private bool serverToggleButtonRunning;
        //private bool collectToggleButtonRunning;

        // 서버와 PLC 시작·정지 작업의 동시 실행 방지
        private bool toggleOperationRunning;

        public ServerMainPresenter(IServerMainView smView, ServerMainService smService)
        {
            this.smView = smView;
            this.smService = smService;

            this.smView.CollectToggleRequested += OnCollectToggleRequested;

            this.smView.ServerToggleRequested += OnServerToggleRequested;

            this.smService.ServerRunningChanged += OnServerRunningChanged;

            this.smService.CollectRunningChanged += OnCollectRunningChanged;

            this.smService.CollectDataChanged += OnCollectDataChanged;
        }

        private async void OnServerToggleRequested(object sender, EventArgs e)
        {
            if (toggleOperationRunning)
            {
                return;
            }

            toggleOperationRunning = true;
            smView.SetOperationEnabled(false);

            try
            {
                if (smService.IsServerRunning()) {

                    bool stopped = await smService.StopServerAsync();
                    
                    if (!stopped) {

                        LogService.Error("[SERVER][TOGGLE][STOP][FAIL] 서버 전체 정리 처리에 실패했습니다.");
                    }

                    return;
                }
                else
                {
                    string errorMessage;

                    bool started = smService.StartServer(out errorMessage);

                    if (!started)
                    {
                        LogService.Error("[SERVER][TOGGLE][START][FAIL] " + errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {

                LogService.Error("[SERVER][TOGGLE][EXCEPTION] " + ex.Message);

            }
            finally
            {
                toggleOperationRunning = false;
                smView.SetOperationEnabled(true);
            }
           
            
        }
        private void OnServerRunningChanged(bool running)
        {
            smView.SetServerRunning(running);
        }

        private void OnCollectDataChanged(IList<PlcData> plcDataList)
        {
           smView.ShowCollectedData(plcDataList);
        }

        private void OnCollectRunningChanged(bool running)
        {
            smView.SetCollectRunning(running);
        }

        private async void OnCollectToggleRequested(object sender, EventArgs e)
        {
            if (toggleOperationRunning) { return; }

            toggleOperationRunning = true;
            smView.SetOperationEnabled(false);

            try
            {
                if (smService.IsCollectRunning())
                {
                    bool stopped = await smService.StopCollectAsync();
                    
                    if (!stopped)
                    {
                        LogService.Error("[PLC_COLLECT][TOGGLE][STOP][FAIL] PLC 반복 수집 정리에 실패했습니다.");
                    }

                    return;
                }

                string errorMessage = string.Empty;

                bool started = smService.StartCollect(out  errorMessage);

                if (!started) 
                {
                    LogService.Error("[PLC_COLLECT][TOGGLE][START][FAIL] " + errorMessage);
                }

            }
            catch (Exception ex)
            {

                LogService.Error("[PLC_COLLECT][TOGGLE][EXCEPTION] " + ex.Message);
            }
            finally {
                toggleOperationRunning = false;
                smView.SetOperationEnabled(true);
            }
        }

        public async Task<bool> ShutdownAsync()
        {
        
            return await smService.StopServerAsync();
        }
    }
}
