using SolveWare_Server_Dll.LeadSide;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Tool_Motor.Base.Abstract;
using SolveWare_Tool_Motor.Base.Interface;
using SolveWare_Tool_Motor.Data;
using SolveWare_Tool_Motor.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Business
{
    public class Motor_DMC3600 : AxisBase
    {
        private volatile int curMovePos = 0;
        private static int CardID_InBit = -1;
        private static int CardInstalled = 0;
        AxisConfigData configData;

        #region ctor
        public Motor_DMC3600(IElement configData) : base(configData)
        {
            this.configData = (configData as AxisConfigData);
        }
        #endregion


        public override int Get_IO_sts()
        {
            int sts = 0;
            if (Simulation) return sts;

            sts = (int)Dll_DMC3600.dmc_axis_io_status((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo);
            return sts;
        }
        public override double Get_AnalogInputValue()
        {
            return 0;
        }
        public override double Get_CurPulse()
        {
            if (Simulation) return 0;
            double curpos = Dll_DMC3600.dmc_get_position((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo);
            return curpos;
        }
        public override double Get_CurUnitPos()
        {
            double position = 0;
            if (Simulation) return CurrentPhysicalPos;


            if (this.mtrTable.IsFormulaAxis)
            {
                double mm = CurrentPulse * mtrTable.UnitPerRevolution / mtrTable.PulsePerRevolution;
                position = FormulaCalc_AngleToUnit(mm);
            }
            else
            {
                position = CurrentPulse * mtrTable.UnitPerRevolution / mtrTable.PulsePerRevolution;
            }

            if (MtrTable.MotorRealDirectionState == DirectionState.Negative && MtrTable.MotorDisplayDirectionState == DirectionState.Positive ||
                MtrTable.MotorRealDirectionState == DirectionState.Positive && MtrTable.MotorDisplayDirectionState == DirectionState.Negative)
            {
                position *= -1;
            }

            return position;

        }
        public override bool Get_Alarm_Signal()
        {
            if (Simulation) return false;
            return (((IO_Status_DMC3600)Get_IO_sts() & IO_Status_DMC3600.ALM) == IO_Status_DMC3600.ALM);
        }
        public override bool Get_InPos_Signal()
        {
            if (Simulation) return false;
            return (((IO_Status_DMC3600)Get_IO_sts() & IO_Status_DMC3600.INP) == IO_Status_DMC3600.INP);
        }
        public override bool Get_NEL_Signal()
        {
            if (Simulation) return false;
            return (((IO_Status_DMC3600)Get_IO_sts() & IO_Status_DMC3600.NEL) == IO_Status_DMC3600.NEL);
        }
        public override bool Get_Origin_Signal()
        {
            if (Simulation) return false;
            return (((IO_Status_DMC3600)Get_IO_sts() & IO_Status_DMC3600.ORG) == IO_Status_DMC3600.ORG);
        }
        public override bool Get_PEL_Signal()
        {
            if (Simulation) return false;
            return (((IO_Status_DMC3600)Get_IO_sts() & IO_Status_DMC3600.PEL) == IO_Status_DMC3600.PEL);
        }
        public override bool Get_ServoStatus()
        {
            if (Simulation) return false;
            short status = Dll_DMC3600.dmc_read_sevon_pin((ushort)mtrTable.CardNo, (ushort)mtrTable.AxisNo);
            bool isOn = status == mtrTable.ServoOn_Logic;
            return isOn;
        }
        public override bool HomeMove()
        {
            bool isHomeSuccessful = false;
            string sErr = string.Empty;
            DateTime st = DateTime.Now;

            if (IsProhibitToHome()) { return false; }


            //HasHome = false;
            isStopReq = false;
            Set_Servo(false);
            Thread.Sleep(100);
            Set_Servo(true);


            isStopReq = false;

            if (Simulation)
            {
                MtrTable.NewPos = 0;
                double DistanceToMove = 0 - currentPulse;
                double EstimateTimeTaken = 0.01 * (Math.Abs(DistanceToMove) / configData.Home_MtrSpeed.Max_Velocity);
                MoveTo(0);
                return true;
            }

            //Use OnBoard Home
            if (MtrTable.OnBoardHome == 1)
            {
                //ORG logic : 0-低电平，1-高电平
                //Home Dir : 0-负向，1-正向
                //Home Mode: 看运动卡提供的回零方式
                //dmc_check_done：0：指定轴正在运行，1：指定轴已停止


                float StartVel = 0;
                float MaxVel = 0;
                double Acc = 0;
                double Dec = 0;

                int dir = MtrTable.MotorHomeDirectionState == DirectionState.Negative ? 0 : 1;

                ConverToMMPerSec(ref StartVel, ref MaxVel, ref Acc, ref Dec);
                Dll_DMC3600.dmc_set_homemode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)dir, 1, (ushort)MtrConfig.Home_mode, 0);
                Dll_DMC3600.dmc_set_home_pin_logic((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.ORG_Logic, 0);
                Dll_DMC3600.dmc_set_profile_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, StartVel, MaxVel, Acc, Dec, 0);
                Dll_DMC3600.dmc_home_move((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo);

                while (true)
                {
                    if (Dll_DMC3600.dmc_check_done((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo) == 1) { isHomeSuccessful = true; break; }
                    TimeSpan ts = DateTime.Now - st;
                    if (ts.TotalMilliseconds > MtrTable.HomeTimeOut) { isHomeSuccessful = false; break; }
                    Thread.Sleep(10);
                }

                if (isHomeSuccessful)
                    Dll_DMC3600.dmc_set_position((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, 0);

            }

            if (isHomeSuccessful)
            {
                if (HasHome == false) hasHome = true;
            }

            return isHomeSuccessful;
        }
        public override bool Init()
        {
            if (Simulation) return true;

            try
            {

                //short dmc_set_alm_mode(WORD CardNo, WORD axis, WORD enable, WORD alm_logic, WORD alm_action)
                //功 能：设置指定轴的 ALM 信号
                //参 数：CardNo 控制卡卡号
                //axis 指定轴号，取值范围：DMC3C00：0~11，DMC3800：0~7，Dll_DMC3600：0~5
                //DMC3400A：0~3
                //enable ALM 信号使能，0：禁止，1：允许
                //alm_logic ALM 信号的有效电平，0：低有效，1：高有效
                //alm_action ALM 信号的制动方式，0：立即停止（只支持该方式）
                //返回值：错误代码
                Dll_DMC3600.dmc_set_alm_mode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.Alm_Enable, (ushort)MtrConfig.Alm_Logic, 0);

                //short dmc_set_softlimit(WORD CardNo, WORD axis, WORD enable, WORD source_sel, WORD
                //SL_action, long N_limit, long P_limit)
                //功 能：设置软限位
                //参 数：CardNo 控制卡卡号
                //axis 指定轴号，取值范围：DMC3C00：0~11，DMC3800：0~7，Dll_DMC3600：0~5
                // DMC3400A：0~3
                // enable 使能状态，0：禁止，1：允许
                // source_sel 计数器选择，0：指令位置计数器，1：编码器计数器
                // SL_action 限位停止方式，0：减速停止，1：立即停止
                // N_limit 负限位位置，单位：pulse 
                // P_limit 正限位位置，单位：pulse
                //返回值：错误代码
                int nelPulse = (int)(MtrTable.MinDistance_SoftLimit * MtrTable.PulsePerRevolution / MtrTable.UnitPerRevolution);
                int pelPulse = (int)(MtrTable.MaxDistance_SoftLimit * MtrTable.PulsePerRevolution / MtrTable.UnitPerRevolution);
                int enableSoftLimint = MtrTable.Enable_SoftLimit ? 1 : 0;
                Dll_DMC3600.dmc_set_softlimit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)enableSoftLimint, 0, 0, nelPulse, pelPulse);

                Dll_DMC3600.dmc_set_ez_mode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)mtrConfig.EZ_Logic, 0, 0);
                Dll_DMC3600.dmc_set_inp_mode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.INP_Enable, (ushort)MtrConfig.INP_Logic);
                Dll_DMC3600.dmc_set_pulse_outmode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.Pulse_Mode);
                Dll_DMC3600.dmc_set_el_mode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.EL_Enable, (ushort)MtrConfig.EL_Logic, (ushort)MtrConfig.EL_Mode);
                Dll_DMC3600.dmc_set_home_pin_logic((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (ushort)MtrConfig.ORG_Logic, 0);
                Dll_DMC3600.dmc_set_counter_inmode((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, 3);


                //short dmc_set_axis_io_map(WORD CardNo, WORD Axis, WORD IoType, WORD MapIoType, WORD
                //MapIoIndex, double filter_time)
                //参 数：CardNo 控制卡卡号
                // Axis 指定轴号，取值范围：DMC3C00 / DMC3800：0~7，Dll_DMC3600：0~5, 
                //DMC3400A： 0~3
                // IoType 指定轴的 IO 信号类型：
                //0：正限位信号，AxisIoInMsg_PEL
                // 1：负限位信号，AxisIoInMsg_NEL
                // 2：原点信号，AxisIoInMsg_ORG
                //3：急停信号，AxisIoInMsg_EMG
                // 4：减速停止信号，AxisIoInMsg_DSTP
                // 5：伺服报警信号，AxisIoInMsg_ALM
                // 6：伺服准备信号，AxisIoInMsg_RDY（保留）
                // 7：伺服到位信号，AxisIoInMsg_INP
                // MapIoType 轴 IO 映射类型：
                //0：正限位输入端口，AxisIoInPort_PEL
                // 1：负限位输入端口，AxisIoInPort_NEL
                // 2：原点输入端口，AxisIoInPort_ORG
                // 3：伺服报警输入端口，AxisIoInPort_ALM
                // 4：伺服准备输入端口，AxisIoInPort_RDY
                // 5：伺服到位输入端口，AxisIoInPort_INP
                // 6：通用输入端口，AxisIoInPort_IO
                // MapIoIndex 轴 IO 映射索引号：
                //1）当轴 IO 映射类型设置为 6 时，此参数可设置为 0~15 整数，表
                //示该映射对应的具体通用输入端口号
                //2）当轴 IO 映射类型设置为 0~5 时，此参数可设置 0~7 整数，表示
                //该映射所对应的具体轴号
                //filter_time 轴 IO 信号滤波时间，单位：s
                // dmc_set_axis_io_map( 0 , 0 , 0 , 1 , 0 , 0 ) = 0
                // dmc_set_axis_io_map( 0 , 0 , 1 , 0 , 0 , 0 ) = 0
                // dmc_set_axis_io_map( 0 , 0 , 2 , 0 , 0 , 0 ) = 0

                ushort PEL_IOType = 0;
                ushort NEL_IOType = 1;
                ushort ORG_IOType = 2;

                ushort PEL_MapIO = (ushort)MtrConfig.PELConfig;
                ushort NEL_MapIO = (ushort)MtrConfig.MELConfig;
                ushort ORG_MapIO = (ushort)MtrConfig.ORGConfig;

                double filter = 0.0;
                Dll_DMC3600.dmc_set_axis_io_map((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, PEL_IOType, PEL_MapIO, (ushort)MtrTable.AxisNo, filter);
                Dll_DMC3600.dmc_set_axis_io_map((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, NEL_IOType, NEL_MapIO, (ushort)MtrTable.AxisNo, filter);
                Dll_DMC3600.dmc_set_axis_io_map((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, ORG_IOType, ORG_MapIO, (ushort)MtrTable.AxisNo, filter);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        object mutex = new object();
        public override bool DoAvoidDangerousPosAction()
        {
            lock (mutex)
            {
                foreach (var pData in MtrSafe.PositionSafeDatas.ToList())
                {
                    if (SafeKeeper.IsPositionDangerous(pData) == false) continue;

                    if (SafeKeeper.MoveToObserverPos(pData) == false)
                        return false;
                }
            }

            return true;
        }
        public override bool MoveToSafeObservedPos(double pos)
        {
            float startVel = 0;
            float maxVel = 0;
            double acc = 0;
            double dec = 0;
            ConverToMMPerSec(ref startVel, ref maxVel, ref acc, ref dec);


            double distanceToMove = pos - Get_CurUnitPos();
            double estimateTimeTaken = 0.01 * (Math.Abs(distanceToMove) / maxVel);
            pos = Math.Round(pos, 5, MidpointRounding.AwayFromZero);
            DateTime commmandStartTime = DateTime.Now;

            MtrTable.NewPos = pos;
            double targetPos = pos / mtrTable.UnitPerRevolution * mtrTable.PulsePerRevolution;
            if (Simulation)
            {
                TimeSpan ts = estimateTimeTaken < 0.5 ?
                                           TimeSpan.FromSeconds(0.5) :
                                           TimeSpan.FromSeconds(estimateTimeTaken);


                TimeSpan ts2 = DateTime.Now - commmandStartTime;
                double temppos = MtrTable.CurPos;
                double tempfliction = 1;
                while (ts2.TotalMilliseconds < ts.TotalMilliseconds)
                {
                    ts2 = DateTime.Now - commmandStartTime;
                    //tempfliction = ts2.TotalMilliseconds / ts.TotalMilliseconds;
                    //if (tempfliction > 1)
                    //    tempfliction = 1;

                    MtrTable.CurPos = temppos + tempfliction * distanceToMove;
                    Thread.Sleep(5);
                    if (isStopReq) break;
                }
                if (!isStopReq)
                {
                    MtrTable.CurPos = MtrTable.NewPos;
                    CurrentPhysicalPos = MtrTable.CurPos;
                }


                return true;
            }

            bool isMoveSuccessful = false;
            double factor = MtrTable.MotorRealDirectionState == DirectionState.Negative && MtrTable.MotorDisplayDirectionState == DirectionState.Positive ||
                            MtrTable.MotorRealDirectionState == DirectionState.Positive && MtrTable.MotorDisplayDirectionState == DirectionState.Negative ? -1 : 1;

            targetPos *= factor;
            Dll_DMC3600.dmc_set_profile_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, startVel, maxVel, acc, dec, 0);
            //0:相对位置，1:绝对位置
            Dll_DMC3600.dmc_pmove_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (int)targetPos, 1);

            while (true)
            {
                if (Dll_DMC3600.dmc_check_done((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo) == (short)1) { isMoveSuccessful = true; break; }
            }

            return isMoveSuccessful;
        }

        public override bool MoveTo(double pos, bool BypassDangerCheck = false, float slowFactor = 1)
        {
            bool isMoveSuccessful = false;
            DateTime st = DateTime.Now;

            double tempPos = 0;
            if (this.MtrTable.IsFormulaAxis)
            {
                tempPos = FormulaCalc_UnitToAngle(pos);
                pos = tempPos;
            }

            if (IsProhibitToMove()) return false;
            if (IsDangerousToMove)
            {
                if (DoAvoidDangerousPosAction() == false) return false;
                //if (IsDangerousToMove) return false;
            }
            if (IsZoneSafeToGo(pos) == false) return false;


            float startVel = 0;
            float maxVel = 0;
            double acc = 0;
            double dec = 0;
            ConverToMMPerSec(ref startVel, ref maxVel, ref acc, ref dec);

            startVel *= slowFactor;
            maxVel *= slowFactor;

            double distanceToMove = pos - Get_CurUnitPos();
            double estimateTimeTaken = 0.01 * (Math.Abs(distanceToMove) / maxVel);
            pos = Math.Round(pos, 5, MidpointRounding.AwayFromZero);
            DateTime commmandStartTime = DateTime.Now;

            MtrTable.NewPos = pos;
            double targetPos = pos / mtrTable.UnitPerRevolution * mtrTable.PulsePerRevolution;
            if (Simulation)
            {
                TimeSpan ts = estimateTimeTaken < 0.5 ?
                                           TimeSpan.FromSeconds(0.5) :
                                           TimeSpan.FromSeconds(estimateTimeTaken);


                TimeSpan ts2 = DateTime.Now - commmandStartTime;
                double temppos = MtrTable.CurPos;
                double tempfliction = 1;
                while (ts2.TotalMilliseconds < ts.TotalMilliseconds)
                {
                    ts2 = DateTime.Now - commmandStartTime;
                    //tempfliction = ts2.TotalMilliseconds / ts.TotalMilliseconds;
                    //if (tempfliction > 1)
                    //    tempfliction = 1;

                    MtrTable.CurPos = temppos + tempfliction * distanceToMove;
                    Thread.Sleep(5);
                    if (isStopReq) break;
                }
                if (!isStopReq)
                {
                    MtrTable.CurPos = MtrTable.NewPos;
                    CurrentPhysicalPos = MtrTable.CurPos;
                }


                return true;
            }


            double factor = MtrTable.MotorRealDirectionState == DirectionState.Negative && MtrTable.MotorDisplayDirectionState == DirectionState.Positive ||
                            MtrTable.MotorRealDirectionState == DirectionState.Positive && MtrTable.MotorDisplayDirectionState == DirectionState.Negative ? -1 : 1;

            targetPos *= factor;
            Dll_DMC3600.dmc_set_profile_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, startVel, maxVel, acc, dec, 0);
            //0:相对位置，1:绝对位置
            Dll_DMC3600.dmc_pmove_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (int)targetPos, 1);

            while (true)
            {
                if (Dll_DMC3600.dmc_check_done((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo) == (short)1) { isMoveSuccessful = true; break; }
                TimeSpan ts = DateTime.Now - st;
                if (ts.TotalMilliseconds > MtrTable.MotionTimeOut) { isMoveSuccessful = false; break; }
            }

            return isMoveSuccessful;
        }
        public override bool ManualMoveTo(double pos, float slowFactor = 1)
        {
            bool isMoveSuccessful = false;
            DateTime st = DateTime.Now;

            double tempPos = 0;
            if (this.MtrTable.IsFormulaAxis)
            {
                tempPos = FormulaCalc_UnitToAngle(pos);
                pos = tempPos;
            }
            if (IsIgnoreDanger == false && IsDangerousToMove) return false;
            if (IsProhibitToMove()) return false;
            if (IsZoneSafeToGo(pos) == false) return false;


            float startVel = 0;
            float maxVel = 0;
            double acc = 0;
            double dec = 0;
            ConverToMMPerSec(ref startVel, ref maxVel, ref acc, ref dec);

            startVel *= slowFactor;
            maxVel *= slowFactor;

            double distanceToMove = pos - Get_CurUnitPos();
            double estimateTimeTaken = 0.01 * (Math.Abs(distanceToMove) / maxVel);
            pos = Math.Round(pos, 5, MidpointRounding.AwayFromZero);
            DateTime commmandStartTime = DateTime.Now;

            MtrTable.NewPos = pos;
            double targetPos = pos / mtrTable.UnitPerRevolution * mtrTable.PulsePerRevolution;
            if (Simulation)
            {
                TimeSpan ts = estimateTimeTaken < 0.5 ?
                                           TimeSpan.FromSeconds(0.5) :
                                           TimeSpan.FromSeconds(estimateTimeTaken);


                TimeSpan ts2 = DateTime.Now - commmandStartTime;
                double temppos = MtrTable.CurPos;
                double tempfliction = 1;
                while (ts2.TotalMilliseconds < ts.TotalMilliseconds)
                {
                    ts2 = DateTime.Now - commmandStartTime;
                    //tempfliction = ts2.TotalMilliseconds / ts.TotalMilliseconds;
                    //if (tempfliction > 1)
                    //    tempfliction = 1;

                    MtrTable.CurPos = temppos + tempfliction * distanceToMove;
                    Thread.Sleep(5);
                    if (isStopReq) break;
                }
                if (!isStopReq)
                {
                    MtrTable.CurPos = MtrTable.NewPos;
                    CurrentPhysicalPos = MtrTable.CurPos;
                }


                return true;
            }


            double factor = MtrTable.MotorRealDirectionState == DirectionState.Negative && MtrTable.MotorDisplayDirectionState == DirectionState.Positive ||
                            MtrTable.MotorRealDirectionState == DirectionState.Positive && MtrTable.MotorDisplayDirectionState == DirectionState.Negative ? -1 : 1;

            targetPos *= factor;
            Dll_DMC3600.dmc_set_profile_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, startVel, maxVel, acc, dec, 0);
            //0:相对位置，1:绝对位置
            Dll_DMC3600.dmc_pmove_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (int)targetPos, 1);

            while (true)
            {
                if (Dll_DMC3600.dmc_check_done((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo) == (short)1) { isMoveSuccessful = true; break; }
                TimeSpan ts = DateTime.Now - st;
                if (ts.TotalMilliseconds > MtrTable.MotionTimeOut) { isMoveSuccessful = false; break; }
            }

            return isMoveSuccessful;
        }
        public override void Jog(bool isPositive)
        {
            float minVel = 0;
            float maxVel = 0;
            double acc = 0;
            double dec = 0;
            ushort dir = isPositive ? (ushort)1 : (ushort)0;
            ConverToMMPerSec(ref minVel, ref maxVel, ref acc, ref dec);

            Dll_DMC3600.dmc_set_profile_unit((ushort)this.MtrTable.CardNo, (ushort)this.MtrTable.AxisNo, minVel, maxVel, acc, dec, 0);
            Dll_DMC3600.dmc_vmove((ushort)this.MtrTable.CardNo, (ushort)this.MtrTable.AxisNo, dir);


            Task.Run(() =>
            {
                while (true)
                {
                    if (IsDangerousToMove) { Stop(); break; }
                    if (IsInZone && !IsZoneSafeToGo(currentPhysicalPos)) { Stop(); break; }
                    if (Dll_DMC3600.dmc_check_done((ushort)this.MtrTable.CardNo, (ushort)this.MtrTable.AxisNo) == 1) { Stop(); break; }
                    if (isStopReq)
                    {
                        Stop(); break;
                    }
                    Thread.Sleep(50);
                }
            });


        }
        public override void Set_Servo(bool Status)
        {

            if (Simulation)
            {
                IsServoOn = Status;
                return;
            }

            //dmc_write_sevon_pin(WORD CardNo, WORD axis, WORD on_off)
            //功 能：控制指定轴的伺服使能端口的输出
            //参 数：CardNo 控制卡卡号
            //axis 指定轴号，取值范围：DMC3C00：0~11，DMC3800：0~7，Dll_DMC3600：0~5
            // DMC3400A：0~3
            //on_off 设置伺服使能端口电平，0：低电平，1：高电平
            //返回值：错误代码
            ushort on_off = 0;
            if (mtrTable.ServoOn_Logic == 0)
            {
                on_off = Status ? (ushort)0 : (ushort)1;
            }
            else
            {
                on_off = Status ? (ushort)1 : (ushort)0;
            }
            Dll_DMC3600.dmc_write_sevon_pin((ushort)mtrTable.CardNo, (ushort)mtrTable.AxisNo, on_off);
        }
        public override void Stop()
        {
            isStopReq = true;
            if (Simulation) return;

            Dll_DMC3600.dmc_stop((ushort)mtrTable.CardNo, (ushort)mtrTable.AxisNo, (ushort)mtrConfig.SD_Mode);
        }
        private bool IsZoneSafeToGo(double pos)
        {
            if (this.IsInZone == false) return true;

            var foundZone = this.MtrSafe.ZoneSafeDatas.ToList().Find(x => x.IsInZone == true);
            if (pos < foundZone.AllowableMinPos || pos > foundZone.AllowableMaxPos) return false;

            return true;
        }

        public override bool MoveToAndStopByIO(double pos, Func<bool> StopAction, bool BypassDangerCheck = false, float slowFactor = 1)
        {
            bool isMoveSuccessful = false;
            DateTime st = DateTime.Now;

            double tempPos = 0;
            if (this.MtrTable.IsFormulaAxis)
            {
                tempPos = FormulaCalc_UnitToAngle(pos);
                pos = tempPos;
            }

            if (IsProhibitToMove()) return false;
            if (IsDangerousToMove)
            {
                if (DoAvoidDangerousPosAction() == false) return false;
                //if (IsDangerousToMove) return false;
            }
            if (IsZoneSafeToGo(pos) == false) return false;


            float startVel = 0;
            float maxVel = 0;
            double acc = 0;
            double dec = 0;
            ConverToMMPerSec(ref startVel, ref maxVel, ref acc, ref dec);

            startVel *= slowFactor;
            maxVel *= slowFactor;

            double distanceToMove = pos - Get_CurUnitPos();
            double estimateTimeTaken = 0.01 * (Math.Abs(distanceToMove) / maxVel);
            pos = Math.Round(pos, 5, MidpointRounding.AwayFromZero);
            DateTime commmandStartTime = DateTime.Now;

            MtrTable.NewPos = pos;
            double targetPos = pos / mtrTable.UnitPerRevolution * mtrTable.PulsePerRevolution;
            if (Simulation)
            {
                TimeSpan ts = estimateTimeTaken < 0.5 ?
                                           TimeSpan.FromSeconds(0.5) :
                                           TimeSpan.FromSeconds(estimateTimeTaken);


                TimeSpan ts2 = DateTime.Now - commmandStartTime;
                double temppos = MtrTable.CurPos;
                double tempfliction = 1;
                while (ts2.TotalMilliseconds < ts.TotalMilliseconds)
                {
                    ts2 = DateTime.Now - commmandStartTime;
                    //tempfliction = ts2.TotalMilliseconds / ts.TotalMilliseconds;
                    //if (tempfliction > 1)
                    //    tempfliction = 1;

                    MtrTable.CurPos = temppos + tempfliction * distanceToMove;
                    Thread.Sleep(5);
                    if (isStopReq) break;
                }
                if (!isStopReq)
                {
                    MtrTable.CurPos = MtrTable.NewPos;
                    CurrentPhysicalPos = MtrTable.CurPos;
                }


                return true;
            }


            double factor = MtrTable.MotorRealDirectionState == DirectionState.Negative && MtrTable.MotorDisplayDirectionState == DirectionState.Positive ||
                            MtrTable.MotorRealDirectionState == DirectionState.Positive && MtrTable.MotorDisplayDirectionState == DirectionState.Negative ? -1 : 1;

            targetPos *= factor;
            Dll_DMC3600.dmc_set_profile_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, startVel, maxVel, acc, dec, 0);
            //0:相对位置，1:绝对位置
            Dll_DMC3600.dmc_pmove_unit((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo, (int)targetPos, 1);

            while (true)
            {
                if (StopAction != null)
                {
                    if (StopAction())
                    {
                        Stop();
                        isMoveSuccessful = true;
                        break;
                    }
                }
                if (Dll_DMC3600.dmc_check_done((ushort)MtrTable.CardNo, (ushort)MtrTable.AxisNo) == (short)1) { isMoveSuccessful = true; break; }
                TimeSpan ts = DateTime.Now - st;
                if (ts.TotalMilliseconds > MtrTable.MotionTimeOut) { isMoveSuccessful = false; break; }
            }

            return isMoveSuccessful;
        }
    }
}
