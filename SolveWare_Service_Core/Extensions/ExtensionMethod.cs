using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveWare_Service_Core.Extensions
{
    public static class ExtensionMethod
    {
        public static void OneNotchUp<TObj>(TObj obj, ref IList<TObj> objs, ref int selectedIndex)
        {
            int index = objs.ToList().FindIndex(x => x.Equals(obj));

            if (index <= 0)
            {
                selectedIndex = 0;
                return;
            }


            var temp = objs[index -1];

            objs[index - 1] = objs[index];
            objs[index] = temp;
            selectedIndex = index - 1;
        }
        public static void OneNotchDown<TObj>(TObj obj, ref IList<TObj> objs, ref int selectedIndex)
        {
            int index = objs.ToList().FindIndex(x => x.Equals(obj));

            if (index >= objs.Count - 1)
            {
                selectedIndex = objs.Count - 1;
                return;
            }


            var temp = objs[index +1];

            objs[index + 1] = objs[index];
            objs[index] = temp;
            selectedIndex = index + 1;
        }
    }
}
