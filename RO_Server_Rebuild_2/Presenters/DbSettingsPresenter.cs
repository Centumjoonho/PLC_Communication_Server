using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Properties;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2.Presenters
{
    public class DbSettingsPresenter
    {
        private readonly IDbSettingsView dbView;
        private readonly DbSettingsService dbService;

        public DbSettingsPresenter(IDbSettingsView dbView, DbSettingsService dbService)
        {
            this.dbView = dbView;
            this.dbService = dbService;

            this.dbView.DbTestRequested += OnDbTestRequested;
            this.dbView.DbApplyRequested += OnDbApplyRequested;
            // 프로그램 실행시 최초 적용
            ApplyInitialDbSetting();
        }
        private void ApplyInitialDbSetting()
        {
            DbSettings settings;
            string errorMessage;

            bool inputSuccess = dbView.TryGetDbSetting(out settings, out errorMessage);

            if (!inputSuccess)
            {
                LogService.Error("[DB_SETTING][INITIALIZE][VALIDATE_FAIL] " + errorMessage);
                return;
            }

            bool applySuccess = dbService.ApplyDbSetting(settings, out errorMessage);

            if (!applySuccess)
            {
                LogService.Error("[DB_SETTING][INITIALIZE][APPLY_FAIL] " + errorMessage);
                return;
            }

            LogService.Log("[DB_SETTING][INITIALIZE][SUCCESS] 기본 DB 설정이 자동으로 적용되었습니다.");
        }

        private async void OnDbTestRequested(object sender, EventArgs e)
        {
            dbView.SetOperationEnabled(false);
            
            try
            {
                DbSettings settings;
                string errorMessage= string.Empty;

                // 입력값 정상 확인
                bool inputSuccess = dbView.TryGetDbSetting(out settings, out errorMessage);

                if (!inputSuccess)
                {
                    LogService.Error("[DB_SETTING][CONNECTION_TEST][VALIDATE_FAIL] " + errorMessage);

                    return;
                }

                // 연결 테스트 확인
                bool connectionSuccess = await Task.Run(() => dbService.TestConnection(settings, out errorMessage));

                if (!connectionSuccess)
                {
                    LogService.Error("[DB_SETTING][CONNECTION_TEST][FAIL] " + errorMessage);

                    return;
                }

                const string successMessage = "DB 연결 테스트에 성공했습니다.";

                dbView.ShowInfo(successMessage);

            }
            catch (Exception ex)
            {
                string exceptionMessage = "DB 연결 테스트 실패 : " + ex.Message;

                LogService.Error("[DB_SETTING][CONNECTION_TEST][EXCEPTION] " + exceptionMessage);
            }
            finally { dbView.SetOperationEnabled(true);}

        }
        private async void OnDbApplyRequested(object sender, EventArgs e)
        {
            dbView.SetOperationEnabled(false);
            try
            {
                DbSettings settings;
                string errorMessage;
                
                // 입력값 정상 확인
                bool inputSuccess = dbView.TryGetDbSetting(out settings, out errorMessage);

                if (!inputSuccess)
                {
                    LogService.Error("[DB_SETTING][APPLY][VALIDATE_FAIL] " + errorMessage);


                    return;
                }

                // Db 셋팅 
                bool applySuccess = await Task.Run(()=> dbService.ApplyDbSetting(settings, out errorMessage));

                if (!applySuccess)
                {
                    LogService.Error("[DB_SETTING][APPLY][FAIL] " + errorMessage);

                    return;
                }
                // 화면에 DB설정값 셋팅
                dbView.ShowDbSetting(settings);

                const string successMessage = "DB 설정이 적용되었습니다.";

                dbView.ShowInfo(successMessage);

            }
            catch (Exception ex)
            {
                string exceptionMessage = "DB 설정 적용 처리 실패 : " + ex.Message;

                LogService.Error("[DB_SETTING][APPLY][EXCEPTION] " + exceptionMessage);

            }
            finally { dbView.SetOperationEnabled(true);}
        }

      
    }
}
