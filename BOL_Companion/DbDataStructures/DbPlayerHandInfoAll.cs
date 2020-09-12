using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DbDataStructures
{
    /// <summary>
    /// The complete set of information about a singe hand for a single player. (handInfo, handPlayerInfoList, playerActionInfoList, boardActionInfoList)
    /// </summary>
    class DbPlayerHandInfoAll
    {
        /// <summary>
        /// The general information about a single hand a player was involved in. (handId, gameId, ante, handPlayerId) 
        /// </summary>
        public DbHandInfo HandInfo { get; set; }

        /// <summary>
        /// The list of information about all the players at the start of a hand. (handPlayerId, chipCountStart, blind)
        /// </summary>
        public List<DbHandPlayerInfo> HandPlayerInfoList { get; set; }

        /// <summary>
        /// The list of all the player actions in a hand. (handPlayerId, chipCountChange, handActionNumber)
        /// </summary>
        public List<DbPlayerActionInfo> PlayerActionInfoList { get; set; }

        /// <summary>
        /// The list of all the board actions in a hand. A board action is when a board card is dealt ie. flop 1, flop 2, flop 3, turn or river. (handActionNumber)
        /// </summary>
        public List<DbBoardActionInfo> BoardActionInfoList { get; set; }

        /// <summary>
        /// Initialize a DbPlayerHandInfoAll instance with a DbHandInfo instance.
        /// </summary>
        /// <param name="HandInfoIn">The DbHandInfo instance that represeents the hand and player you wish to represent.</param>
        public DbPlayerHandInfoAll(DbHandInfo HandInfoIn)
        {
            HandInfo = HandInfoIn;
        }
    }
}
