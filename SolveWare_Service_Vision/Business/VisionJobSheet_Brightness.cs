using SolveWare_Service_Core;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Definitions;
using SolveWare_Service_Core.Manager.Base.Interface;
using SolveWare_Service_Vision.Base.Abstract;
using SolveWare_Tool_Camera.Attributes;
using SolveWare_Tool_Camera.Base.Abstract;
using SolveWare_Tool_Camera.Base.Interface;
using SolveWare_Tool_Lighting.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Service_Vision.Business
{
    public class VisionJobSheet_Brightness : VisionJobSheetBase
    {
        private BrightnessParam brightnessParams= new BrightnessParam();
        public BrightnessParam BrightnessParams
        {
            get { return brightnessParams; }
            set { brightnessParams = value; }
        }

        public override int DoJob()
        {
            OnEntrance();
            
            IResourceProvider provider = SolveWare.Core.MMgr.Get_Single_Resource_Item(ResourceProvider_Kind.Tool, typeof(Resource_Tool_Camera_Indicator)) ;
            CameraBase camera = (CameraBase)provider.Get_Single_Item(BrightnessParams.Camera_Name);
            camera.ExposureTime = BrightnessParams.ExposureTime;
            camera.Gain = BrightnessParams.Gain;
            camera.SetBrightness();

            OnExit();
            return ErrorCode;
        }
    }

    public class BrightnessParam
    {
        public string Camera_Name { get; set; }
        public int Gain { get; set; }
        public int ExposureTime { get; set; }
    }
}
