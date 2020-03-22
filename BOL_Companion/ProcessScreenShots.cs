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
        private bool blnDisposeScreenShotResources, blnAvitarPresent, blnPlayerSittingOut;
        private const int intDyPlayerInfo = 93;
        private const int intHcStatusDx1 = 71;
        private const int intHcStatusDx2 = 97;
        private const int intHcStatusDy1 = 77;
        private const int intHcStatusDy2 = 22;
        private const int intHc1Dx = 69;
        private const int intHc2Dx = 98;
        private const int intCardDx = 25;
        private const int intCardDy = 50;
        private int intIdentifier;
        private int[][,] intLocations;
        private Bitmap bmpScreenShot;
        private Graphics gfxScreenShot;
        private CardStringIntTranslator cdt;

        #endregion

        public ProcessScreenShots(int intId, bool blnDisplayThread)
        {
            intIdentifier = intId;

            InitializeVariables();
            InitializeArrays();
        }

        private void InitializeVariables()
        {
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

            clrPixel1 = bmpScreenShot.GetPixel(77, 26 + intDyPlayerInfo);
            clrPixel2 = bmpScreenShot.GetPixel(78, 26 + intDyPlayerInfo);

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
                rctCropped = new Rectangle(30, intDyPlayerInfo, bmpScreenShot.Width - 60, 23);
            }
            else
            {
                rctCropped = new Rectangle(70, intDyPlayerInfo, bmpScreenShot.Width - 70, 23);
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
                    rctCropped = new Rectangle(30, 30 + intDyPlayerInfo, bmpScreenShot.Width - 60, 23);
                }
                else
                {
                    rctCropped = new Rectangle(70, 30 + intDyPlayerInfo, bmpScreenShot.Width - 70, 23);
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

                strChipStack = strChipStack.Replace(",", "");
                strChipStack = strChipStack.Replace(".", "");

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
            clrPixel1 = bmpScreenShot.GetPixel(intHcStatusDx1, intHcStatusDy1);
            clrPixel2 = bmpScreenShot.GetPixel(intHcStatusDx1 + 1, intHcStatusDy1);
            clrPixel3 = bmpScreenShot.GetPixel(intHcStatusDx1 - 1, intHcStatusDy2);
            clrPixel4 = bmpScreenShot.GetPixel(intHcStatusDx1, intHcStatusDy2);

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((clrPixel1.R > 220 && clrPixel1.G > 220 && clrPixel1.B > 220) || (clrPixel2.R > 220 && clrPixel2.G > 220 && clrPixel2.B > 220))
            {
                intHc1Status = 0;
            }
            // Check for corner white area or brighness of a card that is present and shown
            else if (clrPixel3.R > 100 || clrPixel4.R > 100 || clrPixel3.B > 100 || clrPixel4.B > 100 || (clrPixel3.R > 3 && clrPixel3.G > 100) || (clrPixel4.R > 3 && clrPixel4.G > 100))
            {
                intHc1Status = 1;
            }
            else
            {
                intHc1Status = -1;
            }

            // Second hold card
            clrPixel1 = bmpScreenShot.GetPixel(intHcStatusDx2, intHcStatusDy1);
            clrPixel2 = bmpScreenShot.GetPixel(intHcStatusDx2 + 1, intHcStatusDy1);
            clrPixel3 = bmpScreenShot.GetPixel(intHcStatusDx2 + 3, intHcStatusDy2);
            clrPixel4 = bmpScreenShot.GetPixel(intHcStatusDx2 + 4, intHcStatusDy2);

            // Check for the white of the facedown card edge to check for hold card present but not shown
            if ((clrPixel1.R > 220 && clrPixel1.G > 220 && clrPixel1.B > 220) || (clrPixel2.R > 220 && clrPixel2.G > 220 && clrPixel2.B > 220))
            {
                intHc2Status = 0;
            }
            // Check for corner white area or brighness of a card that is present
            else if (clrPixel3.R > 100 || clrPixel4.R > 100 || clrPixel3.B > 100 || clrPixel4.B > 100 || (clrPixel3.R > 3 && clrPixel3.G > 100) || (clrPixel4.R > 3 && clrPixel4.G > 100))
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
            
            if (blnHc1)
            {
                clrPix1 = bmpScreenShot.GetPixel(intHc1Dx + 1, 35);
                clrPix2 = bmpScreenShot.GetPixel(intHc1Dx + 0, 35);
                clrPix3 = bmpScreenShot.GetPixel(intHc1Dx + 2, 35);
            }
            else
            {
                clrPix1 = bmpScreenShot.GetPixel(intHc2Dx + 1, 35);
                clrPix2 = bmpScreenShot.GetPixel(intHc2Dx + 0, 35);
                clrPix3 = bmpScreenShot.GetPixel(intHc2Dx + 2, 35);
            }

            intHcSuit = GetCardSuit(clrPix1, clrPix2, clrPix3);

            // Check to make sure a card is present
            if (intHcSuit > 0)
            {
                string strHc, strHcNumber;
                var varOcr = OcrApi.Create();
                Bitmap bmpHc;
                Rectangle rctCropped;

                if (blnHc1)
                {
                    bmpHc = bmpScreenShot.Clone(new Rectangle(intHc1Dx, 0, intCardDx, intCardDy), bmpScreenShot.PixelFormat);
                    strHcNumber = "Hc#1";
                }
                else
                {
                    bmpHc = bmpScreenShot.Clone(new Rectangle(intHc2Dx, 0, intCardDx, intCardDy), bmpScreenShot.PixelFormat);
                    strHcNumber = "Hc#2";
                }

                if (CardRaisedHc(blnHc1))
                {
                    rctCropped = new Rectangle(0, 0, bmpHc.Width, bmpHc.Height - 12);
                }
                else
                {
                    rctCropped = new Rectangle(0, 12, bmpHc.Width, bmpHc.Height - 12);
                }

                bmpHc = bmpHc.Clone(rctCropped, bmpHc.PixelFormat);
                ColorSimplifyCard(bmpHc);

                varOcr.Init(Patagames.Ocr.Enums.Languages.English);
                strHc = varOcr.GetTextFromImage(bmpHc);
                varOcr.Release();

                if (!string.IsNullOrWhiteSpace(strHc))
                {
                    strHc = strHc.Substring(0, strHc.IndexOf("\n"));
                }

                if (strHc == "E")
                {
                    strHc = "6";
                }
                else if (strHc == "l]")
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

            clrPix1 = bmpScreenShot.GetPixel(1, 35);
            clrPix2 = bmpScreenShot.GetPixel(0, 35);
            clrPix3 = bmpScreenShot.GetPixel(2, 35);

            intBoardCardSuit = GetCardSuit(clrPix1, clrPix2, clrPix3);

            // Check to make sure a card is present
            if (intBoardCardSuit > 0)
            {
                string strBoardCard;
                var varOcr = OcrApi.Create();
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
                else if (strBoardCard == "l]")
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

        public void SaveBitmap()
        {
            bmpScreenShot.Save("E:\\Documents\\Visual Studio 2017\\Projects\\Poker\\Bet Online_Other\\Saved Bitmaps\\" +
                DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SaveBitmapPlayer(Bitmap bmp)
        {
            bmp.Save("E:\\Documents\\Visual Studio 2017\\Projects\\Poker\\Bet Online_Other\\Saved Bitmaps\\" +
                DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + " Player Name" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SaveBitmapChips(Bitmap bmp)
        {
            bmp.Save("E:\\Documents\\Visual Studio 2017\\Projects\\Poker\\Bet Online_Other\\Saved Bitmaps\\" +
                DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + " Player Chips" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void SaveBitmapHc(Bitmap bmp, string strHc)
        {
            bmp.Save("E:\\Documents\\Visual Studio 2017\\Projects\\Poker\\Bet Online_Other\\Saved Bitmaps\\" +
                DateTime.Now.ToString("yyyy.MM.dd_") + intIdentifier.ToString("D2") + "_" + strHc + 
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
            Pen penBlue, penRed, penPen;

            penBlue = new Pen(Color.Blue, 1);
            penRed = new Pen(Color.Red, 1);

            for (int i = 0; i < 9; i++)
            {
                if (i == intDealer)
                {
                    penPen = penRed;
                }
                else
                {
                    penPen = penBlue;
                }

                // Firts hold card (no need to check for a second hold card, if the first hold card is present the dealer has cards)
                intX = BitmapRect(i, 0).Left + intHcStatusDx1 - intLocations[16][0, 0];
                intY = BitmapRect(i, 0).Top + intHcStatusDy1 - intLocations[16][0, 1];
                gfxScreenShot.DrawRectangle(penBlue, intX - 1, intY - 1, 3, 2);

                intX = BitmapRect(i, 0).Left + intHcStatusDx1 - 1 - intLocations[16][0, 0];
                intY = BitmapRect(i, 0).Top + intHcStatusDy2 - intLocations[16][0, 1];
                gfxScreenShot.DrawRectangle(penBlue, intX - 1, intY - 1, 3, 2);
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

            clrPixel1 = bmpScreenShot.GetPixel(67, 26 + intDyPlayerInfo);
            clrPixel2 = bmpScreenShot.GetPixel(68, 26 + intDyPlayerInfo);

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

        private bool BannerPresent()
        {
            bool blnBanner;
            Color clrPixel;

            clrPixel = bmpScreenShot.GetPixel(bmpScreenShot.Width / 2, bmpScreenShot.Height - 1);

            if (clrPixel.R > 150 && clrPixel.G > 150 && clrPixel.B > 150)
            {
                blnBanner = true;
            }
            else
            {
                blnBanner = false;
            }

            return blnBanner;
        }

        private int GetCardSuit(Color clrPixel1, Color clrPixel2, Color clrPixel3)
        {
            int intSuit;

            // intSuit
            // -1 = not present, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades

            if (clrPixel1.R == clrPixel1.G && clrPixel1.G == clrPixel1.B)
            {
                intSuit = 4;
            }
            else if (clrPixel1.R > clrPixel1.G && clrPixel1.R > clrPixel1.B)
            {
                intSuit = 3;
            }
            else if (clrPixel1.B > clrPixel1.R && clrPixel1.B > clrPixel1.G)
            {
                intSuit = 2;
            }
            else if (clrPixel1.G > clrPixel1.R && clrPixel1.G > clrPixel1.B)
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

        private void InitializeIntLocations(bool bln9Players)
        {
            int intPlayerDx, intPlayerDy, intActionBarDx, intActionBarDy, intHcDy, intBoardDx, intPotDx, intPotDy;

            intPlayerDx = 245;
            intPlayerDy = 155;
            intActionBarDx = -5;
            intActionBarDy = 175;
            intHcDy = 0;
            intBoardDx = 86;
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
                {749, 368, 0, 0}
            };

            // Boar Card 2 (Flop 2): x1, y1, x2, y2
            intLocations[11] = new int[1, 4]
            {
                {0, 368, 0, 0}
            };

            // Boar Card 3 (Flop 3): x1, y1, x2, y2
            intLocations[12] = new int[1, 4]
            {
                {0, 368, 0, 0}
            };

            // Boar Card 4 (Turn): x1, y1, x2, y2
            intLocations[13] = new int[1, 4]
            {
                {0, 368, 0, 0}
            };

            // Boar Card 5 (River): x1, y1, x2, y2
            intLocations[14] = new int[1, 4]
            {
                {0, 368, 0, 0}
            };

            // Pot: x1, y1, x2, y2
            intLocations[15] = new int[1, 4]
            {
                {902, 289, 0, 0}
            };

            // Area of interest (everything needed to collect game data)
            intLocations[16] = new int[1, 4]
            {
                {279, 81, 1182, 723}
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
            // intLocations[1, -] = Player info (name and chipstack) (x, y, width, height)
            // intLocations[1, -] = Hold Card 1 info (x1, y1, width, height)
            // intLocations[1, -] = Hold Card 2 info (x2, y2, width, height)
            // intLocations[1, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

            // Player 1: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[0] = new int[4, 4]
            {
                {317, 194, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 597, 317}
            };

            // Player 2: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[1] = new int[4, 4]
            {
                {638, 69, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 681, 269}
            };

            // Player 3: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[2] = new int[4, 4]
            {
                {1036, 69, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1238, 269}
            };

            // Player 4: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[3] = new int[4, 4]
            {
                {1356, 194, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1323, 317}
            };

            // Player 5: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[4] = new int[4, 4]
            {
                {1387, 393, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1392, 463}
            };

            // Player 6: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[5] = new int[4, 4]
            {
                {1157, 585, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1126, 679}
            };

            // Player 7: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[6] = new int[4, 4]
            {
                {837, 625, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 842, 688}
            };

            // Player 8: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[7] = new int[4, 4]
            {
                {516, 585, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 759, 650}
            };

            // Player 9: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[8] = new int[4, 4]
            {
                {286, 393, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 527, 463}
            };

            // Player 10: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            // This player doesn't exist in this configuration
            intLocations[9] = new int[4, 4]
            {
                {285, 627, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 285, 802}
            };
        }

        private void PlayerLocations10Players()
        {
            // intLocations[1, -] = Player info (name and chipstack) (x, y, width, height)
            // intLocations[1, -] = Hold Card 1 info (x1, y1, width, height)
            // intLocations[1, -] = Hold Card 2 info (x2, y2, width, height)
            // intLocations[1, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

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
                {525, 589, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 766, 656}
            };

            // Player 10: x1, y1, x2, y2 for name and chips stack, hold card 1, hold card 2
            // and x1, y1 for action bar and dealer chip
            intLocations[9] = new int[4, 4]
            {
                {286, 447, 0, 0},
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
