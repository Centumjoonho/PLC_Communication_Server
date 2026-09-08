using RO_Server_Rebuild_2.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Views
{
    public interface IApiSettingsView
    {
        event EventHandler ApiTestRequested;
        event EventHandler ApiApplyRequested;

        bool TryGetApiSettings(out ApiSettings apiSettings, out string errorMessage);
        void ShowApiSettings(ApiSettings apiSettings);
        void ShowInfo(string message);
        void ShowError(string message);
        void SetApiOperationEnabled(bool enabled);
    }
}
