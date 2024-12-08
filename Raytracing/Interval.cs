using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Raytracing
{
    public class Interval
    {
        public double min;
        public double max;
        public Interval(double min, double max)
        {
            this.min = min;
            this.max = max;
        }
        public double Size()
        {
            return max - min;
        }
        public bool Contains(double t)
        {
            return min <= t && t <= max;
        }
        public bool Surrounds(double t)
        {
            return min < t && t < max;
        }
        public double Clamp(double t)
        {
            if (t < this.min) return min;
            if (t > this.max) return max;
            return t;
        }
        static Interval empty = new Interval(double.PositiveInfinity, double.NegativeInfinity);
        static Interval universe = new Interval(double.NegativeInfinity, double.PositiveInfinity);
    }
    
}
