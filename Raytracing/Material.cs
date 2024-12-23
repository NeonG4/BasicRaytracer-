using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing
{
    public abstract class Material
    {
        public abstract bool Scatter(Ray rIn, HitRecord rec, out Vec3 attenuation, out Ray scattered);
    }
    public class Lambertian : Material // solid color
    {
        private Vec3 albedo;
        private Texture tex;
        public Lambertian(Vec3 albedo)
        {
            tex = new SolidColor(albedo);
            this.albedo = albedo;            
        }
        public Lambertian(Texture tex)
        {
            this.tex = tex;
        }
        override public bool Scatter(Ray rIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
        {
            Vec3 scatterDirection = rec.normal + Vec3.RandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = rec.normal;
            }

            scattered = new Ray(rec.p, scatterDirection, rIn.time);
            attenuation = tex.Value(rec.u, rec.v, rec.p);
            return true;
        }
    }
    public class Metal : Material // reflective
    {
        Vec3 albedo;
        double fuzz;
        public Metal(Vec3 albedo, double fuzz)
        {
            this.albedo = albedo;
            if (fuzz < 1) { this.fuzz = fuzz; }
            else { this.fuzz = 1; }
        }
        override public bool Scatter(Ray rIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
        {
            Vec3 reflected = Vec3.Reflect(rIn.direction, rec.normal);
            reflected = Vec3.Unit(reflected) + (fuzz * Vec3.RandomUnitVector());
            scattered = new Ray(rec.p, reflected, rIn.time);
            attenuation = this.albedo;
            return Vec3.Dot(scattered.direction, rec.normal) > 0;
        }
    }
    public class Dielectric : Material
    {
        double refractionIndex;
        public Dielectric(double refractionIndex)
        {
            this.refractionIndex = refractionIndex;
        }
        override public bool Scatter(Ray rIn, HitRecord rec, out Vec3 attenuation, out Ray scattered)
        {
            attenuation = new Vec3(1, 1, 1);
            double ri = rec.frontFace ? (1 / refractionIndex) : refractionIndex;

            Vec3 unitDirection = Vec3.Unit(rIn.direction);
            double cosTheta = Math.Min(Vec3.Dot(0 - unitDirection, rec.normal), 1);
            double sinTheta = Math.Sqrt(1 - cosTheta * cosTheta);

            bool cannnotRefract = ri * sinTheta > 1;
            Vec3 direction;

            if (cannnotRefract || Reflectance(cosTheta, ri) > Util.RandomDouble())
            {
                direction = Vec3.Reflect(unitDirection, rec.normal);
            }
            else
            {
                direction = Vec3.Refract(unitDirection, rec.normal, ri);
            }

            scattered = new Ray(rec.p, direction, rIn.time);
            return true;
        }
        private static double Reflectance(double cosine, double refractionIndex)
        {
            double r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow((1 - cosine), 5);
        }
    }
}