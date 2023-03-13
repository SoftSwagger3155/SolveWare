using SolveWare_Service_Core.Base.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Data
{
    public class MtrSpeed : ModelBase
    {

        // 专区
        [Category("Speed")]
        [DisplayName("Start Velocity 初速")]
        [Description("Start velocity")]
        public double Start_Velocity { get; set; }

        [Category("Speed")]
        [DisplayName("Max Velocity 最大速度")]
        [Description("Max velocity")]
        public double Max_Velocity { get; set; }

        [Category("Speed")]
        [DisplayName("Acceleration 加速度")]
        [Description("Acceleration")]
        public double Acceleration { get; set; }

        [Category("Speed")]
        [DisplayName("Deceleration 减速度")]
        [Description("Deceleration")]
        public double Deceleration { get; set; }

        [Category("Speed")]
        [DisplayName("Jerk 拉力")]
        [Description("Jerk")]
        public double Jerk { get; set; }


        #region ctor
        public MtrSpeed()
        {
            Start_Velocity = 0;
            Max_Velocity = 10;
            Acceleration = 0.1;
            Deceleration = 0.1;
            Jerk = 1;
        }
        #endregion
    }
}
