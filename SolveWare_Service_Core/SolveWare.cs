using SolveWare_Service_Core.Manager.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core
{
    public class SolveWare
    {
        static SolveWare core;
        static object mutex = new object();
        IMainManager mmgr = null;

        public IMainManager MMgr => mmgr;

        private SolveWare()
        {

        }

        public static SolveWare Core
        {
            get
            {
                if (core == null)
                {
                    lock (mutex)
                    {
                        if (core == null)
                            core = new SolveWare();
                    }
                }

                return core;
            }
        }
    }
}
