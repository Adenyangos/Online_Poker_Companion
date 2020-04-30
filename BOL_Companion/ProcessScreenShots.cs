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
        #region Varibale Definitions
        private bool blnDisposeScreenShotResources, blnAvitarPresent, blnPlayerSittingOut, blnBannerPresent;
        private const int intDyPlayerInfo = 106;
        private const int intHcStatusDx1 = 59;
        private const int intHcStatusDx2 = 73;
        private const int intHcStatusDy1 = 89;
        private const int intHcStatusDy2 = 65;
        private const int intHc1Dx = 57;
        private const int intHc2Dx = 99;
        private const int intHcDy = 6;
        private const int intCardDx = 32;
        private const int intCardDy = 50;
        private int intIdentifier;
        private int[][,] intLocations;
        private string strBmpSaveLocation;
        private Point pntOpenSeatCheckPixel_1, pntOpenSeatCheckPixel_2, pntAvitarPresentCheckPixel_1, pntAvitarPresentCheckPixel_2;
        private Point pntFirstHoldCardPresent_1, pntFirstHoldCardPresent_2, pntFirstHoldCardPresent_3, pntFirstHoldCardPresent_4;
        private Point pntSecondHoldCardPresent_1, pntSecondHoldCardPresent_2, pntSecondHoldCardPresent_3, pntSecondHoldCardPresent_4;
        private Point pntBannerPresentCheck, pntBoardCardSuitCheck_1, pntBoardCardSuitCheck_2, pntBoardCardSuitCheck_3;
        private Point pntHc1SuitCheck_1, pntHc1SuitCheck_2, pntHc1SuitCheck_3, pntHc2SuitCheck_1, pntHc2SuitCheck_2, pntHc2SuitCheck_3;
        private Bitmap bmpScreenShot;
        private Graphics gfxScreenShot;
        private CardStringIntTranslator cdt;

        #endregion

        public ProcessScreenShots(int intId, string strBmpSaveLoc)
        {
            intIdentifier = intId;
            strBmpSaveLocation = strBmpSaveLoc;

            InitializeVariables();
            InitializeArrays();
        }

        private void InitializeVariables()
        {
            pntOpenSeatCheckPixel_1 = new Point(0, 0);
            pntOpenSeatCheckPixel_2 = new Point(0, 0);
            pntAvitarPresentCheckPixel_1 = new Point(0, 0);
            pntAvitarPresentCheckPixel_2 = new Point(0, 0);
            pntFirstHoldCardPresent_1 = new Point(0, 0);
            pntFirstHoldCardPresent_2 = new Point(0, 0);
            pntFirstHoldCardPresent_3 = new Point(0, 0);
            pntFirstHoldCardPresent_4 = new Point(0, 0);
            pntSecondHoldCardPresent_1 = new Point(0, 0);
            pntSecondHoldCardPresent_2 = new Point(0, 0);
            pntSecondHoldCardPresent_3 = new Point(0, 0);
            pntSecondHoldCardPresent_4 = new Point(0, 0);
            pntBannerPresentCheck = new Point(0, 0);
            pntBoardCardSuitCheck_1 = new Point(0, 0);
            pntBoardCardSuitCheck_2 = new Point(0, 0);
            pntBoardCardSuitCheck_3 = new Point(0, 0);
            pntHc1SuitCheck_1 = new Point(0, 0);
            pntHc1SuitCheck_2 = new Point(0, 0);
            pntHc1SuitCheck_3 = new Point(0, 0);
            pntHc2SuitCheck_1 = new Point(0, 0);
            pntHc2SuitCheck_2 = new Point(0, 0);
            pntHc2SuitCheck_3 = new Point(0, 0);
            blnAvitarPresent = false;
            blnPlayerSittingOut = false;
            blnDisposeScreenShotResources = false;
            cdt = new CardStringIntTranslator();
        }

        #region Public Methods

        public bool OpenSeat()
        {
            bool blnOpenSeat;
            Color clrPixel1, clrPixel2;

            pntOpenSeatCheckPixel_1.X = 84;
            pntOpenSeatCheckPixel_1.Y = 30 + intDyPlayerInfo;
            pntOpenSeatCheckPixel_2.X = 85;
            pntOpenSeatCheckPixel_2.Y = 30 + intDyPlayerInfo;

            clrPixel1 = bmpScreenShot.GetPixel(pntOpenSeatCheckPixel_1.X, pntOpenSeatCheckPixel_1.Y);
            clrPixel2 = bmpScreenShot.GetPixel(pntOpenSeatCheckPixel_2.X, pntOpenSeatCheckPixel_2.Y);

            // The text "Open Seat" is shown on the line between where the player's name and chip count would be
            // so if there is text in that space in between it is an open seat.
            if ((clrPixel1.R > 120 && clrPixel1.R < 155) || (clrPixel2.R > 120 && clrPixel2.R < 155))
            {
                blnOpenSeat = true;
            }
            else
            {
                blnOpenSeat = false;
            }

            return blnOpenSeat;
        }

        public bool SittingOut()
        {
            return blnPlayerSittingOut;
        }

        public string GetPlayerName(bool blnSaveBitmap)
        {
            string strName;
            Rectangle rctCropped;
            Bitmap bmpPlayerName;
            var varOcr = OcrApi.Create();

            if (!blnAvitarPresent)
            {
                rctCropped = new Rectangle(30, intDyPlayerInfo, bmpScreenShot.Width - 60, 26);
            }
            else
            {
                rctCropped = new Rectangle(70, intDyPlayerInfo, bmpScreenShot.Width - 70, 26);
            }
 
            bmpPlayerName = bmpScreenShot.Clone(rctCropped, bmpScreenShot.PixelFormat);

            CheckIfSittingOut(bmpPlayerName);

            ColorSimplifyPlayerName(bmpPlayerName);
            varOcr.Init(Patagames.Ocr.Enums.Languages.English);
            strName = varOcr.GetTextFromImage(bmpPlayerName);
            varOcr.Release();

            if (blnSaveBitmap)
            {
                SaveBitmapPlayer(bmpPlayerName);
            }

            if (!string.IsNullOrWhiteSpace(strName))
            {
                strName = strName.Substring(0, strName.IndexOf("\n"));
            }

            return strName;
        }

        public int GetChipStack(bool blnSaveBitmap)
        {
            int intChipStack;

            if (!BannerPresent())
            {
                string strChipStack;
                Rectangle rctCropped;
                Bitmap bmpChipStack;
                var varOcr = OcrApi.Create();

                if (!AvitarPresent())
                {
                    rctCropped = new Rectangle(30, 31 + intDyPlayerInfo, bmpScreenShot.Width - 60, 26);
                }
                else
                {
                    rctCropped = new Rectangle(70, 31 + intDyPlayerInfo, bmpScreenShot.Width - 70, 26);
                }

                bmpChipStack = bmpScreenShot.Clone(rctCropped, bmpScreenShot.PixelFormat);

                ColorSimplifyChipStack(bmpChipStack);
                varOcr.Init(Patagames.Ocr.Enums.Languages.English);
                strChipStack = varOcr.GetTextFromImage(bmpChipStack);
                varOcr.Release();

                if (blnSaveBitmap)
                {
                    SaveBitmapChips(bmpChipStack);
                }

                if (!string.IsNullOrWhiteSpace(strChipStack))
                {
                    strChipStack = strChipStack.Substring(0, strChipStack.IndexOf("\n"));
                }

                // Remove ',' and characters that ',' might be misinterpreted as from the chip stack string
                strChipStack = strChipStack.Replace(",", "");
                strChipStack = strChipStack.Replace(".", "");
                strChipStack = strChipStack.Replace(" ", "");

                if (!int.TryParse(strChipStack, out intChipStack))
                {
                    intChipStack = -3;
                }
            }
            else
            {
                intChipStack = -2;
            }

            return intChipStack;
        }

        public int HcStatus()
        {
            int intHcStatus, intHc1Status, intHc2Status;
            Color clrPixel1, clrPixel2, clrPixel3, clrPixel4;

            // intHcStatus (hold card status) == 
            // -1 no hold card (no cards folded or not playing)
            // 0 both hold cards face down (the hand has not been shown)
            // 1 both hold cards shown
            // 2 only first hold card shown
            // 3 only second hold card shown

            // if intHc1Status(hold card #1 status) or intHc2Status(hold card #2 status) == 
            // -1 no hold card
            // 0 hold card face down
            // 1 hold card shown

            // Firts hold card
            pntFirstHoldCardPresent_1.X = intHcStatusDx1;
            pntFirstHoldCardPresent_1.Y = intHcStatusDy1;
            pntFirstHoldCardPresent_2.X = intHcStatusDx1 + 1;
            pntFirstHoldCardPresent_2.Y = intHcStatusDy1;
            pntFirstHoldCardPresent_3.X = intHcStatusDx2;
            pntFirstHoldCardPresent_3.Y = intHcStatusDy2;
            pntFirstHoldCardPresent_4.X = intHcStatusDx2;
            pntFirstHoldCardPresent_4.Y = intHcStatusDy2 + 9;

            clrPixel1 = bmpScreenShot.GetPixel(pntFirstHoldCardPresent_1.X, pntFirstHoldCardPresent_1.Y);
            clrPixel2 = bmpScreenShot.GetPixel(pntFirstHoldCardPresent_2.X, pntFirstHoldCardPresent_2.Y);
            clrPixel3 = bmpScreenShot.GetPixel(pntFirstHoldCardPresent_3.X, pntFirstHoldCardPresent_3.Y);
            clrPixel4 = bmpScreenShot.GetPixel(pntFirstHoldCardPresent_4.X, pntFirstHoldCardPresent_4.Y);

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((clrPixel1.R > 220 && clrPixel1.G > 220 && clrPixel1.B > 220) || (clrPixel2.R > 220 && clrPixel2.G > 220 && clrPixel2.B > 220))
            {
                intHc1Status = 0;
            }
            // Check for the white of the card suit symbol (clubs, diamonds, hearts, spades) to check if the card is face up and being shown
            else if ((clrPixel3.R > 220 && clrPixel3.G > 220 && clrPixel3.B > 220) || (clrPixel4.R > 220 && clrPixel4.G > 220 && clrPixel4.B > 220))
            {
                intHc1Status = 1;
            }
            else
            {
                intHc1Status = -1;
            }

            // Second hold card
            pntSecondHoldCardPresent_1.X = pntFirstHoldCardPresent_1.X + intHc2Dx - intHc1Dx - 7;
            pntSecondHoldCardPresent_1.Y = pntFirstHoldCardPresent_1.Y;
            pntSecondHoldCardPresent_2.X = pntFirstHoldCardPresent_2.X + intHc2Dx - intHc1Dx - 7;
            pntSecondHoldCardPresent_2.Y = pntFirstHoldCardPresent_2.Y;
            pntSecondHoldCardPresent_3.X = pntFirstHoldCardPresent_3.X + intHc2Dx - intHc1Dx;
            pntSecondHoldCardPresent_3.Y = pntFirstHoldCardPresent_3.Y;
            pntSecondHoldCardPresent_4.X = pntFirstHoldCardPresent_4.X + intHc2Dx - intHc1Dx;
            pntSecondHoldCardPresent_4.Y = pntFirstHoldCardPresent_4.Y;

            clrPixel1 = bmpScreenShot.GetPixel(pntSecondHoldCardPresent_1.X, pntSecondHoldCardPresent_1.Y);
            clrPixel2 = bmpScreenShot.GetPixel(pntSecondHoldCardPresent_2.X, pntSecondHoldCardPresent_2.Y);
            clrPixel3 = bmpScreenShot.GetPixel(pntSecondHoldCardPresent_3.X, pntSecondHoldCardPresent_3.Y);
            clrPixel4 = bmpScreenShot.GetPixel(pntSecondHoldCardPresent_4.X, pntSecondHoldCardPresent_4.Y);

            // Check for the white edge of a facedown card to check for hold card present but not shown
            if ((clrPixel1.R > 220 && clrPixel1.G > 220 && clrPixel1.B > 220) || (clrPixel2.R > 220 && clrPixel2.G > 220 && clrPixel2.B > 220))
            {
                intHc2Status = 0;
            }
            // Check for the white of the card suit symbol (clubs, diamonds, hearts, spades) to check if the card is face up and being shown
            else if ((clrPixel3.R > 220 && clrPixel3.G > 220 && clrPixel3.B > 220) || (clrPixel4.R > 220 && clrPixel4.G > 220 && clrPixel4.B > 220))
            {
                intHc2Status = 1;
            }
            else
            {
                intHc2Status = -1;
            }

            if (intHc1Status == 1 && intHc2Status == 1)
            {
                intHcStatus = 1;
            }
            else if (intHc1Status == 1 && intHc2Status == 0)
            {
                intHcStatus = 2;
            }
            else if (intHc1Status == 0 && intHc2Status == 1)
            {
                intHcStatus = 3;
            }
            else if (intHc1Status == 0 && intHc2Status == 0)
            {
                intHcStatus = 0;
            }
            else
            {
                intHcStatus = -1;
            }

            return intHcStatus;
        }

        public int FindHc(bool blnHc1, bool blnSaveHcBmp)
        {
            // Following GetBoardCard Pattern/Logic
            int intHc, intHcSuit;
            Color clrPix1, clrPix2, clrPix3;

            intHc = -1;

            pntHc1SuitCheck_1.X = intHc1Dx + intCardDx - 3;
            pntHc1SuitCheck_1.Y = intHcDy;
            pntHc1SuitCheck_2.X = intHc1Dx + intCardDx - 2;
            pntHc1SuitCheck_2.Y = intHcDy;
            pntHc1SuitCheck_3.X = intHc1Dx + intCardDx - 1;
            pntHc1SuitCheck_3.Y = intHcDy;

            pntHc2SuitCheck_1.X = intHc2Dx + intCardDx - 3;
            pntHc2SuitCheck_1.Y = pntHc1SuitCheck_1.Y;
            pntHc2SuitCheck_2.X = intHc2Dx + intCardDx - 2;
            pntHc2SuitCheck_2.Y = pntHc1SuitCheck_2.Y;
            pntHc2SuitCheck_3.X = intHc2Dx + intCardDx - 1;
            pntHc2SuitCheck_3.Y = pntHc1SuitCheck_3.Y;

            if (blnHc1)
            {
                clrPix1 = bmpScreenShot.GetPixel(pntHc1SuitCheck_1.X, pntHc1SuitCheck_1.Y);
                clrPix2 = bmpScreenShot.GetPixel(pntHc1SuitCheck_2.X, pntHc1SuitCheck_2.Y);
                clrPix3 = bmpScreenShot.GetPixel(pntHc1SuitCheck_3.X, pntHc1SuitCheck_3.Y);
            }
            else
            {
                clrPix1 = bmpScreenShot.GetPixel(pntHc2SuitCheck_1.X, pntHc2SuitCheck_1.Y);
                clrPix2 = bmpScreenShot.GetPixel(pntHc2SuitCheck_2.X, pntHc2SuitCheck_2.Y);
                clrPix3 = bmpScreenShot.GetPixel(pntHc2SuitCheck_3.X, pntHc2SuitCheck_3.Y);
            }

            intHcSuit = GetCardSuit(clrPix1, clrPix2, clrPix3);

            // Check to make sure a card is present
            if (intHcSuit > 0)
            {
                string strHc, strHcNumber;
                var varOcr = OcrApi.Create();
                Bitmap bmpHc;                

                if (blnHc1)
                {
                    bmpHc = bmpScreenShot.Clone(new Rectangle(intHc1Dx, intHcDy, intCardDx, intCardDy), bmpScreenShot.PixelFormat);
                    strHcNumber = "Hc#1";
                }
                else
                {
                    bmpHc = bmpScreenShot.Clone(new Rectangle(intHc2Dx, intHcDy, intCardDx, intCardDy), bmpScreenShot.PixelFormat);
                    strHcNumber = "Hc#2";
                }

                // In the most recent update of Bet Online they don't highlight the winning combination of cards by raising them like they used
                // to so we never need to change the rectangle we send to the "get text from image" tool which is why i have commented out the 
                // code below. 2020.04.04
                /*
                Rectangle rctCropped;
                
                if (CardRaisedHc(blnHc1))
                {
                    rctCropped = new Rectangle(0, 0, bmpHc.Width, bmpHc.Height - 12);
                }
                else
                {
                    rctCropped = new Rectangle(0, 12, bmpHc.Width, bmpHc.Height - 12);
                }
                */

                bmpHc = bmpHc.Clone(new Rectangle(0, 0, bmpHc.Width, bmpHc.Height), bmpHc.PixelFormat);
                ColorSimplifyCard(bmpHc);

                varOcr.Init(Patagames.Ocr.Enums.Languages.English);
                strHc = varOcr.GetTextFromImage(bmpHc);
                varOcr.Release();

                if (!string.IsNullOrWhiteSpace(strHc))
                {
                    strHc = strHc.Substring(0, strHc.IndexOf("\n"));
                }

                if (strHc == "E" || strHc == "B" || strHc == "b")
                {
                    strHc = "6";
                }
                else if (strHc == "l]" || strHc == "[l")
                {
                    strHc = "Q";
                }

                intHc = CardSuitAndStringToInt(intHcSuit, strHc);

                if (blnSaveHcBmp)
                {
                    SaveBitmapHc(bmpHc, strHcNumber);
                }
            }

            return intHc;
        }

        public int GetBoardCard()
        {
            int intBoardCard, intBoardCardSuit;
            Color clrPix1, clrPix2, clrPix3;

            intBoardCard = -1;

            pntBoardCardSuitCheck_1.X = intCardDx - 3;
            pntBoardCardSuitCheck_1.Y = 0;
            pntBoardCardSuitCheck_2.X = intCardDx - 2;
            pntBoardCardSuitCheck_2.Y = 0;
            pntBoardCardSuitCheck_3.X = intCardDx - 1;
            pntBoardCardSuitCheck_3.Y = 0;

            clrPix1 = bmpScreenShot.GetPixel(pntBoardCardSuitCheck_1.X, pntBoardCardSuitCheck_1.Y);
            clrPix2 = bmpScreenShot.GetPixel(pntBoardCardSuitCheck_2.X, pntBoardCardSuitCheck_2.Y);
            clrPix3 = bmpScreenShot.GetPixel(pntBoardCardSuitCheck_3.X, pntBoardCardSuitCheck_3.Y);

            intBoardCardSuit = GetCardSuit(clrPix1, clrPix2, clrPix3);

            // Check to make sure a card is present
            if (intBoardCardSuit > 0)
            {
                string strBoardCard;
                var varOcr = OcrApi.Create();

                // In the most recent update of Bet Online they don't highlight the winning combination of cards by raising them like they used
                // to so we never need to change the rectangle we send to the "get text from image" tool which is why i have commented out the 
                // code below. 2020.04.04
                /*
                Rectangle rctCropped;

                if (CardRaised(bmpScreenShot))
                {
                    rctCropped = new Rectangle(0, 0, bmpScreenShot.Width, bmpScreenShot.Height - 12);
                }
                else
                {
                    rctCropped = new Rectangle(0, 12, bmpScreenShot.Width, bmpScreenShot.Height - 12);
                }
                bmpScreenShot = bmpScreenShot.Clone(rctCropped, bmpScreenShot.PixelFormat);
                */

                bmpScreenShot = bmpScreenShot.Clone(new Rectangle(0, 0, bmpScreenShot.Width, bmpScreenShot.Height), bmpScreenShot.PixelFormat);
                ColorSimplifyCard(bmpScreenShot);

                varOcr.Init(Patagames.Ocr.Enums.Languages.English);
                strBoardCard = varOcr.GetTextFromImage(bmpScreenShot);
                varOcr.Release();

                if (!string.IsNullOrWhiteSpace(strBoardCard))
                {
                    strBoardCard = strBoardCard.Substring(0, strBoardCard.IndexOf("\n"));
                }

                if (strBoardCard == "E")
                {
                    strBoardCard = "6";
                }
                else if (strBoardCard == "B")
                {
                    strBoardCard = "6";
                }
                else if (strBoardCard == "l]")
                {
                    strBoardCard = "Q";
                }
                else if (strBoardCard == "[l")
                {
                    strBoardCard = "Q";
                }

                intBoardCard = CardSuitAndStringToInt(intBoardCardSuit, strBoardCard);
            }

            return intBoardCard;
        }

        public int GetPot()
        {
            int intPot;
            string strPot;
            var varOcr = OcrApi.Create();

            ColorSimplifyPot();

            varOcr.Init(Patagames.Ocr.Enums.Languages.English);
            strPot = varOcr.GetTextFromImage(bmpScreenShot);
            varOcr.Release();

            // Remove "Pot: " and any commas from the string as well as any misread characters
            strPot = strPot.Replace("\n", "");
            strPot = strPot.Replace("Pot: ", "");
            strPot = strPot.Replace(",", "");
            strPot = strPot.Replace(".", "");
            strPot = strPot.Replace("D", "0");
            strPot = strPot.Replace("Z", "2");
            strPot = strPot.Replace("o", "0");
            strPot = strPot.Replace("O", "0");

            if (!int.TryParse(strPot, out intPot))
            {
                if (strPot == "")
                {
                    intPot = -2;
                }
                else
                {
                    intPot = -1;
                }
            }

            return intPot;
        }

        public int FindDealer()
        {
            bool blnDealerFound;
            int intCount, intDealer, intX, intY;
            Color clrPixel;

            blnDealerFound = false;
            intCount = 0;
            intDealer = -1;

            while (!blnDealerFound && intCount < 10)
            {
                intX = intLocations[intCount][3, 2] - intLocations[16][0, 0];
                intY = intLocations[intCount][3, 3] - intLocations[16][0, 1];

                // Get color
                clrPixel = bmpScreenShot.GetPixel(intX, intY);

                if (clrPixel.R > 180)
                {
                    intDealer = intCount;

                    blnDealerFound = true;
                }

                intCount++;
            }

            return intDealer;
        }

        public bool DealerHasHoldCards(int intDealer)
        {
            bool blnDealHasHoldCards = false;
            Color clrPixel1, clrPixel2, clrPixel3, clrPixel4;

            // Firts hold card (no need to check for a second hold card, if the first hold card is present the dealer has cards)
            clrPixel1 = bmpScreenShot.GetPixel(BitmapRect(intDealer, 0).Left + intHcStatusDx1 - intLocations[16][0, 0], BitmapRect(intDealer, 0).Top + intHcStatusDy1 - intLocations[16][0, 1]);
            clrPixel2 = bmpScreenShot.GetPixel(BitmapRect(intDealer, 0).Left + intHcStatusDx1 + 1 - intLocations[16][0, 0], BitmapRect(intDealer, 0).Top + intHcStatusDy1 - intLocations[16][0, 1]);
            clrPixel3 = bmpScreenShot.GetPixel(BitmapRect(intDealer, 0).Left + intHcStatusDx1 - 1 - intLocations[16][0, 0], BitmapRect(intDealer, 0).Top + intHcStatusDy2 - intLocations[16][0, 1]);
            clrPixel4 = bmpScreenShot.GetPixel(BitmapRect(intDealer, 0).Left + intHcStatusDx1 - intLocations[16][0, 0], BitmapRect(intDealer, 0).Top + intHcStatusDy2 - intLocations[16][0, 1]);

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((clrPixel1.R > 220 && clrPixel1.G > 220 && clrPixel1.B > 220) || (clrPixel2.R > 220 && clrPixel2.G > 220 && clrPixel2.B > 220))
            {
                blnDealHasHoldCards = true;
            }
            // Check for corner white area or brighness of a card that is present and shown
            else if (clrPixel3.R > 100 || clrPixel4.R > 100 || clrPixel3.B > 100 || clrPixel4.B > 100 || (clrPixel3.R > 3 && clrPixel3.G > 100) || (clrPixel4.R > 3 && clrPixel4.G > 100))
            {
                blnDealHasHoldCards = true;
            }

            return blnDealHasHoldCards;
        }

        public int FindActionPlayer()
        {
            bool blnActionPlayerFound;
            int intCount, intActionPlayer, intX, intY;
            Color clrPixel;

            blnActionPlayerFound = false;
            intCount = 0;
            intActionPlayer = -1;

            while (!blnActionPlayerFound && intCount < 10)
            {
                intX = intLocations[intCount][3, 0] - intLocations[16][0, 0];
                intY = intLocations[intCount][3, 1] - intLocations[16][0, 1];

                // Get color
                clrPixel = bmpScreenShot.GetPixel(intX, intY);

                if (clrPixel.R > 150)
                {
                    intActionPlayer = intCount;

                    blnActionPlayerFound = true;
                }

                intCount++;
            }

            return intActionPlayer;
        }

        public void NewScreenShot(Bitmap bmpNewSs)
        {
            if (blnDisposeScreenShotResources)
            {
                bmpScreenShot.Dispose();
                gfxScreenShot.Dispose();
            }

            bmpScreenShot = bmpNewSs;
            gfxScreenShot = Graphics.FromImage(bmpScreenShot);

            blnDisposeScreenShotResources = true;
        }

        public Bitmap ShareScreenShot()
        {
            return bmpScreenShot;
        }

        public void ChangeBitmapSaveLocation(string strNewSaveLocation)
        {
            strBmpSaveLocation = strNewSaveLocation;
        }

        public void SaveBitmap()
        {
            bmpScreenShot.Save(strBmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SaveBitmapPlayer(Bitmap bmp)
        {
            bmp.Save(strBmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + " Player Name" + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SaveBitmapChips(Bitmap bmp)
        {
            bmp.Save(strBmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + " Player Chips" + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void SaveBitmapHc(Bitmap bmp, string strHc)
        {
            bmp.Save(strBmpSaveLocation + "\\" + DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + "_" + strHc + 
                ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public Rectangle BitmapRect(int intId, int intIndex)
        {
            Rectangle rctToCopy;
            if (intId > -1 && intId < 17 && intIndex > -1 && intIndex < 4)
            {
                if (intIndex == 0 || intId < 10)
                {
                    rctToCopy = new Rectangle(intLocations[intId][intIndex, 0], intLocations[intId][intIndex, 1], intLocations[intId][intIndex, 2], intLocations[intId][intIndex, 3]);
                }
                else
                {
                    rctToCopy = new Rectangle(0, 0, 0, 0);
                }
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc1FaceDownCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntFirstHoldCardPresent_1.X, intLocations[intId][0, 1] + pntFirstHoldCardPresent_1.Y,
                    pntFirstHoldCardPresent_2.X - pntFirstHoldCardPresent_1.X, pntFirstHoldCardPresent_2.Y - pntFirstHoldCardPresent_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc1FaceUpCheckRect_1(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntFirstHoldCardPresent_3.X, intLocations[intId][0, 1] + pntFirstHoldCardPresent_3.Y, 0, 0);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc1FaceUpCheckRect_2(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntFirstHoldCardPresent_4.X, intLocations[intId][0, 1] + pntFirstHoldCardPresent_4.Y, 0, 0);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc2FaceDownCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntSecondHoldCardPresent_1.X, intLocations[intId][0, 1] + pntSecondHoldCardPresent_1.Y,
                    pntSecondHoldCardPresent_2.X - pntSecondHoldCardPresent_1.X, pntSecondHoldCardPresent_2.Y - pntSecondHoldCardPresent_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc2FaceUpCheckRect_1(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntSecondHoldCardPresent_3.X, intLocations[intId][0, 1] + pntSecondHoldCardPresent_3.Y, 0, 0);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc2FaceUpCheckRect_2(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntSecondHoldCardPresent_4.X, intLocations[intId][0, 1] + pntSecondHoldCardPresent_4.Y, 0, 0);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle AvitarPresentCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntAvitarPresentCheckPixel_1.X, intLocations[intId][0, 1] + pntAvitarPresentCheckPixel_1.Y,
                    pntAvitarPresentCheckPixel_2.X - pntAvitarPresentCheckPixel_1.X, pntAvitarPresentCheckPixel_2.Y - pntAvitarPresentCheckPixel_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle OpenSeatCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntOpenSeatCheckPixel_1.X, intLocations[intId][0, 1] + pntOpenSeatCheckPixel_1.Y,
                    pntOpenSeatCheckPixel_2.X - pntOpenSeatCheckPixel_1.X, pntOpenSeatCheckPixel_2.Y - pntOpenSeatCheckPixel_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle BannerPresentCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntBannerPresentCheck.X, intLocations[intId][0, 1] + pntBannerPresentCheck.Y, 0, 0);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle BoardCardSuitCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > 9 && intId < 15)
            {
                rctToCopy = new Rectangle(intLocations[intId][0, 0] + pntBoardCardSuitCheck_1.X, intLocations[intId][0, 1] + pntBoardCardSuitCheck_1.Y, 
                    pntBoardCardSuitCheck_3.X - pntBoardCardSuitCheck_1.X, pntBoardCardSuitCheck_3.Y - pntBoardCardSuitCheck_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc1SuitCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][1, 0] + pntHc1SuitCheck_1.X -intHc1Dx, intLocations[intId][1, 1] + pntHc1SuitCheck_1.Y - intHcDy,
                    pntHc1SuitCheck_3.X - pntHc1SuitCheck_1.X, pntHc1SuitCheck_3.Y - pntHc1SuitCheck_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public Rectangle Hc2SuitCheckRect(int intId)
        {
            Rectangle rctToCopy;

            if (intId > -1 && intId < 10)
            {
                rctToCopy = new Rectangle(intLocations[intId][2, 0] + pntHc2SuitCheck_1.X - intHc2Dx, intLocations[intId][2, 1] + pntHc2SuitCheck_1.Y - intHcDy,
                    pntHc2SuitCheck_3.X - pntHc2SuitCheck_1.X, pntHc2SuitCheck_3.Y - pntHc2SuitCheck_1.Y);
            }
            else
            {
                rctToCopy = new Rectangle(0, 0, 0, 0);
            }

            return rctToCopy;
        }

        public void DrawDealerRects()
        {
            int intX, intY;
            Pen penRed;

            penRed = new Pen(Color.Red, 1);

            for (int i = 0; i < 10; i++)
            {
                intX = intLocations[i][3, 2] - intLocations[16][0, 0];
                intY = intLocations[i][3, 3] - intLocations[16][0, 1];

                gfxScreenShot.DrawRectangle(penRed, intX - 1, intY - 1, 2, 2);
            }
        }

        public void DrawHoldCardCheckRects(int intDealer)
        {
            int intX, intY;
            Pen penYellow, penPurple, penPen;

            penYellow = new Pen(Color.Yellow, 1);
            penPurple = new Pen(Color.Purple, 1);

            for (int i = 0; i < 9; i++)
            {
                if (i == intDealer)
                {
                    penPen = penYellow;
                }
                else
                {
                    penPen = penPurple;
                }

                intX = BitmapRect(i, 0).Left + intHcStatusDx1 - intLocations[16][0, 0];
                intY = BitmapRect(i, 0).Top + intHcStatusDy1 - intLocations[16][0, 1];
                gfxScreenShot.DrawRectangle(penYellow, intX - 1, intY - 1, 3, 2);

                intX = BitmapRect(i, 0).Left + intHcStatusDx1 - 1 - intLocations[16][0, 0];
                intY = BitmapRect(i, 0).Top + intHcStatusDy2 - intLocations[16][0, 1];
                gfxScreenShot.DrawRectangle(penPurple, intX - 1, intY - 1, 3, 2);
            }
        }

        public void DrawActionBarRects()
        {
            int intX, intY;
            Pen penBlack;

            penBlack = new Pen(Color.Black, 1);

            for (int i = 0; i < 10; i++)
            {
                intX = intLocations[i][3, 0] - intLocations[16][0, 0];
                intY = intLocations[i][3, 1] - intLocations[16][0, 1];

                gfxScreenShot.DrawRectangle(penBlack, intX - 1, intY - 1, 2, 2);
            }
        }

        public string CardIntToString(int intCard)
        {
            return cdt.CardIntToString(intCard);
        }

        public void PlayerCount9()
        {
            InitializeIntLocations(true);
        }

        public void PlayerCount10()
        {
            InitializeIntLocations(false);
        }

        #endregion

        #region Helper Methods

        private bool AvitarPresent()
        {
            Color clrPixel1, clrPixel2;

            pntAvitarPresentCheckPixel_1.X = 72;
            pntAvitarPresentCheckPixel_1.Y = 27 + intDyPlayerInfo;
            pntAvitarPresentCheckPixel_2.X = 73;
            pntAvitarPresentCheckPixel_2.Y = 27 + intDyPlayerInfo;

            clrPixel1 = bmpScreenShot.GetPixel(pntAvitarPresentCheckPixel_1.X, pntAvitarPresentCheckPixel_1.Y);
            clrPixel2 = bmpScreenShot.GetPixel(pntAvitarPresentCheckPixel_2.X, pntAvitarPresentCheckPixel_2.Y);

            if (clrPixel1.R > 75 || clrPixel2.R > 75)
            {
                blnAvitarPresent = true;
            }
            else
            {
                blnAvitarPresent = false;
            }

            return blnAvitarPresent;
        }

        private void CheckIfSittingOut(Bitmap bmp)
        {
            int intXStart, intXEnd, intY;
            Color clrPixel;

            intXStart = bmp.Width / 3;
            intXEnd = intXStart * 2;
            intY = bmp.Height / 2;
            blnPlayerSittingOut = true;

            if (!blnBannerPresent)
            {
                for (int i = intXStart; i < intXEnd; i++)
                {
                    clrPixel = bmp.GetPixel(i, intY);

                    if (clrPixel.R > 250 && clrPixel.R == clrPixel.G && clrPixel.R == clrPixel.B)
                    {
                        blnPlayerSittingOut = false;
                        i = bmp.Width;
                    }
                }
            }
            else
            {
                blnPlayerSittingOut = false;
            }
        }

        private bool BannerPresent()
        {
            Color clrPixel;

            pntBannerPresentCheck.X = bmpScreenShot.Width / 2;
            pntBannerPresentCheck.Y = bmpScreenShot.Height - 2;

            clrPixel = bmpScreenShot.GetPixel(pntBannerPresentCheck.X, pntBannerPresentCheck.Y);

            if (clrPixel.R > 150 && clrPixel.G > 150 && clrPixel.B > 150)
            {
                blnBannerPresent = true;
            }
            else
            {
                blnBannerPresent = false;
            }

            return blnBannerPresent;
        }

        private int GetCardSuit(Color clrPixel1, Color clrPixel2, Color clrPixel3)
        {
            int intSuit;

            // intSuit
            // -1 = not present, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades

            if (clrPixel2.R == clrPixel2.G && clrPixel2.G == clrPixel2.B)
            {
                intSuit = 4;
            }
            else if (clrPixel2.R > clrPixel2.G && clrPixel2.R > clrPixel2.B)
            {
                intSuit = 3;
            }
            else if (clrPixel2.B > clrPixel2.R && clrPixel2.B > clrPixel2.G)
            {
                intSuit = 2;
            }
            else if (clrPixel2.G > clrPixel2.R && clrPixel2.G > clrPixel2.B)
            {
                if (clrPixel1.R > 20 || clrPixel2.R > 20 || clrPixel3.R > 20)
                {
                    intSuit = 1;
                }
                else
                {
                    intSuit = -1;
                }
            }
            else
            {
                intSuit = -1;
            }
            
            return intSuit;
        }

        private bool CardRaised(Bitmap bmpCard)
        {
            bool blnRaised;
            Color clrPixel;

            clrPixel = bmpCard.GetPixel(bmpCard.Width / 2, 0);

            if (clrPixel.R > 175 || clrPixel.G > 175 || clrPixel.B > 175)
            {
                blnRaised = true;
            }
            else
            {
                blnRaised = false;
            }

            return blnRaised;
        }

        private bool CardRaisedHc(bool blnHc1_)
        {
            bool blnRaised;
            Color clrPixel;

            if (blnHc1_)
            {
                clrPixel = bmpScreenShot.GetPixel(intHc1Dx + 5, 0);
            }
            else
            {
                clrPixel = bmpScreenShot.GetPixel(intHc2Dx + 55, 0);
            }

            if (clrPixel.R > 175 || clrPixel.G > 175 || clrPixel.B > 175)
            {
                blnRaised = true;
            }
            else
            {
                blnRaised = false;
            }

            return blnRaised;
        }

        private void ColorSimplifyPot()
        {
            // int intColorThreashold;
            Color clrPixel;

            // intColorThreashold = 220;

            for (int i = 0; i < bmpScreenShot.Width; i++)
            {
                for (int j = 0; j < bmpScreenShot.Height; j++)
                {
                    clrPixel = bmpScreenShot.GetPixel(i, j);

                    //if (clrPixel.R < intColorThreashold)
                    if (clrPixel.G > clrPixel.R - 1 && clrPixel.G < clrPixel.R + 1)
                    {
                        bmpScreenShot.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bmpScreenShot.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        private void ColorSimplifyCard(Bitmap bmpCard)
        {
            int intColorThreashold;
            Color clrPixel;

            intColorThreashold = 155;

            // Test for card brightness (basically if the card is in the fadded/darkened state) before setting the color threashold
            for (int i = 0; i < bmpCard.Width; i++)
            {
                clrPixel = bmpCard.GetPixel(i, bmpCard.Height / 2);

                if (clrPixel.R > 252 && clrPixel.G > 252 && clrPixel.B > 252)
                {
                    intColorThreashold = 252;
                    i = bmpCard.Width;
                }
            }

            for (int i = 0; i < bmpCard.Width; i++)
            {
                for (int j = 0; j < bmpCard.Height; j++)
                {
                    clrPixel = bmpCard.GetPixel(i, j);

                    if (clrPixel.R == clrPixel.G && clrPixel.G == clrPixel.B && clrPixel.R > intColorThreashold)
                    {
                        bmpCard.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bmpCard.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        private void ColorSimplifyPlayerName(Bitmap bmpPlyrName)
        {
            int intColorThreashold;
            Color clrPixel;

            intColorThreashold = 225;

            for (int i = 0; i < bmpPlyrName.Width; i++)
            {
                for (int j = 0; j < bmpPlyrName.Height; j++)
                {
                    clrPixel = bmpPlyrName.GetPixel(i, j);

                    // if clrPixel.R >= intColorThreashold this as a white character
                    // else if clrPixel.B > 200 && clrPixel.B > 2*clrPixel.R register this as a blue character "Fold", "Bet", "Call",  
                    // "Check", "All-In" etc.
                    if (clrPixel.R >= intColorThreashold || (clrPixel.B > 200 && clrPixel.B > 2 * clrPixel.R))
                    {
                        bmpPlyrName.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bmpPlyrName.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        private void ColorSimplifyChipStack(Bitmap bmpChpStack)
        {
            int intColorThreashold;
            Color clrPixel;

            intColorThreashold = 150;

            for (int i = 0; i < bmpChpStack.Width; i++)
            {
                for (int j = 0; j < bmpChpStack.Height; j++)
                {
                    clrPixel = bmpChpStack.GetPixel(i, j);

                    if (clrPixel.G > (clrPixel.R + 2) && clrPixel.G > (clrPixel.B + 2) && clrPixel.G > intColorThreashold)
                    {
                        bmpChpStack.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bmpChpStack.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        private int CardSuitAndStringToInt(int intSuit, string strCard)
        {
            // -1 = not present, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades
            return cdt.CardSuitAndStringToInt(intSuit, strCard);
        }

        #endregion

        #region Initializations/Setup

        private void InitializeArrays()
        {
            // Default is a 9-player table (the true parameter of this methods means 9-player table)
            InitializeIntLocations(true);
        }

        #endregion

        #region Locations Arrays
        // These arrays define where to look for the data (chip stacks, hold cards, action player, dealer, board cards etc.)

        private void InitializeIntLocations(bool bln9Players)
        {
            int intPlayerDx, intPlayerDy, intActionBarDx, intActionBarDy, intBoardDx, intPotDx, intPotDy;

            intPlayerDx = 258;
            intPlayerDy = 171;
            intActionBarDx = -4;
            intActionBarDy = 192;
            intBoardDx = 117;
            intPotDx = 123;
            intPotDy = 19;

            #region Locations array defining dimensions

            intLocations = new int[17][,];

            if (bln9Players)
            {
                // This method sets the values for the intloactions array for a 9 player table
                PlayerLocations9Players();
            }
            else
            {
                // This method sets the values for the intloactions array for a 10 player table
                PlayerLocations10Players();
            }

            // Boar Card 1 (Flop 1): x1, y1, x2, y2
            intLocations[10] = new int[1, 4]
            {
                {673, 348, 0, 0}
            };

            // Boar Card 2 (Flop 2): x1, y1, x2, y2
            intLocations[11] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 3 (Flop 3): x1, y1, x2, y2
            intLocations[12] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 4 (Turn): x1, y1, x2, y2
            intLocations[13] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 5 (River): x1, y1, x2, y2
            intLocations[14] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Pot: x1, y1, x2, y2
            intLocations[15] = new int[1, 4]
            {
                {899, 269, 0, 0}
            };

            // Area of interest (everything needed to collect game data): x1, y1, width, height
            intLocations[16] = new int[1, 4]
            {
                {251, 41, 1215, 763}
            };

            #endregion

            #region Add in constant offsets

            // For player locations
            for (int i = 0; i < 10; i++)
            {
                intLocations[i][0, 2] = intPlayerDx;
                intLocations[i][0, 3] = intPlayerDy;
                intLocations[i][1, 0] = intLocations[i][0, 0] + intHc1Dx;
                intLocations[i][1, 1] = intLocations[i][0, 1] + intHcDy;
                intLocations[i][1, 2] = intCardDx;
                intLocations[i][1, 3] = intCardDy;
                intLocations[i][2, 0] = intLocations[i][0, 0] + intHc2Dx;
                intLocations[i][2, 1] = intLocations[i][0, 1] + intHcDy;
                intLocations[i][2, 2] = intCardDx;
                intLocations[i][2, 3] = intCardDy;
                intLocations[i][3, 0] = intLocations[i][0, 0] + intActionBarDx;
                intLocations[i][3, 1] = intLocations[i][0, 1] + intActionBarDy;
            }

            // For board card locations
            for (int i = 10; i < 15; i++)
            {
                if (i > 10)
                {
                    intLocations[i][0, 0] = intLocations[i - 1][0, 0] + intBoardDx;
                }

                intLocations[i][0, 2] = intCardDx;
                intLocations[i][0, 3] = intCardDy;
            }

            // For Pot Annoucement Location
            intLocations[15][0, 2] = intPotDx;
            intLocations[15][0, 3] = intPotDy;

            #endregion

        }

        private void PlayerLocations9Players()
        {
            // intLocations[-][0, -] = Player info (name and chipstack) (x, y, width, height)
            // intLocations[-][1, -] = Hold Card 1 info (x1, y1, width, height)
            // intLocations[-][2, -] = Hold Card 2 info (x2, y2, width, height)
            // intLocations[-][3, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

            // Player 1: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[0] = new int[4, 4]
            {
                {288, 161, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 580, 298}
            };

            // Player 2: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[1] = new int[4, 4]
            {
                {621, 31, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 669, 248}
            };

            // Player 3: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[2] = new int[4, 4]
            {
                {1037, 31, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1251, 248}
            };

            // Player 4: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[3] = new int[4, 4]
            {
                {1370, 161, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1339, 298}
            };

            // Player 5: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[4] = new int[4, 4]
            {
                {1403, 369, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1411, 450}
            };

            // Player 6: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[5] = new int[4, 4]
            {
                {1163, 569, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1133, 676}
            };

            // Player 7: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[6] = new int[4, 4]
            {
                {829, 611, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 837, 685}
            };

            // Player 8: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[7] = new int[4, 4]
            {
                {495, 569, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 751, 646}
            };

            // Player 9: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[8] = new int[4, 4]
            {
                {255, 369, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 509, 450}
            };

            // Player 10: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            // This player doesn't exist in this configuration
            intLocations[9] = new int[4, 4]
            {
                {289, 162, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 581, 299}
            };
        }

        private void PlayerLocations10Players()
        {
            // intLocations[-][0, -] = Player info (name and chipstack) (x, y, width, height)
            // intLocations[-][1, -] = Hold Card 1 info (x1, y1, width, height)
            // intLocations[-][2, -] = Hold Card 2 info (x2, y2, width, height)
            // intLocations[-][3, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

            // Player 1: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[0] = new int[4, 4]
            {
                {286, 255, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 563, 413}
            };

            // Player 2: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[1] = new int[4, 4]
            {
                {525, 104, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 802, 245}
            };

            // Player 3: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[2] = new int[4, 4]
            {
                {837, 63, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1081, 262}
            };

            // Player 4: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[3] = new int[4, 4]
            {
                {1149, 104, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1303, 298}
            };

            // Player 5: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[4] = new int[4, 4]
            {
                {1387, 255, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1356, 414}
            };

            // Player 6: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[5] = new int[4, 4]
            {
                {1387, 447, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1358, 552}
            };

            // Player 7: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[6] = new int[4, 4]
            {
                {1149, 589, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1118, 670}
            };

            // Player 8: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[7] = new int[4, 4]
            {
                {837, 625, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 839, 672}
            };

            // Player 9: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[8] = new int[4, 4]
            {
                {555, 589, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 766, 656}
            };

            // Player 10: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[9] = new int[4, 4]
            {
                {555, 447, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 560, 552}
            };
        }

        #endregion

        #region Properties

        #endregion
    }
}
