using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolveWare_Service_Core.Manager.Base.Interface
{
    public interface IMachineUI
    {
        TabControl Tabs { get; set; }
        StatusBar InstrumentBar { get; set; }
        StatusBar AxisStatusBar { get; set; }
    }
}
