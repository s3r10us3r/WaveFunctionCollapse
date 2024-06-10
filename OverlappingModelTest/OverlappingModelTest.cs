using OverlappingModel;
using System.Drawing;

namespace OverlappingModelTest
{
    public class OverlappingModelTest
    {
        private const string FILE_PATH = "samples";
        private const string SAVE_PATH = "C:/Users/jedyn/OneDrive/Pulpit/TestResults";
        private const int SEED = 21372137;
        private void ShouldCorrectlyCreateBitmapForFile(string fileName, OverlappingModel.OverlappingModel model)
        {
                model.CollapseAll();
                Bitmap bitmap = model.Image;
                bitmap.Save($"{SAVE_PATH}/{fileName}");
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapForCityPNG()
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/City.png");
            OverlappingModelBuilder builder = new();
            builder.SetReflectionsEnabled(true);
            builder.SetRotationsEnabled(true);
            builder.SetBitmap(bitmap);
            builder.SetHeight(100);
            builder.SetWidth(100);
            builder.SetN(2);
            builder.SetSeed(SEED);
            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("City_100x100.png", model);
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapForCitySkyline2()
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/Skyline2.png");
            OverlappingModelBuilder builder = new();
            builder.SetReflectionsEnabled(true);
            builder.LockTop(true);
            builder.LockBottom(true);
            builder.SetRotationsEnabled(false);
            builder.SetBitmap(bitmap);
            builder.SetHeight(50);
            builder.SetWidth(50);
            builder.SetN(3);
            builder.SetSeed(SEED);
            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("Skyline_50x50.png", model);
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapForFlowersPNG()
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/Flowers.png");

            OverlappingModelBuilder builder = new();

            builder.SetHeight(50);
            builder.SetWidth(100);
            builder.SetBitmap(bitmap);
            builder.SetReflectionsEnabled(true);
            builder.LockBottom(true);
            builder.LockLeft(true);
            builder.LockRight(true);
            builder.LockTop(true);
            builder.SetHeight(50);
            builder.SetWidth(100);
            builder.SetN(3);
            builder.SetSeed(SEED);

            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("Flowers_100x50.png", model);
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapForPlatformerPNG()
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/Platformer.png");
            OverlappingModelBuilder builder = new();

            builder.SetHeight(100);
            builder.SetWidth(500);
            builder.SetBitmap(bitmap);
            builder.SetReflectionsEnabled(true);
            builder.LockBottom(true);
            builder.LockTop(true);
            builder.SetN(3);
            builder.SetSeed(SEED);
            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("Platformer_500x100.png", model);
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapForWaterPNG()
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/Water.png");
        
            OverlappingModelBuilder builder = new();

            builder.SetHeight(200);
            builder.SetWidth(200);
            builder.SetBitmap(bitmap);
            builder.SetN(3);
            builder.SetSeed(SEED);

            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("Water_200x200.png", model);
        }

        [Test]
        public void ShouldCorrectlyCreateBitmapFor3BricksPNG()
        {
            
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/3Bricks.png");
        
            OverlappingModelBuilder builder = new();

            builder.SetHeight(100);
            builder.SetWidth(200);
            builder.SetBitmap(bitmap);
            builder.SetN(3);
            builder.SetSeed(SEED);
            builder.SetRotationsEnabled(true);
            builder.SetReflectionsEnabled(true);

            OverlappingModel.OverlappingModel model = builder.Build();
            ShouldCorrectlyCreateBitmapForFile("3Bricks_200x100.png", model);
        }
    }
}
