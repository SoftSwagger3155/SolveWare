using SolveWare_Service_Core;
using SolveWare_Service_Core.Definitions;
using SolveWare_Tool_Motor.Attributes;
using SolveWare_Tool_Motor.Base.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Extension
{
    public static class Motor_Extension_Method
    {
        public static AxisBase GetAxisBase(this string axisName)
        {
            AxisBase axis = null;
            var resource = SolveWare.Core.MMgr.Get_Single_Resource_Item(ResourceProvider_Kind.Tool, typeof(Resource_Tool_Motor_Indicator));
            axis = (AxisBase)resource.Get_Single_Item(axisName);
            return axis;
        }
        public static double GetUnitPos(this string axisName)
        {
            var resource = SolveWare.Core.MMgr.Get_Single_Resource_Item(ResourceProvider_Kind.Tool, typeof(Resource_Tool_Motor_Indicator));
            AxisBase mtr = (AxisBase)resource.Get_Single_Item(axisName);
            double pos = mtr.Get_CurUnitPos();
            return pos;
        }
    }
}
