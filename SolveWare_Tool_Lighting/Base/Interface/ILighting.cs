using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Communications.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Lighting.Base.Interface
{
    public interface ILighting: IElement
    {
        void Setup(IInstrumentChassis chassis, int ch);
        void Set_Intensity(int value);
    }
}
