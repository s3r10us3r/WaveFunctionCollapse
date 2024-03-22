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

        private Dictionary<string, Tile[]> tileDict;
        private Dictionary<string, TileModel[]> modelDict;

        public Dictionary<string, Tile[]> GetDict()
        {
            return tileDict;
        }


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
            tileDict = new Dictionary<string, Tile[]>();
            modelDict = new Dictionary<string, TileModel[]>();

            foreach (TileModel tileModel in Tiles)
            {
                tileModel.Init(path, Format, N);
            }

            foreach (TileModel tileModel in Tiles)
            {
                tileModel.InitNeighbours();
                Tile[] transformations = new Tile[8];
                TileModel[] tileModels = new TileModel[8];

                tileModels[0] = tileModel;
                transformations[0] = new Tile(tileModel);

                TileModel transformedTile = tileModel.Rotate();
                for (int i = 1; i < 4; i++)
                {
                    transformations[i] = new Tile(transformedTile);
                    tileModels[i] = transformedTile;
                    transformedTile = transformedTile.Rotate();
                }
                transformedTile = tileModel.Reflect();
                for (int i = 4; i < 8; i++)
                {
                    transformations[i] = new Tile(transformedTile);
                    tileModels[i] = transformedTile;
                    transformedTile = transformedTile.Rotate();
                }
                tileDict.Add(tileModel.Name, transformations);
                modelDict.Add(tileModel.Name, tileModels);
            }

            List<Tile> result = new List<Tile>();
            foreach (string key in tileDict.Keys)
            {
                for(int i  = 0; i < 8; i++)
                {
                    MakeNeighbours(modelDict[key][i], tileDict[key][i]);
                    result.Add(tileDict[key][i]);
                }
            }
            
            return result;
        }

        private void MakeNeighbours(TileModel model, Tile tile)
        {
            tile.TopNeighbors = ConvertNeighbours(model.Top);
            tile.RightNeighbors = ConvertNeighbours(model.Right);
            tile.BottomNeighbors = ConvertNeighbours(model.Bottom);
            tile.LeftNeighbors = ConvertNeighbours(model.Left);
        }

        private HashSet<Tile> ConvertNeighbours(NeighbourCollection neighbourCollection)
        {
            HashSet<Tile> result = new HashSet<Tile>();

            foreach(Neighbour neighbour in neighbourCollection.Neighbours)
            {
                result.Add(tileDict[neighbour.Name][neighbour.TransformationIndex]);
            }

            return result;
        }
    }
}
