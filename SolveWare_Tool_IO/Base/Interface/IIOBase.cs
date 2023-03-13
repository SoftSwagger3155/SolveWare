using SolveWare_Service_Core.Base.Interface;
using SolveWare_Tool_IO.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_IO.Base.Interface
{
    public interface IIOBase : IElement
    {
        IOStatus Status { get; set; }
        IOType IOType { get; set; }
        bool Simulation { get; set; }
        bool IsOn();
        bool IsOff();
        void On();
        void Off();
        void UpdateStatus();
    }
}
