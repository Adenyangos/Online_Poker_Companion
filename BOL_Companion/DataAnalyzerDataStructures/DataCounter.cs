using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DataAnalyzerDataStructures
{
    /// <summary>
    /// A record of the poker actions (folds, checks, calls and bets) taken under specific hand circumstances (the current table or any table, the current number of players at the table or any number of players at the table) for each betting round (preflop, flop, turn and river).
    /// </summary>
    class DataCounter
    {
        /// <summary>
        /// All the preflop poker actions taken under each of the possible hand circumstances. 
        /// </summary>
        public HandCircumstances Preflop { get; set; }

        /// <summary>
        /// All the flop poker actions taken under each of the possible hand circumstances. 
        /// </summary>
        public HandCircumstances Flop { get; set; }

        /// <summary>
        /// All the turn poker actions taken under each of the possible hand circumstances. 
        /// </summary>
        public HandCircumstances Turn { get; set; }

        /// <summary>
        /// All the river poker actions taken under each of the possible hand circumstances. 
        /// </summary>
        public HandCircumstances River { get; set; }

        /// <summary>
        /// Initialize DataCounter with zerod HandCircumstances instances.
        /// </summary>
        public DataCounter()
        {
            Preflop = new HandCircumstances();
            Flop = new HandCircumstances();
            Turn = new HandCircumstances();
            River = new HandCircumstances();
        }
    }
}
