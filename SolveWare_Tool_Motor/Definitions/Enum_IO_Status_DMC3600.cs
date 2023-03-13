using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Definitions
{
    public enum IO_Status_DMC3600
    {
        ALM = 1,
        PEL = 2,
        NEL = 4,
        EMG = 8,
        ORG = 16,
        PSL = 64,
        MSL = 128,
        INP = 256,
        EZ = 512,
        RDY = 1024,
        DSTP = 2048
    }
}
