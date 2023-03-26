using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public static class Deck
    {
        public static List<Card> Cards = AddAllCards();

        /// <summary>
        /// Заповнює колоду всіма 36-ма картами
        /// </summary>
        /// <returns></returns>
        static List<Card> AddAllCards()
        {
            var allCards = new List<Card>();
            for (int i = 0; i < 4; i++)
                for (int j = 6; j < 15; j++)
                    allCards.Add(new Card 
                    { 
                        Current = new KeyValuePair<Suits, Ranks>((Suits)i, (Ranks)j) 
                    });
            return allCards;
        }
    }

    public class Player
    {
        public List<Card> Cards = new List<Card>();
    }

    public static class Table
    {
        public static List<(Card, Card)> CardsPairs = new List<(Card, Card)>();
        public static List<Card> TakenCards = new List<Card>();

        public static Card AttackingCard = new Card();
        public static Card DefendingCard = new Card();
    }

    public static class DiscardPile
    {
        public static List<Card> Cards = new List<Card>();
    }
}