using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using AprEmu.GB;
using System.Diagnostics;
using SharpDX.DirectInput;
using AprGBemu.tool;

namespace AprGBemu
{
    public partial class AprGBemu_MainUI : Form
    {
        Graphics Screen_Panel;
        Thread gb_emu = null;
        public Apr_GB gb_machine = new Apr_GB();
        public Dictionary<string, string> AppConfigure = new Dictionary<string, string>();
        string ConfigureFile = Application.StartupPath + @"\AprGBemu.ini";
        bool UILimitFPS = true;
        int UIScreenSize = 1;
        public DateTime Release_Time;
        ScreenPalette gb_palette = ScreenPalette.DarkWhite;
        Dictionary<int, KeyMap> GB_KeyMAP = new Dictionary<int, KeyMap>();
        public Dictionary<string, KeyMap> GB_KeyMAP_joypad = new Dictionary<string, KeyMap>();

        public AprGBemu_MainUI()
        {
            InitializeComponent();
            Configure_Read();
            GB_init_KeyMap();
            Release_Time = VersionTime();

        }
        protected static AprGBemu_MainUI instance;
        public static AprGBemu_MainUI GetInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new AprGBemu_MainUI();
            return instance;
        }

        public void Configure_Read()
        {
            if (!File.Exists(ConfigureFile))
            {
                //建立預設
                AppConfigure["key_A"] = key_A.ToString();
                AppConfigure["key_B"] = key_B.ToString();
                AppConfigure["key_SELECT"] = key_SELECT.ToString();
                AppConfigure["key_START"] = key_START.ToString();
                AppConfigure["key_UP"] = key_UP.ToString();
                AppConfigure["key_DOWN"] = key_DOWN.ToString();
                AppConfigure["key_LEFT"] = key_LEFT.ToString();
                AppConfigure["key_RIGHT"] = key_RIGHT.ToString();
                AppConfigure["LimitFPS"] = "1";
                AppConfigure["ScreenSize"] = "1";
                AppConfigure["CaptureScreenPath"] = Application.StartupPath;
                AppConfigure["ScreenPalette"] = "0";
                AppConfigure["joypad_A"] = "";
                AppConfigure["joypad_B"] = "";
                AppConfigure["joypad_SELECT"] = "";
                AppConfigure["joypad_START"] = "";
                AppConfigure["joypad_UP"] = "";
                AppConfigure["joypad_DOWN"] = "";
                AppConfigure["joypad_LEFT"] = "";
                AppConfigure["joypad_RIGHT"] = "";

                Configure_Write();
                return;
            }
            List<string> lines = File.ReadAllLines(ConfigureFile).ToList();
            foreach (string i in lines)
            {
                List<string> keyvalue = i.Split(new char[] { '=' }).ToList();
                AppConfigure[keyvalue[0]] = keyvalue[1];
            }
            UILimitFPS = false;
            if (AppConfigure["LimitFPS"] == "1")
                UILimitFPS = true;

            gb_palette = (ScreenPalette)int.Parse(AppConfigure["ScreenPalette"]);

            key_A = int.Parse(AppConfigure["key_A"]);
            key_B = int.Parse(AppConfigure["key_B"]);
            key_SELECT = int.Parse(AppConfigure["key_SELECT"]);
            key_START = int.Parse(AppConfigure["key_START"]);
            key_RIGHT = int.Parse(AppConfigure["key_RIGHT"]);
            key_LEFT = int.Parse(AppConfigure["key_LEFT"]);
            key_UP = int.Parse(AppConfigure["key_UP"]);
            key_DOWN = int.Parse(AppConfigure["key_DOWN"]);

            joypad_A = AppConfigure["joypad_A"];
            joypad_B = AppConfigure["joypad_B"];
            joypad_SELECT = AppConfigure["joypad_SELECT"];
            joypad_START = AppConfigure["joypad_START"];
            joypad_UP = AppConfigure["joypad_UP"];
            joypad_DOWN = AppConfigure["joypad_DOWN"];
            joypad_LEFT = AppConfigure["joypad_LEFT"];
            joypad_RIGHT = AppConfigure["joypad_RIGHT"];

            GB_KeyMAP_joypad[joypad_A] = KeyMap.GB_btn_A;
            GB_KeyMAP_joypad[joypad_B] = KeyMap.GB_btn_B;
            GB_KeyMAP_joypad[joypad_SELECT] = KeyMap.GB_btn_SELECT;
            GB_KeyMAP_joypad[joypad_START] = KeyMap.GB_btn_START;
            GB_KeyMAP_joypad[joypad_UP] = KeyMap.GB_btn_UP;
            GB_KeyMAP_joypad[joypad_DOWN] = KeyMap.GB_btn_DOWN;
            GB_KeyMAP_joypad[joypad_LEFT] = KeyMap.GB_btn_LEFT;
            GB_KeyMAP_joypad[joypad_RIGHT] = KeyMap.GB_btn_RIGHT;
        }

        string joypad_A = "";
        string joypad_B = "";
        string joypad_SELECT = "";
        string joypad_START = "";
        string joypad_UP = "";
        string joypad_DOWN = "";
        string joypad_LEFT = "";
        string joypad_RIGHT = "";

        public void Configure_Write()
        {

            AppConfigure["key_A"] = key_A.ToString();
            AppConfigure["key_B"] = key_B.ToString();
            AppConfigure["key_SELECT"] = key_SELECT.ToString();
            AppConfigure["key_START"] = key_START.ToString();
            AppConfigure["key_UP"] = key_UP.ToString();
            AppConfigure["key_DOWN"] = key_DOWN.ToString();
            AppConfigure["key_LEFT"] = key_LEFT.ToString();
            AppConfigure["key_RIGHT"] = key_RIGHT.ToString();

            AppConfigure["joypad_A"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_A))
                AppConfigure["joypad_A"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_A).Key;

            AppConfigure["joypad_B"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_B))
                AppConfigure["joypad_B"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_B).Key;

            AppConfigure["joypad_SELECT"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_SELECT))
                AppConfigure["joypad_SELECT"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_SELECT).Key;

            AppConfigure["joypad_START"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_START))
                AppConfigure["joypad_START"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_START).Key;

            AppConfigure["joypad_UP"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_UP))
                AppConfigure["joypad_UP"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_UP).Key;

            AppConfigure["joypad_DOWN"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_DOWN))
                AppConfigure["joypad_DOWN"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_DOWN).Key;

            AppConfigure["joypad_LEFT"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_LEFT))
                AppConfigure["joypad_LEFT"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_LEFT).Key;

            AppConfigure["joypad_RIGHT"] = "";
            if (GB_KeyMAP_joypad.Values.Contains(KeyMap.GB_btn_RIGHT))
                AppConfigure["joypad_RIGHT"] = GB_KeyMAP_joypad.FirstOrDefault(x => x.Value == KeyMap.GB_btn_RIGHT).Key;

            string conf = "";
            foreach (string i in AppConfigure.Keys)
                conf += i + "=" + AppConfigure[i] + "\r\n";
            File.WriteAllText(ConfigureFile, conf);

            Configure_Read();
            GB_init_KeyMap();

            if (gb_machine != null && gb_emu != null)
            {
                gb_machine.LimitFPS = UILimitFPS;
                gb_machine.GB_ScreenSize = int.Parse(AppConfigure["ScreenSize"]);
                gb_machine.ConfigureScreenColor(gb_palette);
            }
        }
        public int key_A = 65;
        public int key_B = 83;
        public int key_SELECT = 88;
        public int key_START = 90;
        public int key_RIGHT = 39;
        public int key_LEFT = 37;
        public int key_UP = 38;
        public int key_DOWN = 40;
        private void GB_init_KeyMap()
        {
            GB_KeyMAP.Clear();
            GB_KeyMAP[key_A] = KeyMap.GB_btn_A;
            GB_KeyMAP[key_B] = KeyMap.GB_btn_B;
            GB_KeyMAP[key_SELECT] = KeyMap.GB_btn_SELECT;
            GB_KeyMAP[key_START] = KeyMap.GB_btn_START;
            GB_KeyMAP[key_RIGHT] = KeyMap.GB_btn_RIGHT;
            GB_KeyMAP[key_LEFT] = KeyMap.GB_btn_LEFT;
            GB_KeyMAP[key_UP] = KeyMap.GB_btn_UP;
            GB_KeyMAP[key_DOWN] = KeyMap.GB_btn_DOWN;
        }

        string ROM_FILE = "";
        private void UI_OpenROM_select(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "GameBoy ROM|*.gb;*.zip"; //"GameBoy ROM|*.sgb;*.gbc;*.gb"; not support all

            if (fd.ShowDialog() != DialogResult.OK) return;
            if (gb_emu != null)
            {
                gb_machine.WaitUnlock(); //避免在Graphics Device寫入階段中斷thread
                gb_emu.Abort();
                gb_emu = null;
                GC.Collect();
            }
            ROM_FILE = fd.FileName;
            RunningStatus.Visible = Cat.Visible = CatDream.Visible = false;
            gb_machine.GB_ScreenSize = int.Parse(AppConfigure["ScreenSize"]);
            gb_machine.LimitFPS = UILimitFPS;
            gb_machine.ConfigureScreenColor(gb_palette);
            LoadRom();
        }

        private void LoadRom()
        {
            byte[] RomBytes = null;

            //-- ZIP SUPPORT
            if ((new FileInfo(ROM_FILE)).Extension.ToLower() == ".zip")
            {
                FileStream fs = File.OpenRead(ROM_FILE);
                ZipFile zf = new ZipFile(fs);

                foreach (ZipEntry zipEntry in zf)
                {
                    string subname = (new FileInfo(zipEntry.Name)).Extension.ToLower();
                    if (subname == ".gb")
                        RomBytes = io_tool.GetBytesFromStream(zf.GetInputStream(zipEntry));
                }

                fs.Close();
                zf.Close();

            }
            else
            {
                FileStream romfile = File.Open(ROM_FILE, FileMode.Open);
                RomBytes = io_tool.GetBytesFromFileStream(romfile);
                romfile.Close();
            }
            if (RomBytes == null)
            {
                MessageBox.Show("Rom File error !");
                return;
            }
            gb_machine.GB_Init_LoadRom(RomBytes);
            Screen_Panel = UI_LCD_panel.CreateGraphics();
            gb_machine.bind_Screen(ref Screen_Panel, 48, 40);

            if (gb_machine.GB_ScreenSize != 1)
                gb_machine.Screen_loc = new Point(0, 0);

            gb_emu = new Thread(gb_machine.GB_run);
            gb_emu.Start();
        }

        private void AprGBemu_MainUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyValue == 80)
            {
                GBCaptureScreen();
                return;
            }
            if (gb_machine.start_run == false) return;

            if (!GB_KeyMAP.ContainsKey(e.KeyValue)) return;
            gb_machine.GB_JoyPad_KeyDown(GB_KeyMAP[e.KeyValue]);
        }
        private void AprGBemu_MainUI_KeyUp(object sender, KeyEventArgs e)
        {
            if (gb_machine.start_run == false) return;
            if (!GB_KeyMAP.ContainsKey(e.KeyValue)) return;

            gb_machine.GB_JoyPad_KeyUp(GB_KeyMAP[e.KeyValue]);
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
        private void AprGBemu_MainUI_LocationChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }
        private void Configure_Click(object sender, EventArgs e)
        {
            GBEMU_ConfigureUI.GetInstance().StartPosition = FormStartPosition.CenterParent;
            GBEMU_ConfigureUI.GetInstance().ShowDialog(this);
        }
        private void AprGBemu_MainUI_FormClosing(object sender, FormClosingEventArgs e)
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
        private void AprGBemu_MainUI_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y);
        }

        private void AprGBemu_MainUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }
        #endregion
        private void AprGBemu_MainUI_Deactivate(object sender, EventArgs e)
        {
            if (gb_emu != null)
            {
                try
                {
                    gb_emu.Suspend();
                }
                catch
                {
                }

                FPS_timer.Enabled = false;
                RunningStatus.Visible = Cat.Visible = CatDream.Visible = true;

            }
            this.TopMost = true;
            UI_AppName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(206)))), ((int)(((byte)(250)))));
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(206)))), ((int)(((byte)(250)))));
            this.TopMost = false;
        }
        public void ChangeUIszie(string size)
        {
            switch (size)
            {
                case "1":
                    UI_LCD_panel.Width = 256;
                    UI_LCD_panel.Height = 224;
                    this.Width = 280;
                    this.Height = 280;
                    Cat.Location = new Point(3, 152);
                    RunningStatus.Location = new Point(63, 127);
                    CatDream.Location = new Point(125, 90);
                    UI_Close_hide.Location = new Point(216, 6);
                    UI_Close_btn.Location = new Point(246, 6);
                    UI_AppName.Location = new Point(12, 264);
                    FPS_inf.Location = new Point(214, 264);
                    try
                    {
                        gb_machine.Screen_loc = new Point(48, 40);
                    }
                    catch
                    {
                    }
                    break;

                case "2":
                    UI_LCD_panel.Width = 320;
                    UI_LCD_panel.Height = 288;
                    this.Width = 280 + 64;
                    this.Height = 280 + 64;
                    Cat.Location = new Point(3, 152 + 64);
                    RunningStatus.Location = new Point(63, 127 + 64);
                    CatDream.Location = new Point(125, 90 + 64);
                    UI_Close_hide.Location = new Point(216 + 64, 6);
                    UI_Close_btn.Location = new Point(246 + 64, 6);
                    UI_AppName.Location = new Point(12, 264 + 64);
                    FPS_inf.Location = new Point(214 + 64, 264 + 64);
                    try
                    {
                        gb_machine.GB_ScreenSize = 2;
                        gb_machine.Screen_loc = new Point(0, 0);
                    }
                    catch
                    {
                    }
                    Screen_Panel = UI_LCD_panel.CreateGraphics();
                    gb_machine.bind_Screen(ref Screen_Panel, 0, 0);
                    break;

                case "3":
                    UI_LCD_panel.Width = 480; // 256 - 320
                    UI_LCD_panel.Height = 432; // 224 - 208
                    this.Width = 280 + 224;
                    this.Height = 280 + 208;
                    Cat.Location = new Point(3, 152 + 208);
                    RunningStatus.Location = new Point(63, 127 + 208);
                    CatDream.Location = new Point(125, 90 + 208);
                    UI_Close_hide.Location = new Point(216 + 224, 6);
                    UI_Close_btn.Location = new Point(246 + 224, 6);
                    UI_AppName.Location = new Point(12, 264 + 208);
                    FPS_inf.Location = new Point(214 + 224, 264 + 208);
                    try
                    {
                        gb_machine.GB_ScreenSize = 3;
                        gb_machine.Screen_loc = new Point(0, 0);
                    }
                    catch
                    {
                    }
                    Screen_Panel = UI_LCD_panel.CreateGraphics();
                    gb_machine.bind_Screen(ref Screen_Panel, 0, 0);
                    break;

                case "4":
                    UI_LCD_panel.Width = 640; // 256 - 384
                    UI_LCD_panel.Height = 576; // 224 - 352
                    this.Width = 280 + 384;
                    this.Height = 280 + 352;
                    Cat.Location = new Point(3, 152 + 352);
                    RunningStatus.Location = new Point(63, 127 + 352);
                    CatDream.Location = new Point(125, 90 + 352);
                    UI_Close_hide.Location = new Point(216 + 384, 6);
                    UI_Close_btn.Location = new Point(246 + 384, 6);
                    UI_AppName.Location = new Point(12, 264 + 352);
                    FPS_inf.Location = new Point(214 + 384, 264 + 352);
                    try
                    {
                        gb_machine.GB_ScreenSize = 4;
                        gb_machine.Screen_loc = new Point(0, 0);
                    }
                    catch
                    {
                    }
                    Screen_Panel = UI_LCD_panel.CreateGraphics();
                    gb_machine.bind_Screen(ref Screen_Panel, 0, 0);
                    break;
            }
        }

        private void AprGBemu_MainUI_Activated(object sender, EventArgs e)
        {
            if (gb_emu != null)
            {
                try
                {
                    gb_emu.Resume();
                }
                catch
                {
                }
                RunningStatus.Visible = Cat.Visible = CatDream.Visible = false;

                FPS_timer.Enabled = true;

            }
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
        private DateTime VersionTime()
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
            GBEMU_AboutUI.GetInstance().StartPosition = FormStartPosition.CenterParent;
            GBEMU_AboutUI.GetInstance().ShowDialog(this);
        }
        DirectInput directInput = new DirectInput();
        class JoyPadListener
        {
            Joystick joystick;
            public JoyPadListener(Joystick joypad)
            {
                joystick = joypad;
            }

            private string JoyPadWayName(string xy_name, int value)
            {
                string tmp = "";

                if (xy_name == "X")
                {
                    if (value == 0)
                        return "LEFT";

                    if (value == 65535)
                        return "RIGHT";
                }

                if (xy_name == "Y")
                {
                    if (value == 0)
                        return "UP";

                    if (value == 65535)
                        return "DOWN";
                }

                return tmp;
            }
            public void start()
            {
                int press_key = 0;
                while (true)
                {
                    Thread.Sleep(10);

                    joystick.Poll();

                    JoystickUpdate[] datas = joystick.GetBufferedData();
                    foreach (JoystickUpdate state in datas)
                    {
                        // Console.WriteLine(joystick.Information.InstanceGuid.ToString() + " " + state.RawOffset + " " + state.Offset + " " + state.Value);

                        AprGBemu_MainUI.GetInstance().Invoke(new MethodInvoker(() =>
                        {
                            if (GBEMU_ConfigureUI.GetInstance().Visible == true)
                            {
                                GBEMU_ConfigureUI.GetInstance().Setup_JoyPad_define(joystick.Information.InstanceGuid.ToString(), state.Offset.ToString(), state.RawOffset, state.Value);
                            }
                        }));

                        KeyMap joy = KeyMap.GB_btn_A;
                        if (state.Offset.ToString().StartsWith("Buttons"))
                        {
                            string key = joystick.Information.InstanceGuid.ToString() + "," + state.Offset.ToString() + "," + state.RawOffset.ToString();
                            if (AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.ContainsKey(key))
                                joy = AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad[key];
                            else
                                continue;
                        }
                        if (state.Offset.ToString().StartsWith("X") || state.Offset.ToString().StartsWith("Y"))
                        {
                            string key = joystick.Information.InstanceGuid.ToString() + "," + JoyPadWayName(state.Offset.ToString(), state.Value) + "," + state.RawOffset.ToString() + "," + state.Value.ToString();

                            if (AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.ContainsKey(key))
                                joy = AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad[key];
                            else
                            {
                                string key_a = joystick.Information.InstanceGuid.ToString() + "," + JoyPadWayName(state.Offset.ToString(), 0) + "," + state.RawOffset.ToString() + "," + "0";
                                string key_b = joystick.Information.InstanceGuid.ToString() + "," + JoyPadWayName(state.Offset.ToString(), 65535) + "," + state.RawOffset.ToString() + "," + "65535";

                                if (AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.ContainsKey(key_a) || (AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.ContainsKey(key_b)))
                                {
                                    if (state.Offset.ToString() == "X")
                                    {
                                        AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_LEFT);
                                        AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_RIGHT);
                                    }

                                    if (state.Offset.ToString() == "Y")
                                    {
                                        AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_UP);
                                        AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_DOWN);
                                    }
                                }
                                continue;
                            }
                        }
                        if (AprGBemu_MainUI.GetInstance().gb_machine.start_run == true)
                        {

                            switch (joy)
                            {
                                case KeyMap.GB_btn_A:
                                    {
                                        if (state.Value == 128)
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_A);
                                        else
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_A);

                                    }
                                    break;
                                case KeyMap.GB_btn_B:
                                    {
                                        if (state.Value == 128)
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_B);
                                        else
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_B);
                                    }
                                    break;

                                case KeyMap.GB_btn_SELECT:
                                    {
                                        if (state.Value == 128)
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_SELECT);
                                        else
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_SELECT);
                                    }
                                    break;
                                case KeyMap.GB_btn_START:
                                    {
                                        if (state.Value == 128)
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_START);
                                        else
                                            AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyUp(KeyMap.GB_btn_START);
                                    }
                                    break;

                                case KeyMap.GB_btn_UP:
                                    AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_UP);
                                    break;

                                case KeyMap.GB_btn_DOWN:
                                    AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_DOWN);
                                    break;
                                case KeyMap.GB_btn_LEFT:
                                    AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_LEFT);
                                    break;

                                case KeyMap.GB_btn_RIGHT:
                                    AprGBemu_MainUI.GetInstance().gb_machine.GB_JoyPad_KeyDown(KeyMap.GB_btn_RIGHT);
                                    break;
                            }

                        }
                    }
                }
            }

        }

        List<Guid> joypads = new List<Guid>();
        private void AprGBemu_MainUI_Shown(object sender, EventArgs e)
        {
            #region joypad init
            //from http://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
            var joystickGuid = Guid.Empty;

            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    joypads.Add(deviceInstance.InstanceGuid);

            if (joypads.Count == 0)
            {
                Console.WriteLine("No joystick/Gamepad found.");
            }
            else
            {

                foreach (Guid i in joypads)
                {
                    Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", i.ToString());

                    Joystick joystick = new Joystick(directInput, i);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();

                    JoyPadListener JoyPadListener_obj = new JoyPadListener(joystick);
                    new Thread(JoyPadListener_obj.start).Start();
                }
            }
            #endregion


            FPS_timer.Enabled = true;
            new Thread(() =>
            {
                Invoke(new MethodInvoker(() =>
                {
                    APP_VER.Text = "Release " + Release_Time.ToShortDateString() + " " + Release_Time.ToShortTimeString();
                }));
                Thread.Sleep(3000);
                Invoke(new MethodInvoker(() =>
                {
                    APP_VER.Visible = false;
                }));
                return;
            }).Start();

            ChangeUIszie(AppConfigure["ScreenSize"]);
        }
        bool writing = false;
        public void GBCaptureScreen()
        {
            if (gb_emu == null)
                return;
            if (writing == true)
                return;
            writing = true;
            while (gb_machine.screen_lock)
                Thread.Sleep(0);
            gb_emu.Suspend();
            DateTime dt = DateTime.Now;
            string stamp = (dt.ToLongDateString() + " " + dt.ToLongTimeString()).Replace(":", "-");
            try
            {
                gb_machine.GetScreenFrame().Save(AppConfigure["CaptureScreenPath"] + @"\Screen-" + stamp + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch { }
            gb_emu.Resume();
            Console.WriteLine("Screen-" + stamp + ".png" + " write finish !");
            writing = false;
        }

        private void UI_Restart_btn_MouseClick(object sender, MouseEventArgs e)
        {

            if (ROM_FILE == "")
                return;

            if (gb_emu != null)
            {
                gb_machine.WaitUnlock(); //避免在Graphics Device寫入階段中斷thread
                gb_emu.Abort();
                gb_emu = null;
                GC.Collect();
            }
            RunningStatus.Visible = Cat.Visible = false;
            gb_machine.GB_ScreenSize = int.Parse(AppConfigure["ScreenSize"]);

            if (gb_machine.GB_ScreenSize != 1)
                gb_machine.Screen_loc = new Point(0, 0);

            gb_machine.LimitFPS = UILimitFPS;
            gb_machine.ConfigureScreenColor(gb_palette);
            LoadRom();
        }
    }
}