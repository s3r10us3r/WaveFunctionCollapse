using OverlappingModel;
using System.Drawing;

namespace OverlappingModelTest
{
    public class ImageAnalyzerTests
    {

        private const string RESULT_PATH = "C:/Users/jedyn/OneDrive/Pulpit/TestResults";
        private const string FILE_PATH = "samples";
        

        private Bitmap ConstructBitmapFromPatterns(int n, Pattern[] patterns)
        {
            int quantity = patterns.Length;
            int colCount = Math.Min(quantity, 1000 / 3 / n);
            int rowCount = quantity / colCount;

            List<Pattern> patternList = patterns.ToList();

            Bitmap result = new Bitmap(colCount * 3 * n, rowCount * 3 * n);
            int imgIndex = 0;

            for (int col = 0; col < colCount; col++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    int xOffset = col * 3 * n + n;
                    int yOffset = row * 3 * n + n;

                    Pattern pattern = patternList[imgIndex++];

                    for (int x = 0; x < n; x++)
                    {
                        for (int y = 0; y < n; y++)
                        {
                            Color color = Color.FromArgb(pattern[x, y]);
                            result.SetPixel(xOffset + x, yOffset + y, color);
                        }
                    }
                }
            }

            return result;
        }

        private void CutAndSave(string fileName, string resultName, bool reflection, bool rotation, int n, int expectedNumOfPatterns)
        {
            Bitmap bitmap = new Bitmap($"{FILE_PATH}/{fileName}");
            if (bitmap is null)
                Console.WriteLine("BITMAP IS NULL");
            ImageAnalyzer analyzer = new ImageAnalyzer(bitmap, rotation, reflection, n);
            Pattern[] patterns = analyzer.Analyze().Patterns;
            Bitmap patternBitmap = ConstructBitmapFromPatterns(n, patterns);
            string patternFilePath = $"{RESULT_PATH}/{resultName}.png";

            patternBitmap.Save(patternFilePath);
            Assert.IsNotNull(patterns);
            Assert.That(patterns.Count, Is.EqualTo(expectedNumOfPatterns));
        }

        [Test]
        public void ShouldCutBitmapProperlyForCity()
        {
            CutAndSave("City.png", "Pattern_City", false, false, 3, 25);
        }

        [Test]
        public void ShouldCutBitmapProperlyFor3Bricks()
        {
            CutAndSave("3Bricks.png", "3Bricks_Patterns", false, false, 5, 761);
        }

        [Test]
        public void ShouldCutBitmapProperlyFor3BricksRotatedAndReflected()
        {
            CutAndSave("3Bricks.png", "3Bricks_Patterns_Rotated_Reflected", true, true, 3, 1342);
        }

        [Test]
        public void ShouldCutBitmapProperlyForSkyline2()
        {
            CutAndSave("Skyline2.png", "Skyline_Patterns", true, false, 4, 481);
        }

        [Test]
        public void ShouldCutBitmapProperlyForCityWhenRotatedAndReflected()
        {
            CutAndSave("City.png", "CityPatterns_Rotated_Reflected", true, true, 3, 25);
        }
    }
}