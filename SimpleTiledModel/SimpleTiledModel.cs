using System.Drawing;
using SimpleModel.XMLModels;
using WaveFunctionCollapse.Interfaces;
using WaveFunctionCollapse.Interfaces.Errors;

namespace SimpleModel
{
    public class SimpleTiledModel : IWaveFunctionCollapseModel
    {
        private IImageTile[,] image;
        private List<Tile> tiles;
        private Random random;

        private int observationsLeft;
        private int width;
        private int height;
        private int n;

        private TileSetModel tileSetModel;
        private HashSet<(int, int)> updatedTiles;
        private Bitmap bitmap;

        public int CollapsesLeft => observationsLeft;
        public Bitmap Image => GenerateImage();

        public SimpleTiledModel(string tilesetPath, string tilesetName, int width, int height) : this(tilesetPath, tilesetName, width, height, new Random()) { }

        public SimpleTiledModel(string tilesetPath, string tilesetName, int width, int height, int seed) : this(tilesetPath, tilesetName, width, height, new Random(seed)) { }


        private SimpleTiledModel(string tilesetPath, string tilesetName, int width, int height, Random random)
        {
            this.width = width;
            this.height = height;
            string xmlFilePath = Path.Combine(tilesetPath, tilesetName);
            tileSetModel = TileSetModel.DeserialeFromXML(xmlFilePath);
            this.random = random;
            n = tileSetModel.N;
            tiles = tileSetModel.MakeTiles(tilesetPath);
            updatedTiles = new HashSet<(int, int)>();
            Reset();
        }

        public void Reset()
        {
            image = new IImageTile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    image[i, j] = new WaveElement(tiles.ToHashSet());
                }
            }

            observationsLeft = width * height;
            InitBitmap();
        }

        public void CollapseNTimes(int n)
        {
            while (n-- > 0 && observationsLeft > 0)
            {
                Collapse();
            }
        }

        public void CollapseAll()
        {
            while (observationsLeft > 0)
            {
                Collapse();
            }
        }

        public int Collapse()
        {
            if (observationsLeft == 0)
            {
                return 0;
            }
            try
            {
                (int x, int y) = Observe();


                updatedTiles.Add((x, y));
                Tile chosenState = ((WaveElement)image[x, y]).Collapse(random);
                image[x, y] = chosenState;
                Propagate(x, y);

            }
            catch (InvalidOperationException e)
            {

            }
            return --observationsLeft;
        }

        private Bitmap GenerateImage()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach ((int x, int y) in updatedTiles)
                {
                    g.DrawImage(image[x, y].GetImage(n), x * n, y * n);   
                }
            }
            updatedTiles = new();
            return bitmap;
        }

        private void Propagate(int x, int y)
        {
            Queue<(int, int)> observationQue = new Queue<(int, int)>();
            Tile collapsedTile = (Tile)image[x, y];
            PropagateAround(collapsedTile, x, y, observationQue);

            while (observationQue.Count > 0)
            {
                (x, y) = observationQue.Dequeue();
                PropagateAround(image[x, y], x, y, observationQue);
            }
        }

        private void PropagateAround(IImageTile elem, int x, int y, Queue<(int, int)> observationQue)
        {
            if (x > 0)
            {
                PropagateOnElem(x - 1, y, elem.LeftNeighbors, observationQue);
            }
            if (x < width - 1)
            {
                PropagateOnElem(x + 1, y, elem.RightNeighbors, observationQue);
            }
            if (y > 0)
            {
                PropagateOnElem(x, y - 1, elem.TopNeighbors, observationQue);
            }
            if (y < height - 1)
            {
                PropagateOnElem(x, y + 1, elem.BottomNeighbors, observationQue);
            }
        }

        private void PropagateOnElem(int x, int y, HashSet<Tile> tileSet, Queue<(int, int)> observationQue)
        {
            if (image[x, y] is WaveElement observedElem)
            {
                updatedTiles.Add((x, y));
                int initialEntropy = observedElem.Entropy;
                observedElem.LowerEntropy(tileSet);
                if (observedElem.Entropy < initialEntropy)
                {
                    observationQue.Enqueue((x, y));
                }
            }
        }

        private (int x, int y) Observe()
        {
            (int x, int y) lowestEntropy = (-1, -1);
            int lowestEntropyValue = int.MaxValue;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (image[i, j] is WaveElement waveElement && waveElement.Entropy < lowestEntropyValue)
                    {
                        lowestEntropyValue = waveElement.Entropy;
                        lowestEntropy = (i, j);

                        if (lowestEntropyValue == 0)
                        {
                            throw new NoPossibleCollapseException($"No possible collapse esception at {i} {j}");
                        }
                    }
                }
            }

            if (lowestEntropy == (-1, -1))
            {
                throw new InvalidOperationException("All wave elements are null!");
            }

            return lowestEntropy;
        }

        private void InitBitmap()
        {
            bitmap = new Bitmap(width * n, height * n);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        g.DrawImage(image[x, y].GetImage(n), x * n, y * n);
                    }
                }
            }
        }
    }
}
