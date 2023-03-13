using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Tool_Lighting.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Lighting.Data
{
    public class LightingConfigData: ModelBase, IElement
    {
        public string Resource { get; set; }
        public Lighting_Communication_Kind Communication_Kind { get; set; }
        public bool IsOnLine { get; set; }
        public string Name { get; set; }
        public int Channel { get; set; }
        public long Id { get; }
    }
}
