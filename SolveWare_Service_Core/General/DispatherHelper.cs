using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SolveWare_Service_Core.General
{
    public class DispatherHelper
    {
        private static DispatherHelper instance;
        private static object mutex = new object();

        private DispatherHelper()
        {

        }

        public static DispatherHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mutex)
                    {
                        instance = new DispatherHelper();
                    }
                }

                return instance;
            }
        }


        public void DoEvent(Action action, DispatcherObject dispObj)
        {
            dispObj.Dispatcher.BeginInvoke(action, DispatcherPriority.Normal, null);
        }
    }
}
