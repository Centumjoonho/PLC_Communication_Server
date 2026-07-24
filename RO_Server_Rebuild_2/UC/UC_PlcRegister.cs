using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Models;
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
    public partial class UC_PlcRegister : UserControl , IPlcRegisterView
    {
        public event EventHandler SaveRequested;
        public event EventHandler DeleteRequested;
      

        public UC_PlcRegister()
        {
            InitializeComponent();
        }

        public void SetPlcCollectRunning( bool running)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetPlcCollectRunning(running)));
                
                return;
            }

            inputPanel.Enabled = !running;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPlcCode.Clear();
            txtPlcName.Clear();
            txtPlcIp.Clear();
            txtPlcPort.Text = "502";
            txtMemoryAddress.Text = "30";
            chkUseYn.Checked = true;
            txtPlcCode.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Reference.LoadUserControls(Reference.Instance.MainPanel, Reference.Instance.UC_ServerMain);
        }

        private void gridPlcMaster_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex<0) return;

            PlcMaster selectedPlc = gridPlcMaster.Rows[e.RowIndex].DataBoundItem as PlcMaster;

            if(selectedPlc == null) return;

            ShowPlcMasterInput(selectedPlc);
        }

        private void ShowPlcMasterInput(PlcMaster selectedPlc)
        {
            txtPlcCode.Text = selectedPlc.PlcCode;
            txtPlcName.Text = selectedPlc.PlcName;
            txtPlcIp.Text = selectedPlc.PlcIp;
            txtPlcPort.Text = selectedPlc.PlcPort.ToString();
            txtMemoryAddress.Text = selectedPlc.MemoryAddress;
            chkUseYn.Checked = selectedPlc.UseYn;
        }

        public string GetSelectedPlcCode()
        {
            if(gridPlcMaster.CurrentRow == null)
            {
                return string.Empty;
            }

            PlcMaster selectedPlc = gridPlcMaster.CurrentRow.DataBoundItem as PlcMaster;

            if(selectedPlc == null)
            {
                return string.Empty;
            }

            return selectedPlc.PlcCode;

        }

        public void ShowPlcMasterList(IList<PlcMaster> plcMasterList)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowPlcMasterList(plcMasterList)));

                return;
            }

            // null을 방지하고 Grid에 연결할 목록 복사본 생성
            List<PlcMaster> displayList;

            if(plcMasterList == null)
            {
                displayList = new List<PlcMaster>();
            }
            else
            {
                displayList = new List<PlcMaster>(plcMasterList);
            }

            gridPlcMaster.DataSource = null;
            gridPlcMaster.DataSource = displayList;
        }

        public bool TryGetInputPlcMaster(out PlcMaster plcMaster, out string errorMessage)
        {
            int plcPort = 0;
            plcMaster = null;
            errorMessage = string.Empty;

            if(!int.TryParse(txtPlcPort.Text.Trim(), out plcPort)){
                
                errorMessage = "Port는 숫자로 입력하세요";
                return false;

            }

            plcMaster = new PlcMaster
            {
                PlcCode = txtPlcCode.Text.Trim(),
                PlcName = txtPlcName.Text.Trim(),
                PlcIp = txtPlcIp.Text.Trim(),
                PlcPort = plcPort,
                MemoryAddress = txtMemoryAddress.Text.Trim(),
                UseYn = chkUseYn.Checked
            };

            return true;
            
        }

        public void ShowInfo(string message)
        {
            if(IsDisposed ||Disposing) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowInfo(message)));
                
                return;
            }

            MessageBox.Show(this,message,"알람",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        public void ShowError(string message)
        {
            if(IsDisposed || Disposing) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowError(message)));

                return;
            }

            MessageBox.Show(this, message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void SetOperationEnabled(bool enabled)
        {
            if (InvokeRequired) {

                BeginInvoke(new Action(() => SetOperationEnabled(enabled)));
                return;
            }
            txtPlcCode.Enabled = enabled;
            txtPlcName.Enabled = enabled;
            txtPlcIp.Enabled = enabled;
            txtPlcPort.Enabled = enabled;
            txtMemoryAddress.Enabled = enabled;
            chkUseYn.Enabled = enabled;

            btnClear.Enabled = enabled;
            btnSave.Enabled = enabled;
            btnDelete.Enabled = enabled;
            gridPlcMaster.Enabled = enabled;
        }

        public bool ConfirmDelete(string plcCode)
        {
            DialogResult dialogResult = MessageBox.Show(this, plcCode + ": PLC 정보를 삭제 하겠습니까?", "PLC 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return dialogResult == DialogResult.Yes;
        }
    }
}
