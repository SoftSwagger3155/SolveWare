using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Core.Base.Interface;
using SolveWare_Service_Core.General;
using SolveWare_Tool_IO.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_IO.Data
{
    public class IOConfigData : ElementModelBase
    {
        string name;
        IOType ioType = IOType.Input;
        Master_Driver_IO ioMasterDriver = Master_Driver_IO.LeadSide_DMC3600;

        public IOConfigData()
        {
            if (Id == 0) Id = IdentityGenerator.IG.GetIdentity();
        }
        public long Id
        {
            get;
            set;
        }
        public string Name
        {
            get => name;
            set => UpdateProper(ref name, value);
        }
        public string Description
        {
            get;
            set;
        }
        public int CardNo { get; set; }
        public int Bit { get; set; }
        public int Logic { get; set; }
        public bool Simulation { get; set; }
        public bool IsForSelect { get; set; }
        public string DynamicStatus
        {
            get;
        }
        public IOType IOType
        {
            get => ioType;
            set => UpdateProper(ref ioType, value);
        }
        public Master_Driver_IO IOMasterDriver
        {
            get => ioMasterDriver;
            set => UpdateProper(ref ioMasterDriver, value);
        }
    }
}
