using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Gameplay
{
    public class MyActions
    {
        /// <summary>
        /// Симулює реакцію комп'ютора на атаку
        /// </summary>
        public static void SimulateMyAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - ввести КАУНТЕР КІЛ (??)
            // - перевіряючи відбій
            //TODO - не дозволяти "пропускати хід" не кинувши першу карту

            Console.WriteLine("\n                 =======( Your turn )=======");
            int possibleAttacksNumber = 
                GameState.Players[GameState.Attacker].Cards.Count > 6
                ? 6 : GameState.Players[GameState.Attacker].Cards.Count 
                > GameState.Players[GameState.Attacked].Cards.Count
                ? GameState.Players[GameState.Attacked].Cards.Count 
                : GameState.Players[GameState.Attacker].Cards.Count;

            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                Gameplay.ShowMyCards();
                Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                    : "\n\n                -Choose the next attacking card!-")
                    + "\n                      (or \"0\" to pass)\n                              ");

                int myAttackingCardIndex = ChooseMyCard(CardPurpose.Attack);
                if (myAttackingCardIndex == -1) break;

                var higherCards = ComputerActions.FindHigherCards();
                if (higherCards.Count > 0)
                    ComputerActions.SimulateDefendingAttackWithComputer(higherCards);
                else
                {
                    ComputerActions.SimulateDefenceAbandoningByComputer(myAttackingCardIndex, possibleAttacksNumber - i);
                    break;
                }
            }
            Gameplay.ReplaceAllToDiscardPile();
        }

        //************************************************************************************************

        /// <summary>
        /// Cимулює МОЮ реакцію на атаку комп'ютора
        /// </summary>
        /// <param name="possibleAttacksNumber">Початкова кількість можливих атак</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак комп'ютора, чи ні</returns>
        public static bool MyResponseToComputerAttack(int possibleAttacksNumber, int i)
        {
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                 -Choose your defending card!-"
                : "\n\n                -Choose the next defending card!-")
                + "\n                  (or \"0\" to abandone defence)\n                              ");

            //Вибір МНОЮ карти для оборони
            int myDefendingCardIndex = ChooseMyCard(CardPurpose.Defence);
            if (myDefendingCardIndex == -1)
            {
                //У випадку невдачі - знімання карт МНОЮ
                ComputerActions.SimulateDefenceAbandoningByMe(possibleAttacksNumber - i - 1);
                return true;
            }
            //Видалення використаних карт з рук гравців
            /*
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
            CardsRemoving();
            return false;
        }

        public static bool DefenseAbandoningOfMYAttackByComputer()
        {
            Gameplay.ShowMyCards();
            Console.Write("\n\n                 -Choose the next giving card!-"
                + "\n                      (or \"0\" to pass)\n                             ");

            //TODO - не обов'язково я, можливо й інший робот
            int attackCardIndex = ChooseMyCard(CardPurpose.Giving);
            if (attackCardIndex == -1) return true;
            return false;
        }


        //************************************************************************************************

        /// <summary>
        /// Видаляє атакуючу і обороняючу карти з рук гравців
        /// </summary>
        public static void CardsRemoving()
        {
            Table.CardsPairs.Add((Table.AttackingCard, Table.DefendingCard)); //TODO - in particular method

            for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
            {
                if (GameState.Players[GameState.Attacker].Cards[j] == Table.AttackingCard)
                    GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
                if (GameState.Players[GameState.Attacker].Cards[j] == Table.DefendingCard)
                    GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
            }

            for (int j = 0; j < GameState.Players[GameState.Attacked].Cards.Count; j++)
            {
                if (GameState.Players[GameState.Attacked].Cards[j] == Table.AttackingCard)
                    GameState.Players[GameState.Attacked].Cards.RemoveAt(j);
                if (GameState.Players[GameState.Attacked].Cards[j] == Table.DefendingCard)
                    GameState.Players[GameState.Attacked].Cards.RemoveAt(j);
            }
            //GameState.Players[GameState.Attacked].Cards.RemoveAt(myDefendingCardIndex);

            Gameplay.ShowTable();
        }

        //************************************************************************************************

        /// <summary>
        /// Реалізує вибір карти МНОЮ для: атаки, докидування опоненту чи захисту
        /// </summary>
        /// <param name="purpose">Призначення вибору карти</param>
        /// <returns>Номер вибраної карти в МОЇЙ колоді</returns>
        public static int ChooseMyCard(CardPurpose purpose)
        {
            string myAnswer;
            int selectedCardIndex = 0;
            //Table.AttackingCard = null;
            bool cardIsSuitable = false;
            int errorCounter = 0;

            while (!cardIsSuitable)
            {
                Console.Write(errorCounter == 0 ? "" 
                    : "                 -this card is not suitable-\n                              ");
                errorCounter++;
                myAnswer = Console.ReadLine();

                while (!int.TryParse(myAnswer, out selectedCardIndex)
                    || selectedCardIndex > GameState.Players[0].Cards.Count 
                    || selectedCardIndex < 0)
                {
                    Console.Write("                    -write correct number-\n                              ");
                    myAnswer = Console.ReadLine();
                }
                if (--selectedCardIndex == -1)
                    return selectedCardIndex;

                if (purpose != CardPurpose.Defence)
                {
                    Table.AttackingCard = GameState.Players[GameState.Attacker].Cards[selectedCardIndex];
                    if (purpose == CardPurpose.Attack)
                        cardIsSuitable = IsThisSuitableForAttack();
                    else
                        cardIsSuitable = IsThisSuitableForGiving();
                }
                else
                {
                    Table.DefendingCard = GameState.Players[GameState.Attacked].Cards[selectedCardIndex];
                    cardIsSuitable = IsThisSuitableForDefence();
                }
            }
            return selectedCardIndex;
        }

        //************************************************************************************************

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

            //NEWFEATURE - check
            if (!isSameTypeCard)
                Table.AttackingCard = null;

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

            //NEWFEATURE - check
            if (!isSameTypeCard)
                Table.AttackingCard = null;

            return isSameTypeCard;
        }

        /// <summary>
        /// Перевіряє чи дана захисна карта може бути використана ДЛЯ ВІДБИВАННЯ АТАКИ звіряючись з атакуючою карторю
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForDefence()
        {
            bool isSutable = Table.AttackingCard.Current.Key != GameState.TrumpSuit
                    && Table.DefendingCard.Current.Key == GameState.TrumpSuit
                    || Table.DefendingCard.Current.Key == Table.AttackingCard.Current.Key
                    && Table.DefendingCard.Current.Value > Table.AttackingCard.Current.Value;

            //TODO - implement 
            if (!isSutable)
                Table.DefendingCard = null;

            return isSutable;
        }
    }
}
