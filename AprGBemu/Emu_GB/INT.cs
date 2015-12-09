using System;
//using System.Windows.Forms;

namespace AprEmu.GB
{
    public partial class Apr_GB
    {
        private void GB_Interrupt()
        {
            if (!flagIME) return;

            byte i = (byte)(GB_MEM[reg_IE_addr] & GB_MEM[reg_IF_addr]);
            if ((i & 1) > 0) //vblank  //fix 11/25
            {
                flagIME = false;
                flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFE;
                MEM_w8(--r_SP, (byte)(r_PC >> 8));
                MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x40;
                cycles += 20; // fix 2015.11.25

            }
            else if ((i & 2) > 0) //stat
            {
                flagIME = false;
                flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFD;
                MEM_w8(--r_SP, (byte)(r_PC >> 8));
                MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x48;
                cycles += 20;

            }
            else if ((i & 4) > 0) //timer 
            {
                flagIME = false;
                flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFB;
                MEM_w8(--r_SP, (byte)(r_PC >> 8));
                MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x50;
                cycles += 20;
            }
            // else if ((i & 8) > 1) { MessageBox.Show("editing !"); }
            else if ((i & 16) > 0) // buttons
            {
                flagIME = false;
                flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xEF;
                MEM_w8(--r_SP, (byte)(r_PC >> 8));
                MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x60;
                cycles += 20;
            }
        }
    }
}
