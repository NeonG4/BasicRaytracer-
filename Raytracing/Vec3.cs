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
    public class Vec3
    {
        // Vector class
        public double x, y, z;
        public double[] e;
        public Vec3(double xi, double yi, double zi)
        {
            x = xi;
            y = yi;
            z = zi;
            e = [x, y, z];
        }
        // addition operator overloads
        public static Vec3 operator +(Vec3 vec1, Vec3 vec2)
        {
            return new Vec3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
        }
        public static Vec3 operator +(Vec3 vec1, double t)
        {
            return new Vec3(vec1.x + t, vec1.y + t, vec1.z + t);
        }
        public static Vec3 operator +(double t, Vec3 vec1)
        {
            return new Vec3(vec1.x + t, vec1.y + t, vec1.z + t);
        }
        // subtraction operator overloads
        public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
        {
            return new Vec3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }
        public static Vec3 operator -(Vec3 vec1, double t)
        {
            return new Vec3(vec1.x - t, vec1.y - t, vec1.z - t);
        }
        public static Vec3 operator -(double t, Vec3 vec1)
        {
            return new Vec3(t - vec1.x, t - vec1.y, t - vec1.z);
        }
        // multiplication operator overloads
        public static Vec3 operator *(Vec3 vec1, double t)
        {
            return new Vec3(vec1.x * t, vec1.y * t, vec1.z * t);
        }
        public static Vec3 operator *(double t, Vec3 vec1)
        {
            return new Vec3(vec1.x * t, vec1.y * t, vec1.z * t);
        }
        // division operator overloads
        public static Vec3 operator /(Vec3 vec1, double t)
        {
            return new Vec3(vec1.x / t, vec1.y / t, vec1.z / t);
        }
        public static Vec3 operator /(double t, Vec3 vec1)
        {
            return new Vec3(t / vec1.x, t / vec1.y, vec1.z / t);
        }
        // dot product
        public static double Dot(Vec3 vec1, Vec3 vec2)
        {
            return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
        }
        public static Vec3 operator *(Vec3 vec1, Vec3 vec2)
        {
            return new Vec3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
        }
        // length
        public double LengthSquared()
        {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }
        public double Length()
        {
            return Math.Sqrt(this.LengthSquared());
        }
        // unit vector
        public static Vec3 Unit(Vec3 vec1)
        {
            return vec1 / vec1.Length();
        }
        // cross vector
        public static Vec3 Cross(Vec3 vec1, Vec3 vec2)
        {
            return new Vec3(vec1.y * vec2.z - vec1.z * vec2.y,
                            vec1.z * vec2.x - vec1.x * vec2.z,
                            vec1.x * vec2.y - vec1.y * vec2.x);
        }
        public static Vec3 RandomUnitVector()
        {
            Vec3 p = Util.Random(-1, 1);
            double lensq = p.LengthSquared();
            return p / Math.Sqrt(lensq);
        }
        public static Vec3 RandomOnHemisphere(Vec3 normal)
        {
            Vec3 onUnitSphere = RandomUnitVector();
            if (Vec3.Dot(onUnitSphere, normal) > 0)
            {
                return onUnitSphere;
            }
            else
            {
                return 0 - onUnitSphere;
            }
        }
        public bool NearZero()
        {
            var s = 1e-8;
            return (Math.Abs(this.x) < s) && (Math.Abs(this.y) < s) && (Math.Abs(this.z) < s);
        }
        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * Vec3.Dot(v, n) * n;
        }
        public static Vec3 Refract(Vec3 uv, Vec3 n, double etaiOverEtat)
        {
            double cosTheta = Math.Min(Vec3.Dot(0 - uv, n), 1);
            Vec3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
            Vec3 rOutParellel = 0 - Math.Sqrt(Math.Abs(1 - rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParellel;
        }
        public static Vec3 RandomInUnitDisk()
        {
            Vec3 p;
            while (true)
            {
                p = new Vec3(Util.RandomDouble(), Util.RandomDouble(), 0);
                if (p.LengthSquared() < 1)
                { }
                    return p;
                
            }
        }
    }
}
