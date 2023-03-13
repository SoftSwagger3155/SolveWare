using SolveWare_Tool_Motor.Base.Abstract;
using SolveWare_Tool_Motor.Base.Interface;
using SolveWare_Tool_Motor.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Business
{
    public class SafeKeeper : ISafeKeeper, INotifyPropertyChanged
    {
        AxisBase motor;
        IList<AxisBase> motors;

        CancellationTokenSource cancelSource;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public AxisBase Motor
        {
            get => motor;
            set
            {
                motor = value;
                OnPropertyChanged(nameof(Motor));
            }
        }
        public SafeKeeper(AxisBase motor, IList<AxisBase> motors)
        {
            this.Motor = motor;
            this.motors = motors;
        }

        public void RunSafeKeeper()
        {
            if (cancelSource != null) return;
            cancelSource = new CancellationTokenSource();

            Task task = new Task(new Action(() =>
            {

                while (!cancelSource.Token.IsCancellationRequested)
                {
                    PositionDangerMonitoring();
                    ZoneMonitoring();
                    Thread.Sleep(10);
                }
            }), cancelSource.Token, TaskCreationOptions.LongRunning);

            task.Start();
        }
        public void StopSafeKeeper()
        {
            if (cancelSource == null) return;
            cancelSource.Cancel();
        }
        public bool IsPositionDangerous(PositionSafeData data)
        {
            if (motors.Count == 0) return false;
            var mtr = this.motors.ToList().Find(x => x.MtrTable.Name == data.ObservedAxisName);
            double curPos = 0;
            bool isDangerous = false;
            switch (data.SafeOperand)
            {
                case PositionSafetyOperand.GreaterThan:
                    curPos = mtr.Get_CurUnitPos();
                    isDangerous = curPos > data.ObservedPos;
                    data.IsDangerousToMove = isDangerous;
                    break;
                case PositionSafetyOperand.LowerThan:
                    isDangerous = curPos < data.ObservedPos;
                    data.IsDangerousToMove = isDangerous;
                    break;
            }

            return isDangerous;
        }
        private void PositionDangerMonitoring()
        {
            if (Motor.MtrSafe.PositionSafeDatas.Count == 0)
            {
                Motor.IsDangerousToMove = false;
                return;
            }

            bool isDangerous = false;
            foreach (var pData in motor.MtrSafe.PositionSafeDatas.ToList())
            {
                if (IsPositionDangerous(pData) == false) continue;


                isDangerous = true;
                break;
            }
            Motor.IsDangerousToMove = isDangerous;
        }
        private void ZoneMonitoring()
        {
            if (Motor.MtrSafe.ZoneSafeDatas.Count == 0)
            {
                Motor.IsInZone = false;
                return;
            }

            Motor.MtrSafe.ZoneSafeDatas.ToList().ForEach(x =>
            {
                var mtr = motors.ToList().Find(m => m.MtrTable.Name == x.ObservedZoneAxisName);
                double curPos = mtr.Get_CurUnitPos();

                x.IsInZone = curPos <= x.ObservedZoneMaxPos && curPos >= x.ObservedZoneMinPos;
                Motor.IsInZone = true;
            });
        }
        public bool CheckZoneSafeToMove(double pos)
        {
            var affectedZones = Motor.MtrSafe.ZoneSafeDatas.ToList().FindAll(x => x.IsInZone == true);
            ZoneSafeData foundData = null;

            foreach (var zData in affectedZones)
            {
                var mtr = motors.ToList().Find(m => m.MtrTable.Name == zData.ObservedZoneAxisName);
                double curPos = mtr.Get_CurUnitPos();

                if (curPos <= zData.ObservedZoneMaxPos && curPos >= zData.ObservedZoneMinPos)
                {
                    foundData = zData;
                    break;
                }
            }

            bool isSafe = false;
            isSafe = pos <= foundData.AllowableMaxPos && pos >= foundData.AllowableMinPos;
            return isSafe;
        }

        public bool MoveToObserverPos(PositionSafeData pSafeData)
        {
            AxisBase observedMotor = this.motors.ToList().Find(x => x.Name == pSafeData.ObservedAxisName);
            return observedMotor.MoveToSafeObservedPos(pSafeData.ObservedPos);
        }
    }
}
