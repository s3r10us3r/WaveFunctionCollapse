using System.Numerics;

namespace OverlappingModel
{
    internal class CoefficienceSet
    {
        public int Count => _coefficients.Sum(BitOperations.PopCount);

        private readonly List<ulong> _coefficients;
        private int _size;

        public CoefficienceSet(int size)
        {
            int arraySize = (size / 64) + 1;
            _coefficients = Enumerable.Repeat(0UL, arraySize).ToList();
            _size = size;
        }

        public void SetAll()
        {
            for (int i = 0; i < _size; i++)
                Add(i);
        }

        public void SetAll(IEnumerable<int> ids)
        {
            foreach (var id in ids)
                Add(id);
        }

        public void Add(int id)
        {
            Pad(id);

            int i = id / 64;
            int bit = id % 64;

            ulong original = _coefficients[i];
            _coefficients[i] |= 1UL << bit;
        }

        private void Pad(int size)
        {
            if (size <= _size)
                return;
            while ((_coefficients.Count - 1) * 64 < size)
            {
                _coefficients.Add(0);
            }
            _size = size;
        }

        public bool Contains(int id)
        {
            int i = id / 64;
            int bit = id % 64;

            return (_coefficients[i] & (1UL << bit)) != 0;
        }

        public void IntersectWith(CoefficienceSet set)
        {
            PadTwoSets(set);
            for (int i = 0; i < _coefficients.Count; i++)
            {
                _coefficients[i] &= set._coefficients[i];
            }
        }

        public void UnionWith(CoefficienceSet set)
        {
            PadTwoSets(set);
            for (int i = 0; i < _coefficients.Count; i++)
            {
                _coefficients[i] |= set._coefficients[i];
            }
        }

        private void PadTwoSets(CoefficienceSet set)
        {
            int maxSize = Math.Max(set._size, _size);
            Pad(maxSize);
            set.Pad(maxSize);
        }

        public List<int> GetIds()
        {
            List<int> result = new();
            for (int i = 0; i < _size; i++)
            {
                if (Contains(i))
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}
