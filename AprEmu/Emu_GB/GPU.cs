using System.Drawing;
using BitmapProcessing;

namespace AprEmu.GB
{
    public partial class Apr_GB
    {

        byte[] GB_Color_Palette = new byte[] { 255, 192, 96, 0 };
        Bitmap Frame_Screen_Buffer = new Bitmap(160, 144);

        int[] Palette_obj_use = null;
        int[] Palette_obj_0 = new int[4];
        int[] Palette_obj_1 = new int[4];
        int[] Palette_bgp = new int[4];

        byte[][] Buffer_Background0_t0_array = new byte[256][];
        byte[][] Buffer_Background0_t1_array = new byte[256][];
        byte[][] Buffer_Background1_t0_array = new byte[256][];
        byte[][] Buffer_Background1_t1_array = new byte[256][];

        byte[][] Buffer_Background_array_use;
        byte[][] Buffer_Screen_array = new byte[160][];

        bool map0_update = true;
        bool map1_update = true;
        bool title_update = true;

        private void GB_GPU_Model_0() // H-blank
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 0)
            {
                GB_MEM[reg_STAT_addr] &= 0xFC; // 設定狀態
                if ((GB_MEM[reg_STAT_addr] & 8) != 0)
                    GB_MEM[reg_IF_addr] |= 2; // 設定中斷發生flag
            }
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
        }

        private void GB_GPU_Model_2() // OAM in use
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 2)
            {
                GB_MEM[reg_STAT_addr] = (byte)((GB_MEM[reg_STAT_addr] & 0xFC) | 2);
                if ((GB_MEM[reg_STAT_addr] & 0x20) != 0)
                    GB_MEM[reg_IF_addr] |= 2;
            }
        }

        private void GB_GPU_Model_3() //OAM+VRAM Busy , 沒有中斷功能設定對應的模式
        {
            if ((GB_MEM[reg_STAT_addr] & 3) != 3)
                GB_MEM[reg_STAT_addr] |= 3;
            GB_LineToFrameBuffer();
        }

        bool background_update = false;

        public void GB_UpdataBackground(ushort bg_map_adder, ref  byte[][] Buffer_Background, int title_set)
        {
            for (byte x = 0; x <= 31; x++)
                for (byte y = 0; y <= 31; y++)
                {
                    byte at = GB_MEM[bg_map_adder + (x * 32 + y)];
                    for (byte i = 0; i < 8; i++)
                    {

                        byte high = 0;
                        byte low = 0;

                        if (title_set == 1)//(GB_MEM[reg_LCDC_addr] & 10) > 0)
                        {

                            high = GB_MEM[0x8000 + at * 16 + i * 2 + 1];
                            low = GB_MEM[0x8000 + at * 16 + i * 2];
                        }
                        else
                        {
                            sbyte ats = (sbyte)at;

                            if (ats >= 0 && at <= 127)
                            {
                                high = GB_MEM[0x9000 + at * 16 + i * 2 + 1];
                                low = GB_MEM[0x9000 + at * 16 + i * 2];
                            }
                            else if (ats >= -128 && ats <= -1)
                            {
                                at = (byte)(ats);
                                high = GB_MEM[0x8000 + at * 16 + i * 2 + 1];
                                low = GB_MEM[0x8000 + at * 16 + i * 2];
                            }
                        }

                        for (byte j = 0; j < 8; j++)
                        {
                            byte mask = (byte)(1 << j);
                            byte pixel = (byte)((((high & mask) << 1) + (low & mask)) >> j);
                            Buffer_Background[(7 - j) + (y * 8)][i + (x * 8)] = pixel; //GB_Color_Palette[Palette_bgp[pixel]];
                        }
                    }
                }
        }

        public void GB_LineToFrameBuffer()
        {
            int ly = GB_MEM[reg_LY_addr];

            #region background screen buffer update
            if (ly == 0 && background_update)
            {
                if (title_update)
                {
                    GB_UpdataBackground(0x9800, ref Buffer_Background0_t0_array, 0);
                    GB_UpdataBackground(0x9c00, ref Buffer_Background1_t0_array, 0);

                    GB_UpdataBackground(0x9800, ref Buffer_Background0_t1_array, 1);
                    GB_UpdataBackground(0x9c00, ref Buffer_Background1_t1_array, 1);

                    title_update = false;
                }
                else
                {
                    if (map0_update)
                    {
                        GB_UpdataBackground(0x9800, ref Buffer_Background0_t0_array, 0);
                        GB_UpdataBackground(0x9800, ref Buffer_Background0_t1_array, 1);
                        map0_update = false;
                    }
                    if (map1_update)
                    {
                        GB_UpdataBackground(0x9c00, ref Buffer_Background1_t0_array, 0);
                        GB_UpdataBackground(0x9c00, ref Buffer_Background1_t1_array, 1);
                        map1_update = false;
                    }
                }
                background_update = false;
            }
            #endregion
            #region 160x144 screen

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

            for (byte x = 0; x < 160; x++)
            {
                int x_c = x + GB_MEM[0xff43];
                int y_c = ly + GB_MEM[0xff42];
                if (x_c > 255) x_c = x_c - 256;
                if (y_c > 255) y_c = y_c - 256;
                Buffer_Screen_array[x][ly] = (byte)Palette_bgp[Buffer_Background_array_use[x_c][y_c]];
            }
            #endregion
            #region window
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
                    Buffer_Screen_array[x][ly] = (byte)Palette_bgp[Buffer_Background_array_use[x - wx + 7][fix_y]];

            }
            #endregion
            #region sprite rendering to Screen Frame Buffer
            //OAM 層 PS.其實應該還可以做更高速優化,但即解title似乎也就可以了
            if ((GB_MEM[reg_LCDC_addr] & 2) > 0) // check display Sprite
            {

                for (byte i = 0; i <= 39; i++)
                {

                    byte tmp = (byte)(i * 4);

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

                    if (!((options & 0x80) == 0 && (ly >= fixed_y) && (ly < fixed_y + size) &&
                        fixed_y >= -16 && fixed_x >= -8 && fixed_x < 160 && fixed_y < 144)) continue;

                    if ((options & 0X10) == 0)
                        Palette_obj_use = Palette_obj_0;
                    else
                        Palette_obj_use = Palette_obj_1;

                    byte j = (byte)(ly - fixed_y);

                    //sprite使用的title從0x8000開始
                    byte fix_j = j;
                    if (flip_y)
                        fix_j = (byte)(7 - j);
                    byte high = GB_MEM[0x8000 + title_number * 16 + fix_j * 2 + 1];
                    byte low = GB_MEM[0x8000 + title_number * 16 + fix_j * 2];

                    //pixel rendering層
                    if ((size == 8) && (ly < fixed_y + 8) || (size == 16 && (ly < fixed_y + 8)))
                    {
                        for (byte k = 0; k < 8; k++)
                        {
                            byte mask = (byte)(1 << k);
                            byte pixel = (byte)((((high & mask) << 1) + (low & mask)) >> k);

                            int x_p;

                            if (flip_x) //check x-filp
                                x_p = k + fixed_x;
                            else
                                x_p = (7 - k) + fixed_x;

                            if (x_p > 0 && x_p < 160 && pixel != 0)
                                Buffer_Screen_array[x_p][ly] = (byte)Palette_obj_use[pixel];
                        }
                    }
                    else if (size == 16 && (ly >= fixed_y + 8))
                    {
                        j = (byte)(ly - (fixed_y + 8));

                        fix_j = j;
                        if (flip_y)
                            fix_j = (byte)(7 - j);

                        high = GB_MEM[0x8000 + title_down * 16 + fix_j * 2 + 1];
                        low = GB_MEM[0x8000 + title_down * 16 + fix_j * 2];

                        for (byte k = 0; k < 8; k++)
                        {
                            byte mask = (byte)(1 << k);
                            byte pixel = (byte)((((high & mask) << 1) + (low & mask)) >> k);

                            int x_p;

                            if (flip_x) //check x-filp
                                x_p = k + fixed_x;
                            else
                                x_p = (7 - k) + fixed_x; //below 8x8 block

                            if (x_p > 0 && x_p < 160 && pixel != 0)
                                Buffer_Screen_array[x_p][ly] = (byte)Palette_obj_use[pixel];
                        }
                    }
                }
            }
            #endregion
        }

        //GPU screen frame update
        public int frame_fps_count = 0;
        private void GB_ScreenUpdate()
        {
            FastBitmap screen_fast = new FastBitmap(Frame_Screen_Buffer);
            screen_fast.LockImage();

            for (byte x = 0; x <= 159; x++)
                for (byte y = 0; y <= 143; y++)
                    screen_fast.SetPixelByGray(x, y, Buffer_Screen_array[x][y]);

            screen_fast.UnlockImage();
            Screen_Panel.DrawImageUnscaled(Frame_Screen_Buffer, Screen_loc);
        }

    }
}