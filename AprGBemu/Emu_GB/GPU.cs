using System.Drawing;
using System.Windows.Forms;
//using hqx; 須要克服效能問題
using System;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ScalexFilter;
using NativeWIN32API;

namespace AprEmu.GB
{
    public partial class Apr_GB
    {
        uint[] GB_Color_Palette_DarkWhite = new uint[] { 0xffffffff, 0xffc0c0c0, 0xff606060, 0xff000000 };
        uint[] GB_Color_Palette_ClassicGreen = new uint[] { 0xffe0f8d0, 0xff88c070, 0xff346856, 0xff081820 };//palette參考bgb

        uint[] GB_Color_Palette_CGB_BG = new uint[] { 0, 0, 0, 0 };
        uint[] GB_Color_Palette_CGB_SP = new uint[] { 0, 0, 0, 0 };



        uint[] GB_Color_Palette_use = null;

        uint[] CGB_COLOR_TABLE = new uint[65536];

        //CGB Palette
        byte[] CGB_BG_Color_Palette = new byte[64];
        int CGB_BG_Color_Palette_use_index = 0;
        int CGB_BG_Palette_internal_add = 0;
        bool CGB_BG_PAL_add_auto_inc = false;

        ushort[] CGB_SP_Color_Palette = new ushort[64];
        int CGB_SP_Color_Palette_use_index = 0;
        int CGB_SP_Palette_internal_add = 0;
        bool CGB_SP_PAL_add_auto_inc = false;

        bool VRAMBank1 = false;

        public byte[] VRAM_BANK_1 = new byte[8192];

        // bool cgb_mod = true;

        Bitmap Frame_Screen_Buffer = new Bitmap(160, 144);
        //byte[][][] title_sets = new byte[384][][]; 

        //記錄 title sets 的各 title 是否更新
        byte[] title_update_mark = new byte[384]; // 0:未更新 1:已更新
        byte[] title_update_mark_cgb = new byte[384]; // 0:未更新 1:已更新

        byte[] title_sets = new byte[384 * 8 * 8];
        byte[] title_sets_v1 = new byte[384 * 8 * 8];


        uint[] Palette_obj_use = null;
        uint[] Palette_obj_0 = new uint[4];
        uint[] Palette_obj_1 = new uint[4];
        uint[] Palette_bgp = new uint[4];


        byte[][] Buffer_Background0_t0_array = new byte[256][];
        byte[][] Buffer_Background0_t1_array = new byte[256][];
        byte[][] Buffer_Background1_t0_array = new byte[256][];
        byte[][] Buffer_Background1_t1_array = new byte[256][];

        byte[][] Buffer_Background0_t0_array_v1 = new byte[256][];
        byte[][] Buffer_Background0_t1_array_v1 = new byte[256][];
        byte[][] Buffer_Background1_t0_array_v1 = new byte[256][];
        byte[][] Buffer_Background1_t1_array_v1 = new byte[256][];


        byte[][] Buffer_Background_array_use;

        uint[][] Buffer_Screen_array = new uint[160][];
        uint[][] Buffer_Screen_array2xbuffer = new uint[320][];
        uint[][] Buffer_Screen_array4xbuffer = new uint[640][];

        uint[] speed_buffer_6x = new uint[960 * 864];
        uint[] speed_buffer_4x = new uint[640 * 576];
        uint[] speed_buffer_3x = new uint[480 * 432];
        uint[] speed_buffer_2x = new uint[320 * 288];
        uint[] speed_buffer_1x = new uint[160 * 144];

        bool map0_update = true;
        bool map1_update = true;

        bool map0_update_cgb = true;
        bool map1_update_cgb = true;

        bool title_update = true;
        bool title_update_V1 = true;


        Rectangle NewRectangle_fitSize = new Rectangle(0, 0, 256, 224);
        // public bool FullScreen = false;
        public bool LimitFPS = true;
        public ScreenPalette ScreenColor = ScreenPalette.DarkWhite;

        // byte[][] title_dec_table = new byte[65536][];
        public void title_decode_table()
        {

            /*
            for (int i = 0; i < 65536; i++)
                title_dec_table[i] = new byte[8];


            for (ushort h = 0; h <= 255; h++)
                for (ushort l = 0; l <= 255; l++)
                    for (byte k = 0; k < 8; k++)
                    {
                        byte mask = (byte)(1 << k);
                        byte pixel = (byte)((((l & mask) << 1) + (h & mask)) >> k);
                        title_dec_table[(ushort)((h << 8) | l)][k] = pixel;

                    }*/
        }

        private void GB_GPU_Model_0() // H-blank
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 0)
            {
                GB_MEM[reg_STAT_addr] &= 0xFC; // 設定狀態
                if ((GB_MEM[reg_STAT_addr] & 8) != 0)
                    GB_MEM[reg_IF_addr] |= 2; // 設定中斷發生flag
            }
            //onsole.WriteLine("g0");
        }



        private void GB_GPU_Model_1() //V-blank
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 1)
            {
                GB_MEM[reg_STAT_addr] = (byte)((GB_MEM[reg_STAT_addr] & 0xFC) | 1);
                if ((GB_MEM[reg_STAT_addr] & 0x10) != 0)
                    GB_MEM[reg_IF_addr] |= 2;
                GB_MEM[reg_IF_addr] |= 1; // fix 11/25
            }

            GB_ScreenUpdate();

            // Console.WriteLine("g1");
            // Console.ReadKey();

        }
        private void GB_GPU_Model_2() // OAM in use
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 2)
            {
                GB_MEM[reg_STAT_addr] = (byte)((GB_MEM[reg_STAT_addr] & 0xFC) | 2);
                if ((GB_MEM[reg_STAT_addr] & 0x20) != 0)
                    GB_MEM[reg_IF_addr] |= 2;
            }

            // Console.WriteLine("g2");
        }
        private void GB_GPU_Model_3() //OAM+VRAM Busy , 沒有中斷功能設定對應的模式
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 3)
                GB_MEM[reg_STAT_addr] |= 3;
            // GB_MEM[reg_STAT_addr] = (byte)((GB_MEM[reg_STAT_addr] & 0xFC) | 3);

            GB_LineToFrameBuffer();
            //Console.WriteLine("g3");
        }
        bool background_update = false;

        // org 版本

        public void CGB_UpdateBackground(ushort bg_map_adder, ref  byte[][] Buffer_Background, int title_set)
        {


            //bg_map_adder = (ushort)( bg_map_adder - 0x8000);

            for (int x = 31; x >= 0; x--)
                for (int y = 31; y >= 0; y--)
                {
                    byte at = GB_MEM[bg_map_adder + ((x << 5) + y)];

                    //CGB_VRAM_BANK[ bg_map_adder  + ((x << 5) + y)];

                    //GB_MEM[bg_map_adder + ((x << 5) + y)];
                    int at_fix = at;
                    for (int i = 7; i >= 0; i--)
                    {
                        if (title_set == 0)
                        {
                            sbyte ats = (sbyte)at;
                            if (ats >= 0 && at <= 127)
                                at_fix = 256 + at;
                        }
                        for (int j = 7; j >= 0; j--)
                            Buffer_Background[(7 - j) + (y << 3)][i + (x << 3)] = title_sets_v1[(at_fix << 6) + (i << 3) + j];
                    }
                }
        }

        public void GB_UpdateBackground(ushort bg_map_adder, byte[][] Buffer_Background, int title_set)
        {
            //for (int x = 31; x >= 0; x--)

            Parallel.For(0, 32, x =>
            {
                for (int y = 31; y >= 0; y--)
                {
                    byte at = GB_MEM[bg_map_adder + ((x << 5) + y)];
                    int at_fix = at;
                    for (int i = 7; i >= 0; i--)
                    {
                        if (title_set == 0)
                        {
                            sbyte ats = (sbyte)at;
                            if (ats >= 0 && at <= 127)
                                at_fix = 256 + at;
                        }
                        for (int j = 7; j >= 0; j--)
                            Buffer_Background[(7 - j) + (y << 3)][i + (x << 3)] = title_sets[(at_fix << 6) + (i << 3) + j];
                    }
                }
            });
        }




        public void ConfigureScreenColor(ScreenPalette palette)
        {
            switch (palette)
            {
                case ScreenPalette.DarkWhite:
                    GB_Color_Palette_use = GB_Color_Palette_DarkWhite;
                    break;
                case ScreenPalette.ClassicGreen:
                    GB_Color_Palette_use = GB_Color_Palette_ClassicGreen;
                    break;
            }
            for (int i = 0; i < 4; i++)
            {
                Palette_obj_0[i] = GB_Color_Palette_use[((pal_obj0 & (3 << (i * 2))) >> (i * 2))];
                Palette_obj_1[i] = GB_Color_Palette_use[((pal_obj1 & (3 << (i * 2))) >> (i * 2))];
                Palette_bgp[i] = GB_Color_Palette_use[((pal_bg & (3 << (i * 2))) >> (i * 2))];
            }
        }


        public void CGB_UpdateTitlesV1()
        {
            for (int i = 383; i >= 0; i--)
            {
                if (title_update_mark_cgb[i] == 1)
                    continue;

                for (int j = 7; j >= 0; j--)
                {
                    byte high = VRAM_BANK_1[(i << 4) + (j << 1) + 1];
                    //GB_MEM[0x8000 + (i << 4) + (j << 1) + 1];
                    byte low = VRAM_BANK_1[(i << 4) + (j << 1)];
                    //GB_MEM[0x8000 + (i << 4) + (j << 1)];

                    for (byte k = 0; k < 8; k++)
                    {
                        byte mask = (byte)(1 << k);
                        byte pixel = (byte)((((high & mask) << 1) + (low & mask)) >> k);
                        title_sets_v1[(i << 6) + (j << 3) + k] = pixel;
                    }
                }
                title_update_mark_cgb[i] = 1; //已更新
            }
        }




        public void GB_UpdateTitles()
        {


            // Parallel.For(0, 384, i =>
            // {
            // });




            // for (int i = 383; i >= 0; i--)
            Parallel.For(0, 384, i =>
           {
               if (title_update_mark[i] != 1)
               {
                   //continue;

                   for (int j = 7; j >= 0; j--)
                   //Parallel.For(0, 8, j =>
                   {
                       byte high = GB_MEM[0x8000 + (i << 4) + (j << 1) + 1];
                       byte low = GB_MEM[0x8000 + (i << 4) + (j << 1)];

                       for (byte k = 0; k < 8; k++)
                       {
                           byte mask = (byte)(1 << k);
                           byte pixel = (byte)((((high & mask) << 1) + (low & mask)) >> k);
                           title_sets[(i << 6) + (j << 3) + k] = pixel;
                       }
                   }//);
                   title_update_mark[i] = 1; //已更新
               }
           });
        }


        public void GB_LineToFrameBuffer()
        {
            int ly = GB_MEM[reg_LY_addr];
            #region background screen buffer update


            // title_update_cgb = title_update = background_update = true;

            if (ly == 0 && background_update)
            {
                if (title_update)
                    GB_UpdateTitles();

                if (title_update_V1)
                    CGB_UpdateTitlesV1();


                if (title_update_V1)
                {

                    CGB_UpdateBackground(0x9800, ref Buffer_Background0_t0_array_v1, 0);
                    CGB_UpdateBackground(0x9c00, ref Buffer_Background1_t0_array_v1, 0);
                    CGB_UpdateBackground(0x9800, ref Buffer_Background0_t1_array_v1, 1);
                    CGB_UpdateBackground(0x9c00, ref Buffer_Background1_t1_array_v1, 1);


                    title_update_V1 = false;
                    map0_update_cgb = false;
                    map1_update_cgb = false;
                }
                else
                {
                    if (map0_update_cgb)
                    {
                        CGB_UpdateBackground(0x9800, ref Buffer_Background0_t0_array_v1, 0);
                        CGB_UpdateBackground(0x9800, ref Buffer_Background0_t1_array_v1, 1);
                        map0_update_cgb = false;
                    }
                    if (map1_update_cgb)
                    {
                        CGB_UpdateBackground(0x9c00, ref Buffer_Background1_t0_array_v1, 0);
                        CGB_UpdateBackground(0x9c00, ref Buffer_Background1_t1_array_v1, 1);
                        map1_update_cgb = false;
                    }
                }



                if (title_update)
                {


                    GB_UpdateBackground(0x9800, Buffer_Background0_t0_array, 0);
                    GB_UpdateBackground(0x9c00, Buffer_Background1_t0_array, 0);
                    GB_UpdateBackground(0x9800, Buffer_Background0_t1_array, 1);
                    GB_UpdateBackground(0x9c00, Buffer_Background1_t1_array, 1);


                    title_update = false;
                    map0_update = false;
                    map1_update = false;
                }
                else
                {
                    if (map0_update)
                    {
                        GB_UpdateBackground(0x9800, Buffer_Background0_t0_array, 0);
                        GB_UpdateBackground(0x9800, Buffer_Background0_t1_array, 1);
                        map0_update = false;
                    }
                    if (map1_update)
                    {
                        GB_UpdateBackground(0x9c00, Buffer_Background1_t0_array, 0);
                        GB_UpdateBackground(0x9c00, Buffer_Background1_t1_array, 1);
                        map1_update = false;
                    }
                }
                background_update = false;
            }
            #endregion

            #region 160x144 screen

            // cgb_mod = false;

            //if ( runtype != HardwareType.GameBoyColor)
            //{

            //if ( )
            //{

            if (CartridgeSupport == HardwareType.GameBoy || CartridgeSupport == HardwareType.SuperGameBoy ||
                (runtype == HardwareType.GameBoyColor && disable_boot == true && run_bootstrap == true && CartridgeSupport != HardwareType.GameBoyColor))
            {
                if ((GB_MEM[reg_LCDC_addr] & 8) > 0)
                {
                    Buffer_Background_array_use = Buffer_Background1_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background1_t1_array;
                }
                else
                {
                    Buffer_Background_array_use = Buffer_Background0_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background0_t1_array;
                }

                //Parallel.For(0, 160, x =>
                for (int x = 159; x >= 0; x--)
                {
                    int x_c = x + GB_MEM[0xff43];
                    int y_c = ly + GB_MEM[0xff42];
                    if (x_c > 255) x_c = x_c - 256;
                    if (y_c > 255) y_c = y_c - 256;

                    //if (runtype == HardwareType.GameBoy && (CartridgeSupport == HardwareType.GameBoy || CartridgeSupport == HardwareType.SuperGameBoy))
                    Buffer_Screen_array[x][ly] = Palette_bgp[Buffer_Background_array_use[x_c][y_c]];
                    //else if (runtype == HardwareType.GameBoyColor && (CartridgeSupport == HardwareType.GameBoy || CartridgeSupport == HardwareType.SuperGameBoy))
                    //  Buffer_Screen_array[x][ly] = GB_Color_Palette_CGB_BG[Buffer_Background_array_use[x_c][y_c]];

                }


            }



            if ((CartridgeSupport == HardwareType.GameBoyColor && runtype == HardwareType.GameBoyColor) || (runtype == HardwareType.GameBoyColor && disable_boot == false && run_bootstrap == true))
            {
                bool bg1 = false;
                if ((GB_MEM[reg_LCDC_addr] & 8) > 0)
                    bg1 = true;

                for (int x = 159; x >= 0; x--)
                {
                    int x_c = x + GB_MEM[0xff43];
                    int y_c = ly + GB_MEM[0xff42];
                    if (x_c > 255) x_c = x_c - 256;
                    if (y_c > 255) y_c = y_c - 256;

                    //CGB 多了 屬性資料和更多的調色盤可以使用
                    byte attr = (byte)(VRAM_BANK_1[0x1800 + (x_c / 8) + (y_c / 8) * 32]);

                    bool vram1_title = false;
                    bool h_flip = false;

                    if ((attr & 0x20) != 0)
                        h_flip = true;

                    bool v_flip = false;

                    if ((attr & 0x40) != 0)
                        v_flip = true;


                    if ((attr & 8) != 0) //水平鏡射
                        vram1_title = true;

                    if (bg1)
                    {
                        if (vram1_title == true)
                        {
                            Buffer_Background_array_use = Buffer_Background1_t0_array_v1;
                            if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                Buffer_Background_array_use = Buffer_Background1_t1_array_v1;
                        }
                        else
                        {
                            Buffer_Background_array_use = Buffer_Background1_t0_array;
                            if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                Buffer_Background_array_use = Buffer_Background1_t1_array;
                        }
                    }
                    else
                    {
                        if (vram1_title == true)
                        {
                            Buffer_Background_array_use = Buffer_Background0_t0_array_v1;
                            if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                Buffer_Background_array_use = Buffer_Background0_t1_array_v1;
                        }
                        else
                        {
                            Buffer_Background_array_use = Buffer_Background0_t0_array;
                            if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                Buffer_Background_array_use = Buffer_Background0_t1_array;
                        }

                    }

                    byte color_index_attr = (byte)(attr & 7);

                    int fix_xc = x_c;
                    int fix_yc = y_c;

                    if (v_flip)
                    {
                        fix_yc = (7 - (fix_yc % 8)) + (fix_yc / 8) * 8;
                    }

                    if (h_flip)
                    {
                        fix_xc = (7 - (fix_xc % 8)) + (fix_xc / 8) * 8;
                    }


                    Buffer_Screen_array[x][ly] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2])];



                }
            }
            #endregion

            #region window


            if (CartridgeSupport == HardwareType.GameBoy || CartridgeSupport == HardwareType.SuperGameBoy ||
    (runtype == HardwareType.GameBoyColor && disable_boot == true && run_bootstrap == true && CartridgeSupport != HardwareType.GameBoyColor))
            {
                Buffer_Background_array_use = Buffer_Background0_t0_array;
                if ((GB_MEM[reg_LCDC_addr] & 0x40) > 0)
                {
                    Buffer_Background_array_use = Buffer_Background1_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background1_t1_array;
                }
                else
                {
                    Buffer_Background_array_use = Buffer_Background0_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background0_t1_array;
                }
                byte wy = GB_MEM[reg_WY_addr];
                byte wx = GB_MEM[reg_WX_addr];
                if ((GB_MEM[reg_LCDC_addr] & 0x21) == 0x21 && ly >= wy)
                {
                    int x_start = wx - 7;
                    if (x_start < 0) x_start = 0;
                    int x_end = 153 + wx;
                    if (x_end > 160) x_end = 160;
                    byte fix_y = (byte)(ly - wy);
                    for (int x = x_start; x < x_end; x++)
                    {


                        if (runtype == HardwareType.GameBoy && CartridgeSupport != HardwareType.GameBoyColor)
                            Buffer_Screen_array[x][ly] = Palette_bgp[Buffer_Background_array_use[x - wx + 7][fix_y]];
                        else if (runtype == HardwareType.GameBoyColor && CartridgeSupport != HardwareType.GameBoyColor)
                            Buffer_Screen_array[x][ly] = GB_Color_Palette_CGB_BG[Buffer_Background_array_use[x - wx + 7][fix_y]];
                    }
                }
            }
            else if ((CartridgeSupport == HardwareType.GameBoyColor && runtype == HardwareType.GameBoyColor) || (runtype == HardwareType.GameBoyColor && disable_boot == false && run_bootstrap == true))
            {
                //editing
                /*Buffer_Background_array_use = Buffer_Background0_t0_array;
                if ((GB_MEM[reg_LCDC_addr] & 0x40) > 0)
                {
                    Buffer_Background_array_use = Buffer_Background1_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background1_t1_array;
                }
                else
                {
                    Buffer_Background_array_use = Buffer_Background0_t0_array;
                    if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                        Buffer_Background_array_use = Buffer_Background0_t1_array;
                }*/

                bool bg1 = false;
                if ((GB_MEM[reg_LCDC_addr] & 0x40) > 0)
                    bg1 = true;

                byte wy = GB_MEM[reg_WY_addr];
                byte wx = GB_MEM[reg_WX_addr];
                if ((GB_MEM[reg_LCDC_addr] & 0x21) == 0x21 && ly >= wy) //CGB need fix !!!!
                {
                    int x_start = wx - 7;
                    if (x_start < 0) x_start = 0;
                    int x_end = 153 + wx;
                    if (x_end > 160) x_end = 160;
                    byte fix_y = (byte)(ly - wy);
                    for (int x = x_start; x < x_end; x++)
                    {

                        //CGB 多了 屬性資料和更多的調色盤可以使用
                        byte attr = (byte)(VRAM_BANK_1[0x1800 + ((x - wx + 7) / 8) + (fix_y / 8) * 32]);

                        bool vram1_title = false;
                        bool h_flip = false;

                        if ((attr & 0x20) != 0)
                            h_flip = true;

                        bool v_flip = false;

                        if ((attr & 0x40) != 0)
                            v_flip = true;

                        if ((attr & 8) != 0) //水平鏡射
                            vram1_title = true;


                        if (bg1)
                        {
                            if (vram1_title == true)
                            {
                                Buffer_Background_array_use = Buffer_Background1_t0_array_v1;
                                if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                    Buffer_Background_array_use = Buffer_Background1_t1_array_v1;
                            }
                            else
                            {
                                Buffer_Background_array_use = Buffer_Background1_t0_array;
                                if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                    Buffer_Background_array_use = Buffer_Background1_t1_array;
                            }
                        }
                        else
                        {
                            if (vram1_title == true)
                            {
                                Buffer_Background_array_use = Buffer_Background0_t0_array_v1;
                                if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                    Buffer_Background_array_use = Buffer_Background0_t1_array_v1;
                            }
                            else
                            {
                                Buffer_Background_array_use = Buffer_Background0_t0_array;
                                if ((GB_MEM[reg_LCDC_addr] & 0x10) > 0)
                                    Buffer_Background_array_use = Buffer_Background0_t1_array;
                            }

                        }



                        byte color_index_attr = (byte)(attr & 7);

                        int fix_xc = x - wx + 7;
                        int fix_yc = fix_y;

                        if (v_flip)
                        {
                            fix_yc = (7 - (fix_yc % 8)) + (fix_yc / 8) * 8;
                        }

                        if (h_flip)
                        {
                            fix_xc = (7 - (fix_xc % 8)) + (fix_xc / 8) * 8;
                        }


                        Buffer_Screen_array[x][ly] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2])];




                        /* if (runtype == HardwareType.GameBoy && CartridgeSupport != HardwareType.GameBoyColor)
                             Buffer_Screen_array[x][ly] = Palette_bgp[Buffer_Background_array_use[x - wx + 7][fix_y]];
                         else if (runtype == HardwareType.GameBoyColor && CartridgeSupport != HardwareType.GameBoyColor)
                             Buffer_Screen_array[x][ly] = GB_Color_Palette_CGB_BG[Buffer_Background_array_use[x - wx + 7][fix_y]];*/
                    }
                }
            }
            #endregion
            #region sprite rendering to Screen Frame Buffer
            //OAM 層 PS.其實應該還可以做更高速優化,但即解title似乎也就可以了

            if (CartridgeSupport == HardwareType.GameBoy || CartridgeSupport == HardwareType.SuperGameBoy ||
    (runtype == HardwareType.GameBoyColor && disable_boot == true && run_bootstrap == true && CartridgeSupport != HardwareType.GameBoyColor))
            {
                //if (CartridgeSupport != HardwareType.GameBoyColor)
                if ((GB_MEM[reg_LCDC_addr] & 2) > 0) // check display Sprite
                {
                    for (int i_oam = 39; i_oam >= 0; i_oam--)
                    //Parallel.For(0, 40, i_oam =>
                    {

                        int tmp = i_oam << 2;

                        byte Y_loc = GB_MEM[0xfe00 + tmp];
                        byte X_loc = GB_MEM[0xfe00 + tmp + 1];

                        byte title_number = GB_MEM[0xfe00 + tmp + 2], title_up = 0, title_down = 0;
                        byte options = GB_MEM[0xfe00 + tmp + 3];

                        byte size = 8;

                        bool flip_x = false;
                        if ((options & 0x20) > 0) flip_x = true;
                        bool flip_y = false;
                        if ((options & 0x40) > 0) flip_y = true;

                        if ((GB_MEM[reg_LCDC_addr] & 4) > 0)// 8*16 sprite size
                        {
                            size = 16;
                            title_up = (byte)(title_number & 0xfe);
                            title_down = (byte)(title_number | 1);
                            title_number = title_up;
                        }

                        int fixed_x = X_loc - 8;
                        int fixed_y = Y_loc - 16;

                        //if (!((options & 0x80) == 0 && (ly >= fixed_y) && (ly < fixed_y + size) &&
                        //    fixed_y >= -16 && fixed_x >= -8 && fixed_x < 160 && fixed_y < 144)) continue;

                        if (!((ly >= fixed_y) && (ly < fixed_y + size) &&
                            fixed_y >= -16 && fixed_x >= -8 && fixed_x < 160 && fixed_y < 144))
                            continue;

                        bool AboveBackground = false;
                        if ((options & 0x80) == 0) AboveBackground = true;

                        if ((options & 0X10) == 0)
                            Palette_obj_use = Palette_obj_0;
                        else
                            Palette_obj_use = Palette_obj_1;

                        byte j = (byte)(ly - fixed_y);

                        //sprite使用的title從0x8000開始
                        byte fix_j = j;
                        if (flip_y)
                            fix_j = (byte)(7 - j);

                        //pixel rendering層
                        if ((size == 8) && (ly < fixed_y + 8) || (size == 16 && (ly < fixed_y + 8)))
                        {
                            for (int k = 7; k >= 0; k--)
                            {
                                byte pixel = title_sets[(title_number << 6) + (fix_j << 3) + k];// title_sets[title_number][fix_j][k];     
                                int x_p;
                                if (flip_x) //check x-filp
                                    x_p = k + fixed_x;
                                else
                                    x_p = (7 - k) + fixed_x;
                                if (x_p > 0 && x_p < 160 && pixel != 0)// && (!AboveBackground && Buffer_Background_array_use[x_p][ly] == 0))
                                {

                                    // 

                                    if (AboveBackground)
                                    {
                                        Buffer_Screen_array[x_p][ly] = Palette_obj_use[pixel];
                                    }
                                    else
                                    {
                                        //need fix
                                        Buffer_Screen_array[x_p][ly] = Palette_obj_use[pixel];
                                    }
                                }
                            }
                        }
                        else if (size == 16 && (ly >= fixed_y + 8))
                        {
                            j = (byte)(ly - (fixed_y + 8));
                            fix_j = j;
                            if (flip_y)
                                fix_j = (byte)(7 - j);
                            for (int k = 7; k >= 0; k--)
                            {
                                byte pixel = title_sets[(title_down << 6) + (fix_j << 3) + k]; //title_sets[title_down][fix_j][k];// (byte)((((high & mask) << 1) + (low & mask)) >> k);
                                int x_p;
                                if (flip_x) //check x-filp
                                    x_p = k + fixed_x;
                                else
                                    x_p = (7 - k) + fixed_x; //below 8x8 block
                                if (x_p > 0 && x_p < 160 && pixel != 0)//  && (!AboveBackground && Buffer_Background_array_use[x_p][ly] == 0  ) )
                                {
                                    if (AboveBackground)
                                        Buffer_Screen_array[x_p][ly] = Palette_obj_use[pixel];
                                    else
                                    {
                                        //need fix
                                        Buffer_Screen_array[x_p][ly] = Palette_obj_use[pixel];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if ((CartridgeSupport == HardwareType.GameBoyColor && runtype == HardwareType.GameBoyColor) || (runtype == HardwareType.GameBoyColor && disable_boot == false && run_bootstrap == true))
            {
                if ((GB_MEM[reg_LCDC_addr] & 2) > 0) // check display Sprite
                {
                    for (int i_oam = 39; i_oam >= 0; i_oam--)
                    {

                        int tmp = i_oam << 2;

                        byte Y_loc = GB_MEM[0xfe00 + tmp];
                        byte X_loc = GB_MEM[0xfe00 + tmp + 1];

                        byte title_number = GB_MEM[0xfe00 + tmp + 2], title_up = 0, title_down = 0;
                        byte options = GB_MEM[0xfe00 + tmp + 3];

                        byte size = 8;

                        bool flip_x = false;
                        if ((options & 0x20) > 0) flip_x = true;
                        bool flip_y = false;
                        if ((options & 0x40) > 0) flip_y = true;

                        if ((GB_MEM[reg_LCDC_addr] & 4) > 0)// 8*16 sprite size
                        {
                            size = 16;
                            title_up = (byte)(title_number & 0xfe);
                            title_down = (byte)(title_number | 1);
                            title_number = title_up;
                        }

                        int fixed_x = X_loc - 8;
                        int fixed_y = Y_loc - 16;

                        if (!((ly >= fixed_y) && (ly < fixed_y + size) &&
                            fixed_y >= -16 && fixed_x >= -8 && fixed_x < 160 && fixed_y < 144)) continue;

                        bool AboveBackground = false;
                        if ((options & 0x80) == 0) AboveBackground = true;


                        /*if ((options & 0X10) == 0)
                            Palette_obj_use = Palette_obj_0;
                        else
                            Palette_obj_use = Palette_obj_1;*/

                        byte palette_num = (byte)(options & 7);

                        bool tilev1 = false;
                        if ((options & 8) != 0)
                            tilev1 = true;



                        byte j = (byte)(ly - fixed_y);

                        //sprite使用的title從0x8000開始
                        byte fix_j = j;
                        if (flip_y)
                            fix_j = (byte)(7 - j);

                        //pixel rendering層
                        if ((size == 8) && (ly < fixed_y + 8) || (size == 16 && (ly < fixed_y + 8)))
                        {
                            for (int k = 7; k >= 0; k--)
                            {
                                byte pixel = 0;

                                if (tilev1 == true)
                                    pixel = title_sets_v1[(title_number << 6) + (fix_j << 3) + k];
                                else
                                    pixel = title_sets[(title_number << 6) + (fix_j << 3) + k];
                                //title_sets[(title_number << 6) + (fix_j << 3) + k];

                                // title_sets[(title_number << 6) + (fix_j << 3) + k];// title_sets[title_number][fix_j][k];     

                                int x_p;
                                if (flip_x) //check x-filp
                                    x_p = k + fixed_x;
                                else
                                    x_p = (7 - k) + fixed_x;
                                if (x_p > 0 && x_p < 160 && pixel != 0)// && (!AboveBackground && Buffer_Background_array_use[x_p][ly] == 0))
                                {
                                    if (AboveBackground)
                                    {
                                        Buffer_Screen_array[x_p][ly] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * palette_num + pixel * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * palette_num + pixel * 2])];

                                    }
                                    else
                                    {
                                        //need fix
                                        // Buffer_Screen_array[x_p][ly] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * palette_num + pixel * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * palette_num + pixel * 2])];

                                    }
                                }
                            }
                        }
                        else if (size == 16 && (ly >= fixed_y + 8))
                        {

                            j = (byte)(ly - (fixed_y + 8));
                            fix_j = j;
                            if (flip_y)
                                fix_j = (byte)(7 - j);
                            for (int k = 7; k >= 0; k--)
                            {
                                byte pixel = 0;

                                if (tilev1 == true)
                                    pixel = title_sets_v1[(title_down << 6) + (fix_j << 3) + k];
                                else
                                    pixel = title_sets[(title_down << 6) + (fix_j << 3) + k];




                                int x_p;
                                if (flip_x) //check x-filp
                                    x_p = k + fixed_x;
                                else
                                    x_p = (7 - k) + fixed_x; //below 8x8 block
                                if (x_p > 0 && x_p < 160 && pixel != 0)//  && (!AboveBackground && Buffer_Background_array_use[x_p][ly] == 0  ) )
                                {
                                    if (AboveBackground)
                                    {
                                        //      Buffer_Screen_array[x][ly] = CGB_COLOR_TABLE[(CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2 + 1] << 8 | CGB_BG_Color_Palette[8 * color_index_attr + Buffer_Background_array_use[fix_xc][fix_yc] * 2])];


                                        Buffer_Screen_array[x_p][ly] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * palette_num + pixel * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * palette_num + pixel * 2])];

                                    }
                                    else
                                    {
                                        //need fix
                                        // Buffer_Screen_array[x_p][ly] = CGB_COLOR_TABLE[(CGB_SP_Color_Palette[8 * palette_num + pixel * 2 + 1] << 8 | CGB_SP_Color_Palette[8 * palette_num + pixel * 2])];

                                    }
                                }
                            }

                        }
                    }
                }

            }


            #endregion

        }
        //GPU screen frame update
        public int frame_fps_count = 0;
        public bool screen_lock = false;

        Bitmap screen2x = new Bitmap(320, 288);
        Bitmap screen3x = new Bitmap(480, 432);
        Bitmap screen4x = new Bitmap(640, 576);

        //Graphics x;//= Graphics.FromImage(screen4x);

        public int GB_ScreenSize = 1;
        private void GB_ScreenUpdate()
        {
            screen_lock = true;
            // http://stackoverflow.com/questions/7690546/replace-gdi-drawimage-with-pinvoked-gdi-and-transparent-pngs
            //http://www.cnblogs.com/carekee/articles/2178308.html
            // http://stackoverflow.com/questions/2302550/bitblt-code-not-working
            switch (GB_ScreenSize)
            {
                case 1:
                    {
                        for (int x = 159; x >= 0; x--)
                            for (int y = 143; y >= 0; y--)
                                speed_buffer_1x[y * 160 + x] = Buffer_Screen_array[x][y];
                        NativeGDI.DrawImageHighSpeedtoDevice();
                    }
                    break;
                case 2:
                    ScalexTool.toScale2x_dx(Buffer_Screen_array, 160, 144, speed_buffer_2x);
                    NativeGDI.DrawImageHighSpeedtoDevice();
                    break;
                case 3:
                    ScalexTool.toScale3x_dx(Buffer_Screen_array, 160, 144, speed_buffer_3x);
                    NativeGDI.DrawImageHighSpeedtoDevice();
                    break;
                case 4:
                    ScalexTool.toScale4x_dx(Buffer_Screen_array, 160, 144, speed_buffer_4x);
                    NativeGDI.DrawImageHighSpeedtoDevice();
                    break;
                case 6:
                    ScalexTool.toScale6x_dx(Buffer_Screen_array, 160, 144, speed_buffer_6x);
                    NativeGDI.DrawImageHighSpeedtoDevice();
                    break;
            }

            screen_lock = false;
        }

        public Bitmap GetScreenFrame()
        {
            switch (GB_ScreenSize)
            {
                case 1: return new Bitmap(160, 144, 160 * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(speed_buffer_1x, 0));
                case 2: return new Bitmap(160 * 2, 144 * 4, 160 * 2 * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(speed_buffer_2x, 0));
                case 3: return new Bitmap(160 * 3, 144 * 3, 160 * 3 * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(speed_buffer_3x, 0));
                case 4: return new Bitmap(160 * 4, 144 * 4, 160 * 4 * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(speed_buffer_4x, 0));
            }
            return null;
        }




    }
}