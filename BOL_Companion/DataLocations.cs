using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL_Companion
{
    /// <summary>
    /// A class that holds the locations where data, such as chip counts and player names, can be found on the screen.
    /// </summary>
    public static class DataLocations
    {
        // *******************************************************************************************************************

        // Locations Array breakdown

        // Players
        // Locations[0][*, *] - Locations[9][*, *] 
        // Locations[0 - 9][0, *] = Player info (name and chipstack) (x, y, width, height)
        // Locations[0 - 9][1, -] = Hold Card 1 (x, y, width, height)
        // Locations[0 - 9][2, -] = Hold Card 2 (x, y, width, height)
        // Locations[0 - 9][3, -] = Action bar and dealer chip locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

        // Board Cards
        // Locations[10][*, *] - Locations[14][*, *] 
        // Locations[10 - 14][0, *] = Board card (x, y, width, height)

        // Pot
        // Locations[15][*, *]
        // Locations[15][0, *] = Pot (x, y, width, height)

        // Action bar and dealer chip area (the screen area that contains every player's action bar and dealer chip location)
        // Locations[16][*, *]
        // Locations[15][0, *] = Action bar and dealer chip area (x, y, width, height) 
        
        // *******************************************************************************************************************

        /// <summary>
        /// The array that contains the locations where data, such as chip counts and player names, can be found on the screen
        /// </summary>
        public static int[][,] Locations { get; private set; }

        /// <summary>
        /// The width of a playing card
        /// </summary>
        public static int CardWidth { get; } = 32;

        /// <summary>
        /// The height of a playing card
        /// </summary>
        public static int CardHeight { get; } = 50;

        /// <summary>
        /// The x distance from the player's information area to the player's first hold card
        /// </summary>
        public static int Hc1Dx { get; } = 57;

        /// <summary>
        /// The x distance from the player's information area to the player's second hold card
        /// </summary>
        public static int Hc2Dx { get; } = 99;

        /// <summary>
        /// The y distance from the player's information area to the player's hold cards
        /// </summary>
        public static int HcDy { get; } = 6;

        /// <summary>
        /// The y distance from the player's information area to the location of the player's name and chipstack
        /// </summary>
        public static int DyPlayerNameAndChipstack { get; } = 106;

        /// <summary>
        /// The x coordinate to check for the white edge of a facedown hold card
        /// </summary>
        public static int HcStatusDx1 { get; } = 59;

        /// <summary>
        /// The x coordinate to check for the white of a card suit symbol
        /// </summary>
        public static int HcStatusDx2 { get; } = 73;

        /// <summary>
        /// The y coordinate to check for the white edge of a facedown hold card
        /// </summary>
        public static int HcStatusDy1 { get; } = 89;

        /// <summary>
        /// The y coordinate to check for the white of a card suit symbol
        /// </summary>
        public static int HcStatusDy2 { get; } = 65;

        /// <summary>
        /// Initialize the array that contains the locations where data, such as chip counts and player names, can be found on the screen.
        /// </summary>
        public static void InitializeDataLocations()
        {
            // Player information area width
            int playerWidth = 258;
            // Player information area height
            int playerHeight = 171;
            // The x distance from the player's information area to the player's action bar 
            int actionBarDx = -4;
            // The y distance from the player's information area to the player's action bar 
            int actionBarDy = 192;

            #region Locations array defining dimensions

            Locations = new int[17][,];

            PlayerLocations9Players();

            // Boar Card 1 (Flop 1) location: x, y
            Locations[10] = new int[1, 4]
            {
                {673, 348, 0, 0}
            };

            // Boar Card 2 (Flop 2) location: x, y
            Locations[11] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 3 (Flop 3) location: x, y
            Locations[12] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 4 (Turn) location: x, y
            Locations[13] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Boar Card 5 (River) location: x, y
            Locations[14] = new int[1, 4]
            {
                {0, 348, 0, 0}
            };

            // Pot location: x, y
            Locations[15] = new int[1, 4]
            {
                {899, 269, 0, 0}
            };

            // Action bar and dealer chip area (the screen area that contains every player's action bar and dealer chip location): x, y, width, height
            Locations[16] = new int[1, 4]
            {
                {251, 41, 1215, 763}
            };

            #endregion

            #region Add in constants

            // For player locations
            for (int i = 0; i < 10; i++)
            {
                Locations[i][0, 2] = playerWidth;
                Locations[i][0, 3] = playerHeight;
                Locations[i][1, 0] = Locations[i][0, 0] + Hc1Dx;
                Locations[i][1, 1] = Locations[i][0, 1] + HcDy;
                Locations[i][1, 2] = CardWidth;
                Locations[i][1, 3] = CardHeight;
                Locations[i][2, 0] = Locations[i][0, 0] + Hc2Dx;
                Locations[i][2, 1] = Locations[i][0, 1] + HcDy;
                Locations[i][2, 2] = CardWidth;
                Locations[i][2, 3] = CardHeight;
                Locations[i][3, 0] = Locations[i][0, 0] + actionBarDx;
                Locations[i][3, 1] = Locations[i][0, 1] + actionBarDy;
            }

            // The x distance between board cards
            int boardDx = 117;

            // For board card locations
            for (int i = 10; i < 15; i++)
            {
                if (i > 10)
                {
                    Locations[i][0, 0] = Locations[i - 1][0, 0] + boardDx;
                }

                Locations[i][0, 2] = CardWidth;
                Locations[i][0, 3] = CardHeight;
            }

            // The pot display area width
            int potWidth = 123;
            // The pot display area height
            int potHeight = 19;

            // For Pot Annoucement Location
            Locations[15][0, 2] = potWidth;
            Locations[15][0, 3] = potHeight;

            #endregion

        }

        /// <summary>
        /// Set the values in the Loactions array for the player info and dealer chip locations for a 9 player table (array elements 0 - 8).
        /// </summary>
        private static void PlayerLocations9Players()
        {
            // Locations[X][0, -] = Player info (name and chipstack) (x, y, width, height)
            // Locations[X][1, -] = Hold Card 1 (x, y, width, height)
            // Locations[X][2, -] = Hold Card 2 (x, y, width, height)
            // Locations[X][3, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

            // Player 1:
            Locations[0] = new int[4, 4]
            {
                {288, 161, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 580, 298}
            };

            // Player 2: 
            Locations[1] = new int[4, 4]
            {
                {621, 31, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 669, 248}
            };

            // Player 3: 
            Locations[2] = new int[4, 4]
            {
                {1037, 31, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1251, 248}
            };

            // Player 4: 
            Locations[3] = new int[4, 4]
            {
                {1370, 161, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1339, 298}
            };

            // Player 5:
            Locations[4] = new int[4, 4]
            {
                {1403, 369, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1411, 450}
            };

            // Player 6:
            Locations[5] = new int[4, 4]
            {
                {1163, 569, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1133, 676}
            };

            // Player 7:
            Locations[6] = new int[4, 4]
            {
                {829, 611, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 837, 685}
            };

            // Player 8: 
            Locations[7] = new int[4, 4]
            {
                {495, 569, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 751, 646}
            };

            // Player 9: 
            Locations[8] = new int[4, 4]
            {
                {255, 369, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 509, 450}
            };

            // Player 10:
            // This player doesn't exist at a 9 player table
            Locations[9] = new int[4, 4]
            {
                {289, 162, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 581, 299}
            };
        }

        /// <summary>
        /// Set the values in the Loactions array for the player info and dealer chip locations for a 10 player table (array elements 0 - 9).
        /// </summary>
        private static void PlayerLocations10Players()
        {
            // Locations[X][0, -] = Player info (name and chipstack) (x, y, width, height)
            // Locations[X][1, -] = Hold Card 1 (x, y, width, height)
            // Locations[X][2, -] = Hold Card 2 (x, y, width, height)
            // Locations[X][3, -] = action bar and dealer chip check locations (x_Action, y_Action, x_DealerChip, y_DealerChip)

            // Player 1:
            Locations[0] = new int[4, 4]
            {
                {286, 255, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 563, 413}
            };

            // Player 2: 
            Locations[1] = new int[4, 4]
            {
                {525, 104, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 802, 245}
            };

            // Player 3: 
            Locations[2] = new int[4, 4]
            {
                {837, 63, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1081, 262}
            };

            // Player 4: 
            Locations[3] = new int[4, 4]
            {
                {1149, 104, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1303, 298}
            };

            // Player 5: 
            Locations[4] = new int[4, 4]
            {
                {1387, 255, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1356, 414}
            };

            // Player 6: 
            Locations[5] = new int[4, 4]
            {
                {1387, 447, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1358, 552}
            };

            // Player 7: 
            Locations[6] = new int[4, 4]
            {
                {1149, 589, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 1118, 670}
            };

            // Player 8: 
            Locations[7] = new int[4, 4]
            {
                {837, 625, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 839, 672}
            };

            // Player 9: 
            Locations[8] = new int[4, 4]
            {
                {555, 589, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 766, 656}
            };

            // Player 10:
            Locations[9] = new int[4, 4]
            {
                {555, 447, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
                {0, 0, 560, 552}
            };
        }
    }
}
