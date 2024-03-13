using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.WaveFunctionCollapse
{
    public class Tile
    {
        //this list contains all tile instances according to their ids
        public static List<Tile> list = new List<Tile>();
        private static int idCounter = 0;

        public static void ClearTileList()
        {
            list = new List<Tile>();
            idCounter = 0;
        }

        public HashSet<int> TopNeighbors { get; set; }
        public HashSet<int> RightNeighbors { get; set; }
        public HashSet<int> BottomNeighbors { get; set; }
        public HashSet<int> LeftNeighbors { get; set; }

        public readonly int id;
        public readonly int[] bitMap;

        public Tile(int[] bitMap)
        {
            id = idCounter++;
            this.bitMap = bitMap;
            list.Add(this);
        }


    }
}
