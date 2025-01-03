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
    public class Util
    { 
        static Random rand = new Random();
        public static double DegreesToRadians(double degrees)
        {
            return ((degrees *  Math.PI) / 180);
        }
        public static double RandomDouble()
        {
            return rand.NextDouble();
        }
        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble(); 
        }
        public static Vec3 Random()
        {
            return new Vec3(RandomDouble(), RandomDouble(), RandomDouble());
        }
        public static Vec3 Random(double min, double max)
        {
            return new Vec3(RandomDouble(min, max), RandomDouble(min, max), RandomDouble(min, max));
        }
        public static double LinearToGamma(double linearComponent)
        {
            if (linearComponent > 0)
            {
                return Math.Sqrt(linearComponent);
            }
            return 0;
        }
        public static int RandomInt(int min, int max)
        {
            return (int)Util.RandomDouble(min, max);
        }
    }
}