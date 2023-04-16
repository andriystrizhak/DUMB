using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Gameplay
{
    /// <summary>
    /// Містить інструкції для симуляції поведінки КОМП'ЮТОРА за різних ситуацій
    /// </summary>
    public class ComputerActions
    {
        /// <summary>
        /// Симулює атаку КОМП'ЮТОРА
        /// </summary>
        public static void SimulateComputerAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - перевіряючи відбій

            Console.WriteLine($"\n                 =====( Player's {GameState.Attacker + 1} turn )=====");
            //Визначення максимально можливої кількості атак за хід
            int possibleAttacksNumber = 
                GameState.Players[GameState.Attacker].Cards.Count > 6
                ? 6 : GameState.Players[GameState.Attacker].Cards.Count 
                > GameState.Players[GameState.Attacked].Cards.Count
                ? GameState.Players[GameState.Attacked].Cards.Count 
                : GameState.Players[GameState.Attacker].Cards.Count;

            //Цикл атак
            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                /*
                //Вибір атакуючої карти (першої чи наступної)
                if (i == 0) ChooseComputerFirstAttackingCard();
                else
                {
                    if (ChooseComputerAttackingCard())
                    {
                        Table.AttackingCard = null;
                        Console.WriteLine($"               ==( Player {GameState.Attacker + 1} completed the attack )==");
                        break;
                    }
                }

                //Відповідь "АТАКОВАНОГО" на атаку цього КОМП'ЮТОРА
                if (ComputerAttackResponse(possibleAttacksNumber - 1, i))
                    break;
                */
                //Симуляція атаки КОМП'ЮТОРА
                if (ComputerAttack(possibleAttacksNumber - i, i))
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
            Gameplay.ReplaceAllToDiscardPile();
        }


        //******************************( АТАКА / ОБОРОНА / ПІДКИДУВАННЯ )*******************************

        //.........................................( АТАКА ).............................................

        /// <summary>
        /// Симулює атаку КОМП'ЮТОРА
        /// </summary>
        /// <param name="remainingAttacksNumber">Кількість можливих атак, що залишилась</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак КОМП'ЮТОРА, чи ні</returns>
        static bool ComputerAttack(int remainingAttacksNumber, int i)
        {
            //Вибір атакуючої карти (першої чи наступної)
            if (i == 0) ChooseComputerFirstAttackingCard();
            else
            {
                if (ChooseComputerAttackingCard())
                {
                    Table.AttackingCard = null;
                    Console.WriteLine($"               ==( Player {GameState.Attacker + 1} completed the attack )==");
                    return true;
                }
            }

            //Відповідь "АТАКОВАНОГО" на атаку цього КОМП'ЮТОРА
            if (ComputerAttackResponse(remainingAttacksNumber, i))
                return true;
            return false;
        }

        //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

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
        /// Знаходить найменшу карту в списку карт
        /// </summary>
        /// <returns>Найменшу карту зі списку</returns>
        static Card FindSmallestCard(List<Card> cards)
        {
            Card currentCard = cards[0];
            bool isTrumpCardThere = false;

            //Перевірка списку карт на наявність козирних
            for (int i = 0; i < cards.Count; i++)
                if (cards[i].Current.Key != GameState.TrumpSuit)
                    isTrumpCardThere = true;

            //Пошук найменшої карти
            for (int i = 1; i < cards.Count; i++)
                if (currentCard.Current.Value > cards[i].Current.Value
                    && (isTrumpCardThere ? cards[i].Current.Key != GameState.TrumpSuit : true))
                    currentCard = cards[i];

            return currentCard;
        }

        /// <summary>
        /// Вибирає підходящу для атаки карту (або присвоює null)
        /// </summary>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак КОМП'ЮТОРА, чи ні</returns>
        static bool ChooseComputerAttackingCard()
        {
            //Знаходимо всі підходящі для атаки карти
            var suitableForAttackingCard = FindSuitableForAttackingCards();
            if (suitableForAttackingCard.Count == 0)
                return true;
            Card smallestSutableCard = FindSmallestCard(suitableForAttackingCard);

            //Рандомно визначаємо чи давата атакуючу карту, якщо вона козирна
            Random rnd = new Random();
            bool doAttackWithTrumpCard = rnd.Next(3) == 2 ? true : false;

            if (smallestSutableCard.Current.Key == GameState.TrumpSuit
                && !doAttackWithTrumpCard)
                return true;
            Table.AttackingCard = smallestSutableCard;

            Console.WriteLine("\n                  ** attack continuation **"
                + $"\n                        {Table.AttackingCard.Current} =>\n");
            return false;
        }

        /// <summary>
        /// Знаходить всі підходящі для атаки карти
        /// </summary>
        /// <returns>Список підходящих для атаки карт</returns>
        static List<Card> FindSuitableForAttackingCards() //REMOVE - CardPurpose purpose = CardPurpose.Defence)
        {
            List<Card> suitableCards = new List<Card>();

            //Шукаємо всі карти, що підходять для атаки
            foreach (var card in GameState.Players[GameState.Attacker].Cards)
                if (IsThisSuitableForComputerAttack(card)) //REMOVE - , purpose))
                    suitableCards.Add(card);

            return suitableCards;
        }

        /// <summary>
        /// Перевіряє чи дана карта може бути використана ДЛЯ АТАКИ чи ПІДКИДАННЯ звіряючись з картами на "столі" чи картами "на знімання"
        /// </summary>
        /// <param name="currentCard">Поточна карта, яка перевірятиметься</param>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForComputerAttack(Card currentCard) //REMOVE - , CardPurpose purpose)
        {
            //Список, що міститиме карти зі "столу" та "карт на зняття"
            List<Card> tableCards = new List<Card>();
            //REMOVE - if (purpose == CardPurpose.Defence)
            foreach (var cardPair in Table.CardsPairs)
            {
                tableCards.Add(cardPair.Item1);
                tableCards.Add(cardPair.Item2);
            }
            //REMOVE - else
            tableCards.AddRange(Table.TakenCards);

            //Перевірка чи є карта такого ж типу(рангу) в списку
            bool isSameTypeCard = false;
            foreach (var card in tableCards)
                if (card.Current.Value == currentCard.Current.Value)
                    isSameTypeCard = true;

            return isSameTypeCard;
        }


        //TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO
        //TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO
        //TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO+TODO

        //........................................( ОБОРОНА )............................................

        /// <summary>
        /// Викликає набір дій для симуляції реакції комп'ютора або МОЮ на атаку комп'ютора
        /// </summary>
        /// <param name="remainingAttacksNumber">Початкова кількість можливих атак</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак комп'ютора, чи ні</returns>
        static bool ComputerAttackResponse(int remainingAttacksNumber, int i)
        {
            if (GameState.Players[GameState.Attacked] is Me)
                return MyActions.MyAttackResponse(remainingAttacksNumber, i);
            else
                return ComputerResponseToAttack(remainingAttacksNumber);
        }

        /// <summary>
        /// Cимулює реакцію КОМП'ЮТОРА на атаку
        /// </summary>
        /// <param name="remainingAttacksNumber">Кількість можливих атак, що залишилась</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак ГРАВЦЯ, чи ні</returns>
        public static bool ComputerResponseToAttack(int remainingAttacksNumber)
        {
            /*
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                : "\n\n                -Choose the next attacking card!-")
                + "\n                      (or \"0\" to pass)\n                              ");

            int myAttackingCardIndex = MyActions.ChooseMyCard(CardPurpose.Attack);
            if (myAttackingCardIndex == -1) return true;
            */
            var higherCards = FindHigherCards();
            if (higherCards.Count > 0)
                SimulateDefendingAttackWithComputer(higherCards);
            else
            {
                SimulateDefenceAbandoningByComputer(remainingAttacksNumber);
                return true;
            }
            return false;
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
            var sutableCards = FindSuitableForAttackingCards();//CardPurpose.Giving);
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
        /// Симулює відбивання атаки КОМП'ЮТОРОМ
        /// </summary>
        /// <param name="higherCards">Список більших від атакуючої карт</param>
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
        /// <param name="movesLeft">Скільки карт Я можу додати</param>
        public static void SimulateDefenceAbandoningByComputer(int movesLeft)
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
                    if (MyActions.GivingCardsByMe()) break;
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
        /// Знаходить найменшу карту зі списку більших від атакуючої карт
        /// </summary>
        /// <param name="cards">Список більших від атакуючої карт</param>
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
