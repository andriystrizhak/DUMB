﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public static class Trump
    {
        public static readonly Suits Suit;

        static Trump()
        {
            Random rnd = new Random();
            Suit = (Suits)rnd.Next(0, 4);
        }
    }
}