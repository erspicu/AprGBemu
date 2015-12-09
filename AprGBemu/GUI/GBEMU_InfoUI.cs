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
            e.Cancel = true;
            this.Hide();
        }

        private void GBEMU_InfoUI_Shown(object sender, EventArgs e)
        {
            richTextBox1.Text = AprGBemu_MainUI.GetInstance().ReadCartInfo();
        }

    }
}
