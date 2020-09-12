using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DbDataStructures
{
    /// <summary>
    /// The information about a player at the start of a hand. (handPlayerId, chipCountStart, blind)
    /// </summary>
    class DbHandPlayerInfo
    {
        /// <summary>
        /// The handPlayerId (database key) for this player in this hand.
        /// </summary>
        public long HandPlayerId { get; set; }

        /// <summary>
        /// The number of chips this player had at the beginning of this hand before any antes or blinds.
        /// </summary>
        public int ChipCountStart { get; set; }

        /// <summary>
        /// The number of chips this player posted for a required blind bet in this hand.
        /// </summary>
        public int Blind { get; set; }
    }
}
