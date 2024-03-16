using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    [XmlRoot("TileSet")]
    internal class TileSetModel
    {
        [XmlElement("Tile")]
        public List<TileModel> Tiles { get; set; }
        public string Format { get; set; }
        public int N { get; set; }

        int idCounter = 0;
        public void MakeTiles(string path)
        {
            idCounter = 0;
            List<TileModel> tiles = new List<TileModel>();
            Dictionary<string, int[]> idDict = new Dictionary<string, int[]>();

            foreach (TileModel tileModel in Tiles)
            {
                idDict.Add(tileModel.Name, SetUpRotations(path, tiles, tileModel));
            }

            List<Tile> result = new List<Tile>();

            foreach(TileModel tileModel in Tiles)
            {
                //left here
            }
        }

        private int[] SetUpRotations(string path, List<TileModel> tiles, TileModel tileModel)
        {
            tileModel.Init(path, Format, N);
            int[] rotations = new int[8];

            rotations[0] = idCounter++;
            tiles.Add(tileModel);
            switch (tileModel.SymmetryType)
            {
                case 'X':
                    for (int i = 1; i < 8; i++)
                    {
                        rotations[i] = rotations[0];
                    }
                    break;
                case 'I':
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
                case 'T':
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
                case 'L':
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
                case '\\':
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
                case 'F':
                    {
                        TileModel rotatedTile = tileModel;
                        for (int i = 1; i < 4; i++)
                        {
                            rotatedTile = tileModel.Rotate();
                            rotations[i] = idCounter++;
                            tiles.Add(tileModel);
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
