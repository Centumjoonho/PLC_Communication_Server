using RO_Server_Rebuild_2.Models;
using RO_Server_Rebuild_2.Services;
using RO_Server_Rebuild_2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RO_Server_Rebuild_2.Presenters
{
    public class PlcRegisterPresenter
    {
        private readonly IPlcRegisterView prView;
        private readonly PlcRegisterService prService;

        public PlcRegisterPresenter(IPlcRegisterView prView, PlcRegisterService prService)
        {
            this.prView = prView;
            this.prService = prService;

            prView.SaveRequested += OnSaveRequested;
            prView.DeleteRequested += OnDeleteRequested;
        }

   
        private async void OnDeleteRequested(object sender, EventArgs e)
        {
            
            string plcCode = prView.GetSelectedPlcCode();

            if (string.IsNullOrWhiteSpace(plcCode))
            {
                const string validationMessage = "삭제할 PLC를 선택하세요";

                prView.ShowError(validationMessage );
                
                return;
            }

            if (!prView.ConfirmDelete(plcCode))
            {
                return;
            }
            // 삭제 처리 중 중복 조작 방지
            prView.SetOperationEnabled(false);

            try
            {
                string errorMessage = string.Empty;

                bool deleteSuccess = await Task.Run(() => prService.DeletePlcMaster(plcCode, out errorMessage));

                if (!deleteSuccess) {
                  
                    prView.ShowError(errorMessage);

                    return;
                }
                // 삭제 했으니까 리로드
                await ReloadPlcMasterListAsync();

                string successMessage = plcCode + ": PLC 정보가 삭제되었습니다.";

                prView.ShowInfo(successMessage);

            }
            catch (Exception ex)
            {
                LogService.Error("PLC  정보 삭제 처리 실패: "+ex.Message);
            }
            finally { prView.SetOperationEnabled(true); }

        }
        private async void OnSaveRequested(object sender, EventArgs e)
        {
            prView.SetOperationEnabled(false );
            try
            {
                PlcMaster plcMaster;
                string errorMessage = string.Empty;

                bool inputSuccess = prView.TryGetInputPlcMaster(out plcMaster, out errorMessage);

                if (!inputSuccess) {

                    prView.ShowError(errorMessage);

                    return ;
                }

                bool saveSuccess = await Task.Run(()=>prService.SavePlcMaster(plcMaster, out errorMessage));

                if (!saveSuccess) {

                    prView.ShowError(errorMessage);
                    
                    return;
                }
                await ReloadPlcMasterListAsync();

                const string successMessage = "PLC 정보가 저장되었습니다.";

                prView.ShowInfo(successMessage);
            }
            catch (Exception ex)
            {
                LogService.Error("PLC 정보 저장 처리 실패:" + ex.Message);
            }
            finally
            {
                prView.SetOperationEnabled(true);
            }
        }
        // 관리 대상 plc 목록 리로드
        private async Task ReloadPlcMasterListAsync()
        {
            IList<PlcMaster> plcMasterList = await Task.Run(() => prService.ReadPlcMasterList());
            
            prView.ShowPlcMasterList(plcMasterList);
        }
        public async Task RefreshPlcMasterListAsync()
        {
            prView.SetOperationEnabled(false);

            try
            {
                await ReloadPlcMasterListAsync();
            }
            catch (Exception ex)
            {
                LogService.Error("PLC 목록 조회 실패" +ex.Message);  
            }
            finally { prView.SetOperationEnabled(true);}
        }

    }
}
