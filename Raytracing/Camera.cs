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
        Pen render = new Pen(Color.FromArgb(0, 0, 0), 1);
        public int imageHeight;
        public int samplesPerPixel;
        int pixelSamplesScale;
        public int imageWidth;
        public double aspectRatio;
        double viewportHeight;
        double viewportWidth;
        double focalLength;
        public Vec3 center;
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
        private void Initialize()
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
        public void Render(Hittable world, PaintEventArgs e, int resolution)
        {
            Initialize();
            Ray r;
            Vec3 pixelColor;
            for (double j = 0; j < imageHeight; j++)
            {
                for (double i = 0; i < imageWidth; i++)
                {
                    pixelColor = new Vec3(0, 0, 0);
                    for (int sample = 0; sample < samplesPerPixel; sample++)
                    {
                        r = GetRay(i, j);
                        pixelColor += RayColor(r, maxDepth, world); 
                    }
                    DrawColor(pixelColor / samplesPerPixel, i, j, e, resolution);
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
            if (world.Hit(r, new Interval(0.001, INFINITY), out rec))
            {
                Ray scattered;
                Vec3 attenuation = new Vec3(0, 0, 0);
                if (rec.mat.Scatter(r, rec, out attenuation, out scattered))
                {
                    return attenuation * RayColor(scattered, depth - 1, world);
                }
                return new Vec3(0, 0, 0);
            }
            Vec3 unitDirection = Vec3.Unit(r.direction);
            double a = 0.5 * (unitDirection.y + 1);
            return (1 - a) * new Vec3(1, 1, 1) + a * new Vec3(0.5, 0.7, 1);
        }
        private void DrawColor(Vec3 color, double x, double y, PaintEventArgs e, int resolution)
        {
            Interval intensity = new Interval(0, 0.999);
            Rectangle rect = new Rectangle((int)x * resolution, (int)y * resolution, resolution + 1, resolution + 1);
            double r = color.x;
            double g = color.y;
            double b = color.z;
            r = Util.LinearToGamma(r);
            b = Util.LinearToGamma(b);
            g = Util.LinearToGamma(g);
            int ra = (int) (256 * intensity.Clamp(r));
            int ga = (int) (256 * intensity.Clamp(g));
            int ba = (int) (256 * intensity.Clamp(b));
            render.Color = Color.FromArgb(ra, ga, ba);
            e.Graphics.DrawRectangle(render, rect);
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
