using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion.DbDataStructures
{
    /// <summary>
    /// The information about a single board action in a hand. This is when a board card is dealt ie. flop 1, flop 2, flop 3, turn or river. (handActionNumber)
    /// </summary>
    class DbBoardActionInfo
    {
        /// <summary>
        /// The handActionNumber corresponding to this board action in this hand.
        /// </summary>
        public int HandActionNumber { get; set; }
    }
}
