using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace AprGBemu
{
    public partial class GBEMU_AboutUI : Form
    {
        DateTime VER;
        public GBEMU_AboutUI()
        {
            InitializeComponent();

        }

        public void init()
        {
            VER = AprGBemu_MainUI.GetInstance().Release_Time;
            label3.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["version"] + " " + VER.ToLongDateString() + " " + VER.ToShortTimeString();
            this.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["aboutapp"];
            label2.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["author"] + " " + "erspicu_brox";
            linkLabel1.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["visit_site"];
            button1.Text = LangINI.lang_table[AprGBemu_MainUI.GetInstance().AppConfigure["Lang"]]["ok"];
        }

        protected static GBEMU_AboutUI instance;
        public static GBEMU_AboutUI GetInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new GBEMU_AboutUI();
            return instance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            //Hide();
        }

        private void GBEMU_AboutUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://dl.dropboxusercontent.com/u/61164954/project/AprGBEmu/index.htm");
        }

        private void GBEMU_AboutUI_Shown(object sender, EventArgs e)
        {
        }

    }
}
