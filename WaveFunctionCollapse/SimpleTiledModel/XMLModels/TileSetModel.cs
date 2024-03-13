using System;
using System.Collections.Generic;
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
        public bool RotationEnabled { get; set; }
        public bool ReflectionEnabled { get; set; }
        public int N { get; set; }

        public void MakeTiles()
        {
        }
    }
}
