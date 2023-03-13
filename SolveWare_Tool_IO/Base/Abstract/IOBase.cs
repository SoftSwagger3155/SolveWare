using SolveWare_Service_Core.Base.Interface;
using SolveWare_Tool_IO.Base.Interface;
using SolveWare_Tool_IO.Data;
using SolveWare_Tool_IO.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_IO.Base.Abstract
{
    public abstract class IOBase : IIOBase, INotifyPropertyChanged
    {
        protected IElement configData;
        IOStatus status = IOStatus.On;
        IOType ioType = IOType.Input;
        #region ctor
        public IOBase(IElement data)
        {
            this.configData = data;
            this.Simulation = (data as IOConfigData).Simulation;
        }
        public void Setup(IElement configData)
        {
            this.configData = configData as IOConfigData;
        }
        #endregion

        public string Name
        {
            get;
            set;
        }
        public long Id
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public IOStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string DynamicStatus
        {
            get;
        }
        public IOType IOType
        {
            get => ioType;
            set
            {
                ioType = value;
                OnPropertyChanged(nameof(IOType));
            }
        }

        public bool Simulation
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        public bool IsOff()
        {
            return Status == IOStatus.Off;
        }
        public bool IsOn()
        {
            return Status == IOStatus.On;
        }
        public abstract void Off();
        public abstract void On();
        public abstract void UpdateStatus();
    }
}
