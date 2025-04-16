/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Drawing;

namespace BlackJack.Classes.Serialization
{
    public static class XmlFormatting
    {
        /* Xml formatting methods */
        public static int[] ParseXyFormat(string s)
        {
            var sp = s.Split(',');
            var inr = new int[sp.Length];
            for (var i = 0; i < sp.Length; ++i)
            {
                inr[i] = int.Parse(sp[i]);
            }
            return inr;
        }

        public static string WriteXyFormat(int x, int y)
        {
            return x + "," + y;
        }

        public static Point ParsePointFormat(string s)
        {
            var i = ParseXyFormat(s);
            return new Point(i[0], i[1]);
        }

        public static string WritePointFormat(Point p)
        {
            return WriteXyFormat(p.X, p.Y);
        }

        public static Size ParseSizeFormat(string s)
        {
            var i = ParseXyFormat(s);
            return new Size(i[0], i[1]);
        }

        public static string WriteSizeFormat(Size s)
        {
            return WriteXyFormat(s.Width, s.Height);
        }

        public static Rectangle ParseRectangleFormat(string s)
        {
            var i = ParseXyFormat(s);
            return new Rectangle(i[0], i[1], i[2], i[3]);
        }

        public static string WriteRectangleFormat(Rectangle r)
        {
            return WritePointFormat(r.Location) + "," + WriteSizeFormat(r.Size);
        }

        public static Rectangle ParseRbRectangleFormat(string s)
        {
            var i = ParseXyFormat(s);
            return Rectangle.FromLTRB(i[0], i[1], i[2], i[3]);
        }

        public static string WriteRbRectangleFormat(Rectangle r)
        {
            return WritePointFormat(r.Location) + "," + WriteXyFormat(r.Right, r.Bottom);
        }
    }
}
