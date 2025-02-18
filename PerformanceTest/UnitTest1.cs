using System.Drawing;
using OverlappingModel;
using SkiaSharp;

namespace PerformanceTest
{
    [TestFixture]
    public class Tests
    {
        private SKBitmap bitmap;
        private OverlappingModelBuilder builder;

        [SetUp]
        public void Setup()
        {
            var imagePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Flowers.png");
            Assert.IsTrue(File.Exists(imagePath));
            bitmap = SKBitmap.Decode(imagePath);

            builder = new OverlappingModelBuilder();
            builder.Bitmap = bitmap;
            builder.N = 3;
            builder.Width = 50;
            builder.Height = 50;
            builder.ReflectionsEnabled = true;
            builder.RotationsEnabled = true;
            builder.LockLeft = true;
            builder.LockRight = true;
            builder.LockTop = true;
            builder.LockBottom = true;
            builder.Seed = 42;
        }

        [Test]
        public void PerformanceTest()
        {
            var model = builder.Build();
            model.CollapseAll();
        }

        [TearDown]
        public void TearDown()
        {
            bitmap.Dispose();
        }
    }
}