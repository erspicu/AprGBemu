using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

using AprEmu.GB;

namespace AprGBemu
{
    public partial class AprGBemu_MainUI : Form
    {

        Graphics Screen_Panel;
        public Thread gb_emu = null;
        Apr_GB gb_machine = new Apr_GB();

        public AprGBemu_MainUI()
        {
            InitializeComponent();

            GB_init_KeyMap();
            Screen_Panel = UI_LCD_panel.CreateGraphics();
        }

        int key_A = 65;
        int key_B = 83;
        int key_SELECT = 88;
        int key_START = 90;
        int key_RIGHT = 39;
        int key_LEFT = 37;
        int key_UP = 38;
        int key_DOWN = 40;

        private void GB_init_KeyMap()
        {
            //日後可以從這method擴充讀取按鍵設定對應功能
            gb_machine.GB_KeyMAP[key_A] = KeyMap.GB_btn_A;
            gb_machine.GB_KeyMAP[key_B] = KeyMap.GB_btn_B;
            gb_machine.GB_KeyMAP[key_SELECT] = KeyMap.GB_btn_SELECT;
            gb_machine.GB_KeyMAP[key_START] = KeyMap.GB_btn_START;
            gb_machine.GB_KeyMAP[key_RIGHT] = KeyMap.GB_btn_RIGHT;
            gb_machine.GB_KeyMAP[key_LEFT] = KeyMap.GB_btn_LEFT;
            gb_machine.GB_KeyMAP[key_UP] = KeyMap.GB_btn_UP;
            gb_machine.GB_KeyMAP[key_DOWN] = KeyMap.GB_btn_DOWN;
        }

       
        private void OpenROM_select(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "GameBoy ROM|*.gb"; //"GameBoy ROM|*.sgb;*.gbc;*.gb"; not support all

            if (fd.ShowDialog() != DialogResult.OK)
                return;

            if (gb_emu != null)
            {
                gb_machine.WaitUnlock(); //避免在Graphics Device寫入階段中斷thread
                gb_emu.Abort();
                gb_emu = null;
                GC.Collect();
            }
            gb_machine.GB_Init_LoadRom(fd.FileName);
            gb_machine.bind_Screen(ref Screen_Panel , 48,40);
            gb_emu = new Thread(gb_machine.GB_run);
            gb_emu.Start();
        }

        private void GBEMU_MainUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (gb_machine.start_run == false)
                return;
            if (!gb_machine.GB_KeyMAP.ContainsKey(e.KeyValue))
                return;
            gb_machine.GB_JoyPad_KeyDown(e.KeyValue);
        }


        private void GBEMU_MainUI_KeyUp(object sender, KeyEventArgs e)
        {
            if (gb_machine.start_run == false)
                return;
            if (!gb_machine.GB_KeyMAP.ContainsKey(e.KeyValue))
                return;
            gb_machine.GB_JoyPad_KeyUp(e.KeyValue);
        }

        private void UI_Timer_Fps_Count(object sender, EventArgs e)
        {
            int tmp_fps = gb_machine.frame_fps_count;
            gb_machine.frame_fps_count = 0;
            Invoke(new MethodInvoker(() =>
            {
                FPS_inf.Text = "FPS : " + tmp_fps.ToString();
            }));
        }

        private void GBEMU_MainUI_LocationChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void Configure_Click(object sender, EventArgs e)
        {
            MessageBox.Show("editing...");
        }

        private void GBEMU_MainUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(1);
        }

        private void UI_Close_btn_MouseClick(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void UI_Close_btn_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void UI_Close_btn_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void UI_Close_hide_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #region 視窗拖曳處理
        //http://www.vcskicks.com/drag_form.php
        Point lastClick;
        private void GBEMU_MainUI_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y);
        }

        private void GBEMU_MainUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }
        #endregion

        private void GBEMU_MainUI_Deactivate(object sender, EventArgs e)
        {
            this.TopMost = true;
            UI_AppName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(206)))), ((int)(((byte)(250))))); 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(206)))), ((int)(((byte)(250)))));
            this.TopMost = false;
        }

        private void GBEMU_MainUI_Activated(object sender, EventArgs e)
        {
            UI_AppName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
            this.TopMost = false;
        }

        private void UI_AppName_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void UI_AppName_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        //http://stackoverflow.com/questions/1600962/displaying-the-build-date
        private DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }

        private void UI_AppName_Click(object sender, EventArgs e)
        {
            MessageBox.Show("editing");
        }

        private void AprGBemu_MainUI_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Invoke(new MethodInvoker(() =>
                {
                    APP_VER.Text = "Release " + RetrieveLinkerTimestamp().ToShortDateString() + " " + RetrieveLinkerTimestamp().ToShortTimeString();
                }));
                Thread.Sleep(3000);
                Invoke(new MethodInvoker(() =>
                {
                    APP_VER.Visible = false;
                }));
                return;
            }).Start();
        }

    }
}
