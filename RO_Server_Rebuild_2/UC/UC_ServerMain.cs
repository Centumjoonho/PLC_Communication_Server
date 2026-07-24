using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Presenters;
using RO_Server_Rebuild_2.Services;
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
    public partial class UC_ServerMain : UserControl, IServerMainView
    {
       
        public UC_ServerMain()
        {
            InitializeComponent();
            InitializeView();
        }

        public event EventHandler ServerToggleRequested;
        public event EventHandler CollectToggleRequested;

        private void InitializeView()
        {
            SetServerRunning(false);
            SetCollectRunning(false);
        }

        private void btnServerStart_Click(object sender, EventArgs e)
        {
            ServerToggleRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnCollectStart_Click(object sender, EventArgs e)
        {
            CollectToggleRequested?.Invoke(this, EventArgs.Empty);
        }

        public void SetServerRunning(bool running)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetServerRunning(running)));
                return;
            }

            btnServerStart.Text = running ? "서버 정지" : "서버 시작";
            lblApiState.Text = running ? "서버 실행" : "서버 정지";
            lblApiState.ForeColor = running ? Color.White : Color.Black;
            lblApiState.BackColor =running ? Color.FromArgb(0, 128, 64) : Color.LightGray;

        }

        public void SetCollectRunning(bool running)
        {
           if(InvokeRequired)
            {
                BeginInvoke(new Action(()=> SetCollectRunning(running)));
                return;
            }
            lblRepeatState.Text = running ? "PLC 통신중" : "PLC 통신 정지";

            btnCollectStart.Text =running ? "PLC 통신 정지" : "PLC 통신 시작";

            lblRepeatState.ForeColor = running ? Color.White : Color.Black;

            lblRepeatState.BackColor = running ? Color.FromArgb(0, 128, 64) : Color.LightGray;
        }

        public void ShowCollectedData(IList<PlcData> dataList)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowCollectedData(dataList)));
                
                return;
            }

            // null을 방지하고 Grid에 연결할 목록 복사본 생성
            List<PlcData> displayList;
            
            if(dataList == null)
            {
                displayList =new List<PlcData>();
            }
            else
            {
                displayList =new List<PlcData>(dataList);
            }

            // 그리드 셋팅
            gridCollect.DataSource = null;
            gridCollect.DataSource = displayList;

        }
    }
}
