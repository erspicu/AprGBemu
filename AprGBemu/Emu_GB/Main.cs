using System.IO;
using System.Threading;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Diagnostics;
using NativeWIN32API;

using hqx_speed;
using XBRz_speed;


namespace AprEmu.GB
{
    public partial class Apr_GB
    {


        public IntPtr handle_sound;


        public int filter_use = 0;

        public bool sound_use = false;

        int flagZ = FlagClear, flagN = FlagClear, flagH = FlagClear, flagC = FlagClear;

        byte r_A = 0, r_B = 0, r_C = 0, r_D = 0, r_E = 0, r_H = 0, r_L = 0; //8bits Register
        ushort r_SP = 0, r_PC = 0; // Stack & Program Counter

        //memory
        byte[] GB_RomPack;
        public byte[] GB_MEM = new byte[65536];

        //for cgb
        byte[] GB_MEM_WRAM_CGB_BANK = new byte[4 * 1024 * 7];
        int CGB_WRAM_index = 1;

        //system flag
        bool flagIME = true;
        bool flagHalt = false;
        //bool flagStop = false;
        bool disable_boot = false;

        public bool start_run = false;

        //for timing
        int cycles = 0;
        int GB_LCD_trick = 0;
        int GB_Timer_trick = 0;
        int GB_DIV_trick = 0;
        int timer_overfolow = 1024;
        Stopwatch StopWatch = new Stopwatch();

        //for mbc
        byte Cartridge_type = 0;

        ushort rom_bank_select = 1;
        byte ram_bank_select = 0;

        byte mbc_h_bits = 0, mbc_l_bits = 0;

        //for screen output
        Graphics Screen_Panel = null;
        public Point Screen_loc; // = new Point(48, 40);

        public bool run_bootstrap = false;
        public bool Only_for_gbc = false;
        public HardwareType runtype = HardwareType.GameBoy;
        public HardwareType CartridgeSupport = HardwareType.GameBoy;

        public string Info = "";

        IntPtr panel_Hanlde;

        SoundChip soundChip;


        public string rom_path = "";


        string save_rom_path = "";

        public void GB_Init_LoadRom(byte[] rom_bytes)
        {
            Info = "";

            Console.WriteLine("init");

            StopWatch.Restart();

            HS_HQ.initTable();
            HS_XBRz.initTable();

            soundChip = new SoundChip(handle_sound);


            GB_RomPack = rom_bytes;
            Cartridge_type = GB_RomPack[0x147];


            switch (Cartridge_type)
            {
                case 3: //ROM_MBC1_RAM_BATT 
                    {
                        save_rom_path = rom_path + ".sav";
                        if (!File.Exists(save_rom_path))
                        {
                            switch (GB_RomPack[0x149])
                            {
                                case 0:
                                    break;

                                case 1:
                                    File.WriteAllBytes(save_rom_path, new byte[2 * 1024]); //2k
                                    break;

                                case 2:
                                    File.WriteAllBytes(save_rom_path, new byte[8 * 1024]); //8k
                                    break;

                                case 3:
                                    File.WriteAllBytes(save_rom_path, new byte[32 * 1024]); //32k
                                    break;

                                case 4: File.WriteAllBytes(save_rom_path, new byte[128 * 1024]); //128k
                                    break;

                            }
                        }
                        else //load sav to memory
                        {
                            byte[] sav = File.ReadAllBytes(save_rom_path);

                            for (int i = 0; i < sav.Length; i++)
                                GB_SwitchableRAM[i] = sav[i];

                        }


                    }
                    break;
            }



            if (GB_RomPack[0x146] == 3)
                CartridgeSupport = HardwareType.SuperGameBoy;

            if (GB_RomPack[0x143] == 0x80 || GB_RomPack[0x143] == 0xC0)
            {
                CartridgeSupport = HardwareType.GameBoyColor;
                if (GB_RomPack[0x143] == 0x80)
                    Only_for_gbc = false;
                else
                    Only_for_gbc = true;
            }

            CartridgeSupport = HardwareType.GameBoy;

            string title = "" + Convert.ToChar(GB_RomPack[0x134]) +
                Convert.ToChar(GB_RomPack[0x135]) +
                Convert.ToChar(GB_RomPack[0x136]) +
                Convert.ToChar(GB_RomPack[0x137]) +
                Convert.ToChar(GB_RomPack[0x138]) +
                Convert.ToChar(GB_RomPack[0x139]) +
                Convert.ToChar(GB_RomPack[0x13A]) +
                Convert.ToChar(GB_RomPack[0x13B]) +
                Convert.ToChar(GB_RomPack[0x13C]) +
                Convert.ToChar(GB_RomPack[0x13D]) +
                Convert.ToChar(GB_RomPack[0x13E]) +
                Convert.ToChar(GB_RomPack[0x13F]) +
                Convert.ToChar(GB_RomPack[0x140]) +
                Convert.ToChar(GB_RomPack[0x141]) +
               Convert.ToChar(GB_RomPack[0x142]
                );

            title = title.Replace("\0", "");

            Console.WriteLine("Cartridge Title : " + title);
            Info += "Cartridge Title : " + title + "\n";

            Console.WriteLine("Cartridge Type : " + CartridgeSupport.ToString());
            Info += "Cartridge Type : " + CartridgeSupport.ToString() + "\n";

            if (Only_for_gbc == true)
            {
                Console.WriteLine("Only for GameBoyColor Hardware!");
                Info += "Only for GameBoyColor Hardware!\n";
            }

            if (CartridgeSupport == HardwareType.GameBoyColor && Only_for_gbc == false)
            {
                // Compatibility
                Console.WriteLine("Compatibility for GameBoy Hardware!");
                Info += "Compatibility for GameBoy Hardware!\n";
            }

            Console.WriteLine("Cartridge MBC: 0x" + Cartridge_type.ToString("X2") + " " + ((CartridgeType)Cartridge_type).ToString());

            Info += "Cartridge MBC: 0x" + Cartridge_type.ToString("X2") + " " + ((CartridgeType)Cartridge_type).ToString() + "\n";

            switch (GB_RomPack[0x148])
            {
                case 0:
                    Console.WriteLine("ROM Size : 32KB");
                    Info += "Cartridge ROM Size :  32KB\n";
                    break;

                case 1:
                    Console.WriteLine("ROM Size : 64KB");
                    Info += "Cartridge ROM Size :  64KB\n";
                    break;

                case 2:
                    Console.WriteLine("ROM Size : 128KB");
                    Info += "Cartridge ROM Size :  128KB\n";
                    break;

                case 3:
                    Console.WriteLine("ROM Size : 256KB");
                    Info += "Cartridge ROM Size :  256KB\n";
                    break;

                case 4:
                    Console.WriteLine("ROM Size : 512KB");
                    Info += "Cartridge ROM Size :  512KB\n";
                    break;

                case 5:
                    Console.WriteLine("ROM Size : 1MByte");
                    Info += "Cartridge ROM Size : 1Mbyte\n";
                    break;

                case 6:
                    Console.WriteLine("ROM Size : 2MByte");
                    Info += "Cartridge ROM Size : 2Mbyte\n";
                    break;

                case 0x52:
                    Console.WriteLine("ROM Size : 1.1MByte");
                    Info += "Cartridge ROM Size : 1.1Mbyte\n";
                    break;

                case 0x53:
                    Console.WriteLine("ROM Size : 1.2MByte");
                    Info += "Cartridge ROM Size : 1.2Mbyte\n";
                    break;

                case 0x54:
                    Console.WriteLine("ROM Size : 1.5MByte");
                    Info += "Cartridge ROM Size : 1.5Mbyte\n";
                    break;
            }
            switch (GB_RomPack[0x149])
            {
                case 0:
                    Console.WriteLine("RAM Size : None");
                    Info += "Cartridge RAM Size : None\n";
                    break;
                case 1:
                    Console.WriteLine("RAM Size : 2KB");
                    Info += "Cartridge RAM Size :  2KB\n";
                    break;
                case 2:
                    Console.WriteLine("RAM Size : 8KB");
                    Info += "Cartridge RAM Size :  8KB\n";
                    break;
                case 3:
                    Console.WriteLine("RAM Size : 32KB");
                    Info += "Cartridge RAM Size :  32KB\n";
                    break;
                case 4:
                    Console.WriteLine("RAM Size : 128KB");
                    Info += "Cartridge RAM Size :  32KB\n";
                    break;
            }


            disable_boot = false;
            flagHalt = false;

            flagZ = flagN = flagH = flagC = FlagClear;
            r_A = r_B = r_C = r_D = r_E = r_H = r_L = 0;
            r_SP = r_PC = 0;

            gbPin14 = gbPin15 = 0xff;

            for (int i = 0; i < 65536; i++) GB_MEM[i] = 0;

            //init buffer
            for (int i = 0; i < 256; i++)
            {
                Buffer_Background0_t0_array[i] = new byte[256];
                Buffer_Background1_t0_array[i] = new byte[256];
                Buffer_Background0_t1_array[i] = new byte[256];
                Buffer_Background1_t1_array[i] = new byte[256];
                Buffer_Background0_t0_array_v1[i] = new byte[256];
                Buffer_Background1_t0_array_v1[i] = new byte[256];
                Buffer_Background0_t1_array_v1[i] = new byte[256];
                Buffer_Background1_t1_array_v1[i] = new byte[256];
            }


            //init title decode table
            title_decode_table();


            //init for CGB Color Table
            for (int i = 0; i <= 0xffff; i++)
            {
                byte r = (byte)(i & 0x1f);
                byte g = (byte)((i >> 5) & 0x1F);
                byte b = (byte)((i >> 10) & 0x1F);
                CGB_COLOR_TABLE[i] = (uint)((0xff << 24) | ((r * 13 + g * 2 + b) >> 1) << 16 | (g * 3 + b) << 9 | (r * 3 + g * 2 + b * 11) >> 1);
            }
            for (int i = 0; i < 640 * 576; i++)
            {
                speed_buffer_4x[i] = 0xff;
            }

            for (int i = 0; i < 384; i++)
                title_update_mark[i] = title_update_mark_cgb[i] = 0;

            for (int i = 0; i < 160; i++)
                Buffer_Screen_array[i] = new uint[144];

            for (int i = 0; i < 320; i++)
                Buffer_Screen_array2xbuffer[i] = new uint[288];


            for (int i = 0; i < 640; i++)
                Buffer_Screen_array4xbuffer[i] = new uint[576];

            //  for (int i = 0; i < 16; i++)
            //      GB_SwitchableRAM[i] = new byte[0x2000];


            string bootfile_GBC = Application.StartupPath + @"\bootstrap\gbc_bios.bin";
            string bootfile_DMG = Application.StartupPath + @"\bootstrap\DMG_ROM.bin";

            bool boot_exit = false;

            if (File.Exists(bootfile_GBC) && run_bootstrap == true && runtype == HardwareType.GameBoyColor)
            {
                try
                {
                    BootStrapMen = File.ReadAllBytes(bootfile_GBC);
                    bootsize = BootStrapMen.Length;
                    Console.WriteLine("Load gbc_bios.bin OK!");
                    Info += "Load gbc_bios.bin OK!\n";
                    boot_exit = true;
                }
                catch
                {
                }
            }

            if (File.Exists(bootfile_DMG) & run_bootstrap == true)// && runtype == HardwareType.GameBoy)
            {

                try
                {
                    BootStrapMen = File.ReadAllBytes(bootfile_DMG);
                    bootsize = BootStrapMen.Length;
                    Console.WriteLine("Load DMG_ROM.bin OK!");
                    Info += "Load DMG_ROM.bin OK!\n";
                    boot_exit = true;
                }
                catch
                {
                }
            }

            if (GB_RomPack[0x143] == 0x0 && runtype == HardwareType.GameBoyColor && run_bootstrap == false)
            {
                runtype = HardwareType.GameBoy;
            }

            if (boot_exit == false)
            {
                GB_init();
                disable_boot = true;
            }
            start_run = true;
            StopWatch.Stop();

            //x = Graphics.FromImage(screen4x);

            Console.WriteLine("init finish - " + StopWatch.ElapsedMilliseconds + "ms");

        }
        public void bind_Screen(ref Graphics dc, IntPtr pHandle, int x, int y, int width, int height)
        {
            panel_Hanlde = pHandle;

            Screen_loc = new Point(x, y);
            Screen_Panel = dc;

            switch (width)
            {
                case 960: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_6x, 0, 0); break;
                case 800: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_5x, 0, 0); break;
                case 640: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_4x, 0, 0); break;
                case 480: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_3x, 0, 0); break;
                case 320: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_2x, 0, 0); break;
                case 256: NativeGDI.initHighSpeed(Screen_Panel, width, height, speed_buffer_1x, 48, 40); break;
            }
        }
        public bool device_maybe_locking = false;
        public void WaitUnlock()
        {
            device_maybe_locking = true;
            start_run = false;
            Thread.Sleep(4);
            while (device_maybe_locking != false)
                Thread.Sleep(0);
        }

        public void SaveSRAM()
        {

            switch (Cartridge_type)
            {
                case 3: //ROM_MBC1_RAM_BATT 
                    {
                        save_rom_path = rom_path + ".sav";
                        if (!File.Exists(save_rom_path))
                            return;

                        switch (GB_RomPack[0x149])
                        {
                            case 0:
                                break;

                            case 1:
                                {
                                    byte[] sav = new byte[2 * 1024];
                                    for (int i = 0; i < 2 * 1024; i++)
                                        sav[i] = GB_SwitchableRAM[i];
                                    File.WriteAllBytes(save_rom_path, sav);
                                }
                                break;

                            case 2:
                                {
                                    byte[] sav = new byte[8 * 1024];
                                    for (int i = 0; i < 8 * 1024; i++)
                                        sav[i] = GB_SwitchableRAM[i];
                                    File.WriteAllBytes(save_rom_path, sav);
                                }
                                break;

                            case 3:
                                {
                                    byte[] sav = new byte[32 * 1024];
                                    for (int i = 0; i < 32 * 1024; i++)
                                        sav[i] = GB_SwitchableRAM[i];
                                    File.WriteAllBytes(save_rom_path, sav);
                                }
                                break;

                            case 4:
                                {
                                    byte[] sav = new byte[128 * 1024];
                                    for (int i = 0; i < 128 * 1024; i++)
                                        sav[i] = GB_SwitchableRAM[i];
                                    File.WriteAllBytes(save_rom_path, sav);
                                }
                                break;

                        }
                    }
                    break;
            }






            save_rom_path = "";
        }




        int s = 0;
        int os = 0;

        int halt_cycle = 0;

        public void GB_run()
        {
            if (Screen_Panel == null)
            {
                MessageBox.Show("need bind Graphics");
                return;
            }
            if (start_run == false)
            {
                MessageBox.Show("not initialized");
                return;
            }
            StopWatch.Restart();


            while (true & !device_maybe_locking)
            {
                //cpu 處理opcode
                if (!flagHalt) //&& !flagStop)
                    GB_CPU_exec();
                else
                {
                    cycles = 4;
                    halt_cycle -= 4;
                    if (halt_cycle <= 0)
                    {
                        flagHalt = false;
                        halt_cycle = 0;
                    }

                    if (!flagHalt)
                        GB_CPU_exec();
                }

                GB_Interrupt();

                GB_Timing(); //時序處理

            }
            device_maybe_locking = false;
        }


        public void GB_init()
        {
            switch (runtype)
            {

                case HardwareType.GameBoy:
                    {
                        r_PC = 0x100;
                        r_SP = 0xFFFF;
                        r_H = 0x01; r_L = 0x4D;
                        r_D = 0x00; r_E = 0xD8;
                        r_B = 0x00; r_C = 0x13;
                        r_A = 0x01;


                        flagZ = FlagSet;
                        flagN = FlagClear;
                        flagH = FlagSet;
                        flagC = FlagSet;

                        GB_MEM[reg_P1_addr] = 0xFF;
                        GB_MEM[reg_DIV_addr] = 0xAF;
                        GB_MEM[reg_TIMA_addr] = 0x00;
                        GB_MEM[reg_TAC_addr] = 0xF8;
                        GB_MEM[reg_IF_addr] = 0x00;

                        /*
                         * NR 相關 reg 尚未實作功能,暫時不需初始化 
                         */

                        GB_MEM[reg_LCDC_addr] = 0x91;
                        GB_MEM[reg_SCY_addr] = 0x00;
                        GB_MEM[reg_SCX_addr] = 0x00;
                        GB_MEM[reg_LY_addr] = 0x00;
                        GB_MEM[reg_LYC_addr] = 0x00;
                        GB_MEM[reg_WY_addr] = 0x00;
                        GB_MEM[reg_WX_addr] = 0x00;
                        GB_MEM[reg_IE_addr] = 0x00;

                        //GB palette
                        MEM_w8(reg_BGP_addr, 0xFC); // 調色盤會特殊處理,調用 GB_MEM_w8 method寫入
                        MEM_w8(reg_OBP0_addr, 0xFF);
                        MEM_w8(reg_OBP1_addr, 0xFF);

                    }
                    break;


                case HardwareType.GameBoyColor:
                    {
                        r_PC = 0x100;
                        r_SP = 0xFFFF;
                        r_H = 0x01; r_L = 0x4D;
                        r_D = 0x00; r_E = 0xD8;
                        r_B = 0x00; r_C = 0x13;
                        r_A = 0x01;

                        flagZ = FlagSet;
                        flagN = FlagClear;
                        flagH = FlagSet;
                        flagC = FlagSet;




                        GB_MEM[reg_P1_addr] = 0xFF;
                        GB_MEM[reg_DIV_addr] = 0xAF;
                        GB_MEM[reg_TIMA_addr] = 0x00;
                        GB_MEM[reg_TAC_addr] = 0xF8;
                        GB_MEM[reg_IF_addr] = 0x00;

                        /*
                         * NR 相關 reg 尚未實作功能,暫時不需初始化 
                         */

                        GB_MEM[reg_LCDC_addr] = 0x91;
                        GB_MEM[reg_SCY_addr] = 0x00;
                        GB_MEM[reg_SCX_addr] = 0x00;
                        GB_MEM[reg_LY_addr] = 0x00;
                        GB_MEM[reg_LYC_addr] = 0x00;
                        GB_MEM[reg_WY_addr] = 0x00;
                        GB_MEM[reg_WX_addr] = 0x00;
                        GB_MEM[reg_IE_addr] = 0x00;
                    }
                    break;


            }


        }

        //timing 參考 gearboy 專案觀念修正

        public void CompareLYToLYC()
        {



            Console.WriteLine(GB_MEM[reg_LY_addr]);
            //  Console.ReadKey();


            if (GB_MEM[reg_LY_addr] == GB_MEM[reg_LYC_addr])
            {
                //  GB_MEM[reg_STAT_addr] |= 0x04;
                //  if ((GB_MEM[reg_STAT_addr] & 0x40) != 0)
                {
                    GB_MEM[reg_IF_addr] |= 2;
                }
            }
            // else
            //   GB_MEM[reg_STAT_addr] &= 0xFB;
        }


        int m_iStatusModeCounterAux = 0;
        int m_iStatusVBlankLine = 0;

        private void GB_Timing()
        {
            #region div
            GB_DIV_trick += cycles;
            if (GB_DIV_trick >= 256)
            {
                GB_DIV_trick -= 256;
                GB_MEM[reg_DIV_addr] = (byte)((++GB_MEM[reg_DIV_addr]) & 0xff);
            }
            #endregion

            #region  LCD GPU model detect
            if ((GB_MEM[0xFF40] & 0x80) > 0)
            {
                GB_LCD_trick += cycles;



                /*int stat = GB_MEM[reg_STAT_addr] & 3;

                if (stat == 0)
                {
                    //HB

                    GB_GPU_Model_0();

                    if (GB_LCD_trick >= 204)
                    {
                        GB_LCD_trick -= 204;
                        GB_GPU_Model_2();
                        GB_MEM[reg_LY_addr]++;


                       // Console.Write("from hb 1 ");
                        CompareLYToLYC();

                        if (GB_MEM[reg_LY_addr] == 144)
                        {

                            m_iStatusVBlankLine = 0;
                            m_iStatusModeCounterAux = GB_LCD_trick;

                            GB_MEM[reg_STAT_addr] = (byte)((GB_MEM[reg_STAT_addr] & 0xfc) | 1);

                            //--render start
                            GB_GPU_Model_1();
                            frame_fps_count++;
                            if (LimitFPS)
                                while (StopWatch.Elapsed.TotalSeconds < 0.0167)
                                    Thread.Sleep(0);
                            StopWatch.Restart();
                            //--rendering end

                        }
                        else
                        {
                        }

                    }
                }
                else if (stat == 1)
                {
                    //VB

                    GB_GPU_Model_1();

                    m_iStatusModeCounterAux += cycles ; 

                    if (m_iStatusModeCounterAux >= 456)
                    {
                        m_iStatusModeCounterAux -= 456;
                        m_iStatusVBlankLine++;

                        if (m_iStatusVBlankLine <= 9)
                        {
                            GB_MEM[reg_LY_addr]++;

                           // Console.Write("from vb 1 ");
                            CompareLYToLYC();
                        }
                    }

                    if ((GB_LCD_trick >= 4104) && (m_iStatusModeCounterAux >= 4) && (GB_MEM[reg_LY_addr] == 153))
                        GB_MEM[reg_LY_addr] = 0;

                    if (GB_LCD_trick >= 4560)
                    {
                        GB_LCD_trick -= 4560;
                        GB_GPU_Model_2();
                        //Console.Write("from vb 2 ");
                        CompareLYToLYC();
                    }
                }
                else if (stat == 2)
                {
                    //OAM lock

                    GB_GPU_Model_2();
                    if (GB_LCD_trick >= 80)
                    {
                        GB_LCD_trick -= 80;
                        GB_GPU_Model_3();
                    }

                }
                else if (stat == 3)
                {
                    //OAM & VRAM lock

                    GB_GPU_Model_3();
                    if (GB_LCD_trick >= 172)
                    {
                        GB_LCD_trick -= 172;
                        GB_GPU_Model_0();
                    }
                }*/




                #region old code

                if (GB_LCD_trick >= 456)//完成一條線與H-blank
                {
                    if ((++GB_MEM[reg_LY_addr]) == 154) GB_MEM[reg_LY_addr] -= 154;


                    #region LY 跟 LYC 比較觸發中斷處理

                    if (GB_MEM[reg_LY_addr] == GB_MEM[reg_LYC_addr])
                    {
                        GB_MEM[reg_STAT_addr] |= 0x04;
                        if ((GB_MEM[reg_STAT_addr] & 0x40) != 0)
                        {
                            GB_MEM[reg_IF_addr] |= 2;
                        }
                    }
                    else
                        GB_MEM[reg_STAT_addr] &= 0xFB;
                    #endregion

                    GB_LCD_trick -= 456; //完成一條scanline周期,重置累加


                    if (GB_MEM[reg_LY_addr] == 144)
                    {

                        if (sound_use)
                            soundChip.outputSound();

                        GB_GPU_Model_1();
                        frame_fps_count++;

                        //  GameBoy 每秒 刷59.73張畫面,一張畫面約停留 0.01674 秒,
                        //  延長一張畫面暫留時間到這個更新速度!但不確定是否是好的處理方法.
                        //  會有些許的小誤差,但幾乎沒影響.

                        if (LimitFPS)
                            while (StopWatch.Elapsed.TotalSeconds < 0.01674) //0.0167
                                Thread.Sleep(0);



                        StopWatch.Restart();

                        // sound_mix();//?

                        //build 1/60 sound
                        //outsound();


                    }




                }
                if (GB_MEM[reg_LY_addr] < 144)
                {
                    if (GB_LCD_trick <= 204)
                        GB_GPU_Model_0();
                    else if (GB_LCD_trick <= 284)
                        GB_GPU_Model_2();
                    else
                        GB_GPU_Model_3();
                }
                #endregion


            }
            else
            {
                //?
            }


            #endregion

            #region timer
            if ((GB_MEM[reg_TAC_addr] & 4) > 0) //timer start
            {
                GB_Timer_trick += cycles;

                while (GB_Timer_trick >= timer_overfolow)
                {
                    GB_Timer_trick -= timer_overfolow;

                    ushort tima = GB_MEM[reg_TIMA_addr];
                    tima++;

                    if (tima == 0xff)
                    {
                        GB_MEM[reg_TIMA_addr] = GB_MEM[reg_TMA_addr];
                        GB_MEM[reg_IF_addr] = (byte)(GB_MEM[reg_IF_addr] | 4);
                    }
                    else
                        GB_MEM[reg_TIMA_addr] = (byte)tima;

                }
            }
            #endregion

            cycles = 0;
        }
    }
}
