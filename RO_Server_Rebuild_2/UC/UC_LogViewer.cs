using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.Services;
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
    public partial class UC_LogViewer : UserControl
    {
        public UC_LogViewer()
        {
            InitializeComponent();

        }
        /// <summary>
        /// 로그를 UI 스레드에서 비동기로 출력
        /// </summary>
        public void WriteMessage(string message, bool isError)
        {
            if(IsDisposed || Disposing) return;

            // 백그라운 스레드에서 호출 된 경우 InvokeRequired = true
            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        if(IsDisposed || Disposing) return;

                        AppendMessage(message, isError);
                    }));

                }
                catch (InvalidOperationException)
                {
                    // 상태 검사 직후 프로그램이 종료되어
                    // 컨트롤 Handle이 제거된 경우 로그 출력을 중단
                }
                return;
            }
            // UI 스레드에서 호출 된 경우 InvokeRequired = true
            AppendMessage(message, isError);
        }

        private void AppendMessage(string message, bool isError)
        {
            // 전체 로그에는 정상과 오류 모두 출력
            AppendText(txtLog, message);

            // 오류 로그에는 오류만 추가 출력
            if (isError)
            {
                AppendText(txtError, message);
            }
        }

        private void AppendText(RichTextBox target, string message)
        {
            target.AppendText(
                message + Environment.NewLine);

            if (target.Lines.Length > 100)
            {
                string[] lines = target.Lines;

                target.Lines =
                    lines.Skip(lines.Length - 100).ToArray();
            }

            target.SelectionStart =
                target.TextLength;

            target.ScrollToCaret();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Reference.LoadUserControls(Reference.Instance.MainPanel, Reference.Instance.UC_ServerMain);
        }
    }
}
