using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Views
{
    public interface IPlcRegisterView
    {
        event EventHandler SaveRequested;
        event EventHandler DeleteRequested;

        bool TryGetInputPlcMaster(out PlcMaster plcMaster, out string errorMessage);
        string GetSelectedPlcCode();
        void ShowPlcMasterList(IList<PlcMaster> plcMasterList);
        void ShowInfo(string message);
        void ShowError(string message);
        void SetOperationEnabled(bool enabled);
        bool ConfirmDelete(string plcCode);

    }
}
