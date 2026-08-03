using RO_Server_Rebuild_2.Models;
using System.Collections.Generic;

namespace RO_Server_Rebuild_2.Store
{
    public class PlcDataStore
    {
        private readonly object dataLock = new object();

        private List<PlcData> collectList = new List<PlcData>();

        public void SetCollectList(IList<PlcData> plcDataList)
        {
            List<PlcData> newList = new List<PlcData>();

            if (plcDataList != null)
            {
                foreach (PlcData data in plcDataList)
                {
                    if (data != null)
                    {
                        newList.Add(CopyPlcData(data));
                    }
                }
            }

            lock (dataLock)
            {
                collectList = newList;
            }
        }

        public List<PlcData> GetCollectList()
        {
            List<PlcData> result = new List<PlcData>();

            lock (dataLock)
            {
                foreach (PlcData data in collectList)
                {
                    result.Add(CopyPlcData(data));
                }
            }

            return result;
        }

        private PlcData CopyPlcData(PlcData source)
        {
            return new PlcData
            {
                PlcCode = source.PlcCode,
                PlcName = source.PlcName,
                PlcIp = source.PlcIp,
                PlcPort = source.PlcPort,
                MemoryAddress = source.MemoryAddress,
                ReceiveData = source.ReceiveData,
                Status = source.Status,
                TotalSeconds = source.TotalSeconds,
                Rate = source.Rate,
                ReceiveTime = source.ReceiveTime,
                HourSeconds = source.HourSeconds == null ? new int[24] : (int[])source.HourSeconds.Clone(),
                ErrorMessage = source.ErrorMessage
            };
        }
    }
}