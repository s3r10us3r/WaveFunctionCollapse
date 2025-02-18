namespace OverlappingModel
{
    internal class Pattern
    {
        public int[,] Pixels { get; }
        public int Frequency = 1;
        private readonly int n;
        public int Id { get; set; }

        public CoefficienceSet[,] OverlappingPatterns { get; private set; }

        public Pattern(int n, int[,] pixels)
        {
            if (pixels.GetLength(0) != n || pixels.GetLength(1) != n)
            {
                throw new ArgumentException("Pixels array must have dimensions of nxn");
            }
            this.n = n;
            Pixels = pixels;
            InitOverlappingPatterns();
        }

        private void InitOverlappingPatterns()
        {
            OverlappingPatterns = new CoefficienceSet[2 * n - 1, 2 * n - 1];
            for (int i = 0; i < 2 * n - 1; i++)
            {
                for (int j = 0; j < 2 * n - 1; j++)
                {
                    OverlappingPatterns[i, j] = new CoefficienceSet(0);
                }
            }
        }

        public void MatchPattern(Pattern pattern)
        {
            for (int i = -n + 1; i < n; i++)
                for (int j = -n + 1; j < n; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int x = i + n - 1;
                    int y = j + n - 1;

                    if (DoesOverlap(i, j, pattern))
                    {
                        OverlappingPatterns[x, y].Add(pattern.Id);
                    }
                }
        }

        public bool DoesOverlap (int xOffset, int yOffset, Pattern pattern)
        {
            int xStart = xOffset < 0 ? - xOffset : 0;
            int xEnd = xOffset > 0 ? n - 1 - xOffset : n - 1;

            int yStart = yOffset < 0 ? - yOffset : 0;
            int yEnd = yOffset > 0 ? n - 1 - yOffset : n - 1;

            for (int i = xStart; i <= xEnd; i++)
            {
                for (int j = yStart; j <= yEnd; j++)
                {
                    int x = xOffset < 0 ? i - xStart : xOffset + i;
                    int y = yOffset < 0 ? j - yStart : yOffset + j;

                    if (pattern[i, j] != this[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public CoefficienceSet GetOverlappingPatterns(int xOffset, int yOffset)
        {
            int x = xOffset + n - 1;
            int y = yOffset + n - 1;

            return OverlappingPatterns[x, y];
        }

        public int this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= n || y < 0 || y >= n)
                {
                    throw new IndexOutOfRangeException($"x and y must be in range <0, {n - 1}>");
                }
                return Pixels[x, y];
            }
        }

        public Pattern Rotate()
        {
            int[,] rotatedPixels = new int[n, n];
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    rotatedPixels[n - y - 1, x] = Pixels[x, y];
                }
            }

            Pattern result = new(n, rotatedPixels);
            result.Frequency = Frequency;
            return result;
        }

        public Pattern Reflect()
        {
            int[,] reflectedPixels = new int[n, n];
            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    reflectedPixels[n - 1 - x, y] = Pixels[x, y];
                }
            }
            
            Pattern result = new(n, reflectedPixels);
            result.Frequency = Frequency;
            return result;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Pattern p)
                return false;
            
            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                {
                    if (Pixels[x, y] != p.Pixels[x, y])
                        return false;
                }

            return true;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
