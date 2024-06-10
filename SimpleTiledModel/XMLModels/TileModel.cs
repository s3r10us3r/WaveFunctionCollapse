using System.Drawing.Imaging;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;

namespace SimpleModel.XMLModels
{
    public class TileModel
    {
        //this here is for later resolving symmetry types in neighbours using InitNeighbours
        public static Dictionary<string, string> nameSymmetry = new Dictionary<string, string>();

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("symmetry")]
        public string SymmetryType { get; set; }

        [XmlAttribute("weight")]
        public double Weight { get; set; } = 1;

        [XmlElement("Top")]
        public NeighborCollection Top { get; set; }
        [XmlElement("Right")]
        public NeighborCollection Right { get; set; }
        [XmlElement("Left")]
        public NeighborCollection Left { get; set; }
        [XmlElement("Bottom")]
        public NeighborCollection Bottom { get; set; }

        [XmlIgnore]
        public int[] Bitmap { get; protected set; }
        //image height and width
        private int n;

        //rotation index represents transformations that were applied to this tile
        //indices 0-3 represents rotated tile (0 - not rotated, 1 - rotated by 90 deg., 2 - rotated by 180 deg. 3 - rotated by 270-deg) indices 4 - 7 represent rotations applied to a tile of index 0 reflected along y-axis
        private int rotationIndex = 0;

        //this method copies the bitmap from path to a 1d array and completes neighbour collections according to its symmetry type and fills the nameSymmetry dictionary
        public void Init(string path, string format, int n)
        {
            this.n = n;
            ValidateArgs(format);

            nameSymmetry[Name] = SymmetryType;
            PixelFormat pixelFormat = format == "png" ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb;

            Bitmap preProcessedBitmap;
            string bitmapPath = path + "\\" + Name + "." + format;

            if (!File.Exists(bitmapPath))
            {
                throw new XMLException($"Could not find image {bitmapPath} corresponding to tile {Name}");
            }

            try
            {
                preProcessedBitmap = new Bitmap(bitmapPath);
            }
            catch (Exception e) when (e is ArgumentException || e is OutOfMemoryException)
            {
                throw new XMLException($"Error loading image {bitmapPath} corresponding to tile {Name}", e);
            }

            if (preProcessedBitmap.Width != n && preProcessedBitmap.Height != n)
            {
                throw new XMLException($"Provided n:{n} does not match image width:{preProcessedBitmap.Width} and height:{preProcessedBitmap.Height}");
            }
            CopyBitMapToArray(preProcessedBitmap, pixelFormat);
            switch (SymmetryType)
            {
                case "X":
                    Right = Top.Rotate();
                    Bottom = Right.Rotate();
                    Left = Bottom.Rotate();
                    break;
                case "T":
                    Left = Right.Reflect();
                    break;
                case "I":
                    Bottom = Top.Rotate().Rotate();
                    Left = Right.Reflect();
                    break;
                case "\\":
                    Right = Top.Rotate();
                    Bottom = Right.Rotate();
                    Left = Bottom.Rotate();
                    break;
                case "L":
                    Left = Bottom.Reflect().Rotate();
                    Right = Top.Reflect().Rotate();
                    break;
                case "F":
                    break;
                default:
                    throw new ArgumentException($"Invalid symmetry type '{SymmetryType}'. Symmetry type must be one of: 'X', 'T', 'I', 'L', '\\', 'F'");
            }
        }

        //this has to be done after the nameSymmetry dictionary has been filled up
        public void InitNeighbors()
        {
            Top.Init(nameSymmetry);
            Right.Init(nameSymmetry);
            Bottom.Init(nameSymmetry);
            Left.Init(nameSymmetry);
        }

        public TileModel Rotate()
        {
            //we only account for symmetry type for the untrasformed tile so we do not have to pass the symetry type any further
            TileModel rotatedModel = new TileModel
            {
                Name = Name,
                Top = Left.Rotate(),
                Right = Top.Rotate(),
                Bottom = Right.Rotate(),
                Left = Bottom.Rotate()
            };
            rotatedModel.Bitmap = RotateBitmap();
            rotatedModel.n = n;
            rotatedModel.rotationIndex = rotationIndex % 4 == 3 ? rotationIndex - 3 : rotationIndex + 1;

            return rotatedModel;
        }

        public TileModel Reflect()
        {
            TileModel reflectedModel = new TileModel
            {
                Name = Name,
                Top = Top.Reflect(),
                Right = Left.Reflect(),
                Bottom = Bottom.Reflect(),
                Left = Right.Reflect()
            };

            reflectedModel.Bitmap = ReflectBitmap();
            reflectedModel.n = n;
            reflectedModel.rotationIndex = rotationIndex > 4 ? rotationIndex - 4 : rotationIndex + 4;

            return reflectedModel;
        }

        public int GetTransformationID()
        {
            return rotationIndex;
        }

        private void ValidateArgs(string format)
        {
            if (Name is null || Name.Length < 1)
            {
                throw new XMLException($"Invalid Name '{Name}'");
            }
            if (format != "png" && format != "jpg")
            {
                throw new XMLException($"Invalid format specified '{format}' format must be either 'png' or 'jpg'");
            }
            if (!"XTIL\\F".Contains(SymmetryType))
            {
                throw new XMLException($"Invalid symmetry type {SymmetryType} symmetry type can be 'X' 'T' 'I' 'L' '\\' 'F'");
            }
            if (Weight <= 0)
            {
                throw new XMLException($"Weight must be more than 0 (if set to 0 it will default to 1)");
            }
        }

        private void CopyBitMapToArray(Bitmap bitmap, PixelFormat pixelFormat)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, n, n), ImageLockMode.ReadOnly, pixelFormat);

            nint ptr = bmpData.Scan0;
            Bitmap = new int[n * n];
            System.Runtime.InteropServices.Marshal.Copy(ptr, Bitmap, 0, n * n);
        }

        private int[] RotateBitmap()
        {
            int[] rotatedBitmap = new int[n * n];

            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    int index = row * n + col;
                    int rotatedRow = col;
                    int rotatedCol = n - 1 - row;
                    int rotatedIndex = rotatedRow * n + rotatedCol;
                    rotatedBitmap[rotatedIndex] = Bitmap[index];
                }
            }

            return rotatedBitmap;
        }

        private int[] ReflectBitmap()
        {
            int[] reflectedBitmap = new int[n * n];

            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    reflectedBitmap[row * n + (n - 1 - col)] = Bitmap[row * n + col];
                }
            }

            return reflectedBitmap;
        }
    }
}
