using System.Drawing;


namespace WaveFunctionCollapse.Interfaces
{
    public interface IWaveFunctionCollapseModel
    {
        Bitmap Image { get; }
        int CollapsesLeft { get; }
        void Reset();
        void CollapseNTimes(int n);
        void CollapseAll();
        int Collapse();
    }
}
