namespace AprGBemu
{
    partial class AprGBemu_MainUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AprGBemu_MainUI));
            this.UI_OptionMenu = new System.Windows.Forms.MenuStrip();
            this.UI_openRomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UI_Configure = new System.Windows.Forms.ToolStripMenuItem();
            this.UI_LCD_panel = new System.Windows.Forms.Panel();
            this.APP_VER = new System.Windows.Forms.Label();
            this.FPS_inf = new System.Windows.Forms.Label();
            this.FPS_timer = new System.Windows.Forms.Timer(this.components);
            this.UI_AppName = new System.Windows.Forms.Label();
            this.UI_Restart_btn = new System.Windows.Forms.PictureBox();
            this.UI_Close_hide = new System.Windows.Forms.PictureBox();
            this.UI_Close_btn = new System.Windows.Forms.PictureBox();
            this.InfoBox = new System.Windows.Forms.PictureBox();
            this.UI_OptionMenu.SuspendLayout();
            this.UI_LCD_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Restart_btn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Close_hide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Close_btn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InfoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // UI_OptionMenu
            // 
            this.UI_OptionMenu.AutoSize = false;
            this.UI_OptionMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.UI_OptionMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.UI_OptionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UI_openRomMenuItem,
            this.UI_Configure});
            this.UI_OptionMenu.Location = new System.Drawing.Point(12, 7);
            this.UI_OptionMenu.Name = "UI_OptionMenu";
            this.UI_OptionMenu.Padding = new System.Windows.Forms.Padding(0);
            this.UI_OptionMenu.Size = new System.Drawing.Size(120, 24);
            this.UI_OptionMenu.TabIndex = 1;
            this.UI_OptionMenu.Text = "menuStrip1";
            this.UI_OptionMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseDown);
            this.UI_OptionMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseMove);
            // 
            // UI_openRomMenuItem
            // 
            this.UI_openRomMenuItem.AutoSize = false;
            this.UI_openRomMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(169)))), ((int)(((byte)(184)))));
            this.UI_openRomMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UI_openRomMenuItem.Font = new System.Drawing.Font("Microsoft JhengHei", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.UI_openRomMenuItem.ForeColor = System.Drawing.Color.White;
            this.UI_openRomMenuItem.Name = "UI_openRomMenuItem";
            this.UI_openRomMenuItem.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.UI_openRomMenuItem.Size = new System.Drawing.Size(60, 24);
            this.UI_openRomMenuItem.Text = "ROM";
            this.UI_openRomMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.UI_openRomMenuItem.Click += new System.EventHandler(this.UI_OpenROM_select);
            // 
            // UI_Configure
            // 
            this.UI_Configure.AutoSize = false;
            this.UI_Configure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(153)))), ((int)(((byte)(204)))));
            this.UI_Configure.Font = new System.Drawing.Font("Microsoft JhengHei", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.UI_Configure.ForeColor = System.Drawing.Color.White;
            this.UI_Configure.Name = "UI_Configure";
            this.UI_Configure.Size = new System.Drawing.Size(60, 24);
            this.UI_Configure.Text = "Setting";
            this.UI_Configure.Click += new System.EventHandler(this.Configure_Click);
            // 
            // UI_LCD_panel
            // 
            this.UI_LCD_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.UI_LCD_panel.Controls.Add(this.APP_VER);
            this.UI_LCD_panel.Font = new System.Drawing.Font("Microsoft JhengHei", 6F, System.Drawing.FontStyle.Bold);
            this.UI_LCD_panel.Location = new System.Drawing.Point(12, 38);
            this.UI_LCD_panel.Name = "UI_LCD_panel";
            this.UI_LCD_panel.Size = new System.Drawing.Size(256, 224);
            this.UI_LCD_panel.TabIndex = 3;
            // 
            // APP_VER
            // 
            this.APP_VER.AutoSize = true;
            this.APP_VER.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.APP_VER.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.APP_VER.Location = new System.Drawing.Point(0, 0);
            this.APP_VER.Name = "APP_VER";
            this.APP_VER.Size = new System.Drawing.Size(0, 16);
            this.APP_VER.TabIndex = 0;
            // 
            // FPS_inf
            // 
            this.FPS_inf.AutoSize = true;
            this.FPS_inf.Font = new System.Drawing.Font("Microsoft JhengHei", 8F, System.Drawing.FontStyle.Bold);
            this.FPS_inf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.FPS_inf.Location = new System.Drawing.Point(214, 270);
            this.FPS_inf.Name = "FPS_inf";
            this.FPS_inf.Size = new System.Drawing.Size(35, 15);
            this.FPS_inf.TabIndex = 0;
            this.FPS_inf.Text = "FPS : ";
            // 
            // FPS_timer
            // 
            this.FPS_timer.Interval = 1000;
            this.FPS_timer.Tick += new System.EventHandler(this.UI_Timer_Fps_Count);
            // 
            // UI_AppName
            // 
            this.UI_AppName.BackColor = System.Drawing.Color.Transparent;
            this.UI_AppName.Font = new System.Drawing.Font("Microsoft JhengHei", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.UI_AppName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.UI_AppName.Location = new System.Drawing.Point(12, 270);
            this.UI_AppName.Margin = new System.Windows.Forms.Padding(0);
            this.UI_AppName.Name = "UI_AppName";
            this.UI_AppName.Size = new System.Drawing.Size(70, 14);
            this.UI_AppName.TabIndex = 5;
            this.UI_AppName.Text = "Apr GB Emu";
            this.UI_AppName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UI_AppName.Click += new System.EventHandler(this.UI_AppName_Click);
            this.UI_AppName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseDown);
            this.UI_AppName.MouseEnter += new System.EventHandler(this.UI_AppName_MouseEnter);
            this.UI_AppName.MouseLeave += new System.EventHandler(this.UI_AppName_MouseLeave);
            this.UI_AppName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseMove);
            // 
            // UI_Restart_btn
            // 
            this.UI_Restart_btn.Image = global::AprGBemu.Properties.Resources.new2restart;
            this.UI_Restart_btn.Location = new System.Drawing.Point(136, 6);
            this.UI_Restart_btn.Margin = new System.Windows.Forms.Padding(0);
            this.UI_Restart_btn.Name = "UI_Restart_btn";
            this.UI_Restart_btn.Size = new System.Drawing.Size(24, 24);
            this.UI_Restart_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.UI_Restart_btn.TabIndex = 7;
            this.UI_Restart_btn.TabStop = false;
            this.UI_Restart_btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UI_Restart_btn_MouseClick);
            this.UI_Restart_btn.MouseEnter += new System.EventHandler(this.UI_Close_btn_MouseEnter);
            this.UI_Restart_btn.MouseLeave += new System.EventHandler(this.UI_Close_btn_MouseLeave);
            // 
            // UI_Close_hide
            // 
            this.UI_Close_hide.Image = global::AprGBemu.Properties.Resources.new2down;
            this.UI_Close_hide.Location = new System.Drawing.Point(216, 6);
            this.UI_Close_hide.Margin = new System.Windows.Forms.Padding(0);
            this.UI_Close_hide.Name = "UI_Close_hide";
            this.UI_Close_hide.Size = new System.Drawing.Size(24, 24);
            this.UI_Close_hide.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.UI_Close_hide.TabIndex = 6;
            this.UI_Close_hide.TabStop = false;
            this.UI_Close_hide.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UI_Close_hide_MouseClick);
            this.UI_Close_hide.MouseEnter += new System.EventHandler(this.UI_Close_btn_MouseEnter);
            this.UI_Close_hide.MouseLeave += new System.EventHandler(this.UI_Close_btn_MouseLeave);
            // 
            // UI_Close_btn
            // 
            this.UI_Close_btn.Image = global::AprGBemu.Properties.Resources.new2close;
            this.UI_Close_btn.Location = new System.Drawing.Point(246, 6);
            this.UI_Close_btn.Name = "UI_Close_btn";
            this.UI_Close_btn.Size = new System.Drawing.Size(24, 24);
            this.UI_Close_btn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.UI_Close_btn.TabIndex = 4;
            this.UI_Close_btn.TabStop = false;
            this.UI_Close_btn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UI_Close_btn_MouseClick);
            this.UI_Close_btn.MouseEnter += new System.EventHandler(this.UI_Close_btn_MouseEnter);
            this.UI_Close_btn.MouseLeave += new System.EventHandler(this.UI_Close_btn_MouseLeave);
            // 
            // InfoBox
            // 
            this.InfoBox.Image = ((System.Drawing.Image)(resources.GetObject("InfoBox.Image")));
            this.InfoBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("InfoBox.InitialImage")));
            this.InfoBox.Location = new System.Drawing.Point(163, 6);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.Size = new System.Drawing.Size(24, 24);
            this.InfoBox.TabIndex = 8;
            this.InfoBox.TabStop = false;
            this.InfoBox.Click += new System.EventHandler(this.InfoBox_Click);
            this.InfoBox.MouseEnter += new System.EventHandler(this.UI_AppName_MouseEnter);
            this.InfoBox.MouseLeave += new System.EventHandler(this.UI_AppName_MouseLeave);
            // 
            // AprGBemu_MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(280, 290);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.UI_Restart_btn);
            this.Controls.Add(this.FPS_inf);
            this.Controls.Add(this.UI_Close_hide);
            this.Controls.Add(this.UI_AppName);
            this.Controls.Add(this.UI_Close_btn);
            this.Controls.Add(this.UI_LCD_panel);
            this.Controls.Add(this.UI_OptionMenu);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.UI_OptionMenu;
            this.MaximizeBox = false;
            this.Name = "AprGBemu_MainUI";
            this.Text = "Apr";
            this.Activated += new System.EventHandler(this.AprGBemu_MainUI_Activated);
            this.Deactivate += new System.EventHandler(this.AprGBemu_MainUI_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AprGBemu_MainUI_FormClosing);
            this.Shown += new System.EventHandler(this.AprGBemu_MainUI_Shown);
            this.LocationChanged += new System.EventHandler(this.AprGBemu_MainUI_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AprGBemu_MainUI_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AprGBemu_MainUI_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AprGBemu_MainUI_MouseMove);
            this.UI_OptionMenu.ResumeLayout(false);
            this.UI_OptionMenu.PerformLayout();
            this.UI_LCD_panel.ResumeLayout(false);
            this.UI_LCD_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Restart_btn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Close_hide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UI_Close_btn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InfoBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip UI_OptionMenu;
        private System.Windows.Forms.ToolStripMenuItem UI_openRomMenuItem;
        public System.Windows.Forms.Panel UI_LCD_panel;
        private System.Windows.Forms.Timer FPS_timer;
        private System.Windows.Forms.ToolStripMenuItem UI_Configure;
        private System.Windows.Forms.Label FPS_inf;
        private System.Windows.Forms.PictureBox UI_Close_btn;
        private System.Windows.Forms.Label UI_AppName;
        private System.Windows.Forms.PictureBox UI_Close_hide;
        private System.Windows.Forms.Label APP_VER;
        private System.Windows.Forms.PictureBox UI_Restart_btn;
        private System.Windows.Forms.PictureBox InfoBox;
    }
}

