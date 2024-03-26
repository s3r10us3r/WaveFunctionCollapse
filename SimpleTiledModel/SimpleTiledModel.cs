using System.Drawing;
using System.IO;
using WaveFunctionCollapse.SimpleTiledModel.Errors;
using WaveFunctionCollapse.SimpleTiledModel.XMLModels;

namespace WaveFunctionCollapse.SimpleTiledModel
{
    public class SimpleTiledModel
    {
        private Tile[,] image;
        private HashSet<Tile>?[,] wave;
        private List<Tile> tiles;
        private Random random;
        private string format;

        private int observationsLeft;
        private (int x, int y) lowestEntropy;
        private int width;
        private int height;
        private int n;
        public SimpleTiledModel(string tileSetPath, string tileSetName, int width, int height)
        {
            random = new Random();
            Init(tileSetPath, tileSetName, width, height, random);
        }

        public SimpleTiledModel(string tileSetPath, string tileSetName, int width, int height, int seed)
        {
            random = new Random(seed);
            Init(tileSetPath, tileSetName, width, height, random);
        }

        private void Init(string tileSetPath, string tileSetName, int width, int height, Random random) 
        {
            this.width = width;
            this.height = height;
            string xmlFilePath = Path.Combine(tileSetPath, tileSetName + ".xml");
            TileSetModel tileSetModel = TileSetModel.DeserialeFromXML(xmlFilePath);


            format = tileSetModel.Format;
            n = tileSetModel.N;
            tiles = tileSetModel.MakeTiles(tileSetPath);

            image = new Tile[width, height];
            wave = new HashSet<Tile>[width, height];


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    wave[i, j] = tiles.ToHashSet();
                }
            }

            //since entropy is equal for every empty tile at the beggining we can just start with this 
            lowestEntropy = (0, 0);
            this.random = random;
            observationsLeft = width * height;
        }

        public void CollapseNTimes(int n)
        {
            while (n--  > 0 && observationsLeft-- > 0)
            {
                Collapse();
            }
        }

        public void CollapseAll()
        {
            while(observationsLeft > 0)
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

            int x = lowestEntropy.x;
            int y = lowestEntropy.y;

            List<Tile> stateList = wave[x, y].ToList();

            int bound = stateList.Count;
            int chosenIndex = random.Next(bound);

            Tile chosenState = stateList[chosenIndex];
            image[x, y] = chosenState;

            //we set the cell to null whenever 
            wave[x, y] = null;

            Observe(chosenState, x, y);

            return --observationsLeft;
        }

        public Bitmap GenerateImage()
        {
            Bitmap bitmap = new Bitmap(width * n, height * n);
            
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (image[x,y] is null)
                        {
                            g.DrawRectangle(Pens.Black, new Rectangle(x * n, y * n, n, n));
                        }
                        else
                        {
                            g.DrawImage(image[x, y].GetImage(n), x * n, y * n);
                        }
                    }
                }
            }

            return bitmap;
        }

        private void Observe(Tile chosenState, int x, int y)
        {
            int lowestEntropyValue = int.MaxValue;

            //right
            if (x < width - 1 && wave[x + 1, y] is not null)
            {
                wave[x + 1, y].IntersectWith(chosenState.RightNeighbors);
                if (wave[x + 1, y].Count == 0)
                {
                    throw new NoPossibleCollapseException();
                }

                if (lowestEntropyValue < wave[x + 1, y].Count)
                {
                    lowestEntropyValue = wave[x + 1, y].Count;
                    lowestEntropy = (x + 1, y);
                }
            }
            //left
            if (x > 0 && wave[x - 1, y] is not null)
            {
                wave[x - 1, y].IntersectWith(chosenState.LeftNeighbors);
                if (wave[x - 1, y].Count == 0)
                {
                    throw new NoPossibleCollapseException();
                }

                if (lowestEntropyValue < wave[x - 1, y].Count)
                {
                    lowestEntropyValue = wave[x - 1, y].Count;
                    lowestEntropy = (x - 1, y);
                }
            }
            //top
            if (y > 0 && wave[x, y - 1] is not null)
            {
                wave[x, y - 1].IntersectWith(chosenState.TopNeighbors);
                if (wave[x, y - 1].Count == 0)
                {
                    throw new NoPossibleCollapseException();
                }

                if (lowestEntropyValue < wave[x, y - 1].Count)
                {
                    lowestEntropyValue = wave[x, y - 1].Count;
                    lowestEntropy = (x, y - 1);
                }
            }
            //bottom
            if (y < height - 1 && wave[x, y + 1] is not null)
            {
                wave[x, y + 1].IntersectWith(chosenState.BottomNeighbors);
                if (wave[x, y + 1].Count == 0)
                {
                    throw new NoPossibleCollapseException();
                }

                if (lowestEntropyValue < wave[x, y + 1].Count)
                {
                    lowestEntropyValue = wave[x, y + 1].Count;
                    lowestEntropy = (x, y + 1);
                }
            }
        }
    }
}
