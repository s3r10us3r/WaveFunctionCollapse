using System.Drawing;

namespace OverlappingModel
{
    public class OverlappingModelBuilder
    {
        private int n;
        private int? seed = null;
        private Bitmap? bitmap;
        private int width;
        private int height;
        private bool rotationsEnabled = false;
        private bool reflectionsEnabled = false;
        private bool lockTop = false;
        private bool lockBottom = false;
        private bool lockLeft = false;
        private bool lockRight = false;

        public void SetSeed(int seed) => this.seed = seed;
        public void SetN(int n) => this.n = n;
        public void SetBitmap(Bitmap bitmap) => this.bitmap = bitmap;
        public void SetHeight(int height) => this.height = height;
        public void SetWidth(int width) => this.width = width;
        public void SetRotationsEnabled(bool enable) => rotationsEnabled = enable;
        public void SetReflectionsEnabled(bool enable) => reflectionsEnabled = enable;
        public void LockTop(bool isLock) => lockTop = isLock; 
        public void LockBottom(bool isLock) => lockBottom = isLock; 
        public void LockLeft(bool isLock) => lockLeft = isLock; 
        public void LockRight(bool isLock) => lockRight = isLock; 

        public OverlappingModel Build()
        {
            if (n < 0 || bitmap is null || width < n || height < n)
            {
                throw new ArgumentException("N, bitmap, width and height must be explicitly set, height and width must be at least n");
            }
            Random rand = seed is null ? new() : new((int)seed);
            return new OverlappingModel(n, bitmap, width, height, rotationsEnabled, reflectionsEnabled, lockTop, lockBottom, lockLeft, lockRight, rand);
        }
    }
}

