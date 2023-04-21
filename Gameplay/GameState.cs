using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Durak.Gameplay
{
    /// <summary>
    /// Містить основні дані поточної гри
    /// </summary>
    public static class GameState
    {
        public static List<Player> Players { get; private set; } = new List<Player>();
        public static int Attacker { get; private set; }
        public static int Attacked { get; private set; }

        /// <summary>
        /// Поточна козирна масть
        /// </summary>
        public static Suits TrumpSuit { get; private set; }

        static GameState()
        {
            SetPlayersQuantity(2);
            SetNewTrumpSuit();
        }

        /// <summary>
        /// Випадковим чином генерує нову козирну масть
        /// </summary>
        private static void SetNewTrumpSuit()
        {
            Random rnd = new Random();
            TrumpSuit = (Suits)rnd.Next(0, 4);
        }

        /// <summary>
        /// Створює List<Player> гравців
        /// </summary>
        /// <param name="playersNumber">Кількість гравців</param>
        public static void SetPlayersQuantity(int playersNumber)
        {
            Players = new List<Player>();
            for (int i = 0; i < playersNumber; i++)
            {
                if (i == 0)
                    Players.Add(new Me());
                else
                    Players.Add(new Computer());
            }
        }

         
        public static void RemovePlayer(int playerNumber)
        {
            Players[playerNumber] = null;
        }

        /// <summary>
        /// Обирає на початку гри гравця який ходитиме першим
        /// </summary>
        /// <returns>Номер гравця, який ходитиме першим</returns>
        public static void ChoosePlayerWhoStarts()
        {
            int playerNumber = -1;
            int cardIndex = -1;
            bool flag = true;

            for (int i = 0; flag && i < Players.Count; i++)
                for (int j = 0; flag && j < Players[i].Cards.Count; j++)
                    if (Players[i].Cards[j].Current.Key == TrumpSuit)
                    {
                        playerNumber = i;
                        cardIndex = j;
                        flag = false;
                    }
            if (playerNumber != -1)
            {
                Card currentCard = Players[playerNumber].Cards[cardIndex];
                for (int i = playerNumber; i < Players.Count; i++)
                    for (int j = 0; j < Players[i].Cards.Count; j++)
                        if (Players[i].Cards[j].Current.Key == TrumpSuit
                            && Players[i].Cards[j].Current.Value < currentCard.Current.Value)
                        {
                            currentCard = Players[i].Cards[j];
                            playerNumber = i;
                        }
                Attacker = playerNumber;
            }
            else
                Attacker = 0;

            Attacked = (Attacker + 1) % Players.Count;
        }

        /// <summary>
        /// Здійснює перехід ходу до іншого гравця
        /// </summary>
        public static void ResetAttackingPlayer()
        {
            Attacker = ++Attacker % Players.Count;
            Attacked = (Attacker + 1) % Players.Count;
        }
    }
}