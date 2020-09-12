using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A class that represents the possible states of a hold card or pair of hold cards.
    /// </summary>
    public static class HoldCardState
    {
        /// <summary>
        /// The possible states of an single hold card.
        /// </summary>
        public enum SingleHoldCard
        {
            /// <summary>
            /// No hold card present
            /// </summary>
            NoCard,

            /// <summary>
            /// Hold card is face down
            /// </summary>
            CardFaceDown,

            /// <summary>
            /// Hold card is shown
            /// </summary>
            CardShown
        }

        /// <summary>
        /// The possible states of a pair of hold cards.
        /// </summary>
        public enum PairOfHoldCards
        {
            /// <summary>
            /// No hold cards present. The player has either already folded or is not playing)
            /// </summary>
            NoCards,

            /// <summary>
            /// Both hold cards are face down
            /// </summary>
            BothCardsFaceDown,

            /// <summary>
            /// Both hold cards are shown (face up)
            /// </summary>
            BothCardsShown,

            /// <summary>
            /// Only the first hold card is being shown. The second hold card is face down
            /// </summary>
            OnlyCard1Shown,

            /// <summary>
            /// Only the second hold card is being shown. The first hold card is face down
            /// </summary>
            OnlyCard2Shown
        }
    }
}
