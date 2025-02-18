using SkiaSharp;

namespace OverlappingModel
{
    public class OverlappingModelBuilder
    {
        public int N { get; set; } = 0;
        public int? Seed { get; set; } = null;
        public SKBitmap? Bitmap { get; set; } = null;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public bool RotationsEnabled { get; set; } = false;
        public bool ReflectionsEnabled { get; set; } = false;
        public bool LockTop { get; set; } = false;
        public bool LockBottom { get; set; } = false;
        public bool LockLeft { get; set; } = false;
        public bool LockRight { get; set; } = false;

        public OverlappingModel Build()
        {
            if (N < 0 || Bitmap is null || Width < N || Height < N)
            {
                throw new ArgumentException("N, bitmap, width and height must be explicitly set, height and width must be at least n");
            }
            Random rand = Seed is null ? new() : new((int)Seed);
            return new OverlappingModel(N, Bitmap, Width, Height, RotationsEnabled, ReflectionsEnabled, LockTop, LockBottom, LockLeft, LockRight, rand);
        }
    }
}

