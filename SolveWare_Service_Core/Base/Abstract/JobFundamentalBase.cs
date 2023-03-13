using SolveWare_Service_Core.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SolveWare_Service_Core.Base.Abstract
{
    public abstract class JobFundamentalBase : ElementModelBase
    {
        protected int priority;
        protected int errorCode = 0;
        protected DateTime st = DateTime.Now;
        protected JobStatus status = JobStatus.Unknown;


        public int Priority
        {
            get => priority;
            set => UpdateProper(ref priority, value);
        }
        public int ErrorCode
        {
            get => errorCode;
            set => UpdateProper(ref errorCode, value);
        }

        public JobStatus Status
        {
            get => status;
            set => UpdateProper(ref status, value);
        }


        protected void LogActionMessage()
        {
            //MainManager.Core.Infohandler.LogActionMessage
        }

        protected void OnEntrance()
        {
            this.st = DateTime.Now;
            this.Status = JobStatus.Entrance;
        }
        public void OnExit()
        {
            this.Status = ErrorCode == 0 ? JobStatus.Done : JobStatus.Fail;
            SolveWare.Core.MMgr.Infohandler.LogActionMessage(this.Name, this.GetType().Name, st, errorCode);
        }
    }
}
