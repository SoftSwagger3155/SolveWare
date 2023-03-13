using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Definitions;
using SolveWare_Service_Core.Info.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Manager.Base.Interface
{
    public interface IMainManager : IMachineStatus
    {
        IView MainWint { get; set; }
        bool HasIdenticalWindow();
        string LoadingStatus { get; set; }
        void Initialize();
        void DoButtonClickTask(Func<int> action);
        IMachineUI MachineUI { get; set; }
        IInfoHandler Infohandler { get; set; }
        IList<IDataResourceProvider> Resource_Data_Center { get; set; }
        IList<IToolResourceProvider> Resource_Tool_Center { get; set; }
        IResourceProvider Get_Single_Resource_Item(ResourceProvider_Kind resourceKind, Type classType);
        void CloseAll();
    }
}
