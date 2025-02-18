namespace OverlappingModel
{
    internal class WaveCell
    {
        public bool HasCollapsed { get; private set; } = false;
        public int Entropy => HasCollapsed ? -1 : patterns.Count;

        private CoefficienceSet patterns;
        private CoefficienceSet[,] overlapping;
        private readonly int n;
        public Pattern CollapsedValue { get; private set; }

        public WaveCell(int n)
        {
            var size = PatternsSingleton.Patterns.Length;
            this.n = n;
            patterns = new CoefficienceSet(size);
            patterns.SetAll();
            overlapping = new CoefficienceSet[2 * n - 1, 2 * n - 1];
            for (int i = 0; i < 2 * n - 1; i++)
            {
                for (int j = 0; j < 2 * n - 1; j++)
                {
                    overlapping[i, j] = new CoefficienceSet(size);
                    overlapping[i, j].SetAll();
                }
            }
        }

        public void Collapse(Random rand)
        {
            if (HasCollapsed)
            {
                throw new InvalidOperationException("Wave cell has already collapsed!");
            }

            var ids = patterns.GetIds();

            int cumulativeSum = GetCumulativeSum(ids);
            double randValue = rand.NextDouble() * cumulativeSum;

            int currentSum = 0;
            foreach (var id in ids)
            {
                var pattern = PatternsSingleton.Patterns[id];
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

        private int GetCumulativeSum(List<int> ids)
        {
            int sum = 0;
            foreach (int id in ids)
            {
                sum += PatternsSingleton.Patterns[id].Frequency;
            }

            return sum;
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
            List<int> ids = patterns.GetIds();
            var patternList = ids.Select(id => PatternsSingleton.Patterns[id]).ToList();

            for (int i = -n + 1; i <= n - 1; i++)
                for (int j = -n + 1; j <= n - 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    UpdateOverlappingPatterns(i, j, patternList);
                }
        }

        private void UpdateOverlappingPatterns(int xOffset, int yOffset, List<Pattern> patternList)
        {
            int x = xOffset + n - 1;
            int y = yOffset + n - 1;

            CoefficienceSet newPatterns = new(PatternsSingleton.Patterns.Length);
            foreach (var pattern in patternList)
            {
                newPatterns.UnionWith(pattern.GetOverlappingPatterns(xOffset, yOffset));
            }
            overlapping[x, y] = newPatterns;
        }

        public int GetPixelValue(int x, int y)
        {
            if (Entropy == 0)
            {
                return 0x00FF00; // We return green color if entropy is 0 for debugging
            }

            if (HasCollapsed)
            {
                return CollapsedValue[x, y];
            }

            List<int> ids = patterns.GetIds();
            int r = 0, g = 0, b = 0;
            foreach(var id in ids)
            {
                var pattern = PatternsSingleton.Patterns[id];
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
