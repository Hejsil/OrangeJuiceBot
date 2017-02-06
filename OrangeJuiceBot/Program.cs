using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AForge.Imaging;
using OrangeJuiceBot.Imaging;
using OrangeJuiceBot.Model;

namespace OrangeJuiceBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = Process.GetProcessesByName("100orange")[0];
            var ojUpdater = new GameState(process);

            while (true)
            {
                ojUpdater.Update();
                Thread.Sleep(1000);
                Console.Clear();
            }

            //var image = new Bitmap(@"D:\Users\r\Pictures\OJBot\ScoreNumbers\Stars\8-.bmp");
            //var matches = Matchers[8].ProcessImage(image, Templates[8]);

            //var data = image.LockBits(
            //    new Rectangle(0, 0, image.Width, image.Height),
            //    ImageLockMode.ReadWrite, image.PixelFormat);

            //foreach (var m in matches)
            //{
            //    Drawing.Rectangle(data, m.Rectangle, Color.Red);
            //    // do something else with matching
            //}
            //image.UnlockBits(data);

            //image.Save(@"D:\Users\r\Pictures\OJBot\ScoreNumbers\test.bmp");

            //return;

            foreach (var file in Directory.GetFiles(@"D:\Users\r\Pictures\OJBot\ScoreNumbers\Stars"))
            {
                var number = int.Parse(Path.GetFileNameWithoutExtension(file).Split('-')[0]);
                var result = DeterminNumber(new Bitmap(file));

                if (number != result)
                    Console.WriteLine($"{file}: {result}");
            }

            Console.ReadKey();
        }

        public static Bitmap[] Templates =
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


        public static ExhaustiveTemplateMatching[] Matchers =
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

        public static int DeterminNumber(Bitmap image)
        {
            var number = new List<Tuple<int, int>>();

            for (var i = 0; i < Templates.Length; i++)
            {
                var matches = Matchers[i].ProcessImage(image, Templates[i]);

                foreach (var match in matches)
                    number.Add(new Tuple<int, int>(match.Rectangle.X, i));
            }

            number.Sort((i1, i2) => i1.Item1.CompareTo(i2.Item1));

            var result = 0;
            for (var i = 0; i < number.Count; i++)
            {
                var test = number[i].Item2 * Math.Pow(10, number.Count - (i + 1));
                result += (int)test;
            }

            return result;
        }
    }
}
