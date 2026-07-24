using RO_Server_Rebuild_2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Views
{
    public interface IDbSettingsView
    {
        event EventHandler DbTestRequested;
        event EventHandler DbApplyRequested;
        bool TryGetDbSetting(out DbSettings settings, out string errorMessage);
        void ShowDbSetting(DbSettings settings);
        void ShowInfo(string message);
        void ShowError(string message);
        void SetOperationEnabled(bool enabled);

    }
}
