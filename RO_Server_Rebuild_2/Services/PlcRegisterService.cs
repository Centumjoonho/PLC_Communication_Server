using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class PlcRegisterService
    {
        private readonly PlcDb plcDb;

        public PlcRegisterService(PlcDb plcDb)
        {
            if(plcDb == null)
            {
                throw new ArgumentNullException(nameof(plcDb));
            }

            this.plcDb = plcDb;
        }

        public IList<PlcMaster> ReadPlcMasterList()
        {
            return plcDb.ReadAllPlcs();
        }

        public bool SavePlcMaster(PlcMaster plcMaster , out string errorMessage)
        {
            errorMessage = string.Empty;

            if(!IsValidPlcMaster(plcMaster,out errorMessage)){
                
                return false;
            }

            try
            {
                plcDb.SavePlcMaster(plcMaster);
                
                return true;
            }
            catch (Exception ex)
            {

                errorMessage = "PLC 정보 저장 실패 : " + ex.Message;
               
                return false;
            }
        }

        public bool DeletePlcMaster(string plcCode , out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(plcCode))
            {
                errorMessage = "삭제할 PLC를 선택하세요.";
                
                return false;
            }
            try
            {
                plcDb.DeletePlcMaster(plcCode);

                return true;
            }
            catch (Exception ex)
            {

                errorMessage = "PLC 정보 삭제 실패 : " + ex.Message;
                
                return false;
            }
        }

        private bool IsValidPlcMaster(PlcMaster plcMaster , out string errorMessage) { 
         
            errorMessage= string.Empty;
            
            if(plcMaster == null)
            {
                errorMessage = "PLC 정보가 없습니다.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(plcMaster.PlcCode))
            {
                errorMessage = "PLC Code를 입력하세요.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(plcMaster.PlcName))
            {
                errorMessage = "PLC Name을 입력하세요.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(plcMaster.PlcIp))
            {
                errorMessage = "PLC IP를 입력하세요.";
                return false;
            }

            IPAddress ipAddress;

            if (!IPAddress.TryParse(plcMaster.PlcIp, out ipAddress))
            {
                errorMessage = "PLC IP 형식이 올바르지 않습니다.";
                return false;
            }

            if (plcMaster.PlcPort <= 0 || plcMaster.PlcPort > 65535)
            {
                errorMessage = "PLC Port는 1부터 65535 사이로 입력하세요.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(plcMaster.MemoryAddress))
            {
                errorMessage = "Memory Address를 입력하세요.";
                return false;
            }

            return true;

        }


    }
}
