using System.Drawing;
using WaveFunctionCollapse.Interfaces.Errors;

namespace SimpleModel
{
    internal class WaveElement : IImageTile
    {
        public enum Side
        {
            TOP, RIGHT, BOTTOM, LEFT
        }

        public int Entropy
        {
            get
            {
                return tiles.Count;
            }
        }

        public HashSet<Tile> TopNeighbors { get; private set; }
        public HashSet<Tile> RightNeighbors { get; private set; }
        public HashSet<Tile> LeftNeighbors { get; private set; }
        public HashSet<Tile> BottomNeighbors { get; private set; }

        public HashSet<Tile> tiles;
        private HashSet<string> tileNames;

        public WaveElement(HashSet<Tile> tiles)
        {
            this.tiles = tiles;
            MakeNeighborsSum();
            MakeTileNamesSet();
        }

        public void MakeNeighborsSum()
        {
            TopNeighbors = new HashSet<Tile>();
            RightNeighbors = new HashSet<Tile>();
            LeftNeighbors = new HashSet<Tile>();
            BottomNeighbors = new HashSet<Tile>();

            foreach (Tile tile in tiles)
            {
                TopNeighbors.UnionWith(tile.TopNeighbors);
                RightNeighbors.UnionWith(tile.RightNeighbors);
                LeftNeighbors.UnionWith(tile.LeftNeighbors);
                BottomNeighbors.UnionWith(tile.BottomNeighbors);
            }

        }

        public void LowerEntropy(HashSet<Tile> possibleTiles)
        {
            tiles.IntersectWith(possibleTiles);
            MakeNeighborsSum();
            MakeTileNamesSet();
        }

        private void MakeTileNamesSet()
        {
            tileNames = new HashSet<string>();
            foreach (Tile tile in tiles)
            {
                tileNames.Add(tile.name);
            }
        }

        //This returns a bitmap with all possible tiles overlapping each other
        public Bitmap GetImage(int n)
        {
            int[] rsum = new int[n * n];
            int[] gsum = new int[n * n];
            int[] bsum = new int[n * n];
            int denominator = tiles.Count;

            foreach (Tile tile in tiles)
            {
                int[] bitArray = tile.bitMap;
                for (int i = 0; i < n * n; i++)
                {
                    rsum[i] += GetR(bitArray[i]);
                    gsum[i] += GetG(bitArray[i]);
                    bsum[i] += GetB(bitArray[i]);
                }
            }

            Bitmap bitMap = new Bitmap(n, n);

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    if (denominator == 0)
                    {
                        bitMap.SetPixel(y, x, Color.Black);
                        continue;
                    }
                    Color color = Color.FromArgb(rsum[x * n + y] / denominator, gsum[x * n + y] / denominator, bsum[x * n + y] / denominator);
                    bitMap.SetPixel(y, x, color);
                }
            }

            return bitMap;
        }

        //this method first constructs a dictionary because some tiles after rotation can have multiple instances (for example any tile with symmetry type X will wlways have 8 instances of itself)
        //I still want the chance to choose this tile to be equal to its weight / (weight of all tiles in the set) that is why we first randomly choose a tile and then if there is more than one instance of it
        //in the set we will randomly choose one of them
        public Tile Collapse(Random random)
        {
            if (Entropy == 0)
            {
                throw new NoPossibleCollapseException("This wave cannot have a defined state! (The number of possible states that it can collapse to is equal to 0)");
            }

            Dictionary<string, List<Tile>> collapseDict = new();
            double sumOfWeights = 0;
            foreach (Tile tile in tiles)
            {
                if (collapseDict.ContainsKey(tile.name))
                {
                    collapseDict[tile.name].Add(tile);
                }
                else
                {
                    collapseDict[tile.name] = [tile];
                    sumOfWeights += tile.weight;
                }
            }

            double randomWeight = random.NextDouble() * sumOfWeights;
            double cumulativeSum = 0;
            foreach (List<Tile> list in collapseDict.Values)
            {
                cumulativeSum += list[0].weight;
                if (randomWeight <= cumulativeSum)
                {
                    return list[random.Next(list.Count)];
                }
            }

            //this error should never be thrown unless there is a bug
            throw new InvalidOperationException("Unexpected condition occured while collapsing!");
        }

        private int GetR(int pixel)
        {
            return pixel >> 16 & 0xFF;
        }

        private int GetG(int pixel)
        {
            return pixel >> 8 & 0xFF;
        }

        private int GetB(int pixel)
        {
            return pixel & 0xFF;
        }
    }
}
