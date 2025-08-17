using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
            
            cam.background = new Vec3(0.7, 0.8, 1.0);
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
            cam.background = new Vec3(0.7, 0.8, 1.0);
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

            cam.background = new Vec3(0.7, 0.8, 1.0);
        }
        void PerlinSpheres()
        {
            Texture perTex = new NoiseTexture(4);
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(perTex)));
            world.Add(new Sphere(new Vec3(0, 2, 0), 2, new Lambertian(perTex)));
            
            cam.aspectRatio = 16.0/9.0;
            cam.imageWidth = 400;
            cam.samplesPerPixel = 10;

            cam.vfov = 20;
            cam.lookFrom = new Vec3(13, 2, 3);
            cam.lookAt = new Vec3(0, 0, 0);
            cam.vup = new Vec3(0, 1, 0);

            cam.defocusAngle = 0;
            cam.background = new Vec3(0.7, 0.8, 1.0);
        }
        void Quads()
        {
            Lambertian leftRed = new Lambertian(new Vec3(1.0, 0.2, 0.2));
            Lambertian backGreen = new Lambertian(new Vec3(0.2, 1.0, 0.2));
            Lambertian rightBlue = new Lambertian(new Vec3(0.2, 0.2, 1.0));
            Lambertian upperOrange = new Lambertian(new Vec3(1.0, 0.5, 0.0));
            Lambertian lowerTeal = new Lambertian(new Vec3(0.2, 0.8, 0.8));
            world.Add(new Quad(new Vec3(-3, -2, 5), new Vec3(0, 0, -4), new Vec3(0, 4, 0), leftRed));
            world.Add(new Quad(new Vec3(-2, -2, 0), new Vec3(4, 0, 0), new Vec3(0, 4, 0), backGreen));
            world.Add(new Quad(new Vec3(3, -2, 1), new Vec3(0, 0, 4), new Vec3(0, 4, 0), rightBlue));
            world.Add(new Quad(new Vec3(-2, 3, 1), new Vec3(4, 0, 0), new Vec3(0, 0, 4), upperOrange));
            world.Add(new Quad(new Vec3(-2, -3, 5), new Vec3(4, 0, 0), new Vec3(0, 0, -4), lowerTeal));

            cam.aspectRatio = 1.00;
            cam.imageWidth = 400;
            cam.samplesPerPixel = 10;
            
            cam.vfov = 80; 
            cam.lookFrom = new Vec3(0, 0, 9);
            cam.lookAt = new Vec3(0, 0, 0);
            cam.vup = new Vec3(0, 1, 0);
            
            cam.defocusAngle = 0;
            cam.background = new Vec3(0.7, 0.8, 1.0);
        }
        void SimpleLight()
        {
            var pertex = new NoiseTexture(4);
            world.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(pertex)));
            world.Add(new Sphere(new Vec3(0, 2, 0), 2, new Lambertian(pertex)));

            var difflight = new DifuseLight(new Vec3(4, 4, 4));
            world.Add(new Quad(new Vec3(3, 1, -2), new Vec3(2, 0, 0), new Vec3(0, 2, 0), difflight));
            world.Add(new Sphere(new Vec3(0, 7, 0), 2, difflight));

            cam.aspectRatio = 16.0 / 9.0;
            cam.imageWidth = 400;
            cam.samplesPerPixel = 10;
            cam.background = new Vec3(0, 0, 0);

            cam.vfov = 20;
            cam.lookFrom = new Vec3(26, 3, 6);
            cam.lookAt = new Vec3(0, 2, 0);
            cam.vup = new Vec3(0, 1, 0);
            cam.defocusAngle = 0;
        }
        void CornellBox()
        {
            Lambertian red = new Lambertian(new Vec3(0.65, 0.05, 0.05));
            Lambertian white = new Lambertian(new Vec3(0.73, 0.73, 0.73));
            Lambertian green = new Lambertian(new Vec3(0.12, 0.45, 0.15));
            DifuseLight light = new DifuseLight(new Vec3(15, 15, 15));

            world.Add(new Quad(new Vec3(555, 0, 0), new Vec3(0, 555, 0), new Vec3(0, 0, 555), green));
            world.Add(new Quad(new Vec3(0, 0, 0), new Vec3(0, 555, 0), new Vec3(0, 0, 555), red));
            world.Add(new Quad(new Vec3(343, 554, 332), new Vec3(-130, 0, 0), new Vec3(0, 0, -105), light));
            world.Add(new Quad(new Vec3(0, 0, 0), new Vec3(555, 0, 0), new Vec3(0, 0, 555), white));
            world.Add(new Quad(new Vec3(555, 555, 555), new Vec3(-555, 0, 0), new Vec3(0, 0, -555), white));
            world.Add(new Quad(new Vec3(0, 0, 555), new Vec3(555, 0, 0), new Vec3(0, 555, 0), white));
            
            world.Add(Quad.Box(new Vec3(130, 0, 65), new Vec3(295, 165, 230), white));
            world.Add(Quad.Box(new Vec3(265, 0, 295), new Vec3(430, 330, 460), white));
            
            cam.aspectRatio = 1.0;
            cam.imageWidth = 600;
            cam.samplesPerPixel = 10;
            cam.background = new Vec3(0, 0, 0);
            
            cam.vfov = 40;
            cam.lookFrom = new Vec3(278, 278, -800);
            cam.lookAt = new Vec3(278, 278, 0);
            cam.vup = new Vec3(0, 1, 0);

            cam.defocusAngle = 0;
        }
        public Raytracer()
        {
            InitializeComponent();
        }

        private Bitmap? _renderedBitmap = null;
        public void Raytracer_Paint(object sender, PaintEventArgs e)
        {
            if (_renderedBitmap == null)
            {
                // Create the specific world to render.
                switch (0)
                {
                    case 0: BouncingSpheres(); break;
                    case 1: CheckeredSpheres(); break;
                    case 2: Earth(); break;
                    case 3: PerlinSpheres(); break;
                    case 4: Quads(); break;
                    case 5: SimpleLight(); break;
                    case 6: CornellBox(); break;
                }

                // Initialize the world to configure the image has a width and height that may be
                // used to create a bitmap.
                cam.Initialize();
                _renderedBitmap = new Bitmap(cam.imageWidth, cam.imageHeight);

                int numSections = Environment.ProcessorCount;
                List<Thread> threads = new List<Thread>();
                for (int section = 0; section < numSections; section++)
                {
                    int localSection = section;
                    Thread thread = new Thread(() =>
                    {
                        cam.Render(world, _renderedBitmap, resolution, localSection, numSections);

                        // The thread has completed, so lock the pool and remove the thread.
                        lock (threads)
                        {
                            threads.Remove(Thread.CurrentThread);
                        }
                    });

                    // Have the thread run at a lower priority so that the UI remains responsive.
                    thread.Priority = ThreadPriority.BelowNormal;

                    // Add the thread into the pool:
                    // - Since nothing is started yet, we don't need to lock yet.
                    threads.Add(thread);
                }

                // Start all threads in the pool.
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        thread.Start();
                    }
                }

                // While there are still threads in the pool, draw the current state of the
                // bitmap:
                while (threads.Count > 0)
                {
                    // Need to lock the graphics object to prevent background threads from
                    // modifying while we draw.
                    lock (_renderedBitmap)
                    {
                        e.Graphics.DrawImage(_renderedBitmap, 0, 0);
                    }

                    // Wait a 1/2 second before checking again.
                    Thread.Sleep(500);
                }
            }

            e.Graphics.DrawImage(_renderedBitmap, 0, 0);
        }
    }
}
