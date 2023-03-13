using SolveWare_Service_Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Manager.Base.Interface
{
    public interface IToolResourceProvider : IResourceProvider
    {
        IFactory Factory { get; }
        IList<IElement> WareHouse { get; }
    }
}
