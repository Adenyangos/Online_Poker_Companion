using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DbDataStructures
{
    /// <summary>
    /// The information about a single player action in a hand. (handPlayerId, chipCountChange, handActionNumber)
    /// </summary>
    class DbPlayerActionInfo
    {
        /// <summary>
        /// The handPlayerId (database key) for this player in this hand.
        /// </summary>
        public long HandPlayerId { get; set; }

        /// <summary>
        /// The change in the player's chip count as a result of this action (positive numbers are bets negative numbers are chips won).
        /// </summary>
        public int ChipCountChange { get; set; }

        /// <summary>
        /// The handActionNumber (database key) for this action in this hand.
        /// </summary>
        public int HandActionNumber { get; set; }
    }
}
