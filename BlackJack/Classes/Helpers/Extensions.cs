/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace BlackJack.Classes.Helpers
{
    internal static class StringExtensions
    {
        internal static string FormatNumber(this int num)
        {
            if (num >= 1000000000)
            {
                return (num / 1000000000D).ToString("0.##B");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            return num >= 1000 ? (num / 1000D).ToString("0.##k") : num.ToString("#,0");
        }
    }

    internal static class ListExtensions
    {
        private static readonly Random Rng = new Random();

        /* List extension */
        internal static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    internal static class GraphicsExtensions
    {
        /* Drawing extensions */
        internal static void DrawImageTiled(this Graphics graphics, Image image, Size size)
        {
            using (var brush = new TextureBrush(image, WrapMode.Tile))
            {
                graphics.FillRectangle(brush, 0, 0, size.Width, size.Height);
            }
        }

        internal static void DrawImageStretched(this Graphics graphics, Image image, Size destSize)
        {
            graphics.DrawImage(image, 0, 0, destSize.Width, destSize.Height);
        }

        internal static void DrawImageOpaque(this Graphics graphics, Image image, Rectangle destRect, float opacity)
        {
            var colormatrix = new ColorMatrix { Matrix33 = opacity };
            using (var imgAttribute = new ImageAttributes())
            {
                imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttribute);
            }
        }

        internal static void DrawGradient(this Graphics graphics, Color[] colors, Size size)
        {
            var rect = new Rectangle(0, 0, size.Width, size.Height);
            using (var gradient = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical))
            {
                var cb = new ColorBlend
                {
                    Colors = colors,
                    Positions = colors.Select((t, i) => (float)i / (colors.Length - 1)).ToArray()
                };
                gradient.InterpolationColors = cb;
                graphics.FillRectangle(gradient, new Rectangle(0, 0, size.Width, size.Height));
            }
        }

        internal static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            using (var path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /* Private methods */
        internal static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(bounds.Location, size);
            var path = new GraphicsPath();
            /* Just return a rectangle if radius is 0 */
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            /* Top left arc */
            path.AddArc(arc, 180, 90);
            /* Top right arc */
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            /* Bottom right arc */
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            /* Bottom left arc */
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            /* Close path and return result */
            path.CloseFigure();
            return path;
        }
    }
}
