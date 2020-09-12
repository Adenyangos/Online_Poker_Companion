using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A class that converts between string and integer representations of playing cards.
    /// </summary>
    public static class CardTranslator
    {
        /// <summary>
        /// The largest valid integer representation of a playing card
        /// </summary>
        public static int MaxValidCard { get; } = 52;

        /// <summary>
        /// The smallest valid integer representation of a playing card
        /// </summary>
        public static int MinValidCard { get; } = -1;

        /// <summary>
        /// The integer representation of "no card present"
        /// </summary>
        public static int NoCardPresent { get; } = -1;

        /// <summary>
        /// The integer representation of "card face down"
        /// </summary>
        public static int CardFaceDown { get; } = 0;

        /// <summary>
        /// The integer representation of the lowest face-up card value
        /// </summary>
        public static int LowestFaceUpCardValue { get; } = 1;

        #region Public Methods

        /// <summary>
        /// Convert the integer representation of a playing card to the string represenation of that card.
        /// </summary>
        /// <param name="cardInt">Integer representaion of a playing card</param>
        /// <returns>The string represenation of the card</returns>
        public static string CardIntToString(int cardInt)
        {
            // No card present: 
            //      integer representation = -1
            //      string representaiton = " - - "
            // Card face down:
            //       integer representation = 0
            //       string representaiton = "XX" 

            // The string representation of the cardInt. Setting default value to "ERROR!"
            string cardStr = "ERROR!";

            // This is an actual playing card (52 playing cards in a deck)
            if (cardInt >= 1 && cardInt <= 52)
            {
                cardStr = CardValueIntToStr(cardInt) + CardSuitIntToString(cardInt);
            }
            // No card
            else if (cardInt == -1)
            {
                cardStr = " - - ";
            }
            // Card face down
            else if (cardInt == 0)
            {
                cardStr = "XX";
            }

            return cardStr;
        }

        /// <summary>
        /// Convert the string representation of a playing card to the integer representation of that card.
        /// </summary>
        /// <param name="suit">The card's suit</param>
        /// <param name="cardStr">The string representation of the playing card's value</param>
        /// <returns>The integer representation of the card</returns>
        public static int CardSuitAndStringToInt(CardSuit suit, string cardStr)
        {
            // The integer representation of the card.
            int cardInt;

            // Get the integer representation of the card's value
            cardInt = CardValueStrToInt(cardStr);

            // Make sure the value of the card is an actuial playing card
            if (cardInt != NoCardPresent)
            {
                // Clubs = card value + 0
                // Diamonds = card value + 13
                // Hearts = card value + 26
                // Spades = card value + 39

                // Add the integer offset that represents the card's suit
                cardInt += CardSuitOffset(suit);
            }

            return cardInt;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert the integer representation of a playing card to the string represenation of that card's value.
        /// </summary>
        /// <param name="cardInt_">The integer representation of the playing card</param>
        /// <returns>The string representation of the playing card's value</returns>
        static private string CardValueIntToStr(int cardInt_)
        {
            // The integer value of the card excluding the suit
            int cardValueInt = cardInt_ % 13;

            // The string value of the card excluding the suit
            string cardValueStr = "";

            switch (cardValueInt)
            {
                case 0:
                    cardValueStr = "K";
                    break;
                case 1:
                    cardValueStr = "A";
                    break;
                case 2:
                    cardValueStr = "2";
                    break;
                case 3:
                    cardValueStr = "3";
                    break;
                case 4:
                    cardValueStr = "4";
                    break;
                case 5:
                    cardValueStr = "5";
                    break;
                case 6:
                    cardValueStr = "6";
                    break;
                case 7:
                    cardValueStr = "7";
                    break;
                case 8:
                    cardValueStr = "8";
                    break;
                case 9:
                    cardValueStr = "9";
                    break;
                case 10:
                    cardValueStr = "10";
                    break;
                case 11:
                    cardValueStr = "J";
                    break;
                case 12:
                    cardValueStr = "Q";
                    break;
            }

            return cardValueStr;
        }

        /// <summary>
        /// Convert the string representation of a playing card's value to the integer represenation of that card's value.
        /// </summary>
        /// <param name="CardValueStr">The string representation of the playing card's value</param>
        /// <returns>The integer represenation of the playing card's value</returns>
        static private int CardValueStrToInt(string CardValueStr)
        {
            // The integer representation of the card's value. Setting default value to the integer representation of no card present
            int cardValueInt = NoCardPresent;

            switch (CardValueStr)
            {
                case "A":
                    cardValueInt = 1;
                    break;
                case "2":
                    cardValueInt = 2;
                    break;
                case "3":
                    cardValueInt = 3;
                    break;
                case "4":
                    cardValueInt = 4;
                    break;
                case "5":
                    cardValueInt = 5;
                    break;
                case "6":
                    cardValueInt = 6;
                    break;
                case "7":
                    cardValueInt = 7;
                    break;
                case "8":
                    cardValueInt = 8;
                    break;
                case "9":
                    cardValueInt = 9;
                    break;
                case "10":
                    cardValueInt = 10;
                    break;
                case "J":
                    cardValueInt = 11;
                    break;
                case "Q":
                    cardValueInt = 12;
                    break;
                case "K":
                    cardValueInt = 13;
                    break;
            }

            return cardValueInt;
        }


        /// <summary>
        /// Convert the integer representation of a playing card to the string represenation of that card's suit.
        /// </summary>
        /// <param name="cardInt_">The integer representation of the playing card</param>
        /// <returns>The string representation of the playing card's suit</returns>
        static private string CardSuitIntToString(int cardInt_)
        {
            // The integer representation of the playing card's suit
            int cardSuitInt = (cardInt_ - 1) / 13;

            // The string representation of the playing card's suit
            string cardSuitStr = "";

            switch (cardSuitInt)
            {
                case 0:
                    cardSuitStr = "C";
                    break;
                case 1:
                    cardSuitStr = "D";
                    break;
                case 2:
                    cardSuitStr = "H";
                    break;
                case 3:
                    cardSuitStr = "S";
                    break;
            }

            return cardSuitStr;
        }

        /// <summary>
        /// The card's integer offset due to it's suit. (cards 1 - 13 are clubs, cards 14 - 26 are diamonds, cards 27 - 39 are hearts and cards 40 - 52 are spades)
        /// </summary>
        /// <param name="suit_">The playing card's suit</param>
        /// <returns>The card's integer offset based on it's suit</returns>
        static private int CardSuitOffset(CardSuit suit_)
        {
            // cardSuitInt:
            // -1 = no suit, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades

            // A card's integer offset due to it's suit
            int cardSuitIntOffset = 0;

            switch (suit_)
            {
                case CardSuit.diamonds:
                    cardSuitIntOffset += 13;
                    break;
                case CardSuit.hearts:
                    cardSuitIntOffset += 26;
                    break;
                case CardSuit.spades:
                    cardSuitIntOffset += 39;
                    break;
            }

            return cardSuitIntOffset;
        }

        #endregion

        #region Enums

        /// <summary>
        /// A playing card's suit (clubs, diamonds, hearts, spades or unknown)
        /// </summary>
        public enum CardSuit
        {
            unknown,
            clubs,
            diamonds,
            hearts,
            spades
        }

        #endregion
    }
}
