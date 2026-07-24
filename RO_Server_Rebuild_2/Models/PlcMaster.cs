using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Models
{
    public class PlcMaster
    {
        public string PlcCode { get; set; }
        public string PlcName { get; set; }
        public string PlcIp { get; set; }
        public int PlcPort { get; set; }
        public string MemoryAddress { get; set; }
        public bool UseYn {  get; set; }

    }
}
