//Downloaded from
//Visual C# Kicks - http://www.vcskicks.com/
using System;
using System.Drawing;
using System.Drawing.Imaging;

//add SetPixelByGray method by erspicu_brox ;
//¦³¨Ç­×§ï 
namespace BitmapProcessingScalex
{
    unsafe public struct FastBitmap
    {

        private Bitmap workingBitmap;// = null;
        private int width;//= 0;
        private BitmapData bitmapData;// = null;
        private Byte* pBase;//= null;
        private PixelData* pixelData;// = null;

        public FastBitmap(ref Bitmap inputBitmap)
        {

            workingBitmap = null;
            width = 0;
            bitmapData  = null;
            pBase = null;
            pixelData  = null; 

            workingBitmap = inputBitmap;
        }

        private struct PixelData
        {
            public byte blue;
            public byte green;
            public byte red;
            public byte alpha;

        }

        private struct PixelDataInt
        {
            public uint ColorValue;
        }

        public void LockImage()
        {
            Rectangle bounds = new Rectangle(Point.Empty, workingBitmap.Size);
            width = (int)(bounds.Width * sizeof(PixelData));
            if (width % 4 != 0) width = 4 * (width / 4 + 1);
            bitmapData = workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }

        public void SetPixelByRGBValue(int x, int y, uint v)
        {
            try
            {
                PixelDataInt* data = (PixelDataInt*)(pBase + y * width + x * sizeof(PixelDataInt));
                data->ColorValue = v;
            }
            catch
            {
            }
        }

        public void SetPixelByGray(int x, int y, byte v)
        {
            try
            {
                PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
                data->alpha = 255;
                data->red = data->green = data->blue = v;
            }
            catch
            {
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            data->alpha = color.A;
            data->red = color.R;
            data->green = color.G;
            data->blue = color.B;
        }

        public Color GetPixel(int x, int y)
        {
            PixelDataInt* data = (PixelDataInt*)(pBase + y * width + x * sizeof(PixelData));
            return Color.FromArgb((int)data->ColorValue);
        }


        public void UnlockImage()
        {
            workingBitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }
    }
}
