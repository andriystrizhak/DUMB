using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public static class Trump
    {
        public static Suits Suit = GenerateRandomTrump();

        /// <summary>
        /// Генерує випадковим чином козирну масть
        /// </summary>
        /// <returns>Конкретну рандомну масть</returns>
        static Suits GenerateRandomTrump()
        {
            Random rnd = new Random();
            return (Suits)rnd.Next(0, 4);
        }
    }
}