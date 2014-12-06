using System;
namespace AprEmu.GB
{
    public partial class Apr_GB
    {

        byte[][] GB_SwitchableRAM = new byte[31][];

        private byte GB_MEM_r8(ushort address)
        {
            if (address <= 0xFF)
                if (disable_boot == false) //讀取GB BIOS
                    return BootStrap_DMG[address];
                else
                    return GB_RomPack[address]; //讀取ROM BIOS Blank 0
            if (address < 0x4000) //固定的 bank 0
                return GB_RomPack[address];
            if (address < 0x8000)
                switch (Cartridge_type) //需要實作更多MBC特性
                {
                    case 0:
                        return GB_RomPack[address]; //only 32KB ROM
                    case 1:
                    case 2:
                        return GB_RomPack[address + (rom_bank_select - 1) * 0x4000]; //ROM+MBC1
                }
            if (address < 0xA000) return GB_MEM[address]; //vram
            if (address < 0xC000)
                switch (Cartridge_type)
                {
                    case 0:
                    case 1:
                        return GB_MEM[address];
                    case 2:
                        if (ram_bank_select == 1)
                            return GB_MEM[address]; // GB_RomPack[address + (rom_bank_select - 1) * 0x4000]; //ROM+MBC1
                        else
                            return GB_SwitchableRAM[ram_bank_select - 2][address - 0xA000];
                }
            //return GB_MEM[address]; //需實作更完善的MBC特性
            if (address < 0xE000) return GB_MEM[address]; //Internal ram
            if (address < 0xFE00) return GB_MEM[address - 0x2000]; //Echo 8kB Internal Ram (part 7.5kB)
            return GB_MEM[address]; //剩下都是線性對應記憶體特性部分
        }
        private void GB_MEM_w8(ushort address, byte v)
        {
            if (address < 0x8000) //需要實作更多完整MBC特性 PS.不能真的寫入到rom記憶址內容,用來設定ROM Bank用途
            {
                switch (Cartridge_type)
                {
                    case 1: //mbc1 ROM+MBC1
                        {
                            if (address >= 0x2000 && address <= 0x3fff)
                                mbc1_l_bits = (byte)(v & 0x1f);
                            else if (address >= 0x4000 && address <= 0x5fff)
                                mbc1_h_bits = (byte)(v & 0xc0);// mbc1_h_bits = (byte)(v & 0x1f);
                            rom_bank_select = (byte)((mbc1_h_bits >> 1) | mbc1_l_bits);
                            if (rom_bank_select == 0) rom_bank_select += 1;
                        }
                        break;
                    case 2: //mbc1 ROM+MBC1+RAM
                        {
                            if (address >= 0x2000 && address <= 0x3fff)
                                rom_bank_select = (byte)(v & 0x1f);
                            else if (address >= 0x4000 && address <= 0x5fff)
                                ram_bank_select = (byte)(v & 3);
                            if (rom_bank_select == 0) rom_bank_select += 1;
                            if (ram_bank_select == 0) ram_bank_select += 1;
                        }
                        break;

                }
                return;
            }
            if (address <= 0xA000)//VRAM
            {

                if (v != GB_MEM[address])
                {
                    background_update = true;

                    if (address >= 0x9800 && address <= 0x9bff && v != GB_MEM[address])
                        map0_update = true;
                    else if (address >= 0x9c00 && address <= 0x9fff && v != GB_MEM[address])
                        map1_update = true;

                    if (address >= 0x8000 && address <= 0x97ff )
                        title_update = true;

                    GB_MEM[address] = v;
                    return;
                }

                return;
            }
            if (address < 0xC000) //需實作更多MBC完整特性
            {
                GB_MEM[address] = v;
                return;
            }
            if (address < 0xE000) //8kB Internal Ram
            {
                GB_MEM[address] = v;
                return;
            }
            if (address < 0xFE00) //Echo 8kB Internal Ram (part 7.5kB)
            {
                GB_MEM[address - 0x2000] = v;
                return;
            }
            //之後的記體位置都符合線性
            GB_MEM[address] = v;

            //底下是需要特別的register或是flag
            switch (address)
            {
                case reg_P1_addr:
                    {
                        byte t1 = (byte)(v & 0x30);
                        switch (t1)
                        {
                            case 0:
                                GB_MEM[reg_P1_addr] = (byte)(gbPin15 & gbPin14);
                                break;
                            case 0x10:
                                GB_MEM[reg_P1_addr] = gbPin15;
                                break;
                            case 0x20:
                                GB_MEM[reg_P1_addr] = gbPin14;
                                break;
                            case 0x30:
                                GB_MEM[reg_P1_addr] = 0xff;
                                break;
                        }
                    }
                    break;
                case reg_DIV_addr: //任何值寫入到此,都會初始化為0
                    GB_MEM[reg_DIV_addr] = 0;
                    break;
                case reg_TAC_addr:
                    {
                        switch (v & 3)
                        {
                            case 0:
                                timer_overfolow = 1024;
                                break;
                            case 1:
                                timer_overfolow = 16;
                                break;
                            case 2:
                                timer_overfolow = 64;
                                break;
                            case 3:
                                timer_overfolow = 256;
                                break;
                        }
                    }
                    break;

                case reg_OBP0_addr:
                    Palette_obj_0[0] = GB_Color_Palette[v & 3];
                    Palette_obj_0[1] = GB_Color_Palette[(v & 0xC) >> 2];
                    Palette_obj_0[2] = GB_Color_Palette[(v & 0x30) >> 4];
                    Palette_obj_0[3] = GB_Color_Palette[(v & 0xC0) >> 6];
                    break;

                case reg_OBP1_addr:
                    Palette_obj_1[0] = GB_Color_Palette[v & 3];
                    Palette_obj_1[1] = GB_Color_Palette[(v & 0xC) >> 2];
                    Palette_obj_1[2] = GB_Color_Palette[(v & 0x30) >> 4];
                    Palette_obj_1[3] = GB_Color_Palette[(v & 0xC0) >> 6];
                    break;

                case reg_BGP_addr:
                    Palette_bgp[0] = GB_Color_Palette[v & 3];
                    Palette_bgp[1] = GB_Color_Palette[(v & 0xC) >> 2];
                    Palette_bgp[2] = GB_Color_Palette[(v & 0x30) >> 4];
                    Palette_bgp[3] = GB_Color_Palette[(v & 0xC0) >> 6];
                    break;

                case reg_LCDC_addr:
                    {
                        if ((v & 0x80) == 0)
                        {
                            GB_MEM[reg_LY_addr] = 0;
                            GB_LCD_trick = 0;
                        }
                    }
                    break;
                case reg_DisBootStrap_addr:
                    disable_boot = true;
                    break;
                case reg_DMA_addr:
                    Buffer.BlockCopy(GB_MEM, v << 8, GB_MEM, 0xfe00, 160);
                    break;
            }
        }
    }
}
