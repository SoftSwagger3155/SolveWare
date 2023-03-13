using SolveWare_Service_Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Manager.Base.Interface
{
    public interface IResourceProvider
    {
        Type ResourceKey { get; }
        void AssignResourceKey(Type key);
        string Name { get; set; }
        bool Initialize();
        void Save();
        bool Load();
        IElement Get_Single_Item(string name);
        IList<IElement> Get_All_Items();
        IList<string> Get_All_Item_Name();
    }
}
