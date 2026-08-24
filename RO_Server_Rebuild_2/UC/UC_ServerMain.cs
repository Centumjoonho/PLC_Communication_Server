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
        private bool serverRunning;
        private bool collectRunning;

        // 서버 또는 PLC 시작 정지 작업 중인지 따른 버튼 사용 상태
        private bool operationEnabled = true;

        // RUN 상태 동그라미 표시 전환값
        private bool statusBlinkOn;

        // 현재 시간 화면 표시용 타이머
        private readonly System.Windows.Forms.Timer currentTimeTimer;

        public UC_ServerMain()
        {
            InitializeComponent();
            InitializeView();
            // Status 셀의 색상과 표시 형식 변경
            gridCollect.CellFormatting += GridCollect_CellFormatting;

            // 현재 PC 시간을 1초마다 화면에 표시
            currentTimeTimer = new System.Windows.Forms.Timer();
            currentTimeTimer.Interval = 1000;
            currentTimeTimer.Tick += CurrentTimeTimer_Tick;
            currentTimeTimer.Start();

            // 컨트롤이 제거될 때 타이머 정리
            Disposed += UC_ServerMain_Disposed;

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
            // 현재 API 서버 상태를 먼저 저장
            serverRunning = running;

            btnServerStart.Text = running ? "서버 정지" : "서버 시작";

            lblApiState.Text = running ? "서버 실행" : "서버 정지";
            
            lblApiState.ForeColor = running ? Color.White : Color.Black;
            
            lblApiState.BackColor =running ? Color.FromArgb(0, 128, 64) : Color.LightGray;
            
            btnCollectStart.Enabled = operationEnabled && serverRunning;
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


            collectRunning = running;
        }
        public void SetOperationEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(()=>SetOperationEnabled(enabled)));
                
                return;
                
            }
            operationEnabled = enabled;
            // 작업 중에는 서버 버튼 조작 방지
            btnServerStart.Enabled = enabled;

            // PLC 버튼은 작업 중이 아니고 서버가 실행 중일 때만 사용 가능
            btnCollectStart.Enabled = enabled && serverRunning;
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
        private void GridCollect_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView grid = sender as DataGridView;

            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];

            // Status 열이 아니면 처리하지 않음
            bool statusColumn =
                string.Equals(
                    column.Name,
                    "Status",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    column.DataPropertyName,
                    "Status",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    column.HeaderText,
                    "Status",
                    StringComparison.OrdinalIgnoreCase);

            if (!statusColumn)
            {
                return;
            }

            string status = Convert.ToString(e.Value)
                .Trim()
                .ToUpperInvariant();

            if (status == "RUN")
            {
                Color runColor = Color.FromArgb(0, 128, 64);

                e.CellStyle.ForeColor = runColor;
                e.CellStyle.SelectionForeColor = runColor;

                // 통신 중에는 채운 원과 빈 원을 번갈아 표시
                if (collectRunning)
                {
                    e.Value = statusBlinkOn ? "● RUN" : "○ RUN";
                }
                else
                {
                    e.Value = "○ RUN";
                }

                e.FormattingApplied = true;
            }
            else if (status == "ERROR")
            {
                Color errorColor = Color.FromArgb(190, 45, 45);

                e.CellStyle.ForeColor = errorColor;
                e.CellStyle.SelectionForeColor = errorColor;
            }
            else if (status == "WAIT")
            {
                Color waitColor = Color.FromArgb(190, 120, 0);

                e.CellStyle.ForeColor = waitColor;
                e.CellStyle.SelectionForeColor = waitColor;
            }
            else if (status == "STOP")
            {
                Color stopColor = Color.DimGray;

                e.CellStyle.ForeColor = stopColor;
                e.CellStyle.SelectionForeColor = stopColor;
            }
            else
            {
                // 다른 값은 그리드 기본 색상 사용
                e.CellStyle.ForeColor =
                    grid.DefaultCellStyle.ForeColor;

                e.CellStyle.SelectionForeColor =
                    grid.DefaultCellStyle.SelectionForeColor;
            }
        }

        private void CurrentTimeTimer_Tick(object sender, EventArgs e)
        {
            // 현재 PC의 로컬 시간을 표시
            lblStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // PLC 통신 중일 때만 RUN 동그라미 상태 변경
            if (collectRunning)
            {
                statusBlinkOn = !statusBlinkOn;
            }
            else
            {
                statusBlinkOn = false;
            }

            // Status 셀을 다시 그려 동그라미 표시 변경
            gridCollect.Invalidate();
        }
        private void UC_ServerMain_Disposed(object sender, EventArgs e)
        {
            currentTimeTimer.Stop();
            currentTimeTimer.Tick -= CurrentTimeTimer_Tick;
            currentTimeTimer.Dispose();
        }

       
    }
}
