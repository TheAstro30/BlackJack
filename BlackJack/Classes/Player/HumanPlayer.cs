/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;

namespace BlackJack.Classes.Player
{
    public class HumanPlayer : IPlayer
    {
        public Point ChipRegion { get; set; }

        public Point CardRegion { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public int Money { get; set; }

        public int Bet { get; set; }

        public int Total { get; set; }

        public bool Stand { get; set; }

        public List<Card> Hand { get; set; }

        public HumanPlayer()
        {
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
            /* Not used for human player */
        }

        public void ComputeHand()
        {
            /* Not used for human player */
        }

        public override string ToString()
        {
            return Bet > 0
                ? $"{Name} - ${Formatting.FormatNumber(Money)}\r\nBet: ${Formatting.FormatNumber(Bet)}"
                : $"{Name} - ${Formatting.FormatNumber(Money)}";
        }
    }
}
