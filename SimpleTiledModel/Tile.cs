using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleModel.XMLModels;

namespace SimpleModel
{
    public class Tile : IImageTile
    {
        public HashSet<Tile> TopNeighbors { get; set; }
        public HashSet<Tile> RightNeighbors { get; set; }
        public HashSet<Tile> BottomNeighbors { get; set; }
        public HashSet<Tile> LeftNeighbors { get; set; }

        public readonly int[] bitMap;

        public readonly int tID;
        public readonly string name;
        public readonly double weight;

        public Tile(TileModel tileModel)
        {
            if (tileModel is null)
            {
                throw new ArgumentNullException("tileModel can not be null!");
            }

            if (tileModel.Name is null || tileModel.Bitmap is null)
            {
                throw new ArgumentException("tileModel must be initialized!");
            }

            name = tileModel.Name;
            tID = tileModel.GetTransformationID();
            bitMap = tileModel.Bitmap;
            weight = tileModel.Weight;
        }

        public Bitmap GetImage(int n)
        {
            Bitmap bitmap = new Bitmap(n, n);


            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    Color color = Color.FromArgb(bitMap[x * n + y]);
                    bitmap.SetPixel(y, x, color);
                }
            }

            return bitmap;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, tID);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Tile)
            {
                return false;
            }
            Tile tile = (Tile)obj;
            return tile.name == name && tile.tID == tID;
        }
    }
}
