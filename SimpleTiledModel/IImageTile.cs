using System.Drawing;

namespace SimpleModel
{
    internal interface IImageTile
    {
        public HashSet<Tile> TopNeighbors { get; }
        public HashSet<Tile> RightNeighbors { get; }
        public HashSet<Tile> BottomNeighbors { get; }
        public HashSet<Tile> LeftNeighbors { get; }
        public Bitmap GetImage(int n);
    }
}
