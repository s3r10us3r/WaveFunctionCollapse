using System.Numerics;

namespace OverlappingModel
{
    internal class CoefficienceSet
    {
        public int Count { get; private set; }
        private ulong[] coefficients;

        public CoefficienceSet(int size)
        {
            int arraySize = (size / 64) + 1;
            coefficients = new ulong[arraySize];
            UpdateCount();
        }

        public void SetAll(Pattern[] patterns)
        {
            foreach (var pattern in patterns)
            {
                Add(pattern.ID);
            }
            Count = patterns.Length;
        }

        public void Add(int id)
        {
            int i = id / 64;
            int bit = id % 64;

            ulong original = coefficients[i];
            coefficients[i] |= 1UL << bit;
            if (original != coefficients[i])
            {
                Count++;
            }
        }

        public void Remove(int id)
        {
            int i = id / 64;
            int bit = id % 64;

            ulong original = coefficients[i];
            coefficients[i] &= ~(1UL << bit);
            if (original != coefficients[i])
            {
                Count--;
            }
        }

        public bool Contains(int id)
        {
            int i = id / 64;
            int bit = id % 64;

            return (coefficients[i] & (1UL << bit)) != 0;
        }

        public void IntersectWith(CoefficienceSet set)
        {
            for (int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] &= set.coefficients[i];
            }
            UpdateCount();
        }

        public void UnionWith(CoefficienceSet set)
        {
            for (int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] |= set.coefficients[i];
            }
            UpdateCount();
        }

        public List<Pattern> GetPatterns(Pattern[] allPatterns)
        {
            List<Pattern> result = new();
            for (int i = 0; i < allPatterns.Length; i++)
            {
                if (Contains(i))
                {
                    result.Add(allPatterns[i]);
                }
            }
            return result;
        }

        public int GetCumulativeSum(int[] frequencies)
        {
            int sum = 0;
            for (int i = 0; i < frequencies.Length; i++)
            {
                if (Contains(i))
                {
                    sum += frequencies[i];
                }
            }
            return sum;
        }

        private void UpdateCount()
        {
            int count = 0;
            for (int i = 0; i < coefficients.Length; i++)
            {
                count += BitOperations.PopCount(coefficients[i]);
            }
            Count = count;
        }
    }
}
