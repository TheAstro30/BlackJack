/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;
using BlackJack.Properties;

namespace BlackJack.Classes.Helpers
{
    public static class GraphicsBuilder
    {
        /* Stores an unmodified, unsorted list of all 52 cards */
        public static Deck MasterDeck = new Deck();

        public static readonly Size CardSize = new Size(79, 123);

        /* This stores the card images used during drawing */
        public static Dictionary<KeyValuePair<Suit, int>, Image> Cards = new Dictionary<KeyValuePair<Suit, int>, Image>();

        public static Dictionary<int, Image> Chips = new Dictionary<int, Image>();

        public static void BuildDeck()
        {
            using (var cards = Resources.card_set)
            {
                for (var y = 0; y <= 3; y++)
                {
                    var startY = CardSize.Height * y;
                    for (var x = 0; x <= 12; x++)
                    {
                        /* Set each card image */
                        System.Diagnostics.Debug.Print(" > Set card Suit: {0} Value: {1}", y, x + 1);
                        var cardImage = new Bitmap(CardSize.Width, CardSize.Height);
                        var src = new Rectangle(x * CardSize.Width, startY, CardSize.Width, CardSize.Height);
                        GetImage(cardImage, src, cards, CardSize);
                        var card = new Card
                        {
                            Suit = (Suit)y,
                            Value = x + 1
                        };
                        /* Push new image to the deck */
                        MasterDeck.Add(card);
                        Cards.Add(new KeyValuePair<Suit, int>(card.Suit, card.Value), cardImage);
                    }
                }
            }
        }

        private static void GetImage(Image cardBmp, Rectangle srcRegion, Image srcBitmap, Size cardSize)
        {
            using (var gfx = Graphics.FromImage(cardBmp))
            {
                gfx.DrawImage(srcBitmap, new Rectangle(0, 0, cardSize.Width, cardSize.Height), srcRegion, GraphicsUnit.Pixel);
            }
        }
    }
}
