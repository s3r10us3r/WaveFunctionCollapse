using SkiaSharp;
using WaveFunctionCollapse.Interfaces;

namespace OverlappingModel
{
    public class OverlappingModel : IWaveFunctionCollapseModel
    {
        public SKBitmap Image => wave.Image;
        
        public int CollapsesLeft => wave.CollapsesLeft;
        private Wave wave;
        
        internal OverlappingModel(int n, SKBitmap bitmap, int width, int height, bool rotationsEnabled, bool reflectionsEnabled, bool lockTop, bool lockBottom, bool lockLeft, bool lockRight, Random rand)
        {
            var analyzer = new ImageAnalyzer(bitmap, rotationsEnabled, reflectionsEnabled, n);
            wave = new Wave(width, height, n, rand, analyzer.Analyze(), lockTop, lockBottom, lockRight, lockLeft);
        }

        public void CollapseNTimes(int n)
        {
            while (n-- > 0 && Collapse() > 0);
        }

        public void CollapseAll()
        {
            int collapes = 0;
            while (Collapse() > 0)
            {
                collapes++;
            }
        }

        public int Collapse()
        {
            if (CollapsesLeft > 0)
            {
                wave.Collapse();
            }
            return CollapsesLeft;
        }

        public void Reset()
        {
            wave.Reset();
        }

    }
}
