using System.CodeDom;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace Raytracing
{   
    public class HitRecord
    {
        public Material mat;
        public Vec3 p; 
        public Vec3 normal;
        public double t;
        public double u;
        public double v;
        public bool frontFace;
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

        public AABB BoundingBox;
    }
    // sphere hittable class
    public class Sphere : Hittable
    {
        Material mat;
        public double radius;
        private Ray center;
        public AABB bbox;
        // stationary sphere
        public Sphere(Vec3 staticCenter, double radius, Material mat)
        {
            this.center = new Ray(staticCenter, new Vec3(0, 0, 0));
            this.mat = mat;
            this.radius = radius;
            if (radius < 0) // check code for circles with radius of less than zero
            {
                this.radius = 0;
            }
            Vec3 rvec = new Vec3(radius, radius, radius);
            AABB box1 = new AABB(center.at(0) - rvec, center.at(0) + rvec);
            AABB box2 = new AABB(center.at(1) - rvec, center.at(1) + rvec);
            this.bbox = new AABB(box1, box2);
            BoundingBox = bbox;
        }
        // moving sphere
        public Sphere(Vec3 center1, Vec3 center2, double radius, Material mat)
        {
            this.center = new Ray(center1, center2 - center1);
            this.mat = mat;
            this.radius = radius;
            if (radius < 0) // check code for circles with radius of less than zero
            {
                this.radius = 0;
            }
            Vec3 rvec = new Vec3(radius, radius, radius);
            AABB box1 = new AABB(center.at(0) - rvec, center.at(0) + rvec);
            AABB box2 = new AABB(center.at(1) - rvec, center.at(1) + rvec);
            this.bbox = new AABB(box1, box2);
            BoundingBox = bbox;
        }
        override public bool Hit(Ray r, Interval rayT, out HitRecord rec)
        {
            Vec3 currentCenter = center.at(r.time);
            Vec3 oc = currentCenter - r.point;
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
            Vec3 outwardNormal = (rec.p - currentCenter) / radius;
            rec.SetFaceNormal(r, outwardNormal);
            getSphereUV(outwardNormal, out rec.u, out rec.v);
            rec.mat = mat;
            return true;
        }
        static void getSphereUV(Vec3 p, out double u, out double v)
        {
            double theta = Math.Acos(-p.y);
            double phi = Math.Atan2(-p.z, p.x) + Math.PI;

            u = phi / (2 * Math.PI);
            v = theta / Math.PI;
        }
    }
    public class Quad : Hittable
    {
        private Vec3 q;
        private Vec3 u, v;
        private Material mat;
        private AABB bbox;
        private Vec3 normal;
        private Vec3 w;
        private double d;
        public Quad(Vec3 q, Vec3 u, Vec3 v, Material mat)
        {
            this.q = q;
            this.u = u;
            this.v = v;
            this.mat = mat;

            Vec3 n = Vec3.Cross(u, v);
            normal = Vec3.Unit(n);
            d = Vec3.Dot(normal, q);
            w = n / Vec3.Dot(n, n);


            this.SetBoundingBox();
            BoundingBox = bbox;
        }
        public void SetBoundingBox()
        {
            AABB bboxDiagonal1 = new AABB(q, q + u + v);
            AABB bboxDiagonal2 = new AABB(q + u, q + v);
            this.bbox = new AABB(bboxDiagonal1, bboxDiagonal2);
        }
        public override bool Hit(Ray r, Interval rayT, out HitRecord rec)
        {
         
            rec = new HitRecord();
            double denom = Vec3.Dot(normal, r.direction);

            if(Math.Abs(denom) < 1e-8)
            {
                return false;
            }
            double t = (d - Vec3.Dot(normal, r.point)) / denom;
            if (!rayT.Contains(t))
            {
                return false;
            }
            Vec3 intersection = r.at(t);
            Vec3 planarHitPtVector = intersection - q;
            double alpha = Vec3.Dot(w, Vec3.Cross(planarHitPtVector, v));
            double beta = Vec3.Dot(w, Vec3.Cross(u, planarHitPtVector));

            if (!IsInterior(alpha, beta, out rec))
            {
                return false;
            }


            rec.t = t;
            rec.p = intersection;
            rec.mat = mat;
            rec.SetFaceNormal(r, normal);
            return true;
        }
        public bool IsInterior(double a, double b, out HitRecord rec)
        {
            rec = new HitRecord();
            Interval unitInterval = new Interval(0, 1);
            if (!unitInterval.Contains(a) || !unitInterval.Contains(b))
            {
                return false;
            }
            rec.u = a;
            rec.v = b;
            return true;
        }

    }
    public class HittableList : Hittable
    {
        public List<Hittable> objects = new List<Hittable>();
        private AABB bbox = new AABB(new Interval(0, 1), new Interval(0, 1), new Interval(0, 1));

        public void Clear()
        {
            objects.Clear();
        }
        public void Add(Hittable obj)
        {
            objects.Add(obj);
            bbox = new AABB(bbox, obj.BoundingBox);
            bbox = new AABB(bbox, obj.BoundingBox);
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
