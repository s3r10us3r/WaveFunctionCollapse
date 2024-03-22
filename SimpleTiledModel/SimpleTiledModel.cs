using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFunctionCollapse.SimpleTiledModel.XMLModels;

namespace WaveFunctionCollapse.SimpleTiledModel
{
    public class SimpleTiledModel
    {
        public SimpleTiledModel(string tileSetPath, string tileSetName)
        {
            string xmlFilePath = Path.Combine(tileSetPath, tileSetName + ".xml");
            TileSetModel tileSetModel = TileSetModel.DeserialeFromXML(xmlFilePath);
        }
    }
}
