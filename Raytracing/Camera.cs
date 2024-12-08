using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
        public int imageWidth;
        public double aspectRatio;
        double viewportHeight;
        double viewportWidth;
        double focalLength;
        Vec3 cameraCenter;
        Vec3 viewportU;
        Vec3 viewportV;
        Vec3 pixelDeltaU;
        Vec3 pixelDeltaV;
        Vec3 viewportUpperLeft;
        Vec3 pixel00Loc;
        private void Initialize()
        {
            imageHeight = (int)(imageWidth / aspectRatio);
            if (imageHeight < 1) { imageHeight = 1; }

            cameraCenter = new Vec3(0, 0, 0);
            
            focalLength = 1;
            viewportHeight = 2;
            viewportWidth = viewportHeight * (imageWidth / imageHeight);
            
            viewportU = new Vec3(viewportWidth, 0, 0);
            viewportV = new Vec3(0, -viewportHeight, 0);
            
            pixelDeltaU = viewportU / imageWidth;
            pixelDeltaV = viewportV / imageHeight;
            
            viewportUpperLeft = cameraCenter - new Vec3(0, 0, focalLength) - viewportU / 2 - viewportV / 2;
            pixel00Loc = viewportUpperLeft + 0.5 * (pixelDeltaU + pixelDeltaV);
        }
        public void Render(Hittable world, PaintEventArgs e)
        {
            Initialize();
            Vec3 pixelCenter;
            Vec3 rayDirection;
            Ray r;
            Vec3 unitDirection;
            Vec3 color;
            Vec3 n;
            for (double j = 0; j < imageHeight; j++)
            {
                for (double i = 0; i < imageWidth; i++)
                {
                    pixelCenter = pixel00Loc + (i * pixelDeltaU) + (j * pixelDeltaV);
                    rayDirection = pixelCenter - cameraCenter;
                    r = new Ray(cameraCenter, rayDirection);
                    color = rayColor(r, world);
                    DrawColor(color, i, j, e);
                }
            }
        }
        private Vec3 rayColor(Ray r, Hittable world)
        {
            HitRecord rec;
            if (world.Hit(r, new Interval(0, INFINITY), out rec))
            {
                return 0.5 * (rec.normal + new Vec3(1, 1, 1));
            }
            Vec3 unitDirection = Vec3.Unit(r.direction);
            double a = 0.5 * (unitDirection.y + 1);
            return (1 - a) * new Vec3(1, 1, 1) + a * new Vec3(0.5, 0.7, 1);
        }
        private void DrawColor(Vec3 color, double x, double y, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle((int)x, (int)y, 1, 1);
            render.Color = Color.FromArgb((int)(255 * color.x), (int)(255 * color.y), (int)(255 * color.z));
            e.Graphics.DrawRectangle(render, rect);
        }
    }
}
