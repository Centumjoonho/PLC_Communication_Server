using RO_Server_Rebuild_2.Base;
using RO_Server_Rebuild_2.UC;
using System;
using System.Threading;

namespace RO_Server_Rebuild_2.Services
{
    public static class LogService
    {
        private static long logSequence;

        public static void Log(string message)
        {
            Write("LOG" , message , false);
        }

        public static void Error(string message)
        {
            Write("ERROR", message , true);
        }

        private static void Write(string level, string message, bool isError)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            // 여러 PLC Task가 동시에 호출해도 중복되지 않는 순번
            long sequence = Interlocked.Increment(ref logSequence);

            DateTime occurredAt = DateTime.Now;

            string logText =
                $"[{sequence:D6}] " +
                $"[{occurredAt:HH:mm:ss.fff}] " +
                $"[{level}] {message}";


            UC_LogViewer logViewer = Reference.Instance.UC_LogViewer;

            if(logViewer == null || logViewer.IsDisposed || logViewer.Disposing)
            {
                return;
            }

            logViewer.WriteMessage(logText, isError);
        }
    }
}
