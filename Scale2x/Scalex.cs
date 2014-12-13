using System.Drawing;
using BitmapProcessingScalex;
//ref 
// http://forum.unity3d.com/threads/scale2x-sample-code-and-project.174656/
// http://6bit.net/shonumi/2013/04/10/in-depth-scale2x/
// http://scale2x.sourceforge.net/algorithm.html
namespace ScalexFilter
{
    public class ScalexTool
    {
        static uint s_A, s_B, s_C, s_D, s_E, s_F, s_G, s_H, s_I, s_E0, s_E1, s_E2, s_E3, s_E4, s_E5, s_E6, s_E7, s_E8;
        static int x_dec_1, x_add_1, y_dec_1, y_add_1;
        static FastBitmap dec_fast;
        //2次 Scale4x計算
        public static void toScale4x(ref uint[][] src_fast, ref uint[][] mid_buffer_2x, ref Bitmap dec, int org_width, int org_height)
        {
            toScale2xAr(src_fast, mid_buffer_2x, org_width, org_height);
            toScale2x(ref mid_buffer_2x, ref dec, org_width << 1, org_height << 1);
        }
        public static void toScale3x(ref uint[][] src_fast, ref Bitmap dec, int org_width, int org_height)
        {
            dec_fast = new FastBitmap(ref dec);
            dec_fast.LockImage();
            for (int x = org_width - 1; x >= 0; x--)
                for (int y = org_height - 1; y >= 0; y--)
                {
                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;
                    s_A = s_B = s_G = s_I = s_C = s_D = s_H = s_F = s_E0 = s_E1 = s_E2 = s_E3 = s_E4 = s_E5 = s_E6 = s_E7 = s_E8 = s_E = src_fast[x][y];
                    if (x_dec_1 >= 0 && y_dec_1 >= 0) s_A = src_fast[x_dec_1][y_dec_1];
                    if (x_dec_1 >= 0 && y_add_1 < org_height) s_G = src_fast[x_dec_1][y_add_1];
                    if (x_add_1 < org_width && y_add_1 < org_height) s_I = src_fast[x_add_1][y_add_1];
                    if (x_add_1 < org_width && y_dec_1 >= 0) s_C = src_fast[x_add_1][y_dec_1];
                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];
                    if (s_B != s_H && s_D != s_F)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if ((s_D == s_B && s_E != s_C) || (s_B == s_F && s_E != s_A)) s_E1 = s_B;
                        if (s_B == s_F) s_E2 = s_F;
                        if ((s_D == s_B && s_E != s_G) || (s_D == s_H && s_E != s_A)) s_E3 = s_D;
                        //s_E4 = s_E;
                        if ((s_B == s_F && s_E != s_I) || (s_H == s_F && s_E != s_C)) s_E5 = s_F;
                        if (s_D == s_H) s_E6 = s_D;
                        if ((s_D == s_H && s_E != s_I) || (s_H == s_F && s_E != s_G)) s_E7 = s_H;
                        if (s_H == s_F) s_E8 = s_F;
                    }
                    dec_fast.SetPixelByRGBValue(0 + x * 3, 0 + y * 3, s_E0);
                    dec_fast.SetPixelByRGBValue(1 + x * 3, 0 + y * 3, s_E1);
                    dec_fast.SetPixelByRGBValue(2 + x * 3, 0 + y * 3, s_E2);
                    dec_fast.SetPixelByRGBValue(0 + x * 3, 1 + y * 3, s_E3);
                    dec_fast.SetPixelByRGBValue(1 + x * 3, 1 + y * 3, s_E4);
                    dec_fast.SetPixelByRGBValue(2 + x * 3, 1 + y * 3, s_E5);
                    dec_fast.SetPixelByRGBValue(0 + x * 3, 2 + y * 3, s_E6);
                    dec_fast.SetPixelByRGBValue(1 + x * 3, 2 + y * 3, s_E7);
                    dec_fast.SetPixelByRGBValue(2 + x * 3, 2 + y * 3, s_E8);
                }
            dec_fast.UnlockImage();
        }
        private static void toScale2xAr(uint[][] src_fast, uint[][] dec, int org_width, int org_height)
        {
            for (int x = org_width - 1; x >= 0; x--)
                for (int y = org_height - 1; y >= 0; y--)
                {
                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;
                    s_E = src_fast[x][y];
                    s_E0 = s_E1 = s_E2 = s_E3 = s_D = s_F = s_B = s_H = s_E;
                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];
                    if (s_B != s_H && s_D != s_F)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }
                    dec[0 + (x << 1)][0 + (y << 1)] = s_E0;
                    dec[1 + (x << 1)][0 + (y << 1)] = s_E1;
                    dec[0 + (x << 1)][1 + (y << 1)] = s_E2;
                    dec[1 + (x << 1)][1 + (y << 1)] = s_E3;
                }
        }
        public static void toScale2x(ref uint[][] src_fast, ref Bitmap dec, int org_width, int org_height)
        {
            dec_fast = new FastBitmap(ref dec);
            dec_fast.LockImage();
            for (int x = org_width - 1; x >= 0; x--)
                for (int y = org_height - 1; y >= 0; y--)
                {
                    s_D = s_F = s_B = s_H = s_E0 = s_E1 = s_E2 = s_E3 = s_E = src_fast[x][y];
                    x_dec_1 = x - 1;
                    x_add_1 = x + 1;
                    y_dec_1 = y - 1;
                    y_add_1 = y + 1;
                    if (x_dec_1 >= 0) s_D = src_fast[x_dec_1][y];
                    if (x_add_1 < org_width) s_F = src_fast[x_add_1][y];
                    if (y_dec_1 >= 0) s_B = src_fast[x][y_dec_1];
                    if (y_add_1 < org_height) s_H = src_fast[x][y_add_1];
                    if (s_B != s_H && s_D != s_F)
                    {
                        if (s_D == s_B) s_E0 = s_D;
                        if (s_B == s_F) s_E1 = s_F;
                        if (s_D == s_H) s_E2 = s_D;
                        if (s_H == s_F) s_E3 = s_F;
                    }
                    dec_fast.SetPixelByRGBValue(0 + (x << 1), 0 + (y << 1), s_E0);
                    dec_fast.SetPixelByRGBValue(1 + (x << 1), 0 + (y << 1), s_E1);
                    dec_fast.SetPixelByRGBValue(0 + (x << 1), 1 + (y << 1), s_E2);
                    dec_fast.SetPixelByRGBValue(1 + (x << 1), 1 + (y << 1), s_E3);
                }
            dec_fast.UnlockImage();
        }
    }
}
