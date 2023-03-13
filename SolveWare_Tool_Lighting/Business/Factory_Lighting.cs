using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Communications.Base.Interface;
using SolveWare_Service_Core.Communications.Business;
using SolveWare_Service_Core.Manager.Base.Interface;
using SolveWare_Tool_Lighting.Base.Interface;
using SolveWare_Tool_Lighting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Lighting.Business
{
    public class Factory_Lighting : IFactory
    {
        public IElement BuildTool(IElement configData)
        {
            IElement lighting = null;
            LightingConfigData config = configData as LightingConfigData;

            switch (config.Communication_Kind)
            {
                case Definitions.Lighting_Communication_Kind.TCPIP:
                    IInstrumentChassis tcp_Chassis = new EthernetChassis(config.Name, config.Resource, config.IsOnLine);
                    ILighting tcp_Lighting = new LightingDevice();
                    tcp_Lighting.Setup(tcp_Chassis, config.Channel);

                    lighting = tcp_Lighting;
                    break;
                case Definitions.Lighting_Communication_Kind.SerialPort:
                    IInstrumentChassis port_Chassis = new SerialPortChassis(config.Name, config.Resource, config.IsOnLine);
                    ILighting port_Lighting = new LightingDevice();
                    port_Lighting.Setup(port_Chassis, config.Channel);

                    lighting = port_Lighting;
                    break;
            }


            return lighting;
        }
    }
}
