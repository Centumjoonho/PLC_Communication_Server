using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Plc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class ServerMainService
    {
        private readonly PlcDb plcDb;
        private readonly PlcReader plcReader;
        private const int MaxPlcParallelCount = 15;
        private CancellationTokenSource cancellationTokenSource;
        private Task collectTask;
        private volatile bool collectRunning;
        public event Action<bool> CollectRunningChanged;

        public ServerMainService(PlcDb plcDb , PlcReader plcReader)
        {
            this.plcDb = plcDb;
            this.plcReader = plcReader;

        }
        public bool CollectRunning()
        {
            return collectRunning;
        }



        public async Task<IList<PlcData>> ReadAllPlcDataAsync(IList<PlcMaster> plcMasterList)
        {
            
            if(plcMasterList == null)
            {
                return new List<PlcData>();
            }

            int parallelCount = Math.Min(MaxPlcParallelCount,Math.Max(1,plcMasterList.Count));

            using(SemaphoreSlim plcSemaphore = new SemaphoreSlim(parallelCount))
            {
                List<Task<PlcData>> readTaskList = new List<Task<PlcData>>();

                foreach (PlcMaster plcMaster in plcMasterList)
                {

                    Task<PlcData> plcWork = ReadPlcWorker(plcMaster,plcSemaphore);

                    readTaskList.Add(plcWork);
                }

                PlcData[] resultArray = await Task.WhenAll(readTaskList);

                return new List<PlcData>(resultArray);

            }
           
        }

        private async Task<PlcData> ReadPlcWorker(PlcMaster plcMaster, SemaphoreSlim plcSemaphore)
        {
            await plcSemaphore.WaitAsync();

            try
            {
                return await plcReader.ReadPlcData(plcMaster);
            }
            catch (Exception ex)
            {
                return new PlcData
                {
                    PlcCode = plcMaster.PlcCode,
                    PlcName = plcMaster.PlcName,
                    PlcIp = plcMaster.PlcIp,
                    PlcPort = plcMaster.PlcPort,
                    MemoryAddress = plcMaster.MemoryAddress,
                    ReceiveData = "90000000",
                    Status = "ERROR",
                    ReceiveTime = DateTime.Now,
                    ErrorMessage = ex.Message
                };
            }
            finally
            {
                plcSemaphore.Release();
            }
        }
    }
}
