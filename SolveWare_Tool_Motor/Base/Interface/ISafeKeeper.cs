using SolveWare_Tool_Motor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Base.Interface
{
    public interface ISafeKeeper
    {
        bool IsPositionDangerous(PositionSafeData pSafeData);
        void RunSafeKeeper();
        void StopSafeKeeper();
        bool CheckZoneSafeToMove(double pos);

        bool MoveToObserverPos(PositionSafeData pSafeData);
    }
}
