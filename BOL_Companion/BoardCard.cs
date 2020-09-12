using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A class that represents a board card position.
    /// </summary>
    class BoardCard
    {
        #region Properties for accessing class level variables

        /// <summary>
        /// The integer representation of the board card from the current screen shot.
        /// </summary>
        public int CardCurrInt { get; set; } = CardTranslator.NoCardPresent;

        /// <summary>
        /// The integer representation of the board card from the previous screen shot.
        /// </summary>
        public int CardPrevInt { get; set; } = CardTranslator.NoCardPresent;

        /// <summary>
        /// The previous BoardActionId (database key).
        /// </summary>
        public long PrevBoardActionId { get; set; } = -1;

        #endregion

        /// <summary>
        /// Initialize board card with the "no card present condition"
        /// </summary>
        public BoardCard()
        {
            
        }
    }
}
