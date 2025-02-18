using System.Drawing;
using System.Drawing.Imaging;

namespace OverlappingModel
{
    internal class ImageAnalyzer
    {
        private Bitmap bitmap;
        private bool rotationEnabled;
        private bool reflectionEnabled;
        private int n;
        private PatternSet patternSet = new();

        private HashSet<int> topPatterns;
        private HashSet<int> bottomPatterns;
        private HashSet<int> leftPatterns;
        private HashSet<int> rightPatterns;

        public ImageAnalyzer(Bitmap bitmap, bool rotationEnabled, bool reflectionEnabled, int n)
        {
            this.rotationEnabled = rotationEnabled;
            this.reflectionEnabled = reflectionEnabled;
            this.bitmap = bitmap;
            this.n = n;
            topPatterns= new();
            bottomPatterns = new();
            leftPatterns = new();
            rightPatterns = new();
        }
        
        public ImageAnalysisResult Analyze()
        {
            List<Pattern> patterns = new();
            for (int y = 0; y <= bitmap.Height - n; y++)
            {
                for (int x = 0; x <= bitmap.Width - n; x++)
                {
                    int[,] pixels = CutBitmap(x, y);
                    Pattern pattern = new(n, pixels);
                    AddToPatterns(pattern, x, y);

                    if (reflectionEnabled)
                    {
                        HandleReflection(pattern, x, y);
                    }

                    if (rotationEnabled)
                    {
                        HandleRotation(pattern, x, y);
                    }
                }
            }

            var allInstances = patternSet.GetPatterns();
            var frequencies = patternSet.GetFrequencies(); 


            return new ImageAnalysisResult(allInstances, frequencies, ListToCSet(topPatterns, allInstances.Length), ListToCSet(leftPatterns, allInstances.Length), ListToCSet(rightPatterns, allInstances.Length), ListToCSet(bottomPatterns, allInstances.Length));
        }

        private void HandleRotation(Pattern pattern, int x, int y)
        {
            for (int i = 0; i < 3; i++)
            {
                pattern = pattern.Rotate();
                AddToPatterns(pattern, x, y);
                if (reflectionEnabled)
                    HandleReflection(pattern, x, y);
            }
        }

        private void HandleReflection(Pattern pattern, int x, int y)
        {
            pattern = pattern.Reflect();
            AddToPatterns(pattern, bitmap.Width - x - n, y);
        }

        private void AddToPatterns(Pattern pattern, int x, int y)
        {
            var ind = patternSet.TryAddPattern(pattern);

            if (x == 0)
                leftPatterns.Add(ind);
            if (y == 0)
                topPatterns.Add(ind);
            if (x == bitmap.Width - n)
                rightPatterns.Add(ind);
            if (y == bitmap.Height - n)
                bottomPatterns.Add(ind);
        }

        private int[,] CutBitmap(int x, int y)
        {
            int[,] cut = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    cut[i, j] = bitmap.GetPixel(x + i, y + j).ToArgb();
                }
            }
            return cut;
        }

        private CoefficienceSet ListToCSet(IEnumerable<int> ids, int size)
        {
            CoefficienceSet cset = new(size);
            cset.SetAll(ids);

            return cset;
        }
    }
}

