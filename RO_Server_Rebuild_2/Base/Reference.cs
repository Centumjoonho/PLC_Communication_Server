using RO_Server_Rebuild_2.UC;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2.Base
{
    internal class Reference
    {
        private static readonly Reference _instance = new Reference();

        public static Reference Instance
        {
            get
            {
                return _instance;
            }
        }

        private Reference() { }

        // 패널에 UC를 올리고 선택한 UC만 보이게 합니다.
        public static void LoadUserControls(Panel panel, UserControl userControl)
        {
            if (panel == null || userControl == null) { return; }

            if(!panel.Controls.Contains(userControl)) { return; }

            foreach (Control control in panel.Controls) 
            {
                control.Visible = false;
            }

            userControl.Visible = true;
            userControl.BringToFront();
        
        }

        public MainForm MainForm { get; set; }
        public Panel MainPanel { get; set; }
        public UC_ServerMain UC_ServerMain { get; set; }
        public UC_PlcRegister UC_PlcRegister {  get; set; } 
        public UC_LogViewer UC_LogViewer { get; set; }
        public UC_AdminSettings UC_AdminSettings { get; set; }
    }
}
