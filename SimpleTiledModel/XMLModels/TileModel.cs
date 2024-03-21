using System.Drawing.Imaging;
using System.Drawing;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    public class TileModel
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("symmetry")]
        public string SymmetryType { get; set; }

        [XmlElement("Top")]
        public NeighbourCollection Top { get; set; }
        [XmlElement("Right")]
        public NeighbourCollection Right { get; set; }
        [XmlElement("Left")]
        public NeighbourCollection Left { get; set; }
        [XmlElement("Bottom")]
        public NeighbourCollection Bottom { get; set; }


        public int[] Bitmap { get; set; }
        private int n;

        //rotation index represents transformations that were applied to this tile
        //indices 0-3 represents rotated tile (0 - not rotated, 1 - rotated by 90 deg., 2 - rotated by 180 deg. 3 - rotated by 270-deg) indices 4 - 7 represent same rotations but applied to a tile reflected along y-axis
        private int rotationIndex = 0;

        //n is image height and width
        public void Init(string path, string format, int n)
        {
            this.n = n;
            ValidateArgs(format);

            PixelFormat pixelFormat = format == "png" ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb;
            Bitmap preProcessedBitmap = new Bitmap(path + "\\" + Name + "." + format);
            if (preProcessedBitmap.Width != n && preProcessedBitmap.Height != n)
            {
                throw new ArgumentException($"Provided n:{n} does not match image width:{preProcessedBitmap.Width} and height:{preProcessedBitmap.Height}");
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
                    Right = Top.Reflect().Rotate();
                    Bottom = Right.Reflect().Rotate();
                    Left = Bottom.Reflect().Rotate();
                    break;
                case "L":
                    Left = Bottom.Rotate();
                    Right = Top.Rotate();
                    break;
                case "F":
                    break;
                default:
                    throw new ArgumentException($"Invalid symmetry type '{SymmetryType}'. Symmetry type must be one of: 'X', 'T', 'I', 'L', '\\', 'F'");
            }
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

            return reflectedModel;
        }

        private void ValidateArgs(string format)
        {
            if (Name is null || Name.Length < 1)
            {
                throw new ArgumentException($"Invalid Name '{Name}'");
            }
            if (format != "png" && format != "jpg")
            {
                throw new ArgumentException($"Invalid format specified '{format}' format must be either 'png' or 'jpg'");
            }
            if (!"XTIL\\F".Contains(SymmetryType))
            {
                throw new ArgumentException($"Invalid symmetry type {SymmetryType} symmetry type can be 'X' 'T' 'I' 'L' '\\' 'F'");
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
