/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;

namespace BlackJack.Classes.Player
{
    public class HumanPlayer : IPlayer
    {
        public PlayerState State { get; set; }

        public event Action<IPlayer> EndBet;

        public event Action<IPlayer> EndTurn;
        public int Index { get; set; }

        public string Name { get; set; }

        public int Money { get; set; }

        public int Bet { get; set; }

        public int Total { get; set; }

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

        public void BeginBet()
        {
            /* Not used for human player */
            MessageBox.Show("My bet");
            Bet = 100;
            Money -= 100;
        }

        public void BeginTurn()
        {
            /* Not used for human player */
            if (MessageBox.Show("My turn. Hit?", "Test code", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                State = PlayerState.Hit;
                return;
            }
            State = PlayerState.Stand;
        }

        public override string ToString()
        {
            return Bet > 0
                ? $"{Name} - ${Money.FormatNumber()}\r\nBet: ${Bet.FormatNumber()}"
                : $"{Name} - ${Money.FormatNumber()}";
        }
    }
}
