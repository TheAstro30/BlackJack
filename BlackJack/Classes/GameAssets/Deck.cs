/* BlackJack - Version 1.0
 * Written by Jason James Newland
 * ©2025 - KangaSoft Software */
using System;
using System.Collections;
using System.Collections.Generic;

namespace BlackJack.Classes.GameAssets
{
    public class Deck : IList<Card>
    {
        private readonly List<Card> _cards = new List<Card>();

        public Deck()
        {
            /* Default empty constructor */
        }

        public void Add(Card item)
        {
            _cards.Add(item);
        }

        public void AddRange(IList<Card> cards)
        {
            _cards.AddRange(cards);
        }

        public void Clear()
        {
            _cards.Clear();
        }

        public bool Contains(Card item)
        {
            return _cards.Contains(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            _cards.CopyTo(array, arrayIndex);
        }

        public bool Remove(Card item)
        {
            return _cards.Remove(item);
        }

        public int Count => _cards.Count;

        public bool IsReadOnly => false;

        public int IndexOf(Card item)
        {
            return _cards.IndexOf(item);
        }

        public void Insert(int index, Card item)
        {
            _cards.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _cards.RemoveAt(index);
        }

        public Card this[int index]
        {
            get => _cards[index];
            set => _cards[index] = value;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Shuffle()
        {
            _cards.Shuffle();
        }
    }

    public static class ListExtensions
    {
        private static readonly Random Rng = new Random();

        /* List extension */
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
