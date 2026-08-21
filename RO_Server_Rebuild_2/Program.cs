using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2
{
    internal static class Program
    {
        // 프로그램이 실행되는 동안 Mutex 객체가 유지되도록 필드로 선언
        private static Mutex appMutex;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;

            // 같은 이름의 Mutex가 없으면 createdNew가 true
            appMutex = new Mutex(
                true,
                @"Global\RO_Server_Rebuild_2_Mutex",
                out createdNew);

            // 이미 실행 중인 서버가 있는 경우 프로그램 실행 중단
            if (!createdNew)
            {
                MessageBox.Show(
                    "RO Server가 이미 실행 중입니다.",
                    "중복 실행 방지",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                appMutex.Dispose();
                return;
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            finally
            {
                // 프로그램 종료 시 Mutex 소유권과 운영체제 자원 해제
                appMutex.ReleaseMutex();
                appMutex.Dispose();
            }
        }
    }
}
