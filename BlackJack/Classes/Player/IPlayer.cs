/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BlackJack.Classes.GameAssets;

namespace BlackJack.Classes.Player
{
    public interface IPlayer
    {
        Point ChipRegion { get; set; }

        Point CardRegion { get; set; }

        int Index { get; set; }

        string Name { get; set; }

        int Money { get; set; }

        int Bet { get; set; }

        int Total { get; set; }

        bool Stand { get; set; }

        List<Card> Hand { get; set; }

        void AddCard(Card c);

        void ComputeBet();

        void ComputeHand();

        string ToString();
    }
}
