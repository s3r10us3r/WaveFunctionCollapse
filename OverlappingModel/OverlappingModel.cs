using System.Drawing;
using WaveFunctionCollapse.Interfaces;

namespace OverlappingModel
{
    public class OverlappingModel : IWaveFunctionCollapseModel
    {
        public Bitmap Image => wave.Image;
        
        public int CollapsesLeft => wave.CollapsesLeft;
        private Wave wave;
        private int width;
        private int height;
        
        internal OverlappingModel(int n, Bitmap bitmap, int width, int height, bool rotationsEnabled, bool reflectionsEnabled, bool lockTop, bool lockBottom, bool lockLeft, bool lockRight, Random rand)
        {
            this.width = width;
            this.height = height;
            ImageAnalyzer analyzer = new ImageAnalyzer(bitmap, rotationsEnabled, reflectionsEnabled, n);
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
