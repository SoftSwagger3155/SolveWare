using HalconDotNet;
using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Tool_Camera.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Camera.Base.Abstract
{
    public abstract class CameraBase : ElementModelBase, ICamera
    {
        public string Id_Camera { get; set; }
        protected HImage image = new HImage();
        public HImage Image 
        { 
            get => image; 
            private set => image = value; 
        }
        public byte[] image_Buffer { get; }
        public int ExposureTime { get; set; }
        public int Gain { get; set; }
        public double FrameRate { get; set; }
        public long GrabTime { get; set; }

        public abstract void GetExposureTime();

        public abstract void GetFrameRate();

        public abstract int GrabImageOnce();

        public abstract int SetBrightness();

        public abstract int SetFrameRate();

        public abstract int StartLive(int delayTime_ms = 100);

        public abstract int StopLive(int delayTime__ms = 100);

        public abstract void CloseCamera();

        public abstract void AssingCamera(object obj_Camera);
    }
}
