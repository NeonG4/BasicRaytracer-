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
        private void Initialize()
        {
            imageHeight = (int)(imageWidth / aspectRatio);
            if (imageHeight < 1) { imageHeight = 1; }

            center = new Vec3(0, 0, 0);
            
            focalLength = 1;
            viewportHeight = 2;
            viewportWidth = viewportHeight * (imageWidth / imageHeight);
            
            viewportU = new Vec3(viewportWidth, 0, 0);
            viewportV = new Vec3(0, -viewportHeight, 0);
            
            pixelDeltaU = viewportU / imageWidth;
            pixelDeltaV = viewportV / imageHeight;
            
            viewportUpperLeft = center - new Vec3(0, 0, focalLength) - viewportU / 2 - viewportV / 2;
            pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV);
            samplesPerPixel = 100;
            pixelSamplesScale = 1 / samplesPerPixel;
            maxDepth = 10;
        }
        public void Render(Hittable world, PaintEventArgs e)
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
                    DrawColor(pixelColor / samplesPerPixel, i, j, e);
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
                Vec3 direction = rec.normal + Vec3.RandomUnitVector();
                return 0.5 * RayColor(new Ray(rec.p, direction), depth - 1, world);
            }
            Vec3 unitDirection = Vec3.Unit(r.direction);
            double a = 0.5 * (unitDirection.y + 1);
            return (1 - a) * new Vec3(1, 1, 1) + a * new Vec3(0.5, 0.7, 1);
        }
        private void DrawColor(Vec3 color, double x, double y, PaintEventArgs e)
        {
            Interval intensity = new Interval(0, 0.999);
            Rectangle rect = new Rectangle((int)x, (int)y, 1, 1);
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
            Vec3 offset = Ray.sampleSquare();
            Vec3 pixelSample = pixel00Loc + ((x + offset.x) * pixelDeltaU) + ((y + offset.y) * pixelDeltaV);
            Vec3 rayOrigin = center;
            Vec3 rayDirection = pixelSample - rayOrigin;
            return new Ray(rayOrigin, rayDirection);
        }
    }
}
