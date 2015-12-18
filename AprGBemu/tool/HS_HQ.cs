using System;
using System.Threading.Tasks;

//速度優化 from project https://code.google.com/p/hqx-sharp

// https://csnative.codeplex.com/downloads/get/1507161
// https://github.com/dotnet/roslyn
//https://github.com/icebreaker/2dimagefilter

namespace hqx_speed
{
    public class HS_HQ
    {

        static int[] lTable;

        const int Ymask = 0x00ff0000;
        const int Umask = 0x0000ff00;
        const int Vmask = 0x000000ff;

        const uint Mask4 = 0xff000000;
        const uint Mask2 = 0x0000ff00;
        const uint Mask13 = 0x00ff00ff;

        static uint trY = 3145728, trU = 1792, trV = 6, trA = 0;



        public static unsafe void initTable()
        {

            if (lTable != null)
            {
                //Console.WriteLine("RGB2YUB Table 已建立.");
                return;
            }
            lTable = new int[0x1000000];

            for (uint i = 0; i < lTable.Length; i++)
            {

                float r = (i & 0xff0000) >> 16;
                float g = (i & 0x00ff00) >> 8;
                float b = (i & 0x0000ff);

                lTable[i] = (byte)(.299 * r + .587 * g + .114 * b) | ((byte)((int)(-.169 * r - .331 * g + .5 * b) + 128) << 8) | ((byte)((int)(.5 * r - .419 * g - .081 * b) + 128) << 16);
            }

            // Console.WriteLine("RGB2YUB Table 建立完成.");
        }


        private static bool Diff(uint c1, uint c2)
        {
            int YUV1 = lTable[c1 & 0x00ffffff];
            int YUV2 = lTable[c2 & 0x00ffffff];

            if (((c1 & 0xff000000) ^ (c2 & 0xff000000)) != 0) return true;

            int t2 = (YUV1 & Umask) - (YUV2 & Umask);
            if (t2 * t2 > 3211264) return true;

            int t3 = (YUV1 & Vmask) - (YUV2 & Vmask);
            if (t3 * t3 > 36) return true;

            int t1 = (YUV1 & Ymask) - (YUV2 & Ymask);
            if ((long)t1 * t1 >= 9895604649984) return true;

            /*return ((Math.Abs((YUV1 & Ymask) - (YUV2 & Ymask)) > trY) ||
 (Math.Abs((YUV1 & Umask) - (YUV2 & Umask)) > trU) ||
 (Math.Abs((YUV1 & Vmask) - (YUV2 & Vmask)) > trV) ||
 (Math.Abs(((int)((uint)c1 >> 24) - (int)((uint)c2 >> 24))) > trA));*/


            return false;
        }


        /*
        private static uint Mix3To1(uint c1, uint c2)
        {
            if (c1 == c2) return c1;
            return (c1 * 3 + c2) >> 2;
        }

        private static uint Mix2To1To1(uint c1, uint c2, uint c3)
        {
            return ((c1 << 1) + c2 + c3) >> 2;
        }

        private static uint Mix7To1(uint c1, uint c2)
        {
            if (c1 == c2) return c1;
            return (c1 * 7 + c2) >> 3;
        }

        private static uint Mix2To7To7(uint c1, uint c2, uint c3)
        {
            return ((c1 << 1) + (c2 + c3) * 7) >> 4;
        }

        private static uint MixEven(uint c1, uint c2)
        {
            if (c1 == c2) return c1;
            return (c1 + c2) >> 1;
        }

        public static uint Mix4To2To1(uint c1, uint c2, uint c3)
        {
            return (c1 * 5 + (c2 << 1) + c3) >> 3;
        }

        public static uint Mix6To1To1(uint c1, uint c2, uint c3)
        {
            return (c1 * 6 + c2 + c3) >> 3;
        }

        public static uint Mix5To3(uint c1, uint c2)
        {
            if (c1 == c2) return c1;
            return (c1 * 5 + c2 * 3) >> 3;
        }

        public static uint Mix2To3To3(uint c1, uint c2, uint c3)
        {
            return ((c1 << 1) + (c2 + c3) * 3) >> 3;
        }

        public static uint Mix14To1To1(uint c1, uint c2, uint c3)
        {
            return (c1 * 14 + c2 + c3) >> 4;
        }*/

        public static uint Mix3To1(uint c1, uint c2)
        {
            //return (c1*3+c2) >> 2;
            if (c1 == c2)
            {
                return c1;

            }
            return ((((c1 & Mask2) * 3 + (c2 & Mask2)) >> 2) & Mask2) |
                ((((c1 & Mask13) * 3 + (c2 & Mask13)) >> 2) & Mask13) |
                ((((c1 & Mask4) >> 2) * 3 + ((c2 & Mask4) >> 2)) & Mask4);
        }

        public static uint Mix2To1To1(uint c1, uint c2, uint c3)
        {
            //return (c1*2+c2+c3) >> 2;
            return ((((c1 & Mask2) * 2 + (c2 & Mask2) + (c3 & Mask2)) >> 2) & Mask2) |
                  ((((c1 & Mask13) * 2 + (c2 & Mask13) + (c3 & Mask13)) >> 2) & Mask13) |
                ((((c1 & Mask4) >> 2) * 2 + ((c2 & Mask4) >> 2) + ((c3 & Mask4) >> 2)) & Mask4);
        }

        public static uint Mix7To1(uint c1, uint c2)
        {
            //return (c1*7+c2)/8;
            if (c1 == c2)
            {
                return c1;

            }
            return ((((c1 & Mask2) * 7 + (c2 & Mask2)) >> 3) & Mask2) |
                ((((c1 & Mask13) * 7 + (c2 & Mask13)) >> 3) & Mask13) |
                ((((c1 & Mask4) >> 3) * 7 + ((c2 & Mask4) >> 3)) & Mask4);
        }

        public static uint Mix2To7To7(uint c1, uint c2, uint c3)
        {
            //return (c1*2+(c2+c3)*7)/16;
            return ((((c1 & Mask2) * 2 + (c2 & Mask2) * 7 + (c3 & Mask2) * 7) >> 4) & Mask2) |
                  ((((c1 & Mask13) * 2 + (c2 & Mask13) * 7 + (c3 & Mask13) * 7) >> 4) & Mask13) |
                ((((c1 & Mask4) >> 4) * 2 + ((c2 & Mask4) >> 4) * 7 + ((c3 & Mask4) >> 4) * 7) & Mask4);
        }

        public static uint MixEven(uint c1, uint c2)
        {
            //return (c1+c2) >> 1;
            if (c1 == c2)
            {
                return c1;

            }
            return ((((c1 & Mask2) + (c2 & Mask2)) >> 1) & Mask2) |
                ((((c1 & Mask13) + (c2 & Mask13)) >> 1) & Mask13) |
                ((((c1 & Mask4) >> 1) + ((c2 & Mask4) >> 1)) & Mask4);
        }

        public static uint Mix4To2To1(uint c1, uint c2, uint c3)
        {
            //return (c1*5+c2*2+c3)/8;
            return ((((c1 & Mask2) * 5 + (c2 & Mask2) * 2 + (c3 & Mask2)) >> 3) & Mask2) |
                  ((((c1 & Mask13) * 5 + (c2 & Mask13) * 2 + (c3 & Mask13)) >> 3) & Mask13) |
                ((((c1 & Mask4) >> 3) * 5 + ((c2 & Mask4) >> 3) * 2 + ((c3 & Mask4) >> 3)) & Mask4);
        }

        public static uint Mix6To1To1(uint c1, uint c2, uint c3)
        {
            //return (c1*6+c2+c3)/8;
            return ((((c1 & Mask2) * 6 + (c2 & Mask2) + (c3 & Mask2)) >> 3) & Mask2) |
                  ((((c1 & Mask13) * 6 + (c2 & Mask13) + (c3 & Mask13)) >> 3) & Mask13) |
                ((((c1 & Mask4) >> 3) * 6 + ((c2 & Mask4) >> 3) + ((c3 & Mask4) >> 3)) & Mask4);
        }

        public static uint Mix5To3(uint c1, uint c2)
        {
            //return (c1*5+c2*3)/8;
            if (c1 == c2)
            {
                return c1;

            }
            return ((((c1 & Mask2) * 5 + (c2 & Mask2) * 3) >> 3) & Mask2) |
                  ((((c1 & Mask13) * 5 + (c2 & Mask13) * 3) >> 3) & Mask13) |
                ((((c1 & Mask4) >> 3) * 5 + ((c2 & Mask4) >> 3) * 3) & Mask4);
        }

        public static uint Mix2To3To3(uint c1, uint c2, uint c3)
        {
            //return (c1*2+(c2+c3)*3)/8;
            return ((((c1 & Mask2) * 2 + (c2 & Mask2) * 3 + (c3 & Mask2) * 3) >> 3) & Mask2) |
                  ((((c1 & Mask13) * 2 + (c2 & Mask13) * 3 + (c3 & Mask13) * 3) >> 3) & Mask13) |
                ((((c1 & Mask4) >> 3) * 2 + ((c2 & Mask4) >> 3) * 3 + ((c3 & Mask4) >> 3) * 3) & Mask4);
        }

        public static uint Mix14To1To1(uint c1, uint c2, uint c3)
        {
            //return (c1*14+c2+c3)/16;
            return ((((c1 & Mask2) * 14 + (c2 & Mask2) + (c3 & Mask2)) >> 4) & Mask2) |
                  ((((c1 & Mask13) * 14 + (c2 & Mask13) + (c3 & Mask13)) >> 4) & Mask13) |
                ((((c1 & Mask4) >> 4) * 14 + ((c2 & Mask4) >> 4) + ((c3 & Mask4) >> 4)) & Mask4);
        }

        static uint[] speed_buffer_3x;
        public static void Scale6(uint[][] sp, int Xres, int Yres, uint[] dp)
        {

            if (speed_buffer_3x == null)
            {
                speed_buffer_3x = new uint[Xres * 3 * Yres * 3];
            }

            Scale3(sp, Xres, Yres, speed_buffer_3x);
            _Scale2(speed_buffer_3x, Xres * 3, Yres * 3, dp);

        }

        public static void Scale4(uint[][] sp, int Xres, int Yres, uint[] dp)
        {
            int dpL = Xres << 2;

            Parallel.For(0, Yres, j =>
            {

                int prevline = 0;
                int nextline = 0;
                int j_prevline = 0;
                int j_nextline = 0;

                uint e0, e1, e2, e3, e4, e5, e6, e7, e8;

                // prevline = nextline = 0;

                if (j > 0) prevline = -1;
                if (j < Yres - 1) nextline = 1;

                j_prevline = j + prevline;
                j_nextline = j + nextline;

                for (int i = Xres - 1; i >= 0; i--)
                {

                    e1 = sp[i][j_prevline];
                    e4 = sp[i][j];
                    e7 = sp[i][j_nextline];

                    if (i > 0)
                    {
                        e0 = sp[i - 1][j_prevline];
                        e3 = sp[i - 1][j];
                        e6 = sp[i - 1][j_nextline];
                    }
                    else
                    {
                        e0 = e1;
                        e3 = e4;
                        e6 = e7;
                    }

                    if (i < Xres - 1)
                    {
                        e2 = sp[i + 1][j_prevline];
                        e5 = sp[i + 1][j];
                        e8 = sp[i + 1][j_nextline];
                    }
                    else
                    {
                        e2 = e1;
                        e5 = e4;
                        e8 = e7;
                    }

                    int pattern = 0;

                    if (e0 != e4 && Diff(e4, e0)) pattern |= 1;
                    if (e1 != e4 && Diff(e4, e1)) pattern |= 2;
                    if (e2 != e4 && Diff(e4, e2)) pattern |= 4;
                    if (e3 != e4 && Diff(e4, e3)) pattern |= 8;
                    if (e5 != e4 && Diff(e4, e5)) pattern |= 16;
                    if (e6 != e4 && Diff(e4, e6)) pattern |= 32;
                    if (e7 != e4 && Diff(e4, e7)) pattern |= 64;
                    if (e8 != e4 && Diff(e4, e8)) pattern |= 128;


                    //-------

                    //----------------------------------------------
                    switch (pattern)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 32:
                        case 128:
                        case 5:
                        case 132:
                        case 160:
                        case 33:
                        case 129:
                        case 36:
                        case 133:
                        case 164:
                        case 161:
                        case 37:
                        case 165:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 2:
                        case 34:
                        case 130:
                        case 162:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 16:
                        case 17:
                        case 48:
                        case 49:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 8:
                        case 12:
                        case 136:
                        case 140:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 3:
                        case 35:
                        case 131:
                        case 163:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 6:
                        case 38:
                        case 134:
                        case 166:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 20:
                        case 21:
                        case 52:
                        case 53:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 144:
                        case 145:
                        case 176:
                        case 177:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 192:
                        case 193:
                        case 196:
                        case 197:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 96:
                        case 97:
                        case 100:
                        case 101:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 40:
                        case 44:
                        case 168:
                        case 172:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 9:
                        case 13:
                        case 137:
                        case 141:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 18:
                        case 50:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 80:
                        case 81:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 72:
                        case 76:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 10:
                        case 138:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 66:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 24:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 7:
                        case 39:
                        case 135:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 148:
                        case 149:
                        case 180:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 224:
                        case 228:
                        case 225:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 41:
                        case 169:
                        case 45:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 22:
                        case 54:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 208:
                        case 209:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 104:
                        case 108:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 11:
                        case 139:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 19:
                        case 51:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e1, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix2To1To1(e5, e4, e1);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 146:
                        case 178:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 84:
                        case 85:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e5, e7);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 112:
                        case 113:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix2To1To1(e5, e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 200:
                        case 204:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix2To1To1(e3, e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 73:
                        case 77:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e3, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 42:
                        case 170:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e3, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 14:
                        case 142:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e1, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix2To1To1(e3, e4, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 67:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 70:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 28:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 152:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 194:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 98:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 56:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 25:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 26:
                        case 31:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 82:
                        case 214:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 88:
                        case 248:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 74:
                        case 107:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 27:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 86:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 216:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 106:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 30:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 210:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 120:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 75:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 29:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 198:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 184:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 99:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 57:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 71:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 156:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 226:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 60:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 195:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 102:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 153:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 58:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 83:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 92:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 202:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 78:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 154:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 114:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                break;
                            }
                        case 89:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 90:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 55:
                        case 23:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e1, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix2To1To1(e5, e4, e1);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 182:
                        case 150:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 213:
                        case 212:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e5, e7);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 241:
                        case 240:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix2To1To1(e5, e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 236:
                        case 232:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix2To1To1(e3, e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 109:
                        case 105:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e3, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 171:
                        case 43:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e3, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 143:
                        case 15:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e1, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix2To1To1(e3, e4, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 124:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 203:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 62:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 211:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 118:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 217:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 110:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 155:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 188:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 185:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 61:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 157:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 103:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 227:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 230:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 199:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 220:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 158:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 234:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 242:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                break;
                            }
                        case 59:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 121:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 87:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 79:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 122:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 94:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 218:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 91:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 229:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 167:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 173:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 181:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 186:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 115:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                break;
                            }
                        case 93:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 206:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 205:
                        case 201:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 174:
                        case 46:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 179:
                        case 147:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 117:
                        case 116:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                break;
                            }
                        case 189:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 231:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 126:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 219:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 125:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e3, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 221:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e5, e7);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix2To1To1(e7, e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 207:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e1, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix2To1To1(e3, e4, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 238:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix2To1To1(e3, e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 190:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e5, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 187:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix2To1To1(e1, e4, e3);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e3, e1);
                                    dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 243:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix2To1To1(e5, e4, e7);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix3To1(e4, e7);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e7, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e7, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 119:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e1, e4);
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e1, e5);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix2To1To1(e5, e4, e1);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 237:
                        case 233:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e5, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 175:
                        case 47:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e5, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                break;
                            }
                        case 183:
                        case 151:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix6To1To1(e4, e3, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 245:
                        case 244:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 250:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                break;
                            }
                        case 123:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 95:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 222:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 252:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e0);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 249:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix4To2To1(e4, e1, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                break;
                            }
                        case 235:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e5, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 111:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e5, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 63:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 159:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix4To2To1(e4, e7, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 215:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix4To2To1(e4, e3, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 246:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix4To2To1(e4, e3, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 254:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e0);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e0);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 253:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e1);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e1);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 251:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e2);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e2);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                break;
                            }
                        case 239:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                dp[(i << 2) + 2 + (j << 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + (j << 2) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = Mix5To3(e4, e5);
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix5To3(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e5);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e5);
                                break;
                            }
                        case 127:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 2 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = MixEven(e1, e5);
                                    dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = MixEven(e5, e4);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 2) * dpL] = MixEven(e3, e4);
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = MixEven(e7, e3);
                                    dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e8);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e8);
                                break;
                            }
                        case 191:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e7);
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix5To3(e4, e7);
                                break;
                            }
                        case 223:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = MixEven(e1, e3);
                                    dp[(i << 2) + 1 + (j << 2) * dpL] = MixEven(e1, e4);
                                    dp[(i << 2) + ((j << 2) + 1) * dpL] = MixEven(e3, e4);
                                }
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e6);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = MixEven(e5, e4);
                                    dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = MixEven(e7, e4);
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = MixEven(e7, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e6);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 247:
                            {
                                dp[(i << 2) + (j << 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + (j << 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix5To3(e4, e3);
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = Mix7To1(e4, e3);
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 255:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + (j << 2) * dpL] = Mix2To1To1(e4, e1, e3);
                                }
                                dp[(i << 2) + 1 + (j << 2) * dpL] = e4;
                                dp[(i << 2) + 2 + (j << 2) * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + (j << 2) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 2) + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 1) * dpL] = e4;
                                dp[(i << 2) + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 1 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 2) * dpL] = e4;
                                dp[(i << 2) + 3 + ((j << 2) + 2) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 2) + 1 + ((j << 2) + 3) * dpL] = e4;
                                dp[(i << 2) + 2 + ((j << 2) + 3) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 2) + 3 + ((j << 2) + 3) * dpL] = Mix2To1To1(e4, e7, e5);
                                }
                                break;
                            }
                    }




                    //------
                }
            });



        }




        public static void Scale3(uint[][] sp, int Xres, int Yres, uint[] dp)
        {
            int dpL = Xres * 3;

            Parallel.For(0, Yres, j =>
            {

                int prevline = 0, nextline = 0, j_prevline, j_nextline;

                uint e0, e1, e2, e3, e4, e5, e6, e7, e8;

                if (j > 0) prevline = -1;
                if (j < Yres - 1) nextline = 1;

                j_prevline = j + prevline;
                j_nextline = j + nextline;

                for (int i = Xres - 1; i >= 0; i--)
                {

                    e1 = sp[i][j_prevline];
                    e4 = sp[i][j];
                    e7 = sp[i][j_nextline];

                    if (i > 0)
                    {
                        e0 = sp[i - 1][j_prevline];
                        e3 = sp[i - 1][j];
                        e6 = sp[i - 1][j_nextline];
                    }
                    else
                    {
                        e0 = e1;
                        e3 = e4;
                        e6 = e7;
                    }

                    if (i < Xres - 1)
                    {
                        e2 = sp[i + 1][j_prevline];
                        e5 = sp[i + 1][j];
                        e8 = sp[i + 1][j_nextline];
                    }
                    else
                    {
                        e2 = e1;
                        e5 = e4;
                        e8 = e7;
                    }

                    int pattern = 0;

                    if (e0 != e4 && Diff(e4, e0)) pattern |= 1;
                    if (e1 != e4 && Diff(e4, e1)) pattern |= 2;
                    if (e2 != e4 && Diff(e4, e2)) pattern |= 4;
                    if (e3 != e4 && Diff(e4, e3)) pattern |= 8;
                    if (e5 != e4 && Diff(e4, e5)) pattern |= 16;
                    if (e6 != e4 && Diff(e4, e6)) pattern |= 32;
                    if (e7 != e4 && Diff(e4, e7)) pattern |= 64;
                    if (e8 != e4 && Diff(e4, e8)) pattern |= 128;

                    //-----
                    switch (pattern)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 32:
                        case 128:
                        case 5:
                        case 132:
                        case 160:
                        case 33:
                        case 129:
                        case 36:
                        case 133:
                        case 164:
                        case 161:
                        case 37:
                        case 165:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 2:
                        case 34:
                        case 130:
                        case 162:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 16:
                        case 17:
                        case 48:
                        case 49:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 8:
                        case 12:
                        case 136:
                        case 140:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 3:
                        case 35:
                        case 131:
                        case 163:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 6:
                        case 38:
                        case 134:
                        case 166:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 20:
                        case 21:
                        case 52:
                        case 53:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 144:
                        case 145:
                        case 176:
                        case 177:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 192:
                        case 193:
                        case 196:
                        case 197:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 96:
                        case 97:
                        case 100:
                        case 101:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 40:
                        case 44:
                        case 168:
                        case 172:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 9:
                        case 13:
                        case 137:
                        case 141:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 18:
                        case 50:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 80:
                        case 81:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 72:
                        case 76:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 10:
                        case 138:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 66:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 24:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 7:
                        case 39:
                        case 135:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 148:
                        case 149:
                        case 180:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 224:
                        case 228:
                        case 225:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 41:
                        case 169:
                        case 45:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 22:
                        case 54:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 208:
                        case 209:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 104:
                        case 108:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 11:
                        case 139:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 19:
                        case 51:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 146:
                        case 178:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 84:
                        case 85:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 112:
                        case 113:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                break;
                            }
                        case 200:
                        case 204:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 73:
                        case 77:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 42:
                        case 170:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 14:
                        case 142:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 67:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 70:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 28:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 152:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 194:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 98:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 56:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 25:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 26:
                        case 31:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 82:
                        case 214:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 88:
                        case 248:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 74:
                        case 107:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 27:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 86:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 216:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 106:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 30:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 210:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 120:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 75:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 29:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 198:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 184:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 99:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 57:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 71:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 156:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 226:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 60:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 195:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 102:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 153:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 58:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 83:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 92:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 202:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 78:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 154:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 114:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 89:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 90:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 55:
                        case 23:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 182:
                        case 150:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 213:
                        case 212:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 241:
                        case 240:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                break;
                            }
                        case 236:
                        case 232:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 109:
                        case 105:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 171:
                        case 43:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 143:
                        case 15:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 124:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 203:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 62:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 211:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 118:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 217:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 110:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 155:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 188:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 185:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 61:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 157:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 103:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 227:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 230:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 199:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 220:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 158:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 234:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 242:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 59:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 121:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 87:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 79:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 122:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 94:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 218:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 91:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 229:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 167:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 173:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 181:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 186:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 115:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 93:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 206:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 205:
                        case 201:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 174:
                        case 46:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 179:
                        case 147:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 117:
                        case 116:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 189:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 231:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 126:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 219:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 125:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 221:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 207:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 238:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = MixEven(e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 190:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e5, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 187:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = MixEven(e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e3, e4);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 243:
                            {
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e7, e4);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = MixEven(e5, e7);
                                }
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                break;
                            }
                        case 119:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e1, e4);
                                    dp[i * 3 + 2 + j * 3 * dpL] = MixEven(e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 237:
                        case 233:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 175:
                        case 47:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 183:
                        case 151:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 245:
                        case 244:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 250:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 123:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 95:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 222:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 252:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 249:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 235:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 111:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 63:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 159:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 215:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 246:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 254:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 253:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 1 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e1);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 251:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                }
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e2);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 239:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                dp[i * 3 + 2 + j * 3 * dpL] = Mix3To1(e4, e5);
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 127:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To7To7(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e7, e3);
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                }
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 191:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 223:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                    dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To7To7(e4, e3, e1);
                                    dp[i * 3 + (j * 3 + 1) * dpL] = Mix7To1(e4, e3);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + j * 3 * dpL] = Mix7To1(e4, e1);
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                    dp[i * 3 + 2 + (j * 3 + 1) * dpL] = Mix7To1(e4, e5);
                                }
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 1 + (j * 3 + 2) * dpL] = Mix7To1(e4, e7);
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To7To7(e4, e5, e7);
                                }
                                break;
                            }
                        case 247:
                            {
                                dp[i * 3 + j * 3 * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + (j * 3 + 2) * dpL] = Mix3To1(e4, e3);
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 255:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[i * 3 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + j * 3 * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[i * 3 + 1 + j * 3 * dpL] = e4;
                                if (Diff(e1, e5))
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + j * 3 * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[i * 3 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 1 + (j * 3 + 1) * dpL] = e4;
                                dp[i * 3 + 2 + (j * 3 + 1) * dpL] = e4;
                                if (Diff(e7, e3))
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[i * 3 + 1 + (j * 3 + 2) * dpL] = e4;
                                if (Diff(e5, e7))
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = e4;
                                }
                                else
                                {
                                    dp[i * 3 + 2 + (j * 3 + 2) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                    }
                    //-----
                }
            });

        }


        public static void Scale2(uint[][] sp, int Xres, int Yres, uint[] dp)
        {
            int dpL = Xres * 2;

            Parallel.For(0, Yres, j =>
            {

                int prevline = 0, nextline = 0, j_prevline, j_nextline;

                uint e0, e1, e2, e3, e4, e5, e6, e7, e8;

                if (j > 0) prevline = -1;
                if (j < Yres - 1) nextline = 1;

                j_prevline = j + prevline;
                j_nextline = j + nextline;

                for (int i = Xres - 1; i >= 0; i--)
                {

                    e1 = sp[i][j_prevline];
                    e4 = sp[i][j];
                    e7 = sp[i][j_nextline];

                    if (i > 0)
                    {
                        e0 = sp[i - 1][j_prevline];
                        e3 = sp[i - 1][j];
                        e6 = sp[i - 1][j_nextline];
                    }
                    else
                    {
                        e0 = e1;
                        e3 = e4;
                        e6 = e7;
                    }

                    if (i < Xres - 1)
                    {
                        e2 = sp[i + 1][j_prevline];
                        e5 = sp[i + 1][j];
                        e8 = sp[i + 1][j_nextline];
                    }
                    else
                    {
                        e2 = e1;
                        e5 = e4;
                        e8 = e7;
                    }

                    int pattern = 0;

                    if (e0 != e4 && Diff(e4, e0)) pattern |= 1;
                    if (e1 != e4 && Diff(e4, e1)) pattern |= 2;
                    if (e2 != e4 && Diff(e4, e2)) pattern |= 4;
                    if (e3 != e4 && Diff(e4, e3)) pattern |= 8;
                    if (e5 != e4 && Diff(e4, e5)) pattern |= 16;
                    if (e6 != e4 && Diff(e4, e6)) pattern |= 32;
                    if (e7 != e4 && Diff(e4, e7)) pattern |= 64;
                    if (e8 != e4 && Diff(e4, e8)) pattern |= 128;

                    switch (pattern)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 32:
                        case 128:
                        case 5:
                        case 132:
                        case 160:
                        case 33:
                        case 129:
                        case 36:
                        case 133:
                        case 164:
                        case 161:
                        case 37:
                        case 165:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 2:
                        case 34:
                        case 130:
                        case 162:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 16:
                        case 17:
                        case 48:
                        case 49:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 8:
                        case 12:
                        case 136:
                        case 140:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 3:
                        case 35:
                        case 131:
                        case 163:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 6:
                        case 38:
                        case 134:
                        case 166:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 20:
                        case 21:
                        case 52:
                        case 53:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 144:
                        case 145:
                        case 176:
                        case 177:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 192:
                        case 193:
                        case 196:
                        case 197:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 96:
                        case 97:
                        case 100:
                        case 101:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 40:
                        case 44:
                        case 168:
                        case 172:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 9:
                        case 13:
                        case 137:
                        case 141:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 18:
                        case 50:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 80:
                        case 81:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 72:
                        case 76:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 10:
                        case 138:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 66:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 24:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 7:
                        case 39:
                        case 135:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 148:
                        case 149:
                        case 180:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 224:
                        case 228:
                        case 225:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 41:
                        case 169:
                        case 45:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 22:
                        case 54:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 208:
                        case 209:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 104:
                        case 108:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 11:
                        case 139:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 19:
                        case 51:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 146:
                        case 178:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                break;
                            }
                        case 84:
                        case 85:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                break;
                            }
                        case 112:
                        case 113:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 200:
                        case 204:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 73:
                        case 77:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 42:
                        case 170:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 14:
                        case 142:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 67:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 70:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 28:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 152:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 194:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 98:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 56:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 25:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 26:
                        case 31:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 82:
                        case 214:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 88:
                        case 248:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 74:
                        case 107:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 27:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 86:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 216:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 106:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 30:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 210:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 120:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 75:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 29:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 198:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 184:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 99:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 57:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 71:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 156:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 226:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 60:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 195:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 102:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 153:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 58:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 83:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 92:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 202:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 78:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 154:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 114:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 89:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 90:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 55:
                        case 23:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 182:
                        case 150:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                break;
                            }
                        case 213:
                        case 212:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                break;
                            }
                        case 241:
                        case 240:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 236:
                        case 232:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 109:
                        case 105:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 171:
                        case 43:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 143:
                        case 15:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 124:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 203:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 62:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 211:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 118:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 217:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 110:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 155:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 188:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 185:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 61:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 157:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 103:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 227:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 230:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 199:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 220:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 158:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 234:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 242:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 59:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 121:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 87:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 79:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 122:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 94:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 218:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 91:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 229:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 167:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 173:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 181:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 186:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 115:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 93:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 206:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 205:
                        case 201:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 174:
                        case 46:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 179:
                        case 147:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 117:
                        case 116:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 189:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 231:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 126:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 219:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 125:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 221:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 207:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 238:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 190:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 187:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 243:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 119:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 237:
                        case 233:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 175:
                        case 47:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 183:
                        case 151:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 245:
                        case 244:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 250:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 123:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 95:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 222:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 252:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 249:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 235:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 111:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 63:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 159:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 215:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 246:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 254:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 253:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 251:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 239:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 127:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 191:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 223:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 247:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 255:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                    }
                }
            });
        }



        public static void _Scale2(uint[] sp, int Xres, int Yres, uint[] dp)
        {
            int dpL = Xres * 2;

            Parallel.For(0, Yres, j =>
            {

                int prevline = 0, nextline = 0;//, j_prevline, j_nextline;

                uint e0, e1, e2, e3, e4, e5, e6, e7, e8;

                if (j > 0) prevline = -Xres;
                if (j < Yres - 1) nextline = Xres;

                //j_prevline = j + prevline;
                // j_nextline = j + nextline;

                for (int i = Xres - 1; i >= 0; i--)
                {

                    e1 = sp[i + j * Xres + prevline];
                    e4 = sp[i + j * Xres];
                    e7 = sp[i + j * Xres + nextline];

                    if (i > 0)
                    {
                        e0 = sp[i - 1 + j * Xres + prevline];
                        e3 = sp[i - 1 + j * Xres];
                        e6 = sp[i - 1 + j * Xres + nextline];
                    }
                    else
                    {
                        e0 = e1;
                        e3 = e4;
                        e6 = e7;
                    }

                    if (i < Xres - 1)
                    {
                        e2 = sp[i + 1 + j * Xres + prevline];
                        e5 = sp[i + 1 + j * Xres];
                        e8 = sp[i + 1 + j * Xres + nextline];
                    }
                    else
                    {
                        e2 = e1;
                        e5 = e4;
                        e8 = e7;
                    }

                    int pattern = 0;

                    if (e0 != e4 && Diff(e4, e0)) pattern |= 1;
                    if (e1 != e4 && Diff(e4, e1)) pattern |= 2;
                    if (e2 != e4 && Diff(e4, e2)) pattern |= 4;
                    if (e3 != e4 && Diff(e4, e3)) pattern |= 8;
                    if (e5 != e4 && Diff(e4, e5)) pattern |= 16;
                    if (e6 != e4 && Diff(e4, e6)) pattern |= 32;
                    if (e7 != e4 && Diff(e4, e7)) pattern |= 64;
                    if (e8 != e4 && Diff(e4, e8)) pattern |= 128;

                    switch (pattern)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 32:
                        case 128:
                        case 5:
                        case 132:
                        case 160:
                        case 33:
                        case 129:
                        case 36:
                        case 133:
                        case 164:
                        case 161:
                        case 37:
                        case 165:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 2:
                        case 34:
                        case 130:
                        case 162:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 16:
                        case 17:
                        case 48:
                        case 49:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 8:
                        case 12:
                        case 136:
                        case 140:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 3:
                        case 35:
                        case 131:
                        case 163:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 6:
                        case 38:
                        case 134:
                        case 166:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 20:
                        case 21:
                        case 52:
                        case 53:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 144:
                        case 145:
                        case 176:
                        case 177:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 192:
                        case 193:
                        case 196:
                        case 197:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 96:
                        case 97:
                        case 100:
                        case 101:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 40:
                        case 44:
                        case 168:
                        case 172:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 9:
                        case 13:
                        case 137:
                        case 141:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 18:
                        case 50:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 80:
                        case 81:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 72:
                        case 76:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 10:
                        case 138:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 66:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 24:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 7:
                        case 39:
                        case 135:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 148:
                        case 149:
                        case 180:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 224:
                        case 228:
                        case 225:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 41:
                        case 169:
                        case 45:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 22:
                        case 54:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 208:
                        case 209:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 104:
                        case 108:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 11:
                        case 139:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 19:
                        case 51:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 146:
                        case 178:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                break;
                            }
                        case 84:
                        case 85:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                break;
                            }
                        case 112:
                        case 113:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 200:
                        case 204:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 73:
                        case 77:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 42:
                        case 170:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 14:
                        case 142:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 67:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 70:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 28:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 152:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 194:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 98:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 56:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 25:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 26:
                        case 31:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 82:
                        case 214:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 88:
                        case 248:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 74:
                        case 107:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 27:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 86:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 216:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 106:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 30:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 210:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 120:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 75:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 29:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 198:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 184:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 99:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 57:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 71:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 156:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 226:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 60:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 195:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 102:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 153:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 58:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 83:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 92:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 202:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 78:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 154:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 114:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 89:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 90:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 55:
                        case 23:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 182:
                        case 150:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                break;
                            }
                        case 213:
                        case 212:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                break;
                            }
                        case 241:
                        case 240:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 236:
                        case 232:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 109:
                        case 105:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 171:
                        case 43:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 143:
                        case 15:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 124:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 203:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 62:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 211:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 118:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 217:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 110:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 155:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 188:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 185:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 61:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 157:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 103:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 227:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 230:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 199:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 220:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 158:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 234:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 242:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 59:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 121:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 87:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 79:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 122:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 94:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 218:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 91:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 229:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 167:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 173:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 181:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 186:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 115:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 93:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 206:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 205:
                        case 201:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 174:
                        case 46:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix6To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 179:
                        case 147:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix6To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 117:
                        case 116:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix6To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 189:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 231:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 126:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 219:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 125:
                            {
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 221:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e5, e1);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                break;
                            }
                        case 207:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix4To2To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 238:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e5);
                                }
                                break;
                            }
                        case 190:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e5, e7);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 187:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To3To3(e4, e3, e1);
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e3, e7);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 243:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix4To2To1(e4, e7, e3);
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To3To3(e4, e5, e7);
                                }
                                break;
                            }
                        case 119:
                            {
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix4To2To1(e4, e1, e3);
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To3To3(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 237:
                        case 233:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 175:
                        case 47:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                break;
                            }
                        case 183:
                        case 151:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 245:
                        case 244:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 250:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 123:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 95:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 222:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 252:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 249:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 235:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e2, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 111:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e5);
                                break;
                            }
                        case 63:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e8, e7);
                                break;
                            }
                        case 159:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 215:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e6, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 246:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e0, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 254:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e0);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 253:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e1);
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e1);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 251:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e2);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 239:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                dp[(i << 1) + 1 + (j << 1) * dpL] = Mix3To1(e4, e5);
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e5);
                                break;
                            }
                        case 127:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix2To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e7, e3);
                                }
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e8);
                                break;
                            }
                        case 191:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix3To1(e4, e7);
                                break;
                            }
                        case 223:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix2To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e6);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix2To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 247:
                            {
                                dp[(i << 1) + (j << 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix3To1(e4, e3);
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                        case 255:
                            {
                                if (Diff(e3, e1))
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + (j << 1) * dpL] = Mix14To1To1(e4, e3, e1);
                                }
                                if (Diff(e1, e5))
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + (j << 1) * dpL] = Mix14To1To1(e4, e1, e5);
                                }
                                if (Diff(e7, e3))
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e7, e3);
                                }
                                if (Diff(e5, e7))
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = e4;
                                }
                                else
                                {
                                    dp[(i << 1) + 1 + ((j << 1) + 1) * dpL] = Mix14To1To1(e4, e5, e7);
                                }
                                break;
                            }
                    }
                }
            });
        }



    }
}
