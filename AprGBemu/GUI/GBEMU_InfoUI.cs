using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AprGBemu
{
    public partial class GBEMU_InfoUI : Form
    {
        public GBEMU_InfoUI()
        {
            InitializeComponent();
        }

        protected static GBEMU_InfoUI instance;
        public static GBEMU_InfoUI GetInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new GBEMU_InfoUI();
            return instance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GBEMU_InfoUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            // e.Cancel = true;
            // this.Hide();
        }

        private void GBEMU_InfoUI_Shown(object sender, EventArgs e)
        {

        }

        public void init()
        {
            button1.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["ok"];
            this.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["rom_inf"];

            string inf = AprGBemu_MainUI.GetInstance().ReadCartInfo();

            if (inf == "")
                return;


            List<string> line = inf.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            string str = "";

            str += LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["CartridgeTitle"] + " : " + line[0].Remove(0, "Cartridge Title : ".Count()) + "\n";
            str += LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["CartridgeType"] + " : " + line[1].Remove(0, "Cartridge Type : ".Count()) + "\n";
            str += LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["CartridgeMBC"] + " : " + line[2].Remove(0, "Cartridge MBC: ".Count()) + "\n";
            str += LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["CartridgeROM"] + " : " + line[3].Remove(0, "Cartridge ROM Size :  ".Count()) + "\n";
            str += LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["CartridgeRAM"] + " : " + line[4].Remove(0, "Cartridge RAM Size : ".Count()) + "\n";

            richTextBox1.Text = str;
        }

    }
}
