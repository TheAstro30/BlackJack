/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Drawing;
using BlackJack.Classes.GameAssets;

namespace BlackJack.Classes.Player
{
    /* This enum is used only for the human player interaction */
    public enum PlayerAction
    {
        None = 0,
        Betting = 1,
        Insurance = 2,
        PlayerTurn = 3
    }

    public enum PlayerState
    {
        None = 0,
        Hit = 1,
        DoubleDown = 2,
        Stand = 3,
        BlackJack = 4,
        Bust = 5
    }

    public interface IPlayer
    {
        PlayerState State { get; set; }

        event Action<IPlayer, PlayerAction> PlayerActionRequired;

        event Action<IPlayer> EndBet;

        event Action<IPlayer> EndInsurance;

        event Action<IPlayer> EndTurn;

        int Index { get; set; }

        string Name { get; set; }

        int Money { get; set; }

        int Bet { get; set; }

        int Total { get; set; }

        List<Card> Hand { get; set; }

        void AddCard(Card c);

        void EndPlayerActionRequired(PlayerAction action);

        void BeginBet();

        void BeginInsurance();

        void BeginTurn();

        string ToString();
    }
}
