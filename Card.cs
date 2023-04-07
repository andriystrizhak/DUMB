using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            => ((attackingCard.Current.Key != Trump.Suit
                && Current.Key == Trump.Suit)
                || (Current.Key == attackingCard.Current.Key
                && Current.Value > attackingCard.Current.Value))
                ? 1 : -1;
        /*
        {
            if ((attackingCard.Current.Key != Trump.Suit
                && Current.Key == Trump.Suit) 
                || (Current.Key == attackingCard.Current.Key
                && Current.Value > attackingCard.Current.Value))
                return 1;
            else return -1;
        }
        */

        public bool Equals(Card? currentCard)
            => Current.Value == currentCard.Current.Value;
    }
}