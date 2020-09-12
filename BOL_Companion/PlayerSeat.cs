using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// This class represents one seat at the poker table.
    /// </summary>
    class PlayerSeat
    {
        /// <summary>
        /// Has the player sitting in this seat changed (including a change from a player being in the seat to an empty seat)
        /// </summary>
        public bool PlayerChange { get; set; } = false;

        /// <summary>
        /// Is there a new player at the table
        /// </summary>
        public bool NewPlayer { get; set; } = false;

        /// <summary>
        /// Is this seat at the poker table is open (no player in the seat) based on the most recently processed screenshot
        /// </summary>
        public bool OpenSeatCurr { get; set; } = false;

        /// <summary>
        /// Is this seat at the poker table is open (no player in the seat) based on the previously processed screenshot
        /// </summary>
        public bool OpenSeatPrev { get; set; } = false;

        /// <summary>
        /// Did this player's seat go from being filled to open or open to filled on the previously processed screenshot
        /// </summary>
        public bool OpenSeatChangePrev { get; set; } = false;

        /// <summary>
        /// Is the player in this seat sitting out based on the most recently processed screenshot
        /// </summary>
        public bool SittingOutCurr { get; set; } = true;

        /// <summary>
        /// Is the player in this seat sitting out based on the previously processed screenshot
        /// </summary>
        public bool SittingOutPrev { get; set; } = true;

        /// <summary>
        /// Did the player's chipstack change on the previously processed screenshot
        /// </summary>
        public bool ChipStackChangePrev { get; set; } = false;

        /// <summary>
        /// Did this player's hold cards change (including changing from face down to face up or no cards to face down cards)
        /// </summary>
        public bool HoldCardChange { get; set; } = true;

        /// <summary>
        /// Do not process screenshot data for this player
        /// </summary>
        public bool SkipInfoCheck { get; set; } = false;

        /// <summary>
        /// Did this player fold immediately at the beginning of the hand
        /// <para>This variable tracks the special case where a player is the first to act in a hand and folds immediately before a screen shot is captured showing them having hold cards. Without this variable a fold under these conditions is never registered.</para>
        /// </summary>
        public bool FirstActionFastFold { get; set; } = false;

        /// <summary>
        /// The player's chipstack based on the most recently processed screenshot
        /// </summary>
        public int ChipStackCurr { get; set; } = -1;

        /// <summary>
        /// The player's chipstack based on the previously processed screenshot
        /// </summary>
        public int ChipStackPrev { get; set; } = -1;

        /// <summary>
        /// The number of chips the player currrently has in the pot
        /// </summary>
        public int ChipsInPot { get; set; } = 0;

        /// <summary>
        /// The player's database player Id
        /// </summary>
        public int DbPlayerId { get; set; } = 0;

        /// <summary>
        /// The player's handPlayer Id for the current hand
        /// </summary>
        public long DbHandPlayerId { get; set; } = -1;

        /// <summary>
        /// The player's name
        /// </summary>
        public string Name { get; set; } = "\f";

        /// <summary>
        /// Something temporary being shown in place of the player's name such as "Fold", "Check", "Bet" etc. or a player's name that has not been verified yet
        /// </summary>
        public string NameTemp { get; set; } = "\f";

        /// <summary>
        /// The player's two hold cards based on the most recently processed screenshot
        /// </summary>
        public int[] HoldCardsCurr { get; set; }

        /// <summary>
        /// The player's two hold cards based on the previously processed screenshot
        /// </summary>
        public int[] HoldCardsPrev { get; set; }

        public PlayerSeat()
        {
            // Initialize the hold card arrays
            HoldCardsCurr = new int[] { -1, -1 };
            HoldCardsPrev = new int[] { -1, -1 };
        }
    }
}
