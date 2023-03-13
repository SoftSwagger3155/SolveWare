using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_Motor.Data
{
    public class MtrSafe : ModelBase
    {
        public MtrSafe()
        {
            this.PositionSafeDatas = new ObservableCollection<PositionSafeData>();
            this.ZoneSafeDatas = new ObservableCollection<ZoneSafeData>();
        }

        private bool isPositionDataUpated;
        public bool IsPositionDataUpated
        {
            get => isPositionDataUpated;
            set => UpdateProper(ref isPositionDataUpated, value);
        }

        private bool isZoneDataUpated;
        public bool IsZoneDataUpated
        {
            get => isZoneDataUpated;
            set => UpdateProper(ref isZoneDataUpated, value);
        }
        public ObservableCollection<PositionSafeData> PositionSafeDatas { get; set; }
        public ObservableCollection<ZoneSafeData> ZoneSafeDatas { get; set; }

        public void NotifyUpdate()
        {
            OnPropertyChanged(nameof(IsPositionDataUpated));
        }
    }

    public abstract class SafetyDataBase : ModelBase, IElement
    {
        protected string name;
        protected string description;
        protected bool isSelected;


        public SafetyDataBase()
        {
            if (this.Id == 0) this.Id = IdentityGenerator.IG.GetIdentity();
        }


        public string Name
        {
            get => name;
            set => UpdateProper(ref name, value);
        }
        public string Description
        {
            get => description;
            set => UpdateProper(ref description, value);
        }
        public long Id
        {
            get;
            set;
        }
        public bool IsSelected
        {
            get => isSelected;
            set => UpdateProper(ref isSelected, value);
        }

        public string WatchingAxisName { get; set; }

    }
    public class PositionSafeData : SafetyDataBase
    {
        private string observedAxisName;
        private double observedPos;
        private bool isDangerousToMove;
        private PositionSafetyOperand safeOperand;

        public PositionSafeData()
        {

        }
        public string ObservedAxisName
        {
            get => observedAxisName;
            set { UpdateProper(ref observedAxisName, value); FormateDescription(); }
        }
        public double ObservedPos
        {
            get => observedPos;
            set { UpdateProper(ref observedPos, value); FormateDescription(); }
        }
        public bool IsDangerousToMove
        {
            get => isDangerousToMove;
            set => UpdateProper(ref isDangerousToMove, value);
        }
        public PositionSafetyOperand SafeOperand
        {
            get => safeOperand;
            set { UpdateProper(ref safeOperand, value); FormateDescription(); }
        }

        private void FormateDescription()
        {
            this.Description = $"{this.ObservedAxisName} {SafeOperand}  {ObservedPos}";
        }
    }
    public class ZoneSafeData : SafetyDataBase
    {
        private bool isInZone;
        private string observedZoneAxisName;
        private double observedZoneMinPos;
        private double observedZoneMaxPos;
        private double allowableMinPos;
        private double allowableMaxPos;
        private double allowableAxisName;

        public bool IsInZone
        {
            get => isInZone;
            set { UpdateProper(ref isInZone, value); FormateDescription(); }
        }
        public string ObservedZoneAxisName
        {
            get => observedZoneAxisName;
            set { UpdateProper(ref observedZoneAxisName, value); FormateDescription(); }
        }
        public double ObservedZoneMinPos
        {
            get => observedZoneMinPos;
            set { UpdateProper(ref observedZoneMinPos, value); FormateDescription(); }
        }
        public double ObservedZoneMaxPos
        {
            get => observedZoneMaxPos;
            set { UpdateProper(ref observedZoneMaxPos, value); FormateDescription(); }
        }
        public double AllowableMinPos
        {
            get => allowableMinPos;
            set { UpdateProper(ref allowableMinPos, value); FormateDescription(); }
        }
        public double AllowableMaxPos
        {
            get => allowableMaxPos;
            set { UpdateProper(ref allowableMaxPos, value); FormateDescription(); }
        }
        public double AllowableAxisName
        {
            get => allowableAxisName;
            set { UpdateProper(ref allowableAxisName, value); FormateDescription(); }
        }

        private void FormateDescription()
        {
            this.Description = $"{this.ObservedZoneAxisName} From {ObservedZoneMinPos} To {ObservedZoneMaxPos} Allow From {AllowableMinPos} To {AllowableMaxPos}";
        }
    }

    public enum PositionSafetyOperand
    {
        GreaterThan,
        LowerThan
    }
}
