using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Durak.Gameplay;

namespace Durak
{
    /// <summary>
    /// Реалізує сутність "Гральна Карта"
    /// </summary>
    public class Card : IComparable<Card>, IEquatable<Card>
    {
        public readonly KeyValuePair<Suits, Ranks> Current;
        public Card(KeyValuePair<Suits, Ranks> currentCard)
            => Current = currentCard;

        public int CompareTo(Card? attackingCard)
            => ((attackingCard.Current.Key != GameState.TrumpSuit
                && Current.Key == GameState.TrumpSuit)
                || (Current.Key == attackingCard.Current.Key
                && Current.Value > attackingCard.Current.Value))
                ? 1 : -1;

        public bool Equals(Card? currentCard)
            => Current.Value == currentCard.Current.Value;

        //**********************************************************

        public static bool operator >(Card x, Card y)
        {
            return (x.Current.Key != GameState.TrumpSuit
                && y.Current.Key == GameState.TrumpSuit)
                || (y.Current.Key == x.Current.Key
                && y.Current.Value > x.Current.Value);
        }

        public static bool operator <(Card x, Card y)
        {
            return !((x.Current.Key != GameState.TrumpSuit
                && y.Current.Key == GameState.TrumpSuit)
                || (y.Current.Key == x.Current.Key
                && y.Current.Value > x.Current.Value));
        }
    }
}