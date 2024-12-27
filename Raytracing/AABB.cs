using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xaml.Permissions;

namespace Raytracing
{
    public class AABB
    {
        public Interval x, y, z;
        public AABB(Interval x, Interval y, Interval z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            PadToMinimums();
        }
        public AABB(Vec3 a, Vec3 b)
        {
            x = (a.x <= b.x) ? new Interval(a.x, b.x) : new Interval(b.x, a.x);
            y = (a.y <= b.y) ? new Interval(a.y, b.y) : new Interval(b.y, a.y);
            z = (a.z <= b.z) ? new Interval(a.z, b.z) : new Interval(b.z, a.z);

            PadToMinimums();
        }
        public AABB(AABB box0, AABB box1)
        {
            if (box0 == null)
            {
                box0 = new AABB(new Vec3(0, 0, 0), new Vec3(1, 1, 1));
            }
            if (box1 == null)
            {
                box1 = new AABB(new Vec3(0, 0, 0), new Vec3(1, 1, 1));
            }
            this.x = new Interval(box0.x, box1.x);
            this.y = new Interval(box0.y, box1.y);
            this.z = new Interval(box0.z, box1.z);
        }
        public Interval AxisInterval(int n)
        {
            if (n == 1) return y;
            if (n == 2) return z;
            return x;
        }
        public int LongestAxis()
        {
            if (x.Size() > y.Size())
                return x.Size() > z.Size() ? 0 : 2;
            else
                return y.Size() > z.Size() ? 1 : 2;
        }
        public bool Hit(Ray r, Interval rayT)
        {
            Vec3 rayOrig = r.point;
            Vec3 rayDir = r.direction;
            for (int axis = 0; axis < 3; axis++)
            {
                Interval ax = AxisInterval(axis);
                double adinv = 1 / rayDir.e[axis];

                var t0 = (ax.min - rayOrig.e[axis]) * adinv;
                var t1 = (ax.max - rayOrig.e[axis]) * adinv;

                if (t0 < t1)
                {
                    if (t0 > rayT.min) rayT.min = t0;
                    if (t1 < rayT.max) rayT.max = t1;
                }
                else
                {
                    if (t1 > rayT.min) rayT.min = t1;
                    if (t0 < rayT.max) rayT.max = t0;
                }
                if (rayT.max < rayT.min)
                {
                    return false;
                }    
            }
            return true;
        }
        private void PadToMinimums()
        {
            double delta = 0.0001;
            if (x.Size() < delta) this.x = x.Expand(delta);
            if (y.Size() < delta) this.y = y.Expand(delta);
            if (z.Size() < delta) this.z = z.Expand(delta);
        }
    }
    public class BVHNode : Hittable
    {
        private Hittable left;
        private Hittable right;
        private AABB bbox;
        public BVHNode(HittableList list) 
        {
            new BVHNode(list.objects, 0, list.objects.Count);
        }
        public BVHNode(List<Hittable> objects, int start, int end)
        {
            bbox = new AABB(new Vec3(0, 0, 0), new Vec3(0, 0, 0));
            for (int objectIndex = start; objectIndex < end; objectIndex++)
            {
                bbox = new AABB(bbox, objects[objectIndex].BoundingBox);
            }
            int axis = bbox.LongestAxis();
            
            if (axis == 0)
            {
                var comparator = boxXCompare;
            }
            else if (axis == 1)
            {
                var comparator = boxYCompare;
            }
            else
            {
                var comparator = boxZCompare;
            }
            
            int objectSpan = end - start;

            if (objectSpan == 1)
            {
                left = right = objects[start];
            }
            else if (objectSpan == 2)
            {
                left = objects[start];
                right = objects[start + 1];
            }
            else
            {
                var mid = start + objectSpan / 2;
                left = new BVHNode(objects, start, mid);
                right = new BVHNode(objects, mid, end);
            }
        }
        public override bool Hit(Ray r, Interval rayT, out HitRecord rec)
        {
            if (!bbox.Hit(r, rayT))
            {
                rec = null;
                return false;
            }
            bool hitLeft = left.Hit(r, rayT, out rec);
            bool hitRight = right.Hit(r, rayT, out rec);
            return hitLeft || hitRight;
        }
        private static bool BoxCompare(Hittable a, Hittable b, int axisIndex)
        {
            var aAxisInterval = a.BoundingBox.AxisInterval(axisIndex);
            var bAxisInterval = b.BoundingBox.AxisInterval(axisIndex);
            return aAxisInterval.min < bAxisInterval.min;
        }
        private static bool boxXCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 0);
        }
        private static bool boxYCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 1);
        }
        private static bool boxZCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 2);
        }
    }
}
