using System.Drawing;
using WaveFunctionCollapse.Interfaces.Errors;

namespace OverlappingModel
{
    internal class Wave
    {
        public int CollapsesLeft { get; private set; }
        public Bitmap Image
        {
            get
            {
                var bitmap = new Bitmap(width, height);
                for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Color color = GetPixel(x, y);
                    bitmap.SetPixel(x, y, color);
                }
                return bitmap;
            }
        }

        private readonly int n;
        private readonly Random rand;
        //pattern matching starts from top left corner
        private WaveCell[,] wave;

        private readonly CoefficienceSet topPatterns;
        private readonly CoefficienceSet bottomPatterns;
        private readonly CoefficienceSet leftPatterns;
        private readonly CoefficienceSet rightPatterns;

        private readonly int width;
        private readonly int height;

        private readonly bool lockTop;
        private readonly bool lockBottom;
        private readonly bool lockRight;
        private readonly bool lockLeft;

        private int lowestEntropy = int.MaxValue;
        private (int x, int y) lowestEntropyCords;

        public Wave(int width, int height, int n, Random rand, ImageAnalysisResult analysisResult, bool lockTop, bool lockBottom, bool lockRight, bool lockLeft)
        {
            this.width = width;
            this.height = height;
            this.n = n;
            this.rand = rand;
            this.lockTop = lockTop;
            this.lockBottom = lockBottom;
            this.lockRight = lockRight;
            this.lockLeft = lockLeft;
            PatternsSingleton.Patterns = analysisResult.Patterns;
            topPatterns = analysisResult.TopPatterns;
            bottomPatterns = analysisResult.BottomPatterns;
            rightPatterns = analysisResult.RightPatterns;
            leftPatterns = analysisResult.LeftPatterns;
            Reset();
        }

        public void Reset()
        {
            wave = new WaveCell[width - n + 1, height - n + 1];
            
            for (int x = 0; x <= width - n; x++)
            {
                for (int y = 0; y <= height - n; y++)
                {
                    wave[x, y] = new(n);
                    if (y == 0 && lockTop)
                    {
                        wave[x, y].Update(topPatterns);
                    }
                    if (y == height - n && lockBottom)
                    {
                        wave[x, y].Update(bottomPatterns);
                    }
                    if (x == 0 && lockLeft)
                    {
                        wave[x, y].Update(leftPatterns);
                    }
                    if (x == width - n && lockRight)
                    {
                        wave[x, y].Update(rightPatterns);
                    }
                }
            }

            CollapsesLeft = (width - n + 1) * (height - n + 1);
            ObserveAll();
        }

        public int Collapse()
        {
            CollapsesLeft--;
            wave[lowestEntropyCords.x, lowestEntropyCords.y].Collapse(rand);
            Observe(lowestEntropyCords.x, lowestEntropyCords.y);
            if (lowestEntropy == 0)
            {
                throw new NoPossibleCollapseException($"Entropy equal to 0 at {lowestEntropyCords.x},{lowestEntropyCords.y}");
            }
            return CollapsesLeft;
        }

        private void ObserveAll()
        {
            for (int x = 0; x <= width - n; x++)
            {
                for (int y = 0; y <= height - n; y++)
                {
                    Observe(x, y);
                }
            }
        }

        private void Observe(int x, int y)
        {
            lowestEntropy = int.MaxValue;
            Queue<(int, int)> observationQueue = new([(x, y)]);
            HashSet<(int, int)> inQueue = new([(x, y)]);

            while (observationQueue.Count > 0)
            {
                (x, y) = observationQueue.Dequeue();
                inQueue.Remove((x, y));

                int startI = Math.Max(x - n + 1, 0);
                int endI = Math.Min(x + n, width - n + 1);

                int startJ = Math.Max(y - n + 1, 0);
                int endJ = Math.Min(y + n, height - n + 1);

                for (int i = startI; i < endI; i++)
                for (int j = startJ; j < endJ; j++)
                {
                    int xOffset = i - x;
                    int yOffset = j - y;
                    if ((xOffset == 0 && yOffset == 0) || wave[i, j].HasCollapsed)
                    {
                        continue;
                    }
                    int initialEntropy = wave[i, j].Entropy;
                    CoefficienceSet boundingSet = wave[x, y].GetOverlappingPatterns(xOffset, yOffset);
                    wave[i, j].Update(boundingSet);
                    if (wave[i, j].Entropy != initialEntropy && !inQueue.Contains((i, j)))
                    {
                        observationQueue.Enqueue((i, j));
                        inQueue.Add((i, j));
                    }
                }
            }
            FindLowestEntropy();
        }

        private void FindLowestEntropy()
        {
            lowestEntropy = int.MaxValue;
            for (int i = 0; i < width - n + 1; i++)
            {
                for (int j = 0; j < height - n + 1; j++)
                {
                    if (!wave[i,j].HasCollapsed && wave[i,j].Entropy < lowestEntropy)
                    {
                        lowestEntropy = wave[i, j].Entropy;
                        lowestEntropyCords = (i, j);
                    }
                }
            }
        }

        public Color GetPixel(int x, int y)
        {
            int rSum = 0, gSum = 0, bSum = 0;
            int count = 0;

            for (int i = Math.Max(x - n + 1, 0); i <= Math.Min(x,  width - n); i++)
            {
                for (int j = Math.Max(y - n + 1, 0); j <= Math.Min(y, height - n); j++)
                {
                    count++;
                    int xOffset = x - i;
                    int yOffset = y - j;
                    int pixelValue = wave[i, j].GetPixelValue(xOffset, yOffset);
                    rSum += pixelValue >> 16 & 0xFF;
                    gSum += pixelValue >> 8 & 0xFF;
                    bSum += pixelValue & 0xFF;
                }
            }

            rSum /= count; gSum /= count; bSum /= count;
            Color result = Color.FromArgb(rSum, gSum, bSum);
            return result;
        }
    }
}

