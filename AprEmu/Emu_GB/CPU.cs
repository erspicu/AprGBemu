//#define debug
using System;
using System.Windows.Forms;

namespace AprEmu.GB
{
    public partial class Apr_GB
    {
        //相當長的code...以後會再提出解析opcode來decode處理極少程式碼行數版本
        private void GB_CPU_exec()
        {
            byte opcode = GB_MEM_r8(r_PC);

#if debug
            Debug(opcode);
#endif
            r_PC++;

            switch (opcode)
            {
                //checked 2014.11.08
                #region 8bit Load
                case 0x06: //LD B,n
                    r_B = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x0E: //LD C,n
                    r_C = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x16: //LD D,n
                    r_D = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x1E: //LD E,n
                    r_E = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break
                        ;
                case 0x26: //LD H,n
                    r_H = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x2E: //LD L,n
                    r_L = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x7F: //LD A,A
                    Cpu_cycles = 4;
                    break;

                case 0x78: //LD A,B
                    r_A = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x79: //LD A,C
                    r_A = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x7A: //LD A,D
                    r_A = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x7B://LD A,E
                    r_A = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x7C://LD A,H
                    r_A = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x7D://LD A,L
                    r_A = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x7E: //LD A,(HL)
                    r_A = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x40://LD B,B
                    Cpu_cycles = 4;
                    break;

                case 0x41://LD B,C
                    r_B = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x42://LD ,B,D
                    r_B = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x43://LD B,C
                    r_B = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x44://LD B,H
                    r_B = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x45://LD B,L
                    r_B = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x46: //LD B,(HL)
                    r_B = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x48:// LD C,B
                    r_C = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x49://LD C,C
                    Cpu_cycles = 4;
                    break;

                case 0x4A://LD C,D
                    r_C = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x4B://LD C,E
                    r_C = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x4C: //LD C,H
                    r_C = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x4D: //LD C,L
                    r_C = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x4E: //LD C,(HL)
                    r_C = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x50://LD D,B
                    r_D = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x51://LD D,C
                    r_D = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x52://LD D,D
                    Cpu_cycles = 4;
                    break;

                case 0x53:// LD D,E
                    r_D = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x54: //LD D,H
                    r_D = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x55://LD D,L
                    r_D = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x56: //LD D,(HL)
                    r_D = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x58://LD E,B
                    r_E = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x59://LD E,C
                    r_E = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x5A: //LD E,D
                    r_E = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x5B: //LD E,E
                    Cpu_cycles = 4;
                    break;

                case 0x5c: //LD E,H
                    r_E = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x5D: //LD E,L
                    r_E = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x5E: //LD E,(HL)
                    r_E = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x60://LD H,B
                    r_H = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x61://LD H,C
                    r_H = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x62://LD H,D
                    r_H = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x63://LD H,E
                    r_H = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x64:// LD H,H
                    Cpu_cycles = 4;
                    break;

                case 0x65://LD H,L
                    r_H = r_L;
                    Cpu_cycles = 4;
                    break;

                case 0x66://LD H,(HL)
                    r_H = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;

                case 0x68: //LD L,B
                    r_L = r_B;
                    Cpu_cycles = 4;
                    break;

                case 0x69: // LD L,C
                    r_L = r_C;
                    Cpu_cycles = 4;
                    break;

                case 0x6A://LD L,D
                    r_L = r_D;
                    Cpu_cycles = 4;
                    break;

                case 0x6B: //LD L,E
                    r_L = r_E;
                    Cpu_cycles = 4;
                    break;

                case 0x6C: //LD L,H
                    r_L = r_H;
                    Cpu_cycles = 4;
                    break;

                case 0x6D://LD L,L
                    Cpu_cycles = 4;
                    break;

                case 0x6E: //LD L,(HL)
                    r_L = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    Cpu_cycles = 8;
                    break;


                case 0x70://LD (HL),B
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_B);
                    Cpu_cycles = 8;
                    break;

                case 0x71://LD (HL),C
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_C);
                    Cpu_cycles = 8;
                    break;

                case 0x72://LD (HL),D
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_D);
                    Cpu_cycles = 8;
                    break;

                case 0x73://LD (HL),E
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_E);
                    Cpu_cycles = 8;
                    break;

                case 0x74://LD (HL),H
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_H);
                    Cpu_cycles = 8;
                    break;

                case 0x75://LD (HL),L
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_L);
                    Cpu_cycles = 8;
                    break;

                case 0x36://LD (HL),n
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), GB_MEM_r8(r_PC++));
                    Cpu_cycles = 12;
                    break;

                case 0x0A: //LD A,(BC)
                    r_A = GB_MEM_r8((ushort)(r_B << 8 | r_C));
                    Cpu_cycles = 8;
                    break;

                case 0x1A://LD A,(DE)
                    r_A = GB_MEM_r8((ushort)(r_D << 8 | r_E));
                    Cpu_cycles = 8;
                    break;

                case 0xFA: //LD A,(nn)
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        r_A = GB_MEM_r8((ushort)(t2 << 8 | t1));
                        Cpu_cycles = 16;
                        break;
                    }

                case 0x3E: // LD A,n
                    r_A = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 8;
                    break;

                case 0x47://LD B,A
                    r_B = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x4F://LD C,A
                    r_C = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x57://LD D,A
                    r_D = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x5F://LD ,E,A
                    r_E = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x67://LD H,A
                    r_H = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x6F://LD L,A
                    r_L = r_A;
                    Cpu_cycles = 4;
                    break;

                case 0x02://LD (BC),A
                    GB_MEM_w8((ushort)(r_B << 8 | r_C), r_A);
                    Cpu_cycles = 8;
                    break;

                case 0x12://LD (DE),A
                    GB_MEM_w8((ushort)(r_D << 8 | r_E), r_A);
                    Cpu_cycles = 8;
                    break;

                case 0x77://LD (HL),A
                    GB_MEM_w8((ushort)(r_H << 8 | r_L), r_A);
                    Cpu_cycles = 8;
                    break;

                case 0xEA://LD (nn),A
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        GB_MEM_w8((ushort)(t2 << 8 | t1), r_A);
                        Cpu_cycles = 16;
                        break;
                    }
                case 0xF2: //LD A,($FF00+C)
                    r_A = GB_MEM_r8((ushort)(0xFF00 | r_C));
                    Cpu_cycles = 8;
                    break;

                case 0xE2://LD ($FF00+C),A
                    GB_MEM_w8((ushort)(0xFF00 | r_C), r_A);
                    Cpu_cycles = 8;
                    break;

                case 0x3A://LD A,(HLD)
                    {
                        //fix 11/05
                        ushort t1 = (ushort)(r_H << 8 | r_L);
                        ushort t2 = (ushort)(t1 - 1);
                        r_A = GB_MEM_r8(t1);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x32: //LD (HDL),A
                    {
                        //fix 11/05
                        ushort t1 = (ushort)(r_H << 8 | r_L);
                        ushort t2 = (ushort)(t1 - 1);
                        GB_MEM_w8(t1, r_A);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x2A://LD A,(HLI)
                    {
                        ushort t1 = (ushort)(r_H << 8 | r_L);
                        ushort t2 = (ushort)(t1 + 1);
                        r_A = GB_MEM_r8(t1);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x22://LD (HLI),A
                    {
                        ushort t1 = (ushort)((r_H << 8 | r_L) + 1);
                        GB_MEM_w8((ushort)(r_H << 8 | r_L), r_A);
                        r_H = (byte)(t1 >> 8);
                        r_L = (byte)(t1 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xE0://LD ($FF00+n),A
                    GB_MEM_w8((ushort)(0xFF00 | GB_MEM_r8(r_PC++)), r_A);
                    Cpu_cycles = 12;
                    break;

                case 0xF0://LD A,($FF00+n)
                    r_A = GB_MEM_r8((ushort)(0xFF00 | GB_MEM_r8(r_PC++)));
                    Cpu_cycles = 12;
                    break;
                #endregion
                #region 16bit load
                case 0x01: //LD BC,nn
                    r_C = GB_MEM_r8(r_PC++);
                    r_B = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 12;
                    break;

                case 0x11://LD DE,nn
                    r_E = GB_MEM_r8(r_PC++);
                    r_D = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 12;
                    break;

                case 0x21://LD HL,nn
                    r_L = GB_MEM_r8(r_PC++);
                    r_H = GB_MEM_r8(r_PC++);
                    Cpu_cycles = 12;
                    break;

                case 0x31: //LD SP,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        r_SP = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                        break;
                    }

                case 0xF9://LD SP,HL
                    r_SP = (ushort)(r_H << 8 | r_L);
                    Cpu_cycles = 8;
                    break;

                case 0xF8: //LDHL SP,n
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        flagZ = Flag_Status.clear;
                        flagN = Flag_Status.set;
                        if (r_SP + t1 > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_SP & 0xFFF) + (t1 & 0xFFF) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_SP = (ushort)(r_SP + t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0x08: //LD (nn),SP
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        ushort t3 = (ushort)(t2 << 8 | t1);
                        GB_MEM_w8(t3, (byte)(r_SP >> 8));
                        GB_MEM_w8((ushort)(t3 + 1), (byte)(r_SP & 0xFF));
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xF5: //PUSH AF
                    GB_MEM_w8(--r_SP, r_A);
                    GB_MEM_w8(--r_SP, (byte)((byte)flagZ << 7 | (byte)flagN << 6 | (byte)flagH << 5 | (byte)flagC << 4));
                    Cpu_cycles = 16;
                    break;

                case 0xC5: //PUSH BC
                    //MessageBox.Show((rSP - 1).ToString());
                    GB_MEM_w8(--r_SP, r_B);
                    GB_MEM_w8(--r_SP, r_C);
                    //r_SP = (ushort)(r_SP - 2);
                    Cpu_cycles = 16;
                    break;

                case 0xD5://PUSH DE
                    GB_MEM_w8(--r_SP, r_D);
                    GB_MEM_w8(--r_SP, r_E);
                    //r_SP = (ushort)(r_SP - 2);
                    Cpu_cycles = 16;
                    break;

                case 0xE5://PUSH HL
                    GB_MEM_w8(--r_SP, r_H);
                    GB_MEM_w8(--r_SP, r_L);
                    //r_SP = (ushort)(r_SP - 2);
                    Cpu_cycles = 16;
                    break;

                case 0xF1://POP AF 
                    {
                        // 11/25 fix
                        byte t1 = GB_MEM_r8(r_SP++);
                        if ((t1 & 0x80) >> 7 == 1) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        if ((t1 & 0x40) >> 6 == 1) flagN = Flag_Status.set; else flagN = Flag_Status.clear;
                        if ((t1 & 0x20) >> 5 == 1) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((t1 & 0x10) >> 4 == 1) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = GB_MEM_r8(r_SP++);
                        //r_SP = (ushort)(r_SP + 2);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xC1://POP BC
                    r_C = GB_MEM_r8(r_SP++);
                    r_B = GB_MEM_r8(r_SP++);
                    //r_SP = (ushort)(r_SP + 2);
                    Cpu_cycles = 12;
                    break;

                case 0xD1://POP DE
                    r_E = GB_MEM_r8(r_SP++);
                    r_D = GB_MEM_r8(r_SP++);
                    //r_SP = (ushort)(r_SP + 2);
                    Cpu_cycles = 12;
                    break;

                case 0xE1://POP HL
                    r_L = GB_MEM_r8(r_SP++);
                    r_H = GB_MEM_r8(r_SP++);
                    //r_SP = (ushort)(r_SP + 2);
                    Cpu_cycles = 12;
                    break;
                #endregion
                #region 8bit ALU
                case 0x87: //ADD A,A
                    flagN = Flag_Status.clear;
                    if (r_A + r_A > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_A & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_A;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x80://ADD A,B
                    flagN = Flag_Status.clear;
                    if (r_A + r_B > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_B & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_B;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x81://ADD A,C
                    flagN = Flag_Status.clear;
                    if (r_A + r_C > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_C & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_C;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x82://ADD A,D
                    flagN = Flag_Status.clear;
                    if (r_A + r_D > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_D & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_D;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x83:// ADD A,E
                    flagN = Flag_Status.clear;
                    if (r_A + r_E > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_E & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_E;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x84://ADD A,H
                    // 11/25 fixed
                    flagN = Flag_Status.clear;
                    if (r_A + r_H > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_H & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_H;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x85: //ADD A,L
                    flagN = Flag_Status.clear;
                    if (r_A + r_L > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if ((r_A & 0xF) + (r_L & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    r_A += r_L;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x86://ADD A,(HL)
                    {
                        byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        flagN = Flag_Status.clear;
                        if (r_A + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (t1 & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += t1;
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xC6: //ADD A,n
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        flagN = Flag_Status.clear;
                        if (r_A + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (t1 & 0xF) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += t1;
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x8F: //ADC A,A
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_A + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_A & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_A);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x88: //ADC A,B
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if ((r_A + r_B + t1) > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((r_A & 0xF) + (r_B & 0xF) + t1) > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_B);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x89://ADC A,C
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_C + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_C & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_C);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x8A://ADC A,D
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_D + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_D & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_D);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x8B://ADC A,E
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_E + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_E & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_E);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x8C://ADC A,H
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_H + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_H & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_H);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x8D://ADC A,L
                    {
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + r_L + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (r_L & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + r_L);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x8E://ADC A,(HL)
                    {
                        byte t2 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + t2 + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (t2 & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + t2);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xCE://ADC A,n
                    { //fix 11/8

                        byte t2 = GB_MEM_r8(r_PC++);
                        flagN = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        if (r_A + t2 + t1 > 0xFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if ((r_A & 0xF) + (t2 & 0xF) + t1 > 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        r_A += (byte)(t1 + t2);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;


                        Cpu_cycles = 8;
                    }
                    break;

                case 0x97://SUB A,A
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagN = Flag_Status.set;
                    flagZ = Flag_Status.set;
                    r_A = 0;
                    Cpu_cycles = 4;
                    break;

                case 0x90://SUB A,B
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_B & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_B & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_B;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x91://SUB A,C
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_C & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_C & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_C;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x92://SUB A,D
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_D & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_D & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_D;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x93://SUB A,E
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_E & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_E & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_E;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x94://SUB A,H
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_H & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_H & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_H;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x95://SUB A,L
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_L & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_L & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    r_A -= r_L;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x96://SUB A,(HL)
                    {
                        byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        flagN = Flag_Status.set;
                        if ((r_A & 0xF) < (t1 & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < (t1 & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A -= t1;
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xD6://SUB A,n
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        flagN = Flag_Status.set;
                        if ((r_A & 0xF) < (t1 & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < (t1 & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A -= t1;
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x9F: //SBC A,A
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_A & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_A & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_A - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x98://SBC A,B
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_B & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_B & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_B - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x99://SBC A,C
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_C & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_C & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_C - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x9A://SBC A,D
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_D & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_D & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_D - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x9B://SBC A,E
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_E & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_E & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_E - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x9C://SBC A,H
                    {
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_H & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_H & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_H - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;


                case 0x9D://SBC A,L
                    {//add 11/8
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        if ((r_A & 0xF) < ((r_L & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((r_L & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - r_L - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x9E://SBC A,(HL)
                    {//fix 11/8
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        byte t2 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        if ((r_A & 0xF) < ((t2 & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((t2 & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - t2 - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xDE://SBC A,n
                    {//add 11/8 opcode need check
                        flagN = Flag_Status.set;
                        byte t1 = (byte)flagC;
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((r_A & 0xF) < ((t2 & 0xF) + t1)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < ((t2 & 0xFF) + t1)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        r_A = (byte)(r_A - t2 - t1);
                        if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;

                        Cpu_cycles = 8;//?
                    }
                    break;

                case 0xA7: //AND A,A
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA0://AND A,B
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_B;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA1://AND A,C
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_C;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA2://AND A,D
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_D;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA3://AND A,E
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_E;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA4://AND A,H
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_H;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA5://AND A,L
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= r_L;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA6://AND A,(HL)
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xE6://AND A,n
                    flagC = Flag_Status.clear;
                    flagH = Flag_Status.set;
                    flagN = Flag_Status.clear;
                    r_A &= GB_MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xB7://OR A,A
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB0://OR A,B
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_B;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB1://OR A,C
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_C;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB2://OR A,D
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_D;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB3://OR A,E
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_E;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB4://OR A,H
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_H;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB5://OR A,L
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= r_L;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xB6://OR A,(HL)
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xF6://OR A,n
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A |= GB_MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xAF://XOR A,A
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_A;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;


                case 0xA8://XOR A,B
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_B;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xA9://XOR A,C
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_C;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xAA://XOR A,D
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_D;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xAB://XOR A,E
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_E;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xAC://XOR A,H
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_H;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xAD://XOR A,L
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= r_L;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xAE://XOR A,(HL)
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= GB_MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xEE: //XOR A,n
                    //MessageBox.Show("need check -" + opcode.ToString("X2") + " PC:"+rPC.ToString("x2"));
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.clear;
                    r_A ^= GB_MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 8;
                    break;

                case 0xBF: //CP A
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_A & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_A & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    flagZ = Flag_Status.set;
                    Cpu_cycles = 4;
                    Cpu_cycles = 4;
                    break;

                case 0xB8://CP B
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_B & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_B & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_B) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear; ;
                    Cpu_cycles = 4;
                    break;

                case 0xB9://CD C
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_C & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_C & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_C) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xBA://CP D
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_D & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_D & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_D) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xBB://CP E
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_E & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_E & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_E) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;


                case 0xBC://CP H
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_H & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_H & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_H) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xBD://CP L
                    flagN = Flag_Status.set;
                    if ((r_A & 0xF) < (r_L & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if ((r_A & 0xFF) < (r_L & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                    if (r_A == r_L) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0xBE://CP (HL)
                    {
                        byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        flagN = Flag_Status.set;
                        if ((r_A & 0xF) < (t1 & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < (t1 & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (r_A == t1) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xFE: //CP A,n
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        flagN = Flag_Status.set;
                        if ((r_A & 0xF) < (t1 & 0xF)) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if ((r_A & 0xFF) < (t1 & 0xFF)) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (r_A == t1) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x3C://INC A
                    flagN = Flag_Status.clear;
                    r_A++;
                    if ((r_A & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x04://INC B
                    flagN = Flag_Status.clear;
                    r_B++;
                    if ((r_B & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x0C: //INC C
                    flagN = Flag_Status.clear;
                    r_C++;
                    if ((r_C & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x14: //INC D
                    flagN = Flag_Status.clear;
                    r_D++;
                    if ((r_D & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x1C: //INC E
                    flagN = Flag_Status.clear;
                    r_E++;
                    if ((r_E & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x24: //INC H
                    flagN = Flag_Status.clear;
                    r_H++;
                    if ((r_H & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x2C: //INC L
                    flagN = Flag_Status.clear;
                    r_L++;
                    if ((r_L & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x34: //INC (HL)
                    {
                        byte t1 = (byte)(GB_MEM_r8((ushort)(r_H << 8 | r_L)));
                        flagN = Flag_Status.clear;
                        t1++;
                        if ((t1 & 0xF) == 0) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        GB_MEM_w8((ushort)(r_H << 8 | r_L), t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0x3D: //DEC A
                    flagN = Flag_Status.set;
                    r_A--;
                    if ((r_A & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x05://DEC B
                    flagN = Flag_Status.set;
                    r_B--;
                    if ((r_B & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x0D://DEC C
                    flagN = Flag_Status.set;
                    r_C--;
                    if ((r_C & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x15://DEC D
                    flagN = Flag_Status.set;
                    r_D--;
                    if ((r_D & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x1D://DEC E
                    flagN = Flag_Status.set;
                    r_E--;
                    if ((r_E & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x25://DEC H
                    flagN = Flag_Status.set;
                    r_H--;
                    if ((r_H & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x2D://DEC L
                    flagN = Flag_Status.set;
                    r_L--;
                    if ((r_L & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                    if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x35://DEC (HL)
                    {
                        byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                        t1--;
                        GB_MEM_w8((ushort)(r_H << 8 | r_L), t1);
                        flagN = Flag_Status.set;
                        if ((t1 & 0xF) == 0xF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        Cpu_cycles = 12;
                    }
                    break;
                #endregion
                #region 16bit ALU
                case 0x09: //ADD HL,BC
                    {
                        flagN = Flag_Status.clear;
                        ushort t1 = (ushort)(r_B << 8 | r_C);
                        ushort t2 = (ushort)(r_H << 8 | r_L);
                        if ((t1 + t2) > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((t1 & 0xFFF) + (t2 & 0XFFF)) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        t2 = (ushort)((t1 + t2) & 0xFFFF);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x19://ADD HL,DE
                    {
                        flagN = Flag_Status.clear;
                        ushort t1 = (ushort)(r_D << 8 | r_E);
                        ushort t2 = (ushort)(r_H << 8 | r_L);
                        if ((t1 + t2) > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((t1 & 0xFFF) + (t2 & 0XFFF)) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        t2 = (ushort)((t1 + t2) & 0xFFFF);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x29://ADD HL,HL
                    {
                        flagN = Flag_Status.clear;
                        ushort t1 = (ushort)(r_H << 8 | r_L);
                        if ((t1 + t1) > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((t1 & 0xFFF) + (t1 & 0XFFF)) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        t1 = (ushort)((t1 + t1) & 0xFFFF);
                        r_H = (byte)(t1 >> 8);
                        r_L = (byte)(t1 & 0xFF);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x39://ADD HL,SP
                    {
                        flagN = Flag_Status.clear;
                        ushort t1 = r_SP;
                        ushort t2 = (ushort)(r_H << 8 | r_L);
                        if ((t1 + t2) > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((t1 & 0xFFF) + (t2 & 0XFFF)) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear;
                        t2 = (ushort)((t1 + t2) & 0xFFFF);
                        r_H = (byte)(t2 >> 8);
                        r_L = (byte)(t2 & 0xFF);


                        Cpu_cycles = 8;
                    }
                    break;

                case 0xE8://ADD SP,n
                    {
                        flagZ = Flag_Status.clear;
                        flagN = Flag_Status.clear;
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        if (((r_SP + t1) & 0xFFFF) > 0xFFFF) flagC = Flag_Status.set; else flagC = Flag_Status.clear;
                        if (((r_SP + t1) & 0xFFF) > 0xFFF) flagH = Flag_Status.set; else flagH = Flag_Status.clear; // need check
                        r_SP = (ushort)(r_SP + t1);
                        Cpu_cycles = 16;
                    }
                    break;

                case 0x03: //INC BC
                    {
                        ushort t1 = (ushort)((r_B << 8 | r_C) + 1);
                        r_B = (byte)(t1 >> 8);
                        r_C = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x13://INC DE
                    {
                        ushort t1 = (ushort)((r_D << 8 | r_E) + 1);
                        r_D = (byte)(t1 >> 8);
                        r_E = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x23://INC HL
                    {
                        ushort t1 = (ushort)((r_H << 8 | r_L) + 1);
                        r_H = (byte)(t1 >> 8);
                        r_L = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x33://INC SP
                    {
                        //r_SP = (ushort) ((r_SP + 1) & 0xFFFF);
                        //r_SP++;
                        ++r_SP;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x0B: //DEC BC
                    { //fix  11/20
                        ushort t1 = (ushort)((r_B << 8 | r_C) - 1);
                        r_B = (byte)(t1 >> 8);
                        r_C = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x1B://DEC DE
                    { //fix 11/20
                        ushort t1 = (ushort)((r_D << 8 | r_E) - 1);
                        r_D = (byte)(t1 >> 8);
                        r_E = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x2B://DEC HL
                    {
                        ushort t1 = (ushort)((r_H << 8 | r_L) - 1);
                        r_H = (byte)(t1 >> 8);
                        r_L = (byte)(t1 & 0xff);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x3B://DEC SP
                    { //fix 11/20
                        //r_SP--;
                        --r_SP;
                        Cpu_cycles = 8;
                    }
                    break;
                #endregion
                #region miscellaneous
                //SWAP move to Rotates & shifts
                case 0x27: //DAA
                    {
                        // REF https://github.com/drhelius/Gearboy/blob/master/src/opcodes.cpp  void Processor::OPCode0x27()
                        // DAA 這指令的運作,在GAMEBOY Z80上似乎有自己的特性
                        ushort t1 = r_A;
                        if (flagN == Flag_Status.clear)
                        {
                            if (flagH == Flag_Status.set || ((t1 & 0xF) > 9))
                                t1 += 0x06;
                            if (flagC == Flag_Status.set || (t1 > 0x9F))
                                t1 += 0x60;
                        }
                        else
                        {
                            if (flagH == Flag_Status.set)
                                t1 = (ushort)((t1 - 6) & 0xFF);
                            if (flagC == Flag_Status.set)
                                t1 -= 0x60;
                        }
                        flagH = Flag_Status.clear;
                        if ((t1 & 0x100) == 0x100)
                            flagC = Flag_Status.set;
                        t1 &= 0xff;
                        if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                        r_A = (byte)t1;
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x2F://CPL
                    flagN = Flag_Status.set;
                    flagH = Flag_Status.set;
                    r_A = (byte)(~r_A);
                    Cpu_cycles = 4;
                    break;

                case 0x3F://CCF
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    if (flagC == Flag_Status.clear)
                        flagC = Flag_Status.set;
                    else
                        flagC = Flag_Status.clear;
                    Cpu_cycles = 4;
                    break;

                case 0x37://SCF 11/9 fixed
                    flagN = Flag_Status.clear;
                    flagH = Flag_Status.clear;
                    flagC = Flag_Status.set;
                    break;

                case 0x00: //NOP
                    Cpu_cycles = 4;
                    break;

                case 0x76://HALT
                    flagHalt = true;
                    Cpu_cycles = 4;
                    break;

                case 0x10://STOP
                    //flagStop = true;
                    Console.WriteLine("stop:" + r_PC.ToString("x2"));
                    //MessageBox.Show
                    Cpu_cycles = 4;
                    break;

                case 0xF3://DI
                    flagIME = false;
                    Cpu_cycles = 4;
                    break;

                case 0xFB://EI
                    flagIME = true;
                    Cpu_cycles = 4;
                    break;
                #endregion
                #region jumps , calls ,restarts , returns
                case 0xc3: //JP nn
                    {

                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xC2: //JP NZ,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 0)
                            r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xCA://JP Z,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 1)
                            r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xD2://JP NC,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 0)
                            r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xDA://JP C,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 1)
                            r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xE9://JP  (HL)
                    {
                        r_PC = (ushort)(r_H << 8 | r_L);
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x18://JR n
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        r_PC = (ushort)(r_PC + t1);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x20: //JR NZ,n (signed byte)
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 0)
                            r_PC = (ushort)(r_PC + t1);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x28://JR Z,n
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 1)
                            r_PC = (ushort)(r_PC + t1);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x30://JR NC,n
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 0)
                            r_PC = (ushort)(r_PC + t1);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0x38://JR C,n
                    {
                        sbyte t1 = (sbyte)GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 1)
                            r_PC = (ushort)(r_PC + t1);
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xCD: //CALL nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = (ushort)(t2 << 8 | t1);
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xC4://CALL NZ,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 0)
                        {
                            GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                            GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                            r_SP -= 2;
                            r_PC = (ushort)(t2 << 8 | t1);
                        }
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xCC://CALL Z,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagZ == 1)
                        {
                            GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                            GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                            r_SP -= 2;
                            r_PC = (ushort)(t2 << 8 | t1);
                        }
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xD4://CALL NC,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 0)
                        {
                            GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                            GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                            r_SP -= 2;
                            r_PC = (ushort)(t2 << 8 | t1);
                        }
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xDC://CALL C,nn
                    {
                        byte t1 = GB_MEM_r8(r_PC++);
                        byte t2 = GB_MEM_r8(r_PC++);
                        if ((byte)flagC == 1)
                        {
                            GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                            GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                            r_SP -= 2;
                            r_PC = (ushort)(t2 << 8 | t1);
                        }
                        Cpu_cycles = 12;
                    }
                    break;

                case 0xC7: //RST 00H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xCF://RST 08H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x08;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xD7://RST 10H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x10;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xDF://RST 18H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x18;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xE7://RST 20H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x20;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xEF://RST 28H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x28;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xF7://RST 30H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x30;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xFF://RST 38H
                    {
                        GB_MEM_w8((ushort)(r_SP - 1), (byte)(r_PC >> 8));
                        GB_MEM_w8((ushort)(r_SP - 2), (byte)(r_PC & 0xFF));
                        r_SP -= 2;
                        r_PC = 0x38;
                        Cpu_cycles = 32;
                    }
                    break;

                case 0xC9://RET
                    {
                        byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                        byte t2 = GB_MEM_r8(r_SP);
                        r_PC = (ushort)(t1 << 8 | t2);
                        r_SP += 2;
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xC0://RET NZ
                    {
                        if (flagZ == Flag_Status.clear)
                        {
                            byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                            byte t2 = GB_MEM_r8(r_SP);
                            r_PC = (ushort)(t1 << 8 | t2);
                            r_SP += 2;
                        }
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xC8://RET Z
                    {
                        if (flagZ == Flag_Status.set)
                        {
                            byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                            byte t2 = GB_MEM_r8(r_SP);
                            r_PC = (ushort)(t1 << 8 | t2);
                            r_SP += 2;
                        }
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xD0: //RET NC
                    {
                        if (flagC == Flag_Status.clear)
                        {
                            byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                            byte t2 = GB_MEM_r8(r_SP);
                            r_PC = (ushort)(t1 << 8 | t2);
                            r_SP += 2;
                        }
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xD8://RET C
                    {
                        if (flagC == Flag_Status.set)
                        {
                            byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                            byte t2 = GB_MEM_r8(r_SP);
                            r_PC = (ushort)(t1 << 8 | t2);
                            r_SP += 2;
                        }
                        Cpu_cycles = 8;
                    }
                    break;

                case 0xD9:
                    {
                        // MessageBox.Show("RETI opcode unfinish");
                        byte t1 = GB_MEM_r8((ushort)(r_SP + 1));
                        byte t2 = GB_MEM_r8(r_SP);
                        r_PC = (ushort)(t1 << 8 | t2);
                        r_SP += 2;
                        flagIME = true;
                        Cpu_cycles = 8;
                    }
                    break;

                #endregion
                #region Rotates & Shifts without CB prefix

                case 0x07://RLCA
                    {

                        flagZ = Flag_Status.clear; // Z flag 被清除才是正確的,非官方規格文件描述許多有誤,被搞死.. orz...
                        flagN = Flag_Status.clear;
                        flagH = Flag_Status.clear;
                        flagC = (Flag_Status)((r_A >> 7) & 1);
                        r_A = (byte)((r_A << 1) | (byte)flagC);
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x17://RLA
                    {
                        flagZ = Flag_Status.clear;
                        flagN = Flag_Status.clear;
                        flagH = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        flagC = (Flag_Status)(r_A >> 7 & 1);
                        r_A <<= 1;
                        r_A |= t1;
                        Cpu_cycles = 4;
                    }
                    break;
                case 0x0F://RRCA
                    {
                        flagZ = Flag_Status.clear;
                        flagN = Flag_Status.clear;
                        flagH = Flag_Status.clear;
                        flagC = (Flag_Status)(r_A & 1);
                        r_A = (byte)((r_A >> 1) | ((byte)flagC << 7));
                        Cpu_cycles = 4;
                    }
                    break;

                case 0x1F://RRA
                    {
                        flagZ = Flag_Status.clear;
                        flagN = Flag_Status.clear;
                        flagH = Flag_Status.clear;
                        byte t1 = (byte)flagC;
                        flagC = (Flag_Status)(r_A & 1);
                        r_A = (byte)((r_A >> 1) | (t1 << 7));
                        //if (rA == 0) flagZ = flag_status.set; else flagZ = flag_status.clear;
                        Cpu_cycles = 4;
                    }
                    break;
                #endregion
                #region Opcode with 0xCB
                case 0xCb:
                    byte cb_code = GB_MEM_r8(r_PC++);
                    #region Bit opcode
                    if ((cb_code & 0xC0) == 0x40) //BIT
                    {
                        flagN = Flag_Status.clear;
                        flagH = Flag_Status.set;

                        byte b = (byte)((cb_code & 0x38) >> 3);
                        byte reg = (byte)(cb_code & 7);


                        if (reg == 7) //rA
                        {
                            if ((byte)((r_A >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 0)//rB
                        {
                            if ((byte)((r_B >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 1) //rC
                        {
                            if ((byte)((r_C >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 2) //rD
                        {
                            if ((byte)((r_D >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 3) //rE
                        {
                            if ((byte)((r_E >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 4) //rH bit test
                        {
                            if ((byte)((r_H >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 5) //rL
                        {
                            if ((byte)((r_L >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 8;
                        }
                        else if (reg == 6) //(HL)
                        {
                            if (((GB_MEM_r8((ushort)(r_H << 8 | r_L)) >> b) & 1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                            Cpu_cycles = 16;
                        }

                        break;
                    }
                    else if ((cb_code & 0xC0) == 0xC0) //SET
                    {
                        byte b = (byte)((cb_code & 0x38) >> 3);
                        byte reg = (byte)(cb_code & 7);

                        if (reg == 7) //rA
                        {
                            r_A = (byte)(r_A | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 0)//rB
                        {
                            r_B = (byte)(r_B | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 1) //rC
                        {
                            r_C = (byte)(r_C | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 2) //rD
                        {
                            r_D = (byte)(r_D | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 3) //rE
                        {
                            r_E = (byte)(r_E | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 4) //rH
                        {
                            r_H = (byte)(r_H | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 5) //rL
                        {
                            r_L = (byte)(r_L | (1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 6) //(HL)
                        {
                            GB_MEM_w8((ushort)(r_H << 8 | r_L), (byte)(GB_MEM_r8((ushort)(r_H << 8 | r_L)) | (1 << b)));
                            Cpu_cycles = 16;
                        }

                        break;
                    }
                    else if ((cb_code & 0xC0) == 0x80) //RES
                    {
                        byte b = (byte)((cb_code & 0x38) >> 3);
                        byte reg = (byte)(cb_code & 7);

                        if (reg == 7) //rA
                        {
                            r_A = (byte)(r_A & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 0)//rB
                        {
                            r_B = (byte)(r_B & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 1) //rC
                        {
                            r_C = (byte)(r_C & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 2) //rD
                        {
                            r_D = (byte)(r_D & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 3) //rE
                        {
                            r_E = (byte)(r_E & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 4) //rH
                        {
                            r_H = (byte)(r_H & ~(1 << b));
                        }
                        else if (reg == 5) //rL
                        {
                            r_L = (byte)(r_L & ~(1 << b));
                            Cpu_cycles = 8;
                        }
                        else if (reg == 6) //(HL)
                        {
                            GB_MEM_w8((ushort)(r_H << 8 | r_L), (byte)(GB_MEM_r8((ushort)(r_H << 8 | r_L)) & ~(1 << b)));
                            Cpu_cycles = 16;
                        }


                        break;
                    }
                    #endregion
                    #region  Rotates & Shifts with CB prefix
                    switch (cb_code)
                    {
                        case 0x07://RLC A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_A >> 7 & 1);
                                r_A <<= 1;
                                r_A |= (byte)flagC;
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x00://RLC B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_B >> 7 & 1);
                                r_B <<= 1;
                                r_B |= (byte)flagC;
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x01://RLC C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_C >> 7 & 1);
                                r_C <<= 1;
                                r_C |= (byte)flagC;
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x02://RLC D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_D >> 7 & 1);
                                r_D <<= 1;
                                r_D |= (byte)flagC;
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x03://RLC E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_E >> 7 & 1);
                                r_E <<= 1;
                                r_E |= (byte)flagC;
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x04://RLC H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_H >> 7 & 1);
                                r_H <<= 1;
                                r_H |= (byte)flagC;
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x05://RLC L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_L >> 7 & 1);
                                r_L <<= 1;
                                r_L |= (byte)flagC;
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x06://RLC (HL)
                            {
                                byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(t1 >> 7 & 1);
                                t1 <<= 1;
                                t1 |= (byte)flagC;
                                GB_MEM_w8((ushort)(r_H << 8 | r_L), t1);
                                if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x17://RL A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_A >> 7 & 1);
                                r_A = (byte)(r_A << 1 | t1);
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x10://RL B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_B >> 7 & 1);
                                r_B = (byte)(r_B << 1 | t1);
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x11://RL C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_C >> 7 & 1);
                                r_C = (byte)(r_C << 1 | t1);
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x12://RL D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_D >> 7 & 1);
                                r_D = (byte)(r_D << 1 | t1);
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x13://RL E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_E >> 7 & 1);
                                r_E = (byte)(r_E << 1 | t1);
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x14://RL H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_H >> 7 & 1);
                                r_H = (byte)(r_H << 1 | t1);
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x15://RL L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_L >> 7 & 1);
                                r_L = (byte)(r_L << 1 | t1);
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x16://RL (HL)
                            {
                                ushort t1 = (ushort)(r_H << 8 | r_L);
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t2 = (byte)flagC;
                                flagC = (Flag_Status)(GB_MEM_r8(t1) >> 7 & 1);
                                GB_MEM_w8(t1, (byte)(GB_MEM_r8(t1) << 1 | t2));
                                if (GB_MEM_r8(t1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x0F://RRC A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_A & 1);
                                r_A = (byte)((r_A >> 1) | ((byte)flagC << 7));
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x08://RRC B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_B & 1);
                                r_B = (byte)((r_B >> 1) | ((byte)flagC << 7));
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x09://RRC C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_C & 1);
                                r_C = (byte)((r_C >> 1) | ((byte)flagC << 7));
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x0A://RRC D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_D & 1);
                                r_D = (byte)((r_D >> 1) | ((byte)flagC << 7));
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x0B://RRC E
                            {
                                // flagZ = flag_status.clear;
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_E & 1);
                                r_E = (byte)((r_E >> 1) | ((byte)flagC << 7));
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x0C://RRC H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_H & 1);
                                r_H = (byte)((r_H >> 1) | ((byte)flagC << 7));
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x0D://RRC L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_L & 1);
                                r_L = (byte)((r_L >> 1) | ((byte)flagC << 7));
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x0E://RRC (HL)
                            {
                                ushort t1 = (ushort)(r_H << 8 | r_L);
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(GB_MEM_r8(t1) & 1);
                                GB_MEM_w8(t1, (byte)((GB_MEM_r8(t1) >> 1) | ((byte)flagC << 7)));
                                if (GB_MEM_r8(t1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x1F://RR A 
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_A & 1);
                                r_A = (byte)((r_A >> 1) | (t1 << 7));
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x18://RR B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_B & 1);
                                r_B = (byte)((r_B >> 1) | (t1 << 7));
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x19://RR C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_C & 1);
                                r_C = (byte)((r_C >> 1) | (t1 << 7));
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x1A://RR D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_D & 1);
                                r_D = (byte)((r_D >> 1) | (t1 << 7));
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x1B://RR E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_E & 1);
                                r_E = (byte)((r_E >> 1) | (t1 << 7));
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x1C://RR H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_H & 1);
                                r_H = (byte)((r_H >> 1) | (t1 << 7));
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x1D://RR L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t1 = (byte)flagC;
                                flagC = (Flag_Status)(r_L & 1);
                                r_L = (byte)((r_L >> 1) | (t1 << 7));
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x1E://RR (HL)
                            {
                                byte t1 = GB_MEM_r8((ushort)(r_H << 8 | r_L));
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                byte t2 = (byte)flagC;
                                flagC = (Flag_Status)(t1 & 1);


                                t1 = (byte)((t1 >> 1) | (t2 << 7));
                                GB_MEM_w8((ushort)(r_H << 8 | r_L), t1);
                                if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x27://SLA A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_A >> 7);
                                r_A <<= 1;
                                r_A &= 0xFE;
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x20://SLA B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_B >> 7);
                                r_B <<= 1;
                                r_B &= 0xFE;
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x21://SLA C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_C >> 7);
                                r_C <<= 1;
                                r_C &= 0xFE;
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x22://SLA D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_D >> 7);
                                r_D <<= 1;
                                r_D &= 0xFE;
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x23://SLA E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_E >> 7);
                                r_E <<= 1;
                                r_E &= 0xFE;
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x24://SLA H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_H >> 7);
                                r_H <<= 1;
                                r_H &= 0xFE;
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x25://SLA L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_L >> 7);
                                r_L <<= 1;
                                r_L &= 0xFE;
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x26://SLA (HL)
                            {
                                ushort t1 = (ushort)(r_H << 8 | r_L);
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(GB_MEM_r8(t1) >> 7);
                                GB_MEM_w8(t1, (byte)(GB_MEM_r8(t1) << 1));
                                GB_MEM_w8(t1, (byte)(GB_MEM_r8(t1) & 0xFE));
                                if (GB_MEM_r8(t1) == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x2F://SRA A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_A & 1);
                                byte t1 = (byte)(r_A >> 7);
                                r_A = (byte)((r_A >> 1) | (t1 << 7));
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x28://SRA B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_B & 1);
                                byte t1 = (byte)(r_B >> 7);
                                r_B = (byte)((r_B >> 1) | (t1 << 7));
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x29://SRA C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_C & 1);
                                byte t1 = (byte)(r_C >> 7);
                                r_C = (byte)((r_C >> 1) | (t1 << 7));
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x2A://SRA D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_D & 1);
                                byte t1 = (byte)(r_D >> 7);
                                r_D = (byte)((r_D >> 1) | (t1 << 7));
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x2B://SRA E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_E & 1);
                                byte t1 = (byte)(r_E >> 7);
                                r_E = (byte)((r_E >> 1) | (t1 << 7));
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x2C://SRA H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_H & 1);
                                byte t1 = (byte)(r_H >> 7);
                                r_H = (byte)((r_H >> 1) | (t1 << 7));
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x2D://SRA L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_L & 1);
                                byte t1 = (byte)(r_L >> 7);
                                r_L = (byte)((r_L >> 1) | (t1 << 7));
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x2E://SRA (HL)
                            {
                                ushort t1 = (ushort)(r_H << 8 | r_L);
                                byte t2 = GB_MEM_r8(t1);
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(t2 & 1);
                                byte t3 = (byte)(t2 >> 7);
                                t2 = (byte)((t2 >> 1) | (t3 << 7));
                                if (t2 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                GB_MEM_w8(t1, t2);

                                Cpu_cycles = 16;
                            }
                            break;

                        case 0x3F://SRL A
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_A & 1);
                                r_A = (byte)((r_A >> 1) & 0x7f);
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x38://SRL B
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_B & 1);
                                r_B = (byte)((r_B >> 1) & 0x7f);
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x39://SRL C
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_C & 1);
                                r_C = (byte)((r_C >> 1) & 0x7f);
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x3A: //SRL D
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_D & 1);
                                r_D = (byte)((r_D >> 1) & 0x7f);
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x3B://SRL E
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_E & 1);
                                r_E = (byte)((r_E >> 1) & 0x7f);
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x3C://SRL H
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_H & 1);
                                r_H = (byte)((r_H >> 1) & 0x7f);
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x3D://SRL L
                            {
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(r_L & 1);
                                r_L = (byte)((r_L >> 1) & 0x7f);
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x3E://SRL (HL)
                            {
                                ushort t1 = (ushort)(r_H << 8 | r_L);
                                byte t2 = GB_MEM_r8(t1);
                                flagN = Flag_Status.clear;
                                flagH = Flag_Status.clear;
                                flagC = (Flag_Status)(t2 & 1);
                                t2 = (byte)((t2 >> 1) & 0x7f);
                                if (t2 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                GB_MEM_w8(t1, t2);
                                Cpu_cycles = 16;
                            }
                            break;

                        //SWAP START fix 11/20
                        case 0x37: //SWAP A
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_A = (byte)(((r_A & 0xF) << 4) | ((r_A & 0xF0) >> 4));
                                if (r_A == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x30: //SWAP B
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_B = (byte)(((r_B & 0xF) << 4) | ((r_B & 0xF0) >> 4));
                                if (r_B == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x31: //SWAP C
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_C = (byte)(((r_C & 0xF) << 4) | ((r_C & 0xF0) >> 4));
                                if (r_C == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x32://SWAP D
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_D = (byte)(((r_D & 0xF) << 4) | ((r_D & 0xF0) >> 4));
                                if (r_D == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x33://SWAP E
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_E = (byte)(((r_E & 0xF) << 4) | ((r_E & 0xF0) >> 4));
                                if (r_E == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x34://SWAP H
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_H = (byte)(((r_H & 0xF) << 4) | ((r_H & 0xF0) >> 4));
                                if (r_H == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x35://SWAP L
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                r_L = (byte)(((r_L & 0xF) << 4) | ((r_L & 0xF0) >> 4));
                                if (r_L == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 8;
                            }
                            break;

                        case 0x36://SWAP (HL)
                            {
                                flagN = flagH = flagC = Flag_Status.clear;
                                byte t1 = (byte)(((GB_MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF) << 4) | ((GB_MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF0) >> 4));
                                GB_MEM_w8((ushort)(r_H << 8 | r_L), (byte)(((GB_MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF) << 4) | ((GB_MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF0) >> 4)));
                                if (t1 == 0) flagZ = Flag_Status.set; else flagZ = Flag_Status.clear;
                                Cpu_cycles = 16;
                            }
                            break;
                        //SWAP END
                    }
                    #endregion
                    break;
                #endregion
                default:
                    MessageBox.Show("unkonw opcode ! - " + opcode.ToString("X2"));
                    break;
            }
        }
    }
}
