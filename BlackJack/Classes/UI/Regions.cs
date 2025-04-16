/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Properties;

namespace BlackJack.Classes.UI
{
    public class Regions
    {
        /* This class stores drawing regions for player cards, bet chips, etc. */
        public List<Point> Text { get; }

        public List<Point> Count { get; }

        public List<Point> Bet { get; }

        public List<Point> Chips { get; }

        public List<Point> Cards { get; }

        public Regions()
        {
            var center = Resources.table.Width / 2;
            Text = new List<Point>(new[]
            {
                new Point(1138, 371),
                new Point(696, 671),
                new Point(256, 371),
                new Point(center, 179) /* Dealer */
            });

            Count = new List<Point>(new []
            {
                new Point(1138, 250),
                new Point(696,555),
                new Point(256, 250),
                new Point(center,155) /* Dealer */
            });

            Bet = new List<Point>(new []
            {
                new Point(1138, 316),
                new Point(696, 623),
                new Point(256, 316),
                Point.Empty /* Dealer has no bet */
            });

            Chips = new List<Point>();

            Cards = new List<Point>(new[]
            {
                new Point(1105, 130),
                new Point(665, 435),
                new Point(224, 130),
                new Point(center - Resources.card_back.Width / 2 + 8, 35) /* Dealer */
            });
        }
    }
}
