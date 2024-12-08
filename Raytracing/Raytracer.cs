﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// tutorial is from https://raytracing.github.io/books/RayTracingInOneWeekend.html 
namespace Raytracing
{
    public partial class Raytracer : Form
    {
        double PI = Math.PI;
        HittableList world = new HittableList();
        Vec3 cameraCenter;
        Camera cam = new Camera();
        public void SetVariables()
        {
            // World Setup
            world.Add(new Sphere(new Vec3(0, 0, -1), 0.5));
            world.Add(new Sphere(new Vec3(0, -100.5, -1), 100));
            cam.aspectRatio = 16 / 9;
            cam.imageWidth = 400;
        }
        public double DegreesToRadions(double degrees)
        {
            return degrees * PI / 180;
        }
        public Raytracer()
        {
            InitializeComponent();
        }
        public void Raytracer_Paint(object sender, PaintEventArgs e)
        {
            SetVariables();
            cam.Render(world, e);
        }
    }
}