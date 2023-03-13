using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Manager.Base.Interface;
using SolveWare_Tool_Motor.Base.Abstract;
using SolveWare_Tool_Motor.Data;
using SolveWare_Tool_Motor.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Business
{
    public class Factory_Motor : IFactory
    {
        public IElement BuildTool(IElement configData)
        {
            IElement tool = null;
            if (configData is AxisConfigData == false) return tool;

            AxisBase mtr = null;
            AxisConfigData config = (configData as AxisConfigData);
            switch (config.Driver)
            {
                case Master_Driver_Motor.LeadSide_DMC3600:
                    mtr = new Motor_DMC3600(configData);
                    tool = mtr;
                    break;
                case Master_Driver_Motor.LeadSide_SMC:
                    break;
                case Master_Driver_Motor.YangKong_MCC800P:
                    break;
                case Master_Driver_Motor.ACS:
                    break;
            }

            return tool;
        }
    }
}
