using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Presenters
{
    public class ApiSettingsPresenter
    {
        private readonly IApiSettingsView apiView;
        private readonly ApiSettingsService apiService;

        public ApiSettingsPresenter(IApiSettingsView apiView, ApiSettingsService apiService)
        {
            this.apiView = apiView;
            this.apiService = apiService;

            this.apiView.ApiTestRequested += OnApiTestRequested;
            this.apiView.ApiApplyRequested += OnApiApplyRequested;

            ShowCurrentApiSettings();
        }

        private void OnApiApplyRequested(object sender, EventArgs e)
        {
            apiView.SetApiOperationEnabled(false);

            try
            {
                ApiSettings apiSettings;
                string errorMessage;

                bool inputSuccess = apiView.TryGetApiSettings(out apiSettings, out errorMessage);

                if (!inputSuccess) 
                {
                    apiView.ShowError(errorMessage);

                    return;
                }

                bool applySuccess = apiService.ApplyApiSettings(apiSettings,out errorMessage);

                if (!applySuccess) {

                    apiView.ShowError(errorMessage);

                    return;
                }

                apiView.ShowApiSettings(apiSettings);

                apiView.ShowInfo("API 설정이 적용되었습니다.");

            }
            catch (Exception ex)
            {
                LogService.Error("API 설정 적용 처리 실패 : " + ex.Message);
            }
            finally
            {
                apiView.SetApiOperationEnabled(true);
            }
        }
        // API Port와 API Key 입력값만 확인
        private void OnApiTestRequested(object sender, EventArgs e)
        {
            apiView.SetApiOperationEnabled(false);
            try
            {
                ApiSettings apiSettings;
                string errorMessage;

                bool inputSuccess = apiView.TryGetApiSettings(out apiSettings, out errorMessage);

                if (!inputSuccess) {

                    apiView.ShowError(errorMessage);
                    
                    return;
                }

                bool testSucess = apiService.TestApiSettings(apiSettings, out errorMessage);

                if (!testSucess) 
                { 
                    apiView.ShowError(errorMessage);

                    return;
                }

                apiView.ShowInfo("API 설정 입력값이 정상입니다.");


            }
            catch (Exception ex)
            {
                LogService.Error("API 설정 입력 확인 실패 :" + ex.Message);
            }
            finally
            {
                apiView.SetApiOperationEnabled(true);
            }
        }
        // Service가 관리 중인 현재 API 설정을 화면에 표시
        private void ShowCurrentApiSettings()
        {
            ApiSettings apiSettings;

            if(apiService.TryGetCurrentSettings(out apiSettings))
            {
                apiView.ShowApiSettings(apiSettings);
            }
        }

    }
}
