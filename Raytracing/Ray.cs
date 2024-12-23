using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing
{
    public class Ray
    {
        public Vec3 point;
        public Vec3 direction;
        public double time;
        public Ray(Vec3 point, Vec3 direction, double time)
        {
            this.point = point;
            this.direction = direction;
            this.time = time;
        }
        public Ray(Vec3 point, Vec3 direction)
        {
            this.point = point;
            this.direction = direction;
            this.time = 0;
        }
        public Vec3 at(double t)
        {
            return this.point + this.direction * t;
        }
        public static Vec3 SampleSquare()
        {
            return new Vec3(Util.RandomDouble() - 0.5, Util.RandomDouble() - 0.5, 0);
        }
    }
}
