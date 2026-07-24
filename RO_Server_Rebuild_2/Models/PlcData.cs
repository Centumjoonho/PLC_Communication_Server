using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Models
{
    public class PlcData
    {
        public string PlcCode { get; set; }
        public string PlcName { get; set; }
        public string PlcIp { get; set; }
        public int PlcPort { get; set; }
        public string MemoryAddress { get; set; }
        public string ReceiveData { get; set; }
        public string Status { get; set; }
        public int TotalSeconds { get; set; }
        public double Rate { get; set; }
        public DateTime ReceiveTime { get; set; }
        public int[] HourSeconds { get; set; } = new int[24];
        public string ErrorMessage { get; set; }

    }
}
