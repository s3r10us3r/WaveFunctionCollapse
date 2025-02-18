using SkiaSharp;

namespace OverlappingModel
{
    public static class SkExtensions
    {
        public static int ToArgb(this SKColor color)
        {
            return (color.Alpha << 24) | (color.Red << 16) | (color.Green << 8) | color.Blue;
        }
    }
}
