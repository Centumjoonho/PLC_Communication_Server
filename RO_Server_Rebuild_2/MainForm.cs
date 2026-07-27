
using RO_Server_Rebuild_2.Api;
using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Plc;
using RO_Server_Rebuild_2.Presenters;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Store;
using RO_Server_Rebuild_2.UC;
using System;
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
        private readonly PlcDataStore _plcDataStore;
        private readonly DbSettingsService _dbSettingsService;
        private readonly DbSettingsPresenter _dbSettingsPresenter;
        private readonly PlcRegisterService _plcRegisterService;
        private readonly PlcRegisterPresenter _plcRegisterPresenter;
        private readonly ServerMainService _serverMainService;
        private readonly ServerMainPresenter _serverMainPresenter;
        private readonly ApiSettingsService _apiSettingsService;
        private readonly ApiSettingsPresenter _apiSettingsPresenter;

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

            // Service  와 Presenter 생성 및 연결
            _dbSettingsService = new DbSettingsService();
            _dbSettingsPresenter = new DbSettingsPresenter(Reference.Instance.UC_AdminSettings, _dbSettingsService);

            _plcDb = new PlcDb(_dbSettingsService);
            _plcReader = new PlcReader();
            _apiServer = new ApiServer();
            _plcDataStore = new PlcDataStore();

            _plcRegisterService = new PlcRegisterService(_plcDb);
            _plcRegisterPresenter =new PlcRegisterPresenter(Reference.Instance.UC_PlcRegister, _plcRegisterService);

            _apiSettingsService = new ApiSettingsService();
            _apiSettingsPresenter = new ApiSettingsPresenter(Reference.Instance.UC_AdminSettings, _apiSettingsService);

            _serverMainService = new ServerMainService(_plcDb, _plcReader, _apiServer , _plcDataStore, _apiSettingsService);
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

            LogService.Log("통합 로그창이 초기화되었습니다.");

        }

    

        private void AddUserControl(UserControl userControl) { 
            
            if(userControl == null) { return; }

            if(mainPanel.Controls.Contains(userControl)) {return; }
            
            userControl.Dock = DockStyle.Fill;
            userControl.Visible = false;

            mainPanel.Controls.Add(userControl);

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
           
            shutdownInProgress = true;

            try
            {
                LogService.Log("프로그램 종료 요청");

                bool shutdownSuccess = await _serverMainPresenter.ShutdownAsync();

                if (!shutdownSuccess) {

                    shutdownInProgress = false;

                    LogService.Error("프로그램 종료 준비에 실패했습니다.");

                    return;
                }

                shutdownCompleted = true;

                Close();
            }
            catch (Exception ex)
            {

                shutdownInProgress = false;

                LogService.Error("프로그램 종료 처리 실패 : " + ex.Message);
            }
                
        }
    }
}
