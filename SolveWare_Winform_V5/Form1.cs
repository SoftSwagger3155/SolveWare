using SolveWare_Service_Core.Extensions;
using SolveWare_Tool_Motor.Business;
using SolveWare_Tool_Motor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolveWare_Winform_V5
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //propertyGrid.PropertySort = PropertySort.Categorized;
            //propertyGrid.SelectedObject = new Student();


            //ListBox lsv = new ListBox();
            //lsv.Width = 200;
            //lsv.Height = 300;

            //lsv.BeginUpdate();
            //lsv.Location = new Point(300, 100);
            //lsv.Items.Add($"讯息");
            //for (int i = 0; i < 100; i++)
            //{
            //    //ListViewItem viewItem = new ListViewItem();
            //    //viewItem.Text = $"次数 {i + 1}";

            //    //viewItem.SubItems.Add($"SubItem次数 {i + 1}");
            //    //lsv.Items.Add(viewItem);
            //    lsv.Items.Add($"次数 {i + 1}");
            //}
            //lsv.EndUpdate();
            //lsv.SelectedItem = lsv.Items[lsv.Items.Count-1];

            //this.Controls.Add(lsv);
            //Form formMes = new Form();
            //formMes.Controls.Add(lsv);
            //formMes.Show();


            //IList<string> sequence = new List<string>
            //{
            //    "AAA",
            //    "BBB",
            //    "CCC"
            //};

            //string secondStr = "BBB";
            //int selectedIndex = -1;
            //ExtensionMethod.OneNotchUp(secondStr, ref sequence, ref selectedIndex);
            //List<string> check = sequence.ToList();
            //string checkStr = string.Empty;
            //check.ForEach(x => checkStr += x);
            //MessageBox.Show($"List {checkStr} | Index {selectedIndex}");

            InitPropertyGrid();
            
        }
        private void InitPropertyGrid()
        {
            Motor_DMC3600 mtr = new Motor_DMC3600(new AxisConfigData());

            //Table
            PropertyGrid grid_Table = new PropertyGrid();
            grid_Table.SelectedObject = mtr.MtrTable;
            grid_Table.Width = tabPage_MtrTable.Width;
            grid_Table.Height = tabPage_MtrTable.Height;
            grid_Table.PropertySort = PropertySort.Categorized;
            tabPage_MtrTable.Controls.Add(grid_Table);

            //Speed
            PropertyGrid grid_Speed = new PropertyGrid();
            grid_Speed.SelectedObject = mtr.AutoMtrSpeed;
            grid_Speed.Width = tabPage_MtrSpeed.Width;
            grid_Speed.Height = tabPage_MtrSpeed.Height;
            grid_Speed.PropertySort = PropertySort.Categorized;
            tabPage_MtrSpeed.Controls.Add(grid_Speed);

            //Config
            PropertyGrid grid_Config = new PropertyGrid();
            grid_Config.SelectedObject = mtr.MtrConfig;
            grid_Config.Width = tabPage_MtrConfig.Width;
            grid_Config.Height = tabPage_MtrConfig.Height;
            grid_Config.PropertySort = PropertySort.Categorized;
            tabPage_MtrConfig.Controls.Add(grid_Config);
        }

    }

}
