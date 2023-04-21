﻿using System;
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
        int movesLeft { get; set; }

        //******************************( АТАКА / ОБОРОНА / ПІДКИДУВАННЯ )*******************************

        /// <summary>
        /// Симулює атаку ГРАВЦЯ
        /// </summary>
        public static void MyAttack()
        {
            //TODO - щоб перше коло максимум 5 карт атака
            // - перевіряючи відбій
            //TODO - не дозволяти "пропускати хід" не кинувши першу карту

            Console.WriteLine("\n                 =======( Your turn )=======");
            int possibleAttacksNumber = Gameplay.PossibleAttacksNumber();

            //Цикл атак
            for (int i = 0; i < possibleAttacksNumber; i++)
            {
                Gameplay.ShowMyCards();
                Console.Write(i == 0 ? ("\n                -Choose your attacking card!-"
                    + "\n                      (or \"0\" to pass)\n                              ")
                    : "\n\n                -Choose the next attacking card!-");

                //Вибір ГРАВЦЕМ атакуючої карти
                if (ChooseMyCard(CardPurpose.Attack)) break;
                //Виклик реакції КОМП'ЮТОРА на атаку
                if (ComputerActions.ComputerAttackResponse(possibleAttacksNumber - i)) break;
            }
            //REMOVE - Gameplay.ReplaceAllToDiscardPile();
            Console.WriteLine("\n\n             ==( You have completed the attack )== ");
        }

        /// <summary>
        /// Cимулює реакцію ГРАВЦЯ на атаку
        /// </summary>
        /// <param name="remainingAttacksNumber">Кількість можливих атак, що залишилась</param>
        /// <param name="i">Кількість вже здійснених атак</param>
        /// <returns>Значення типу bool що вказує чи переривати цикл атак КОМП'ЮТОРА, чи ні</returns>
        public static bool MyAttackResponse(int movesLeft, int i)
        {
            Gameplay.ShowMyCards();
            Console.Write((i == 0 ? "\n                 -Choose your defending card!-"
                : "\n\n                 -Choose the next defending card!-")
                + "\n                  (or \"0\" to abandone defence)\n                              ");

            //Вибір ГРАВЦЕМ карти для оборони
            if (ChooseMyCard(CardPurpose.Defence))
            {
                //У випадку невдачі - знімання карт ГРАВЦЕМ
                ComputerActions.GivingCardsByComputer(movesLeft - 1);
                return true;
            }
            CardsRemoving();
            return false;
        }

        /// <summary>
        /// Симулює випадок, коли ГРАВЕЦЬ підкидує карти для знімання
        /// </summary>
        /// <returns>Значення типу bool що вказує чи переривати цикл підкидування ГРАВЦЕМ, чи ні</returns>
        public static void GivingCardsByMe(int movesLeft)
        {
            for (int i = 0; i < movesLeft; i++)
            {
                Gameplay.ShowMyCards();
                Console.Write("\n\n                 -Choose the next giving card!-"
                    + "\n                      (or \"0\" to pass)\n                             ");

                //Вибір ГРАВЦЕМ карти для циклу підкидування
                if (ChooseMyCard(CardPurpose.Giving)) break;
            }
        }


        //***********************************( ВИБІР КАРТИ ГРАВЦЕМ )*************************************

        /// <summary>
        /// Реалізує вибір карти ГРАВЦЕМ для: атаки, підкидування чи захисту
        /// </summary>
        /// <param name="purpose">Призначення вибору карти</param>
        /// <returns>Значення типу bool що вказує чи перериває ГРАВЕЦЬ цикл атаки / оборони / підкидування , чи ні</returns>
        static bool ChooseMyCard(CardPurpose purpose)
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


        //****************************( ПЕРЕВІРКА КАРТИ НА "ПІДХОДЯЩІСТЬ" )******************************

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
            return IsSameTypeCard(tableCards);
        }

        /// <summary>
        /// Перевіряє чи дана атакуюча карта може бути використана ДЛЯ ПІДКИДУВАННЯ звіряючись з картами "до знімання"
        /// </summary>
        /// <returns>Значення типу bool що вказує чи підходить ця карта, чи ні</returns>
        static bool IsThisSuitableForGiving()
            => IsSameTypeCard(Table.TakenCards);

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

            //Якщо ця оборонна карта не підходить, то вона стає null
            if (!isSutable)
                Table.DefendingCard = null;

            return isSutable;
        }

        /// <summary>
        /// Визначає чи є в списку карта такого ж типу(рангу) що й "атакуюча"
        /// </summary>
        /// <param name="cards">Список карт</param>
        /// <returns>начення типу bool що вказує чи є така карта, чи ні</returns>
        static bool IsSameTypeCard(List<Card> cards)
        {
            //Перевірка того чи є карта такого ж типу(рангу) в "cards"
            bool isSameTypeCard = false;
            foreach (var card in cards)
                if (card.Current.Value == Table.AttackingCard.Current.Value)
                    isSameTypeCard = true;

            //Якщо немає, то атакуюча стає null
            if (!isSameTypeCard)
                Table.DefendingCard = null;

            return isSameTypeCard;
        }


        //++++++++++++++++++++++++++++++++++++( TODO - до Gameplay )+++++++++++++++++++++++++++++++++++++
        //СЛУЖБОВЕ
        /// <summary>
        /// Переміщує атакуючу і обороняючу карти з рук гравців до "столу"
        /// </summary> 
        public static void CardsRemoving()
        {
            Table.CardsPairs.Add((Table.AttackingCard, Table.DefendingCard)); //TODO - in particular method

            for (int j = 0; j < GameState.Players[GameState.Attacker].Cards.Count; j++)
            {
                if (GameState.Players[GameState.Attacker].Cards[j] == Table.AttackingCard)
                    GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
                else if (GameState.Players[GameState.Attacker].Cards[j] == Table.DefendingCard)
                    GameState.Players[GameState.Attacker].Cards.RemoveAt(j);
            }
            for (int j = 0; j < GameState.Players[GameState.Attacked].Cards.Count; j++)
            {
                if (GameState.Players[GameState.Attacked].Cards[j] == Table.AttackingCard)
                    GameState.Players[GameState.Attacked].Cards.RemoveAt(j);
                else if (GameState.Players[GameState.Attacked].Cards[j] == Table.DefendingCard)
                    GameState.Players[GameState.Attacked].Cards.RemoveAt(j);
            }
            //TODO - а після видалення кількість карт в лісті не зміниться?
            Gameplay.ShowTable();
        }
    }
}
