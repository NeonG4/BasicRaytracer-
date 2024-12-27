using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing
{
    public abstract class Texture
    {
        public abstract Vec3 Value(double u, double v, Vec3 p);
    }
    public class SolidColor : Texture
    {
        private Vec3 albedo;
        public SolidColor(Vec3 albedo)
        {
            this.albedo = albedo;
        }
        public SolidColor(double red, double green, double blue)
        {
            this.albedo = new Vec3(red, green, blue);
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return albedo;
        }
    }
    public class CheckerTexture : Texture
    {
        private double invScale;
        Texture even = new SolidColor(new Vec3(0, 0, 0));
        Texture odd = new SolidColor(new Vec3(1, 1, 1));
        public CheckerTexture(double scale, Texture evenIn, Texture oddIn)
        {
            this.invScale = 1 / scale;
            this.even = evenIn;
            this.odd = oddIn;
        }
        public CheckerTexture(double scale, Vec3 c1, Vec3 c2)
        {
            this.invScale = 1 / scale;
            this.even = new SolidColor(c1);
            this.odd = new SolidColor(c2);
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            int xInterger = (int)(Math.Floor(invScale * p.x));
            int yInterger = (int)(Math.Floor(invScale * p.y)); 
            int zInterger = (int)(Math.Floor(invScale * p.z));
            
            bool isEven = (xInterger + yInterger + zInterger) % 2 == 0;

            return isEven ? this.even.Value(u, v, p) : this.odd.Value(u, v, p); // this.even and this.odd are default values
        }
    }
    public class ImageTexture : Texture
    {
        RTWImage imageTexture;
        public ImageTexture(string fileName)
        {
            imageTexture = new RTWImage(fileName);
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            if (imageTexture.imageHeight <= 0) return new Vec3(0, 1, 1);
            u = new Interval(0, 1).Clamp(u);
            v = 1.0 - new Interval(0, 1).Clamp(v);

            int i = (int) (u * imageTexture.imageWidth);
            int j = (int) (v * imageTexture.imageHeight);
            var pixel = imageTexture.PixelData(i, j);

            double colorScale = 1.0 / 255.0;
            return colorScale * new Vec3(pixel.x, pixel.y, pixel.z);
        }
    }
}
