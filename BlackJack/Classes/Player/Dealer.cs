/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;

namespace BlackJack.Classes.Player
{
    public class Dealer : IPlayer
    {
        public Point ChipRegion { get; set; }

        public Point CardRegion { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public int Money { get; set; } /* Not used for dealer */

        public int Bet { get; set; } /* Not used for dealer - dealer doesn't bet */

        public int Total { get; set; }

        public bool Stand { get; set; }

        public List<Card> Hand { get; set; }

        public Dealer()
        {
            Name = $"Dealer - {PlayerNames.GetRandomFemaleName}";
            Hand = new List<Card>();
        }

        public void AddCard(Card c)
        {
            Total += c.Value > 10 ? 10 : c.Value;
            if (c.Value == 1)
            {
                /* It's an ace, so by default is 11 - unless total is greater than 21, then it's 1 */
                if (Total + 10 <= 21)
                {
                    Total += 10;
                }
            }
            Hand.Add(c);
        }

        public void ComputeBet()
        {
            /* Dealer doesn't compute a bet */
        }

        public void ComputeHand()
        {
            /* Just deal self cards until number equals soft 17 or below 21 */
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
