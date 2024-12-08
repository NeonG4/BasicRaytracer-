using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing
{
    public abstract class Material
    {
        public abstract bool Scatter(Ray rIn, HitRecord rec, Vec3 attenuation, Ray scattered);
    }
    public class Lambertian : Material
    {
        private Vec3 albedo;
        public Lambertian(Vec3 albedo)
        {
            this.albedo = albedo;            
        }
        override public bool Scatter(Ray r, HitRecord rec, Vec3 attenuation, Ray scattered)
        {
            Vec3 scatterDirection = rec.normal + Vec3.RandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = rec.normal;
            }

            scattered = new Ray(rec.p, scatterDirection);
            attenuation = albedo;
            return true;
        }
    }
}