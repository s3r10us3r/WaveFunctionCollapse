using System.Drawing;
using SimpleModel;
using WaveFunctionCollapse.Interfaces.Errors;

namespace WFCTests
{
    internal class SimpleTiledModelTests
    {
        private readonly string testPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testPackage");
        private const string RESULT_PATH = "C:/Users/jedyn/OneDrive/Pulpit/TestResults";

        public void ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(int seed, int size, string tile_set)
        {
            SimpleTiledModel model = new SimpleTiledModel(Path.Combine(testPath, tile_set), tile_set, size, size, seed);
            try
            {
                model.CollapseAll();
            }
            catch (NoPossibleCollapseException e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                Bitmap bitmap = model.Image;
                bitmap.Save($"{RESULT_PATH}/{tile_set}_test_{size}.png");
            }
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnots10x10()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 10, "knots");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnots20x20()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 20, "knots");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnots50x50()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 50, "knots");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnots100x100()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 100, "knots");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnotsRestricted10x10()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 10, "knots_restricted");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnotsRestricted20x20()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 20, "knots_restricted");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnotsRestricted50x50()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 50, "knots_restricted");
        }
        [Test]
        public void ShouldRunAndReturnAValidBitmapForKnotsRestricted100x100()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 100, "knots_restricted");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircles10x10()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 10, "circles");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircles20x20()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 20, "circles");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircles50x50()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 50, "circles");
        }


        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircles100x100()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1050, 100, "circles");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircuit10x10()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 10, "Circuit");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircuit20x20()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 20, "Circuit");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircuit50x50()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 50, "Circuit");
        }

        [Test]
        public void ShouldRunAndReturnAValidBitmapForCircuit100x100()
        {
            ShouldReturnValidBitmapForSeedOrThrowNoPossibleCollapseException(1000, 100, "Circuit");
        }
    }
}
