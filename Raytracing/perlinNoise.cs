namespace Raytracing
{
    public class Perlin
    {
        const int POINTCOUNT = 256;
        Vec3[] randVec = new Vec3[POINTCOUNT];
        int[] permX = new int[POINTCOUNT];
        int[] permY = new int[POINTCOUNT];
        int[] permZ = new int[POINTCOUNT];
        public Perlin()
        {
            for (int i = 0; i < POINTCOUNT; i++)
            {
                randVec[i] = Vec3.RandomUnitVector();
            }
            PerlinGeneratePerm(permX);
            PerlinGeneratePerm(permY);
            PerlinGeneratePerm(permZ);
        }
        public double Noise(Vec3 p)
        {
            double u = p.x - Math.Floor(p.x);
            double v = p.y - Math.Floor(p.y);
            double w = p.z - Math.Floor(p.z);

            int i = (int) Math.Floor(p.x);
            int j = (int) Math.Floor(p.y);
            int k = (int) Math.Floor(p.z);
            Vec3[,,] c = new Vec3[2, 2, 2];

            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = randVec[
                            permX[(i+di) & 255] ^
                            permY[(j+dj) & 255] ^
                            permZ[(k+dk) & 255]
                        ];
                    }
                }
            }

            return PerlinInterp(c, u, v, w);
        }
        public double Turb(Vec3 p, int depth)
        {
            double accum = 0.0;
            Vec3 tempP = p;
            double weight = 1.0;

            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5;
                tempP *= 2;
            }
            return Math.Abs(accum);
        }
        private static void PerlinGeneratePerm(int[] p)
        {
            for (int i = 0; i < POINTCOUNT; i++)
            {
                p[i] = i;
            }
            Permute(p, POINTCOUNT);
        }
        private static void Permute(int[] p, int n)
        {
            for (int i = n - 1; i > 0; i--)
            {
                int target = Util.RandomInt(0, i);
                int tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        } 
        private static double PerlinInterp(Vec3[,,] c, double u, double v, double w)
        {
            double uu = u*u*(3-2*u);
            double vv = v*v*(3-2*v);
            double ww = w*w*(3-2*w);

            double accum = 0;

            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    for(int k = 0; k < 2; k++)
                    {
                        Vec3 weightV = new Vec3(u-i, v-j, w-k);
                        accum += (i*uu + (1-i) * (1-uu))
                                *(j*vv + (1-j) * (1-vv))
                                *(k*ww + (1-k) * (1-ww))
                                * Vec3.Dot(c[i, j, k], weightV);
                    }
                }
            }
            return accum;
        }
    }   
}