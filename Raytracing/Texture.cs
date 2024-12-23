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
        private Vec3 Albedo;
        public SolidColor(Vec3 albedo)
        {
            this.Albedo = albedo;
        }
        public SolidColor(double red, double green, double blue)
        {
            this.Albedo = new Vec3(red, green, blue);
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            return Albedo;
        }
    }
    public class CheckerTexture : Texture
    {
        private double invScale;
        private Texture even = new SolidColor(new Vec3(0, 0, 0));
        private Texture odd = new SolidColor(new Vec3(1, 1, 1));
        public CheckerTexture(double scale, Texture even, Texture odd)
        {
            this.invScale = 1 / scale;
            this.even = even;
            this.odd = odd;
        }
        public CheckerTexture(double scale, Vec3 c1, Vec3 c2)
        {
            new CheckerTexture(scale, new SolidColor(c1), new SolidColor(c2));
        }
        public override Vec3 Value(double u, double v, Vec3 p)
        {
            int xInterger = (int)(Math.Floor(invScale * p.x));
            int yInterger = (int)(Math.Floor(invScale * p.y)); 
            int zInterger = (int)(Math.Floor(invScale * p.z));
            
            bool isEven = (xInterger + yInterger + zInterger) % 2 == 0;

            return isEven ? this.even.Value(u, v, p) : this.odd.Value(u, v, p);
        }
    }
}
