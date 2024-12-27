using System.Windows.Forms.Design;
using Efundies;
using Microsoft.VisualBasic;

namespace Raytracing
{
    public class RTWImage
    {
        public int imageWidth;
        public int imageHeight;
        Bitmap image;
        LockBitmap locked;
        public RTWImage(string imageName)
        {
            this.image = (Bitmap) Image.FromFile(@"c:\temp\" + imageName);
            this.locked = new LockBitmap(image);
            this.imageWidth = image.Width;
            this.imageHeight = image.Height;
            locked.LockBits();

        }
        public Vec3 PixelData(int x, int y)
        {
            Vec3 magenta = new Vec3(255, 0, 255);
            if (image == null)
            {
                return magenta;
            }
            x = Clamp(x, 0, this.imageWidth);
            y = Clamp(y, 0, this.imageHeight);

            Color imgColor = locked.GetPixel(x, y);
            return new Vec3(imgColor.R, imgColor.G, imgColor.B);
        }
        private static int Clamp(int x, int low, int high)
        {
            if (x < low) return low;
            if (x < high) return x;
            return high - 1;
        }

    }
}