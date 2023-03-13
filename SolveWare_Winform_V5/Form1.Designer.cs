namespace SolveWare_Winform_V5
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.tab = new System.Windows.Forms.TabControl();
            this.tabPage_MtrTable = new System.Windows.Forms.TabPage();
            this.tabPage_MtrConfig = new System.Windows.Forms.TabPage();
            this.tabPage_MtrSpeed = new System.Windows.Forms.TabPage();
            this.tab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab
            // 
            this.tab.Controls.Add(this.tabPage_MtrTable);
            this.tab.Controls.Add(this.tabPage_MtrConfig);
            this.tab.Controls.Add(this.tabPage_MtrSpeed);
            this.tab.Location = new System.Drawing.Point(46, 21);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(900, 900);
            this.tab.TabIndex = 0;
            // 
            // tabPage_MtrTable
            // 
            this.tabPage_MtrTable.Location = new System.Drawing.Point(4, 25);
            this.tabPage_MtrTable.Name = "tabPage_MtrTable";
            this.tabPage_MtrTable.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MtrTable.Size = new System.Drawing.Size(892, 871);
            this.tabPage_MtrTable.TabIndex = 0;
            this.tabPage_MtrTable.Text = "Table";
            this.tabPage_MtrTable.UseVisualStyleBackColor = true;
            // 
            // tabPage_MtrConfig
            // 
            this.tabPage_MtrConfig.Location = new System.Drawing.Point(4, 25);
            this.tabPage_MtrConfig.Name = "tabPage_MtrConfig";
            this.tabPage_MtrConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MtrConfig.Size = new System.Drawing.Size(892, 971);
            this.tabPage_MtrConfig.TabIndex = 1;
            this.tabPage_MtrConfig.Text = "Config";
            this.tabPage_MtrConfig.UseVisualStyleBackColor = true;
            // 
            // tabPage_MtrSpeed
            // 
            this.tabPage_MtrSpeed.Location = new System.Drawing.Point(4, 25);
            this.tabPage_MtrSpeed.Name = "tabPage_MtrSpeed";
            this.tabPage_MtrSpeed.Size = new System.Drawing.Size(892, 971);
            this.tabPage_MtrSpeed.TabIndex = 2;
            this.tabPage_MtrSpeed.Text = "Speed";
            this.tabPage_MtrSpeed.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1900, 973);
            this.Controls.Add(this.tab);
            this.MaximumSize = new System.Drawing.Size(2060, 1020);
            this.MinimumSize = new System.Drawing.Size(1900, 1020);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tabPage_MtrTable;
        private System.Windows.Forms.TabPage tabPage_MtrConfig;
        private System.Windows.Forms.TabPage tabPage_MtrSpeed;
    }
}

