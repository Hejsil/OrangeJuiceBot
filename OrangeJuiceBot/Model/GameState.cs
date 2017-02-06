using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;

namespace OrangeJuiceBot.Model
{
    public class GameState
    {
        private readonly Bitmap[] _largerWhiteNumberTemplates =
        {
                null,
                new Bitmap(@".\Imaging\Numbers\White\Larger\1.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Larger\2.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Larger\3.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Larger\4.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Larger\5.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Larger\6.bmp"),
                null,
                null,
                null,
        };

        private readonly Bitmap[] _largeWhiteNumberTemplates =
        {
                new Bitmap(@".\Imaging\Numbers\White\Large\0.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Large\1.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Large\2.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Large\3.bmp"),
                null,
                null,
                null,
                null,
                null,
                null,
        };

        private readonly Bitmap[] _smallWhiteNumberTemplates =
        {
                new Bitmap(@".\Imaging\Numbers\White\Small\0.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\1.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\2.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\3.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\4.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\5.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\6.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\7.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\8.bmp"),
                new Bitmap(@".\Imaging\Numbers\White\Small\9.bmp")
        };

        private readonly ExhaustiveTemplateMatching _defaultMatcher = new ExhaustiveTemplateMatching(0.95f);

        private readonly ExhaustiveTemplateMatching[] _smallWhiteNumberMatchers =
        {
                new ExhaustiveTemplateMatching(0.79f),
                new ExhaustiveTemplateMatching(0.72f),
                new ExhaustiveTemplateMatching(0.78f),
                new ExhaustiveTemplateMatching(0.78f),
                new ExhaustiveTemplateMatching(0.78f),
                new ExhaustiveTemplateMatching(0.78f),
                new ExhaustiveTemplateMatching(0.79f),
                new ExhaustiveTemplateMatching(0.74f),
                new ExhaustiveTemplateMatching(0.79f),
                new ExhaustiveTemplateMatching(0.78f)
        };

        private readonly Process _ojProcess;
        
        public GameStatus Status { get; set; }
        public Field[,] Board { get; set; }
        public Player[] Players { get; } =
        {
            new Player(),
            new Player(),
            new Player(),
            new Player()
        };

        public GameState(Process ojProcess)
        {
            _ojProcess = ojProcess;
        }

        public void Update()
        {
            Rect size;
            GetClientRect(_ojProcess.MainWindowHandle, out size);

            var image = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(image))
            {
                var imageHdc = graphics.GetHdc();
                PrintWindow(_ojProcess.MainWindowHandle, imageHdc, 1);

                graphics.ReleaseHdc(imageHdc);
            }

            GetScores(image);
            GetBattleStats(image);
        }

        private void DeterminStatus(Bitmap image)
        {
            const int statusTextX = 791;
            const int statusTextY = 324;
            const int statusTextWidth = 338;
            const int statusTextHeight = 28;

            const int chooseACardToUseYOffset = 374 - statusTextY;
        }
                 

        private void GetScores(Bitmap image)
        {
            const int xDistance = 1600;
            const int yDistance = 900;

            const int starsXOffset = 121;
            const int starsYOffset = 111;
            const int starsWidth = 39;
            const int starsHeight = 17;

            const int winsXOffset = 172;
            const int winsYOffset = starsYOffset;
            const int winsWidth = 26;
            const int winsHeight = starsHeight;

            // Functions for getting the coordinates for the scores.
            Func<int, int, int> GetX = (player, offset) => offset + (player % 2) * xDistance;
            Func<int, int, int> GetY = (player, offset) => offset + (player / 2) * yDistance;

            for (var i = 0; i < 4; i++)
            {
                var starsRectangle = new Rectangle(
                    GetX(i, starsXOffset),
                    GetY(i, starsYOffset),
                    starsWidth,
                    starsHeight);

                var winsRectangle = new Rectangle(
                    GetX(i, winsXOffset),
                    GetY(i, winsYOffset),
                    winsWidth,
                    winsHeight);


                var starsImage = image.Clone(starsRectangle, image.PixelFormat);
                var winsImage = image.Clone(winsRectangle, image.PixelFormat);

                Players[i].Stars = DeterminScoreNumber(starsImage);
                Players[i].Wins = DeterminScoreNumber(winsImage);
            }
        }

        private void GetBattleStats(Bitmap image)
        {
            const int statXOffset = 670;
            const int statYOffset = 762;
            const int statDistance = 420;
            const int numberWidth = 29;
            const int numberHeight = 23;
            const int numberDistance = 60;
            const int prefixWidth = 16;
            const int prefixHeight = 17;

            const int healthXOffset = 574;
            const int healthYOffset = statYOffset;
            const int healthDistance = 729;
            const int healthWidth = 42;
            const int healthHeight = 32;

            Func<int, int, int> GetStatX = (player, stat) => (statXOffset + player * statDistance) + numberDistance * stat;
            Func<int, int> GetHealthX = player => healthXOffset + player * healthDistance;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var prefixRectangle = new Rectangle(
                        GetStatX(i, j),
                        statYOffset,
                        prefixWidth,
                        prefixHeight);

                    var numberRectangle = new Rectangle(
                        GetStatX(i, j) + prefixWidth,
                        statYOffset,
                        numberWidth,
                        numberHeight);

                    var prefixImage = image.Clone(prefixRectangle, image.PixelFormat);
                    var numberImage = image.Clone(numberRectangle, image.PixelFormat);

                    prefixImage.Save($@"D:\Users\r\Pictures\OJBot\Prefix Player{i}, Stat{j}, {DateTime.Now.Ticks}.bmp");
                    numberImage.Save($@"D:\Users\r\Pictures\OJBot\Number Player{i}, Stat{j}, {DateTime.Now.Ticks}.bmp");
                }

                var healthRectangle = new Rectangle(
                    GetHealthX(i),
                    healthYOffset,
                    healthWidth,
                    healthHeight);

                var healthImage = image.Clone(healthRectangle, image.PixelFormat);
                healthImage.Save($@"D:\Users\r\Pictures\OJBot\Number Player{i}, {DateTime.Now.Ticks}, Health.bmp");
            }
        }
        
        private int DeterminScoreNumber(Bitmap image)
        {
            var number = new List<Tuple<int, int>>();

            for (var i = 0; i < _smallWhiteNumberTemplates.Length; i++)
            {
                var matches = _smallWhiteNumberMatchers[i].ProcessImage(image, _smallWhiteNumberTemplates[i]);

                foreach (var match in matches)
                    number.Add(new Tuple<int, int>(match.Rectangle.X, i));
            }

            if (number.Count == 0)
                return -1;

            number.Sort((i1, i2) => i1.Item1.CompareTo(i2.Item1));

            var result = 0;
            for (var i = 0; i < number.Count; i++)
            {
                var test = number[i].Item2 * Math.Pow(10, number.Count - (i + 1));
                result += (int)test;
            }

            return result;
        }

        [DllImport("user32.dll")]
        protected static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);
        [DllImport("user32.dll")]
        protected static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        protected struct Rect
        {
            public Rect(Rect rectangle)
                : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
            { }

            public Rect(int left, int top, int right, int bottom)
            {
                X = left;
                Y = top;
                Right = right;
                Bottom = bottom;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }

            public int Left
            {
                get { return X; }
                set { X = value; }
            }
            public int Top
            {
                get { return Y; }
                set { Y = value; }
            }

            public int Height
            {
                get { return Bottom - Y; }
                set { Bottom = value + Y; }
            }

            public int Width
            {
                get { return Right - X; }
                set { Right = value + X; }
            }

            public Point Location
            {
                get { return new Point(Left, Top); }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
                set
                {
                    Right = value.Width + X;
                    Bottom = value.Height + Y;
                }
            }

            public static implicit operator Rectangle(Rect rectangle)
            {
                return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
            }
            public static implicit operator Rect(Rectangle rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }
            public static bool operator ==(Rect rectangle1, Rect rectangle2)
            {
                return rectangle1.Equals(rectangle2);
            }
            public static bool operator !=(Rect rectangle1, Rect rectangle2)
            {
                return !rectangle1.Equals(rectangle2);
            }

            public override string ToString()
            {
                return $"{{Left: {X}; Top: {Y}; Right: {Right}; Bottom: {Bottom}}}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(Rect rectangle)
            {
                return rectangle.Left == X && rectangle.Top == Y && rectangle.Right == Right && rectangle.Bottom == Bottom;
            }

            public override bool Equals(object Object)
            {
                if (Object is Rect)
                    return Equals((Rect)Object);

                if (Object is Rectangle)
                    return Equals(new Rect((Rectangle)Object));

                return false;
            }
        }
    }
}
