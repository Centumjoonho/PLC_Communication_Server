using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public class RunRateService
    {
        private const int HourCount = 24;

        private const double OneDaySeconds = 24 * 60 * 60;

        private readonly object stateLock = new object();

        //PLc 별 현재 영업일
        private readonly Dictionary<string, DateTime> workDates = new Dictionary<string, DateTime>();
        
        //PLc 별 시간대별 가동초
        private readonly Dictionary<string, int[]> hourRunSecondsMap = new Dictionary<string, int[]>();
        
        // PLC별 마지막으로 확인된 현재 상태
        private readonly Dictionary<string, string> currentStatuses = new Dictionary<string, string>();

        // 오전 8시를 기준으로 영업일 계산
        public DateTime GetWorkDate(DateTime dateTime)
        {
            if(dateTime.Hour < 8)
            {
                return dateTime.Date.AddDays(-1);
            }

            return dateTime.Date;
        }

        // DB에 저장된 기존 가동초를 사용하여 최초 계산 상태 생성
        public void InitPlcRunTime(string plcCode, DateTime now, int[] savedHourSeconds)
        {
            if (string.IsNullOrWhiteSpace(plcCode))
            {
                throw new ArgumentException("PLC Code가 없습니다.", nameof(plcCode));
            }

            lock (stateLock)
            {

                // 가동초 셋팅되어 있는 PLC 는 DB 호출 제외
                if (hourRunSecondsMap.ContainsKey(plcCode))
                {
                    return;
                }

                workDates[plcCode] = GetWorkDate(now);
                hourRunSecondsMap[plcCode] = CopyHourSeconds(savedHourSeconds);
                // 첫 통신 결과가 들어오기 전에는 가동초 증가 금지
                currentStatuses[plcCode] = "WAIT";
            }
        }

        // PLC 통신을 정지했다가 다시 시작할 때 계산 상태 재설정
        public void ResetPlcRunTime(string plcCode, DateTime now, int[] savedHourSeconds)
        {
            if (string.IsNullOrWhiteSpace(plcCode))
            {
                throw new ArgumentException("PLC Code가 없습니다.", nameof(plcCode));
            }

            lock (stateLock)
            {
                workDates[plcCode] = GetWorkDate(now);
                hourRunSecondsMap[plcCode] = CopyHourSeconds(savedHourSeconds);
                // 재시작 후 첫 응답 전까지 가동초 증가 금지
                currentStatuses[plcCode] = "WAIT";
            }
        }

        // PLC 통신 결과가 들어오면 현재 상태만 갱신
        public bool UpdatePlcStatus(PlcData plcData)
        {
            if (plcData == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(plcData.PlcCode))
            {
                return false;
            }

            lock (stateLock)
            {
                // 초기화되지 않았다면 상태를 변경하지 않음
                if (!hourRunSecondsMap.ContainsKey(plcData.PlcCode))
                {
                    return false;
                }

                currentStatuses[plcData.PlcCode] = plcData.Status ?? string.Empty;

                return true;
            }
        }
       

        // 실제 시계 기준으로 1초마다 호출
        public void CountOneSecond(DateTime now)
        {
            lock (stateLock)
            {
                // 반복 중 Dictionary 값이 바뀌어도 안전하도록 Key 목록 복사
                List<string> plcCodeList = new List<string>(hourRunSecondsMap.Keys);

                foreach (string plcCode in plcCodeList)
                {
                    DateTime currentWorkDate = GetWorkDate(now);

                    // 오전 8시가 지나 새로운 영업일이 시작된 경우
                    if (workDates[plcCode] != currentWorkDate)
                    {
                        workDates[plcCode] = currentWorkDate;
                        hourRunSecondsMap[plcCode] = new int[HourCount];
                    }

                    string currentStatus;

                    // 아직 PLC 상태를 한 번도 받지 못한 경우
                    if (!currentStatuses.TryGetValue(plcCode, out currentStatus))
                    {
                        continue;
                    }

                    // 현재 상태가 RUN일 때만 현재 시간대에 1초 증가
                    if (string.Equals(currentStatus, "RUN", StringComparison.OrdinalIgnoreCase))
                    {
                        hourRunSecondsMap[plcCode][now.Hour] += 1;
                    }
                }
            }
        }

        // 계산된 가동초와 가동률을 PlcData에 반영
        public void ApplyRunRate(PlcData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrWhiteSpace(data.PlcCode))
            {
                throw new ArgumentException("PLC Code가 없습니다.", nameof(data));
            }

            lock (stateLock)
            {
                int[] hourSeconds;

                // 아직 초기화되지 않은 PLC는 0으로 표시
                if (!hourRunSecondsMap.TryGetValue(data.PlcCode, out hourSeconds))
                {
                    data.HourSeconds = new int[HourCount];
                    data.TotalSeconds = 0;
                    data.Rate = 0;

                    return;
                }

                SetResult(data, hourSeconds);
            }
        }

        // PLC 계산 상태가 초기화됐는지 확인
        public bool HasPlcRunTime(string plcCode)
        {
            if (string.IsNullOrWhiteSpace(plcCode))
            {
                return false;
            }

            lock (stateLock)
            {
                return hourRunSecondsMap.ContainsKey(plcCode);
            }
        }

        // 계산 결과를 PlcData에 복사
        private void SetResult(PlcData data, int[] hourSeconds)
        {
            data.HourSeconds = CopyHourSeconds(hourSeconds);
            data.TotalSeconds = SumHourSeconds(data.HourSeconds);
            data.Rate = CalculateTotalRate(data.TotalSeconds);
        }

        // 시간대별 가동초 합계 계산
        private int SumHourSeconds(int[] hourSeconds)
        {
            int totalSeconds = 0;

            for (int hour = 0; hour < HourCount; hour++)
            {
                totalSeconds += hourSeconds[hour];
            }

            return totalSeconds;
        }

        // 하루 24시간을 기준으로 전체 가동률 계산
        private double CalculateTotalRate(int totalSeconds)
        {
            double rate = totalSeconds / OneDaySeconds * 100.0;

            if (rate > 100.0)
            {
                rate = 100.0;
            }

            if (rate < 0)
            {
                rate = 0;
            }

            return Math.Round(rate, 3);
        }

        // 외부 배열과 내부 배열이 같은 객체를 공유하지 않도록 복사
        private int[] CopyHourSeconds(int[] source)
        {
            int[] result = new int[HourCount];

            if (source == null)
            {
                return result;
            }

            int copyLength = Math.Min(source.Length, HourCount);

            Array.Copy(source, result, copyLength);

            return result;
        }
        public void ResetAll()
        {
            lock (stateLock)
            {
                workDates.Clear();
                hourRunSecondsMap.Clear();
                currentStatuses.Clear();
            }
        }


    }
}
