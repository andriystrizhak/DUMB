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

        //Для сортування карт "на руках"
        public int CompareTo(Card? attackingCard)
            => ((attackingCard.Current.Key != GameState.TrumpSuit
                && Current.Key == GameState.TrumpSuit)
                || (Current.Key == attackingCard.Current.Key
                && Current.Value > attackingCard.Current.Value))
                ? 1 : -1;

        public bool Equals(Card? currentCard)
            => Current.Value == currentCard.Current.Value;

        #region RELATIONAL OPERATORS
        /// <summary>
        /// Визначає чи менша карта x від карти y (для атаки), чи ні
        /// </summary>
        /// <param name="x">Карта 1</param>
        /// <param name="y">Карта 2</param>
        /// <returns></returns>
        public static bool operator <(Card x, Card y)
        {
            return (x.Current.Key != GameState.TrumpSuit
                && y.Current.Key == GameState.TrumpSuit)
                || (y.Current.Key == x.Current.Key
                && y.Current.Value > x.Current.Value);
        }

        /// <summary>
        /// Визначає чи більша карта x від карти y (для атаки), чи ні
        /// </summary>
        /// <param name="x">Карта 1</param>
        /// <param name="y">Карта 2</param>
        /// <returns></returns>
        public static bool operator >(Card x, Card y)
        {
            return (y.Current.Key != GameState.TrumpSuit
                && x.Current.Key == GameState.TrumpSuit)
                || (x.Current.Key == y.Current.Key
                && x.Current.Value > y.Current.Value);
        }
        #endregion
    }
}