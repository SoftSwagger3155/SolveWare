using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Base.Interface
{
    public interface IElement
    {
        string Name { get; set; }
        long Id { get; }
    }
}
