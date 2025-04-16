/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
namespace BlackJack.Classes.GameAssets
{
    public enum Suit
    {
        Clubs = 0,
        Diamonds = 1,
        Hearts = 2,
        Spades = 3,
    }

    public class Card
    {
        public Suit Suit { get; set; }

        public int Value { get; set; }

        public bool IsHidden { get; set; }

        public Card()
        {
            /* Empty by default */
        }

        public Card(Card c)
        {
            /* Copy constructor */
            Suit = c.Suit;
            Value = c.Value;
        }
    }
}
