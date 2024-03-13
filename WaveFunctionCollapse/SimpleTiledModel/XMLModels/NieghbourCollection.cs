using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    internal class NeighbourCollection
    {
        [XmlElement("Neighbour", IsNullable = false)]
        public List<Neighbour> Neighbours { get; set; }

        public NeighbourCollection Rotate()
        {
            List<Neighbour> rotatedNeighbours = new List<Neighbour>();
 
            foreach(Neighbour neighbour in Neighbours)
            {
                rotatedNeighbours.Add(neighbour.Rotate());
            }

            return new NeighbourCollection
            {
                Neighbours = rotatedNeighbours
            };
        }

        public NeighbourCollection Reflect()
        {
            List<Neighbour> reflectedNeighbours = new List<Neighbour>();
            
            foreach(Neighbour neighbour in Neighbours)
            {
                reflectedNeighbours.Add(neighbour.Reflect());
            }

            return new NeighbourCollection
            {
                Neighbours = reflectedNeighbours
            }; 
        }
    }
}
