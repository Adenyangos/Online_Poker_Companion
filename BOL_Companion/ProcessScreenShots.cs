using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using Patagames.Ocr;

namespace BOL_Companion
{
    class ProcessScreenShots
    {
        #region Static Constants

        /// <summary>
        /// The player's chips stack value cannot be read because the player's information is covered by a banner displaying other information
        /// </summary>
        public static int ChipStackValueCoverdByBanner { get; } = -2;

        /// <summary>
        /// No string was found for the value of the pot
        /// </summary>
        public static int NoPotParsingError { get; } = -2;

        /// <summary>
        /// An error occured while parsing the value of the player's chip stack
        /// </summary>
        public static int PlayerChipStackParseError = -3;

        #endregion

        #region Varibale Definitions

        /// <summary>
        /// Should the bitmap and graphics objects containing the screenshot be disposed 
        /// </summary>
        private bool disposeScreenShotResources;

        /// <summary>
        /// Is there an avatar in the player information display box
        /// </summary>
        private bool isAvatarPresent;

        /// <summary>
        /// Is the player sitting out
        /// </summary>
        private bool isPlayerSittingOut;

        /// <summary>
        /// Is there a banner present (blocking the player data) inside the player information box
        /// </summary>
        private bool isBannerPresent;

        /// <summary>
        /// The identifier for this instance of the ProcessScreenShots class:
        /// 0 - 9 = the players at the table (9 or 10-player table),
        /// 10 - 14 = the 5 possible board cards (flop, turn, river),
        /// 15 = the pot,
        /// 16 = action player and dealer determination
        /// </summary>
        private int id;

        /// <summary>
        /// The location where the bitmap files will be saved if the user choses to save the bitmap files that are used for processing the data on the screen
        /// </summary>
        private string bmpSaveLocation;

        /// <summary>
        /// The pixel location to check to determine if there is a banner present inside the player information box
        /// </summary>
        private Point bannerPresentPixel;

        /// <summary>
        /// The pixel locations to check to determine if a seat is open (no player in the seat)
        /// </summary>
        private Point[] openSeatCheckPixel;

        /// <summary>
        /// The pixel locations to check to determine the suit of the first hold card
        /// </summary>
        private Point[] hc1SuitCheckPixel;

        /// <summary>
        /// The pixel locations to check to determine the suit of the second hold card
        /// </summary>
        private Point[] hc2SuitCheckPixel;

        /// <summary>
        /// The pixel locations to check to determine the status (face down, face up, not present) of a player's first hold card 
        /// </summary>
        private Point[] firstHcStatusPixel;

        /// <summary>
        /// The pixel locations to check to determine the status (face down, face up, not present) of a player's second hold card 
        /// </summary>
        private Point[] secondHcStatusPixel;

        /// <summary>
        /// The pixel locations to check to determine if a player has an avatar in their information dispaly box
        /// </summary>
        private Point[] avatarPresentCheckPixel;

        /// <summary>
        /// The pixel locations to check to determine the suit of a board card
        /// </summary>
        private Point[] boardCardSuitCheckPixel;
        
        /// <summary>
        /// The screenshot bitmap this object is using to determine the state of the online poker game
        /// </summary>
        private Bitmap screenShotBitmap;

        /// <summary>
        /// Screenshot graphics object
        /// </summary>
        private Graphics screenShotGraphics;

        #endregion

        public ProcessScreenShots(int idIn, string bmpSaveLocationIn)
        {
            id = idIn;
            bmpSaveLocation = bmpSaveLocationIn;

            InitializeVariables();
        }

        /// <summary>
        /// Initialize the class level variables for this object.
        /// </summary>
        private void InitializeVariables()
        {
            disposeScreenShotResources = false;
            isAvatarPresent = false;
            isPlayerSittingOut = false;
            bannerPresentPixel = new Point(0, 0);
            openSeatCheckPixel = new Point[] { new Point(84, 30 + DataLocations.DyPlayerNameAndChipstack),
                new Point(85, 30 + DataLocations.DyPlayerNameAndChipstack)};
            hc1SuitCheckPixel = new Point[3];
            hc2SuitCheckPixel = new Point[3];
            firstHcStatusPixel = new Point[4];
            secondHcStatusPixel = new Point[4];
            avatarPresentCheckPixel = new Point[2];
            boardCardSuitCheckPixel = new Point[3];
        }

        #region Public Methods

        /// <summary>
        /// Check to see if this seat is open (no player in this seat).
        /// </summary>
        /// <returns>True if the seat is open</returns>
        public bool IsOpenSeat()
        {
            // Get the two pixels to check to determine if the seat is open
            Color[] pixelColor = new Color[] {
                screenShotBitmap.GetPixel(openSeatCheckPixel[0].X, openSeatCheckPixel[0].Y),
                screenShotBitmap.GetPixel(openSeatCheckPixel[1].X, openSeatCheckPixel[1].Y) };

            // The text "Open Seat" is shown on the line between where the player's name and chip count would be
            // so if there is text in that space in between it is an open seat.
            if ((pixelColor[0].R > 120 && pixelColor[0].R < 155) || (pixelColor[1].R > 120 && pixelColor[1].R < 155))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is the player in this seat sitting out?
        /// </summary>
        /// <returns>True if the player in this seat is sitting out</returns>
        public bool IsSittingOut()
        {
            return isPlayerSittingOut;
        }

        /// <summary>
        /// Get the name of the player in this seat.
        /// </summary>
        /// <param name="saveBitmap">True to save the bitmap used to determine the player's name</param>
        /// <returns>The player's name</returns>
        public string GetPlayerName(bool saveBitmap)
        {
            // The rectangle containing the player's name
            Rectangle croppedRectangle;

            // Define the size and location of the rectangle containing the player's name based on whether or not there is an aviatar in 
            // the player's information display area
            if (!isAvatarPresent)
            {
                croppedRectangle = new Rectangle(30, DataLocations.DyPlayerNameAndChipstack, screenShotBitmap.Width - 60, 26);
            }
            else
            {
                croppedRectangle = new Rectangle(70, DataLocations.DyPlayerNameAndChipstack, screenShotBitmap.Width - 70, 26);
            }
            
            // The bitmap image containing the player's name
            Bitmap playerNameBitmap = screenShotBitmap.Clone(croppedRectangle, screenShotBitmap.PixelFormat);

            CheckIfSittingOut(playerNameBitmap);

            ColorSimplifyPlayerName(playerNameBitmap);

            string playerName = BitmapToText(playerNameBitmap);

            if (saveBitmap)
            {
                SaveBitmapPlayer(playerNameBitmap);
            }

            if (!string.IsNullOrWhiteSpace(playerName))
            {
                // Remove trailing newline character if present
                playerName = playerName.Substring(0, playerName.IndexOf("\n"));
            }

            return playerName;
        }

        /// <summary>
        /// Get the chip stack (chip count) of the player in this seat.
        /// </summary>
        /// <param name="saveBitmap">True to save the bitmap used to determine the player's chip stack</param>
        /// <returns>The player's chip stack</returns>
        public int GetChipStack(bool saveBitmap)
        {
            int chipStack;

            if (!BannerPresentCheck())
            {
                Rectangle chipStackRectangle;

                // Get the section of the bitmap image that contains the chip stack text
                if (!AvatarPresentCheck())
                {
                    chipStackRectangle = new Rectangle(30, 31 + DataLocations.DyPlayerNameAndChipstack, screenShotBitmap.Width - 60, 26);
                }
                else
                {
                    chipStackRectangle = new Rectangle(70, 31 + DataLocations.DyPlayerNameAndChipstack, screenShotBitmap.Width - 70, 26);
                }
                
                // create a new bitmap with just the chip stack text
                Bitmap chipStackBitmap = screenShotBitmap.Clone(chipStackRectangle, screenShotBitmap.PixelFormat);

                ColorSimplifyChipStack(chipStackBitmap);

                string chipStackString = BitmapToText(chipStackBitmap);

                if (saveBitmap)
                {
                    SaveBitmapChips(chipStackBitmap);
                }

                if (!string.IsNullOrWhiteSpace(chipStackString))
                {
                    // Remove trailing newline character if present
                    chipStackString = chipStackString.Substring(0, chipStackString.IndexOf("\n"));
                }

                // Remove ',' and characters that might be misinterpreted as ',' from the chip stack string
                chipStackString = chipStackString.Replace(",", "");
                chipStackString = chipStackString.Replace(".", "");
                chipStackString = chipStackString.Replace(" ", "");

                if (!int.TryParse(chipStackString, out chipStack))
                {
                    int parseError = PlayerChipStackParseError;
                    chipStack = parseError;
                }
            }
            else
            {
                chipStack = ChipStackValueCoverdByBanner;
            }

            return chipStack;
        }

        /// <summary>
        /// Get the hold card status (cards face down / face up / no cards) for the player in this seat.
        /// </summary>
        /// <returns>The status of the player's two hold cards</returns>
        public HoldCardState.PairOfHoldCards HcState()
        {
            // The status of each of the two individual hold cards
            HoldCardState.SingleHoldCard hc1State, hc2State;

            // Prepare the pixels to be checked to determine the hold card status for the first hold card
            firstHcStatusPixel[0].X = DataLocations.HcStatusDx1;
            firstHcStatusPixel[0].Y = DataLocations.HcStatusDy1;
            firstHcStatusPixel[1].X = DataLocations.HcStatusDx1 + 1;
            firstHcStatusPixel[1].Y = DataLocations.HcStatusDy1;
            firstHcStatusPixel[2].X = DataLocations.HcStatusDx2;
            firstHcStatusPixel[2].Y = DataLocations.HcStatusDy2;
            firstHcStatusPixel[3].X = DataLocations.HcStatusDx2;
            firstHcStatusPixel[3].Y = DataLocations.HcStatusDy2 + 9;

            // Two pixels to check for the white of the facedown card edge
            Color[] pixelColorFaceDown = new Color[] {
                screenShotBitmap.GetPixel(firstHcStatusPixel[0].X, firstHcStatusPixel[0].Y),
                screenShotBitmap.GetPixel(firstHcStatusPixel[1].X, firstHcStatusPixel[1].Y) };

            // Two pixels to check for the white of the card suit symbol
            Color[] pixelColorSuit = new Color[] {
                screenShotBitmap.GetPixel(firstHcStatusPixel[2].X, firstHcStatusPixel[2].Y),
                screenShotBitmap.GetPixel(firstHcStatusPixel[3].X, firstHcStatusPixel[3].Y) };

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((pixelColorFaceDown[0].R > 220 && pixelColorFaceDown[0].G > 220 && pixelColorFaceDown[0].B > 220) || 
                (pixelColorFaceDown[1].R > 220 && pixelColorFaceDown[1].G > 220 && pixelColorFaceDown[1].B > 220))
            {
                hc1State = HoldCardState.SingleHoldCard.CardFaceDown;
            }
            // Check for the white of the card suit symbol (clubs, diamonds, hearts, spades) to check if the card is face up and being shown
            else if ((pixelColorSuit[0].R > 220 && pixelColorSuit[0].G > 220 && pixelColorSuit[0].B > 220) || 
                (pixelColorSuit[1].R > 220 && pixelColorSuit[1].G > 220 && pixelColorSuit[1].B > 220))
            {
                hc1State = HoldCardState.SingleHoldCard.CardShown;
            }
            else
            {
                hc1State = HoldCardState.SingleHoldCard.NoCard;
            }

            // Second hold card
            // Prepare the pixels to be checked to determine the hold card status for the second hold card
            secondHcStatusPixel[0].X = firstHcStatusPixel[0].X + DataLocations.Hc2Dx - DataLocations.Hc1Dx - 7;
            secondHcStatusPixel[0].Y = firstHcStatusPixel[0].Y;
            secondHcStatusPixel[1].X = firstHcStatusPixel[1].X + DataLocations.Hc2Dx - DataLocations.Hc1Dx - 7;
            secondHcStatusPixel[1].Y = firstHcStatusPixel[1].Y;
            secondHcStatusPixel[2].X = firstHcStatusPixel[2].X + DataLocations.Hc2Dx - DataLocations.Hc1Dx;
            secondHcStatusPixel[2].Y = firstHcStatusPixel[2].Y;
            secondHcStatusPixel[3].X = firstHcStatusPixel[3].X + DataLocations.Hc2Dx - DataLocations.Hc1Dx;
            secondHcStatusPixel[3].Y = firstHcStatusPixel[3].Y;

            // Two pixels to check for the white of the facedown card edge
            pixelColorFaceDown[0] = screenShotBitmap.GetPixel(secondHcStatusPixel[0].X, secondHcStatusPixel[0].Y);
            pixelColorFaceDown[1] = screenShotBitmap.GetPixel(secondHcStatusPixel[1].X, secondHcStatusPixel[1].Y);

            // Two pixels to check for the white of the card suit symbol
            pixelColorSuit[0] = screenShotBitmap.GetPixel(secondHcStatusPixel[2].X, secondHcStatusPixel[2].Y);
            pixelColorSuit[1] = screenShotBitmap.GetPixel(secondHcStatusPixel[3].X, secondHcStatusPixel[3].Y);

            // Check for the white edge of a facedown card to check for hold card present but not shown
            if ((pixelColorFaceDown[0].R > 220 && pixelColorFaceDown[0].G > 220 && pixelColorFaceDown[0].B > 220) || 
                (pixelColorFaceDown[1].R > 220 && pixelColorFaceDown[1].G > 220 && pixelColorFaceDown[1].B > 220))
            {
                hc2State = HoldCardState.SingleHoldCard.CardFaceDown;
            }
            // Check for the white of the card suit symbol (clubs, diamonds, hearts, spades) to check if the card is face up and being shown
            else if ((pixelColorSuit[0].R > 220 && pixelColorSuit[0].G > 220 && pixelColorSuit[0].B > 220) || 
                (pixelColorSuit[1].R > 220 && pixelColorSuit[1].G > 220 && pixelColorSuit[1].B > 220))
            {
                hc2State = HoldCardState.SingleHoldCard.CardShown;
            }
            else
            {
                hc2State = HoldCardState.SingleHoldCard.NoCard;
            }

            // The status of the pair of two hold cards
            HoldCardState.PairOfHoldCards hcPairState;

            if (hc1State == HoldCardState.SingleHoldCard.CardShown && hc2State == HoldCardState.SingleHoldCard.CardShown)
            {
                hcPairState = HoldCardState.PairOfHoldCards.BothCardsShown;
            }
            else if (hc1State == HoldCardState.SingleHoldCard.CardShown && hc2State == HoldCardState.SingleHoldCard.CardFaceDown)
            {
                hcPairState = HoldCardState.PairOfHoldCards.OnlyCard1Shown;
            }
            else if (hc1State == HoldCardState.SingleHoldCard.CardFaceDown && hc2State == HoldCardState.SingleHoldCard.CardShown)
            {
                hcPairState = HoldCardState.PairOfHoldCards.OnlyCard2Shown;
            }
            else if (hc1State == HoldCardState.SingleHoldCard.CardFaceDown && hc2State == HoldCardState.SingleHoldCard.CardFaceDown)
            {
                hcPairState = HoldCardState.PairOfHoldCards.BothCardsFaceDown;
            }
            else
            {
                hcPairState = HoldCardState.PairOfHoldCards.NoCards;
            }

            return hcPairState;
        }

        /// <summary>
        /// Find the value of a player's hold card.
        /// </summary>
        /// <param name="isHc1">True if this is the player's first hold card, false if it is the second</param>
        /// <param name="saveHcBmp">True to save the bitmap used to determine the player's hold card</param>
        /// <returns>The integer representation of the hold card</returns>
        public int FindHc(bool isHc1, bool saveHcBmp)
        {
            // Prepare the pixels to be checked to determine the suit of the first hold card
            hc1SuitCheckPixel[0].X = DataLocations.Hc1Dx + DataLocations.CardWidth - 3;
            hc1SuitCheckPixel[0].Y = DataLocations.HcDy;
            hc1SuitCheckPixel[1].X = DataLocations.Hc1Dx + DataLocations.CardWidth - 2;
            hc1SuitCheckPixel[1].Y = DataLocations.HcDy;
            hc1SuitCheckPixel[2].X = DataLocations.Hc1Dx + DataLocations.CardWidth - 1;
            hc1SuitCheckPixel[2].Y = DataLocations.HcDy;

            // Prepare the pixels to be checked to determine the suit of the second hold card
            hc2SuitCheckPixel[0].X = DataLocations.Hc2Dx + DataLocations.CardWidth - 3;
            hc2SuitCheckPixel[0].Y = hc1SuitCheckPixel[0].Y;
            hc2SuitCheckPixel[1].X = DataLocations.Hc2Dx + DataLocations.CardWidth - 2;
            hc2SuitCheckPixel[1].Y = hc1SuitCheckPixel[1].Y;
            hc2SuitCheckPixel[2].X = DataLocations.Hc2Dx + DataLocations.CardWidth - 1;
            hc2SuitCheckPixel[2].Y = hc1SuitCheckPixel[2].Y;

            // The color of the three pixels used to determine the suit of the hold card
            Color[] pixelColorSuit;

            // Hold card 1 is the card the user wishes to find the value of
            if (isHc1)
            {
                // Three pixels to check to determine the suit of the card
                pixelColorSuit = new Color[] {
                    screenShotBitmap.GetPixel(hc1SuitCheckPixel[0].X, hc1SuitCheckPixel[0].Y),
                    screenShotBitmap.GetPixel(hc1SuitCheckPixel[1].X, hc1SuitCheckPixel[1].Y),
                    screenShotBitmap.GetPixel(hc1SuitCheckPixel[2].X, hc1SuitCheckPixel[2].Y) };
            }
            // Hold card 2 is the card the user wishes to find the value of
            else
            {
                // Three pixels to check to determine the suit of the card
                pixelColorSuit = new Color[] {
                    screenShotBitmap.GetPixel(hc2SuitCheckPixel[0].X, hc2SuitCheckPixel[0].Y),
                    screenShotBitmap.GetPixel(hc2SuitCheckPixel[1].X, hc2SuitCheckPixel[1].Y),
                    screenShotBitmap.GetPixel(hc2SuitCheckPixel[2].X, hc2SuitCheckPixel[2].Y) };
            }

            // The suit of the hold card
            CardTranslator.CardSuit hcSuit = GetCardSuit(pixelColorSuit[0], pixelColorSuit[1], pixelColorSuit[2]);

            // The integer representation of the hold card
            int hcIntRepresentation = CardTranslator.NoCardPresent;

            // Check to make sure a card is present
            if (hcSuit != CardTranslator.CardSuit.unknown)
            {
                // The bitmap containing an image of the hold card
                Bitmap hcBitmap;

                // A description of the hold card (either Hc#1 or Hc#2)
                string hcDescription;

                if (isHc1)
                {
                    hcBitmap = screenShotBitmap.Clone(new Rectangle(DataLocations.Hc1Dx, DataLocations.HcDy, DataLocations.CardWidth, DataLocations.CardHeight), screenShotBitmap.PixelFormat);
                    hcDescription = "Hc#1";
                }
                else
                {
                    hcBitmap = screenShotBitmap.Clone(new Rectangle(DataLocations.Hc2Dx, DataLocations.HcDy, DataLocations.CardWidth, DataLocations.CardHeight), screenShotBitmap.PixelFormat);
                    hcDescription = "Hc#2";
                }

                string hcValue = GetCardValue(hcBitmap);

                // Get the integer representation of the hold card
                hcIntRepresentation = CardTranslator.CardSuitAndStringToInt(hcSuit, hcValue);

                if (saveHcBmp)
                {
                    SaveBitmapHc(hcBitmap, hcDescription);
                }
            }

            return hcIntRepresentation;
        }

        /// <summary>
        /// Find the value of a board card (flop, turn, river).
        /// </summary>
        /// <returns>The integer representation of the board card</returns>
        public int GetBoardCard()
        {
            // Prepare the pixels to be checked to determine the suit of the board card
            boardCardSuitCheckPixel[0].X = DataLocations.CardWidth - 3;
            boardCardSuitCheckPixel[0].Y = 0;
            boardCardSuitCheckPixel[1].X = DataLocations.CardWidth - 2;
            boardCardSuitCheckPixel[1].Y = 0;
            boardCardSuitCheckPixel[2].X = DataLocations.CardWidth - 1;
            boardCardSuitCheckPixel[2].Y = 0;

            // Three pixels to check to determine the suit of the board card
            Color[] pixelColorSuit = new Color[] {
                screenShotBitmap.GetPixel(boardCardSuitCheckPixel[0].X, boardCardSuitCheckPixel[0].Y),
                screenShotBitmap.GetPixel(boardCardSuitCheckPixel[1].X, boardCardSuitCheckPixel[1].Y),
                screenShotBitmap.GetPixel(boardCardSuitCheckPixel[2].X, boardCardSuitCheckPixel[2].Y) };

            // Get the suit of the board card
            CardTranslator.CardSuit boardCardSuit = GetCardSuit(pixelColorSuit[0], pixelColorSuit[1], pixelColorSuit[2]);

            // The integer representation of the board card
            int boardCardIntRepresentation = CardTranslator.NoCardPresent;

            // Check to make sure a card is present
            if (boardCardSuit != CardTranslator.CardSuit.unknown)
            {
                string boardCardValue = GetCardValue(screenShotBitmap);

                boardCardIntRepresentation = CardTranslator.CardSuitAndStringToInt(boardCardSuit, boardCardValue);
            }

            return boardCardIntRepresentation;
        }

        /// <summary>
        /// Get the value of the pot (how many chips are in the pot).
        /// </summary>
        /// <returns>How many chips are in the pot</returns>
        public int GetPot()
        {
            ColorSimplifyPot();

            string potString = BitmapToText(screenShotBitmap);

            // Remove "Pot: " and any commas from the string as well as any misread characters
            potString = potString.Replace("\n", "");
            potString = potString.Replace("Pot: ", "");
            potString = potString.Replace(",", "");
            potString = potString.Replace(".", "");
            potString = potString.Replace("D", "0");
            potString = potString.Replace("Z", "2");
            potString = potString.Replace("o", "0");
            potString = potString.Replace("O", "0");

            int pot;

            if (!int.TryParse(potString, out pot))
            {
                if (potString == "")
                {
                    pot = NoPotParsingError;
                }
                else
                {
                    int parseError = -1;
                    pot = parseError;
                }
            }

            return pot;
        }

        /// <summary>
        /// Find the player (seat number) of the current dealer.
        /// </summary>
        /// <returns>The seat number of the dealer</returns>
        public int FindDealer()
        {
            // Has the dealer been found
            bool dealerFound = false;

            // The seat number being checked (iterator)
            int seatNum = 0;

            // The seat number of the dealer
            int dealerSeat = -1;

            // Check each seat until the dealer is found
            while (!dealerFound && seatNum < 10)
            {
                // Set the location where the dealer chip would be found for this seat
                int pixelX = DataLocations.Locations[seatNum][3, 2] - DataLocations.Locations[16][0, 0];
                int pixelY = DataLocations.Locations[seatNum][3, 3] - DataLocations.Locations[16][0, 1];

                // Get the color of the pixel location where the dealer chip would be
                Color pixelColor = screenShotBitmap.GetPixel(pixelX, pixelY);

                // Is the dealer chip present
                if (pixelColor.R > 180)
                {
                    dealerSeat = seatNum;
                    dealerFound = true;
                }

                seatNum++;
            }

            return dealerSeat;
        }

        /// <summary>
        /// Check to see if the dealer has hold cards.
        /// </summary>
        /// <param name="dealerSeat">The seat the dealer is sitting in</param>
        /// <returns>True if the dealer has hold cards</returns>
        public bool DealerHasHoldCards(int dealerSeat)
        {
            // Pixels to check to determine if the dealer has hold cards (either face down or face up).
            // Check for the first hold card only (no need to check for a second hold card, if the first hold card is present the dealer has cards)
            Color[] pixelColor = new Color[] {
            screenShotBitmap.GetPixel(
                BitmapRect(dealerSeat, 0).Left + DataLocations.HcStatusDx1 - DataLocations.Locations[16][0, 0],
                BitmapRect(dealerSeat, 0).Top + DataLocations.HcStatusDy1 - DataLocations.Locations[16][0, 1]),
            screenShotBitmap.GetPixel(
                BitmapRect(dealerSeat, 0).Left + DataLocations.HcStatusDx1 + 1 - DataLocations.Locations[16][0, 0],
                BitmapRect(dealerSeat, 0).Top + DataLocations.HcStatusDy1 - DataLocations.Locations[16][0, 1]),
            screenShotBitmap.GetPixel(
                BitmapRect(dealerSeat, 0).Left + DataLocations.HcStatusDx1 - 1 - DataLocations.Locations[16][0, 0],
                BitmapRect(dealerSeat, 0).Top + DataLocations.HcStatusDy2 - DataLocations.Locations[16][0, 1]),
            screenShotBitmap.GetPixel(
                BitmapRect(dealerSeat, 0).Left + DataLocations.HcStatusDx1 - DataLocations.Locations[16][0, 0],
                BitmapRect(dealerSeat, 0).Top + DataLocations.HcStatusDy2 - DataLocations.Locations[16][0, 1]) };

            // Does the dealer have hold cards
            bool dealHasHoldCards = false;

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((pixelColor[0].R > 220 && pixelColor[0].G > 220 && pixelColor[0].B > 220) || 
                (pixelColor[1].R > 220 && pixelColor[1].G > 220 && pixelColor[1].B > 220))
            {
                dealHasHoldCards = true;
            }

            // Check for corner white area or brighness of a card that is present and shown
            else if (pixelColor[2].R > 100 || pixelColor[3].R > 100 || pixelColor[2].B > 100 || pixelColor[3].B > 100 || 
                (pixelColor[2].R > 3 && pixelColor[2].G > 100) || (pixelColor[3].R > 3 && pixelColor[3].G > 100))
            {
                dealHasHoldCards = true;
            }

            return dealHasHoldCards;
        }

        /// <summary>
        /// Find the player (seat number) of the action player (the player whose turn it is to act).
        /// </summary>
        /// <returns>The seat number of the action player</returns>
        public int FindActionPlayer()
        {
            // Has the action player been found
            bool actionPlayerFound = false;

            // The seat number being checked (iterator)
            int seatNum = 0;

            // The seat number of the action player
            int actionPlayerSeat = -1;

            // Check each seat until the action player is found
            while (!actionPlayerFound && seatNum < 10)
            {
                // Set the location where the player's action bar would be found for this seat
                int pixelX = DataLocations.Locations[seatNum][3, 0] - DataLocations.Locations[16][0, 0];
                int pixelY = DataLocations.Locations[seatNum][3, 1] - DataLocations.Locations[16][0, 1];

                // Get color of the pixel location where the player's action bar would be
                Color pixelColor = screenShotBitmap.GetPixel(pixelX, pixelY);

                // Is the player's action bar present
                if (pixelColor.R > 150)
                {
                    actionPlayerSeat = seatNum;

                    actionPlayerFound = true;
                }

                seatNum++;
            }

            return actionPlayerSeat;
        }

        /// <summary>
        /// Provide a new screenshot for this object to use.
        /// </summary>
        /// <param name="NewScreenShotBitmap">The new screenshot that this object will use</param>
        public void NewScreenShot(Bitmap NewScreenShotBitmap)
        {
            if (disposeScreenShotResources)
            {
                screenShotBitmap.Dispose();
                screenShotGraphics.Dispose();
            }

            screenShotBitmap = NewScreenShotBitmap;
            screenShotGraphics = Graphics.FromImage(screenShotBitmap);

            disposeScreenShotResources = true;
        }

        /// <summary>
        /// Get the sceenshot that this object is using.
        /// </summary>
        /// <returns>The screenshot this object is using</returns>
        public Bitmap ShareScreenShot()
        {
            return screenShotBitmap;
        }

        /// <summary>
        /// Change the location where bitmaps this object uses for processing will be saved (if the user chooses to save bitmap images).
        /// </summary>
        /// <param name="newSaveLocation">The new location where bitmaps this object uses for processing will be saved</param>
        public void ChangeBitmapSaveLocation(string newSaveLocation)
        {
            bmpSaveLocation = newSaveLocation;
        }

        /// <summary>
        /// Save the bitmap sceenshot that this object is using.
        /// </summary>
        public void SaveBitmap()
        {
            screenShotBitmap.Save(bmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + id.ToString("D2") + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        /// <summary>
        /// Save the player name bitmap file.
        /// </summary>
        /// <param name="bmp">The bitmap file to be saved</param>
        public void SaveBitmapPlayer(Bitmap bmp)
        {
            bmp.Save(bmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + id.ToString("D2") + " Player Name" + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        /// <summary>
        /// Save the player chip stack bitmap file.
        /// </summary>
        /// <param name="bmp">The bitmap file to be saved</param>
        public void SaveBitmapChips(Bitmap bmp)
        {
            bmp.Save(bmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + id.ToString("D2") + " Player Chips" + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        /// <summary>
        /// Save the hold card bitmap file.
        /// </summary>
        /// <param name="bmp">The bitmap file to be saved</param>
        /// <param name="hcDescription_">The description of the hold card (either Hc#1 or Hc#2)</param>
        private void SaveBitmapHc(Bitmap bmp, string hcDescription_)
        {
            bmp.Save(bmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + id.ToString("D2") + "_" + hcDescription_ + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }
        
        #region Rectangle request methods

        /// <summary>
        /// Get the rectangle associated with the given data identifier and index.
        /// </summary>
        /// <param name="id_">The data identifier (player [0 -9], board card [10 - 14], pot [15] or action player and dealer [16])</param>
        /// <param name="index_">The index of the specific data</param>
        /// <returns>The rectangle associated with the given data identifier and index</returns>
        public Rectangle BitmapRect(int id_, int index_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the combination of id_ and index_ are invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ and index_ are valid
            if (id_ > -1 && id_ < 17 && index_ > -1 && index_ < 4)
            {
                // Secondary verification that the id_ and index_ are valid
                if (index_ == 0 || id_ < 10)
                {
                    requestedRect = new Rectangle(
                        DataLocations.Locations[id_][index_, 0],
                        DataLocations.Locations[id_][index_, 1],
                        DataLocations.Locations[id_][index_, 2],
                        DataLocations.Locations[id_][index_, 3]);
                }
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine if a player's first hold card is face down.
        /// </summary>
        /// <param name="id_">The id of the player whose first hold card face down rectangle is being requested</param>
        /// <returns>The rectangle used to determine if the player's first hold card is face down</returns>
        public Rectangle Hc1FaceDownCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + firstHcStatusPixel[0].X, 
                    DataLocations.Locations[id_][0, 1] + firstHcStatusPixel[0].Y,
                    firstHcStatusPixel[1].X - firstHcStatusPixel[0].X, 
                    firstHcStatusPixel[1].Y - firstHcStatusPixel[0].Y);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the first rectangle used to determine if a player's first hold card is face up.
        /// </summary>
        /// <param name="id_">The id of the player whose first hold card face up rectangle is being requested</param>
        /// <returns>The first rectangle used to determine if the player's first hold card is face up</returns>
        public Rectangle Hc1FaceUpCheckRect_1(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + firstHcStatusPixel[2].X, 
                    DataLocations.Locations[id_][0, 1] + firstHcStatusPixel[2].Y, 
                    0, 0);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the second rectangle used to determine if a player's first hold card is face up.
        /// </summary>
        /// <param name="intId">The id of the player whose first hold card face up rectangle is being requested</param>
        /// <returns>The second rectangle used to determine if the player's first hold card is face up</returns>
        public Rectangle Hc1FaceUpCheckRect_2(int intId)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (intId > -1 && intId < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[intId][0, 0] + firstHcStatusPixel[3].X, 
                    DataLocations.Locations[intId][0, 1] + firstHcStatusPixel[3].Y, 
                    0, 0);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine if a player's second hold card is face down.
        /// </summary>
        /// <param name="id_">The id of the player whose second hold card face down rectangle is being requested</param>
        /// <returns>The rectangle used to determine if the player's second hold card is face down</returns>
        public Rectangle Hc2FaceDownCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + secondHcStatusPixel[0].X, 
                    DataLocations.Locations[id_][0, 1] + secondHcStatusPixel[0].Y,
                    secondHcStatusPixel[1].X - secondHcStatusPixel[0].X, 
                    secondHcStatusPixel[1].Y - secondHcStatusPixel[0].Y);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the first rectangle used to determine if a player's second hold card is face up.
        /// </summary>
        /// <param name="id_">The id of the player whose second hold card face up rectangle is being requested</param>
        /// <returns>The first rectangle used to determine if the player's second hold card is face up</returns>
        public Rectangle Hc2FaceUpCheckRect_1(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + secondHcStatusPixel[2].X, 
                    DataLocations.Locations[id_][0, 1] + secondHcStatusPixel[2].Y, 
                    0, 0);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the second rectangle used to determine if a player's second hold card is face up.
        /// </summary>
        /// <param name="id_">The id of the player whose second hold card face up rectangle is being requested</param>
        /// <returns>The second rectangle used to determine if the player's second hold card is face up</returns>
        public Rectangle Hc2FaceUpCheckRect_2(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + secondHcStatusPixel[3].X, 
                    DataLocations.Locations[id_][0, 1] + secondHcStatusPixel[3].Y, 
                    0, 0);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine if a player has an avatar present in their information display box.
        /// </summary>
        /// <param name="id_">The id of the player whose information box is to be checked for the presence of an avatar</param>
        /// <returns>The rectangle used to determine if the player has an avatar present in their information display box</returns>
        public Rectangle AvatarPresentCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);
            
            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + avatarPresentCheckPixel[0].X, 
                    DataLocations.Locations[id_][0, 1] + avatarPresentCheckPixel[0].Y,
                    avatarPresentCheckPixel[1].X - avatarPresentCheckPixel[0].X, 
                    avatarPresentCheckPixel[1].Y - avatarPresentCheckPixel[0].Y);
            }

            return requestedRect;
        }


        /// <summary>
        /// Get the rectangle used to determine if a seat is open (no player in that seat).
        /// </summary>
        /// <param name="id_">The id of the seat to check for the presence of a player</param>
        /// <returns>The rectangle used to determine if the seat is open</returns>
        public Rectangle OpenSeatCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + openSeatCheckPixel[0].X, 
                    DataLocations.Locations[id_][0, 1] + openSeatCheckPixel[0].Y,
                    openSeatCheckPixel[1].X - openSeatCheckPixel[0].X, 
                    openSeatCheckPixel[1].Y - openSeatCheckPixel[0].Y);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine if a banner is present (blocking the player data) inside the player information box.
        /// </summary>
        /// <param name="id_">The id of the player to check for the presence of a banner</param>
        /// <returns>The rectangle used to determine if a banner is present</returns>
        public Rectangle BannerPresentCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + bannerPresentPixel.X, 
                    DataLocations.Locations[id_][0, 1] + bannerPresentPixel.Y, 
                    0, 0);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine the suit of a board card.
        /// </summary>
        /// <param name="id_">The id of the board card to find the suit of</param>
        /// <returns>The rectangle used to determine if a banner is present</returns>
        public Rectangle BoardCardSuitCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > 9 && id_ < 15)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][0, 0] + boardCardSuitCheckPixel[0].X, 
                    DataLocations.Locations[id_][0, 1] + boardCardSuitCheckPixel[0].Y,
                    boardCardSuitCheckPixel[2].X - boardCardSuitCheckPixel[0].X, 
                    boardCardSuitCheckPixel[2].Y - boardCardSuitCheckPixel[0].Y);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine the suit of a player's first hold card.
        /// </summary>
        /// <param name="id_">The id of the player whose first hold card suit is to be determined</param>
        /// <returns>The rectangle used to determine the suit of a player's first hold card</returns>
        public Rectangle Hc1SuitCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][1, 0] + hc1SuitCheckPixel[0].X -DataLocations.Hc1Dx, 
                    DataLocations.Locations[id_][1, 1] + hc1SuitCheckPixel[0].Y - DataLocations.HcDy,
                    hc1SuitCheckPixel[2].X - hc1SuitCheckPixel[0].X, 
                    hc1SuitCheckPixel[2].Y - hc1SuitCheckPixel[0].Y);
            }

            return requestedRect;
        }

        /// <summary>
        /// Get the rectangle used to determine the suit of a player's second hold card.
        /// </summary>
        /// <param name="id_">The id of the player whose second hold card suit is to be determined</param>
        /// <returns>The rectangle used to determine the suit of a player's second hold card</returns>
        public Rectangle Hc2SuitCheckRect(int id_)
        {
            // The rectangle that is being requested (initialized with the value to be returned if the id_ is invalid)
            Rectangle requestedRect = new Rectangle(0, 0, 0, 0);

            // Verify the id_ is valid
            if (id_ > -1 && id_ < 10)
            {
                requestedRect = new Rectangle(
                    DataLocations.Locations[id_][2, 0] + hc2SuitCheckPixel[0].X - DataLocations.Hc2Dx, 
                    DataLocations.Locations[id_][2, 1] + hc2SuitCheckPixel[0].Y - DataLocations.HcDy,
                    hc2SuitCheckPixel[2].X - hc2SuitCheckPixel[0].X, 
                    hc2SuitCheckPixel[2].Y - hc2SuitCheckPixel[0].Y);
            }

            return requestedRect;
        }

        #endregion

        /// <summary>
        /// Draw all the rectangles used to determine which player is the dealer.
        /// </summary>
        public void DrawDealerRects()
        {
            // Loop through all the player seats (possible player locations)
            for (int i = 0; i < 10; i++)
            {
                // Prepare the pixel location of the dealer chip for player i
                int pixelX = DataLocations.Locations[i][3, 2] - DataLocations.Locations[16][0, 0];
                int pixelY = DataLocations.Locations[i][3, 3] - DataLocations.Locations[16][0, 1];

                Pen penRed = new Pen(Color.Red, 1);
                screenShotGraphics.DrawRectangle(penRed, pixelX - 1, pixelY - 1, 2, 2);
            }
        }

        /// <summary>
        /// Draw all the rectangles used to check for the presence of player's hold cards.
        /// </summary>
        /// <param name="intDealer"></param>
        public void DrawHoldCardCheckRects(int intDealer)
        {
            Pen penYellow = new Pen(Color.Yellow, 1);
            Pen penPurple = new Pen(Color.Purple, 1);

            // Loop through all the player seats (possible player locations)
            for (int i = 0; i < 9; i++)
            {
                // Prepare the pixel location to determine if player i's hold cards are face down
                int pixelX = BitmapRect(i, 0).Left + DataLocations.HcStatusDx1 - DataLocations.Locations[16][0, 0];
                int pixelY = BitmapRect(i, 0).Top + DataLocations.HcStatusDy1 - DataLocations.Locations[16][0, 1];
                screenShotGraphics.DrawRectangle(penYellow, pixelX - 1, pixelY - 1, 3, 2);

                // Prepare the pixel location to determine the suit of player i's hold cards are face up
                pixelX = BitmapRect(i, 0).Left + DataLocations.HcStatusDx2 - 1 - DataLocations.Locations[16][0, 0];
                pixelY = BitmapRect(i, 0).Top + DataLocations.HcStatusDy2 - DataLocations.Locations[16][0, 1];
                screenShotGraphics.DrawRectangle(penPurple, pixelX - 1, pixelY - 1, 3, 2);
            }
        }

        /// <summary>
        /// Draw all rectangles used to check for the presence of player's action bars.
        /// </summary>
        public void DrawActionBarRects()
        {
            // Loop through all the player seats (possible player locations)
            for (int i = 0; i < 10; i++)
            {
                int pixelX = DataLocations.Locations[i][3, 0] - DataLocations.Locations[16][0, 0];
                int pixelY = DataLocations.Locations[i][3, 1] - DataLocations.Locations[16][0, 1];

                Pen penBlack = new Pen(Color.Black, 1);
                screenShotGraphics.DrawRectangle(penBlack, pixelX - 1, pixelY - 1, 2, 2);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check for the presence of an avatar in this player's information display box
        /// </summary>
        /// <returns>True if there is an avatar present in this player's information display box</returns>
        private bool AvatarPresentCheck()
        {
            // Prepare the pixels to be checked to determine if there is an avatar in the player's information display box
            avatarPresentCheckPixel[0].X = 72;
            avatarPresentCheckPixel[0].Y = 27 + DataLocations.DyPlayerNameAndChipstack;
            avatarPresentCheckPixel[1].X = 73;
            avatarPresentCheckPixel[1].Y = 27 + DataLocations.DyPlayerNameAndChipstack;

            // Two pixels to check for a grey pixel framing an avatar
            Color[] pixelColor = { screenShotBitmap.GetPixel(avatarPresentCheckPixel[0].X, avatarPresentCheckPixel[0].Y),
                screenShotBitmap.GetPixel(avatarPresentCheckPixel[1].X, avatarPresentCheckPixel[1].Y) };

            // Check for a grey pixel framing an avatar in this location
            if (pixelColor[0].R > 75 || pixelColor[1].R > 75)
            {
                isAvatarPresent = true;
            }
            else
            {
                isAvatarPresent = false;
            }

            return isAvatarPresent;
        }

        /// <summary>
        /// Determine if the player in this seat is currently sitting out.
        /// </summary>
        /// <param name="playerNameBitmap_">The bitmap image containing the player's name</param>
        private void CheckIfSittingOut(Bitmap playerNameBitmap_)
        {
            isPlayerSittingOut = true;

            if (!isBannerPresent)
            {
                // Define an area to check for bright white text used to spell out the player's name. 
                // If bright white text exists the player is active (not sitting out). If not the player is sitting out.
                int xStart = playerNameBitmap_.Width / 3;
                int xEnd = xStart * 2;
                int y = playerNameBitmap_.Height / 2;

                // Loop through all the pixels in the x direction from xStart to xEnd checking for bright white pixels.
                for (int i = xStart; i < xEnd; i++)
                {
                    Color pixelColor = playerNameBitmap_.GetPixel(i, y);

                    if (pixelColor.R > 250 && pixelColor.R == pixelColor.G && pixelColor.R == pixelColor.B)
                    {
                        isPlayerSittingOut = false;
                        break;
                    }
                }
            }
            else
            {
                isPlayerSittingOut = false;
            }
        }

        /// <summary>
        /// Check if there is a banner present (blocking the player data) inside the player information box.
        /// </summary>
        /// <returns>True if there is a banner present</returns>
        private bool BannerPresentCheck()
        {
            // Set the x and y poistions of the pixel to be checked to determine if a banner is present
            bannerPresentPixel.X = screenShotBitmap.Width / 2;
            bannerPresentPixel.Y = screenShotBitmap.Height - 2;

            // The pixel color between where the player's name and the player's chips stack would be displayed
            Color pixelColor = screenShotBitmap.GetPixel(bannerPresentPixel.X, bannerPresentPixel.Y);

            // Check for a grey pixel in this location which is between where the player's name and the player's chips stack would be displayed
            if (pixelColor.R > 150 && pixelColor.G > 150 && pixelColor.B > 150)
            {
                isBannerPresent = true;
            }
            else
            {
                isBannerPresent = false;
            }

            return isBannerPresent;
        }

        /// <summary>
        /// Determine the suit of a card based on pixel colors.
        /// </summary>
        /// <param name="pixel1">The 1st pixel used to dertmine the card suit</param>
        /// <param name="pixel2">The 2nd pixel used to dertmine the card suit</param>
        /// <param name="pixel3">The 3rd pixel used to dertmine the card suit</param>
        /// <returns>The suit of the card</returns>
        private CardTranslator.CardSuit GetCardSuit(Color pixel1_, Color pixel2_, Color pixel3_)
        {
            CardTranslator.CardSuit cardSuit;

            // Is pixel2 grey
            if (pixel2_.R == pixel2_.G && pixel2_.G == pixel2_.B)
            {
                cardSuit = CardTranslator.CardSuit.spades;
            }
            // Is pixel2 red
            else if (pixel2_.R > pixel2_.G && pixel2_.R > pixel2_.B)
            {
                cardSuit = CardTranslator.CardSuit.hearts;
            }
            // Is pixel2 blue
            else if (pixel2_.B > pixel2_.R && pixel2_.B > pixel2_.G)
            {
                cardSuit = CardTranslator.CardSuit.diamonds;
            }
            // Is pixel2 green
            else if (pixel2_.G > pixel2_.R && pixel2_.G > pixel2_.B)
            {
                // Is pixel2 a bright green
                if (pixel1_.R > 20 || pixel2_.R > 20 || pixel3_.R > 20)
                {
                    cardSuit = CardTranslator.CardSuit.clubs;
                }
                else
                {
                    cardSuit = CardTranslator.CardSuit.unknown;
                }
            }
            else
            {
                cardSuit = CardTranslator.CardSuit.unknown;
            }
            
            return cardSuit;
        }

        /// <summary>
        /// Determine the value (number or Jack, Queen, King, Ace) of a card.
        /// </summary>
        /// <param name="screenShot">The screenshot containing the image of the card</param>
        /// <returns>The single character string representation of the card value</returns>
        private string GetCardValue(Bitmap screenShot)
        {
            ColorSimplifyCard(screenShot);

            string cardValue = BitmapToText(screenShot);

            if (!string.IsNullOrWhiteSpace(cardValue))
            {
                // Remove trailing newline character if present
                cardValue = cardValue.Substring(0, cardValue.IndexOf("\n"));
            }

            // Correct value if misinterpreted by the tesseract class
            if (cardValue == "E" || cardValue == "B" || cardValue == "b")
            {
                cardValue = "6";
            }
            else if (cardValue == "l]" || cardValue == "[l")
            {
                cardValue = "Q";
            }

            return cardValue;
        }

        /// <summary>
        /// Convert a bitmap image to text.
        /// </summary>
        /// <param name="bmp">The bitmap image to convert to text</param>
        /// <returns>The text contained in the bitmap image</returns>
        private string BitmapToText(Bitmap bmp)
        {
            // Convert bitmap image to text
            var tesseractObject = OcrApi.Create();
            tesseractObject.Init(Patagames.Ocr.Enums.Languages.English);
            // The value (number or Jack, Queen, King, Ace) of the board card
            string bitmapText = tesseractObject.GetTextFromImage(bmp);
            tesseractObject.Dispose();
            tesseractObject.Release();

            return bitmapText;
        }

        /// <summary>
        /// Determine if a board card is raised indicating it is being used to form the winning hand.
        /// </summary>
        /// <param name="cardBitmap">The bitmap image of the board card</param>
        /// <returns>True if the board card is raised</returns>
        private bool CardRaisedBoardCard(Bitmap cardBitmap)
        {
            // Note: In the most recent update of Bet Online they don't highlight the winning combination of cards by raising them like they 
            // used to. As a result this method is never called but I am leaving it here in case Bet Online reverts back to the old system.
            // 2020.04.04

            // Get the pixel color at the top center of this bitmap image. If the card is raised this pixel will be the color of a playing card
            Color pixelColor = cardBitmap.GetPixel(cardBitmap.Width / 2, 0);

            // Is this pixel bright enough to be a playing card
            if (pixelColor.R > 175 || pixelColor.G > 175 || pixelColor.B > 175)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determine if a hold card is raised indicating it is being used to form the winning hand.
        /// </summary>
        /// <param name="isHc1_">True if the first hold card is the card to be checked</param>
        /// <returns>True if the hold card is raised</returns>
        private bool CardRaisedHc(bool isHc1_)
        {
            // Note: In the most recent update of Bet Online they don't highlight the winning combination of cards by raising them like they 
            // used to. As a result this method is never called but I am leaving it here in case Bet Online reverts back to the old system.
            // 2020.04.04

            // The pixel color at the top of the hold card bitmap image. If the card is raised this pixel will be the color of a playing card
            Color pixelColor;

            if (isHc1_)
            {
                // If the first hold card is the card to be checked
                pixelColor = screenShotBitmap.GetPixel(DataLocations.Hc1Dx + 5, 0);
            }
            else
            {
                // If the second hold card is the card to be checked
                pixelColor = screenShotBitmap.GetPixel(DataLocations.Hc2Dx + 55, 0);
            }

            // Is this pixel bright enough to be a playing card
            if (pixelColor.R > 175 || pixelColor.G > 175 || pixelColor.B > 175)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method modifies the bitmap image of the pot so that each pixel is either black or white.
        /// </summary>
        private void ColorSimplifyPot()
        {
            // The color of a pixel being examined
            Color pixelColor;

            // Loop through all the pixels in the bitmap image
            for (int i = 0; i < screenShotBitmap.Width; i++)
            {
                for (int j = 0; j < screenShotBitmap.Height; j++)
                {
                    pixelColor = screenShotBitmap.GetPixel(i, j);

                    // The text is white with a small black shadow on a green background so look for pixels where the value of 
                    // green is equal to the value of red
                    if (pixelColor.G == pixelColor.R)
                    {
                        screenShotBitmap.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        screenShotBitmap.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// This method takes a bitmap image of a card and modifies it so that each pixel is either black or white.
        /// </summary>
        /// <param name="cardBitmap">The bitmap image whose pixels are to be converted to either black or white</param>
        private void ColorSimplifyCard(Bitmap cardBitmap)
        {
            // The color of a pixel being examined
            Color pixelColor;

            // The threashold color value for turning a pixel black or white
            int colorThreashold = 155;

            // Test for card brightness (basically if the card is in the fadded/darkened state) before setting the color threashold
            for (int i = 0; i < cardBitmap.Width; i++)
            {
                // The color of a pixel being examined
                pixelColor = cardBitmap.GetPixel(i, cardBitmap.Height / 2);

                // Check for a bright white pixel
                if (pixelColor.R > 252 && pixelColor.G > 252 && pixelColor.B > 252)
                {
                    colorThreashold = 252;
                    i = cardBitmap.Width;
                }
            }

            // Loop through all the pixels in the bitmap image
            for (int i = 0; i < cardBitmap.Width; i++)
            {
                for (int j = 0; j < cardBitmap.Height; j++)
                {
                    pixelColor = cardBitmap.GetPixel(i, j);

                    // if the value of red, green and blue are all equal and greater than the color threashold this is the text to read
                    // else this is background
                    if (pixelColor.R == pixelColor.G && pixelColor.G == pixelColor.B && pixelColor.R > colorThreashold)
                    {
                        cardBitmap.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        cardBitmap.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// This method takes the bitmap image containing the player's name and modifies it so that each pixel is either black or white.
        /// </summary>
        /// <param name="playerNameBitmap_">The bitmap image whose pixels are to be converted to either black or white</param>
        private void ColorSimplifyPlayerName(Bitmap playerNameBitmap_)
        {
            // Loop through all the pixels in the bitmap image
            for (int i = 0; i < playerNameBitmap_.Width; i++)
            {
                for (int j = 0; j < playerNameBitmap_.Height; j++)
                {
                    int colorThreashold1 = 225;
                    int colorThreashold2 = 220;

                    Color pixelColor = playerNameBitmap_.GetPixel(i, j);

                    // if pixelColor.R >= colorThreashold1 this as a white character
                    // else if pixelColor.B > colorThreashold2 && pixelColor.B > 2 * pixelColor.R this as a blue character ("Fold", "Bet", "Call",  
                    // "Check", "All-In" etc.)
                    if (pixelColor.R >= colorThreashold1 || (pixelColor.B > colorThreashold2 && pixelColor.B > 2 * pixelColor.R))
                    {
                        playerNameBitmap_.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        playerNameBitmap_.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// This method takes the bitmap image containing the player's chip stack and modifies it so that each pixel is either black or white.
        /// </summary>
        /// <param name="chipStackBitmap_">The bitmap image whose pixels are to be converted to either black or white</param>
        private void ColorSimplifyChipStack(Bitmap chipStackBitmap_)
        {
            // Loop through all the pixels in the bitmap image
            for (int i = 0; i < chipStackBitmap_.Width; i++)
            {
                for (int j = 0; j < chipStackBitmap_.Height; j++)
                {
                    int colorThreashold = 150;

                    Color pixelColor = chipStackBitmap_.GetPixel(i, j);

                    // Chip stacks are displayed in green text so look for green text
                    if (pixelColor.G > (pixelColor.R + 2) && pixelColor.G > (pixelColor.B + 2) && pixelColor.G > colorThreashold)
                    {
                        chipStackBitmap_.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        chipStackBitmap_.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
