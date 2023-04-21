using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public interface Player
    {
        public string Name { get; set;}
        public List<Card> Cards { get; set; }
    }

    public class Me : Player
    {
        public string Name { get; set; }//TODO add name
        public List<Card> Cards { get; set; } = new List<Card>();
    }

    public class Computer : Player
    {
        public string Name { get; set; }//TODO - add name
        public List<Card> Cards { get; set; } = new List<Card>();
    }

    //********************************************************************************************

    public static class Deck
    {
        public static List<Card> Cards { get; set; }
        static Deck() => Cards = AddAllCards();

        /// <summary>
        /// Заповнює колоду всіма 36-ма картами
        /// </summary>
        /// <returns></returns>
        private static List<Card> AddAllCards()
        {
            var allCards = new List<Card>();
            for (int i = 0; i < 4; i++)
                for (int j = 6; j < 15; j++)
                    allCards.Add(new Card (new KeyValuePair<Suits, Ranks>((Suits)i, (Ranks)j)));
            return allCards;
        }
    }

    public static class Table
    {
        public static List<(Card, Card)> CardsPairs { get; set; } = new List<(Card, Card)>();
        public static List<Card> TakenCards { get; set; } = new List<Card>();

        public static Card AttackingCard { get; set; }
        public static Card DefendingCard { get; set; }
    }

    public static class DiscardPile
    {
        public static List<Card> Cards { get; set; } = new List<Card>();
    }
}