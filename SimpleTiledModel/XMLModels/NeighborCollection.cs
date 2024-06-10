using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SimpleModel.XMLModels
{
    public class NeighborCollection
    {
        private bool initialized = false;


        [XmlElement("Neighbour", IsNullable = false)]
        public List<Neighbor> Neighbors { get; set; }

        public void Init(Dictionary<string, string> nameSymmetry)
        {
            if (initialized)
            {
                throw new InvalidOperationException("Neighbour collection can be initialized only once");
            }

            List<Neighbor> newNeighbors = new List<Neighbor>(Neighbors);

            foreach (Neighbor neighbor in Neighbors)
            {
                string symmetryType;
                bool isInDict = nameSymmetry.TryGetValue(neighbor.Name, out symmetryType);
                if (!isInDict)
                {
                    throw new XMLException($"No tile named {neighbor.Name} in the tile set");
                }

                switch (symmetryType)
                {
                    case "X":
                        {
                            Neighbor transformedNeighbor = neighbor;
                            for (int i = 0; i < 3; i++)
                            {
                                transformedNeighbor = transformedNeighbor.Rotate();
                                newNeighbors.Add(transformedNeighbor);
                            }
                            transformedNeighbor = neighbor.Reflect();
                            for (int i = 0; i < 4; i++)
                            {
                                newNeighbors.Add(transformedNeighbor);
                                transformedNeighbor = transformedNeighbor.Rotate();
                            }
                        }
                        break;

                    case "I":
                        {
                            Neighbor rotatedNeighbor = neighbor.Rotate().Rotate();
                            newNeighbors.Add(rotatedNeighbor);
                            newNeighbors.Add(neighbor.Reflect());
                            newNeighbors.Add(rotatedNeighbor.Reflect());
                        }
                        break;

                    case "T":
                        {
                            if (neighbor.TransformationIndex % 2 == 0)
                            {
                                newNeighbors.Add(neighbor.Reflect());
                            }
                            else
                            {
                                newNeighbors.Add(neighbor.Rotate().Rotate().Reflect());
                            }
                        }
                        break;

                    case "L":
                        {
                            //bottomPoint means that the pointy side of 'L' is facing this direction for example 'L' without any transformations (transformationID 0) is in rightPoint and topPoint
                            int[] equivalency_array = [5, 6, 7, 4, 3, 0, 1, 2];

                            newNeighbors.Add(new Neighbor
                            {
                                Name = neighbor.Name,
                                TransformationIndex = equivalency_array[neighbor.TransformationIndex]
                            });
                        }
                        break;

                    case "\\":
                        {
                            int[] facingLeft = { 0, 2, 5, 7 };
                            int[] facingRight = { 1, 3, 4, 6 };
                            int[] actualSide = facingLeft.Contains(neighbor.TransformationIndex) ? facingLeft : facingRight;

                            foreach (int i in actualSide)
                            {
                                if (neighbor.TransformationIndex != i)
                                {
                                    newNeighbors.Add(new Neighbor
                                    {
                                        Name = neighbor.Name,
                                        TransformationIndex = i
                                    });
                                }
                            }
                        }
                        break;
                }
            }

            Neighbors = newNeighbors;

            initialized = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (Neighbor n in Neighbors)
            {
                sb.Append(n.Name);
                sb.Append(' ');
                sb.Append(n.TransformationIndex);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        public NeighborCollection Rotate()
        {
            List<Neighbor> rotatedNeighbors = new List<Neighbor>();

            foreach (Neighbor neighbor in Neighbors)
            {
                rotatedNeighbors.Add(neighbor.Rotate());
            }

            return new NeighborCollection
            {
                Neighbors = rotatedNeighbors
            };
        }

        public NeighborCollection Reflect()
        {
            List<Neighbor> reflectedNeighbors = new List<Neighbor>();

            foreach (Neighbor neighbour in Neighbors)
            {
                reflectedNeighbors.Add(neighbour.Reflect());
            }

            return new NeighborCollection
            {
                Neighbors = reflectedNeighbors
            };
        }
    }
}
