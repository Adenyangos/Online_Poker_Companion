using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DataAnalyzerDataStructures
{
    /// <summary>
    /// A record of the poker actions (folds, checks, calls and bets) taken under specific hand circumstances (the current table or any table, the current number of players at the table or any number of players at the table).
    /// </summary>
    class HandCircumstances
    {
        /// <summary>
        /// All the poker actions taken at the current table with the current number of players.
        /// </summary>
        public PokerActions ThisTableCurrNumPlayers { get; set; }

        /// <summary>
        /// All the poker actions taken at the current table with any number of players.
        /// </summary>
        public PokerActions ThisTableAnyNumPlayers { get; set; }

        /// <summary>
        /// All the poker actions taken at any table with the current number of players.
        /// </summary>
        public PokerActions AnyTableCurrNumPlayers { get; set; }

        /// <summary>
        /// All the poker actions taken at any table with any number of players.
        /// </summary>
        public PokerActions AnyTableAnyNumPlayers { get; set; }

        /// <summary>
        /// Initialize HandCircumstances with zerod PokerActions instances.
        /// </summary>
        public HandCircumstances()
        {
            ThisTableCurrNumPlayers = new PokerActions();
            ThisTableAnyNumPlayers = new PokerActions();
            AnyTableCurrNumPlayers = new PokerActions();
            AnyTableAnyNumPlayers = new PokerActions();
        }
    }
}
