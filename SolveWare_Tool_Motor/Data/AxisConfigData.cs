using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using SolveWare_Tool_Motor.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Data
{
    public class AxisConfigData : ElementModelBase
    {

        MtrTable mtrTable;
        MtrConfig mtrConfig;
        MtrSpeed home_MtrSpeed;
        MtrSpeed jog_MtrSpeed;
        MtrSpeed mtrSpeed;
        MtrSafe mtrSafe;
        MtrMisc mtrMisc;
        bool simulation = true;
        Master_Driver_Motor driver = Master_Driver_Motor.LeadSide_DMC3600;


        public string Description
        {
            get;
            set;
        }
        public bool Simulation { get; set; }

        public MtrTable MtrTable
        {
            get => mtrTable;
            set => UpdateProper(ref mtrTable, value);
        }
        public MtrConfig MtrConfig
        {
            get => mtrConfig;
            set => UpdateProper(ref mtrConfig, value);
        }
        public MtrSpeed MtrSpeed
        {
            get => mtrSpeed;
            set => UpdateProper(ref mtrSpeed, value);
        }
        public MtrSafe MtrSafe
        {
            get => mtrSafe;
            set => UpdateProper(ref mtrSafe, value);
        }
        public MtrMisc MtrMisc
        {
            get => mtrMisc;
            set => UpdateProper(ref mtrMisc, value);
        }
        public Master_Driver_Motor Driver
        {
            get => driver;
            set => UpdateProper(ref driver, value);
        }
        public MtrSpeed Home_MtrSpeed
        {
            get => home_MtrSpeed;
            set => UpdateProper(ref home_MtrSpeed, value);
        }
        public MtrSpeed Jog_MtrSpeed
        {
            get => jog_MtrSpeed;
            set => UpdateProper(ref jog_MtrSpeed, value);
        }

        public string DynamicStatus
        {
            get;
        }

        public AxisConfigData()
        {
            mtrTable = new MtrTable();
            mtrConfig = new MtrConfig();
            home_MtrSpeed = new MtrSpeed();
            jog_MtrSpeed = new MtrSpeed();
            mtrSpeed = new MtrSpeed();
            mtrSafe = new MtrSafe();
            mtrMisc = new MtrMisc();
        }

    }
}
