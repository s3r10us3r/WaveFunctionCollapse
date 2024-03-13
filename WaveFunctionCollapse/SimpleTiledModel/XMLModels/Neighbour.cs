using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    internal class Neighbour
    {
        public string Name { get; set; }
        //transformation index indicates which transformations were applied to a tile by default the metadata provides neighbors for unrotated tiles
        //indices 0 - 3 represent image rotated by 0, 90, 180, 270 degrees and indices 4 - 7 correspond to same rotations but with reflection along y-axis applied
        public int TransformationIndex { get; set; }

        public Neighbour Rotate()
        {
            int rotatedIndex = TransformationIndex + 1;
            if (rotatedIndex == 4) rotatedIndex = 0;
            if (rotatedIndex == 8) rotatedIndex = 4;

            return new Neighbour
            {
                Name = Name,
                TransformationIndex = rotatedIndex
            };
        }

        public Neighbour Reflect()
        {
            int reflectedIndex = TransformationIndex + 4;

            if (reflectedIndex > 7)
            {
                reflectedIndex -= 8;
            }

            return new Neighbour
            {
                Name = Name,
                TransformationIndex = reflectedIndex
            };
        }
    }
}
