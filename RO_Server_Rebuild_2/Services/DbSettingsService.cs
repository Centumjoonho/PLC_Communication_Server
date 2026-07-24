using MySqlConnector;
using RO_Server_Rebuild_2.Database;
using System;


namespace RO_Server_Rebuild_2.Services
{
    public class DbSettingsService
    {
        private DbSettings currentSetting;

        private string CreateConnectionString(DbSettings settings)
        {
            MySqlConnectionStringBuilder conn = new MySqlConnectionStringBuilder
            {
                Server = settings.Server,
                Port = (uint)settings.Port,
                Database = settings.DatabaseName,
                UserID = settings.UserId,
                Password = settings.Password,
                CharacterSet = "utf8mb4",
                ConnectionTimeout = 3,
                DefaultCommandTimeout = 10
            };

            return conn.ConnectionString;
        }
        // 외부에서 현재 DB 접속 정보에 접근하기 위한 메서드
        public bool TryGetCurrentConnectionString( out string connectionString , out string errorMessage)
        {
            connectionString = string.Empty;
            errorMessage = string.Empty;

            DbSettings dbSettings;

            // 현재 셋팅된 DB 정보가 있는지 확인
            if (!TryGetCurrentDbSettings(out dbSettings)) {
                
                errorMessage = " : DB 설정을 먼저 적용하세요";
                
                return false;
            }
            // 해당 DB 값이 적절한지 확인
            if (!IsValidDbSettings(dbSettings,out errorMessage))
            {
                return false;
            }

            connectionString = CreateConnectionString(dbSettings);

            return true;

        }
        public bool TestConnection(DbSettings settings, out string errorMessage)
        {
            
            errorMessage =string.Empty;

            if (!IsValidDbSettings(settings, out errorMessage)) { 
            
            return false;

            }

            string connectionString = CreateConnectionString(settings);

            try
            {
                using(MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                }
                return true;

            }
            catch (Exception ex)
            {
                errorMessage = "DB 연결 실패 :" + ex.Message;
                
                return false;
            }

        }
        public bool ApplyDbSetting(DbSettings settings ,out string errorMessage)
        {
            errorMessage =string.Empty;

            if (!TestConnection(settings, out errorMessage))
            {
                return false;
            }

            currentSetting = new DbSettings
            {
                Server = settings.Server,
                Port = settings.Port,
                DatabaseName = settings.DatabaseName,
                UserId = settings.UserId,
                Password = settings.Password
            };

            return true;
        }
        public bool TryGetCurrentDbSettings(out DbSettings settings)
        {
            if (currentSetting == null) { settings = null; return false; }

            settings = new DbSettings
            {
                Server = currentSetting.Server,
                Port = currentSetting.Port,
                DatabaseName = currentSetting.DatabaseName,
                UserId = currentSetting.UserId,
                Password = currentSetting.Password
            };
            return true;
        }

        private bool IsValidDbSettings(DbSettings settings, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (settings == null)
            {
                errorMessage ="DB 설정 정보가 없습니다.";

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                settings.Server))
            {
                errorMessage ="DB Server를 입력하세요.";

                return false;
            }

            if (settings.Port <= 0 || settings.Port > 65535)
            {
                errorMessage = "DB Port는 1부터 65535 사이로 입력하세요.";

                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.DatabaseName))
            {
                errorMessage ="DB Name을 입력하세요.";

                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.UserId))
            {
                errorMessage ="DB User ID를 입력하세요.";

                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.Password))
            {
                errorMessage ="DB Password를 입력하세요.";

                return false;
            }

            return true;
        }
    }
}
