using System.Diagnostics;
using System.Reflection;

namespace Raytracing
{   
    public class HitRecord
    {
        public Material mat;
        public Vec3 p; 
        public Vec3 normal;
        public double t;
        bool frontFace;
        public void SetFaceNormal(Ray r, Vec3 outwardNormal) // outwardNormal has unit length
        {
            frontFace = Vec3.Dot(r.direction, outwardNormal) < 0;
            if (frontFace)
            {
                normal = outwardNormal;
            }
            else
            {
                normal = 0 - outwardNormal;
            }
        }
    }
    // class for all hitable objects
    public abstract class Hittable
    {
        public abstract bool Hit(Ray r, Interval rayT, out HitRecord rec);
    }
    public class Sphere : Hittable
    {
        Material mat;
        public Vec3 center;
        public double radius;
        public Sphere(Vec3 center, double radius)
        {
            this.center = center;
            this.radius = radius;
            if (radius < 0) // check code for circles with radius of less than zero
            {
                this.radius = 0;
            }
        }
        override public bool Hit(Ray r, Interval rayT, out HitRecord rec)
        {
            Vec3 oc = this.center - r.point;
            double a = r.direction.LengthSquared();
            double h = Vec3.Dot(r.direction, oc);
            double c = oc.LengthSquared() - radius * radius;
            double discriminant = h * h - a * c;
            if (discriminant < 0)
            {
                rec = null;
                return false;
            }
            double sqrtd = Math.Sqrt(discriminant);

            // find nearest root
            double root = (h - sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                root = (h + sqrtd) / a;
                if (!rayT.Surrounds(root))
                {
                    rec = null;
                    return false;
                }
            }
            rec = new HitRecord();
            rec.t = root;
            rec.p = r.at(rec.t);
            rec.mat = mat;
            Vec3 outwardNormal = (rec.p - center) / radius;
            rec.SetFaceNormal(r, outwardNormal);
            return true;
        }
    }
    public class HittableList : Hittable
    {
        public List<Hittable> objects = new List<Hittable>();


        public void Clear()
        {
            objects.Clear();
        }
        public void Add(Hittable obj)
        {
            objects.Add(obj);
        }
        override public bool Hit(Ray r, Interval rayT, out HitRecord rec)
        {
            HitRecord tempRec = new HitRecord();
            HitRecord recIn;
            bool hitAnything = false;
            double closestSoFar = rayT.max;
            foreach(Hittable obj in objects)
            {
                if (obj.Hit(r, new Interval(rayT.min, closestSoFar), out recIn))
                {
                    hitAnything = true;
                    closestSoFar = recIn.t;
                    tempRec = recIn;
                }
            }
            rec = tempRec;
            return hitAnything;

        }
    }
}
