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
        public Ray(Vec3 point, Vec3 direction)
        {
            this.point = point;
            this.direction = direction;
        }
        public Vec3 at(double t)
        {
            return this.point + this.direction * t;
        }
    }
}
