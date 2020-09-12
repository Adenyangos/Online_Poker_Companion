using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DataAnalyzerDataStructures
{
    /// <summary>
    /// A record of the poker actions (folds, checks, calls and bets) taken.
    /// </summary>
    class PokerActions
    {
        /// <summary>
        /// The number of fold actions.
        /// </summary>
        public long Fold { get; set; } = 0;

        /// <summary>
        /// The number of check actions.
        /// </summary>
        public long Check { get; set; } = 0;

        /// <summary>
        /// The number of call actions.
        /// </summary>
        public long Call { get; set; } = 0;

        /// <summary>
        /// The number of bet actions.
        /// </summary>
        public long Bet { get; set; } = 0;

        /// <summary>
        /// Initialize poker actions with 0 folds, 0 checks, 0 calls and 0 bets.
        /// </summary>
        public PokerActions()
        {

        }

        /// <summary>
        /// The possible poker actions (bet, call, check and fold).
        /// </summary>
        public enum PossibleActions
        {
            Bet,
            Call,
            Check,
            Fold
        }
    }
}
