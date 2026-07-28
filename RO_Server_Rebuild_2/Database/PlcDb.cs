using MySqlConnector;
using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Database
{
    public class PlcDb
    {
        private readonly DbSettingsService dbSettingsService;

        public PlcDb(DbSettingsService dbSettingsService)
        {
            if(dbSettingsService == null)
            {
                throw new ArgumentNullException(nameof(dbSettingsService));
            }

            this.dbSettingsService = dbSettingsService;
        }
        // db 셋팅 기반 ConnectionString 생성 여부 확인 및 연결 
        private MySqlConnection CreateConnection()
        {
            string connectionString;
            string errorMessage;
            
            bool settingExists = dbSettingsService.TryGetCurrentConnectionString(out connectionString, out errorMessage);

            if (!settingExists)
            {
                throw new InvalidOperationException(errorMessage);
            }

            return new MySqlConnection(connectionString);
        }

        //------------------------------------plcMasterDb Sql --------------------------------------

        private const string sqlReadAllPlcs =
          @"SELECT plc_code, plc_name, plc_ip, plc_port, memory_address, use_yn
                FROM ro_plc_master
                ORDER BY plc_code";

        public List<PlcMaster> ReadAllPlcs()
        {
            List<PlcMaster> plcMasterList = new List<PlcMaster>();

            using (MySqlConnection conn = CreateConnection()) {

                using (MySqlCommand cmd = new MySqlCommand(sqlReadAllPlcs, conn))
                {
                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) {
                        
                            bool useYn = string.Equals(reader.IsDBNull(5) ? "Y" : reader.GetString(5),"Y",StringComparison.OrdinalIgnoreCase);

                            PlcMaster plcMaster = new PlcMaster
                            {
                                PlcCode = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                PlcName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                PlcIp = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                PlcPort = reader.IsDBNull(3) ? 502 : reader.GetInt32(3),
                                MemoryAddress = reader.IsDBNull(4) ? "30" : reader.GetString(4),
                                UseYn = useYn
                            };

                            plcMasterList.Add(plcMaster);
                        }
                    }

                }
                
                return plcMasterList;

            }
        }
        public List<PlcMaster> ReadUsePlcs()
        {
            //return ReadAllPlcs().Where(plc =>plc.UseYn).ToList();

            List<PlcMaster> allPlcs = ReadAllPlcs();
            List<PlcMaster> usePlcs = new List<PlcMaster>();

            foreach (var plc in allPlcs)
            {
                if (plc.UseYn)
                {
                    usePlcs.Add(plc);
                }
            }

            return usePlcs;
        }


        private const string sqlSavePlcMaster =
            @"INSERT INTO ro_plc_master
                (plc_code, plc_name, plc_ip, plc_port, memory_address, use_yn)
              VALUES
                (@plc_code, @plc_name, @plc_ip, @plc_port, @memory_address, @use_yn)
              ON DUPLICATE KEY UPDATE
                plc_name = VALUES(plc_name),
                plc_ip = VALUES(plc_ip),
                plc_port = VALUES(plc_port),
                memory_address = VALUES(memory_address),
                use_yn = VALUES(use_yn)";

        public void SavePlcMaster(PlcMaster plcMaster)
        {
            if(plcMaster == null)
            {
                throw new ArgumentNullException(nameof(plcMaster));
            }

            using(MySqlConnection conn = CreateConnection())
            {

                using(MySqlCommand cmd = new MySqlCommand(sqlSavePlcMaster, conn))
                {
                    cmd.Parameters.AddWithValue("@plc_code", plcMaster.PlcCode);
                    cmd.Parameters.AddWithValue("@plc_name", plcMaster.PlcName);
                    cmd.Parameters.AddWithValue("@plc_ip", plcMaster.PlcIp);
                    cmd.Parameters.AddWithValue("@plc_port", plcMaster.PlcPort);
                    cmd.Parameters.AddWithValue("@memory_address", plcMaster.MemoryAddress);
                    cmd.Parameters.AddWithValue("@use_yn", plcMaster.UseYn ? "Y" : "N");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private const string sqlDeletePlcMaster =
            @"DELETE FROM ro_plc_master
                WHERE plc_code = @plc_code";


        public void DeletePlcMaster(string plcCode)
        {
            if (string.IsNullOrWhiteSpace(plcCode))
            {
                throw new ArgumentException("삭제할 PLC 코드를 입력하세요 ", nameof(plcCode));
            }
            using(MySqlConnection conn = CreateConnection())
            {
                using(MySqlCommand cmd =new MySqlCommand(sqlDeletePlcMaster, conn))
                {
                    cmd.Parameters.AddWithValue("@plc_code", plcCode);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //------------------------------------plcDataDb Sql --------------------------------------
        /// <summary>
        /// PLC 최신 상태 저장
        /// </summary>
        private const string SqlSaveLatest = @"
            INSERT INTO ro_plc_latest
            (plc_code, plc_name, receive_data, status_text, total_seconds, rate, receive_time, client_ip)
            VALUES
            (@plc_code, @plc_name, @receive_data, @status_text, @total_seconds, @rate, @receive_time, @client_ip)
            ON DUPLICATE KEY UPDATE
              plc_name=VALUES(plc_name),
              receive_data=VALUES(receive_data),
              status_text=VALUES(status_text),
              total_seconds=VALUES(total_seconds),
              rate=VALUES(rate),
              receive_time=VALUES(receive_time),
              client_ip=VALUES(client_ip);";


        private const string SqlSaveDaily = @"
            INSERT INTO ro_operation_daily
            (plc_code, work_date, total_seconds,
             hat_00, hat_01, hat_02, hat_03, hat_04, hat_05,
             hat_06, hat_07, hat_08, hat_09, hat_10, hat_11,
             hat_12, hat_13, hat_14, hat_15, hat_16, hat_17,
             hat_18, hat_19, hat_20, hat_21, hat_22, hat_23,
             receive_time)
            VALUES
            (@plc_code, @work_date, @total_seconds,
             @hat_00, @hat_01, @hat_02, @hat_03, @hat_04, @hat_05,
             @hat_06, @hat_07, @hat_08, @hat_09, @hat_10, @hat_11,
             @hat_12, @hat_13, @hat_14, @hat_15, @hat_16, @hat_17,
             @hat_18, @hat_19, @hat_20, @hat_21, @hat_22, @hat_23,
             @receive_time)
            ON DUPLICATE KEY UPDATE
              total_seconds=VALUES(total_seconds),
              hat_00=VALUES(hat_00),
              hat_01=VALUES(hat_01),
              hat_02=VALUES(hat_02),
              hat_03=VALUES(hat_03),
              hat_04=VALUES(hat_04),
              hat_05=VALUES(hat_05),
              hat_06=VALUES(hat_06),
              hat_07=VALUES(hat_07),
              hat_08=VALUES(hat_08),
              hat_09=VALUES(hat_09),
              hat_10=VALUES(hat_10),
              hat_11=VALUES(hat_11),
              hat_12=VALUES(hat_12),
              hat_13=VALUES(hat_13),
              hat_14=VALUES(hat_14),
              hat_15=VALUES(hat_15),
              hat_16=VALUES(hat_16),
              hat_17=VALUES(hat_17),
              hat_18=VALUES(hat_18),
              hat_19=VALUES(hat_19),
              hat_20=VALUES(hat_20),
              hat_21=VALUES(hat_21),
              hat_22=VALUES(hat_22),
              hat_23=VALUES(hat_23),
              receive_time=VALUES(receive_time);";


        private const string SqlSaveHistory = @"
            INSERT IGNORE INTO ro_plc_history
            (message_id, plc_code, receive_data, total_seconds, rate, raw_frame, receive_time)
            VALUES
            (@message_id, @plc_code, @receive_data, @total_seconds, @rate, @raw_frame, @receive_time);";


        private const string SqlReadLatestList = @"
                SELECT m.plc_code, m.plc_name, m.plc_ip, m.plc_port, m.memory_address,
                       l.receive_data, l.status_text, l.total_seconds, l.rate, l.receive_time
                FROM ro_plc_master m
                LEFT JOIN ro_plc_latest l ON l.plc_code = m.plc_code
                WHERE IFNULL(m.use_yn, 'Y') = 'Y'
                ORDER BY m.plc_code";

        private const string SqlReadTodaySeconds = @"
            SELECT hat_00, hat_01, hat_02, hat_03, hat_04, hat_05,
                   hat_06, hat_07, hat_08, hat_09, hat_10, hat_11,
                   hat_12, hat_13, hat_14, hat_15, hat_16, hat_17,
                   hat_18, hat_19, hat_20, hat_21, hat_22, hat_23
            FROM ro_operation_daily
            WHERE plc_code=@plc_code
              AND work_date=@work_date";

        // PLC 최신 상태 저장
        public void SaveLatest(PlcData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = new MySqlCommand(SqlSaveLatest, conn))
            {
                cmd.Parameters.AddWithValue("@plc_code", data.PlcCode);
                cmd.Parameters.AddWithValue("@plc_name", data.PlcName);
                cmd.Parameters.AddWithValue("@receive_data", data.ReceiveData);
                cmd.Parameters.AddWithValue("@status_text", data.Status);
                cmd.Parameters.AddWithValue("@total_seconds", data.TotalSeconds);
                cmd.Parameters.AddWithValue("@rate", data.Rate);
                cmd.Parameters.AddWithValue("@receive_time", data.ReceiveTime);
                cmd.Parameters.AddWithValue("@client_ip", "SERVER");

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // PLC 일별·시간대별 가동시간 저장
        public void SaveDaily(PlcData data, DateTime workDate)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = new MySqlCommand(SqlSaveDaily, conn))
            {
                cmd.Parameters.AddWithValue("@plc_code", data.PlcCode);
                cmd.Parameters.AddWithValue("@work_date", workDate.Date);
                cmd.Parameters.AddWithValue("@total_seconds", data.TotalSeconds);
                cmd.Parameters.AddWithValue("@receive_time", data.ReceiveTime);

                for (int hour = 0; hour < 24; hour++)
                {
                    int second = 0;

                    if (data.HourSeconds != null && hour < data.HourSeconds.Length)
                    {
                        second = data.HourSeconds[hour];
                    }

                    cmd.Parameters.AddWithValue("@hat_" + hour.ToString("00"), second);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // PLC 장애 이력 저장
        public void SaveHistory(PlcData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = new MySqlCommand(SqlSaveHistory, conn))
            {
                cmd.Parameters.AddWithValue("@message_id", Guid.NewGuid().ToString("N"));
                cmd.Parameters.AddWithValue("@plc_code", data.PlcCode);
                cmd.Parameters.AddWithValue("@receive_data", data.ReceiveData);
                cmd.Parameters.AddWithValue("@total_seconds", data.TotalSeconds);
                cmd.Parameters.AddWithValue("@rate", data.Rate);
                cmd.Parameters.AddWithValue("@raw_frame", data.ReceiveData ?? string.Empty);
                cmd.Parameters.AddWithValue("@receive_time", data.ReceiveTime);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // API 응답용 PLC 최신 상태 목록 조회
        public List<PlcData> ReadLatestList()
        {
            List<PlcData> plcDataList = new List<PlcData>();

            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = new MySqlCommand(SqlReadLatestList, conn))
            {
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlcData data = new PlcData
                        {
                            PlcCode = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                            PlcName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            PlcIp = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            PlcPort = reader.IsDBNull(3) ? 502 : reader.GetInt32(3),
                            MemoryAddress = reader.IsDBNull(4) ? "30" : reader.GetString(4),
                            ReceiveData = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            Status = reader.IsDBNull(6) ? "WAIT" : reader.GetString(6),
                            TotalSeconds = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            Rate = reader.IsDBNull(8) ? 0 : reader.GetDouble(8),
                            ReceiveTime = reader.IsDBNull(9) ? DateTime.MinValue : reader.GetDateTime(9)
                        };

                        DateTime workDate = GetWorkDate(data.ReceiveTime);

                        data.HourSeconds = ReadTodaySeconds(data.PlcCode, workDate);

                        plcDataList.Add(data);
                    }
                }
            }

            return plcDataList;
        }

        // PLC의 영업일 기준 시간대별 가동초 조회
        public int[] ReadTodaySeconds(string plcCode, DateTime workDate)
        {
            int[] hourSeconds = new int[24];

            using (MySqlConnection conn = CreateConnection())
            using (MySqlCommand cmd = new MySqlCommand(SqlReadTodaySeconds, conn))
            {
                cmd.Parameters.AddWithValue("@plc_code", plcCode);
                cmd.Parameters.AddWithValue("@work_date", workDate.Date);

                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        for (int hour = 0; hour < 24; hour++)
                        {
                            hourSeconds[hour] = reader.IsDBNull(hour) ? 0 : reader.GetInt32(hour);
                        }
                    }
                }
            }

            return hourSeconds;
        }

        // 오전 8시를 기준으로 영업일 계산
        private DateTime GetWorkDate(DateTime time)
        {
            if (time == DateTime.MinValue)
            {
                return DateTime.Today;
            }

            if (time.Hour < 8)
            {
                return time.Date.AddDays(-1);
            }

            return time.Date;
        }
    }
}
