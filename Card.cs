using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public class Card
    {
        public readonly KeyValuePair<Suits, Ranks> Current;
        public Card(KeyValuePair<Suits, Ranks> currentCard)
            => Current = currentCard;
    }
}