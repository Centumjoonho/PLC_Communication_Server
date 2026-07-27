using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Store
{
    public class PlcDataStore
    {
        private readonly object dataLock = new object();

        private List<PlcData> collectList = new List<PlcData>();

        public void SetCollectList(IList<PlcData> plcDataList)
        {
            lock (dataLock)
            {
                collectList = plcDataList == null
                    ? new List<PlcData>()
                    : new List<PlcData>(plcDataList);
            }
        }

        public List<PlcData> GetCollectList()
        {
            lock (dataLock)
            {
                return new List<PlcData>(collectList);
            }
        }
    }
}
