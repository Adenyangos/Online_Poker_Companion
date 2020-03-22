using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    class Players
    {
        public bool blnPlrChange, blnChipStackChangePrev, blnOpenSeatChangePrev, blnHoldCardChange, blnNewPlr;
        public bool blnOpenSeatCurr, blnOpenSeatPrev, blnSittingOutCurr, blnSittingOutPrev, blnSkipInfoCheck;
        /// <summary>
        /// This variable is needed for the special case where a player is the first to act in a hand and folds immediately before a screen shot was captured showing them having hold cards. Without this variable the fold under these conditions is never registered.
        /// </summary>
        public bool blnFirstFold;
        public int intDbPlayerId, intChipStackCurr, intChipStackPrev, intChipsInPot;
        public long lngDbHandPlayerId;
        public string strName, strNameTemp;
        public int[] intHoldCardsCurr, intHoldCardsPrev;

        public Players()
        {
            blnPlrChange = false;
            blnChipStackChangePrev = false;
            blnOpenSeatChangePrev = false;
            blnNewPlr = false;
            blnOpenSeatCurr = false;
            blnOpenSeatPrev = false;
            blnSittingOutCurr = true;
            blnSittingOutPrev = true;
            blnSkipInfoCheck = false;
            blnFirstFold = false;

            intDbPlayerId = 0;
            intChipStackCurr = -1;
            intChipStackPrev = -1;
            intChipsInPot = 0;

            lngDbHandPlayerId = -1;

            // strName is the player's name
            // strNameTemp is temporary stuff in the place of the name like "Fold", "Check", "Bet" etc. 
            // or possilby a new player's name that hasn't been verified yet
            strName = "\f";
            strNameTemp = "\f";

            intHoldCardsCurr = new int[2];
            intHoldCardsPrev = new int[2];

            for (int i = 0; i < 2; i++)
            {
                intHoldCardsCurr[i] = -1;
                intHoldCardsPrev[i] = -1;
            }
        }
    }
}
