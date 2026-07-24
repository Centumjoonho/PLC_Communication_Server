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

            LogService.MessageLogged += OnMessageLogged;

            Disposed += UC_LogViewer_Disposed;

        }
        //이벤트 구독 해제
        private void UC_LogViewer_Disposed(object sender, EventArgs e)
        {
            LogService.MessageLogged -= OnMessageLogged;
        }

        private void OnMessageLogged(string message, bool isError)
        {
            if(IsDisposed || Disposing) return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => OnMessageLogged(message, isError)));

                }
                catch (InvalidOperationException)
                {
                    // 상태 검사 직후 프로그램이 종료되어
                    // 컨트롤 Handle이 제거된 경우 로그 출력을 중단
                }
                return;
            }
            AppendMessage(message, isError);
        }

        private void AppendMessage(string message, bool isError) {
            
            RichTextBox target;

            if (isError)
            {
                target = txtError;
            }
            else
            {
                target =txtLog;
            }

            target.AppendText(message +  Environment.NewLine);

            if (target.Lines.Length > 100)
            {
                string[]lines = target.Lines;
                target.Lines = lines.Skip(lines.Length-100).ToArray();
          
            }
            
            target.SelectionStart = target.TextLength;
            
            target.ScrollToCaret();
        
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Reference.LoadUserControls(Reference.Instance.MainPanel, Reference.Instance.UC_ServerMain);
        }
    }
}
