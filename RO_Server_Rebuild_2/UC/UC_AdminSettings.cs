using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2.UC
{
    public partial class UC_AdminSettings : UserControl ,IDbSettingsView , IApiSettingsView
    {
        private bool plcCollectRunning;
        private bool apiServerRunning;

        public UC_AdminSettings()
        {
            InitializeComponent();
        }

        public event EventHandler DbTestRequested;
        public event EventHandler DbApplyRequested;
        public event EventHandler ApiTestRequested;
        public event EventHandler ApiApplyRequested;



        private void btnBack_Click(object sender, EventArgs e)
        {
            Reference.LoadUserControls(Reference.Instance.MainPanel, Reference.Instance.UC_ServerMain);
        }

        private void btnDbTest_Click(object sender, EventArgs e)
        {
            DbTestRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnDbApply_Click(object sender, EventArgs e)
        {
            DbApplyRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnApiTest_Click(object sender, EventArgs e)
        {
            ApiTestRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnApiApply_Click(object sender, EventArgs e)
        {
            ApiApplyRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ShowInfo(string message)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowInfo(message)));

                return;
            }
            MessageBox.Show(this, message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void ShowError(string message)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowError(message)));
                return;
            }
            MessageBox.Show(this, message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // PLC 수집중에는 수정 불가 인터록
        public void SetPlcCollectRunning(bool running)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>  SetPlcCollectRunning(running) ));

                return;
            }

            plcCollectRunning = running;

            // PLC 수집 중이거나 API 서버 실행 중이면 DB 설정 잠금
            dbPanel.Enabled = !plcCollectRunning && !apiServerRunning;

            // API 설정은 PLC 수집 중이거나 API 서버 실행 중이면 잠금
            apiPanel.Enabled = !plcCollectRunning && !apiServerRunning;
        }
        // API 서버 실행 중에는 API Port와 API Key를 변경할 수 없음
        public void SetApiServerRunning(bool running)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetApiServerRunning(running)));

                return;
            }

            apiServerRunning = running;

            // PLC 수집 중이거나 API 서버 실행 중이면 DB 설정 잠금
            dbPanel.Enabled = !plcCollectRunning && !apiServerRunning;

            // API 설정은 PLC 수집 중이거나 API 서버 실행 중이면 잠금
            apiPanel.Enabled = !plcCollectRunning && !apiServerRunning;
        }




        // 중복 클릭 방지 함수
        public void SetOperationEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetOperationEnabled(enabled)));
                
                return;
            }

            btnDbTest.Enabled = enabled;
            btnDbApply.Enabled = enabled;
        }
        public void SetApiOperationEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetApiOperationEnabled(enabled)));

                return;

            }

            btnApiTest.Enabled = enabled;
            btnApiApply.Enabled = enabled;

        }
 
        public bool TryGetDbSetting(out DbSettings settings, out string errorMessage)
        {
            settings = null;
            errorMessage = string.Empty;

            int _port;

            if (!int.TryParse(txtDbPort.Text.Trim(), out _port))
            {

                errorMessage = "DB Port는 숫자로 입력하세요";

                return false;
            }

            settings = new DbSettings
            {

                Server = txtDbServer.Text.Trim(),
                Port = _port,
                DatabaseName = txtDbName.Text.Trim(),
                UserId = txtDbUserId.Text.Trim(),
                Password = txtDbPassword.Text

            };

            return true;
        }
        public void ShowDbSetting(DbSettings settings)
        {
            if (settings == null) { return; }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowDbSetting(settings)));

                return;
            }

            txtDbServer.Text = settings.Server;
            txtDbPort.Text = settings.Port.ToString();
            txtDbName.Text = settings.DatabaseName;
            txtDbUserId.Text = settings.UserId;
            txtDbPassword.Text = settings.Password;
        }
        public bool TryGetApiSettings(out ApiSettings apiSettings, out string errorMessage)
        {
            apiSettings = null;
            errorMessage = string.Empty;

            int apiPort;

            if(!int.TryParse(txtApiPort.Text.Trim(),out apiPort))
            {
                errorMessage = "API Port는 숫자로 입력하세요.";

                return false;
            }

            apiSettings = new ApiSettings
            {
                Port = apiPort,
                ApiKey = txtApiKey.Text.Trim(),
            };
            return true;
        }

        public void ShowApiSettings(ApiSettings apiSettings)
        {
            if(apiSettings == null)
            {
                return ;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowApiSettings(apiSettings)));

                return;
            }

            txtApiPort.Text = apiSettings.Port.ToString();
            txtApiKey.Text = apiSettings.ApiKey;
        }
    }
}
