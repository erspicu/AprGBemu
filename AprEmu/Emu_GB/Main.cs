using System.IO;
using System.Threading;
using HresTimer;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
//using System.Diagnostics; for Stopwatch

namespace AprEmu.GB
{
    public partial class Apr_GB
    {


        Flag_Status flagZ = Flag_Status.clear, flagN = Flag_Status.clear, flagH = Flag_Status.clear, flagC = Flag_Status.clear;
        byte r_A = 0, r_B = 0, r_C = 0, r_D = 0, r_E = 0, r_H = 0, r_L = 0; //8bits Register
        ushort r_SP = 0, r_PC = 0; // Stack & Program Counter

        //memory
        byte[] GB_RomPack;
        byte[] GB_MEM = new byte[65536];

        //system flag
        bool flagIME = true;
        bool flagHalt = false;
        bool flagStop = false;
        bool disable_boot = false;

        public bool start_run = false;

        //for timing
        byte Cpu_cycles = 0;
        ushort GB_LCD_trick = 0;
        ushort GB_Timer_trick = 0;
        ushort GB_DIV_trick = 0;

        ushort timer_overfolow = 1024;
        HiPerfTimer StopWatch = new HiPerfTimer();
        //Stopwatch StopWatch = new Stopwatch(); 
        //C#原生Stopwatch不知道是我寫法有誤,不熟悉使用,還是精確度有限,沒辦法像第三方HiPerfTimer那麼好用

        //for mbc
        byte Cartridge_type = 0;
        byte rom_bank_select = 1;
        byte ram_bank_select = 1;
        byte mbc1_h_bits = 0, mbc1_l_bits = 0;

        //for screen output
        Graphics Screen_Panel = null;
        Point Screen_loc; // = new Point(48, 40);

        public void GB_Init_LoadRom(string rom_file)
        {
            Console.WriteLine("init");

            GB_RomPack = File.ReadAllBytes(rom_file);
            Cartridge_type = GB_RomPack[0x147];

            Console.WriteLine("MBC:" + Cartridge_type);

            disable_boot = false;
            flagHalt = false;
            flagStop = false;

            flagZ = flagN = flagH = flagC = Flag_Status.clear;
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
            }

            for (int i = 0; i < 160; i++)
                Buffer_Screen_array[i] = new byte[144];

            for (int i = 0; i < 4; i++)
                GB_SwitchableRAM[i] = new byte[0x2000];

            string bootfile = Application.StartupPath + @"\DMG_ROM.bin";

            bool boot_exit = false;

            if (File.Exists(bootfile))
            {
                try
                {

                    MD5 md5 = MD5.Create();
                    FileStream stream = File.OpenRead(bootfile);
                    string md5check = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    stream.Close();
                    if (md5check == "32fbbd84168d3482956eb3c5051637f5") //check file
                    {
                        BootStrap_DMG = File.ReadAllBytes(Application.StartupPath + @"\DMG_ROM.bin");
                        Console.WriteLine("Load DMG_ROM.bin OK!");
                        boot_exit = true;
                    }
                }
                catch
                {
                }
            }
            if (boot_exit == false)
            {
                GB_init();
                disable_boot = true;
            }
            start_run = true;
            Console.WriteLine("init finish");
        }

        public void bind_Screen(ref Graphics dc, int x, int y)
        {
            Screen_loc = new Point(x, y);
            Screen_Panel = dc;
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
                if (!flagHalt && !flagStop)
                    GB_CPU_exec();
                else
                    Cpu_cycles = 4;

                if (flagIME) //中斷條件檢查
                    GB_Interrupt();

                GB_Timing(); //時序處理
            }
            device_maybe_locking = false;
        }

        public void GB_init()
        {

            r_PC = 0x100;
            r_SP = 0xFFFF;
            r_H = 0x01; r_L = 0x4D;
            r_D = 0x00; r_E = 0xD8;
            r_B = 0x00; r_C = 0x13;
            r_A = 0x01;

            flagZ = Flag_Status.set;
            flagN = Flag_Status.clear;
            flagH = Flag_Status.set;
            flagC = Flag_Status.set;

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
            GB_MEM_w8(reg_BGP_addr, 0xFC); // 調色盤會特殊處理,調用 GB_MEM_w8 method寫入
            GB_MEM_w8(reg_OBP0_addr, 0xFF);
            GB_MEM_w8(reg_OBP1_addr, 0xFF);
            GB_MEM[reg_WY_addr] = 0x00;
            GB_MEM[reg_WX_addr] = 0x00;
            GB_MEM[reg_IE_addr] = 0x00;
        }

        private void GB_Timing()
        {
            #region div
            GB_DIV_trick += Cpu_cycles;

            if (GB_DIV_trick >= 256)
            {
                GB_DIV_trick -= 256;

                ushort t1 = GB_MEM[reg_DIV_addr];
                t1++;
                GB_MEM[reg_DIV_addr] = (byte)(t1 & 0xff);
            }

            #endregion
            #region  LCD GPU model detect
            if ((GB_MEM[0xFF40] & 0x80) > 0)
            {

                GB_LCD_trick += Cpu_cycles;
                if (GB_LCD_trick >= 456)//完成一條線與H-blank
                {

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

                    if ((++GB_MEM[reg_LY_addr]) >= 154) GB_MEM[reg_LY_addr] -= 154;

                    GB_MEM_w8(reg_LY_addr, GB_MEM[reg_LY_addr]);
                    if (GB_MEM[reg_LY_addr] == 144)
                    {

                        GB_GPU_Model_1();
                        frame_fps_count++;

                        /*
                         * GameBoy 每秒 刷59.73張畫面,一張畫面約停留 0.01674 秒,
                         * 延長一張畫面暫留時間到這個更新速度!但不確定是否是好的處理方法.
                         * 會有些許的小誤差,但幾乎沒影響.
                         */

                        while (StopWatch.Duration < 0.0167)
                            Thread.Sleep(0);

                        StopWatch.Restart();

                    }
                    //else if (GBM[reg_LY_addr] == 0) {}
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
            }
            #endregion
            #region timer

            if ((GB_MEM[reg_TAC_addr] & 4) > 0) //timer start
            {
                GB_Timer_trick += Cpu_cycles;

                if (GB_Timer_trick >= timer_overfolow)
                {
                    GB_Timer_trick -= timer_overfolow;

                    ushort t1 = GB_MEM[reg_TIMA_addr];
                    t1++;

                    if (t1 <= 255)
                        GB_MEM[reg_TIMA_addr]++;

                    if (t1 > 255)
                    {
                        GB_MEM[reg_TIMA_addr] = GB_MEM[reg_TMA_addr];
                        GB_MEM[reg_IF_addr] = (byte)(GB_MEM[reg_IF_addr] | 4);
                    }
                }
            }
            #endregion
        }
    }
}
