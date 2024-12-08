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
        public static double DegreesToRadians(double degrees, double radians)
        {
            return degrees *  Raytracer.PI / 180;
        }
        public static double RandomDouble()
        {
            return rand.Next(0, 10000000) / 10000000;
        }
        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble(); 
        }
    }
}