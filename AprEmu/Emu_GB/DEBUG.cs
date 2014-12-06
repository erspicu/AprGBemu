
//#define debug

namespace AprEmu.GB
{
    public partial class Apr_GB
    {
#if debug
        bool debug_start_trace = false;
        ushort debug_stop = 0x0;
        private void Debug(byte opcode)
        {

            return;

            if (r_PC == debug_stop)
                debug_start_trace = true;

            if (debug_start_trace)
            {

                Console.Write("PC:" + r_PC.ToString("X4") + " " + "ROM:" + rom_bank_select + " " + "IE:" + GB_MEM[0xffff].ToString("X2") + " IF:" + GB_MEM[0xff0f].ToString("X2")
                    + " LCDC:" + GB_MEM[0xff40].ToString("X2") + " STAT:" + GB_MEM[0xff41].ToString("x2") + " Ly:" + GB_MEM[reg_LY_addr].ToString("x2"));
                Console.WriteLine(" opcode:" + opcode.ToString("X2"));
                Console.WriteLine(
                    "SP:" + r_SP.ToString("X4") + " " +
                    "ZNHC:" + (byte)flagZ + (byte)flagN + (byte)flagH + (byte)flagC + " " +
                    "A:" + r_A.ToString("X2") + " " + "B:" + r_B.ToString("X2") + " " + "C:" + r_C.ToString("X2") + " " +
                    "D:" + r_D.ToString("X2") + " " + "E:" + r_E.ToString("X2") + " " + "H:" + r_H.ToString("X2") + " " +
                    "L:" + r_L.ToString("X2"));

                Console.WriteLine("jump to : ");
                string jump = Console.ReadLine();
                if (jump != "")
                {
                    debug_stop = (ushort)Convert.ToInt32(jump, 16);
                    debug_start_trace = false;
                }
            }
        }
#endif
    }
}
