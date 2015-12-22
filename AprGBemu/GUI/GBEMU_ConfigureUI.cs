using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AprEmu.GB;


namespace AprGBemu
{
    public partial class GBEMU_ConfigureUI : Form
    {
        public GBEMU_ConfigureUI()
        {
            InitializeComponent();
            
        }

        public void init()
        {
            Ok_btn.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["ok"];
            this.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["settingui"];
            choose_dir.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["selectfolder"];
            groupBox1.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["keypad"];
            groupBox2.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["joypad"];
            groupBox3.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["palette"];
            groupBox6.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["filter"];
            groupBox4.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["screen"];
            groupBox5.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["hardware"];
            Sound_checkBox.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["soundout"];
            LimitFPS_checkBox.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["limitfps"];
            label18.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["langchoose"];
            label9.Text = "Shift + p " + LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["capture_path"];
            radioButton_1.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["classicgreen"];
            radioButton_0.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["gray"];

            GBbootradioButton.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["classic_gb_boot"];
            GBCbootradioButton.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["color_gb_boot"];
            GBradioButton.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["classic_gb"];
            GBCradioButton.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["color_gb"];
            GBSradioButton.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["sgb"];

            comboBox1.Items.Clear();

            int ch = 0;
            foreach (string i in LangINI.lang_map.Keys)
            {
                comboBox1.Items.Add(i + " " + LangINI.lang_map[i]);
                if (i == AprGBemu_MainUI.GetInstance().AppConfigure["Lang"])
                    comboBox1.SelectedIndex = ch;
                ch++;
            }
        }

        protected static GBEMU_ConfigureUI instance;
        public static GBEMU_ConfigureUI GetInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new GBEMU_ConfigureUI();

            

            return instance;
        }

        public void BeforClose()
        {
            AprGBemu_MainUI.GetInstance().Clear();

            if (radioButtonX1.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "1";
            else if (radioButtonX2.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "2";
            else if (radioButtonX3.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "3";
            else if (radioButtonX4.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "4";
            else if (radioButtonX5.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "5";
            else
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"] = "6";

            AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.Clear();

            foreach (string key in GB_KeyMAP_joypad_config.Keys)
                AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad[key] = GB_KeyMAP_joypad_config[key];

            AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] = "DMG";
            AprGBemu_MainUI.GetInstance().ChangeUIszie(AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"]);

            if (GBbootradioButton.Checked == true || GBradioButton.Checked == true)
                AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] = "DMG";
            else if (GBCbootradioButton.Checked == true || GBCradioButton.Checked == true)
                AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] = "CGB";
            else if (GBSradioButton.Checked == true)
                AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] = "SGB";

            if (GBbootradioButton.Checked == true || GBCbootradioButton.Checked == true)
                AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] = "1";
            else
                AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] = "";

            AprGBemu_MainUI.GetInstance().AppConfigure["LimitFPS"] = "0";
            if (LimitFPS_checkBox.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["LimitFPS"] = "1";

            AprGBemu_MainUI.GetInstance().AppConfigure["Sound"] = "0";
            if (Sound_checkBox.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["Sound"] = "1";

            if (scalex_radioButton.Checked)
            {
                AprGBemu_MainUI.GetInstance().AppConfigure["VideoFilter"] = "scalex";
            }
            else if (hqx_radioButton.Checked)
            {
                AprGBemu_MainUI.GetInstance().AppConfigure["VideoFilter"] = "hqx";
            }
            else//xbrz
            {
                AprGBemu_MainUI.GetInstance().AppConfigure["VideoFilter"] = "xbrz";
            }


            AprGBemu_MainUI.GetInstance().AppConfigure["CaptureScreenPath"] = screen_path.Text;
            AprGBemu_MainUI.GetInstance().key_A = key_A;
            AprGBemu_MainUI.GetInstance().key_B = key_B;
            AprGBemu_MainUI.GetInstance().key_SELECT = key_SELECT;
            AprGBemu_MainUI.GetInstance().key_START = key_START;
            AprGBemu_MainUI.GetInstance().key_RIGHT = key_RIGHT;
            AprGBemu_MainUI.GetInstance().key_LEFT = key_LEFT;
            AprGBemu_MainUI.GetInstance().key_UP = key_UP;
            AprGBemu_MainUI.GetInstance().key_DOWN = key_DOWN;

            if (radioButton_0.Checked)
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenPalette"] = "0";
            else
                AprGBemu_MainUI.GetInstance().AppConfigure["ScreenPalette"] = "1";


           AprGBemu_MainUI.GetInstance().AppConfigure["Lang"] = (comboBox1.SelectedItem as string).Split(new char[] { ' ' })[0];

            AprGBemu_MainUI.GetInstance().Configure_Write();
            AprGBemu_MainUI.GetInstance().UI_Restart_btn_MouseClick(null, null);

            AprGBemu_MainUI.GetInstance().init_lang();

        }

        private void OK(object sender, EventArgs e)
        {
            BeforClose();
            Close();
        }

        Dictionary<string, KeyMap> GB_KeyMAP_joypad_config = new Dictionary<string, KeyMap>();
        public void Setup_JoyPad_define(string uid, string btn_name, int raw_id, int value)
        {
            if (joypad_A.Focused)
            {
                if (value != 128) return;
                if (!btn_name.StartsWith("Buttons"))
                {
                    MessageBox.Show("非Button類型輸入!");
                    return;
                }
                joypad_A.Text = btn_name;

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_A))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_A).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + btn_name + "," + raw_id] = KeyMap.GB_btn_A;

            }
            else if (joypad_B.Focused)
            {
                if (!btn_name.StartsWith("Buttons"))
                {
                    MessageBox.Show("非Button類型輸入!");
                    return;
                }
                if (value != 128) return;
                joypad_B.Text = btn_name;

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_B))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_B).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + btn_name + "," + raw_id] = KeyMap.GB_btn_B;
            }
            else if (joypad_START.Focused)
            {
                if (!btn_name.StartsWith("Buttons"))
                {
                    MessageBox.Show("非Button類型輸入!");
                    return;
                }
                if (value != 128) return;
                joypad_START.Text = btn_name;

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_START))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_START).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + btn_name + "," + raw_id] = KeyMap.GB_btn_START;
            }
            else if (joypad_SELECT.Focused)
            {

                if (!btn_name.StartsWith("Buttons"))
                {
                    MessageBox.Show("非Button類型輸入!");
                    return;
                }
                if (value != 128) return;
                joypad_SELECT.Text = btn_name;

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_SELECT))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_SELECT).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + btn_name + "," + raw_id] = KeyMap.GB_btn_SELECT;
            }
            else if (joypad_UP.Focused)
            {
                if (btn_name.StartsWith("Button") && value == 0)
                    return;
                if (!btn_name.StartsWith("X") && !btn_name.StartsWith("Y"))
                {
                    MessageBox.Show("非 X Y 方向鍵類型輸入!");
                    return;
                }
                if (value == 32511) return;
                joypad_UP.Text = JoyPadWayName(btn_name, value);

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_UP))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_UP).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + joypad_UP.Text + "," + raw_id + "," + value] = KeyMap.GB_btn_UP;
            }
            else if (joypad_DOWN.Focused)
            {
                if (btn_name.StartsWith("Button") && value == 0)
                    return;
                if (!btn_name.StartsWith("X") && !btn_name.StartsWith("Y"))
                {
                    MessageBox.Show("非 X Y 方向鍵類型輸入!");
                    return;
                }
                if (value == 32511) return;
                joypad_DOWN.Text = JoyPadWayName(btn_name, value);

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_DOWN))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_DOWN).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + joypad_DOWN.Text + "," + raw_id + "," + value] = KeyMap.GB_btn_DOWN;
            }
            else if (joypad_LEFT.Focused)
            {
                if (btn_name.StartsWith("Button") && value == 0)
                    return;
                if (!btn_name.StartsWith("X") && !btn_name.StartsWith("Y"))
                {
                    MessageBox.Show("非 X Y 方向鍵類型輸入!");
                    return;
                }
                if (value == 32511) return;
                joypad_LEFT.Text = JoyPadWayName(btn_name, value);

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_LEFT))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_LEFT).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + joypad_LEFT.Text + "," + raw_id + "," + value] = KeyMap.GB_btn_LEFT;
            }
            else if (joypad_RIGHT.Focused)
            {
                if (btn_name.StartsWith("Button") && value == 0)
                    return;
                if (!btn_name.StartsWith("X") && !btn_name.StartsWith("Y"))
                {
                    MessageBox.Show("非 X Y 方向鍵類型輸入!");
                    return;
                }
                if (value == 32511) return;
                joypad_RIGHT.Text = JoyPadWayName(btn_name, value);

                if (GB_KeyMAP_joypad_config.Values.Contains(KeyMap.GB_btn_RIGHT))
                {
                    string key = GB_KeyMAP_joypad_config.FirstOrDefault(x => x.Value == KeyMap.GB_btn_RIGHT).Key;
                    GB_KeyMAP_joypad_config.Remove(key);
                }

                GB_KeyMAP_joypad_config[uid + "," + joypad_RIGHT.Text + "," + raw_id + "," + value] = KeyMap.GB_btn_RIGHT;
            }
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


        private void GBEMU_ConfigureUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            // e.Cancel = true;
            // Hide();
        }

        ScreenPalette gb_palette = ScreenPalette.DarkWhite;
        private void GBEMU_ConfigureUI_Shown(object sender, EventArgs e)
        {

            //init();

            GB_KeyMAP_joypad_config.Clear();
            foreach (string key in AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad.Keys)
                GB_KeyMAP_joypad_config[key] = AprGBemu_MainUI.GetInstance().GB_KeyMAP_joypad[key];

            //clear
            joypad_A.Text = joypad_B.Text = joypad_SELECT.Text = joypad_START.Text = joypad_UP.Text = joypad_DOWN.Text = joypad_LEFT.Text = joypad_RIGHT.Text = "";

            foreach (string key in GB_KeyMAP_joypad_config.Keys)
            {
                if (key == "")
                    continue;

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_A)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_A.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_B)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_B.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_SELECT)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_SELECT.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_START)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_START.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_UP)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_UP.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_DOWN)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_DOWN.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_LEFT)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_LEFT.Text = tmp[1];
                }

                if (GB_KeyMAP_joypad_config[key] == KeyMap.GB_btn_RIGHT)
                {
                    List<string> tmp = key.Split(new char[] { ',' }).ToList();
                    joypad_RIGHT.Text = tmp[1];
                }
            }
            switch (AprGBemu_MainUI.GetInstance().AppConfigure["ScreenSize"])
            {
                case "1":
                    radioButtonX1.Checked = true;
                    break;
                case "2":
                    radioButtonX2.Checked = true;
                    break;
                case "3":
                    radioButtonX3.Checked = true;
                    break;
                case "4":
                    radioButtonX4.Checked = true;
                    break;
                case "5":
                    radioButtonX5.Checked = true;
                    break;
                case "6":
                    radioButtonX6.Checked = true;
                    break;
            }


            switch (AprGBemu_MainUI.GetInstance().AppConfigure["VideoFilter"])
            {
                case "scalex":
                    radioButtonX5.Checked = false;
                    radioButtonX5.Enabled = false;
                    scalex_radioButton.Checked = true;
                    break;
                case "hqx":
                    radioButtonX5.Checked = false;
                    radioButtonX5.Enabled = false;
                    hqx_radioButton.Checked = true;
                    break;
                case "xbrz":
                    radioButtonX5.Enabled = true;
                    xbrz_radioButton.Checked = true;
                    break;
            }

            if (AprGBemu_MainUI.GetInstance().AppConfigure["LimitFPS"] == "1")
                LimitFPS_checkBox.Checked = true;
            else
                LimitFPS_checkBox.Checked = false;


            if (AprGBemu_MainUI.GetInstance().AppConfigure["Sound"] == "1")
                Sound_checkBox.Checked = true;
            else
                Sound_checkBox.Checked = false;

            screen_path.Text = AprGBemu_MainUI.GetInstance().AppConfigure["CaptureScreenPath"];

            textBox_A.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_A"])).ToString();
            textBox_B.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_B"])).ToString();
            textBox_SELECT.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_SELECT"])).ToString();
            textBox_START.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_START"])).ToString();
            textBox_UP.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_UP"])).ToString();
            textBox_DOWN.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_DOWN"])).ToString();
            textBox_LEFT.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_LEFT"])).ToString();
            textBox_RIGHT.Text = ((Keys)int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_RIGHT"])).ToString();

            key_A = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_A"]);
            key_B = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_B"]);
            key_SELECT = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_SELECT"]);
            key_START = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_START"]);
            key_UP = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_UP"]);
            key_DOWN = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_DOWN"]);
            key_LEFT = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_LEFT"]);
            key_RIGHT = int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["key_RIGHT"]);

            gb_palette = (ScreenPalette)(int.Parse(AprGBemu_MainUI.GetInstance().AppConfigure["ScreenPalette"]));

            if (gb_palette == ScreenPalette.DarkWhite)
                radioButton_0.Checked = true;
            else if (gb_palette == ScreenPalette.ClassicGreen)
                radioButton_1.Checked = true;

            HardwareType htype = HardwareType.GameBoy;

            if (AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] == "DMG")
            {
                GBradioButton.Checked = true;
                htype = HardwareType.GameBoy;
            }
            else if (AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] == "CGB")
            {
                GBCradioButton.Checked = true;
                htype = HardwareType.GameBoyColor;
            }
            else if (AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] == "SGB")
            {
                GBSradioButton.Checked = true;
                htype = HardwareType.SuperGameBoy;
            }


            if (htype == HardwareType.GameBoy && AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] == "1")
            {
                GBbootradioButton.Checked = true;
            }
            else if (htype == HardwareType.GameBoyColor && AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] == "1")
            {
                GBCbootradioButton.Checked = true;
            }

            if (htype == HardwareType.GameBoy && AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] == "0")
            {
                GBradioButton.Checked = true;
            }
            else if (htype == HardwareType.GameBoyColor && AprGBemu_MainUI.GetInstance().AppConfigure["BootStrap"] == "0")
            {
                GBCradioButton.Checked = true;
            }

            if (!File.Exists(Application.StartupPath + "/bootstrap/gbc_bios.bin"))
            {
                GBCbootradioButton.Enabled = false;
                if (AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] == "CGB")
                    GBCradioButton.Checked = true;
            }

            if (!File.Exists(Application.StartupPath + "/bootstrap/DMG_ROM.bin"))
            {
                GBbootradioButton.Enabled = false;
                if (AprGBemu_MainUI.GetInstance().AppConfigure["HardwareType"] == "DMG")
                    GBradioButton.Checked = true;
            }

            LimitFPS_checkBox.Focus();
        }

        private void choose_dir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() != DialogResult.OK)
                return;
            screen_path.Text = fd.SelectedPath;
        }

        int key_A = 0;
        int key_B = 0;
        int key_SELECT = 0;
        int key_START = 0;
        int key_RIGHT = 0;
        int key_LEFT = 0;
        int key_UP = 0;
        int key_DOWN = 0;

        private void textBox_KeyConfig_KeyUp(object sender, KeyEventArgs e)
        {

            (sender as TextBox).Text = e.KeyData.ToString();
            (sender as TextBox).ReadOnly = true;

            string name = (sender as TextBox).Name.Remove(0, 8);
            switch (name)
            {
                case "A":
                    key_A = e.KeyValue;
                    break;

                case "B":
                    key_B = e.KeyValue;
                    break;

                case "START":
                    key_START = e.KeyValue;
                    break;

                case "SELECT":
                    key_SELECT = e.KeyValue;
                    break;

                case "UP":
                    key_UP = e.KeyValue;
                    break;

                case "DOWN":
                    key_DOWN = e.KeyValue;
                    break;

                case "LEFT":
                    key_LEFT = e.KeyValue;
                    break;

                case "RIGHT":
                    key_RIGHT = e.KeyValue;
                    break;
            }
        }

        private void textBox_KeyConfig_MouseClick(object sender, MouseEventArgs e)
        {
            (sender as TextBox).ReadOnly = false;
        }

        private void radioButton_pal_Click(object sender, EventArgs e)
        {
            RadioButton r = sender as RadioButton;
            string name = r.Name;
            switch (name)
            {
                case "radioButton_0":
                    gb_palette = ScreenPalette.DarkWhite;
                    break;

                case "radioButton_1":
                    gb_palette = ScreenPalette.ClassicGreen;
                    break;
            }
        }

        private void xbrz_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (xbrz_radioButton.Checked)
            {
                radioButtonX5.Enabled = true;
            }
            else
            {
                radioButtonX5.Enabled = false;

                if (radioButtonX5.Checked)
                {
                    radioButtonX5.Checked = false;
                    radioButtonX4.Checked = true;
                }
            }
        }

        private void textBox_A_Leave(object sender, EventArgs e)
        {
            ( sender as TextBox).ReadOnly = true;
        }
    }
}
