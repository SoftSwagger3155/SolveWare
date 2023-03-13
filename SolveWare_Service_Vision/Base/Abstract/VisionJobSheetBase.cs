using SolveWare_Service_Core.Base.Abstract;
using SolveWare_Service_Vision.Base.Interface;
using SolveWare_Service_Vision.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SolveWare_Service_Vision.Base.Abstract
{
    [XmlInclude(typeof(VisionJobSheet_Lighting))]
    public abstract class VisionJobSheetBase : JobFundamentalBase, IVisionJobSheet
    {
        public abstract int DoJob();
    }
}
