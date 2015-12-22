using System;
using System.Windows.Forms;
using System.IO;

namespace AprEmu.GB
{
    public partial class Apr_GB
    {

        byte[] GB_SwitchableRAM = new byte[4 * 8 * 1024];

        byte pal_bg = 0;
        byte pal_obj0 = 0;
        byte pal_obj1 = 0;

        byte new_DMA_So_hi = 0, new_DMA_So_lo = 0, new_DMA_De_hi = 0, new_DMA_De_lo = 0, new_DMA_length = 0;
        NewDMAType dmaType = NewDMAType.General;
        ushort new_DMA_Source = 0, new_DMA_Dest = 0;


        bool DMA_CYCLE = false;


        private byte MEM_r8(ushort address)
        {
            if (address < bootsize)
            {
                if (disable_boot == false && runtype == HardwareType.GameBoyColor && address >= 0x100 && address < 0x200)
                {
                    return GB_RomPack[address];
                }


                if (disable_boot == false) //讀取GB BIOS
                    return BootStrapMen[address];
                else
                    return GB_RomPack[address]; //讀取ROM BIOS Blank 0
            }

            if (address < 0x4000) //固定的 bank 0
            {
                //  if (address == 0x296)
                {
                    //  Console.WriteLine("debug:" + GB_RomPack[address].ToString("x2"));
                }
                return GB_RomPack[address];
            }
            if (address < 0x8000)
                switch (Cartridge_type) //需要實作更多MBC特性
                {
                    case 0:
                        return GB_RomPack[address]; //only 32KB ROM
                    case 1:
                    case 2:
                    case 3:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                        {
                            return GB_RomPack[address + (rom_bank_select - 1) * 0x4000];
                        }
                }

            if (address < 0xA000)
            {
                if (VRAMBank1 == false)
                    return GB_MEM[address]; //vram
                else
                    return VRAM_BANK_1[address - 0x8000];
            }

            if (address < 0xC000)
            {
                switch (Cartridge_type)
                {
                    case 0:
                    case 1:
                        return GB_MEM[address];
                    case 2:
                    case 3:
                    case 0x1b:
                        {

                            return GB_SwitchableRAM[ram_bank_select * 8 * 1024 + (address - 0xA000)];

                            /*
                            if (ram_bank_select == 1)
                                return GB_MEM[address]; // GB_RomPack[address + (rom_bank_select - 1) * 0x4000]; //ROM+MBC1
                            else
                            {
                                try
                                {
                                    return GB_MEM[address];
                                    //return GB_SwitchableRAM[ram_bank_select - 2][address - 0xA000];
                                }
                                catch
                                {
                                    MessageBox.Show("d1:" + (ram_bank_select - 2).ToString());
                                }
                            }*/
                        }
                        break;
                }
            }
            //return GB_MEM[address]; //需實作更完善的MBC特性
            if (address < 0xE000) //Internal ram
            {
                if (address < 0xd000)
                {
                    return GB_MEM[address];

                }

                if (runtype == HardwareType.GameBoyColor)
                {
                    //if (CGB_WRAM_index == 0)
                    // {
                    //     return GB_MEM[address];
                    // }
                    // else
                    // {
                    return GB_MEM_WRAM_CGB_BANK[(CGB_WRAM_index - 1) * 1024 * 4 + (address - 0xd000)];
                    // }
                }
                else
                {
                    return GB_MEM[address];
                }
            }
            if (address < 0xFE00)
            {

                ushort new_address = (ushort)(address - 0x2000);
                if (new_address < 0xd000)
                {
                    return GB_MEM[new_address];
                }

                //這裡要處理CGB的RAM BANK select
                if (runtype != HardwareType.GameBoyColor)
                {
                    return GB_MEM[new_address];
                }
                else
                {
                    return GB_MEM_WRAM_CGB_BANK[(CGB_WRAM_index - 1) * 1024 * 4 + (new_address - 0xd000)];

                }//Echo 8kB Internal Ram (part 7.5kB)
            }


            // if (address >= 0xFF10 && address <= 0xFF3F)
            //     sound_read( );

            return GB_MEM[address]; //剩下都是線性對應記憶體特性部分
        }


        private void MEM_w8(ushort address, byte v)
        {
            if (address < 0x8000) //需要實作更多完整MBC特性 PS.不能真的寫入到rom記憶址內容,用來設定ROM Bank用途
            {
                switch (Cartridge_type)
                {
                    case 1: //mbc1 ROM+MBC1
                        {
                            if (address >= 0x2000 && address <= 0x3fff)
                                mbc_l_bits = (byte)(v & 0x1f);
                            else if (address >= 0x4000 && address <= 0x5fff)
                                mbc_h_bits = (byte)(v & 0x3);

                            if (mbc_l_bits == 0)
                                mbc_l_bits |= 1;

                            rom_bank_select = (byte)((mbc_h_bits << 5) | mbc_l_bits);


                            //rom_bank_select |= 1;

                            // if (rom_bank_select == 0) rom_bank_select = 1;

                            //  Console.WriteLine("rom:" + rom_bank_select);

                        }
                        break;
                    case 3:
                    case 2: //mbc1 ROM+MBC1+RAM
                        {
                            if (address >= 0x2000 && address <= 0x3fff)
                            {
                                mbc_l_bits = (byte)(v & 0x1f);
                                rom_bank_select = mbc_l_bits;
                                if (rom_bank_select == 0) rom_bank_select = 1;


                                //    Console.WriteLine("rom:" + rom_bank_select);
                            }
                            else if (address >= 0x4000 && address <= 0x5fff)
                            {
                                ram_bank_select = (byte)(v & 0x3);
                                // if (ram_bank_select == 0) ram_bank_select = 1;


                                Console.WriteLine("ram:" + ram_bank_select);


                                //MessageBox.Show("ram write ! " + ram_bank_select );
                            }





                        }
                        break;

                    case 0x1b:
                        {
                            MessageBox.Show("unsupport now !");
                        }
                        break;
                }
                return;
            }


            if (address < 0xA000)//VRAM
            {

                //VRAMBank1 = false;

                if (VRAMBank1 == false && v == GB_MEM[address])
                    return;
                if (VRAMBank1 == true && v == VRAM_BANK_1[address - 0x8000])
                    return;

                background_update = true;

                if (VRAMBank1 == false)
                {
                    if (address >= 0x9800 && address <= 0x9bff)
                    {
                        map0_update = true;
                    }
                    else if (address >= 0x9c00 && address <= 0x9fff)
                    {
                        map1_update = true;
                    }
                }
                else
                {
                    if (address >= 0x9800 && address <= 0x9bff)
                    {
                        map0_update_cgb = true;
                    }
                    else if (address >= 0x9c00 && address <= 0x9fff)
                    {
                        map1_update_cgb = true;
                    }
                }


                if (VRAMBank1 == false)
                {
                    if (address >= 0x8000 && address <= 0x97ff)
                    {
                        title_update_mark[(address - 0x8000) >> 4] = 0;
                        title_update = true;
                    }
                }
                else
                {
                    if (address >= 0x8000 && address <= 0x97ff)
                    {
                        title_update_mark_cgb[(address - 0x8000) >> 4] = 0;
                        title_update_V1 = true;
                    }
                }


                if (VRAMBank1 == true)
                    VRAM_BANK_1[address - 0x8000] = v;
                else
                    GB_MEM[address] = v;
                return;
            }
            if (address < 0xC000) //需實作更多MBC完整特性
            {
                switch (Cartridge_type)
                {
                    case 0:
                    case 1:
                        GB_MEM[address] = v;
                        break;
                    case 2:
                    case 3:
                    case 0x1b:
                        {

                            GB_SwitchableRAM[ram_bank_select * 8 * 1024 + (address - 0xA000)] = v;
                            //if (ram_bank_select == 1)
                            //    GB_MEM[address] = v; // GB_RomPack[address + (rom_bank_select - 1) * 0x4000]; //ROM+MBC1
                            //else
                            //GB_MEM[address] = v;
                            //  GB_SwitchableRAM[ram_bank_select - 2][address - 0xA000] = v;
                        }
                        break;
                }
                //GB_MEM[address] = v;
                return;
            }
            if (address < 0xE000) //8kB Internal Ram
            {

                if (address < 0xd000)
                {
                    GB_MEM[address] = v;
                    return;
                }

                //這裡要處理CGB的RAM BANK select
                if (runtype != HardwareType.GameBoyColor)
                {
                    GB_MEM[address] = v;
                }
                else
                {
                    // if (CGB_WRAM_index == 0)
                    // {
                    //     GB_MEM[address] = v;
                    //  }
                    //   else
                    //   {
                    GB_MEM_WRAM_CGB_BANK[(CGB_WRAM_index - 1) * 1024 * 4 + (address - 0xd000)] = v;
                    // }
                }
                return;
            }


            if (address < 0xFE00) //Echo 8kB Internal Ram (part 7.5kB)
            {
                ushort new_address = (ushort)(address - 0x2000);
                if (new_address < 0xd000)
                {
                    GB_MEM[new_address] = v;
                    return;
                }

                //這裡要處理CGB的RAM BANK select
                if (runtype != HardwareType.GameBoyColor)
                {
                    GB_MEM[new_address] = v;
                }
                else
                {
                    MessageBox.Show("cgb wram write echo");
                    //if (CGB_WRAM_index == 0)
                    // {
                    //     GB_MEM[new_address] = v;
                    // }
                    // else
                    // {
                    GB_MEM_WRAM_CGB_BANK[(CGB_WRAM_index - 1) * 1024 * 4 + (new_address - 0xd000)] = v;
                    //}
                }
                return;
            }

            //之後的記體位置都符合線性
            GB_MEM[address] = v;

            //  if (address >= 0xFF10 && address <= 0xFF3F)
            //      sound_write((byte)(address & 0xFF), v);



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
                    {
                        pal_obj0 = v;

                        switch (runtype)
                        {
                            case HardwareType.GameBoy:
                            case HardwareType.SuperGameBoy:
                                Palette_obj_0[0] = GB_Color_Palette_use[v & 3];
                                Palette_obj_0[1] = GB_Color_Palette_use[(v & 0xC) >> 2];
                                Palette_obj_0[2] = GB_Color_Palette_use[(v & 0x30) >> 4];
                                Palette_obj_0[3] = GB_Color_Palette_use[(v & 0xC0) >> 6];
                                break;

                            case HardwareType.GameBoyColor:
                                Palette_obj_0[0] = GB_Color_Palette_CGB_SP[v & 3];
                                Palette_obj_0[1] = GB_Color_Palette_CGB_SP[(v & 0xC) >> 2];
                                Palette_obj_0[2] = GB_Color_Palette_CGB_SP[(v & 0x30) >> 4];
                                Palette_obj_0[3] = GB_Color_Palette_CGB_SP[(v & 0xC0) >> 6];
                                break;

                        }
                    }
                    break;

                case reg_OBP1_addr:
                    {
                        pal_obj1 = v;
                        switch (runtype)
                        {
                            case HardwareType.GameBoy:
                            case HardwareType.SuperGameBoy:
                                Palette_obj_1[0] = GB_Color_Palette_use[v & 3];
                                Palette_obj_1[1] = GB_Color_Palette_use[(v & 0xC) >> 2];
                                Palette_obj_1[2] = GB_Color_Palette_use[(v & 0x30) >> 4];
                                Palette_obj_1[3] = GB_Color_Palette_use[(v & 0xC0) >> 6];
                                break;

                            case HardwareType.GameBoyColor:
                                Palette_obj_1[0] = GB_Color_Palette_CGB_SP[v & 3];
                                Palette_obj_1[1] = GB_Color_Palette_CGB_SP[(v & 0xC) >> 2];
                                Palette_obj_1[2] = GB_Color_Palette_CGB_SP[(v & 0x30) >> 4];
                                Palette_obj_1[3] = GB_Color_Palette_CGB_SP[(v & 0xC0) >> 6];
                                break;

                        }
                    }
                    break;

                case reg_BGP_addr:
                    {
                        pal_bg = v;
                        Palette_bgp[0] = GB_Color_Palette_use[v & 3];
                        Palette_bgp[1] = GB_Color_Palette_use[(v & 0xC) >> 2];
                        Palette_bgp[2] = GB_Color_Palette_use[(v & 0x30) >> 4];
                        Palette_bgp[3] = GB_Color_Palette_use[(v & 0xC0) >> 6];
                    }
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
                    {
                        //MessageBox.Show("!");

                        //  if (dis_b == 0)
                        //  {
                        //       dis_b++;
                        // }

                        // if ( dis_b == 2 )
                        disable_boot = true;

                        //  MessageBox.Show("!!!!");

                        // File.AppendAllText(@"C:\PAL\pal2.log", "#######");

                        if (runtype == HardwareType.GameBoyColor && CartridgeSupport != HardwareType.GameBoyColor)
                        {// 調色盤初始化



                            GB_Color_Palette_CGB_BG[0] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * 0 + 0 * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * 0 + 0 * 2])];
                            GB_Color_Palette_CGB_BG[1] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * 0 + 1 * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * 0 + 1 * 2])];
                            GB_Color_Palette_CGB_BG[2] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * 0 + 2 * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * 0 + 2 * 2])];
                            GB_Color_Palette_CGB_BG[3] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * 0 + 3 * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * 0 + 3 * 2])];


                            GB_Color_Palette_CGB_SP[0] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * 0 + 0 * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * 0 + 0 * 2])];
                            GB_Color_Palette_CGB_SP[1] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * 0 + 1 * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * 0 + 1 * 2])];
                            GB_Color_Palette_CGB_SP[2] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * 0 + 2 * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * 0 + 2 * 2])];
                            GB_Color_Palette_CGB_SP[3] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * 0 + 3 * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * 0 + 3 * 2])];

                        }

                    }
                    break;

                case reg_DMA_addr:
                    for (int i = 0; i < 160; i++)
                        MEM_w8((ushort)(0xfe00 + i), MEM_r8((ushort)((v << 8) + i)));
                    // Buffer.BlockCopy(GB_MEM, v << 8, GB_MEM, 0xfe00, 160);
                    DMA_CYCLE = true;
                    break;

                #region CGB only Register

                case reg_SVBK_addr:
                    CGB_WRAM_index = v & 7;
                    if (CGB_WRAM_index == 0)
                        CGB_WRAM_index = 1;
                    break;

                case reg_VBK_addr:
                    VRAMBank1 = true;
                    if ((v & 1) == 0)
                        VRAMBank1 = false;
                    break;

                case reg_BGPI_addr:
                    CGB_BG_Palette_internal_add = (v & 0x3f);
                    CGB_BG_PAL_add_auto_inc = true;
                    if ((v & 0x80) == 0)
                        CGB_BG_PAL_add_auto_inc = false;
                    break;

                case reg_BGPD_addr:
                    CGB_BG_Color_Palette[CGB_BG_Palette_internal_add] = v;
                    if (CGB_BG_PAL_add_auto_inc)
                    {
                        CGB_BG_Palette_internal_add++;
                        if (CGB_BG_Palette_internal_add > 63)
                            CGB_BG_Palette_internal_add = 0;
                    }
                    break;

                case reg_SPI_addr:
                    CGB_SP_Palette_internal_add = (v & 0x3f);
                    CGB_SP_PAL_add_auto_inc = true;
                    if ((v & 0x80) == 0)
                        CGB_SP_PAL_add_auto_inc = false;
                    break;

                case reg_SPD_addr:
                    CGB_SP_Color_Palette[CGB_SP_Palette_internal_add] = v;
                    if (CGB_SP_PAL_add_auto_inc)
                    {
                        CGB_SP_Palette_internal_add++;
                        if (CGB_SP_Palette_internal_add > 63)
                            CGB_SP_Palette_internal_add = 0;
                    }
                    break;

                case reg_KEY1_addr:
                    Console.WriteLine("cpu speed change !");
                    // editing...
                    break;

                case reg_HDMA1_addr:
                    new_DMA_So_hi = v;
                    break;

                case reg_HDMA2_addr:
                    new_DMA_So_lo = v;
                    break;

                case reg_HDMA3_addr:
                    new_DMA_De_hi = v;

                    break;

                case reg_HDMA4_addr:
                    new_DMA_De_lo = v;
                    break;

                case reg_HDMA5_addr:
                    new_DMA_length = (byte)(v & 0x7f);


                    new_DMA_Source = (ushort)((new_DMA_So_hi << 8) | new_DMA_So_lo);

                    new_DMA_Source &= 0xFFF0;

                    new_DMA_Dest = (ushort)((new_DMA_De_hi << 8) | new_DMA_De_lo);


                    new_DMA_Dest &= 0x1FF0;
                    new_DMA_Dest |= 0x8000;

                    if ((v & 0x80) != 0)
                        dmaType = NewDMAType.HBlank;
                    else
                        dmaType = NewDMAType.General;

                    if (dmaType == NewDMAType.General)
                    {


                        //  Console.WriteLine("General DMA - " + ((new_DMA_length+1)*16).ToString());
                        for (int i = 0; i < (new_DMA_length + 1) * 16; i++)
                        {
                            // Console.WriteLine( rom_bank_select  + " from : " + (new_DMA_Source + i).ToString("X4") + " to : " + (new_DMA_Dest + i).ToString("X4"));

                            //    File.AppendAllText(@"C:\PAL\pal.log", "from : " + (new_DMA_Source + i).ToString("X4") + " to : " + (new_DMA_Dest + i).ToString("X4")  +" " + GB_MEM_r8((ushort)(new_DMA_Source + i)).ToString("X2")+"\n");

                            MEM_w8((ushort)(new_DMA_Dest + i), MEM_r8((ushort)(new_DMA_Source + i)));
                        }
                        //File.AppendAllText(@"C:\PAL\pal.log", "====");
                        //  Console.WriteLine("=");
                        //  MessageBox.Show("bbb" + ((new_DMA_length + 1) * 16).ToString());
                    }
                    else
                    {
                        MessageBox.Show("HBLANK DMA !!");
                    }

                    // Console.WriteLine("CGB only:" + address.ToString("X4"));
                    break;

                case reg_UND6C_addr:
                case reg_UND72_addr:
                case reg_UND73_addr:
                case reg_UND74_addr:
                    GB_MEM[address] = v;
                    break;

                case reg_UND75_addr:
                    GB_MEM[address] = (byte)((v & 0x70) | (GB_MEM[address] & 0x8f));
                    break;

                case reg_UND76_addr:
                case reg_UND77_addr:
                    GB_MEM[address] = 0;
                    break;


                case reg_NR10_addr:
                    soundChip.channel1.setSweep((v & 0x70) >> 4, (v & 0x07), (v & 0x08) == 1);
                    break;
                case reg_NR11_addr:
                    soundChip.channel1.setDutyCycle((v & 0xC0) >> 6);
                    soundChip.channel1.setLength(v & 0x3F);
                    break;
                case reg_NR12_addr:
                    soundChip.channel1.setEnvelope((v & 0xF0) >> 4, (v & 0x07), (v & 0x08) == 8);
                    break;
                case reg_NR13_addr:
                    soundChip.channel1.setFrequency(((int)(v & 0x07) << 8) + v);
                    break;

                case reg_NR14_addr:
                    if ((GB_MEM[0xff14] & 0x80) != 0)
                    {
                        soundChip.channel1.setLength(GB_MEM[0xff11] & 0x3F);
                        soundChip.channel1.setEnvelope((GB_MEM[0xff12] & 0xF0) >> 4, (GB_MEM[0xff12] & 0x07), (GB_MEM[0xff12] & 0x08) == 8);
                    }
                    if ((GB_MEM[0xff14] & 0x40) == 0)
                        soundChip.channel1.setLength(-1);

                    soundChip.channel1.setFrequency((((int)(GB_MEM[0xff14]) & 0x07) << 8) + GB_MEM[0xff13]);
                    break;

                case reg_NR21_addr:
                    soundChip.channel2.setEnvelope((v & 0xF0) >> 4, (v & 0x07), (v & 0x08) == 8);
                    break;

                case reg_NR22_addr:
                    soundChip.channel2.setFrequency(((int)(GB_MEM[0xff19] & 0x07) << 8) + GB_MEM[0xff18]);
                    break;

                case reg_NR23_addr:
                    if ((GB_MEM[0xff19] & 0x80) != 0)
                    {
                        soundChip.channel2.setLength(GB_MEM[0xff21] & 0x3F);
                        soundChip.channel2.setEnvelope((GB_MEM[0xff17] & 0xF0) >> 4, (GB_MEM[0xff17] & 0x07), (GB_MEM[0xff17] & 0x08) == 8);
                    }
                    if ((GB_MEM[0xff19] & 0x40) == 0)
                    {
                        soundChip.channel2.setLength(-1);
                    }
                    soundChip.channel2.setFrequency(((int)(GB_MEM[0xff19] & 0x07) << 8) + GB_MEM[0xff18]);
                    break;

                case reg_NR24_addr:
                    soundChip.channel2.setDutyCycle((v & 0xC0) >> 6);
                    soundChip.channel2.setLength(v & 0x3F);
                    break;

                case reg_NR30_addr:
                    if ((v & 0x80) != 0)
                    {
                        soundChip.channel3.setVolume((GB_MEM[0xff1C] & 0x60) >> 5);
                    }
                    else
                    {
                        soundChip.channel3.setVolume(0);
                    }
                    break;

                case reg_NR31_addr:
                    soundChip.channel3.setLength(v);
                    break;

                case reg_NR32_addr:
                    soundChip.channel3.setVolume((GB_MEM[0xff1C] & 0x60) >> 5);
                    break;

                case reg_NR33_addr:
                    soundChip.channel3.setFrequency(((int)(GB_MEM[0xff1E] & 0x07) << 8) + GB_MEM[0xff1D]);
                    break;

                case reg_NR34_addr:
                    if ((GB_MEM[0xff19] & 0x80) != 0)
                    {
                        soundChip.channel3.setLength(GB_MEM[0xff1B]);
                    }
                    soundChip.channel3.setFrequency(((int)(GB_MEM[0xff1E] & 0x07) << 8) + GB_MEM[0xff1D]);
                    break;

                case reg_NR41_addr:
                    soundChip.channel4.setLength(v & 0x3F);
                    break;

                case reg_NR42_addr:
                    soundChip.channel4.setEnvelope((v & 0xF0) >> 4, (v & 0x07), (v & 0x08) == 8);
                    break;

                case reg_NR44_addr:
                    if ((GB_MEM[0xff23] & 0x80) != 0)
                    {
                        soundChip.channel4.setLength(GB_MEM[0xff20] & 0x3F);
                    }
                    if ((GB_MEM[0xff23] & 0x40) == 0)
                    {
                        soundChip.channel4.setLength(-1);
                    }
                    break;

                //case reg_NR44_addr:
                //  break;




                case 0xff30:
                case 0xff31:
                case 0xff32:
                case 0xff33:
                case 0xff34:
                case 0xff35:
                case 0xff36:
                case 0xff37:
                case 0xff38:
                case 0xff39:
                case 0xff3A:
                case 0xff3B:
                case 0xff3C:
                case 0xff3D:
                case 0xff3E:
                case 0xff3F:
                    soundChip.channel3.setSamplePair(address - 0xff30, v);
                    break;


                #endregion
            }
        }






    }
}
