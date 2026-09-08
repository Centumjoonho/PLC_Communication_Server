
using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Plc;
using RO_Server_Rebuild_2.Presenters;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Store;
using RO_Server_Rebuild_2.UC;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2
{
    public partial class MainForm : Form
    {

        private bool shutdownInProgress;
        private bool shutdownCompleted;

        private readonly PlcDb _plcDb;
        private readonly PlcReader _plcReader;
        private readonly ApiServer _apiServer;
        private readonly ApiHandler _apiHandler;
        private readonly PlcDataStore _plcDataStore;
        private readonly RunRateService _runRateService;
        private readonly DbSaveService _dbSaveService;

        private readonly DbSettingsService _dbSettingsService;
        private readonly DbSettingsPresenter _dbSettingsPresenter;

        private readonly PlcRegisterService _plcRegisterService;
        private readonly PlcRegisterPresenter _plcRegisterPresenter;
        
        private readonly ServerMainService _serverMainService;
        private readonly ServerMainPresenter _serverMainPresenter;
       
        private readonly ApiSettingsService _apiSettingsService;
        private readonly ApiSettingsPresenter _apiSettingsPresenter;
       
        private readonly PlcCollectionService _plcCollectionService;

        public MainForm()
        {
            InitializeComponent();
            
            Reference.Instance.MainForm =this;
            Reference.Instance.MainPanel = mainPanel;
            
            // View 생성
            Reference.Instance.UC_ServerMain = new UC_ServerMain();
            Reference.Instance.UC_PlcRegister = new UC_PlcRegister();
            Reference.Instance.UC_LogViewer = new UC_LogViewer();
            Reference.Instance.UC_AdminSettings = new UC_AdminSettings();

            // DB 설정
            _dbSettingsService = new DbSettingsService();
            _dbSettingsPresenter = new DbSettingsPresenter( Reference.Instance.UC_AdminSettings,_dbSettingsService);
            
            // PLC 와 데이터 저장 구성
            _plcDb = new PlcDb(_dbSettingsService);
            _plcReader = new PlcReader();
            _plcDataStore = new PlcDataStore();
            _runRateService = new RunRateService();
            _dbSaveService = new DbSaveService(_plcDb);

            // PLC 수집 Service
            _plcCollectionService = new PlcCollectionService(_plcDb, _plcReader, _runRateService, _dbSaveService, _plcDataStore);
            
            // API 설정
            _apiSettingsService = new ApiSettingsService();
            _apiSettingsPresenter = new ApiSettingsPresenter(Reference.Instance.UC_AdminSettings, _apiSettingsService);

            // API 요청 처리기와 서버
            _apiHandler = new ApiHandler(_plcDb, _apiSettingsService, _plcDataStore, () => _plcCollectionService.IsRunning());
            _apiServer = new ApiServer(_apiHandler);
            
            // PLC 등록
            _plcRegisterService = new PlcRegisterService(_plcDb);
            _plcRegisterPresenter =new PlcRegisterPresenter(Reference.Instance.UC_PlcRegister, _plcRegisterService);

            // 서버 메인
            _serverMainService = new ServerMainService( _apiServer , _apiSettingsService , _plcCollectionService);
            _serverMainPresenter = new ServerMainPresenter(Reference.Instance.UC_ServerMain, _serverMainService);
  

            // 생성한 화면을 판낼에 등록
            AddUserControl(Reference.Instance.UC_ServerMain);
            AddUserControl(Reference.Instance.UC_PlcRegister);
            AddUserControl(Reference.Instance.UC_LogViewer);
            AddUserControl(Reference.Instance.UC_AdminSettings);
            
            // 등록된 화면을 프로그램 초기 화면으로 사용
            Reference.LoadUserControls(mainPanel, Reference.Instance.UC_ServerMain);

            // 데이터 수집 상태 변경 -> 관리자 페이지 데이터 입력 방지 
            _serverMainService.CollectRunningChanged += SetPlcCollectRunning;
            _serverMainService.ServerRunningChanged += SetApiServerRunning;

            LogService.Log("[PROGRAM][LOG_VIEWER][READY] 통합 로그창이 초기화되었습니다.");

        }

        private void AddUserControl(UserControl userControl) { 
            
            if(userControl == null) { return; }

            if(mainPanel.Controls.Contains(userControl)) {return; }

            // 디자이너의 기본 Button 형식은 그대로 유지하고,
            // 실행 중 비활성화된 버튼의 모양만 변경합니다.
            RegisterDisabledButtonPaint(userControl);

            userControl.Dock = DockStyle.Fill;
            userControl.Visible = false;

            mainPanel.Controls.Add(userControl);

        }

        private void RegisterDisabledButtonPaint(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Button button = control as Button;

                if (button != null)
                {
                    button.Paint += DisabledButton_Paint;
                    button.EnabledChanged += DisabledButton_EnabledChanged;
                }

                if (control.HasChildren)
                {
                    RegisterDisabledButtonPaint(control);
                }
            }
        }

        private void DisabledButton_EnabledChanged(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                button.Invalidate();
            }
        }

        private void DisabledButton_Paint(object sender, PaintEventArgs e)
        {
            Button button = sender as Button;

            if (button == null || button.Enabled)
            {
                return;
            }

            Color disabledBackColor = ControlPaint.Light(button.BackColor, 0.35f);

            using (SolidBrush backgroundBrush = new SolidBrush(disabledBackColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, button.ClientRectangle);
            }

            Rectangle borderRectangle = button.ClientRectangle;
            borderRectangle.Width -= 1;
            borderRectangle.Height -= 1;

            using (Pen borderPen = new Pen(ControlPaint.Dark(disabledBackColor)))
            {
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }

            TextRenderer.DrawText(
                e.Graphics,
                button.Text,
                button.Font,
                button.ClientRectangle,
                Color.LightGray,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix);
        }

        private async void menuPlcRegister_Click(object sender, System.EventArgs e)
        {
            Reference.LoadUserControls(mainPanel,Reference.Instance.UC_PlcRegister);

            await _plcRegisterPresenter.RefreshPlcMasterListAsync();
        }

        private void menuLogViewer_Click(object sender, System.EventArgs e)
        {
            Reference.LoadUserControls(mainPanel , Reference.Instance.UC_LogViewer);
        }

        private void menuAdminSetting_Click(object sender, System.EventArgs e)
        {
            Reference.LoadUserControls(mainPanel,Reference.Instance.UC_AdminSettings);
        }

        private void SetPlcCollectRunning(bool running)
        {
            Reference.Instance.UC_AdminSettings.SetPlcCollectRunning(running);
            Reference.Instance.UC_PlcRegister.SetPlcCollectRunning(running);
        }
        private void SetApiServerRunning(bool running)
        {
            Reference.Instance.UC_AdminSettings.SetApiServerRunning(running);
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
 
            if (shutdownCompleted) { return; }

            e.Cancel = true;

            if(shutdownInProgress) { return; }

            DialogResult result = MessageBox.Show("프로그램을 종료하시겠습니까?", "종료확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {

                e.Cancel = true;

                return;

            }
            shutdownInProgress = true;


            try
            {
                LogService.Log("[PROGRAM][SHUTDOWN][REQUEST] 프로그램 종료가 요청되었습니다.");

                bool shutdownSuccess = await _serverMainPresenter.ShutdownAsync();

                if (!shutdownSuccess) {

                    shutdownInProgress = false;

                    LogService.Error("[PROGRAM][SHUTDOWN][FAIL] 프로그램 종료 준비에 실패했습니다.");

                    return;
                }

                shutdownCompleted = true;

                Close();
            }
            catch (Exception ex)
            {

                shutdownInProgress = false;

                LogService.Error("[PROGRAM][SHUTDOWN][EXCEPTION] " + ex.Message);
            }
                
        }

        private void menuLogViewer_Click_1(object sender, EventArgs e)
        {
            Reference.LoadUserControls(mainPanel, Reference.Instance.UC_LogViewer);
        }
    }
}
