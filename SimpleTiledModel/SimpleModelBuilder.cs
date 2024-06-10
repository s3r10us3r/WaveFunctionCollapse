using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleModel
{
    public class SimpleModelBuilder
    {
        private string tilesetPath;
        private string tilesetName;
        private int width;
        private int height;
        private int? seed;

        public void SetTilesetPath(string tilesetPath)
        {
            this.tilesetPath = Path.GetDirectoryName(tilesetPath);
            tilesetName = Path.GetFileName(tilesetPath);
        }

        public void SetWidth(int width) => this.width = width;
        public void SetHeight(int height) => this.height = height;
        public void SetSeed(int? seed) => this.seed = seed;

        public SimpleTiledModel Build()
        {
            if (tilesetPath is null || tilesetName is null || width == 0 || height == 0)
            {
                throw new InvalidOperationException("Not all required attributes were set!");
            }

            if (seed is not null)
            {
                int iSeed = (int)seed;
                return new SimpleTiledModel(tilesetPath, tilesetName, width, height, iSeed);
            }

            return new SimpleTiledModel(tilesetPath, tilesetName, width, height);
        }
    }
}
