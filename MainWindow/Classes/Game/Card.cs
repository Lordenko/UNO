using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace MainWindow
{
    internal class Card : Object
    {
        public int Color { get; set; } // 1 - red, 2 - yellow, 3 - green, 4 - blue, 5 - black
        public int Suit { get; set; } // 0 - 9, 10 - block, 11 - reverse, 12 - add2, 13 - add4, 14 - change color
        public int zIndex { get; set; }
        public CroppedBitmap DisabledCardImage { get; set; }
        public CroppedBitmap SourseCardImage { get; set; }

        public Card() { }
        public Card(int Color, int Suit, double x, double y, double width, double height, CroppedBitmap SourseCardImage, int zIndex) : base(x, y, width, height)
        {
            this.Color = Color;
            this.Suit = Suit;
            this.zIndex = zIndex;
            this.SourseCardImage = SourseCardImage;
        }

        public void ChangeColor(int suit)
        {
            this.Color = suit;
        }
    }
}
