namespace OverlappingModel
{
    internal class WaveCell
    {
        public bool HasCollapsed { get; private set; } = false;
        public int Entropy => HasCollapsed ? -1 : patterns.Count;

        public CoefficienceSet Patterns => patterns;

        private CoefficienceSet patterns;
        private CoefficienceSet[,] overlapping;
        private Pattern[] allPatterns;
        private int[] frequencies;
        private readonly int n;
        public Pattern CollapsedValue { get; private set; }

        public WaveCell(Pattern[] allPatterns, int[] frequencies, int n)
        {
            this.frequencies = frequencies;
            this.allPatterns = allPatterns;
            int size = allPatterns.Length;
            this.n = n;
            patterns = new CoefficienceSet(size);
            patterns.SetAll(allPatterns);
            overlapping = new CoefficienceSet[2 * n - 1, 2 * n - 1];

            UpdateAllOverlappingPatterns();
        }

        public void Collapse(Random rand)
        {
            if (HasCollapsed)
            {
                throw new InvalidOperationException("Wave cell has already collapsed!");
            }

            List<Pattern> patternsToChoose = patterns.GetPatterns(allPatterns);

            int cumulativeSum = patterns.GetCumulativeSum(frequencies);
            double randValue = rand.NextDouble() * cumulativeSum;

            int currentSum = 0;
            foreach (Pattern pattern in patternsToChoose)
            {
                currentSum += pattern.Frequency;
                if (currentSum >= randValue)
                {
                    CollapsedValue = pattern;
                    overlapping = CollapsedValue.OverlappingPatterns;
                    HasCollapsed = true;
                    break;
                }
            }
        }

        public void Update(CoefficienceSet forcedPatterns)
        {
            if (HasCollapsed)
            {
                throw new InvalidOperationException("Wave cell has already collapsed!");
            }
            int oldEntropy = Entropy;
            patterns.IntersectWith(forcedPatterns);
            if (oldEntropy != Entropy)
            {
                UpdateAllOverlappingPatterns();
            }
        }

        public CoefficienceSet GetOverlappingPatterns(int xOffset, int yOffset)
        {
            int x = xOffset + n - 1;
            int y = yOffset + n - 1;

            return overlapping[x, y];
        }

        private void UpdateAllOverlappingPatterns()
        {
            for (int i = -n + 1; i <= n - 1; i++)
            {
                for (int j = -n + 1; j <= n - 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    UpdateOverlappingPatterns(i, j);
                }
            }
        }

        private void UpdateOverlappingPatterns(int xOffset, int yOffset)
        {
            int x = xOffset + n - 1;
            int y = yOffset + n - 1;

            CoefficienceSet newPatterns = new(allPatterns.Length);
            List<Pattern> consistingPatterns = patterns.GetPatterns(allPatterns);
            foreach (var pattern in consistingPatterns)
            {
                newPatterns.UnionWith(pattern.GetOverlappingPatterns(xOffset, yOffset));
            }
            overlapping[x, y] = newPatterns;
        }

        public int GetPixelValue(int x, int y)
        {
            if (Entropy == 0)
            {
                return 0x00FF00;
            }

            if (HasCollapsed)
            {
                return CollapsedValue[x, y];
            }

            List<Pattern> consistingPatterns = patterns.GetPatterns(allPatterns);
            int r = 0, g = 0, b = 0;
            foreach(Pattern pattern in consistingPatterns)
            {
                int pixel = pattern[x, y];
                r += pixel >> 16 & 0xFF;
                g += pixel >> 8 & 0xFF;
                b += pixel & 0xFF;
            }

            r /= Entropy;
            g /= Entropy;
            b /= Entropy;

            int result = (r << 16) | (g << 8) | b;
            return result;
        }
    }
}
