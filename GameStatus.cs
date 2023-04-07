using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    /// <summary>
    /// Містить основні дані поточної гри
    /// </summary>
    public static class GameStatus
    {
        public static Player[] Players { get; private set; }
        public static int Attacker { get; private set; }
        public static int Attacked { get; private set; }
        /// <summary>
        /// Поточна козирна масть
        /// </summary>
        public static Suits TrumpSuit { get; private set; }

        static GameStatus()
        {
            GetPlayersNumber(2);
            GetNewTrumpSuit();
        }

        /// <summary>
        /// Випадковим чином генерує нову козирну масть
        /// </summary>
        private static void GetNewTrumpSuit()
        {
            Random rnd = new Random();
            TrumpSuit = (Suits)rnd.Next(0, 4);
        }

        /// <summary>
        /// Створює масив гравців Player
        /// </summary>
        /// <param name="playersNumber">Кількість гравців</param>
        public static void GetPlayersNumber(int playersNumber) 
        {
            Players = new Player[playersNumber];
            for (int i = 0; i < Players.Length; i++)
                Players[i] = new Player();
        }

        /// <summary>
        /// Здійснює перехід ходу
        /// </summary>
        public static void ChangeAttackingPlayer()
        {
            Attacker = ++Attacker % Players.Length;
            Attacked = (Attacker + 1) % Players.Length;
        }
    }
}
