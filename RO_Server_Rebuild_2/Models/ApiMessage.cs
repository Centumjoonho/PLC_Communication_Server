
using System.Collections.Generic;


namespace RO_Server_Rebuild_2.Models
{
    public class ApiMessage
    {
        public string Type { get; set; }
        public string ApiKey { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool ServerRepeatRunning { get; set; }
        public List<PlcData> PlcDataList { get; set; }

    }
}
