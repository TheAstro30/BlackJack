/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */

using System;
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;

namespace BlackJack.Classes.Player
{
    public class ComputerPlayer : IPlayer
    {
        /* The "decision" making whether to hit, stand, etc. is going to happen in this class */
        public Point ChipRegion { get; set; }

        public Point CardRegion { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public int Money { get; set; }

        public int Bet { get; set; }

        public int Total { get; set; }

        public bool Stand { get; set; }

        public List<Card> Hand { get; set; }

        public ComputerPlayer()
        {
            Hand = new List<Card>();

            /* Pick a random name for this AI player */
            Name = PlayerNames.GetRandomName;

            /* Set this player's money initially to $2,000 */
            Money = 2000;

            //test
            Bet = 100;
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
            /* Compute a bet on place bet round */
        }

        public void ComputeHand()
        {
            /* AI to calculate to hit or stand */
        }

        public override string ToString()
        {
            return Bet > 0
                ? $"{Name} - ${Formatting.FormatNumber(Money)}\r\nBet: ${Formatting.FormatNumber(Bet)}"
                : $"{Name} - ${Formatting.FormatNumber(Money)}";
        }
    }
}
