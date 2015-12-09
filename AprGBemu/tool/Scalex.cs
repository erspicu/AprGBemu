using System.Drawing;
using System;
using System.Threading.Tasks;

//ref 
// http://forum.unity3d.com/threads/scale2x-sample-code-and-project.174656/
// http://6bit.net/shonumi/2013/04/10/in-depth-scale2x/
// http://scale2x.sourceforge.net/algorithm.html

namespace ScalexFilter
{
    public unsafe class ScalexTool
    {
        //這裡的特性適合使用GPU平行處理加速,未來目標...

        public static void toScale2x_dx(uint[][] src_fast, int org_width, int org_height, uint[] buffer_2x)
        {

            //for (int x = org_width - 1; x >= 0; x--)
            Parallel.For(0, org_width, x =>
            {
                // Parallel.For(0, org_height, y =>
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_B, s_D, s_E, s_F, s_H, s_E0, s_E1, s_E2, s_E3;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;

                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E = src_fast[x][y];

                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];

                    if ((s_B - s_H) != 0 && (s_D - s_F) != 0)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }

                    buffer_2x[(x * 2) + y * 2 * 320] = s_E0;
                    buffer_2x[(x * 2 + 1) + y * 2 * 320] = s_E1;
                    buffer_2x[(x * 2) + (y * 2 + 1) * 320] = s_E2;
                    buffer_2x[(x * 2 + 1) + (y * 2 + 1) * 320] = s_E3;

                }
            });
        }


        public static void toScale3x_dx(uint[][] src_fast, int org_width, int org_height, uint[] buffer_3x)
        {
            Parallel.For(0, org_width, x =>
            {
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_A, s_B, s_C, s_D, s_E, s_F, s_G, s_H, s_I, s_E0, s_E1, s_E2, s_E3, s_E4, s_E5, s_E6, s_E7, s_E8;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;
                    s_A = s_B = s_G = s_I = s_C = s_D = s_H = s_F = s_E0 = s_E1 = s_E2 = s_E3 = s_E4 = s_E5 = s_E6 = s_E7 = s_E8 = s_E = src_fast[x][y];

                    if (x_dec_1 >= 0)
                    {
                        s_D = src_fast[x_dec_1][y];
                        if (y_dec_1 >= 0) s_A = src_fast[x_dec_1][y_dec_1];
                        if (y_add_1 < org_height) s_G = src_fast[x_dec_1][y_add_1];
                    }

                    if (x_add_1 < org_width)
                    {
                        s_F = src_fast[x_add_1][y];
                        if (y_add_1 < org_height) s_I = src_fast[x_add_1][y_add_1];
                        if (y_dec_1 >= 0) s_C = src_fast[x_add_1][y_dec_1];
                    }

                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];


                    if (s_B != s_H && s_D != s_F)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if ((s_D == s_B && s_E != s_C) || (s_B == s_F && s_E != s_A)) s_E1 = s_B;
                        if (s_B == s_F) s_E2 = s_F;
                        if ((s_D == s_B && s_E != s_G) || (s_D == s_H && s_E != s_A)) s_E3 = s_D;
                        if ((s_B == s_F && s_E != s_I) || (s_H == s_F && s_E != s_C)) s_E5 = s_F;
                        if (s_D == s_H) s_E6 = s_D;
                        if ((s_D == s_H && s_E != s_I) || (s_H == s_F && s_E != s_G)) s_E7 = s_H;
                        if (s_H == s_F) s_E8 = s_F;
                    }

                    buffer_3x[(x * 3) + (y * 3) * 480] = s_E0;
                    buffer_3x[(1 + x * 3) + (y * 3) * 480] = s_E1;
                    buffer_3x[(2 + x * 3) + (y * 3) * 480] = s_E2;
                    buffer_3x[(x * 3) + (1 + y * 3) * 480] = s_E3;
                    buffer_3x[(1 + x * 3) + (1 + y * 3) * 480] = s_E4;
                    buffer_3x[(2 + x * 3) + (1 + y * 3) * 480] = s_E5;
                    buffer_3x[(x * 3) + (2 + y * 3) * 480] = s_E6;
                    buffer_3x[(1 + x * 3) + (2 + y * 3) * 480] = s_E7;
                    buffer_3x[(2 + x * 3) + (2 + y * 3) * 480] = s_E8;
                }
            });
        }

        static uint[] speed_buffer_2x = new uint[320 * 288];
        public static void toScale4x_dx(uint[][] src_fast, int org_width, int org_height, uint[] buffer_4x)
        {
            Parallel.For(0, org_width, x =>
            {
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_B, s_D, s_E, s_F, s_H, s_E0, s_E1, s_E2, s_E3;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;

                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E = src_fast[x][y];

                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];

                    if ((s_B - s_H) != 0 && (s_D - s_F) != 0)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }

                    speed_buffer_2x[(x * 2) + y * 2 * 320] = s_E0;
                    speed_buffer_2x[(x * 2 + 1) + y * 2 * 320] = s_E1;
                    speed_buffer_2x[(x * 2) + (y * 2 + 1) * 320] = s_E2;
                    speed_buffer_2x[(x * 2 + 1) + (y * 2 + 1) * 320] = s_E3;

                }
            });

            org_width = 320;
            org_height = 288;

            Parallel.For(0, org_width, x =>
            {
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_B, s_D, s_E, s_F, s_H, s_E0, s_E1, s_E2, s_E3;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;

                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E = speed_buffer_2x[x + y * 320];

                    if (x_dec_1 >= 0) s_D = speed_buffer_2x[x_dec_1 + y * 320];
                    if (x_add_1 < org_width) s_F = speed_buffer_2x[x_add_1 + y * 320];
                    if (y_dec_1 >= 0) s_B = speed_buffer_2x[x + y_dec_1 * 320];
                    if (y_add_1 < org_height) s_H = speed_buffer_2x[x + y_add_1 * 320];

                    if ((s_B - s_H) != 0 && (s_D - s_F) != 0)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }
                    buffer_4x[(x * 2) + y * 2 * 640] = s_E0;
                    buffer_4x[(x * 2 + 1) + y * 2 * 640] = s_E1;
                    buffer_4x[(x * 2) + (y * 2 + 1) * 640] = s_E2;
                    buffer_4x[(x * 2 + 1) + (y * 2 + 1) * 640] = s_E3;

                }
            });
        }

        static uint[] speed_buffer_3x = new uint[480 * 432];
        public static void toScale6x_dx(uint[][] src_fast, int org_width, int org_height, uint[] buffer_6x)
        {
            Parallel.For(0, org_width, x =>
            {
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_A, s_B, s_C, s_D, s_E, s_F, s_G, s_H, s_I, s_E0, s_E1, s_E2, s_E3, s_E4, s_E5, s_E6, s_E7, s_E8;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;
                    s_A = s_B = s_G = s_I = s_C = s_D = s_H = s_F = s_E0 = s_E1 = s_E2 = s_E3 = s_E4 = s_E5 = s_E6 = s_E7 = s_E8 = s_E = src_fast[x][y];

                    if (x_dec_1 >= 0)
                    {
                        s_D = src_fast[x_dec_1][y];
                        if (y_dec_1 >= 0) s_A = src_fast[x_dec_1][y_dec_1];
                        if (y_add_1 < org_height) s_G = src_fast[x_dec_1][y_add_1];
                    }

                    if (x_add_1 < org_width)
                    {
                        s_F = src_fast[x_add_1][y];
                        if (y_add_1 < org_height) s_I = src_fast[x_add_1][y_add_1];
                        if (y_dec_1 >= 0) s_C = src_fast[x_add_1][y_dec_1];
                    }

                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];


                    if (s_B != s_H && s_D != s_F)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if ((s_D == s_B && s_E != s_C) || (s_B == s_F && s_E != s_A)) s_E1 = s_B;
                        if (s_B == s_F) s_E2 = s_F;
                        if ((s_D == s_B && s_E != s_G) || (s_D == s_H && s_E != s_A)) s_E3 = s_D;
                        if ((s_B == s_F && s_E != s_I) || (s_H == s_F && s_E != s_C)) s_E5 = s_F;
                        if (s_D == s_H) s_E6 = s_D;
                        if ((s_D == s_H && s_E != s_I) || (s_H == s_F && s_E != s_G)) s_E7 = s_H;
                        if (s_H == s_F) s_E8 = s_F;
                    }

                    speed_buffer_3x[(x * 3) + (y * 3) * 480] = s_E0;
                    speed_buffer_3x[(1 + x * 3) + (y * 3) * 480] = s_E1;
                    speed_buffer_3x[(2 + x * 3) + (y * 3) * 480] = s_E2;
                    speed_buffer_3x[(x * 3) + (1 + y * 3) * 480] = s_E3;
                    speed_buffer_3x[(1 + x * 3) + (1 + y * 3) * 480] = s_E4;
                    speed_buffer_3x[(2 + x * 3) + (1 + y * 3) * 480] = s_E5;
                    speed_buffer_3x[(x * 3) + (2 + y * 3) * 480] = s_E6;
                    speed_buffer_3x[(1 + x * 3) + (2 + y * 3) * 480] = s_E7;
                    speed_buffer_3x[(2 + x * 3) + (2 + y * 3) * 480] = s_E8;
                }
            });

            org_width = 480;
            org_height = 432;

            Parallel.For(0, org_width, x =>
            {
                for (int y = org_height - 1; y >= 0; y--)
                {

                    uint s_B, s_D, s_E, s_F, s_H, s_E0, s_E1, s_E2, s_E3;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;

                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E = speed_buffer_3x[x + y * 480];

                    if (x_dec_1 >= 0) s_D = speed_buffer_3x[x_dec_1 + y * 480];
                    if (x_add_1 < org_width) s_F = speed_buffer_3x[x_add_1 + y * 480];
                    if (y_dec_1 >= 0) s_B = speed_buffer_3x[x + y_dec_1 * 480];
                    if (y_add_1 < org_height) s_H = speed_buffer_3x[x + y_add_1 * 480];

                    if ((s_B - s_H) != 0 && (s_D - s_F) != 0)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }
                    buffer_6x[(x * 2) + y * 2 * 960] = s_E0;
                    buffer_6x[(x * 2 + 1) + y * 2 * 960] = s_E1;
                    buffer_6x[(x * 2) + (y * 2 + 1) * 960] = s_E2;
                    buffer_6x[(x * 2 + 1) + (y * 2 + 1) * 960] = s_E3;

                }
            });
        }

        public static void _toScale4x_dx(uint[][] src_fast, int org_width, int org_height, uint[] buffer_4x)
        {
            //for low speed pc method 
            // Scale2x  -> normal 2x

            //for (int x = org_width - 1; x >= 0; x--)
            Parallel.For(0, org_width, x =>
            {
                // Parallel.For(0, org_height, y =>
                for (int y = org_height - 1; y >= 0; y--)
                {
                    uint s_B, s_D, s_E, s_F, s_H, s_E0, s_E1, s_E2, s_E3;
                    int x_dec_1, x_add_1, y_dec_1, y_add_1;
                    int t_x, t_y;

                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;

                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E = src_fast[x][y];

                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];

                    if ((s_B - s_H) != 0 && (s_D - s_F) != 0)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }

                    int tx_1, tx_2, tx_3, ty_1, ty_2, ty_3;
                    t_x = x << 2;
                    t_y = y << 2;

                    tx_1 = t_x + 1;
                    tx_2 = t_x + 2;
                    tx_3 = t_x + 3;

                    ty_1 = t_y + 1;
                    ty_2 = t_y + 2;
                    ty_3 = t_y + 3;

                    buffer_4x[(t_x) + (t_y) * 640] = buffer_4x[(tx_1) + (t_y) * 640] = buffer_4x[(t_x) + (ty_1) * 640] = buffer_4x[(tx_1) + (ty_1) * 640] = s_E0;
                    buffer_4x[(tx_2) + (t_y) * 640] = buffer_4x[(tx_3) + (t_y) * 640] = buffer_4x[(tx_2) + (ty_1) * 640] = buffer_4x[(tx_3) + (ty_1) * 640] = s_E1;
                    buffer_4x[(t_x) + (ty_2) * 640] = buffer_4x[(tx_1) + (ty_2) * 640] = buffer_4x[(t_x) + (ty_3) * 640] = buffer_4x[(tx_1) + (ty_3) * 640] = s_E2;
                    buffer_4x[(tx_2) + (ty_2) * 640] = buffer_4x[(tx_3) + (ty_2) * 640] = buffer_4x[(tx_2) + (ty_3) * 640] = buffer_4x[(tx_3) + (ty_3) * 640] = s_E3;

                }
            });
        }
    }
}
