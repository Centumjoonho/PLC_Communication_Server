using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Store;
using System;
using System.Collections.Generic;


namespace RO_Server_Rebuild_2.Api
{
    public class ApiHandler
    {
        private readonly PlcDb plcDb;
        private readonly ApiSettingsService apiSettingsService;
        private readonly PlcDataStore plcDataStore;
        private readonly Func<bool> isCollectRunning;

        public ApiHandler(PlcDb plcDb, ApiSettingsService apiSettingsService, PlcDataStore plcDataStore, Func<bool> isCollectRunning)
        {
            if (plcDb == null)
            {
                throw new ArgumentNullException(nameof(plcDb));
            }
            if (apiSettingsService == null)
            {
                throw new ArgumentNullException(nameof(apiSettingsService));
            }
            if (plcDataStore == null)
            {
                throw new ArgumentNullException(nameof(plcDataStore));
            }
            if (isCollectRunning == null)
            {
                throw new ArgumentNullException(nameof(isCollectRunning));
            }
            this.plcDb = plcDb;
            this.apiSettingsService = apiSettingsService;
            this.plcDataStore = plcDataStore;
            this.isCollectRunning = isCollectRunning;
        }

        // 요청 종류에 맞는 API 응답 생성
        public ApiMessage CreateResponse(ApiMessage request , string clientIp)
        {
            if(request == null)
            {
                return CreateErrorResponse("API 요청 정보가 없습니다");
            }
            
            if(!IsValidApiKey(request.ApiKey))
            {
                LogService.Error(
                    "[API][AUTH][BLOCK] API Key 불일치 / " +
                    "ClientIp: " + clientIp);

                return CreateErrorResponse("API Key가 유효하지 않습니다");
            }

            switch(request.Type)
            {
                case ApiMessageType.Ping:
                    return CreatePingResponse();

                case ApiMessageType.PlcDataList:
                    return CreatePlcDataListResponse();

                default:
                    return CreateErrorResponse("지원하지 않는 API 요청입니다.");
            }
        }

        // 서버와 PLC 수집 상태 확인 응답
        private ApiMessage CreatePingResponse()
        {
            return new ApiMessage
            {
                Type = ApiMessageType.PingResponse,
                Success = true,
                Message = "ACK",
                ServerRepeatRunning = isCollectRunning(),
                PlcDataList = new List<PlcData>()
            };
        }

        //PLC 데이터 목록 응답
        private ApiMessage CreatePlcDataListResponse()
        {
            bool collectRunning = isCollectRunning();
            List<PlcData> plcDataList;

            if (collectRunning)
            {
                // 수집중에는 메모리에 저장된 최신 수집 결과 사용 
                plcDataList = plcDataStore.GetCollectList();
            }
            else
            {
                // 수집 정지 중에는 DB에 저장된 최신 결과 사용 
                plcDataList = plcDb.ReadLatestList();
            }

            return new ApiMessage
            {
                Type = ApiMessageType.PlcDataListResponse,
                Success = true,
                Message = "OK",
                ServerRepeatRunning = collectRunning,
                PlcDataList = plcDataList ?? new List<PlcData>()
            };
        }
        // 오류 응답 생성
        public ApiMessage CreateErrorResponse(string errorMessage)
        {
            return new ApiMessage
            {
                Type = ApiMessageType.Error,
                Success = false,
                Message = errorMessage,
                ServerRepeatRunning = isCollectRunning(),
                PlcDataList = new List<PlcData>()
            };
        }

        // 관리자 설정 화면에서 적용한 현재API Key 확인 
        private bool IsValidApiKey(string requestApiKey)
        {
            ApiSettings apiSettings;

            if(!apiSettingsService.TryGetCurrentSettings(out apiSettings))
            {
                return false;
            }

            if(apiSettings == null)
            {
                return false;
            }

            return string.Equals(requestApiKey, apiSettings.ApiKey, StringComparison.Ordinal);
        }
    }
}
