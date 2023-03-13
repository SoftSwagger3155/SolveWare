using SolveWare_Service_Core.Base.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolveWare_Winform_V5
{
    public partial class Form_PopUp_Message : Form, IView
    {
        public Form_PopUp_Message()
        {
            InitializeComponent();
        }

        public void Setup<TData>(TData data)
        {
            List<string> temp = data as List<string>;

            this.lstBox.BeginUpdate();
            temp.ForEach(x => this.lstBox.Items.Add(x));
            this.lstBox.EndUpdate();
        }
    }
}
