using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// tutorial is from https://raytracing.github.io/ book one is completed
namespace Raytracing
{
    public partial class Raytracer : Form
    {
        public const double PI = Math.PI;
        HittableList world = new HittableList();
        Camera cam = new Camera();

        public int resolution = 2;
        public void BouncingSpheres()
        {
            // World Setup
            Texture checker = new CheckerTexture(0.32, new Vec3(0.2, 0.3, 0.1), new Vec3(0.9, 0.9, 0.9));
            Material groundMaterial = new Lambertian(new Vec3(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checker)));
            
            double chooseMat;
            Vec3 center;
            Material sphereMaterial;
            Vec3 albedo;
            double fuzz;
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    chooseMat = Util.RandomDouble();
                    center = new Vec3(a + 0.9 * Util.RandomDouble(), 0.2, b + 0.9 * Util.RandomDouble());
                    if ((center - new Vec3(4, 0.2, 0)).Length() > 0.9)
                    {
                        if (chooseMat < 0.8)
                        {
                            // difuse
                            albedo = Util.Random() * Util.Random();
                            sphereMaterial = new Lambertian(albedo);
                            Vec3 center2 = center + new Vec3(0, Util.RandomDouble(0, .5), 0);
                            world.Add(new Sphere(center, center2, 0.2, sphereMaterial)); 
                        }
                        else if (chooseMat < 0.95)
                        {
                            // metal
                            albedo = Util.Random(0.5, 1);
                            fuzz = Util.RandomDouble(0, 0.5);
                            sphereMaterial = new Metal(albedo, fuzz);
                            world.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else
                        {
                            sphereMaterial = new Dielectric(1.5);
                            world.Add(new Sphere(center, 0.2, sphereMaterial));
                        }

                    }
                }
            }
            
            Material material1 = new Dielectric(1.5);
            world.Add(new Sphere(new Vec3(0, 1, 0), 1, material1));

            Material material2 = new Lambertian(new Vec3(0.4, 0.2, 0.1));
            world.Add(new Sphere(new Vec3(-4, 1, 0), 1, material2));

            Material material3 = new Metal(new Vec3(0.7, 0.6, 0.5), 0.0);
            world.Add(new Sphere(new Vec3(4, 1, 0), 1, material3));

            cam.aspectRatio = 16.00 / 9.00;
            cam.imageWidth = 400;
            cam.samplesPerPixel = 10;
            cam.vfov = 20;
            cam.lookFrom = new Vec3(13, 2, 3);
            cam.lookAt = new Vec3(0, 0, 0);
            cam.vup = new Vec3(0, 1, 0);
            cam.defocusAngle = 0.6;
            cam.focusDist = 10.0;
        }
        public void CheckeredSpheres()
        {
            Texture checker = new CheckerTexture(0.32, new Vec3(.2, .3, .1), new Vec3(.9, .9, .9));

            world.Add(new Sphere(new Vec3(0, -10, 0), 10, new Lambertian(checker)));
            world.Add(new Sphere(new Vec3(0, 10, 0), 10, new Lambertian(checker)));

            cam.aspectRatio = 16.00 / 9.00;
            cam.imageWidth = 400;
            cam.samplesPerPixel = 10;
            
            cam.vfov = 20;
            cam.lookFrom = new Vec3(13, 2, 3);
            cam.lookAt = new Vec3(0, 0, 0);
            cam.vup = new Vec3(0, 1, 0);
            cam.defocusAngle = 0;
        }
        void Earth()
        {
            Texture earthTexture = new ImageTexture("earthmap.jpg");
            var earthSurface = new Lambertian(earthTexture);
            Sphere globe = new Sphere(new Vec3(0, 0, 0), 2, earthSurface);

            world.Add(globe);
            cam.aspectRatio = 16.0/9.0;
            cam.imageWidth = 400;
            cam.imageHeight = 100;
            
            cam.vfov = 20;
            cam.lookFrom = new Vec3(0, 0, 12);
            cam.lookAt = new Vec3(0, 0, 0);
            cam.vup = new Vec3(0, 1, 0);

            cam.samplesPerPixel = 10;
            cam.defocusAngle = 0;

        }
        public Raytracer()
        {
            InitializeComponent();
        }
        public void Raytracer_Paint(object sender, PaintEventArgs e)
        {
            Earth();
            cam.Render(world, e, resolution);
        }
    }
}
