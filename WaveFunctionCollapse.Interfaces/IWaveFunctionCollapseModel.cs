using SkiaSharp;

namespace WaveFunctionCollapse.Interfaces
{
    public interface IWaveFunctionCollapseModel
    {
        SKBitmap Image { get; }
        int CollapsesLeft { get; }
        void Reset();
        void CollapseNTimes(int n);
        void CollapseAll();
        int Collapse();
    }
}
