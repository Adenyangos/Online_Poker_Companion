using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A class that represents the betting rounds in a poker hand.
    /// </summary>
    public static class BettingRound
    {
        /// <summary>
        /// The four betting rounds
        /// </summary>
        public enum Round
        {
            PreFlop,
            Flop,
            Turn,
            River
        }
    }
}
