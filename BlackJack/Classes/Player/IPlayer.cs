/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;

namespace BlackJack.Classes.Player
{
    public enum PlayerState
    {
        None = 0,
        Hit = 1,
        Stand = 2,
        BlackJack = 3,
        Bust = 4
    }

    public interface IPlayer
    {
        PlayerState State { get; set; }

        event Action<IPlayer> EndBet;

        event Action<IPlayer> EndTurn;

        int Index { get; set; }

        string Name { get; set; }

        int Money { get; set; }

        int Bet { get; set; }

        int Total { get; set; }

        List<Card> Hand { get; set; }

        void AddCard(Card c);

        void BeginBet();

        void BeginTurn();

        string ToString();
    }
}
