using System.Xml.Serialization;

namespace SimpleModel.XMLModels
{
    public class Neighbor
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        //transformation index indicates which transformations were applied to a tile by default the metadata provides neighbors for unrotated tiles
        //indices 0 - 3 represent image rotated by 0, 90, 180, 270 degrees and indices 4 - 7 correspond to same rotations but with reflection along y-axis applied
        [XmlAttribute("tID")]
        public int TransformationIndex { get; set; }

        public Neighbor Rotate()
        {
            int rotatedIndex = TransformationIndex + 1;
            if (rotatedIndex == 4) rotatedIndex = 0;
            if (rotatedIndex == 8) rotatedIndex = 4;

            return new Neighbor
            {
                Name = Name,
                TransformationIndex = rotatedIndex
            };
        }

        public Neighbor Reflect()
        {
            int[] reflectionArray = [4, 7, 6, 5, 0, 3, 2, 1];

            return new Neighbor
            {
                Name = Name,
                TransformationIndex = reflectionArray[TransformationIndex]
            };
        }
    }
}
