using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Gameplay
{
    /// <summary>
    /// Містить інструкції для симуляції поведінки комп'ютора за різних ситуацій
    /// </summary>
    public class ComputerActions
    {
        /// <summary>
        /// Симулює атаку комп'ютора
        /// </summary>
        public static void SimulateComputerAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - ввести КАУНТЕР КІЛ (??)
            // - перевіряючи відбій

            Console.WriteLine($"\n                 =====( Player's {GameState.Attacker + 1} turn )=====");
            //Визначаємо максимально можливу кількість атак за хід
            int possibleAttacksNumber = 
                GameState.Players[GameState.Attacker].Cards.Count > 6
                ? 6 : GameState.Players[GameState.Attacker].Cards.Count 
                > GameState.Players[GameState.Attacked].Cards.Count
                ? GameState.Players[GameState.Attacked].Cards.Count 
                : GameState.Players[GameState.Attacker].Cards.Count;

            //Цикл атак
            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                //Вибір атакуючої карти (першої чи наступних)
                if (i == 0) ChooseComputerFirstAttackingCard();
                else
                {
                    ChooseComputerAttackingCard();
                    if (Table.AttackingCard == null)
                    {
                        Console.WriteLine($"               ==( Player {GameState.Attacker + 1} completed the attack )==");
                        break;
                    }
                }

                //Відповідь "Атакованого" на атаку цього комп'ютора
                if (ComputerAttackResponse(possibleAttacksNumber, i))
                    break;
                /*
                int myDefendingCardIndex = 0;
                if (GameState.Players[GameState.Attacked] is Me)
                {
                    //TODO - не обов'язково я, можливо й інший робот
                    Gameplay.ShowMyCards();
                    Console.Write((i == 0 ? "\n                 -Choose your defending card!-"
                        : "\n\n                -Choose the next defending card!-")
                        + "\n                  (or \"0\" to abandone defence)\n                              ");

                    myDefendingCardIndex = MyActions.ChooseMyCard(CardPurpose.Defence);
                }
                if (myDefendingCardIndex == -1)
                {
                    //TODO - не обов'язково я, можливо й інший робот
                    SimulateDefenceAbandoningByMe(possibleAttacksNumber - i - 1); 
                    break;
                }

                Table.CardsPairs.Add((Table.AttackingCard, GameState.Players[GameState.Attacked].Cards[myDefendingCardIndex])); //TODO - in particular method
                for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
                    if (GameState.Players[GameState.Attacker].Cards[j] == Table.AttackingCard)
                    {
                        GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
                        break;
                    }
                GameState.Players[GameState.Attacked].Cards.RemoveAt(myDefendingCardIndex);
                Gameplay.ShowTable();
                */
            }
            //Переміщуємо всі карти зі стола у відбій
            Gameplay.ReplaceAllToDiscardPile();
        }

        //************************************************************************************************

        /// <summary>
        /// Викликає набір дій для симуляції реакції комп'ютора або МОЮ на атаку комп'ютора
        /// </summary>
        /// <param name="possibleAttacksNumber">Початкова кількість можливих атак</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак комп'ютора, чи ні</returns>
        static bool ComputerAttackResponse(int possibleAttacksNumber, int i)
        {
            if (GameState.Players[GameState.Attacked] is Me)
                return MyActions.MyResponseToComputerAttack(possibleAttacksNumber, i);
            else
                return ComputerResponseToComputerAttack(possibleAttacksNumber, i);
        }

        //TODO - Дублювання
        /// <summary>
        /// Cимулює реакцію іншого комп'ютора на атаку комп'ютора
        /// </summary>
        /// <param name="possibleAttacksNumber">Початкова кількість можливих атак</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак комп'ютора, чи ні</returns>
        public static bool ComputerResponseToComputerAttack(int possibleAttacksNumber, int i)
        {
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                : "\n\n                -Choose the next attacking card!-")
                + "\n                      (or \"0\" to pass)\n                              ");

            int myAttackingCardIndex = MyActions.ChooseMyCard(CardPurpose.Attack);
            if (myAttackingCardIndex == -1) return true;

            var higherCards = ComputerActions.FindHigherCards();
            if (higherCards.Count > 0)
                ComputerActions.SimulateDefendingAttackWithComputer(higherCards);
            else
            {
                ComputerActions.SimulateDefenceAbandoningByComputer(myAttackingCardIndex, possibleAttacksNumber - i);
                return true;
            }
            return false;
        }



        //************************************************************************************************

        /// <summary>
        /// Симулює вибір першої атакуючої карти
        /// </summary>
        static void ChooseComputerFirstAttackingCard()
        {
            Table.AttackingCard = FindSmallestCard(GameState.Players[GameState.Attacker].Cards);

            Console.WriteLine("\n                         ** attack **");
            Console.WriteLine($"                        {Table.AttackingCard.Current} =>\n");
        }

        /// <summary>
        /// Знаходить найменшу карту в Лісті
        /// </summary>
        /// <returns>Найменшу карту гравця</returns>
        static Card FindSmallestCard(List<Card> cards)
        {
            Card currentCard = cards[0];
            bool isTrumpCardThere = false;

            for (int i = 0; i < cards.Count; i++)
                if (cards[i].Current.Key != GameState.TrumpSuit)
                    isTrumpCardThere = true;

            for (int i = 1; i < cards.Count; i++)
                if (currentCard.Current.Value > cards[i].Current.Value
                    && (isTrumpCardThere ? cards[i].Current.Key != GameState.TrumpSuit : true))
                    currentCard = cards[i];

            return currentCard;
        }

        /// <summary>
        /// Вибирає підходящу для атаки карту (або null)
        /// </summary>
        static void ChooseComputerAttackingCard()
        {
            var suitableForAttackingCard = FindSuitableForAttackingCards();
            if (suitableForAttackingCard.Count == 0)
            {
                Table.AttackingCard = null;
                return;
            }
            Card smallestSutableCard = FindSmallestCard(suitableForAttackingCard);

            Random rnd = new Random();
            bool doAttackWithTrumpCard = rnd.Next(3) == 2 ? true : false;

            if (smallestSutableCard.Current.Key == GameState.TrumpSuit
                && !doAttackWithTrumpCard)
            {
                Table.AttackingCard = null;
                return;
            }
            Table.AttackingCard = smallestSutableCard;
            //TODOTODO - ШО ТАМ БЛЯТЬ по захисту від повторів карток, бейб?

            Console.WriteLine("\n                  ** attack continuation **"
                + $"\n                        {Table.AttackingCard.Current} =>\n");
        }

        /// <summary>
        /// Знаходить всі підходящі для атаки карти
        /// </summary>
        /// <returns>Список підходящих для атаки карт</returns>
        static List<Card> FindSuitableForAttackingCards(CardPurpose purpose = CardPurpose.Defence)
        {
            List<Card> suitableCards = new List<Card>();

            foreach (var card in GameState.Players[GameState.Attacker].Cards)
                if (IsThisSuitableForComputerAttack(card, purpose))
                    suitableCards.Add(card);

            return suitableCards;
        }

        /// <summary>
        /// Перевіряє те чи карта підходящою
        /// Перевіряє чи дана карта може бути використана ДЛЯ АТАКИ чи ПІДКИДАННЯ звіряючись з картами на столі чи картами "на знімання"
        /// </summary>
        /// <param name="currentCard">Поточна карта, яка перевірятиметься</param>
        /// <param name="purpose">Призначення перевірки</param>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForComputerAttack(Card currentCard, CardPurpose purpose)
        {
            List<Card> tableCards = new List<Card>();
            if (purpose == CardPurpose.Defence)
                foreach (var cardPair in Table.CardsPairs)
                {
                    tableCards.Add(cardPair.Item1);
                    tableCards.Add(cardPair.Item2);
                }
            else
                tableCards.AddRange(Table.TakenCards);

            bool isSameRangeCard = false;
            foreach (var card in tableCards)
                if (card.Current.Value == currentCard.Current.Value)
                    isSameRangeCard = true;

            return isSameRangeCard;
        }

        //TODO - куди переміститити???
        /// <summary>
        /// Симулює накидування МЕНІ карт на знімання
        /// </summary>
        /// <param name="movesLeft">Скільки карт ще може докинути РОБОТ</param>
        public static void SimulateDefenceAbandoningByMe(int movesLeft)
        {
            Gameplay.ReplaceAllToTakenCards();
            int takenCardsNumber = Table.TakenCards.Count + 1;

            Console.WriteLine($"\n\n                 =( You abandone the defence )=");
            Table.TakenCards.Add(Table.AttackingCard);

            //TODO - все має працювати як все
            var sutableCards = FindSuitableForAttackingCards(CardPurpose.Giving);
            for (int i = 1; i < (movesLeft > sutableCards.Count ? sutableCards.Count : movesLeft); i++)
                Table.TakenCards.Add(sutableCards[i]);

            for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
                for (int k = 0; k < Table.TakenCards.Count; k++)
                    if (GameState.Players[GameState.Attacker].Cards[j].Current.Key == Table.TakenCards[k].Current.Key
                        && GameState.Players[GameState.Attacker].Cards[j].Current.Value == Table.TakenCards[k].Current.Value)
                        GameState.Players[GameState.Attacker].Cards.RemoveAt(j);

            Gameplay.ShowTable();
            takenCardsNumber = Table.TakenCards.Count - takenCardsNumber;
            if (takenCardsNumber != 0)
                Console.WriteLine($"\n               =( Player {GameState.Attacker + 1} gives you {takenCardsNumber} more cards )=");
            Console.WriteLine($"\n                  =( You take {Table.TakenCards.Count} card(-s) )="
                + "\n                      (and miss a turn)");

            Gameplay.ReplaceAllTakenCardsToPlayer();
            GameState.ResetAttackingPlayer();
        }


        //************************************************************************************************


        /// <summary>
        /// Симулює випадок коли комп'ютор відбиває атаку
        /// </summary>
        /// <param name="higherCards">Ліст більших карт</param>
        /// <param name="attackCardIndex">Індекс атакуючої карти</param>
        public static void SimulateDefendingAttackWithComputer(List<Card> higherCards)
        {
            Console.WriteLine($"\n\n               ==( Player {GameState.Attacked + 1} defend the attack )==");

            //var smallestHigherCard = FindSmallestHigherCard(higherCards);
            FindSmallestHigherCard(higherCards);
            MyActions.CardsRemoving();
            /*
            Table.CardsPairs.Add((Table.AttackingCard, Table.DefendingCard));

            for (int i = 0; i < GameState.Players[GameState.Attacked].Cards.Count; i++)
                if (GameState.Players[GameState.Attacked].Cards[i] == Table.DefendingCard)
                {
                    GameState.Players[GameState.Attacked].Cards.RemoveAt(i);
                    break;
                }
            GameState.Players[GameState.Attacker].Cards.RemoveAt(attackCardIndex);

            Gameplay.ShowTable();
            */
        }

        //TODO - переробити в метод підходящий для: і МЕНЕ і для комп'ютора
        /// <summary>
        /// Симулює випадок коли комп'ютор знімає карту(-и)
        /// </summary>
        /// <param name="attackCardIndex">Індекс атакуючої карти</param>
        /// <param name="movesLeft">Скільки карт Я можу додати</param>
        public static void SimulateDefenceAbandoningByComputer(int attackCardIndex, int movesLeft)
        {
            Gameplay.ReplaceAllToTakenCards();
            for (int i = 0; i < movesLeft; i++)
            {
                Console.WriteLine($"\n\n               =( Player {GameState.Attacked + 1} abandones the defence )=");
                Table.TakenCards.Add(Table.AttackingCard);
                //GameState.Players[GameState.Attacker].Cards.RemoveAt(attackCardIndex);
                MyActions.CardsRemoving();
                Gameplay.ShowTable();

                //АБО-АБО
                if (GameState.Players[GameState.Attacker] is Me)
                {
                    if (MyActions.DefenseAbandoningOfMYAttackByComputer()) break;
                }

                else
                    SimulateDefenceAbandoningByMe(movesLeft);
            }
            int takenCardsNumber = Gameplay.ReplaceAllTakenCardsToPlayer();
            Console.WriteLine($"\n               =( Player {GameState.Attacked + 1} takes {takenCardsNumber} card(-s) )="
                + "\n                      (and miss a turn)");
            GameState.ResetAttackingPlayer();
            //GameStatus.Attacker++;
        }

        /// <summary>
        /// Знаходить всі карти більші від атакуючої
        /// </summary>
        /// <returns>Ліст зі всіма більшими картами</returns>
        public static List<Card> FindHigherCards()
        {
            var player = GameState.Players[GameState.Attacked];
            var allHigherCards = new List<Card>();
            for (int i = 0; i < player.Cards.Count; i++)
            {
                if (Table.AttackingCard.Current.Key != GameState.TrumpSuit
                    && player.Cards[i].Current.Key == GameState.TrumpSuit)
                    allHigherCards.Add(player.Cards[i]);

                if (player.Cards[i].Current.Key == Table.AttackingCard.Current.Key
                    && player.Cards[i].Current.Value > Table.AttackingCard.Current.Value)
                    allHigherCards.Add(player.Cards[i]);
            }
            return allHigherCards;
        }

        /*
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
        */

        /// <summary>
        /// Знаходить найменшу карту з Ліста більших від атакуючої карт
        /// </summary>
        /// <param name="cards">Ліст більших карт</param>
        /// <returns>Найменшу карту</returns>
        static void FindSmallestHigherCard(List<Card> cards)
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
            //return currentCard;
            Table.DefendingCard = currentCard;
        }
    }
}
