using SimpleModel;
using SimpleModel.XMLModels;
using System.Drawing;

namespace WFCTests
{
    public class WFCTests
    {
        private readonly string testPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testPackage");

        private string GetXMLPath(string tileSet)
        {
            return Path.Combine(testPath, tileSet, tileSet + ".xml");
        }

        //this makes an image where each row consists of all transformations of same tile from 0 to 7
        private Bitmap ConcatenateTiles(List<Tile> tiles, int n)
        {
            List<Bitmap> images = new();

            foreach (Tile tile in tiles)
            {
                images.Add(tile.GetImage(n));
            }

            int widthInPixels = n * 8;
            int heightInPixels = n * images.Count / 8;

            Bitmap result = new Bitmap(widthInPixels, heightInPixels);

            using (Graphics g = Graphics.FromImage(result))
            {
                int xOffset = 0;
                int yOffset = 0;
                int index = 0;
                foreach (Bitmap image in images)
                {
                    g.DrawImage(image, xOffset, yOffset);
                    index++;
                    xOffset += n;
                    if (xOffset >= widthInPixels)
                    {
                        xOffset = 0;
                        yOffset += n;
                    }
                }
            }

            return result;
        }


        [Test]
        public void CircuitShouldHaveSize112()
        {
            TileSetModel circuit = TileSetModel.DeserialeFromXML(GetXMLPath("Circuit"));
            List<Tile> tiles = circuit.MakeTiles(Path.Combine(testPath, "Circuit"));
            Bitmap testImage = ConcatenateTiles(tiles, circuit.N);

            testImage.Save("circuitTest.png");
            Assert.That(tiles, Has.Count.EqualTo(112));
        }

        [Test]
        public void KnotsShouldHaveSize40()
        {
            TileSetModel knots = TileSetModel.DeserialeFromXML(GetXMLPath("knots"));
            List<Tile> tiles = knots.MakeTiles(Path.Combine(testPath, "knots"));
            Bitmap testImage = ConcatenateTiles(tiles, knots.N);

            testImage.Save("knotsTest.png");
            Assert.That(tiles, Has.Count.EqualTo(40));
        }

        [Test]
        public void CirclesShouldHaveSize64()
        {
            TileSetModel circles = TileSetModel.DeserialeFromXML(GetXMLPath("circles"));
            List<Tile> tiles = circles.MakeTiles(Path.Combine(testPath, "circles"));
            Bitmap testImage = ConcatenateTiles(tiles, circles.N);

            testImage.Save("circleTest.png");
            Assert.That(tiles, Has.Count.EqualTo(64));
        }

        [Test]
        public void ShouldThrowXMLExceptionForXMLWithInvalidAttrs()
        {
            TestDelegate testDelegate = () => TileSetModel.DeserialeFromXML(GetXMLPath("invalidAttrs"));
            Assert.Throws<XMLException>(testDelegate);
        }

        [Test]
        public void ShouldThrowXMLExceptionForXMLWithInvalidNode()
        {
            TestDelegate testDelegate = () => TileSetModel.DeserialeFromXML(GetXMLPath("invalidNode"));
            Assert.Throws<XMLException>(testDelegate);
        }

        [Test]
        public void ShouldThrowXMLExceptionForNonExistingNeighbour()
        {
            TileSetModel corrupted = TileSetModel.DeserialeFromXML(GetXMLPath("invalidNeighbor"));
            TestDelegate testDelegate = () => corrupted.MakeTiles(Path.Combine(testPath, "invalidNeighbor"));
            Assert.Throws<XMLException>(testDelegate);
        }

        [Test]
        public void ShouldThrowXMLExceptionWhenCantFindAFile()
        {
            TileSetModel corrupted = TileSetModel.DeserialeFromXML(GetXMLPath("noFile"));
            TestDelegate testDelegate = () => corrupted.MakeTiles(Path.Combine(testPath, "noFile"));
            Assert.Throws<XMLException>(testDelegate);
        }
    }
}
