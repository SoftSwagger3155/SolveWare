using SolveWare_Service_Core.Communications.Base.Interface;
using SolveWare_Tool_Lighting.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Lighting.Business
{
    public class LightingDevice : ILighting
    {
        IInstrumentChassis chassis;
        public string Name { get; set; }
        int channel { get; set; }
        public long Id { get; private set; }

        public void Setup(IInstrumentChassis chassis, int ch)
        {
            this.chassis = chassis;
            this.channel = ch;
        }

        public void Set_Intensity(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            chassis.Query(bytes, 200);
        }
    }
}
