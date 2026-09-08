using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class DbSaveService
    {
        // DB 장애가 발생해도 저장 요청이 무제한으로 쌓이지 않도록 제한
        private const int MaxSaveQueueCount = 500;
     
        private readonly PlcDb plcDb;
        private readonly object stateLock = new object();

        private BlockingCollection<DbSaveItem> saveQueue;
        private Task saveWorkerTask;

        // DB Worker가 처리할 저장 항목
        private class DbSaveItem
        {
            public PlcData PlcData { get; set; }
            public DateTime WorkDate { get; set; }
            public bool SaveDaily { get; set; }
            public bool SaveHistory { get; set; }
        }

        public DbSaveService(PlcDb plcDb)
        {
            if (plcDb == null)
            {
                throw new ArgumentNullException(nameof(plcDb));
            }

            this.plcDb = plcDb;
        
        }
        // 현재 신규 DB 저장 요청을 받을 수 있는 상태인지 확인
        public bool IsRunning()
        {
            lock (stateLock)
            {
                if(saveQueue == null) { return false;}

                if(saveQueue.IsAddingCompleted) { return false; }

                if(saveWorkerTask == null) { return false; }

                if(saveWorkerTask.IsCompleted) { return false; }

                return true;
            }
        }
        // DB 저장 요청을 처리하는 Worker Task 시작
        public bool DBSaveWorkStart()
        {
            lock (stateLock)
            {
                // 기존 Worker가 실행 중이면 중복 생성하지 않음
                if (saveWorkerTask!= null && !saveWorkerTask.IsCompleted)
                {
                    if(saveQueue == null)
                    {
                        return false;
                    }
                    if(saveQueue.IsAddingCompleted)
                    {
                        return false;
                    }
                    return true;
                }
                // 이전에 사용한 큐가 남아 있다면 정리
                if(saveQueue != null)
                {
                    saveQueue.Dispose();
                }

                saveQueue = new BlockingCollection<DbSaveItem>(MaxSaveQueueCount);

                BlockingCollection<DbSaveItem> runningQueue = saveQueue;
                
                // 순차적으로 Queue에 있는 DB 저장 테스트 처리
                saveWorkerTask = Task.Run(() => ProcessQueue(runningQueue));

                return true;
            }
        }
        // DB 저장 요청을 큐에 등록
        public bool EnqueueSave(PlcData plcData, DateTime workDate, bool saveDaily, bool saveHistory)
        {
            if(plcData == null) { return false; }

            if(string.IsNullOrWhiteSpace(plcData.PlcCode)) { return false; }

            DbSaveItem saveItem = new DbSaveItem
            {
                PlcData = CopyPlcData(plcData),
                WorkDate = workDate.Date,
                SaveDaily = saveDaily,
                SaveHistory = saveHistory

            };

            int queueCount;

            lock(stateLock)
            {
                // Worker가 시작되지 않았거나 종료 처리 중이면 등록하지 않음
                if (saveQueue == null)
                {
                    return false;
                }

                if (saveQueue.IsAddingCompleted)
                {
                    return false;
                }

                bool added = saveQueue.TryAdd(saveItem);

                if (!added)
                {
                    return false;
                }

                queueCount = saveQueue.Count;
            }

            // 큐가 많이 쌓이면 로그로 확인
            if (queueCount >= 50 && queueCount % 50 == 0)
            {
                LogService.Log(
                    "[DB][SAVE_QUEUE][BACKLOG] " +
                    "대기: " + queueCount + "건");
            }

            return true;

        }
        // 신규 입력을 차단하고 큐에 남은 저장 작업 완료 대기
        public async Task<bool> DBSaveWorkStopAsync()
        {
            BlockingCollection<DbSaveItem> runningQueue;
            Task runningWorkerTask;

            lock (stateLock)
            {
                if (saveQueue == null)
                {
                    return true;
                }

                runningQueue = saveQueue;
                runningWorkerTask = saveWorkerTask;

                if (!runningQueue.IsAddingCompleted)
                {
                    runningQueue.CompleteAdding();
                }
            }

            try
            {
                if (runningWorkerTask != null)
                {
                    await runningWorkerTask;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("[DB][SAVE_WORKER][STOP][FAIL] " + ex.Message);

                return false;
            }
            finally
            {
                bool disposeQueue = false;

                lock (stateLock)
                {
                    if (ReferenceEquals(saveQueue, runningQueue))
                    {
                        saveQueue = null;
                        saveWorkerTask = null;
                        disposeQueue = true;
                    }
                }

                if (disposeQueue)
                {
                    runningQueue.Dispose();
                }
            }
        }
        // 저장 큐 처리
        private void ProcessQueue(BlockingCollection<DbSaveItem> runningQueue)
        {
            foreach( DbSaveItem saveItem in runningQueue.GetConsumingEnumerable())
            {
                string plcCode = saveItem.PlcData.PlcCode;
               
                try
                {
                    // 최신 상태는 모든 수집 결과에 대해서 저장
                    plcDb.SaveLatest(saveItem.PlcData);
                }
                catch (Exception ex)
                {
                    LogService.Error(
                        "[DB][" + plcCode + "][LATEST_SAVE][FAIL] " +
                        ex.Message);
                }

                // PLC 시간대별 가동초 저장
                try
                {
                    if (saveItem.SaveDaily)
                    {
                        plcDb.SaveDaily(saveItem.PlcData, saveItem.WorkDate);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(
                        "[DB][" + plcCode + "][DAILY_SAVE][FAIL] " +
                        ex.Message);
                }

                // PLC 통신 장애 이력 저장
                try
                {
                    if (saveItem.SaveHistory)
                    {
                        plcDb.SaveHistory(saveItem.PlcData);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(
                        "[DB][" + plcCode + "][HISTORY_SAVE][FAIL] " +
                        ex.Message);
                } 

            }
           
        }

        // 수집 객체와 DB 저장 객체가 같은 배열을 공유하지 않도록 깊은 복사
        private PlcData CopyPlcData(PlcData plcData)
        {
            return new PlcData
            {
                PlcCode = plcData.PlcCode,
                PlcName = plcData.PlcName,
                PlcIp = plcData.PlcIp,
                PlcPort = plcData.PlcPort,
                MemoryAddress = plcData.MemoryAddress,
                ReceiveData = plcData.ReceiveData,
                Status = plcData.Status,
                TotalSeconds = plcData.TotalSeconds,
                Rate = plcData.Rate,
                ReceiveTime = plcData.ReceiveTime,
                HourSeconds = plcData.HourSeconds == null ? new int[24] : (int[])plcData.HourSeconds.Clone(),
                ErrorMessage = plcData.ErrorMessage
            };
        }
    }

   
}
