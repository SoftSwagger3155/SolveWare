using Basler.Pylon;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.Manager.Base.Interface;
using SolveWare_Tool_Camera.Base.Abstract;
using SolveWare_Tool_Camera.Data;
using SolveWare_Tool_Camera.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Camera.Business
{
    public class Factory_Camera : IFactory
    {
        int index_Basler = -1;
        int index_JAI = -1;
        int index_DaHehng = -1;
        int index_PointGrey =-1;

        public IElement BuildTool(IElement configData)
        {
            IElement cameraBase = null;
            CameraConfigData config = configData as CameraConfigData;

            switch (config.MasterDriver)
            {
                case Master_Driver_Camera.Basler:
                    Camera camera;
                    cameraBase = new Camera_Basler(cameraBase);
                    index_Basler++;

                    List<ICameraInfo> allCameraInfos = CameraFinder.Enumerate();
                    ICameraInfo iCamInfo = allCameraInfos[index_Basler];
                    camera = new Basler.Pylon.Camera(iCamInfo);
                    (cameraBase as Camera_Basler).AssingCamera(camera);
                  
                    break;

                default:
                    break;
            }

            return cameraBase;
        }
    }
}
