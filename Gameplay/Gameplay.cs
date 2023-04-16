using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Durak.Gameplay
{
    public static class Gameplay
    {
        //TODO - rid it (after replacing)

        //TODO - Реалізувати закінчення гри й висвітлення переможця (якщо кількість карт одного з гравців + відбій == 36).
        // - А краще висвітлювати вихід гравця з гри в якого 0 карт при тому що в колоді також 0.
        // - А також врахувати випадок НІЧИЇ.

        //TODO - Реалізувати логіку того, що у відбиваючого закінчилися карти або був перевищений ліміт в 6 карт (сповіщення, що    
        //TODO - З'явився БАГ - комп'ютор в якись момент починає безкінечно підкидувати певну карту

        //TODO - Реалізувати логіку "Пограти ще раз"
        //TODO - Видалити трампа

        public static void Main()
        {
            ToSayHello();

            //bool isTheEnd = false;
            while (!IsTheEndOfGame())
            {
                if (GameState.Players[GameState.Attacker] is Me) MyActions.SimulateMyAttack();
                else ComputerActions.SimulateComputerAttack();

                GameState.ResetAttackingPlayer();
                DistributeCards();
                Console.WriteLine("\n\n*****************************************************************");
                /*
                for (int i = 0; i < GameStatus.Players.Count; i++)
                    if (GameStatus.Players[i].Cards.Count + DiscardPile.Cards.Count == 36)
                    {
                        isTheEnd = true;
                        Console.WriteLine("\n\n\n\n\n"
                            + (i == 0
                            ? "                       ~~~~~~~~~~~~~~~~\n"
                            + "                    <=@| You are DUMB |@=>\n"
                            + "                       ~~~~~~~~~~~~~~~~"
                            : "                     ~~~~~~~~~~~~~~~~~~~~\n"
                            + $"                  <=@| Player {i + 1} is DUMB |@=>\n"
                            + "                     ~~~~~~~~~~~~~~~~~~~~"
                            + "\n\n                        ==( YOU WIN )==")
                            + "\n\n\n\n\n");
                    }
                */
            }
            ToSayBye();
        }

        //************************************************************************************************
        //************************************************************************************************

        /// <summary>
        /// Виводить привітальну текстову інтеракцію в консоль
        /// </summary>
        static void ToSayHello()
        {
            Console.Write("\n     Welcome to DUMB — card-game created by @Andriy_Strizhak"
                + "\n                 Do you want to know the rules?"
                + "\n                       ( \"y\" or \"n\" )\n                              ");
            string answer = Console.ReadLine();
            Console.WriteLine(answer == "y"
                ? "                   Dude, just google it! -_-" : answer == "n"
                ? "                 Think you're the smartest? o_O"
                : "             Um, OK, just don`t cry if you lose o_o");

            GetPlayersQuantity();
            Console.WriteLine("\n*****************************************************************"
                + $"\n                 =@( The Trump Suit is: )@="
                + $"\n                        =>[{GameState.TrumpSuit}]<=");

            DistributeCards();
            Console.WriteLine("\n*****************************************************************");

            GameState.ChoosePlayerWhoStarts();
            Console.WriteLine(GameState.Attacker == 0
                ? "\n                =( The first move is yours )="
                : $"\n                 ===( Player {GameState.Attacker + 1} goes first )===");
                //+ $"\n                (Lowest trump is: {GameStatus.AttackerTrumpCard.Item1.Current})");
        }

        /// <summary>
        /// Виводить інтеракцію для вибору кількості гравців в консоль
        /// </summary>
        static void GetPlayersQuantity()
        {
            string myAnswer;
            int selectedPlayerNumbers = 0;

            Console.Write("       -So, enter player numbers and we'll start the game!-"
                + "\n                      (from \"2\" to \"6\")\n                              ");
            myAnswer = Console.ReadLine();

            while (!int.TryParse(myAnswer, out selectedPlayerNumbers)
                || selectedPlayerNumbers < 2 || selectedPlayerNumbers > 6)
            {
                Console.Write("                    -write correct number-\n                              ");
                myAnswer = Console.ReadLine();
            }
            GameState.SetPlayersQuantity(selectedPlayerNumbers);
        }

        /// <summary>
        /// Визначає чи це кінець гри і якщо так то виводить в консоль спеціальні повідомлення
        /// </summary>
        /// <returns>Значення типу bool що вказує чи це кінець гри, чи ні</returns>
        public static bool IsTheEndOfGame()
        {
            //Шукаємо гравців в яких не залишилося карт й видаляємо (занулюємо) їх
            for (int i = 0; i < GameState.Players.Count; i++)
                if (GameState.Players[i].Cards.Count == 0)      //GameStatus.Players[i].Cards.Count + DiscardPile.Cards.Count == 36)
                {
                    if (GameState.Players[i] is Me)
                        Console.WriteLine("\n                   ==( You leave the game )=="
                            + "\n\n                        ==( YOU WIN )==\n\n"); //TODO - "та вибуваєш з гри"
                    else
                        Console.WriteLine($"\n\n                ==( Player {i + 1} leaves the game )==\n\n");
                    GameState.RemovePlayer(i);
                }

            //Перевіряємо скільки гравців залишилося в грі
            int counter = 0;
            int remainingPlayer = -1;
            for (int i = 0; i < GameState.Players.Count; i++)
            {
                if (GameState.Players[i] == null)
                    counter++;
                else
                    remainingPlayer = i;
            }

            //Якщо 1 то оголошуємо "ДУРНЯ"
            if (counter == 1)
            {
                Console.WriteLine("\n\n\n\n\n"
                    + (remainingPlayer == 0
                    ? "                       ~~~~~~~~~~~~~~~~\n"
                    + "                    <=@| You are DUMB |@=>\n"
                    + "                       ~~~~~~~~~~~~~~~~"
                    : "                     ~~~~~~~~~~~~~~~~~~~~\n"
                    + $"                  <=@| Player {remainingPlayer + 1} is DUMB |@=>\n"
                    + "                     ~~~~~~~~~~~~~~~~~~~~")
                    //+ "\n\n                        ==( YOU WIN )==")
                    + "\n\n\n\n\n");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Виводить прощальну текстову інтеракцію в консоль
        /// </summary>
        static void ToSayBye()
        {
            Console.Write("                What's your final words, man?\n                              ");
            Console.ReadLine();
            Console.WriteLine("                 You better not say anything."
                + "\n                     Okay, see you later!");
            Console.ReadKey(true);
        }

        //************************************************************************************************

        /// <summary>
        /// Роздає всім гравцям стільки карт скільки їм не вистачає 
        /// </summary>
        static void DistributeCards()
        {
            Random rnd = new Random();
            int takesCardsFirst = (GameState.Attacker + 1) % GameState.Players.Count;

            for (int i = 0; i < GameState.Players.Count; i++)
            {
                if (Deck.Cards.Count == 0) return;
                int takesCards = (i + takesCardsFirst) % GameState.Players.Count;

                int mustBeAdded = 6 - GameState.Players[takesCards].Cards.Count;
                int issuedCardsNumber = Deck.Cards.Count > mustBeAdded ? mustBeAdded : Deck.Cards.Count;

                for (int j = 0; j < issuedCardsNumber; j++)
                {
                    int randomCardNumber = rnd.Next(0, Deck.Cards.Count);
                    GameState.Players[takesCards].Cards.Add(Deck.Cards[randomCardNumber]);
                    Deck.Cards.RemoveAt(randomCardNumber);
                }
            }
        }

        //************************************************************************************************

        /// <summary>
        /// Виводить на екран всі МОЇ поточні карти
        /// </summary>
        public static void ShowMyCards()
        {
            Console.WriteLine("\n                       ** your cards **");
            foreach (var card in GameState.Players[0].Cards)
                Console.WriteLine("                        " + card.Current);
        }

        /// <summary>
        /// Виводить на екран всі поточні карти на столі
        /// </summary>
        public static void ShowTable()
        {
            Console.WriteLine("\n                         ** table **");

            foreach (var pair in Table.CardsPairs)
                Console.WriteLine($"                 {pair.Item1.Current} => {pair.Item2.Current}");

            foreach (var card in Table.TakenCards)
                Console.WriteLine($"                      =X {card.Current} X=");
            //for (int i = 1; i < Table.TakenCards.Count; i += 2)
            //    Console.WriteLine($"[{Table.TakenCards[i - 1].Current}] =X= [{Table.TakenCards[i].Current}]");
            //Console.WriteLine($"     [{Table.TakenCards[Table.TakenCards.Count - 1].Current}] =X [X]");

            Console.WriteLine();
        }

        //************************************************************************************************
        /* // SimulateMyAttack()
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
                ShowMyCards();
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
        */
        /* // SimulateDefendingAttackWithComputer()
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

    ShowTable();
}
*/
        /* // SimulateDefenceAbandoningByComputer()
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

                ShowTable();
                ShowMyCards();
                Console.Write("\n\n                 -Choose the next giving card!-"
                    + "\n                      (or \"0\" to pass)\n                             ");

                attackCardIndex = ChooseMyCard(CardPurpose.Giving);
                if (attackCardIndex == -1) break;
            }
            int takenCardsNumber = ReplaceAllTakenCardsToPlayer();
            Console.WriteLine($"\n               =( Player {GameStatus.Attacked + 1} takes {takenCardsNumber} card(-s) )="
                + "\n                      (and miss a turn)");
            GameStatus.ResetAttackingPlayer();
        }
        */
        /* // FindHigherCards()
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
                if (Table.AttackingCard.Current.Key != GameStatus.TrumpSuit
                    && player.Cards[i].Current.Key == GameStatus.TrumpSuit)
                    allHigherCards.Add(player.Cards[i]);

                if (player.Cards[i].Current.Key == Table.AttackingCard.Current.Key
                    && player.Cards[i].Current.Value > Table.AttackingCard.Current.Value)
                    allHigherCards.Add(player.Cards[i]);
            }
            return allHigherCards;
        }
        */
        /* // FindSmallestHigherCard()
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

        /* // ChooseMyCard()
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
        */
        /* // IsThisSuitableForAttack()
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
        */
        /* // IsThisSuitableForGiving()
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
        */
        /* // IsThisSuitableForDefence()
        /// <summary>
        /// Перевіряє чи дана захисна карта може бути використана ДЛЯ ВІДБИВАННЯ АТАКИ звіряючись з атакуючою карторю
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForDefence()
        {
            return Table.AttackingCard.Current.Key != GameStatus.TrumpSuit
                    && Table.DefendingCard.Current.Key == GameStatus.TrumpSuit
                    || Table.DefendingCard.Current.Key == Table.AttackingCard.Current.Key
                    && Table.DefendingCard.Current.Value > Table.AttackingCard.Current.Value;
        }
        */
        //************************************************************************************************

        /// <summary>
        /// Переміщує всі карти зі столу до карт "до знімання"
        /// </summary>
        public static void ReplaceAllToTakenCards()
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
        public static int ReplaceAllTakenCardsToPlayer()
        {
            int takenCardsNumber = Table.TakenCards.Count;
            GameState.Players[GameState.Attacked].Cards.AddRange(Table.TakenCards);
            Table.TakenCards.Clear();
            return takenCardsNumber;
        }

        /// <summary>
        /// Переміщує всі карти зі столу до відбою
        /// </summary>
        public static void ReplaceAllToDiscardPile()
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