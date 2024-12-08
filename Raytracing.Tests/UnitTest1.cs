using System.Numerics;
using System.Windows.Forms;

namespace Raytracing.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Vec3 v1 = new Vec3(1, 2, 3);

            Assert.Equal(1, v1.x);
        }
        [Fact]
        public void Test2()
        {
            Vec3 v1 = new Vec3(1, 2, 3);
            Vec3 v2 = new Vec3(3, 2, 1);
            Vec3 v3 = v1 + v2;
            Assert.Equal(4, v3.x);
        }
        [Fact]
        public void HitTest()
        {
            
        }
    }
}