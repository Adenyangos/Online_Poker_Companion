using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DbDataStructures
{
    /// <summary>
    /// The general information about a single hand a player was involved in. (handId, gameId, ante, handPlayerId) 
    /// </summary>
    class DbHandInfo
    {
        /// <summary>
        /// The hand Id (database key) of this hand.
        /// </summary>
        public long HandId { get; set; }
        
        /// <summary>
        /// The game Id (database key) of the game this hand took place in.
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// The ante for this hand.
        /// </summary>
        public int Ante { get; set; }

        /// <summary>
        /// The handPlayerId (database key) for this player (the player of interest) in this hand.
        /// </summary>
        public long HandPlayerId { get; set; }
    }
}
