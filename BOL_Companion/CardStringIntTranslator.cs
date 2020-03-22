using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    class CardStringIntTranslator
    {
        public CardStringIntTranslator()
        {

        }

        public string CardIntToString(int intCard)
        {
            // -1 = not present, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades
            // XX = Cards not shown " - - " = No hold cards / Folded 

            string strCard;

            strCard = "ERROR!";

            switch (intCard)
            {
                case -1:
                    strCard = " - - ";
                    break;
                case 0:
                    strCard = "XX";
                    break;
                case 1:
                    strCard = "AC";
                    break;
                case 2:
                    strCard = "2C";
                    break;
                case 3:
                    strCard = "3C";
                    break;
                case 4:
                    strCard = "4C";
                    break;
                case 5:
                    strCard = "5C";
                    break;
                case 6:
                    strCard = "6C";
                    break;
                case 7:
                    strCard = "7C";
                    break;
                case 8:
                    strCard = "8C";
                    break;
                case 9:
                    strCard = "9C";
                    break;
                case 10:
                    strCard = "10C";
                    break;
                case 11:
                    strCard = "JC";
                    break;
                case 12:
                    strCard = "QC";
                    break;
                case 13:
                    strCard = "KC";
                    break;
                case 14:
                    strCard = "AD";
                    break;
                case 15:
                    strCard = "2D";
                    break;
                case 16:
                    strCard = "3D";
                    break;
                case 17:
                    strCard = "4D";
                    break;
                case 18:
                    strCard = "5D";
                    break;
                case 19:
                    strCard = "6D";
                    break;
                case 20:
                    strCard = "7D";
                    break;
                case 21:
                    strCard = "8D";
                    break;
                case 22:
                    strCard = "9D";
                    break;
                case 23:
                    strCard = "10D";
                    break;
                case 24:
                    strCard = "JD";
                    break;
                case 25:
                    strCard = "QD";
                    break;
                case 26:
                    strCard = "KD";
                    break;
                case 27:
                    strCard = "AH";
                    break;
                case 28:
                    strCard = "2H";
                    break;
                case 29:
                    strCard = "3H";
                    break;
                case 30:
                    strCard = "4H";
                    break;
                case 31:
                    strCard = "5H";
                    break;
                case 32:
                    strCard = "6H";
                    break;
                case 33:
                    strCard = "7H";
                    break;
                case 34:
                    strCard = "8H";
                    break;
                case 35:
                    strCard = "9H";
                    break;
                case 36:
                    strCard = "10H";
                    break;
                case 37:
                    strCard = "JH";
                    break;
                case 38:
                    strCard = "QH";
                    break;
                case 39:
                    strCard = "KH";
                    break;
                case 40:
                    strCard = "AS";
                    break;
                case 41:
                    strCard = "2S";
                    break;
                case 42:
                    strCard = "3S";
                    break;
                case 43:
                    strCard = "4S";
                    break;
                case 44:
                    strCard = "5S";
                    break;
                case 45:
                    strCard = "6S";
                    break;
                case 46:
                    strCard = "7S";
                    break;
                case 47:
                    strCard = "8S";
                    break;
                case 48:
                    strCard = "9S";
                    break;
                case 49:
                    strCard = "10S";
                    break;
                case 50:
                    strCard = "JS";
                    break;
                case 51:
                    strCard = "QS";
                    break;
                case 52:
                    strCard = "KS";
                    break;
            }

            return strCard;
        }

        public int CardSuitAndStringToInt(int intSuit, string strCard)
        {
            int intCard;

            intCard = -1;

            switch (strCard)
            {
                case "A":
                    intCard = 1;
                    break;
                case "2":
                    intCard = 2;
                    break;
                case "3":
                    intCard = 3;
                    break;
                case "4":
                    intCard = 4;
                    break;
                case "5":
                    intCard = 5;
                    break;
                case "6":
                    intCard = 6;
                    break;
                case "7":
                    intCard = 7;
                    break;
                case "8":
                    intCard = 8;
                    break;
                case "9":
                    intCard = 9;
                    break;
                case "10":
                    intCard = 10;
                    break;
                case "J":
                    intCard = 11;
                    break;
                case "Q":
                    intCard = 12;
                    break;
                case "K":
                    intCard = 13;
                    break;
            }

            if (intSuit > 1)
            {
                // -1 = not present, 1 = clubs, 2 = diamonds, 3 = hearts, 4 = spades
                switch (intSuit)
                {
                    case 2:
                        intCard += 13;
                        break;
                    case 3:
                        intCard += 26;
                        break;
                    case 4:
                        intCard += 39;
                        break;
                }
            }

            return intCard;
        }
    }
}
