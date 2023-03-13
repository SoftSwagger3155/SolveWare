using SolveWare_Service_Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Base.Abstract
{
    public class ElementModelBase : ModelBase, IElement
    {
        private string name;
        public string Name 
        { 
            get => name; 
            set => UpdateProperAction(ref name, value, ac: UpdateContent); 
        }

        public long Id
        {
            get;
            private set;
        }
    }
}
