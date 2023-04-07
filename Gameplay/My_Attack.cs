using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Gameplay
{
    public class MyAttack
    {
        /// <summary>
        /// Симулює реакцію комп'ютора на атаку
        /// </summary>
        static void SimulateMyAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака [ввести КАУНТЕР КІЛ (??)]
            Console.WriteLine("\n                 =======( Your turn )=======");
            int possibleAttacksNumber = GameStatus.Players[GameStatus.Attacker].Cards.Count > 6
                ? 6 : GameStatus.Players[GameStatus.Attacker].Cards.Count > GameStatus.Players[GameStatus.Attacked].Cards.Count
                ? GameStatus.Players[GameStatus.Attacked].Cards.Count : GameStatus.Players[GameStatus.Attacker].Cards.Count;

            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                Gameplay.ShowMyCards();
                Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                    : "\n\n                -Choose the next attacking card!-")
                    + "\n                      (or \"0\" to pass)\n                              ");

                int myAttackingCardIndex = ChooseMyCard(CardPurpose.Attack);
                if (myAttackingCardIndex == -1) break;

                var higherCards = FindHigherCards();
                if (higherCards.Count > 0)
                    SimulateDefendingAttackWithComputer(higherCards, myAttackingCardIndex);
                else
                {
                    SimulateDefenceAbandoningByComputer(myAttackingCardIndex, possibleAttacksNumber - i);
                    break;
                }
            }
            ReplaceAllToDiscardPile();
        }

        //************************************************************************************************

        /*
         * 
        */

        /// <summary>
        /// Реалізує вибір карти для: атаки, докидування опоненту чи захисту
        /// </summary>
        /// <param name="purpose">Призначення вибору карти</param>
        /// <returns>Номер вибраної карти в МОЇЙ колоді</returns>
        static int ChooseMyCard(CardPurpose purpose)
        {
            string myAnswer;
            int selectedCardIndex = 0;
            //Table.AttackingCard = null;
            bool cardIsSuitable = false;
            int errorCounter = 0;

            while (!cardIsSuitable)
            {
                Console.Write(errorCounter == 0 ? "" : "                 -this card is not suitable-\n                              ");
                errorCounter++;
                myAnswer = Console.ReadLine();

                while (!int.TryParse(myAnswer, out selectedCardIndex)
                    || selectedCardIndex > GameStatus.Players[0].Cards.Count || selectedCardIndex < 0)
                {
                    Console.Write("                    -write correct number-\n                              ");
                    myAnswer = Console.ReadLine();
                }
                if (--selectedCardIndex == -1)
                    return selectedCardIndex;

                if (purpose != CardPurpose.Defence)
                {
                    Table.AttackingCard = GameStatus.Players[GameStatus.Attacker].Cards[selectedCardIndex];
                    if (purpose == CardPurpose.Attack)
                        cardIsSuitable = IsThisSuitableForAttack();
                    else
                        cardIsSuitable = IsThisSuitableForGiving();
                }
                else
                {
                    Table.DefendingCard = GameStatus.Players[GameStatus.Attacked].Cards[selectedCardIndex];
                    cardIsSuitable = IsThisSuitableForDefence();
                }
            }
            return selectedCardIndex;
        }

        /// <summary>
        /// Симулює випадок коли комп'ютор відбиває атаку
        /// </summary>
        /// <param name="higherCards">Ліст більших карт</param>
        /// <param name="attackCardIndex">Індекс атакуючої карти</param>
        static void SimulateDefendingAttackWithComputer(List<Card> higherCards, int attackCardIndex)
        {
            Console.WriteLine($"\n\n               ==( Player {GameStatus.Attacked + 1} defend the attack )==");

            var smallestHigherCard = FindSmallestHigherCard(higherCards);
            Table.CardsPairs.Add((Table.AttackingCard, smallestHigherCard));

            for (int i = 0; i < GameStatus.Players[GameStatus.Attacked].Cards.Count; i++)
                if (GameStatus.Players[GameStatus.Attacked].Cards[i] == smallestHigherCard)
                {
                    GameStatus.Players[GameStatus.Attacked].Cards.RemoveAt(i);
                    break;
                }
            GameStatus.Players[GameStatus.Attacker].Cards.RemoveAt(attackCardIndex);

            Gameplay.ShowTable();
        }

        /// <summary>
        /// Симулює випадок коли комп'ютор знімає карту(-и)
        /// </summary>
        /// <param name="attackCardIndex">Індекс атакуючої карти</param>
        /// <param name="movesLeft">Скільки карт Я можу додати</param>
        static void SimulateDefenceAbandoningByComputer(int attackCardIndex, int movesLeft)
        {
            ReplaceAllToTakenCards();
            for (int i = 0; i < movesLeft; i++)
            {
                Console.WriteLine($"\n\n               =( Player {GameStatus.Attacked + 1} abandones the defence )=");
                Table.TakenCards.Add(Table.AttackingCard);
                GameStatus.Players[GameStatus.Attacker].Cards.RemoveAt(attackCardIndex);

                Gameplay.ShowTable();
                Gameplay.ShowMyCards();
                Console.Write("\n\n                 -Choose the next giving card!-"
                    + "\n                      (or \"0\" to pass)\n                             ");

                attackCardIndex = ChooseMyCard(CardPurpose.Giving);
                if (attackCardIndex == -1) break;
            }
            int takenCardsNumber = ReplaceAllTakenCardsToPlayer();
            Console.WriteLine($"\n               =( Player {GameStatus.Attacked + 1} takes {takenCardsNumber} card(-s) )="
                + "\n                      (and miss a turn)");
            GameStatus.ChangeAttackingPlayer();
            //GameStatus.Attacker++;
        }

        //************************************************************************************************

        /// <summary>
        /// Знаходить всі карти більші від атакуючої
        /// </summary>
        /// <returns>Ліст зі всіма більшими картами</returns>
        static List<Card> FindHigherCards()
        {
            var player = GameStatus.Players[GameStatus.Attacked];
            var allHigherCards = new List<Card>();
            for (int i = 0; i < player.Cards.Count; i++)
            {
                if (Table.AttackingCard.Current.Key != Trump.Suit
                    && player.Cards[i].Current.Key == Trump.Suit)
                    allHigherCards.Add(player.Cards[i]);

                if (player.Cards[i].Current.Key == Table.AttackingCard.Current.Key
                    && player.Cards[i].Current.Value > Table.AttackingCard.Current.Value)
                    allHigherCards.Add(player.Cards[i]);
            }
            return allHigherCards;
        }

        /// <summary>
        /// Знаходить найменшу карту з Ліста більших від атакуючої карт
        /// </summary>
        /// <param name="cards">Ліст більших карт</param>
        /// <returns>Найменшу карту</returns>
        static Card FindSmallestHigherCard(List<Card> cards)
        {
            Card currentCard = cards[0];
            int minCardIndex = -1;
            for (int i = 0; i < cards.Count; i++)
                if (cards[i].Current.Key == Table.AttackingCard.Current.Key)
                {
                    minCardIndex = i;
                    break;
                }
            if (minCardIndex == -1)
            {
                for (int i = 1; i < cards.Count; i++)
                    if (currentCard.Current.Value > cards[i].Current.Value)
                        currentCard = cards[i];
            }
            else
            {
                currentCard = cards[minCardIndex];
                for (int i = minCardIndex + 1; i < cards.Count; i++)
                    if (currentCard.Current.Key == cards[i].Current.Key
                        && currentCard.Current.Value > cards[i].Current.Value)
                        currentCard = cards[i];
            }
            return currentCard;
        }

        /// <summary>
        /// Перевіряє чи дана атакуюча карта може бути використана ДЛЯ АТАКИ звіряючись з картами на столі
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForAttack()
        {
            if (Table.CardsPairs.Count == 0) return true;

            List<Card> tableCards = new List<Card>();
            foreach (var cardPair in Table.CardsPairs)
            {
                tableCards.Add(cardPair.Item1);
                tableCards.Add(cardPair.Item2);
            }

            bool isSameTypeCard = false;
            foreach (var card in tableCards)
                if (card.Current.Value == Table.AttackingCard.Current.Value)
                    isSameTypeCard = true;

            return isSameTypeCard;
        }

        /// <summary>
        /// Перевіряє чи дана атакуюча карта може бути використана ДЛЯ ДОКИДУВАННЯ звіряючись з картами "до знімання"
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForGiving()
        {
            bool isSameTypeCard = false;
            foreach (var card in Table.TakenCards)
                if (card.Current.Value == Table.AttackingCard.Current.Value)
                    isSameTypeCard = true;

            return isSameTypeCard;
        }

        /// <summary>
        /// Перевіряє чи дана захисна карта може бути використана ДЛЯ ВІДБИВАННЯ АТАКИ звіряючись з атакуючою карторю
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForDefence()
        {
            return Table.AttackingCard.Current.Key != Trump.Suit
                    && Table.DefendingCard.Current.Key == Trump.Suit
                    || Table.DefendingCard.Current.Key == Table.AttackingCard.Current.Key
                    && Table.DefendingCard.Current.Value > Table.AttackingCard.Current.Value;
        }

        //************************************************************************************************

        /// <summary>
        /// Переміщує всі карти зі столу до карт "до знімання"
        /// </summary>
        static void ReplaceAllToTakenCards()
        {
            if (Table.CardsPairs.Count == 0) return;

            foreach (var cardPair in Table.CardsPairs)
            {
                Table.TakenCards.Add(cardPair.Item1);
                Table.TakenCards.Add(cardPair.Item2);
            }
            Table.CardsPairs.Clear();
        }

        /// <summary>
        /// Переміщує всі карти "до знімання" до карт атакованого гравця 
        /// </summary>
        /// <returns>Кількість "знятих" карт</returns>
        static int ReplaceAllTakenCardsToPlayer()
        {
            int takenCardsNumber = Table.TakenCards.Count;
            GameStatus.Players[GameStatus.Attacked].Cards.AddRange(Table.TakenCards);
            Table.TakenCards.Clear();
            return takenCardsNumber;
        }

        /// <summary>
        /// Переміщує всі карти зі столу до відбою
        /// </summary>
        static void ReplaceAllToDiscardPile()
        {
            foreach (var cardPair in Table.CardsPairs)
            {
                DiscardPile.Cards.Add(cardPair.Item1);
                DiscardPile.Cards.Add(cardPair.Item2);
            }
            Table.CardsPairs.Clear();
        }
    }
}
