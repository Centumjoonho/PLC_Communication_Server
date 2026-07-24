using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Services
{
    public static class LogService
    {
        public static event Action<string, bool> MessageLogged;

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
            string logText =
                $"[{DateTime.Now:HH:mm:ss}] " +
                $"[{level}] {message}";

            MessageLogged?.Invoke(
                logText,
                isError);
        }
    }
}
