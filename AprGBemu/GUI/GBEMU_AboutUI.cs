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
            VER = AprGBemu_MainUI.GetInstance().Release_Time;
            label3.Text = "版本 " + VER.ToLongDateString() + " " + VER.ToShortTimeString();
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
            Hide();
        }

        private void GBEMU_AboutUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://dl.dropboxusercontent.com/u/61164954/project/AprGBEmu/index.htm");
        }

    }
}
