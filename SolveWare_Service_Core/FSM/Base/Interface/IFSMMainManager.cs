using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.FSM.Base.Interface
{
    public interface IFSMMainManager
    {
        IList<IFSMStation> FSM_Stations { get; set; }
        void Home();
        void Reset();
        void Stop();
        void Run_Auto_Cycle();
        void Run_One_Cycle();
        void Build_Tool_Resource();
        void Build_Data_Resource();
        void Build_Vision_Resource();
    }
}
