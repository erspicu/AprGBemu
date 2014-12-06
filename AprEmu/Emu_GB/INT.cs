namespace AprEmu.GB
{
    public partial class Apr_GB
    {
        private void GB_Interrupt()
        {
            byte i = (byte)(GB_MEM[reg_IE_addr] & GB_MEM[reg_IF_addr]);
            if ((i & 1) > 0) //vblank  //fix 11/25
            {
                flagIME = flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFE;
                GB_MEM_w8(--r_SP, (byte)(r_PC >> 8));
                GB_MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x40;
                Cpu_cycles += 32;
            }
            if ((i & 2) > 0) //stat
            {
                flagIME = flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFD;
                GB_MEM_w8(--r_SP, (byte)(r_PC >> 8));
                GB_MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x48;
                Cpu_cycles = 32;
            }
            if ((i & 4) > 0) //timer 
            {
                flagIME = flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xFB;
                GB_MEM_w8(--r_SP, (byte)(r_PC >> 8));
                GB_MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x50;
                Cpu_cycles += 32;
            }
            //ignore if ((i & 8) > 1){}
            if ((i & 16) > 0) // buttons
            {
                flagIME = flagHalt = false;
                GB_MEM[reg_IF_addr] &= 0xEF;
                GB_MEM_w8(--r_SP, (byte)(r_PC >> 8));
                GB_MEM_w8(--r_SP, (byte)(r_PC & 0xFF));
                r_PC = 0x60;
                Cpu_cycles += 32;
            }
        }
    }
}
