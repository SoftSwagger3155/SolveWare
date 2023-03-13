using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolveWare_Service_Core.Info.Base.Interface
{
    public interface IInfoHandler
    {
        void LogMessage(string msg, bool isWindowShow = false, bool isError = false);
        void PopUpHandyControlMessage(string msg);
        void LogActionMessage(string msg, string errorMsg = "", int errorCode = 0);
        void LogActionMessage(string title, string msg, DateTime st, int errorCode = 0, string errorMsg = "");
        void LogExceptionMessage(string msg, Exception ex, DateTime st, bool isWindowShow = true);
        void EnableLog(bool enableLog);
        void SetUI(object uiForDisplayInfo, object uiMessageForm);

        ObservableCollection<string> TotalMessage { get; set; }
        ObservableCollection<string> TotalErrorMessage { get; set; }

        void PopUp_Total_Messages();
        void PopUp_Error_Messages();
    }
}
