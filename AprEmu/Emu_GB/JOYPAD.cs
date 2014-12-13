using System.Collections.Generic;
using System;

namespace AprEmu.GB
{

    public partial class Apr_GB
    {
       
        byte gbPin14 = 0xff;
        byte gbPin15 = 0xff;
        //a:A s:B z:START x:SELECT 大小寫無分
        public void GB_JoyPad_KeyDown(KeyMap key)
        {
            switch (key)
            {
                case KeyMap.GB_btn_A:
                    gbPin15 &= 0xfE;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_B:
                    gbPin15 &= 0xfD;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_SELECT:
                    gbPin15 &= 0xfB;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_START:
                    gbPin15 &= 0xf7;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_RIGHT:
                    gbPin14 &= 0xfe;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_LEFT:
                    gbPin14 &= 0xfD;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_UP:
                    gbPin14 &= 0xfB;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_DOWN:
                    gbPin14 &= 0xf7;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
            }
        }
        public void GB_JoyPad_KeyUp(KeyMap key)
        {
            //if (!GB_KeyMAP.ContainsKey(key))
            //return;
            switch (key)// (GB_KeyMAP[key])
            {
                case KeyMap.GB_btn_A:
                    gbPin15 |= 1;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_B:
                    gbPin15 |= 2;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_SELECT:
                    gbPin15 |= 4;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_START:
                    gbPin15 |= 8;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_RIGHT:
                    gbPin14 |= 1;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_LEFT:
                    gbPin14 |= 2;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_UP:
                    gbPin14 |= 4;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
                case KeyMap.GB_btn_DOWN:
                    gbPin14 |= 8;
                    GB_MEM[reg_IF_addr] |= 16;
                    break;
            }
        }
    }
}
