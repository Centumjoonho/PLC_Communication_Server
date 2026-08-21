using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class ApiSettingsService
    {
        private ApiSettings currentApisettings = new ApiSettings
        {
            Port = 3410,
            ApiKey = "HYUNDAI_RB_RO_2026"
        };
        // 유효성 검사 메서드
        private bool ValidateApiSetting(ApiSettings apiSettings, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (apiSettings == null)
            {
                errorMessage = "API 설정 정보가 없습니다.";
                return false;
            }

            if (apiSettings.Port <= 0 || apiSettings.Port > 65535)
            {
                errorMessage = "API Port는 1부터 65535 사이로 입력하세요.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(apiSettings.ApiKey))
            {
                errorMessage = "API Key를 입력하세요.";
                return false;
            }

            return true;
        }
        // API 설정 테스트 메서드
        public bool TestApiSettings(ApiSettings apiSettings, out string errorMessage)
        {
            return ValidateApiSetting(apiSettings, out errorMessage);
        }
        // API 설정 적용 메서드
        public bool ApplyApiSettings(ApiSettings apiSettings , out string errorMessage)
        {
            if (!ValidateApiSetting(apiSettings, out errorMessage)){
                
                return false;
            }
            // 입력된 값 셋팅
            currentApisettings = new ApiSettings
            {
                Port = apiSettings.Port,
                ApiKey = apiSettings.ApiKey,
            };

            return true;
        }
        // 현재 API 설정 가져오기 메서드
        public bool TryGetCurrentSettings(out ApiSettings apiSettings)
        {
            apiSettings = new ApiSettings
            {
                Port = currentApisettings.Port,
                ApiKey = currentApisettings.ApiKey
            };

            return true;
        }
       
    }
}
