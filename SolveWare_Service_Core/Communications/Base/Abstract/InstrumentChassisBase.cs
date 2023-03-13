using SolveWare_Service_Core.Communications.Base.Interface;
using SolveWare_Service_Core.Communications.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Communications.Base.Abstract
{
    public enum InstrumentChassisArgsType
    {
        Turn_Off_Line,
        Turn_On_Line,
        Turn_On_Simnulation,
        Allocate_Chassis_Resouce,
        Enable_Simulation,
        Disable_Simulation,
    }
    public class InstrumentChassisArgs : EventArgs
    {
        public InstrumentChassisArgsType EventType { get; protected set; }
        public InstrumentChassisArgs(InstrumentChassisArgsType eventType) : base()
        {
            this.EventType = eventType;
        }
    }
    public delegate object InstrumentChassisEventHandler(object sender, InstrumentChassisArgs e);
    public class InstrumentChassisBase : IInstrumentChassis
    {

        protected readonly object mutex = new object();
        protected volatile bool _canAccess;
        Modbus _modbus;

        public event InstrumentChassisEventHandler InstrumentChassisEvent;

        public Modbus Modbus
        {
            get
            {
                if (_modbus == null)
                {
                    lock (mutex)
                    {
                        if (_modbus == null)
                        {
                            _modbus = new Modbus(this);
                        }
                    }
                }
                return _modbus;
            }
        }

        public virtual object Visa
        {
            get
            {
                return this;
            }
        }
        protected int _defaultBufferSize = 1024;
        public int DefaultBufferSize
        {
            get
            {
                return _defaultBufferSize;
            }
            set
            {
                _defaultBufferSize = value;
            }
        }
        public virtual int Timeout_ms
        {
            get;
            set;
        }
        //Action<ExpectedException, string, string> ErrorReportAction
        //{
        //    get;
        //    set;
        //}
        public string Resource { get; protected set; }
        public string Name { get; protected set; }
        public bool IsOnline { get; protected set; }
        //public InstrumentChassisBase(string name, string resource, bool isOnline, Action<string> errorReportAction)
        public InstrumentChassisBase(string name, string resource, bool isOnline/*, Action<ExpectedException, string, string> errorReportAction*/)
        {
            this.Name = name;
            this.Resource = resource;
            this.IsOnline = isOnline;
            //this.ErrorReportAction = errorReportAction;
        }


        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public virtual void Initialize(int timeout)
        {
            throw new NotImplementedException();
        }
        public virtual void WaitRuning()
        {
            throw new NotImplementedException();
        }
        protected virtual void OnTurnOffline()
        {
            if (this.InstrumentChassisEvent != null)
            {
                var ret = this.InstrumentChassisEvent(this, new InstrumentChassisArgs(InstrumentChassisArgsType.Turn_Off_Line));
            }
            this.IsOnline = false;
        }
        protected virtual void OnTurnOnline()
        {
            this.IsOnline = true;
            if (this.InstrumentChassisEvent != null)
            {
                var ret = this.InstrumentChassisEvent(this, new InstrumentChassisArgs(InstrumentChassisArgsType.Turn_On_Line));
            }

        }
        protected virtual void OnTurnOnSimulation()
        {
            this.IsOnline = true;
            if (this.InstrumentChassisEvent != null)
            {
                var ret = this.InstrumentChassisEvent(this, new InstrumentChassisArgs(InstrumentChassisArgsType.Turn_On_Simnulation));
            }

        }
        protected virtual void OnAllocateChassisResouce()
        {
            if (this.InstrumentChassisEvent != null)
            {
                var ret = this.InstrumentChassisEvent(this, new InstrumentChassisArgs(InstrumentChassisArgsType.Allocate_Chassis_Resouce));
            }

        }
        public virtual void Turn_On_Line(bool isOnline)
        {
            if (isOnline)
            {
                this.OnTurnOnline();
            }
            else
            {
                OnTurnOffline();
            }
        }
        public virtual void Turn_On_Simulation()
        {
            OnTurnOnSimulation();
        }


        public virtual bool CanAccess
        {
            get { return _canAccess; }
        }

        public virtual void ReportException(string errorMsg)
        {
            throw new NotImplementedException();
        }


        public virtual void ReportException(string errorMsg, Exception ex)
        {
            throw new NotImplementedException();
        }


        public virtual void FormattedLog_Global(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual void Log_Global(string log)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] Query(byte[] cmd, int delay_ms)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] Query(byte[] cmd, int bytesToRead, int delay_ms)
        {
            throw new NotImplementedException();
        }
        public virtual string Query(string cmd, int delay_ms)
        {
            throw new NotImplementedException();
        }
        public virtual void TryWrite(byte[] cmd)
        {
            throw new NotImplementedException();
        }
        public virtual void TryWrite(string cmd)
        {
            throw new NotImplementedException();
        }

        public virtual void ClearConnection()
        {
            throw new NotImplementedException();
        }
        public virtual void BuildConnection(int timeout_ms)
        {
            throw new NotImplementedException();
        }
        public virtual void ConnectToResource(int timeout_ms, bool forceOnline = false)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] QueryWithLongResponTime(byte[] cmd, int bytesToRead, int delay_ms, int respon_ms)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] QueryWithLongResponTime(byte[] cmd, int delay_ms, int respon_ms)
        {
            throw new NotImplementedException();
        }

        public virtual string QueryWithLongResponTime(string cmd, int delay_ms, int respon_ms)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadString()
        {
            throw new NotImplementedException();
        }
    }
}
