using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Raytracing
{
    internal class Camera
    {

        const double INFINITY = double.PositiveInfinity;
        const double PI = Math.PI;
        public int imageHeight;
        public int samplesPerPixel;
        int pixelSamplesScale;
        public int imageWidth;
        public double aspectRatio;
        double viewportHeight;
        double viewportWidth;
        double focalLength;
        public Vec3 center;
        public Vec3 background;
        int maxDepth;
        Vec3 viewportU;
        Vec3 viewportV;
        Vec3 pixelDeltaU;
        Vec3 pixelDeltaV;
        Vec3 viewportUpperLeft;
        public Vec3 pixel00Loc;
        public double vfov = 90; // Vertical FOV
        public Vec3 lookFrom = new Vec3(0, 0, 0);
        public Vec3 lookAt = new Vec3(0, 0, -1);
        public Vec3 vup = new Vec3(0, 1, 0);
        Vec3 u, v, w;
        double theta;
        public double defocusAngle = 0;
        public double focusDist = 10;
        Vec3 defocusDiskU;
        Vec3 defocusDiskV;
        public void Initialize()
        {
            imageHeight = (int)(imageWidth / aspectRatio);
            if (imageHeight < 1) { imageHeight = 1; }

            center = lookFrom;

            theta = Util.DegreesToRadians(vfov);
            double h = Math.Tan(theta / 2);
            viewportHeight = 2 * h * focusDist;
            viewportWidth = viewportHeight * ((double) imageWidth / imageHeight);

            w = Vec3.Unit(lookFrom - lookAt);
            u = Vec3.Unit(Vec3.Cross(vup, w));
            v = Vec3.Cross(w, u);

            viewportU = viewportWidth * u;
            viewportV = viewportHeight * (0 - v);
            
            pixelDeltaU = viewportU / imageWidth;
            pixelDeltaV = viewportV / imageHeight;
            
            viewportUpperLeft = center - (focusDist * w) - viewportU / 2 - viewportV / 2;
            pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV);

            double defocusRadius = focusDist * Math.Tan(Util.DegreesToRadians(defocusAngle / 2));
            defocusDiskU = u * defocusRadius;
            defocusDiskV = v * defocusRadius;
            pixelSamplesScale = 1 / samplesPerPixel;
            maxDepth = 50;
        }
        public void Render(Hittable world, Bitmap targetBitmap, int resolution, 
            int section, int numSections)
        {
            Ray r;
            Vec3 pixelColor;

            int x1 = 0;
            int y1 = imageHeight * section / numSections;
            int x2 = imageWidth;
            int y2 = imageHeight * (section + 1) / numSections;

            Pen renderPen = new Pen(Color.FromArgb(0, 0, 0), 1);

            for (int j = y1; j < y2; j++)
            {
                for (int i = x1; i < x2; i++)
                {
                    pixelColor = new Vec3(0, 0, 0);
                    for (int sample = 0; sample < samplesPerPixel; sample++)
                    {
                        r = GetRay(i, j);
                        pixelColor += RayColor(r, maxDepth, world); 
                    }
                    DrawColor(pixelColor / samplesPerPixel, i, j, targetBitmap, renderPen, resolution);
                }
            }
        }
        private Vec3 RayColor(Ray r, int depth, Hittable world)
        {
            if (depth <= 0)
            {
                return new Vec3(0, 0, 0);
            }
            HitRecord rec;

            if (!world.Hit(r, new Interval(0.001, INFINITY), out rec))
            {
                return this.background;
            }
            
            Ray scattered;
            Vec3 attenuation;
            Vec3 colorFromEmission = rec.mat.ColorEmitted(rec.u, rec.v, rec.p);
            
            if (!rec.mat.Scatter(r, rec, out attenuation, out scattered))
            {
                return colorFromEmission;
            }
            Vec3 colorFromScatter = attenuation * RayColor(scattered, depth-1, world);

            return colorFromEmission + colorFromScatter;
        }
        private void DrawColor(Vec3 color, int x, int y, Bitmap targetBitmap,
            Pen renderPen, int resolution)
        {
            Interval intensity = new Interval(0, 0.999);
            double r = color.x;
            double g = color.y;
            double b = color.z;
            r = Util.LinearToGamma(r);
            b = Util.LinearToGamma(b);
            g = Util.LinearToGamma(g);
            int ra = (int) (256 * intensity.Clamp(r));
            int ga = (int) (256 * intensity.Clamp(g));
            int ba = (int) (256 * intensity.Clamp(b));

            lock (targetBitmap)
            {
                var pixelColor = Color.FromArgb(ra, ga, ba);

                for (int px = 0; px < resolution; px++)
                {
                    for (int py = 0; py < resolution; py++)
                    {
                        targetBitmap.SetPixel(x + px, y + py, pixelColor);
                    }
                }
            }
        }
        public Ray GetRay(double x, double y)
        {
            Vec3 offset = Ray.SampleSquare();
            Vec3 pixelSample = pixel00Loc + ((x + offset.x) * pixelDeltaU) + ((y + offset.y) * pixelDeltaV);
            Vec3 rayOrigin = (defocusAngle <= 0) ? center : DefocusDiskSample();
            Vec3 rayDirection = pixelSample - rayOrigin;
            double rayTime = Util.RandomDouble();
            return new Ray(rayOrigin, rayDirection, rayTime);
        }
        private Vec3 DefocusDiskSample()
        {
            Vec3 p = Vec3.RandomInUnitDisk();
            return center + (p.x * defocusDiskU) + (p.y * defocusDiskV);
        }

    }
}
