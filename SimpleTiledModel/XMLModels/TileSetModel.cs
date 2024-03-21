using System.IO;
using System.Xml.Serialization;
using WaveFunctionCollapse.SimpleTiledModel;

namespace WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    [XmlRoot("TileSet")]
    public class TileSetModel
    {
        [XmlElement("Tile")]
        public List<TileModel> Tiles { get; set; }

        [XmlAttribute("format")]
        public string Format { get; set; }

        [XmlAttribute("n")]
        public int N { get; set; }

        private Dictionary<string, int[]> idDict = new Dictionary<string, int[]>();
        private int idCounter = 0;

        public static TileSetModel DeserialeFromXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TileSetModel));

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                Object? deserializedModel = serializer.Deserialize(fileStream);
                if (deserializedModel is null)
                {
                    throw new XMLException("XML file is invalid");
                }
                return (TileSetModel)deserializedModel;
            }
        }

        public List<Tile> MakeTiles(string path)
        {
            idCounter = 0;
            List<TileModel> tiles = new List<TileModel>();
            idDict = new Dictionary<string, int[]>();

            foreach (TileModel tileModel in Tiles)
            {
                tileModel.Init(path, Format, N);
                idDict.Add(tileModel.Name, [0, 1, 2, 3, 4, 5, 6, 7]);
            }

            List<Tile> result = new List<Tile>();

            foreach (TileModel tileModel in Tiles)
            {
                TileModel rotatedTile = tileModel;
                for(int i = 0; i < 4; i++)
                {
                    result.Add(MakeTile(rotatedTile));
                    rotatedTile = rotatedTile.Rotate();
                }
                TileModel reflectedTile = tileModel.Reflect();
                for(int i = 0; i < 4; i++)
                {
                    result.Add(MakeTile(reflectedTile));
                    reflectedTile = reflectedTile.Rotate();
                }
            }

            return result;
        }

        private Tile MakeTile(TileModel tileModel)
        {
            HashSet<int> topNeighbours = new();
            foreach (Neighbour neighbour in tileModel.Top.Neighbours)
            {
                topNeighbours.Add(idDict[neighbour.Name][neighbour.TransformationIndex]);
            }

            HashSet<int> rightNeighbours = new();
            foreach (Neighbour neighbour in tileModel.Right.Neighbours)
            {
                rightNeighbours.Add(idDict[neighbour.Name][neighbour.TransformationIndex]);
            }

            HashSet<int> bottomNeighbours = new();
            foreach (Neighbour neighbour in tileModel.Bottom.Neighbours)
            {
                bottomNeighbours.Add(idDict[neighbour.Name][neighbour.TransformationIndex]);
            }

            HashSet<int> leftNeighbours = new();
            foreach (Neighbour neighbour in tileModel.Left.Neighbours)
            {
                leftNeighbours.Add(idDict[neighbour.Name][neighbour.TransformationIndex]);
            }

            return new Tile(tileModel.Bitmap, topNeighbours, rightNeighbours, leftNeighbours, bottomNeighbours);
        }

        private int[] SetUpRotations(string path, List<TileModel> tiles, TileModel tileModel)
        {
            tileModel.Init(path, Format, N);
            int[] rotations = new int[8];

            rotations[0] = idCounter++;
            tiles.Add(tileModel);
            switch (tileModel.SymmetryType)
            {
                case "X":
                    for (int i = 1; i < 8; i++)
                    {
                        rotations[i] = rotations[0];
                    }
                    break;
                case "I":
                    {
                        TileModel rotatedTile = tileModel.Rotate();
                        tiles.Add(rotatedTile);
                        rotations[1] = idCounter++;
                        for (int i = 3; i < 8; i += 2)
                        {
                            rotations[i] = rotations[1];
                        }
                        for (int i = 2; i < 8; i += 2)
                        {
                            rotations[i] = rotations[0];
                        }
                    }
                    break;
                case "T":
                    {
                        TileModel rotatedTile = tileModel;
                        rotations[4] = rotations[0];
                        for (int i = 1; i < 4; i++)
                        {
                            rotatedTile = tileModel.Rotate();
                            rotations[i] = idCounter++;
                            rotations[i + 4] = idCounter;
                            tiles.Add(rotatedTile);
                        }
                    }
                    break;
                case "L":
                    {
                        TileModel rotatedTile = tileModel;
                        rotations[5] = rotations[0];
                        for (int i = 1; i < 4; i++)
                        {
                            rotatedTile = tileModel.Rotate();
                            rotations[i] = idCounter++;
                            rotations[(i + 1) % 4 + 4] = idCounter;
                            tiles.Add(rotatedTile);
                        }
                    }
                    break;
                case "\\":
                    {
                        TileModel rotatedTile = tileModel.Rotate();
                        tiles.Add(rotatedTile);
                        rotations[1] = idCounter++;

                        for (int i = 2; i < 4; i++)
                        {
                            rotations[i] = rotations[i % 2];
                        }

                        for (int i = 4; i < 8; i++)
                        {
                            rotations[i] = rotations[1 - i % 2];
                        }
                    }
                    break;
                case "F":
                    {
                        TileModel rotatedTile = tileModel;
                        for (int i = 1; i < 4; i++)
                        {
                            rotatedTile = rotatedTile.Rotate();
                            rotations[i] = idCounter++;
                            tiles.Add(rotatedTile);
                        }
                        rotatedTile = tileModel.Reflect();
                        for (int i = 4; i < 8; i++)
                        {
                            rotations[i] = idCounter++;
                            tiles.Add(rotatedTile);
                            rotatedTile = rotatedTile.Rotate();
                        }
                    }
                    break;
            }

            return rotations;
        }
    }
}
