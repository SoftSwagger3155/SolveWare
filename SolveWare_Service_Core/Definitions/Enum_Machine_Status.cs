using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Definitions
{
    public enum Machine_Status
    {
        UnInitialised,
        Initialising,
        Idle,
        Auto,
        SingleCycle,
        Busy,
        Error,
        Stop,
        Reset
    }
}
