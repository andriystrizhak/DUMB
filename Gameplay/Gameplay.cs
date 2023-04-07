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
        //{
        public static Player[] Players;
        public static int Attacker;
        public static int Attacked;
        //} - separate to static class


        //TODO - Реалізувати закінчення гри й висвітлення переможця (якщо кількість карт одного з гравців + відбій == 36).
        // - А краще висвітлювати вихід гравця з гри в якого 0 карт при тому що в колоді також 0.
        // - А також врахувати випадок НІЧИЇ.

        public static void Main()
        {
            //{
            int playersNumber = 2;
            Players = new Player[playersNumber];
            for (int i = 0; i < Players.Length; i++)
                Players[i] = new Player();
            //} - separate

            ToSayHello();

            bool isTheEnd = false;
            while (!isTheEnd)
            {
                if (Attacker == 0) SimulateMyAttack();
                else SimulateComputerAttack();

                Attacker = ++Attacker % Players.Length;
                Attacked = (Attacker + 1) % Players.Length;
                DistributeCards();
                Console.WriteLine("\n\n*****************************************************************");

                for (int i = 0; i < Players.Length; i++)
                    if (Players[i].Cards.Count + DiscardPile.Cards.Count == 36)
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
            }
            Console.Write("                What's your final words, man?\n                              ");
            Console.ReadLine();
            Console.WriteLine("                 You better not say anything."
                + "\n                     Okay, see you later!");
            Console.ReadKey(true);
        }

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
            Console.Write("             -So, press any button to start game!-\n                              ");
            Console.ReadKey(true);
            Console.WriteLine("\n*****************************************************************");

            Console.WriteLine($"\n                  =@( The Trump Suit is: )@="
                + $"\n                         =>[{Trump.Suit}]<=");
            DistributeCards();
            Console.WriteLine("\n*****************************************************************");

            var attackerTrumpCard = ChoosePlayerWhoStarts();
            Attacker = attackerTrumpCard.Item2;
            Attacked = (Attacker + 1) % Players.Length;

            Console.WriteLine(Attacker == 0
                ? "\n                =( The first move is yours )="
                : $"\n                 ===( Player {Attacker + 1} goes first )==="
                + $"\n                (Lowest trump is: {attackerTrumpCard.Item1.Current})");
        }

        //************************************************************************************************

        /// <summary>
        /// Обирає на початку гри гравця який ходитиме першим
        /// </summary>
        /// <returns>Номер гравця, який ходитиме першим</returns>
        static (Card, int) ChoosePlayerWhoStarts()
        {
            int playerNumber = -1;
            int cardIndex = -1;
            bool flag = true;
            for (int i = 0; flag && i < Players.Length; i++)
                for (int j = 0; flag && j < Players[i].Cards.Count; j++)
                    if (Players[i].Cards[j].Current.Key == Trump.Suit)
                    {
                        playerNumber = i;
                        cardIndex = j;
                        flag = false;
                    }
            if (playerNumber != -1)
            {
                Card currentCard = Players[playerNumber].Cards[cardIndex];
                for (int i = playerNumber; i < Players.Length; i++)
                    for (int j = 0; j < Players[i].Cards.Count; j++)
                        if (Players[i].Cards[j].Current.Key == Trump.Suit
                            && Players[i].Cards[j].Current.Value < currentCard.Current.Value)
                        {
                            currentCard = Players[i].Cards[j];
                            playerNumber = i;
                        }
                return (currentCard, playerNumber);
            }
            else return (Players[playerNumber + 1].Cards[cardIndex + 1], 0);
        }

        /// <summary>
        /// Роздає всім гравцям стільки карт скільки їм не вистачає 
        /// </summary>
        static void DistributeCards()
        {
            Random rnd = new Random();
            int takesCardsFirst = (Attacker + 1) % Players.Length;

            for (int i = 0; i < Players.Length; i++)
            {
                if (Deck.Cards.Count == 0) return;
                int takesCards = (i + takesCardsFirst) % Players.Length;

                int mustBeAdded = 6 - Players[takesCards].Cards.Count;
                int issuedCardsNumber = Deck.Cards.Count > mustBeAdded ? mustBeAdded : Deck.Cards.Count;

                for (int j = 0; j < issuedCardsNumber; j++)
                {
                    int randomCardNumber = rnd.Next(0, Deck.Cards.Count);
                    Players[takesCards].Cards.Add(Deck.Cards[randomCardNumber]);
                    Deck.Cards.RemoveAt(randomCardNumber);
                }
            }
        }

        //************************************************************************************************

        /// <summary>
        /// Виводить на екран всі МОЇ поточні карти
        /// </summary>
        static void ShowMyCards()
        {
            Console.WriteLine("\n                       ** your cards **");
            foreach (var card in Players[0].Cards)
                Console.WriteLine("                        " + card.Current);
        }

        /// <summary>
        /// Виводить на екран всі поточні карти на столі
        /// </summary>
        static void ShowTable()
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
        //************************************************************************************************


        /// <summary>
        /// Симулює реакцію комп'ютора на атаку
        /// </summary>
        static void SimulateMyAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака [ввести КАУНТЕР КІЛ (??)]
            Console.WriteLine("\n                 =======( Your turn )=======");
            int possibleAttacksNumber = Players[Attacker].Cards.Count > 6
                ? 6 : Players[Attacker].Cards.Count > Players[Attacked].Cards.Count
                ? Players[Attacked].Cards.Count : Players[Attacker].Cards.Count;

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
                    || selectedCardIndex > Players[0].Cards.Count || selectedCardIndex < 0)
                {
                    Console.Write("                    -write correct number-\n                              ");
                    myAnswer = Console.ReadLine();
                }
                if (--selectedCardIndex == -1)
                    return selectedCardIndex;

                if (purpose != CardPurpose.Defence)
                {
                    Table.AttackingCard = Players[Attacker].Cards[selectedCardIndex];
                    if (purpose == CardPurpose.Attack)
                        cardIsSuitable = IsThisSuitableForAttack();
                    else
                        cardIsSuitable = IsThisSuitableForGiving();
                }
                else
                {
                    Table.DefendingCard = Players[Attacked].Cards[selectedCardIndex];
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
            Console.WriteLine($"\n\n               ==( Player {Attacked + 1} defend the attack )==");

            var smallestHigherCard = FindSmallestHigherCard(higherCards);
            Table.CardsPairs.Add((Table.AttackingCard, smallestHigherCard));

            for (int i = 0; i < Players[Attacked].Cards.Count; i++)
                if (Players[Attacked].Cards[i] == smallestHigherCard)
                {
                    Players[Attacked].Cards.RemoveAt(i);
                    break;
                }
            Players[Attacker].Cards.RemoveAt(attackCardIndex);

            ShowTable();
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
                Console.WriteLine($"\n\n               =( Player {Attacked + 1} abandones the defence )=");
                Table.TakenCards.Add(Table.AttackingCard);
                Players[Attacker].Cards.RemoveAt(attackCardIndex);

                ShowTable();
                ShowMyCards();
                Console.Write("\n\n                 -Choose the next giving card!-"
                    + "\n                      (or \"0\" to pass)\n                             ");

                attackCardIndex = ChooseMyCard(CardPurpose.Giving);
                if (attackCardIndex == -1) break;
            }
            int takenCardsNumber = ReplaceAllTakenCardsToPlayer();
            Console.WriteLine($"\n               =( Player {Attacked + 1} takes {takenCardsNumber} card(-s) )="
                + "\n                      (and miss a turn)");
            Attacker++;
        }

        //************************************************************************************************

        /// <summary>
        /// Знаходить всі карти більші від атакуючої
        /// </summary>
        /// <returns>Ліст зі всіма більшими картами</returns>
        static List<Card> FindHigherCards()
        {
            var player = Players[Attacked];
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
            Players[Attacked].Cards.AddRange(Table.TakenCards);
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


        //************************************************************************************************
        //************************************************************************************************


        /// <summary>
        /// Симулює атаку комп'ютора
        /// </summary>
        static void SimulateComputerAttack()
        {
            Console.WriteLine($"\n\n                =====( Player's {Attacker + 1} turn )=====");
            int possibleAttacksNumber = Players[Attacker].Cards.Count > 6
                ? 6 : Players[Attacker].Cards.Count > Players[Attacked].Cards.Count
                ? Players[Attacked].Cards.Count : Players[Attacker].Cards.Count;

            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                if (i == 0) ChooseComputerFirstAttackingCard();
                else
                {
                    ChooseComputerAttackingCard();
                    if (Table.AttackingCard == null)
                    {
                        Console.WriteLine($"               ==( Player {Attacker + 1} completed the attack )==");
                        break;
                    }
                }

                ShowMyCards();
                Console.Write((i == 0 ? "\n                 -Choose your defending card!-"
                    : "\n\n                -Choose the next defending card!-")
                    + "\n                  (or \"0\" to abandone defence)\n                              ");

                int myDefendingCardIndex = ChooseMyCard(CardPurpose.Defence);
                if (myDefendingCardIndex == -1)
                {
                    SimulateDefenceAbandoningByMe(possibleAttacksNumber - i - 1);
                    break;
                }

                Table.CardsPairs.Add((Table.AttackingCard, Players[Attacked].Cards[myDefendingCardIndex])); //TODO - in particular method
                for (int j = 0; j < Players[Attacked].Cards.Count; j++)
                    if (Players[Attacker].Cards[j] == Table.AttackingCard)
                    {
                        Players[Attacker].Cards.RemoveAt(j);
                        break;
                    }
                Players[Attacked].Cards.RemoveAt(myDefendingCardIndex);
                ShowTable();
            }
            ReplaceAllToDiscardPile();
        }

        /// <summary>
        /// Симулює вибір першої атакуючої карти
        /// </summary>
        static void ChooseComputerFirstAttackingCard()
        {
            Table.AttackingCard = FindSmallestCard(Players[Attacker].Cards);

            Console.WriteLine("\n                         ** attack **");
            Console.WriteLine($"                        {Table.AttackingCard.Current} =>");

            Console.WriteLine();
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
                if (cards[i].Current.Key != Trump.Suit)
                    isTrumpCardThere = true;

            for (int i = 1; i < cards.Count; i++)
                if (currentCard.Current.Value > cards[i].Current.Value
                    && (isTrumpCardThere ? cards[i].Current.Key != Trump.Suit : true))
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

            if (smallestSutableCard.Current.Key == Trump.Suit
                && !doAttackWithTrumpCard)
            {
                Table.AttackingCard = null;
                return;
            }
            Table.AttackingCard = smallestSutableCard;

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

            foreach (var card in Players[Attacker].Cards)
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

        /// <summary>
        /// Симулює накидування МЕНІ карт на знімання
        /// </summary>
        /// <param name="movesLeft">Скільки карт ще може докинути РОБОТ</param>
        static void SimulateDefenceAbandoningByMe(int movesLeft)
        {
            ReplaceAllToTakenCards();
            int takenCardsNumber = Table.TakenCards.Count + 1;

            Console.WriteLine($"\n\n                 =( You abandone the defence )=");
            Table.TakenCards.Add(Table.AttackingCard);

            var sutableCards = FindSuitableForAttackingCards(CardPurpose.Giving);
            for (int i = 1; i < (movesLeft > sutableCards.Count ? sutableCards.Count : movesLeft); i++)
                Table.TakenCards.Add(sutableCards[i]);

            for (int j = 0; j < Players[Attacker].Cards.Count; j++)
                for (int k = 0; k < Table.TakenCards.Count; k++)
                    if (Players[Attacker].Cards[j].Current.Key == Table.TakenCards[k].Current.Key
                        && Players[Attacker].Cards[j].Current.Value == Table.TakenCards[k].Current.Value)
                        Players[Attacker].Cards.RemoveAt(j);

            ShowTable();
            takenCardsNumber = Table.TakenCards.Count - takenCardsNumber;
            if (takenCardsNumber != 0)
                Console.WriteLine($"\n               =( Player {Attacker + 1} gives you {takenCardsNumber} more cards )=");
            Console.WriteLine($"\n                  =( You take {Table.TakenCards.Count} card(-s) )="
                + "\n                      (and miss a turn)");

            ReplaceAllTakenCardsToPlayer();
            Attacker++;
        }
    }
}