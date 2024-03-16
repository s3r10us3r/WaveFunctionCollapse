using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.WaveFunctionCollapse.SimpleTiledModel
{
    public class Tile
    {
        public HashSet<int> TopNeighbors { get; set; }
        public HashSet<int> RightNeighbors { get; set; }
        public HashSet<int> BottomNeighbors { get; set; }
        public HashSet<int> LeftNeighbors { get; set; }

        public readonly int[] bitMap;

        public Tile(int[] bitMap, HashSet<int> topNeighbors, HashSet<int> rightNeighbors, HashSet<int> leftNeighbors, HashSet<int> bottomNeighbors)
        {
            this.bitMap = bitMap;
            TopNeighbors = topNeighbors;
            RightNeighbors = rightNeighbors;
            LeftNeighbors = leftNeighbors;
            BottomNeighbors = bottomNeighbors;
        }
    }
}
