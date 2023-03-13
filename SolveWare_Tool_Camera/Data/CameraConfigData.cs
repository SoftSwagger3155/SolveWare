using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Tool_Camera.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Camera.Data
{
    public class CameraConfigData: ElementModelBase
    {
        public string Id_Camera { get; set; }
        public string Camera_Name { get;set; }
        public bool IsSimulation { get; set; }
        public Master_Driver_Camera MasterDriver { get; set; }
    }
}
