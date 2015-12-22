#define debug
using System;
using System.Windows.Forms;
namespace AprEmu.GB
{
    public partial class Apr_GB
    {
        private void GB_CPU_exec()
        {
            //temp var
            byte t1_b, t2_b, t3_b;
            ushort t1_us, t2_us;
            sbyte t1_sb;

            byte opcode = MEM_r8(r_PC);
#if debug
            Debug(opcode);
#endif
            r_PC++;
            cycles += mCycleTable[opcode];
            switch (opcode)
            {
                //checked 2014.11.08
                #region 8bit Load
                case 0x06: //LD B,n
                    r_B = MEM_r8(r_PC++);
                    break;
                case 0x0E: //LD C,n
                    r_C = MEM_r8(r_PC++);
                    break;
                case 0x16: //LD D,n
                    r_D = MEM_r8(r_PC++);
                    break;
                case 0x1E: //LD E,n
                    r_E = MEM_r8(r_PC++);
                    break;
                case 0x26: //LD H,n
                    r_H = MEM_r8(r_PC++);
                    break;
                case 0x2E: //LD L,n
                    r_L = MEM_r8(r_PC++);
                    break;
                case 0x7F: //LD A,A                    
                    break;
                case 0x78: //LD A,B
                    r_A = r_B;
                    break;
                case 0x79: //LD A,C
                    r_A = r_C;
                    break;
                case 0x7A: //LD A,D
                    r_A = r_D;
                    break;
                case 0x7B://LD A,E
                    r_A = r_E;
                    break;
                case 0x7C://LD A,H
                    r_A = r_H;
                    break;
                case 0x7D://LD A,L
                    r_A = r_L;
                    break;
                case 0x7E: //LD A,(HL)
                    r_A = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x40://LD B,B                    
                    break;
                case 0x41://LD B,C
                    r_B = r_C;
                    break;
                case 0x42://LD ,B,D
                    r_B = r_D;
                    break;
                case 0x43://LD B,C
                    r_B = r_E;
                    break;
                case 0x44://LD B,H
                    r_B = r_H;
                    break;
                case 0x45://LD B,L
                    r_B = r_L;
                    break;
                case 0x46: //LD B,(HL)
                    r_B = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x48:// LD C,B
                    r_C = r_B;
                    break;
                case 0x49://LD C,C                    
                    break;
                case 0x4A://LD C,D
                    r_C = r_D;
                    break;
                case 0x4B://LD C,E
                    r_C = r_E;
                    break;
                case 0x4C: //LD C,H
                    r_C = r_H;
                    break;
                case 0x4D: //LD C,L
                    r_C = r_L;
                    break;
                case 0x4E: //LD C,(HL)
                    r_C = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x50://LD D,B
                    r_D = r_B;
                    break;
                case 0x51://LD D,C
                    r_D = r_C;
                    break;
                case 0x52://LD D,D                    
                    break;
                case 0x53:// LD D,E
                    r_D = r_E;
                    break;
                case 0x54: //LD D,H
                    r_D = r_H;
                    break;
                case 0x55://LD D,L
                    r_D = r_L;
                    break;
                case 0x56: //LD D,(HL)
                    r_D = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x58://LD E,B
                    r_E = r_B;
                    break;
                case 0x59://LD E,C
                    r_E = r_C;
                    break;
                case 0x5A: //LD E,D
                    r_E = r_D;
                    break;
                case 0x5B: //LD E,E                    
                    break;
                case 0x5c: //LD E,H
                    r_E = r_H;
                    break;
                case 0x5D: //LD E,L
                    r_E = r_L;
                    break;
                case 0x5E: //LD E,(HL)
                    r_E = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x60://LD H,B
                    r_H = r_B;
                    break;
                case 0x61://LD H,C
                    r_H = r_C;
                    break;
                case 0x62://LD H,D
                    r_H = r_D;
                    break;
                case 0x63://LD H,E
                    r_H = r_E;
                    break;
                case 0x64:// LD H,H                    
                    break;
                case 0x65://LD H,L
                    r_H = r_L;
                    break;
                case 0x66://LD H,(HL)
                    r_H = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x68: //LD L,B
                    r_L = r_B;
                    break;
                case 0x69: // LD L,C
                    r_L = r_C;
                    break;
                case 0x6A://LD L,D
                    r_L = r_D;
                    break;
                case 0x6B: //LD L,E
                    r_L = r_E;
                    break;
                case 0x6C: //LD L,H
                    r_L = r_H;
                    break;
                case 0x6D://LD L,L                    
                    break;
                case 0x6E: //LD L,(HL)
                    r_L = MEM_r8((ushort)(r_H << 8 | r_L));
                    break;
                case 0x70://LD (HL),B
                    MEM_w8((ushort)(r_H << 8 | r_L), r_B);
                    break;
                case 0x71://LD (HL),C
                    MEM_w8((ushort)(r_H << 8 | r_L), r_C);
                    break;
                case 0x72://LD (HL),D
                    MEM_w8((ushort)(r_H << 8 | r_L), r_D);
                    break;
                case 0x73://LD (HL),E
                    MEM_w8((ushort)(r_H << 8 | r_L), r_E);
                    break;
                case 0x74://LD (HL),H
                    MEM_w8((ushort)(r_H << 8 | r_L), r_H);
                    break;
                case 0x75://LD (HL),L
                    MEM_w8((ushort)(r_H << 8 | r_L), r_L);
                    break;
                case 0x36://LD (HL),n
                    MEM_w8((ushort)(r_H << 8 | r_L), MEM_r8(r_PC++));
                    break;
                case 0x0A: //LD A,(BC)
                    r_A = MEM_r8((ushort)(r_B << 8 | r_C));
                    break;
                case 0x1A://LD A,(DE)
                    r_A = MEM_r8((ushort)(r_D << 8 | r_E));
                    break;
                case 0xFA: //LD A,(nn)
                    r_A = MEM_r8((ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8)));
                    break;
                case 0x3E: // LD A,n
                    r_A = MEM_r8(r_PC++);
                    break;
                case 0x47://LD B,A
                    r_B = r_A;
                    break;
                case 0x4F://LD C,A
                    r_C = r_A;
                    break;
                case 0x57://LD D,A
                    r_D = r_A;
                    break;
                case 0x5F://LD ,E,A
                    r_E = r_A;
                    break;
                case 0x67://LD H,A
                    r_H = r_A;
                    break;
                case 0x6F://LD L,A
                    r_L = r_A;
                    break;
                case 0x02://LD (BC),A
                    MEM_w8((ushort)(r_B << 8 | r_C), r_A);
                    break;
                case 0x12://LD (DE),A
                    MEM_w8((ushort)(r_D << 8 | r_E), r_A);
                    break;
                case 0x77://LD (HL),A
                    MEM_w8((ushort)(r_H << 8 | r_L), r_A);
                    break;
                case 0xEA://LD (nn),A
                    MEM_w8((ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8)), r_A);
                    break;
                case 0xF2: //LD A,($FF00+C)
                    r_A = MEM_r8((ushort)(0xFF00 | r_C));
                    break;
                case 0xE2://LD ($FF00+C),A
                    MEM_w8((ushort)(0xFF00 | r_C), r_A);
                    break;
                case 0x3A://LD A,(HLD)
                    //fix 11/05
                    t1_us = (ushort)(r_H << 8 | r_L);
                    t2_us = (ushort)(t1_us - 1);
                    r_A = MEM_r8(t1_us);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0x32: //LD (HDL),A
                    //fix 11/05
                    t1_us = (ushort)(r_H << 8 | r_L);
                    t2_us = (ushort)(t1_us - 1);
                    MEM_w8(t1_us, r_A);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0x2A://LD A,(HLI)
                    t1_us = (ushort)(r_H << 8 | r_L);
                    t2_us = (ushort)(t1_us + 1);
                    r_A = MEM_r8(t1_us);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0x22://LD (HLI),A
                    t1_us = (ushort)((r_H << 8 | r_L) + 1);
                    MEM_w8((ushort)(r_H << 8 | r_L), r_A);
                    r_H = (byte)(t1_us >> 8);
                    r_L = (byte)(t1_us & 0xFF);
                    break;
                case 0xE0://LD ($FF00+n),A
                    MEM_w8((ushort)(0xFF00 | MEM_r8(r_PC++)), r_A);
                    break;
                case 0xF0://LD A,($FF00+n)
                    r_A = MEM_r8((ushort)(0xFF00 | MEM_r8(r_PC++)));
                    break;
                #endregion

                #region 16bit load
                case 0x01: //LD BC,nn
                    r_C = MEM_r8(r_PC++);
                    r_B = MEM_r8(r_PC++);
                    break;
                case 0x11://LD DE,nn
                    r_E = MEM_r8(r_PC++);
                    r_D = MEM_r8(r_PC++);
                    break;
                case 0x21://LD HL,nn
                    r_L = MEM_r8(r_PC++);
                    r_H = MEM_r8(r_PC++);
                    break;
                case 0x31: //LD SP,nn
                    r_SP = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    break;
                case 0xF9://LD SP,HL
                    r_SP = (ushort)(r_H << 8 | r_L);
                    break;
                case 0xF8: //LDHL SP,n
                    flagZ = FlagClear;
                    flagN = FlagClear;
                    flagC = FlagClear;
                    flagH = FlagClear;
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    t1_us = (ushort)(t1_sb + r_SP);
                    if (((r_SP ^ t1_sb ^ t1_us) & 0x100) == 0x100) flagC = FlagSet;
                    if (((r_SP ^ t1_sb ^ t1_us) & 0x10) == 0x10) flagH = FlagSet;
                    r_H = (byte)(t1_us >> 8);
                    r_L = (byte)(t1_us & 0xff);
                    break;
                case 0x08: //LD (nn),SP
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    MEM_w8((ushort)(t1_us + 1), (byte)(r_SP >> 8));
                    MEM_w8(t1_us, (byte)(r_SP & 0xFF));
                    break;
                case 0xF5: //PUSH AF
                    MEM_w8(--r_SP, r_A);
                    MEM_w8(--r_SP, (byte)(flagZ << 7 | flagN << 6 | flagH << 5 | flagC << 4));
                    break;
                case 0xC5: //PUSH BC
                    MEM_w8(--r_SP, r_B);
                    MEM_w8(--r_SP, r_C);
                    break;
                case 0xD5://PUSH DE
                    MEM_w8(--r_SP, r_D);
                    MEM_w8(--r_SP, r_E);
                    break;
                case 0xE5://PUSH HL
                    MEM_w8(--r_SP, r_H);
                    MEM_w8(--r_SP, r_L);
                    break;
                case 0xF1://POP AF 
                    // 11/25 fix
                    t1_b = MEM_r8(r_SP++);
                    flagZ = (t1_b & 0x80) >> 7;
                    flagN = (t1_b & 0x40) >> 6;
                    flagH = (t1_b & 0x20) >> 5;
                    flagC = (t1_b & 0x10) >> 4;
                    r_A = MEM_r8(r_SP++);
                    break;
                case 0xC1://POP BC
                    r_C = MEM_r8(r_SP++);
                    r_B = MEM_r8(r_SP++);
                    break;
                case 0xD1://POP DE
                    r_E = MEM_r8(r_SP++);
                    r_D = MEM_r8(r_SP++);
                    break;
                case 0xE1://POP HL
                    r_L = MEM_r8(r_SP++);
                    r_H = MEM_r8(r_SP++);
                    break;
                #endregion

                #region 8bit ALU
                case 0x87: //ADD A,A
                    flagN = FlagClear;
                    if (r_A + r_A > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_A & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_A;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x80://ADD A,B
                    flagN = FlagClear;
                    if (r_A + r_B > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_B & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_B;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x81://ADD A,C
                    flagN = FlagClear;
                    if (r_A + r_C > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_C & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_C;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x82://ADD A,D
                    flagN = FlagClear;
                    if (r_A + r_D > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_D & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_D;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x83:// ADD A,E
                    flagN = FlagClear;
                    if (r_A + r_E > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_E & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_E;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x84://ADD A,H
                    // 11/25 fixed
                    flagN = FlagClear;
                    if (r_A + r_H > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_H & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_H;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x85: //ADD A,L
                    flagN = FlagClear;
                    if (r_A + r_L > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_L & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += r_L;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x86://ADD A,(HL)
                    t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    flagN = FlagClear;
                    if (r_A + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (t1_b & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += t1_b;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xC6: //ADD A,n
                    t1_b = MEM_r8(r_PC++);
                    flagN = FlagClear;
                    if (r_A + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (t1_b & 0xF) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += t1_b;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;

                    break;
                case 0x8F: //ADC A,A
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_A + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_A & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_A);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x88: //ADC A,B
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if ((r_A + r_B + t1_b) > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if (((r_A & 0xF) + (r_B & 0xF) + t1_b) > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_B);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x89://ADC A,C
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_C + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_C & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_C);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x8A://ADC A,D
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_D + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_D & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_D);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x8B://ADC A,E
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_E + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_E & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_E);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x8C://ADC A,H
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_H + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_H & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_H);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x8D://ADC A,L
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + r_L + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (r_L & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + r_L);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x8E://ADC A,(HL)
                    t2_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + t2_b + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (t2_b & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + t2_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xCE://ADC A,n
                    //{ 
                    t2_b = MEM_r8(r_PC++);
                    flagN = FlagClear;
                    t1_b = (byte)flagC;
                    if (r_A + t2_b + t1_b > 0xFF) flagC = FlagSet; else flagC = FlagClear;
                    if ((r_A & 0xF) + (t2_b & 0xF) + t1_b > 0xF) flagH = FlagSet; else flagH = FlagClear;
                    r_A += (byte)(t1_b + t2_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x97://SUB A,A
                    flagC = FlagClear;
                    flagH = FlagClear;
                    flagN = FlagSet;
                    flagZ = FlagSet;
                    r_A = 0;
                    break;
                case 0x90://SUB A,B
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_B & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_B & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_B;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x91://SUB A,C
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_C & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_C & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_C;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x92://SUB A,D
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_D & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_D & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_D;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x93://SUB A,E
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_E & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_E & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_E;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x94://SUB A,H
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_H & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_H & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_H;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x95://SUB A,L
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_L & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_L & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= r_L;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x96://SUB A,(HL)
                    t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (t1_b & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (t1_b & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= t1_b;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xD6://SUB A,n
                    t1_b = MEM_r8(r_PC++);
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (t1_b & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (t1_b & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    r_A -= t1_b;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9F: //SBC A,A
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_A & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_A & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_A - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x98://SBC A,B
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_B & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_B & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_B - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x99://SBC A,C
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_C & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_C & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_C - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9A://SBC A,D
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_D & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_D & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_D - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9B://SBC A,E
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_E & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_E & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_E - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9C://SBC A,H
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_H & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_H & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_H - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9D://SBC A,L
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    if ((r_A & 0xF) < ((r_L & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((r_L & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - r_L - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x9E://SBC A,(HL)
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    t2_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    if ((r_A & 0xF) < ((t2_b & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((t2_b & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - t2_b - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xDE://SBC A,n
                    flagN = FlagSet;
                    t1_b = (byte)flagC;
                    t2_b = MEM_r8(r_PC++);
                    if ((r_A & 0xF) < ((t2_b & 0xF) + t1_b)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < ((t2_b & 0xFF) + t1_b)) flagC = FlagSet; else flagC = FlagClear;
                    r_A = (byte)(r_A - t2_b - t1_b);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA7: //AND A,A
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA0://AND A,B
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_B;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA1://AND A,C
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_C;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA2://AND A,D
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_D;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA3://AND A,E
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_E;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA4://AND A,H
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_H;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA5://AND A,L
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= r_L;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA6://AND A,(HL)
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xE6://AND A,n
                    flagC = FlagClear;
                    flagH = FlagSet;
                    flagN = FlagClear;
                    r_A &= MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB7://OR A,A
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB0://OR A,B
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_B;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB1://OR A,C
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_C;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB2://OR A,D
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_D;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB3://OR A,E
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_E;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB4://OR A,H
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_H;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB5://OR A,L
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= r_L;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB6://OR A,(HL)
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xF6://OR A,n
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A |= MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAF://XOR A,A
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_A;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA8://XOR A,B
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_B;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xA9://XOR A,C
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_C;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAA://XOR A,D
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_D;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAB://XOR A,E
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_E;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAC://XOR A,H
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_H;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAD://XOR A,L
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= r_L;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xAE://XOR A,(HL)
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= MEM_r8((ushort)(r_H << 8 | r_L));
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xEE: //XOR A,n
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagClear;
                    r_A ^= MEM_r8(r_PC++);
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBF: //CP A
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_A & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_A & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    flagZ = FlagSet;
                    break;
                case 0xB8://CP B
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_B & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_B & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_B) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xB9://CD C
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_C & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_C & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_C) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBA://CP D
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_D & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_D & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_D) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBB://CP E
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_E & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_E & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_E) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBC://CP H
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_H & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_H & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_H) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBD://CP L
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (r_L & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (r_L & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == r_L) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xBE://CP (HL)
                    t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (t1_b & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (t1_b & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == t1_b) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0xFE: //CP A,n
                    t1_b = MEM_r8(r_PC++);
                    flagN = FlagSet;
                    if ((r_A & 0xF) < (t1_b & 0xF)) flagH = FlagSet; else flagH = FlagClear;
                    if ((r_A & 0xFF) < (t1_b & 0xFF)) flagC = FlagSet; else flagC = FlagClear;
                    if (r_A == t1_b) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x3C://INC A
                    flagN = FlagClear;
                    r_A++;
                    if ((r_A & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x04://INC B
                    flagN = FlagClear;
                    r_B++;
                    if ((r_B & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x0C: //INC C
                    flagN = FlagClear;
                    r_C++;
                    if ((r_C & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x14: //INC D
                    flagN = FlagClear;
                    r_D++;
                    if ((r_D & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x1C: //INC E
                    flagN = FlagClear;
                    r_E++;
                    if ((r_E & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x24: //INC H
                    flagN = FlagClear;
                    r_H++;
                    if ((r_H & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x2C: //INC L
                    flagN = FlagClear;
                    r_L++;
                    if ((r_L & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x34: //INC (HL)
                    t1_b = (byte)(MEM_r8((ushort)(r_H << 8 | r_L)));
                    flagN = FlagClear;
                    t1_b++;
                    if ((t1_b & 0xF) == 0) flagH = FlagSet; else flagH = FlagClear;
                    if (t1_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    MEM_w8((ushort)(r_H << 8 | r_L), t1_b);
                    break;
                case 0x3D: //DEC A
                    flagN = FlagSet;
                    r_A--;
                    if ((r_A & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x05://DEC B
                    flagN = FlagSet;
                    r_B--;
                    if ((r_B & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x0D://DEC C
                    flagN = FlagSet;
                    r_C--;
                    if ((r_C & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x15://DEC D
                    flagN = FlagSet;
                    r_D--;
                    if ((r_D & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x1D://DEC E
                    flagN = FlagSet;
                    r_E--;
                    if ((r_E & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x25://DEC H
                    flagN = FlagSet;
                    r_H--;
                    if ((r_H & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x2D://DEC L
                    flagN = FlagSet;
                    r_L--;
                    if ((r_L & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                case 0x35://DEC (HL)
                    t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                    t1_b--;
                    MEM_w8((ushort)(r_H << 8 | r_L), t1_b);
                    flagN = FlagSet;
                    if ((t1_b & 0xF) == 0xF) flagH = FlagSet; else flagH = FlagClear;
                    if (t1_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    break;
                #endregion

                #region 16bit ALU
                case 0x09: //ADD HL,BC
                    flagN = FlagClear;
                    t1_us = (ushort)(r_B << 8 | r_C);
                    t2_us = (ushort)(r_H << 8 | r_L);
                    if ((t1_us + t2_us) > 0xFFFF) flagC = FlagSet; else flagC = FlagClear;
                    if (((t1_us & 0xFFF) + (t2_us & 0XFFF)) > 0xFFF) flagH = FlagSet; else flagH = FlagClear;
                    t2_us = (ushort)((t1_us + t2_us) & 0xFFFF);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0x19://ADD HL,DE
                    flagN = FlagClear;
                    t1_us = (ushort)(r_D << 8 | r_E);
                    t2_us = (ushort)(r_H << 8 | r_L);
                    if ((t1_us + t2_us) > 0xFFFF) flagC = FlagSet; else flagC = FlagClear;
                    if (((t1_us & 0xFFF) + (t2_us & 0XFFF)) > 0xFFF) flagH = FlagSet; else flagH = FlagClear;
                    t2_us = (ushort)((t1_us + t2_us) & 0xFFFF);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0x29://ADD HL,HL
                    flagN = FlagClear;
                    t1_us = (ushort)(r_H << 8 | r_L);
                    if ((t1_us + t1_us) > 0xFFFF) flagC = FlagSet; else flagC = FlagClear;
                    if (((t1_us & 0xFFF) + (t1_us & 0XFFF)) > 0xFFF) flagH = FlagSet; else flagH = FlagClear;
                    t1_us = (ushort)((t1_us + t1_us) & 0xFFFF);
                    r_H = (byte)(t1_us >> 8);
                    r_L = (byte)(t1_us & 0xFF);
                    break;
                case 0x39://ADD HL,SP
                    flagN = FlagClear;
                    t1_us = r_SP;
                    t2_us = (ushort)(r_H << 8 | r_L);
                    if ((t1_us + t2_us) > 0xFFFF) flagC = FlagSet; else flagC = FlagClear;
                    if (((t1_us & 0xFFF) + (t2_us & 0XFFF)) > 0xFFF) flagH = FlagSet; else flagH = FlagClear;
                    t2_us = (ushort)((t1_us + t2_us) & 0xFFFF);
                    r_H = (byte)(t2_us >> 8);
                    r_L = (byte)(t2_us & 0xFF);
                    break;
                case 0xE8://ADD SP,n
                    flagZ = FlagClear;
                    flagN = FlagClear;
                    flagC = FlagClear;
                    flagH = FlagClear;
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    int res = r_SP + t1_sb;
                    if (((r_SP ^ t1_sb ^ (res & 0xffff)) & 0x100) == 0x100) flagC = FlagSet;
                    if (((r_SP ^ t1_sb ^ (res & 0xffff)) & 0x10) == 0x10) flagH = FlagSet;
                    r_SP = (ushort)(r_SP + t1_sb);
                    break;
                case 0x03: //INC BC
                    t1_us = (ushort)((r_B << 8 | r_C) + 1);
                    r_B = (byte)(t1_us >> 8);
                    r_C = (byte)(t1_us & 0xff);
                    break;
                case 0x13://INC DE
                    t1_us = (ushort)((r_D << 8 | r_E) + 1);
                    r_D = (byte)(t1_us >> 8);
                    r_E = (byte)(t1_us & 0xff);
                    break;
                case 0x23://INC HL
                    t1_us = (ushort)((r_H << 8 | r_L) + 1);
                    r_H = (byte)(t1_us >> 8);
                    r_L = (byte)(t1_us & 0xff);
                    break;
                case 0x33://INC SP
                    ++r_SP;
                    break;
                case 0x0B: //DEC BC
                    t1_us = (ushort)((r_B << 8 | r_C) - 1);
                    r_B = (byte)(t1_us >> 8);
                    r_C = (byte)(t1_us & 0xff);
                    break;
                case 0x1B://DEC DE
                    t1_us = (ushort)((r_D << 8 | r_E) - 1);
                    r_D = (byte)(t1_us >> 8);
                    r_E = (byte)(t1_us & 0xff);
                    break;
                case 0x2B://DEC HL
                    t1_us = (ushort)((r_H << 8 | r_L) - 1);
                    r_H = (byte)(t1_us >> 8);
                    r_L = (byte)(t1_us & 0xff);
                    break;
                case 0x3B://DEC SP
                    --r_SP;
                    break;
                #endregion

                #region miscellaneous
                //SWAP move to Rotates & shifts
                case 0x27: //DAA
                    // REF https://github.com/drhelius/Gearboy/blob/master/src/opcodes.cpp  void Processor::OPCode0x27()
                    // DAA 這指令的運作,在GAMEBOY Z80上似乎有自己的特性
                    t1_us = r_A;
                    if (flagN == FlagClear)
                    {
                        if (flagH == FlagSet || ((t1_us & 0xF) > 9)) t1_us += 0x06;
                        if (flagC == FlagSet || (t1_us > 0x9F)) t1_us += 0x60;
                    }
                    else
                    {
                        if (flagH == FlagSet) t1_us = (ushort)((t1_us - 6) & 0xFF);
                        if (flagC == FlagSet) t1_us -= 0x60;
                    }
                    flagH = FlagClear;
                    if ((t1_us & 0x100) == 0x100) flagC = FlagSet;
                    t1_us &= 0xff;
                    if (t1_us == 0) flagZ = FlagSet; else flagZ = FlagClear;
                    r_A = (byte)t1_us;
                    break;
                case 0x2F://CPL
                    flagN = FlagSet;
                    flagH = FlagSet;
                    r_A = (byte)(~r_A);
                    break;
                case 0x3F://CCF
                    flagN = FlagClear;
                    flagH = FlagClear;
                    if (flagC == FlagClear) flagC = FlagSet; else flagC = FlagClear;
                    break;
                case 0x37://SCF 11/9 fixed
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = FlagSet;
                    break;
                case 0x00: //NOP
                    break;
                case 0x76://HALT
                    halt_cycle = 12;
                    flagHalt = true;
                    break;
                case 0x10://STOP
                    Console.WriteLine("stop: 0x" + r_PC.ToString("x2"));
                    break;
                case 0xF3://DI
                    flagIME = false;
                    break;
                case 0xFB://EI
                    flagIME = true;
                    break;
                #endregion
                #region jumps , calls ,restarts , returns
                case 0xc3: //JP nn
                    r_PC = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    break;
                case 0xC2: //JP NZ,nn
                    t1_b = MEM_r8(r_PC++);
                    t2_b = MEM_r8(r_PC++);
                    if (flagZ == 0)
                    {
                        cycles += 1;
                        r_PC = (ushort)(t2_b << 8 | t1_b);
                    }
                    break;
                case 0xCA://JP Z,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagZ == 1) r_PC = t1_us;
                    break;
                case 0xD2://JP NC,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagC == 0)
                    {
                        cycles += 1;
                        r_PC = t1_us;
                    }
                    break;
                case 0xDA://JP C,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagC == 1)
                    {
                        cycles += 1;
                        r_PC = t1_us;
                    }
                    break;
                case 0xE9://JP  (HL)
                    r_PC = (ushort)(r_H << 8 | r_L);
                    break;
                case 0x18://JR n
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    r_PC = (ushort)(r_PC + t1_sb);
                    break;
                case 0x20: //JR NZ,n (signed byte)
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    if (flagZ == 0)
                    {
                        cycles += 1;
                        r_PC = (ushort)(r_PC + t1_sb);
                    }
                    break;
                case 0x28://JR Z,n
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    if (flagZ == 1)
                    {
                        cycles += 1;
                        r_PC = (ushort)(r_PC + t1_sb);
                    }
                    break;
                case 0x30://JR NC,n
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    if (flagC == 0)
                    {
                        cycles += 1;
                        r_PC = (ushort)(r_PC + t1_sb);
                    }
                    break;
                case 0x38://JR C,n
                    t1_sb = (sbyte)MEM_r8(r_PC++);
                    if (flagC == 1)
                    {
                        cycles += 1;
                        r_PC = (ushort)(r_PC + t1_sb);
                    }
                    break;
                case 0xCD: //CALL nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = t1_us;
                    break;
                case 0xC4://CALL NZ,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagZ == 0)
                    {
                        cycles += 3;
                        MEM_w8(--r_SP, (byte)(r_PC >> 8));
                        MEM_w8(--r_SP, (byte)r_PC);
                        r_PC = t1_us;
                    }
                    break;
                case 0xCC://CALL Z,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagZ == 1)
                    {
                        cycles += 3;
                        MEM_w8(--r_SP, (byte)(r_PC >> 8));
                        MEM_w8(--r_SP, (byte)r_PC);
                        r_PC = t1_us;
                    }
                    break;
                case 0xD4://CALL NC,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8)); ;
                    if (flagC == 0)
                    {
                        cycles += 3;
                        MEM_w8(--r_SP, (byte)(r_PC >> 8));
                        MEM_w8(--r_SP, (byte)r_PC);
                        r_PC = t1_us;
                    }
                    break;
                case 0xDC://CALL C,nn
                    t1_us = (ushort)(MEM_r8(r_PC++) | (MEM_r8(r_PC++) << 8));
                    if (flagC == 1)
                    {
                        cycles += 3;
                        MEM_w8(--r_SP, (byte)(r_PC >> 8));
                        MEM_w8(--r_SP, (byte)r_PC);
                        r_PC = t1_us;
                    }
                    break;
                case 0xC7: //RST 00H                    
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0;
                    break;
                case 0xCF://RST 08H                    
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x08;
                    break;
                case 0xD7://RST 10H
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x10;
                    break;
                case 0xDF://RST 18H
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x18;
                    break;
                case 0xE7://RST 20H                    
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x20;
                    break;
                case 0xEF://RST 28H
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x28;
                    break;
                case 0xF7://RST 30H                    
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x30;
                    break;
                case 0xFF://RST 38H                    
                    MEM_w8(--r_SP, (byte)(r_PC >> 8));
                    MEM_w8(--r_SP, (byte)r_PC);
                    r_PC = 0x38;
                    break;
                case 0xC9://RET
                    r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    break;
                case 0xC0://RET NZ                    
                    if (flagZ == FlagClear)
                    {
                        cycles += 3;
                        r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    }
                    break;
                case 0xC8://RET Z                    
                    if (flagZ == FlagSet)
                    {
                        cycles += 3;
                        r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    }
                    break;
                case 0xD0: //RET NC                    
                    if (flagC == FlagClear)
                    {
                        cycles += 3;
                        r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    }
                    break;
                case 0xD8://RET C
                    if (flagC == FlagSet)
                    {
                        cycles += 3;
                        r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    }
                    break;
                case 0xD9:
                    r_PC = (ushort)(MEM_r8(r_SP++) | (MEM_r8(r_SP++) << 8));
                    flagIME = true;
                    break;
                #endregion
                #region Rotates & Shifts without CB prefix

                case 0x07://RLCA
                    flagZ = FlagClear; // Z flag 被清除才是正確的,非官方規格文件描述許多有誤,被搞死.. orz...
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = ((r_A >> 7) & 1);
                    r_A = (byte)((r_A << 1) | flagC);
                    break;
                case 0x17://RLA
                    flagZ = FlagClear;
                    flagN = FlagClear;
                    flagH = FlagClear;
                    t1_b = (byte)flagC;
                    flagC = (r_A >> 7 & 1);
                    r_A <<= 1;
                    r_A |= t1_b;
                    break;
                case 0x0F://RRCA
                    flagZ = FlagClear;
                    flagN = FlagClear;
                    flagH = FlagClear;
                    flagC = (r_A & 1);
                    r_A = (byte)((r_A >> 1) | (flagC << 7));
                    break;
                case 0x1F://RRA
                    flagZ = FlagClear;
                    flagN = FlagClear;
                    flagH = FlagClear;
                    t1_b = (byte)flagC;
                    flagC = (r_A & 1);
                    r_A = (byte)((r_A >> 1) | (t1_b << 7));
                    break;
                #endregion

                #region Opcode with 0xCB
                case 0xCb:
                    byte cb_code = MEM_r8(r_PC++);
                    cycles += cbMCycleTable[cb_code];
                    byte b = (byte)((cb_code & 0x38) >> 3);
                    byte reg = (byte)(cb_code & 7);
                    byte cb_op1 = (byte)(cb_code & 0xC0);

                    #region Bit opcode
                    if (cb_op1 == 0x40) //BIT
                    {
                        flagN = FlagClear;
                        flagH = FlagSet;
                        switch (reg)
                        {
                            case 0:
                                if ((byte)((r_B >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 1:
                                if ((byte)((r_C >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 2:
                                if ((byte)((r_D >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 3:
                                if ((byte)((r_E >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 4:
                                if ((byte)((r_H >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 5:
                                if ((byte)((r_L >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 6:
                                if (((MEM_r8((ushort)(r_H << 8 | r_L)) >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                            case 7:
                                if ((byte)((r_A >> b) & 1) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                break;
                        }
                        break;
                    }
                    else if (cb_op1 == 0xC0) //SET
                    {
                        switch (reg)
                        {
                            case 0:
                                r_B = (byte)(r_B | (1 << b));
                                break;
                            case 1:
                                r_C = (byte)(r_C | (1 << b));
                                break;
                            case 2:
                                r_D = (byte)(r_D | (1 << b));
                                break;
                            case 3:
                                r_E = (byte)(r_E | (1 << b));
                                break;
                            case 4:
                                r_H = (byte)(r_H | (1 << b));
                                break;
                            case 5:
                                r_L = (byte)(r_L | (1 << b));
                                break;
                            case 6:
                                MEM_w8((ushort)(r_H << 8 | r_L), (byte)(MEM_r8((ushort)(r_H << 8 | r_L)) | (1 << b)));
                                break;
                            case 7:
                                r_A = (byte)(r_A | (1 << b));
                                break;
                        }
                        break;
                    }
                    else if (cb_op1 == 0x80) //RES
                    {
                        switch (reg)
                        {
                            case 0:
                                r_B = (byte)(r_B & ~(1 << b));
                                break;
                            case 1:
                                r_C = (byte)(r_C & ~(1 << b));
                                break;
                            case 2:
                                r_D = (byte)(r_D & ~(1 << b));
                                break;
                            case 3:
                                r_E = (byte)(r_E & ~(1 << b));
                                break;
                            case 4:
                                r_H = (byte)(r_H & ~(1 << b));
                                break;
                            case 5: r_L = (byte)(r_L & ~(1 << b));
                                break;
                            case 6:
                                MEM_w8((ushort)(r_H << 8 | r_L), (byte)(MEM_r8((ushort)(r_H << 8 | r_L)) & ~(1 << b)));
                                break;
                            case 7:
                                r_A = (byte)(r_A & ~(1 << b));
                                break;
                        }
                        break;
                    }
                    else // 0x00
                    {
                        byte cb_op2 = (byte)((cb_code & 0x38) >> 3);

                        flagN = FlagClear;
                        flagH = FlagClear;

                        switch (cb_op2)
                        {
                            case 0://RLC
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            flagC = (r_B >> 7 & 1);
                                            r_B <<= 1;
                                            r_B |= (byte)flagC;
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            flagC = (r_C >> 7 & 1);
                                            r_C <<= 1;
                                            r_C |= (byte)flagC;
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            flagC = (r_D >> 7 & 1);
                                            r_D <<= 1;
                                            r_D |= (byte)flagC;
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            flagC = (r_E >> 7 & 1);
                                            r_E <<= 1;
                                            r_E |= (byte)flagC;
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            flagC = (r_H >> 7 & 1);
                                            r_H <<= 1;
                                            r_H |= (byte)flagC;
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            flagC = (r_L >> 7 & 1);
                                            r_L <<= 1;
                                            r_L |= (byte)flagC;
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                                            flagC = (t1_b >> 7 & 1);
                                            t1_b <<= 1;
                                            t1_b |= (byte)flagC;
                                            MEM_w8((ushort)(r_H << 8 | r_L), t1_b);
                                            if (t1_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            flagC = (r_A >> 7 & 1);
                                            r_A <<= 1;
                                            r_A |= (byte)flagC;
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 1://RRC
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            flagC = (r_B & 1);
                                            r_B = (byte)((r_B >> 1) | (flagC << 7));
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            flagC = (r_C & 1);
                                            r_C = (byte)((r_C >> 1) | (flagC << 7));
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            flagC = (r_D & 1);
                                            r_D = (byte)((r_D >> 1) | (flagC << 7));
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            flagC = (r_E & 1);
                                            r_E = (byte)((r_E >> 1) | (flagC << 7));
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            flagC = (r_H & 1);
                                            r_H = (byte)((r_H >> 1) | (flagC << 7));
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            flagC = (r_L & 1);
                                            r_L = (byte)((r_L >> 1) | (flagC << 7));
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_us = (ushort)(r_H << 8 | r_L);
                                            flagC = (MEM_r8(t1_us) & 1);
                                            MEM_w8(t1_us, (byte)((MEM_r8(t1_us) >> 1) | (flagC << 7)));
                                            if (MEM_r8(t1_us) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            flagC = (r_A & 1);
                                            r_A = (byte)((r_A >> 1) | (flagC << 7));
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 2://RL
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            t1_b = (byte)flagC;
                                            flagC = (r_B >> 7 & 1);
                                            r_B = (byte)(r_B << 1 | t1_b);
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            t1_b = (byte)flagC;
                                            flagC = (r_C >> 7 & 1);
                                            r_C = (byte)(r_C << 1 | t1_b);
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            t1_b = (byte)flagC;
                                            flagC = (r_D >> 7 & 1);
                                            r_D = (byte)(r_D << 1 | t1_b);
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            t1_b = (byte)flagC;
                                            flagC = (r_E >> 7 & 1);
                                            r_E = (byte)(r_E << 1 | t1_b);
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            t1_b = (byte)flagC;
                                            flagC = (r_H >> 7 & 1);
                                            r_H = (byte)(r_H << 1 | t1_b);
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            t1_b = (byte)flagC;
                                            flagC = (r_L >> 7 & 1);
                                            r_L = (byte)(r_L << 1 | t1_b);
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_us = (ushort)(r_H << 8 | r_L);
                                            t2_b = (byte)flagC;
                                            flagC = (MEM_r8(t1_us) >> 7 & 1);
                                            MEM_w8(t1_us, (byte)(MEM_r8(t1_us) << 1 | t2_b));
                                            if (MEM_r8(t1_us) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            t1_b = (byte)flagC;
                                            flagC = (r_A >> 7 & 1);
                                            r_A = (byte)(r_A << 1 | t1_b);
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 3://RR
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            t1_b = (byte)flagC;
                                            flagC = (r_B & 1);
                                            r_B = (byte)((r_B >> 1) | (t1_b << 7));
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            t1_b = (byte)flagC;
                                            flagC = (r_C & 1);
                                            r_C = (byte)((r_C >> 1) | (t1_b << 7));
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            t1_b = (byte)flagC;
                                            flagC = (r_D & 1);
                                            r_D = (byte)((r_D >> 1) | (t1_b << 7));
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            t1_b = (byte)flagC;
                                            flagC = (r_E & 1);
                                            r_E = (byte)((r_E >> 1) | (t1_b << 7));
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            t1_b = (byte)flagC;
                                            flagC = (r_H & 1);
                                            r_H = (byte)((r_H >> 1) | (t1_b << 7));
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            t1_b = (byte)flagC;
                                            flagC = (r_L & 1);
                                            r_L = (byte)((r_L >> 1) | (t1_b << 7));
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_b = MEM_r8((ushort)(r_H << 8 | r_L));
                                            t2_b = (byte)flagC;
                                            flagC = (t1_b & 1);
                                            t1_b = (byte)((t1_b >> 1) | (t2_b << 7));
                                            MEM_w8((ushort)(r_H << 8 | r_L), t1_b);
                                            if (t1_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            t1_b = (byte)flagC;
                                            flagC = (r_A & 1);
                                            r_A = (byte)((r_A >> 1) | (t1_b << 7));
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 4://SLA
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            flagC = (r_B >> 7);
                                            r_B <<= 1;
                                            r_B &= 0xFE;
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            flagC = (r_C >> 7);
                                            r_C <<= 1;
                                            r_C &= 0xFE;
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            flagC = (r_D >> 7);
                                            r_D <<= 1;
                                            r_D &= 0xFE;
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            flagC = (r_E >> 7);
                                            r_E <<= 1;
                                            r_E &= 0xFE;
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            flagC = (r_H >> 7);
                                            r_H <<= 1;
                                            r_H &= 0xFE;
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            flagC = (r_L >> 7);
                                            r_L <<= 1;
                                            r_L &= 0xFE;
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_us = (ushort)(r_H << 8 | r_L);
                                            flagC = (MEM_r8(t1_us) >> 7);
                                            MEM_w8(t1_us, (byte)(MEM_r8(t1_us) << 1));
                                            MEM_w8(t1_us, (byte)(MEM_r8(t1_us) & 0xFE));
                                            if (MEM_r8(t1_us) == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            flagC = (r_A >> 7);
                                            r_A <<= 1;
                                            r_A &= 0xFE;
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 5://SRA
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            flagC = (r_B & 1);
                                            t1_b = (byte)(r_B >> 7);
                                            r_B = (byte)((r_B >> 1) | (t1_b << 7));
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            flagC = (r_C & 1);
                                            t1_b = (byte)(r_C >> 7);
                                            r_C = (byte)((r_C >> 1) | (t1_b << 7));
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            flagC = (r_D & 1);
                                            t1_b = (byte)(r_D >> 7);
                                            r_D = (byte)((r_D >> 1) | (t1_b << 7));
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            flagC = (r_E & 1);
                                            t1_b = (byte)(r_E >> 7);
                                            r_E = (byte)((r_E >> 1) | (t1_b << 7));
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            flagC = (r_H & 1);
                                            t1_b = (byte)(r_H >> 7);
                                            r_H = (byte)((r_H >> 1) | (t1_b << 7));
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            flagC = (r_L & 1);
                                            t1_b = (byte)(r_L >> 7);
                                            r_L = (byte)((r_L >> 1) | (t1_b << 7));
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_us = (ushort)(r_H << 8 | r_L);
                                            t2_b = MEM_r8(t1_us);
                                            flagC = (t2_b & 1);
                                            t3_b = (byte)(t2_b >> 7);
                                            t2_b = (byte)((t2_b >> 1) | (t3_b << 7));
                                            if (t2_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            MEM_w8(t1_us, t2_b);
                                            break;
                                        case 7:
                                            flagC = (r_A & 1);
                                            t1_b = (byte)(r_A >> 7);
                                            r_A = (byte)((r_A >> 1) | (t1_b << 7));
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 6://SWAP
                                {
                                    flagC = FlagClear;
                                    switch (reg)
                                    {
                                        case 0:
                                            r_B = (byte)(((r_B & 0xF) << 4) | ((r_B & 0xF0) >> 4));
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            r_C = (byte)(((r_C & 0xF) << 4) | ((r_C & 0xF0) >> 4));
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            r_D = (byte)(((r_D & 0xF) << 4) | ((r_D & 0xF0) >> 4));
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            r_E = (byte)(((r_E & 0xF) << 4) | ((r_E & 0xF0) >> 4));
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            r_H = (byte)(((r_H & 0xF) << 4) | ((r_H & 0xF0) >> 4));
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            r_L = (byte)(((r_L & 0xF) << 4) | ((r_L & 0xF0) >> 4));
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_b = (byte)(((MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF) << 4) | ((MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF0) >> 4));
                                            MEM_w8((ushort)(r_H << 8 | r_L), (byte)(((MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF) << 4) | ((MEM_r8((ushort)(r_H << 8 | r_L)) & 0xF0) >> 4)));
                                            if (t1_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 7:
                                            r_A = (byte)(((r_A & 0xF) << 4) | ((r_A & 0xF0) >> 4));
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                            case 7://SRL
                                {
                                    switch (reg)
                                    {
                                        case 0:
                                            flagC = (r_B & 1);
                                            r_B = (byte)((r_B >> 1) & 0x7f);
                                            if (r_B == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 1:
                                            flagC = (r_C & 1);
                                            r_C = (byte)((r_C >> 1) & 0x7f);
                                            if (r_C == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 2:
                                            flagC = (r_D & 1);
                                            r_D = (byte)((r_D >> 1) & 0x7f);
                                            if (r_D == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 3:
                                            flagC = (r_E & 1);
                                            r_E = (byte)((r_E >> 1) & 0x7f);
                                            if (r_E == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 4:
                                            flagC = (r_H & 1);
                                            r_H = (byte)((r_H >> 1) & 0x7f);
                                            if (r_H == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 5:
                                            flagC = (r_L & 1);
                                            r_L = (byte)((r_L >> 1) & 0x7f);
                                            if (r_L == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                        case 6:
                                            t1_us = (ushort)(r_H << 8 | r_L);
                                            t2_b = MEM_r8(t1_us);
                                            flagC = (t2_b & 1);
                                            t2_b = (byte)((t2_b >> 1) & 0x7f);
                                            if (t2_b == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            MEM_w8(t1_us, t2_b);
                                            break;
                                        case 7:
                                            flagC = (r_A & 1);
                                            r_A = (byte)((r_A >> 1) & 0x7f);
                                            if (r_A == 0) flagZ = FlagSet; else flagZ = FlagClear;
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    break;
                #endregion
                default:
                    MessageBox.Show("unkonw opcode ! - " + opcode.ToString("X2"));
                    break;
            }
            cycles *= 4;

            if (DMA_CYCLE)
            {
                //Console.WriteLine("dma");
                cycles += 671;
                DMA_CYCLE = false;
            }

        }
    }
}

/*
CB OPCODE延伸碼格式說明
*** : REG number
--- : shift value

RLC 00 000 *** 
RRC 00 001 *** 
RL  00 010 *** 
RR  00 011 *** 
SLA 00 100 ***
SRA 00 101 ***
SWP 00 110 ***
SRL 00 111 ***
=================
Bit 01 --- ***
=================
RES 10 --- ***
=================
SET 11 --- ***

 */