using RO_Server_Rebuild_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO_Server_Rebuild_2.Views
{
    public interface IServerMainView
    {
        event EventHandler ServerToggleRequested;
        event EventHandler CollectToggleRequested;

        void SetServerRunning(bool running);
        void SetCollectRunning(bool running);
        void SetOperationEnabled(bool enabled);
        void ShowCollectedData(IList<PlcData> dataList);
    }
}
