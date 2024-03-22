using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    public class NeighbourCollection
    {
        public enum Side
        {
            TOP, RIGHT, BOTTOM, LEFT
        }

        [XmlElement("Neighbour", IsNullable = false)]
        public List<Neighbour> Neighbours { get; set; }

        public void Init(Dictionary<string, string> nameSymmetry, Side side)
        {
            List<Neighbour> newNeighbours = new List<Neighbour>(Neighbours);

            foreach(Neighbour neighbour in Neighbours)
            {
                string symmetryType = nameSymmetry[neighbour.Name];
                switch (symmetryType)
                {
                    case "X":
                        {
                            Neighbour transformedNeighbour = neighbour;
                            for(int i = 0; i < 3; i++)
                            {
                                transformedNeighbour = transformedNeighbour.Rotate();
                                newNeighbours.Add(transformedNeighbour);
                            }
                            transformedNeighbour = neighbour.Reflect();
                            for(int i = 0; i < 4; i++)
                            {
                                newNeighbours.Add(transformedNeighbour);
                                transformedNeighbour = transformedNeighbour.Rotate();
                            }
                        }
                        break;
                    case "I":
                        {
                            Neighbour rotatedNeighbour = neighbour.Rotate().Rotate();
                            newNeighbours.Add(rotatedNeighbour);
                            newNeighbours.Add(neighbour.Reflect());
                            newNeighbours.Add(rotatedNeighbour.Reflect());
                        }
                        break;
                    case "T":
                        {
                            if ((neighbour.TransformationIndex % 2 == 0 && (side == Side.RIGHT || side == Side.LEFT)) || neighbour.TransformationIndex % 2 == 0 && (side == Side.TOP || side == Side.BOTTOM))
                            {
                                Neighbour rotatedNeighbour = neighbour.Rotate().Rotate();
                                newNeighbours.Add(rotatedNeighbour);
                                newNeighbours.Add(neighbour.Reflect());
                                newNeighbours.Add(rotatedNeighbour.Reflect());
                            }
                            break;
                        }
                    case "L":
                        {
                            int[] pointySide = {1, 2, 6, 7};
                            int[] flatSide = { 0, 3, 4, 5 };
                            int[] actualSide = pointySide.Contains(neighbour.TransformationIndex) ? pointySide : flatSide;

                            foreach(int i in actualSide)
                            {
                                if (neighbour.TransformationIndex != i)
                                {
                                    newNeighbours.Add(new Neighbour
                                    {
                                        Name = neighbour.Name,
                                        TransformationIndex = i
                                    });
                                }
                            }
                        }
                        break;
                    case "\\":
                        {
                            int[] facingLeft = {0, 2, 5, 7};
                            int[] facingRight = {1, 3, 4, 6};
                            int[] actualSide = facingLeft.Contains(neighbour.TransformationIndex) ? facingLeft : facingRight;

                            foreach(int i in actualSide)
                            {
                                if (neighbour.TransformationIndex != i)
                                {
                                    newNeighbours.Add(new Neighbour
                                    {
                                        Name = neighbour.Name,
                                        TransformationIndex = i
                                    });
                                }
                            }
                        }
                        break;
                }
            }

            Neighbours = newNeighbours;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach(Neighbour n in Neighbours)
            {
                sb.Append(n.Name);
                sb.Append(' ');
                sb.Append(n.TransformationIndex);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public NeighbourCollection Rotate()
        {
            List<Neighbour> rotatedNeighbours = new List<Neighbour>();

            foreach (Neighbour neighbour in Neighbours)
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

            foreach (Neighbour neighbour in Neighbours)
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
