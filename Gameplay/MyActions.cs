using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Gameplay
{
    /// <summary>
    /// Містить інструкції для симуляції поведінки ГРАВЦЯ за різних ситуацій
    /// </summary>
    public class MyActions
    {
        /// <summary>
        /// Симулює атаку ГРАВЦЯ
        /// </summary>
        public static void SimulateMyAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - перевіряючи відбій
            //TODO - не дозволяти "пропускати хід" не кинувши першу карту

            Console.WriteLine("\n                 =======( Your turn )=======");
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
                //Вибір МНОЮ атакуючої карти
                Gameplay.ShowMyCards();
                Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                    : "\n\n                -Choose the next attacking card!-")
                    + "\n                      (or \"0\" to pass)\n                              ");
                int myAttackingCardIndex = ChooseMyCard(CardPurpose.Attack);
                if (myAttackingCardIndex == -1) break;

                //Симуляція реакції комп'ютора на атаку
                */
                //Симуляція атаки ГРАВЦЯ
                if (MyAttack(possibleAttacksNumber - i, i))
                    break;
                /*
var higherCards = ComputerActions.FindHigherCards();
if (higherCards.Count > 0)
    ComputerActions.SimulateDefendingAttackWithComputer(higherCards);
else
{
    ComputerActions.SimulateDefenceAbandoningByComputer(myAttackingCardIndex, possibleAttacksNumber - i);
    break;
}
*/
            }
            Gameplay.ReplaceAllToDiscardPile();
        }


        //******************************( АТАКА / ОБОРОНА / ПІДКИДУВАННЯ )*******************************

        /// <summary>
        /// Cимулює атаку ГРАВЦЯ
        /// </summary>
        /// <param name="remainingAttacksNumber">Кількість можливих атак, що залишилась</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак ГРАВЦЯ, чи ні</returns>
        public static bool MyAttack(int remainingAttacksNumber, int i)
        {
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                -Choose your attacking card!-"
                : "\n\n                -Choose the next attacking card!-")
                + "\n                      (or \"0\" to pass)\n                              ");

            //Вибір ГРАВЦЕМ атакуючої карти
            //REMOVE - int myAttackingCardIndex = ChooseMyCard(CardPurpose.Attack);
            if (ChooseMyCard(CardPurpose.Attack)) return true;

            //Виклик реакції КОМП'ЮТОРА на атаку
            if (ComputerActions.ComputerResponseToAttack(remainingAttacksNumber))
                return true;
            return false;
        }

        /// <summary>
        /// Cимулює реакцію ГРАВЦЯ на атаку комп'ютора
        /// </summary>
        /// <param name="remainingAttacksNumber">Кількість можливих атак, що залишилась</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак КОМП'ЮТОРА, чи ні</returns>
        public static bool MyAttackResponse(int remainingAttacksNumber, int i)
        {
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                 -Choose your defending card!-"
                : "\n\n                -Choose the next defending card!-")
                + "\n                  (or \"0\" to abandone defence)\n                              ");

            //Вибір ГРАВЦЕМ карти для оборони
            //REMOVE - int myDefendingCardIndex = ChooseMyCard(CardPurpose.Defence);
            if (ChooseMyCard(CardPurpose.Defence))
            {
                //У випадку невдачі - знімання карт ГРАВЦЕМ
                ComputerActions.SimulateDefenceAbandoningByMe(remainingAttacksNumber - 1);
                return true;
            }
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

        /// <summary>
        /// Симулює випадок, коли ГРАВЕЦЬ підкидує карти для знімання
        /// </summary>
        /// <returns>Значення типу bool що вказує чи переривати цикл підкидування ГРАВЦЕМ, чи ні</returns>
        public static bool GivingCardsByMe()
        {
            Gameplay.ShowMyCards();
            Console.Write("\n\n                 -Choose the next giving card!-"
                + "\n                      (or \"0\" to pass)\n                             ");

            //Вибір ГРАВЦЕМ карти для циклу підкидування
            //REMOVE - int attackCardIndex = ChooseMyCard(CardPurpose.Giving);
            if (ChooseMyCard(CardPurpose.Giving)) return true;
            return false;
        }


        //*********************************( ВИБІР КАРТИ ГРАВЦЕМ )****************************************

        /// <summary>
        /// Реалізує вибір карти ГРАВЦЕМ для: атаки, підкидування чи захисту
        /// </summary>
        /// <param name="purpose">Призначення вибору карти</param>
        /// <returns>Значення типу bool що вказує чи перериває ГРАВЕЦЬ цикл атаки / оборони / підкидування , чи ні</returns>
        public static bool ChooseMyCard(CardPurpose purpose)
        {
            string myAnswer;
            int selectedCardIndex = 0;
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
                    return true;

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
            return false;
        }


        //**************************( ПЕРЕВІРКА КАРТИ НА "ПІДХОДЯЩІСТЬ" )**********************************

        /// <summary>
        /// Перевіряє чи дана атакуюча карта може бути використана ДЛЯ АТАКИ звіряючись з картами на "столі"
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForAttack()
        {
            if (Table.CardsPairs.Count == 0) return true;

            //Список, що міститиме карти зі "столу"
            List<Card> tableCards = new List<Card>();
            foreach (var cardPair in Table.CardsPairs)
            {
                tableCards.Add(cardPair.Item1);
                tableCards.Add(cardPair.Item2);
            }

            //Перевірка чи є карта такого ж типу(рангу)
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
        /// Перевіряє чи дана атакуюча карта може бути використана ДЛЯ ПІДКИДУВАННЯ звіряючись з картами "до знімання"
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForGiving()
        {
            //Перевірка чи є карта такого ж типу(рангу) в картах "на знімання"
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
        /// Перевіряє чи дана захисна карта може бути використана ДЛЯ ОБОРОНИ звіряючись з атакуючою карторю
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

        //TODO - Створити метод IsSameTypeCard()






        //+++++++++++++++++++++++++++++++++( TODO - до Gameplay )++++++++++++++++++++++++++++++++++++++++
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

    }
}
