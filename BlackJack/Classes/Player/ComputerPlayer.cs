/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Threading;
using BlackJack.Classes.GameAssets;
using BlackJack.Classes.Helpers;

namespace BlackJack.Classes.Player
{
    public class ComputerPlayer : IPlayer
    {
        /* The "decision" making whether to hit, stand, etc. is going to happen in this class */
        private static readonly Random Rnd = new Random();

        public PlayerState State { get; set; }

        public event Action<IPlayer, PlayerAction> PlayerActionRequired;

        public event Action<IPlayer> EndBet;

        public event Action<IPlayer> EndInsurance;

        public event Action<IPlayer> EndTurn;

        public int Index { get; set; }

        public string Name { get; set; }

        public int Money { get; set; }

        public int Bet { get; set; }

        public int Total { get; set; }

        public List<Card> Hand { get; set; }

        public ComputerPlayer()
        {
            Hand = new List<Card>();

            /* Pick a random name for this AI player */
            Name = PlayerNames.GetRandomName;

            /* Set this player's money initially to $2,000 */
            Money = 2000;
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

        public void EndPlayerActionRequired(PlayerAction action)
        {
            /* Not used on computer player */
        }

        public void BeginBet()
        {
            /* Compute a bet on place bet round */
            PlayerActionRequired?.Invoke(this, PlayerAction.None);
            Bet = 100;
            Money -= 100;
            EndBet?.Invoke(this);
        }

        public void BeginInsurance()
        {
            EndInsurance?.Invoke(this);
        }

        public void BeginTurn()
        {
            /* AI to calculate to hit or stand */
            if (Total < 17)
            {
                if (Total == 16)
                {
                    Thread.Sleep(Rnd.Next(100, 1000));
                    var i = Rnd.Next(0, 20);
                    switch (i)
                    {
                        case 3:
                        case 7:
                        case 11:
                        case 19:
                            State = PlayerState.Hit;
                            break;

                        default:
                            State = PlayerState.Stand;
                            break;
                    }
                }
                else
                {
                    State = PlayerState.Hit;
                }
            }
            else if (Total == 17)
            {
                Thread.Sleep(Rnd.Next(100, 1000));
                var i = Rnd.Next(0, 100);
                switch (i)
                {
                    case 13:
                    case 20:
                    case 50:
                    case 99:
                        State = PlayerState.Hit;
                        break;

                    default:
                        State = PlayerState.Stand;
                        break;
                }
            }
            else
            {
                State = PlayerState.Stand;
            }
            EndTurn?.Invoke(this);
        }

        public override string ToString()
        {
            return Bet > 0
                ? $"{Name} - ${Money.FormatNumber()}\r\nBet: ${Bet.FormatNumber()}"
                : $"{Name} - ${Money.FormatNumber()}";
        }
    }
}
