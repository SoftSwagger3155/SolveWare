using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Vision.Base.Abstract;
using SolveWare_Service_Vision.Base.Interface;
using SolveWare_Tool_Lighting.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Service_Vision.Business
{
    public class VisionJobSheet_Lighting : VisionJobSheetBase
    {
       private List<LightingParam> lightingParams = new List<LightingParam>();
       public List<LightingParam> LightingParams
        {
            get => lightingParams;
            set => lightingParams = value;
        }


        public override int DoJob()
        {
            OnEntrance();
            List<Task> tasks = new List<Task>();
            foreach (LightingParam param in this.LightingParams)
            {
                tasks.Add(Task.Run(() =>
                {
                    (param.Lighting as ILighting).Set_Intensity(param.Intensity);
                    Thread.Sleep(param.DelayTime);
                }));
            }
            Task.Factory.ContinueWhenAll(tasks.ToArray(), act =>
            {
                OnExit();
            });

            return ErrorCode;
        }
    }

    public class LightingParam
    {
        public IElement Lighting { get; set; }
        public int Intensity { get; set; }
        public int DelayTime { get; set; }
    }
}
