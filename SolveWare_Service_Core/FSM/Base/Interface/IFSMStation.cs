using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.FSM.Base.Interface
{
    public interface IFSMStation
    {
        string Name { get; set; }
        FSM_Station_Status Status { get; set; }
        JobFundamentalBase CurrentState { get; }
        void ObtainResource();
        void SetFSMChain();
        void MakeOperationUI();
        void CreateStateInstance();
        void Run();
    }
}
