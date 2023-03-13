using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Base.Interface
{
    public interface IRESTFul
    {
        string FilePath { get; set; }
        bool AddSingleData(IElement data);
        bool DeleteSingleData(IElement data);
        IElement GetSingleData(string name);
        IElement GetSingleData(IElement IElementBase);
        IList<IElement> DataBase { get; set; }
        bool SaveSingleData(IElement data);
    }
}
