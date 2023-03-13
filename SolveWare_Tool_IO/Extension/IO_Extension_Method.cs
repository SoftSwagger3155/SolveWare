using SolveWare_Service_Core;
using SolveWare_Service_Core.Definitions;
using SolveWare_Tool_IO.Attributes;
using SolveWare_Tool_IO.Base.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Tool_IO.Extension
{
    public static class IO_Extension_Method
    {
        public static IOBase GetIOBase(this string ioName)
        {
            IOBase ioBase = null;
            var resource = SolveWare.Core.MMgr.Get_Single_Resource_Item(ResourceProvider_Kind.Tool, typeof(Resource_Tool_IO_Indicator));
            ioBase = (IOBase)resource.Get_Single_Item(ioName);

            return ioBase;
        }
    }
}
