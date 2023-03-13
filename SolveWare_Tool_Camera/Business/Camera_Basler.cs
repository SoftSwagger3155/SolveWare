using Basler.Pylon;
using HalconDotNet;
using SolveWare_Service_Core;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using SolveWare_Tool_Camera.Base.Abstract;
using SolveWare_Tool_Camera.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Tool_Camera.Business
{
    public class Camera_Basler : CameraBase
    {
        Camera camera_Basler;
        Stopwatch stopwatch;
        private PixelDataConverter converter = new PixelDataConverter();
        private IntPtr latestFrameAddress = IntPtr.Zero;

        /// <summary>
        /// if >= Sfnc2_0_0,说明是ＵＳＢ３的相机
        /// </summary>
        static Version Sfnc2_0_0 = new Version(2, 0, 0);
        public  CameraConfigData ConfigData { get; set; }

        public Camera_Basler(IElement configData)
        {
            this.ConfigData = configData as CameraConfigData;
            stopwatch = new Stopwatch();
        }
        public override void AssingCamera(object obj_Camera)
        {
            this.camera_Basler = obj_Camera as Camera;
            string sn = this.camera_Basler.CameraInfo[CameraInfoKey.SerialNumber];
            string modelName = this.camera_Basler.CameraInfo[CameraInfoKey.ModelName];

            this.Id_Camera = $"[{sn}] {modelName}";
            this.ConfigData.Id_Camera = this.Id_Camera;
            this.Name = this.ConfigData.Name;
        }
       
        public override void CloseCamera()
        {
            if (camera_Basler == null) return;
            if (camera_Basler.IsOpen)
            {
                if (camera_Basler.StreamGrabber.IsGrabbing)
                    StopLive(200);

                camera_Basler.Close();
            }
        }

        public override void GetExposureTime()
        {
            if (camera_Basler == null) return;
            this.ExposureTime = (int)camera_Basler.Parameters[PLCamera.ExposureTime].GetValue();
        }

        public override void GetFrameRate()
        {
            if (camera_Basler == null) return;
            this.FrameRate = camera_Basler.Parameters[PLCamera.AcquisitionFrameRateAbs].GetValue();
        }

        public override int GrabImageOnce()
        {
            int errorCode = ErrorCodes.NoError;
            DateTime st = DateTime.Now;
            try
            {
                if (camera_Basler.StreamGrabber.IsGrabbing)
                    StopLive();
                camera_Basler.Parameters[PLCamera.AcquisitionMode].SetValue("SingleFrame");
                camera_Basler.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCodes.VisionFailed;
                SolveWare.Core.MMgr.Infohandler.LogExceptionMessage(ErrorCodes.GetErrorDescription(errorCode), ex, st);
            }

            return errorCode;
        }

        public override int SetBrightness()
        {
            camera_Basler.Parameters[PLCamera.ExposureTime].SetValue(this.ExposureTime);
            camera_Basler.Parameters[PLCamera.Gain].SetValue(this.Gain);

            return 0;
        }

        public override int SetFrameRate()
        {
            throw new NotImplementedException();
        }

        public override int StartLive(int delayTime_ms = 100)
        {
            throw new NotImplementedException();
        }

        public override int StopLive(int delayTime__ms = 100)
        {
            throw new NotImplementedException();
        }

        private void BaingEvent()
        {
            camera_Basler.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
            camera_Basler.StreamGrabber.ImageGrabbed += OnImageGrabbed;                      // 注册采集回调函数

            camera_Basler.ConnectionLost -= OnConnectionLost;
            camera_Basler.ConnectionLost += OnConnectionLost;
        }

        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                HObject ho_Image;
                if (grabResult.GrabSucceeded)
                {
                    GrabTime = stopwatch.ElapsedMilliseconds;
                    {
                        if (latestFrameAddress == IntPtr.Zero)
                        {
                            latestFrameAddress = Marshal.AllocHGlobal((Int32)grabResult.PayloadSize);
                        }
                        converter.OutputPixelFormat = PixelType.Mono8;
                        converter.Convert(latestFrameAddress, grabResult.PayloadSize, grabResult);
                        // 转换为Halcon图像显示
                        HOperatorSet.GenImage1(out ho_Image, "byte", (HTuple)grabResult.Width, (HTuple)grabResult.Height, (HTuple)latestFrameAddress);
                        this.image = ho_Image as HImage;
                    }
                }
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogExceptionMessage($"{this.Name} Grab Image Failed", ex, DateTime.Now);
            }
            finally
            {
                e.DisposeGrabResultIfClone();
            }
        }
        private void OnConnectionLost(Object sender, EventArgs e)
        {
            try
            {
                camera_Basler.Close();

                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        camera_Basler.Open();
                        if (camera_Basler.IsOpen)
                        {
                            //MessageBox.Show("已重新连接上UserID为“" + strUserID + "”的相机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        Thread.Sleep(200);
                    }
                    catch
                    {
                        //MessageBox.Show("请重新连接UserID为“" + strUserID + "”的相机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (camera_Basler == null)
                {
                    //MessageBox.Show("重连超时20s:未识别到UserID为“" + strUserID + "”的相机！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SetHeartBeatTime(5000);
                //imageWidth = camera.Parameters[PLCamera.Width].GetValue();               // 获取图像宽 
                //imageHeight = camera.Parameters[PLCamera.Height].GetValue();              // 获取图像高
                //camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;                      // 注册采集回调函数
                //camera.ConnectionLost += OnConnectionLost;
                BaingEvent();
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogExceptionMessage($"{this.Name} ReConnected Failed", ex, DateTime.Now);
            }
        }
        public void SetHeartBeatTime(long value)
        {
            try
            {
                // 判断是否是网口相机
                if (camera_Basler.GetSfncVersion() < Sfnc2_0_0)
                {
                    camera_Basler.Parameters[PLGigECamera.GevHeartbeatTimeout].SetValue(value);
                }
            }
            catch (Exception ex)
            {
                SolveWare.Core.MMgr.Infohandler.LogExceptionMessage($"{this.Name} Set HeartBeat Time Failed", ex, DateTime.Now);
            }
        }
    }
}
