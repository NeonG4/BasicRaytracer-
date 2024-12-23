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

        public int resolution = 1;
        public void SetVariables()
        {
            // World Setup
            /*
            Material materialGround = new Lambertian(new Vec3(0.8, 0.8, 0.0));
            Material materialCenter = new Lambertian(new Vec3(0.1, 0.2, 0.5));
            Material materialLeft = new Dielectric(1.5);
            Material materialBubble = new Dielectric(1.00 / 1.50);
            Material materialRight = new Metal(new Vec3(0.8, 0.6, 0.2), 1);

            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100, materialGround));
            world.Add(new Sphere(new Vec3(0, 0, -1.2), 0.5, materialCenter));
            world.Add(new Sphere(new Vec3(-1, 0, -1), 0.5, materialLeft));
            world.Add(new Sphere(new Vec3(-1, 0, -1), 0.4, materialBubble));
            world.Add(new Sphere(new Vec3(1, 0, -1), 0.5, materialRight));
            */
            Material groundMaterial = new Lambertian(new Vec3(0.5, 0.5, 0.5));
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, groundMaterial));
            
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
        public Raytracer()
        {
            InitializeComponent();
        }
        public void Raytracer_Paint(object sender, PaintEventArgs e)
        {
            SetVariables();
            cam.Render(world, e, resolution);
        }
    }
}
