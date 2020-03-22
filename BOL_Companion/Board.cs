using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    class Board
    {
        public int intCardCurr, intCardPrev;
        public long lngPrevBoardActionId;

        public Board()
        {
            intCardCurr = -1;
            intCardPrev = -1;
            lngPrevBoardActionId = -1;
        }
    }
}
