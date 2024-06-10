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

        private List<Pattern> topPatterns;
        private List<Pattern> bottomPatterns;
        private List<Pattern> leftPatterns;
        private List<Pattern> rightPatterns;

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
            int idCounter = 0;
            List<Pattern> patterns = new();
            for (int y = 0; y <= bitmap.Height - n; y++)
            {
                for (int x = 0; x <= bitmap.Width - n; x++)
                {
                    int[,] pixels = CutBitmap(x, y);
                    Pattern pattern = new(n, pixels, idCounter++);

                    int ind = patterns.FindIndex(p => p.Equals(pattern));
                    if (ind != -1)
                    {
                        patterns[ind].Frequency++;
                        idCounter--;
                        pattern = patterns[ind];
                    }
                    else
                    {
                        patterns.Add(pattern);
                    }

                    if (x == 0)
                        AddToPatterns(leftPatterns, pattern);
                    if (y == 0)
                        AddToPatterns(topPatterns, pattern);
                    if (x == bitmap.Width - n)
                        AddToPatterns(rightPatterns, pattern);
                    if (y == bitmap.Height - n)
                        AddToPatterns(bottomPatterns, pattern);
                }
            }

            if (rotationEnabled)
            {
                for (int i = 0; i < patterns.Count; i++)
                {
                    Pattern oPattern = patterns[i];
                    for (int j = 0; j < 3; j++)
                    {
                        Pattern pattern = oPattern.Rotate(idCounter++);
                        int ind = patterns.FindIndex(p => p.Equals(pattern));
                        if (ind != -1)
                        {
                            patterns[ind].Frequency += pattern.Frequency;
                            idCounter--;
                        }
                        else
                        {
                            patterns.Add(pattern);
                            if (topPatterns.Contains(oPattern))
                                AddToPatterns(topPatterns, pattern);
                            if (bottomPatterns.Contains(oPattern))
                                AddToPatterns(bottomPatterns, pattern);
                            if (leftPatterns.Contains(oPattern))
                                AddToPatterns(leftPatterns, pattern);
                            if (rightPatterns.Contains(oPattern))
                                AddToPatterns(rightPatterns, pattern);
                        }
                    }

                   
                }
            }

            if (reflectionEnabled)
            {
                for (int i = 0; i < patterns.Count; i++)
                {
                    Pattern oPattern = patterns[i];
                    Pattern pattern = oPattern.Reflect(idCounter++);
                    int ind = patterns.FindIndex(p => p.Equals(pattern));
                    if (ind != -1)
                    {
                        patterns[ind].Frequency += pattern.Frequency;
                        idCounter--;
                    }
                    else
                    {
                        patterns.Add(pattern);
                        if (topPatterns.Contains(oPattern))
                            AddToPatterns(topPatterns, pattern);
                        if (bottomPatterns.Contains(oPattern))
                            AddToPatterns(bottomPatterns, pattern);
                        if (leftPatterns.Contains(oPattern))
                            AddToPatterns(rightPatterns, pattern);
                        if (rightPatterns.Contains(oPattern))
                            AddToPatterns(leftPatterns, pattern);
                    }
                }
            }

            Pattern[] allInstances = patterns.ToArray();
            int[] frequencies = new int[allInstances.Length];
            for (int i = 0; i < allInstances.Length; i++)
            {
                Pattern pattern = allInstances[i];
                pattern.MatchPatterns(patterns);
                frequencies[i] = pattern.Frequency;
            }

            return new ImageAnalysisResult(allInstances, frequencies, ListToCSet(topPatterns, allInstances.Length), ListToCSet(leftPatterns, allInstances.Length), ListToCSet(rightPatterns, allInstances.Length), ListToCSet(bottomPatterns, allInstances.Length));
        }
        private void AddToPatterns(List<Pattern> patterns, Pattern pattern)
        {
            if (!patterns.Contains(pattern))
            {
                patterns.Add(pattern);
            }
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

        private CoefficienceSet ListToCSet(List<Pattern> patterns, int size)
        {
            CoefficienceSet cset = new(size);
            cset.SetAll(patterns.ToArray());

            return cset;
        }

        
    }
}

