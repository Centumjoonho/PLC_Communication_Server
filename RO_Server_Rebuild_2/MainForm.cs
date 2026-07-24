
using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Database;
using RO_Server_Rebuild_2.Presenters;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.UC;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2
{
    public partial class MainForm : Form
    {
        private readonly PlcDb _plcDb;
        private readonly DbSettingsService _dbSettingsService;
        private readonly DbSettingsPresenter _dbSettingsPresenter;
        private readonly PlcRegisterService _plcRegisterService;
        private readonly PlcRegisterPresenter _plcRegisterPresenter;

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

            _plcRegisterService = new PlcRegisterService(_plcDb);
            _plcRegisterPresenter =new PlcRegisterPresenter(Reference.Instance.UC_PlcRegister, _plcRegisterService);

            // 생성한 화면을 판낼에 등록
            AddUserControl(Reference.Instance.UC_ServerMain);
            AddUserControl(Reference.Instance.UC_PlcRegister);
            AddUserControl(Reference.Instance.UC_LogViewer);
            AddUserControl(Reference.Instance.UC_AdminSettings);
            
            // 등록된 화면을 프로그램 초기 화면으로 사용
            Reference.LoadUserControls(mainPanel, Reference.Instance.UC_ServerMain);
            
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

        public void SetPlcCollectRunning(bool running)
        {
            Reference.Instance.UC_AdminSettings.SetPlcCollectRunning(running);
            Reference.Instance.UC_PlcRegister.SetPlcCollectRunning(running);
        }
    }
}
