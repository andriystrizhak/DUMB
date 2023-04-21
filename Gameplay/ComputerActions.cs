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
        //******************************( АТАКА / ОБОРОНА / ПІДКИДУВАННЯ )*******************************

        //******************************************( АТАКА )********************************************

        int movesLeft { get; set; }

        /// <summary>
        /// Симулює атаку КОМП'ЮТОРА
        /// </summary>
        public static void ComputerAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - перевіряючи відбій

            Console.WriteLine($"\n                 =====( Player's {GameState.Attacker + 1} turn )=====");
            int possibleAttacksNumber = Gameplay.PossibleAttacksNumber();

            //Цикл атак
            for (int i = 0; i < possibleAttacksNumber; i++)
            {
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
                if (AttackResponse(possibleAttacksNumber - i, i)) break;
            }
        }

        //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

        //Ввести змінну з кількістю ходів, потім об'єднати в один метод атаки
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

        //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

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
        /// Знаходить всі підходящі для атаки карти
        /// </summary>
        /// <returns>Список підходящих для атаки карт</returns>
        static List<Card> FindSuitableForAttackingCards()
        {
            List<Card> suitableCards = new List<Card>();

            //Шукаємо всі карти, що підходять для атаки
            foreach (var card in GameState.Players[GameState.Attacker].Cards)
                if (IsThisSuitableForComputerAttack(card))
                    suitableCards.Add(card);

            return suitableCards;
        }

        /// <summary>
        /// Перевіряє чи дана карта може бути використана ДЛЯ АТАКИ чи ПІДКИДАННЯ звіряючись з картами на "столі" чи картами "на знімання"
        /// </summary>
        /// <param name="currentCard">Поточна карта, яка перевірятиметься</param>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForComputerAttack(Card currentCard)
        {
            //Список, що міститиме карти зі "столу" та "карт на зняття"
            List<Card> tableCards = new List<Card>();
            foreach (var cardPair in Table.CardsPairs) //TODOTODO - мейбі тут???
            {
                tableCards.Add(cardPair.Item1);
                tableCards.Add(cardPair.Item2);
            }
            tableCards.AddRange(Table.TakenCards);

            //Перевірка чи є карта такого ж типу(рангу) в списку
            bool isSameTypeCard = false; //TODO - вивести в окремий метод
            foreach (var card in tableCards)
                if (card.Current.Value == currentCard.Current.Value)
                    isSameTypeCard = true;

            return isSameTypeCard;
        }


        //******************************************( ОБОРОНА )******************************************

        /// <summary>
        /// Визначає чию симуляцію реакції на атаку потрібно запустити: ГРАВЦЯ чи КОМП'ЮТОРА
        /// </summary>
        /// <param name="movesLeft">Кількість можливих атак, що залишилась</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак комп'ютора, чи ні</returns>
        static bool AttackResponse(int movesLeft, int i)
        {
            if (GameState.Players[GameState.Attacked] is Me)
                return MyActions.MyAttackResponse(movesLeft, i);
            else
                return ComputerAttackResponse(movesLeft);
        }

        /// <summary>
        /// Cимулює реакцію КОМП'ЮТОРА на атаку
        /// </summary>
        /// <param name="movesLeft">Кількість можливих атак, що залишилась</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак ГРАВЦЯ, чи ні</returns>
        public static bool ComputerAttackResponse(int movesLeft)
        {
            var higherCards = FindHigherCards();

            //Атака або буде відбита, або доведеться знімати карти
            if (higherCards.Count > 0)
                DefendingAttackWithComputer(higherCards);
            else
            {
                DefenceAbandoningByComputer(movesLeft);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Симулює відбивання атаки КОМП'ЮТОРОМ
        /// </summary>
        /// <param name="higherCards">Список більших від атакуючої карт</param>
        public static void DefendingAttackWithComputer(List<Card> higherCards)
        {
            Console.WriteLine($"\n\n               ==( Player {GameState.Attacked + 1} defend the attack )==");

            FindDefendingCard(higherCards);
            MyActions.CardsRemoving();
        }

        //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

        /// <summary>
        /// Знаходить всі карти більші від "атакуючої"
        /// </summary>
        /// <returns>Список зі всіма більшими картами</returns>
        public static List<Card> FindHigherCards()
        {
            var player = GameState.Players[GameState.Attacked];
            var allHigherCards = new List<Card>();

            //Пошук всіх більших від "атакуючої" карт
            for (int i = 0; i < player.Cards.Count; i++)
                if (Table.AttackingCard < player.Cards[i])
                    allHigherCards.Add(player.Cards[i]);
            /* REMOVE -
                if (Table.AttackingCard.Current.Key != GameState.TrumpSuit
                    && player.Cards[i].Current.Key == GameState.TrumpSuit)
                    allHigherCards.Add(player.Cards[i]);
                
                if (player.Cards[i].Current.Key == Table.AttackingCard.Current.Key
                    && player.Cards[i].Current.Value > Table.AttackingCard.Current.Value)
                    allHigherCards.Add(player.Cards[i]);
            */
            return allHigherCards;
        }

        //OPTIMIZE
        /// <summary>
        /// Присвоює найменшу карту зі списку до Table.DefendingCard
        /// </summary>
        /// <param name="cards">Список більших від атакуючої карт</param>
        static void FindDefendingCard(List<Card> cards)
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
            Table.DefendingCard = currentCard;
        }


        //***************************************( ПІДКИДУВАННЯ )****************************************

        //TODO - куди переміститити???
        /// <summary>
        /// Симулює підкидування карт "на знімання" КОМП'ЮТОРОМ
        /// </summary>
        /// <param name="movesLeft">Скільки карт ще може докинути КОМП'ЮТОР</param>
        public static void GivingCardsByComputer(int movesLeft)
        {
            //Переміщення всіх карт в карти "до знімання"
            Gameplay.ReplaceAllToTakenCards();
            int takenCardsNumber = Table.TakenCards.Count;
            Console.WriteLine(GameState.Players[GameState.Attacked] is Me 
                ? $"\n\n                 =( You abandone the defence )="
                : $"\n\n             =( Player {GameState.Attacked + 1} abandones the defence )=");

            //
            ReplaceAddedCardsToTakenCards(movesLeft);
            /* REMOVE - 
            for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
                for (int k = 0; k < Table.TakenCards.Count; k++)
                    if (GameState.Players[GameState.Attacker].Cards[j].Current.Key == Table.TakenCards[k].Current.Key
                        && GameState.Players[GameState.Attacker].Cards[j].Current.Value == Table.TakenCards[k].Current.Value)
                        GameState.Players[GameState.Attacker].Cards.RemoveAt(j);

            Gameplay.ShowTable();
            */
            if (GameState.Players[GameState.Attacked] is Me)
            {
                takenCardsNumber = Table.TakenCards.Count - takenCardsNumber;
                if (takenCardsNumber != 0)
                    Console.WriteLine($"\n             =( Player {GameState.Attacker + 1} gives you {takenCardsNumber} more card(-s) )=");
                Console.WriteLine($"\n                  =( You take {Table.TakenCards.Count} card(-s) )="
                    + "\n                      (and miss a turn)");
            }

            GameState.ResetAttackingPlayer();
        }

        //СЛУЖБОВЕ
        /// <summary>
        /// Переміщує всі нові додані "атакуючим" (тим хто підкидує) карти до карт "до знімання"
        /// </summary>
        /// <param name="movesLeft">Скільки карт ще може докинути КОМП'ЮТОР</param>
        public static void ReplaceAddedCardsToTakenCards(int movesLeft)
        {
            //Додавання "атакуючих" карт до карт "до знімання"
            for (int i = 1; i < movesLeft; i++)
            {
                if (ChooseComputerAttackingCard()) break;
                Table.TakenCards.Add(Table.AttackingCard);

                //Видалення поточної "атакуючої" карти в "атакуючого" (того хто підкидував)
                for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
                    if (GameState.Players[GameState.Attacker].Cards[j] == Table.AttackingCard)
                        GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
                //TODO - COPYPAST
            }
            /* REMOVE - 
            //Видалення всіх цих карт в "атакуючого" (того хто підкидував)
            for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
                for (int k = 0; k < Table.TakenCards.Count; k++)
                    if (GameState.Players[GameState.Attacker].Cards[j] == Table.TakenCards[k])
                        GameState.Players[GameState.Attacker].Cards.RemoveAt(j); 
                        //TODO - а після видалення кількість карт в лісті не зміниться?
            */
            Gameplay.ShowTable();
        }


        //*****************************************( ЗНІМАННЯ )******************************************

        /// <summary>
        /// Симулює випадок коли КОМП'ЮТОР знімає карту(-и)
        /// </summary>
        /// <param name="movesLeft">Скільки карт можна додати КОМП'ЮТОРУ</param>
        public static void DefenceAbandoningByComputer(int movesLeft)
        {
            Gameplay.ReplaceAllToTakenCards();
            Console.WriteLine($"\n\n               =( Player {GameState.Attacked + 1} abandones the defence )=");
            Gameplay.ShowTable();

            //Карти буде підкидувати або ГРАВЕЦЬ, або інший КОМП'ЮТОР
            if (GameState.Players[GameState.Attacker] is Me)
                MyActions.GivingCardsByMe(movesLeft);
            else
                GivingCardsByComputer(movesLeft);

            int takenCardsNumber = Table.TakenCards.Count;
            Console.WriteLine($"\n               =( Player {GameState.Attacked + 1} takes {takenCardsNumber} card(-s) )="
                + "\n                      (and miss a turn)");

            GameState.ResetAttackingPlayer();
        }
    }
}
