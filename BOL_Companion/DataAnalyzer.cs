using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BOL_Companion.DbController;

namespace BOL_Companion
{
    class DataAnalyzer
    {
        public DataAnalyzer()
        {

        }

        #region Public Methods

        public DataCounter AnalyzePlayerData(List<DbPlayerHandInfoAll> lstAllPlayerData, int intCurrGameId, int intCurrNumPlayers, long lngPlayerofInterestHandPlayerId)
        {
            //Pre-Flop
            bool blnBlindExists, blnFirstFlopAction, blnFirstTurnAction, blnFirstRiverAction, blnPreFlopCheck, blnPreFlopBetMade, 
                blnFlopBetMade, blnTurnBetMade, blnRiverBetMade, blnFolded;
            int intCurrPlayerIndex, intFlopActionNum, intTurnActionNum, intRiverActionNum, intFirstPlayerToActIndex;
            int[] intChipsInPot, intPlayerChipStackStart, intBlindPlayerIndices;
            DataCounter cnt;

            // intChipsInPot is how many chips each player has in the pot (assuming 10 players per table max)
            intChipsInPot = new int[10];
            // intPlayerChipStackStart is how many chips each player has at the beginning of the hand (assuming 10 players per table max)
            intPlayerChipStackStart = new int[10];
            // intBlindPlayerIndicies are the indicies of the two blind players
            intBlindPlayerIndices = new int[2];

            cnt = new DataCounter();

            // Zero out this array. I added this loop after this method was already working but after reviewing i decided to add it just in case.
            for (int i = 0; i < intChipsInPot.Length; i++)
            {
                intChipsInPot[i] = 0;
                intPlayerChipStackStart[i] = 0;
            }

            // Loop for each hand 
            // [DbPlayerHandInfoAll represents all the info from one hand. This List<DbPlayerHandInfoAll> represents all the hands that 
            // player has been involved in] 
            foreach (DbPlayerHandInfoAll hi in lstAllPlayerData)
            {
                blnBlindExists = false;

                // Check to make sure at least one player posted a blind this hand. 
                // If no blinds this is an incomplete hand -> ignore it (we don't have the data starting from the beginning of the hand)
                foreach (DbHandPlayerInfo hp in hi.lstHandPlayerInfo)
                {
                    if (hp.intBlind > 0)
                    {
                        blnBlindExists = true;
                        break;
                    }
                }

                // A blind player exists (we have the full hand data available in the DB) -> move foward with getting data 
                if (blnBlindExists)
                {
                    intFlopActionNum = 100000;
                    intTurnActionNum = 100000;
                    intRiverActionNum = 100000;

                    // Find intFlopActionNum, intTurnActionNum and intRiverActionNum to determine when the pre-flop, 
                    // flop, turn and river actions
                    for (int i = 0; i < hi.lstBoardActionInfo.Count; i++)
                    {
                        if (i == 0)
                        {
                            intFlopActionNum = hi.lstBoardActionInfo[i].intHandActionNumber;
                        }
                        else if (i == 3)
                        {
                            intTurnActionNum = hi.lstBoardActionInfo[i].intHandActionNumber;
                        }
                        else if (i == 4)
                        {
                            intRiverActionNum = hi.lstBoardActionInfo[i].intHandActionNumber;
                        }
                    }

                    blnFolded = false;
                    blnPreFlopBetMade = false;
                    blnFlopBetMade = false;
                    blnTurnBetMade = false;
                    blnRiverBetMade = false;
                    intCurrPlayerIndex = -1;
                    intBlindPlayerIndices[0] = -1;
                    intBlindPlayerIndices[1] = -1;

                    // Initialize the intChipsInPot array
                    // Note: I am only initializing the intChipsInPot array elements for the players who are actually in the hand
                    // Also find the player of interest's starting chip stack.
                    // Also find the indices of the blind players (i will use that info later to determine the first player to act)
                    for (int i = 0; i < hi.lstHandPlayerInfo.Count; i++)
                    {
                        intChipsInPot[i] = hi.lstHandPlayerInfo[i].intBlind + hi.HandInfo.intAnte;
                        intPlayerChipStackStart[i] = hi.lstHandPlayerInfo[i].intChipCountStart;

                        if (hi.HandInfo.lngHandPlayerId == hi.lstHandPlayerInfo[i].lngHandPlayerId)
                        {
                            intCurrPlayerIndex = i;
                        }

                        if (hi.lstHandPlayerInfo[i].intBlind > 0)
                        {
                            if (intBlindPlayerIndices[0] == -1) 
                            {
                                intBlindPlayerIndices[0] = i;
                            }
                            else
                            {
                                intBlindPlayerIndices[1] = i;
                            }
                        }
                    }

                    // Check to make sure this hand has a winner (i.e. the hand ended with someone gaining chips which is signified by a 
                    // negative intChipCountChange number). First I will check to make sure hi.lstPlayerActionInfo.Count > 0 to avoid an error
                    // in the if statement here. If hi.lstPlayerActionInfo.Count == 0 no need to worry about the code below because it means
                    // this is pre-flop and the player of interest is the first player to act so nothing has happened so far this hand.
                    if (hi.lstPlayerActionInfo.Count > 0 && hi.lstPlayerActionInfo[hi.lstPlayerActionInfo.Count - 1].intChipCountChange >= 0)
                    {
                        // Check to make sure this is the current hand

                        for (int i = 0; i < hi.lstHandPlayerInfo.Count; i++)
                        {
                            // check to see if this is the hand currently in progress (use lngPlayerofInterestHandPlayerId which is the player
                            // of interest's HandPlayerId for the hand in progress)
                            if (hi.lstHandPlayerInfo[i].lngHandPlayerId == lngPlayerofInterestHandPlayerId)
                            {
                                // This is the hand currently in progress and the hand has not been completed

                                int intNextHandActionNumber = 0;
                                int intLargeBet = 1000000000;

                                // Find the correct next hand action number. If the last action was a player action the next hand action number
                                // will be lstPlayerActionInfo.Count. However, if the last action was another board card being shown then the
                                // next hand action number is either intFlopActionNumber + 1, intTurnActionNumber + 1 or intRiverActionNumber + 1

                                // First find latest board action
                                if (intRiverActionNum < 100000)
                                {
                                    intNextHandActionNumber = intRiverActionNum + 1;
                                }
                                else if (intTurnActionNum < 100000)
                                {
                                    intNextHandActionNumber = intTurnActionNum + 1;
                                }
                                else if (intFlopActionNum < 100000)
                                {
                                    intNextHandActionNumber = intFlopActionNum + 1;
                                }

                                // Now see if the latest hand action was more recent
                                if (hi.lstPlayerActionInfo.Count > intNextHandActionNumber)
                                {
                                    intNextHandActionNumber = hi.lstPlayerActionInfo.Count;
                                }

                                // It is the player of interest's (lngPlayerOfInterestHandPlayerId) turn in the hand currently in progress.
                                // Add a large bet HandPlayerAction to the current DbHandPlayerInfoAll for the player of interest so that
                                // players who checked before this player will have there actions counted.
                                hi.lstPlayerActionInfo.Add(new DbPlayerActionInfo
                                {
                                    lngHandPlayerId = lngPlayerofInterestHandPlayerId,
                                    intChipCountChange = intLargeBet,
                                    intHandActionNumber = intNextHandActionNumber
                                });

                                // Add the same large bet to the player's initial chip count to prevent issues that might arise with pre-flop
                                // call/bet minimums and all-in issues in the code
                                hi.lstHandPlayerInfo[i].intChipCountStart += intLargeBet;

                                // If the current player (hi.HandInfo.lngHandPlayerId) is the player of interest 
                                // (lngPlayerOfInterestHandPlayerId) subtract 1 fold from his DataCounter structure
                                if (hi.HandInfo.lngHandPlayerId == lngPlayerofInterestHandPlayerId)
                                {
                                    // Find out if this is a pre-flop, flop, turn or river fold
                                    if (intRiverActionNum < 100000)
                                    {
                                        // We already know this is the current hand in progress so it is "This Table" and it is also
                                        // "Curr Number of Players"
                                        cnt.cirRiver.actAnyTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirRiver.actThisTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirRiver.actAnyTableCurrNumPlayers.lngBet -= 1;
                                        cnt.cirRiver.actThisTableCurrNumPlayers.lngBet -= 1;
                                    }
                                    else if (intTurnActionNum < 100000)
                                    {
                                        // We already know this is the current hand in progress so it is "This Table" and it is also
                                        // "Curr Number of Players"
                                        cnt.cirTurn.actAnyTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirTurn.actThisTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirTurn.actAnyTableCurrNumPlayers.lngBet -= 1;
                                        cnt.cirTurn.actThisTableCurrNumPlayers.lngBet -= 1;
                                    }
                                    else if (intFlopActionNum < 100000)
                                    {
                                        // We already know this is the current hand in progress so it is "This Table" and it is also
                                        // "Curr Number of Players"
                                        cnt.cirFlop.actAnyTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirFlop.actThisTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirFlop.actAnyTableCurrNumPlayers.lngBet -= 1;
                                        cnt.cirFlop.actThisTableCurrNumPlayers.lngBet -= 1;
                                    }
                                    else
                                    {
                                        // We already know this is the current hand in progress so it is "This Table" and it is also
                                        // "Curr Number of Players"
                                        cnt.cirPreFlop.actAnyTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirPreFlop.actThisTableAnyNumPlayers.lngBet -= 1;
                                        cnt.cirPreFlop.actAnyTableCurrNumPlayers.lngBet -= 1;
                                        cnt.cirPreFlop.actThisTableCurrNumPlayers.lngBet -= 1;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    // Figure out who is the first player to act (the first player to post a blind)
                    if (intBlindPlayerIndices[1] == -1)
                    {
                        intFirstPlayerToActIndex = intBlindPlayerIndices[0];
                    }
                    else
                    {
                        // if there are more than two players left
                        if (hi.lstHandPlayerInfo.Count > 2)
                        {
                            // The first sequential player is always the first to act unless the first and the last player in the
                            // sequence were the two blinds. In this case the last sequential player was the first to act.
                            if (!(intBlindPlayerIndices[0] == 0 && intBlindPlayerIndices[1] == hi.lstHandPlayerInfo.Count - 1))
                            {
                                intFirstPlayerToActIndex = intBlindPlayerIndices[0];
                            }
                            else
                            {
                                intFirstPlayerToActIndex = intBlindPlayerIndices[1];
                            }
                        }
                        // only two players are left
                        else
                        {
                            // Here i will assume the player who posted the bigger blind is the dealer -> the other player is the first to act.
                            // This may not be true under the special circumstance that the dealer is all in and has less chips than the small
                            // blind. This is very unlikely but possible. 
                            if (hi.lstHandPlayerInfo[0].intBlind > hi.lstHandPlayerInfo[1].intBlind)
                            {
                                intFirstPlayerToActIndex = 1;
                            }
                            else
                            {
                                intFirstPlayerToActIndex = 0;
                            }
                        }
                    }

                    blnPreFlopCheck = true;
                    blnFirstFlopAction = true;
                    blnFirstTurnAction = true;
                    blnFirstRiverAction = true;

                    // Go through each player action in this hand but only log actions for plotting if 
                    // hi.HandInfo.lngHandPlayerId == hi.lstPlayerActionInfo[i].lngHandPlayerId
                    // else just keep track of how many chips each player has in the pot
                    for (int i = 0; i < hi.lstPlayerActionInfo.Count; i++)
                    {
                        if (!blnFolded)
                        {
                            // Pre-Flop action
                            if (hi.lstPlayerActionInfo[i].intHandActionNumber < intFlopActionNum)
                            {
                                if (ProcessPlayerAciton(hi, i, intChipsInPot, intCurrGameId, intCurrNumPlayers, cnt.cirPreFlop,
                                    intFirstPlayerToActIndex, intCurrPlayerIndex, intPlayerChipStackStart[intCurrPlayerIndex], ref blnPreFlopBetMade, ref blnFolded, true))
                                {
                                    blnPreFlopCheck = false;
                                }
                            }

                            // Flop action
                            else if (hi.lstPlayerActionInfo[i].intHandActionNumber < intTurnActionNum)
                            {
                                if (blnFirstFlopAction)
                                {
                                    if (blnPreFlopCheck && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirPreFlop);
                                        blnPreFlopCheck = false;
                                    }
                                    blnFirstFlopAction = false;
                                }
                                ProcessPlayerAciton(hi, i, intChipsInPot, intCurrGameId, intCurrNumPlayers, cnt.cirFlop, intFirstPlayerToActIndex,
                                    intCurrPlayerIndex, intPlayerChipStackStart[intCurrPlayerIndex], ref blnFlopBetMade, ref blnFolded, false);
                            }

                            // Turn action
                            else if (hi.lstPlayerActionInfo[i].intHandActionNumber < intRiverActionNum)
                            {
                                if (blnFirstTurnAction)
                                {
                                    // Check to see if player checked pre-flop (and no bets were made on the flop)
                                    if (blnPreFlopCheck && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirPreFlop);
                                        blnPreFlopCheck = false;
                                    }

                                    if (!blnFlopBetMade && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirFlop);
                                    }
                                    blnFirstTurnAction = false;
                                }

                                ProcessPlayerAciton(hi, i, intChipsInPot, intCurrGameId, intCurrNumPlayers, cnt.cirTurn, intFirstPlayerToActIndex,
                                    intCurrPlayerIndex, intPlayerChipStackStart[intCurrPlayerIndex], ref blnTurnBetMade, ref blnFolded, false);
                            }

                            // River action
                            else
                            {
                                if (blnFirstRiverAction)
                                {
                                    // Check to see if player checked pre-flop (and no bets were made on the flop or the turn)
                                    if (blnPreFlopCheck && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirPreFlop);
                                        blnPreFlopCheck = false;
                                    }

                                    if (!blnTurnBetMade && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirTurn);
                                    }
                                    blnFirstRiverAction = false;

                                }

                                // Check to see if player checked pre-flop (and no bets were made on the flop, the turn or the river)
                                if (blnPreFlopCheck && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                {
                                    // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                    PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                        intChipsInPot[intCurrPlayerIndex], cnt.cirPreFlop);
                                    blnPreFlopCheck = false;
                                }

                                // This (blnFirstTurnAction) check is needed in case the flop and the turn were checked by all players. We have to go back and
                                // and register the players' check calls on the turn.
                                if (blnFirstTurnAction)
                                {
                                    if (!blnFlopBetMade && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirFlop);
                                    }
                                    blnFirstTurnAction = false;
                                }

                                // Last player action of this hand (also note that this player action is occuring on the river)
                                if (i == hi.lstPlayerActionInfo.Count - 1)
                                {
                                    if (!blnRiverBetMade && !RemainingPlayersAllIn(intChipsInPot, intPlayerChipStackStart, intCurrPlayerIndex, hi.lstHandPlayerInfo.Count))
                                    {
                                        // Checking to see if the player is already all-in is taken care of within PlayerCheckAction
                                        PlayerCheckAction(hi, intCurrGameId, intCurrNumPlayers, intPlayerChipStackStart[intCurrPlayerIndex],
                                            intChipsInPot[intCurrPlayerIndex], cnt.cirRiver);
                                    }
                                }

                                ProcessPlayerAciton(hi, i, intChipsInPot, intCurrGameId, intCurrNumPlayers, cnt.cirRiver, intFirstPlayerToActIndex,
                                    intCurrPlayerIndex, intPlayerChipStackStart[intCurrPlayerIndex], ref blnRiverBetMade, ref blnFolded, false);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return cnt;
        }

        /// <summary>
        /// Plot the data contained in the DataCounter class on the specified chart set
        /// </summary>
        /// <param name="intChartSetIndex">The index of the set of 4 charts representing the data for one player (an integer 0-9)</param>
        /// <param name="cntPlayerData_">All the player's relevent data</param>
        /// <param name="frm"></param>
        public void PlotPlayerDataPreFlop(int intChartSetIndex, DataCounter cntPlayerData_, frmDataDisplay frm)
        {
            long lngSamplecount;

            // This Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers, 0),
                    cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers, 1),
                    cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers, 2),
                    cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers, 3),
                    cntPlayerData_.cirPreFlop.actThisTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][0].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 0, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][0].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][0] = true;
            //frm.chtQuad[intChartSetIndex][0].Invalidate();

            // This Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers, 0),
                    cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers, 1),
                    cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers, 2),
                    cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers, 3),
                    cntPlayerData_.cirPreFlop.actThisTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][1].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 1, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][1].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][1] = true;
            //frm.chtQuad[intChartSetIndex][1].Invalidate();

            // Any Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers, 0),
                    cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers, 1),
                    cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers, 2),
                    cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers, 3),
                    cntPlayerData_.cirPreFlop.actAnyTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][2].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 2, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][2].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][2] = true;
            //frm.chtQuad[intChartSetIndex][2].Invalidate();

            // Any Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers, 0),
                    cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers, 1),
                    cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers, 2),
                    cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers, 3),
                    cntPlayerData_.cirPreFlop.actAnyTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][3].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 3, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][3].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][3] = true;
            //frm.chtQuad[intChartSetIndex][3].Invalidate();
        }

        /// <summary>
        /// Plot the data contained in the DataCounter class on the specified chart set
        /// </summary>
        /// <param name="intChartSetIndex">The index of the set of 4 charts representing the data for one player (an integer 0-9)</param>
        /// <param name="cntPlayerData_">All the player's relevent data</param>
        /// <param name="frm"></param>
        public void PlotPlayerDataFlop(int intChartSetIndex, DataCounter cntPlayerData_, frmDataDisplay frm)
        {
            long lngSamplecount;

            // This Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirFlop.actThisTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableCurrNumPlayers, 0),
                    cntPlayerData_.cirFlop.actThisTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableCurrNumPlayers, 1),
                    cntPlayerData_.cirFlop.actThisTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableCurrNumPlayers, 2),
                    cntPlayerData_.cirFlop.actThisTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableCurrNumPlayers, 3),
                    cntPlayerData_.cirFlop.actThisTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][0].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 0, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][0].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][0] = true;
            //frm.chtQuad[intChartSetIndex][0].Invalidate();

            // This Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirFlop.actThisTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableAnyNumPlayers, 0),
                    cntPlayerData_.cirFlop.actThisTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableAnyNumPlayers, 1),
                    cntPlayerData_.cirFlop.actThisTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableAnyNumPlayers, 2),
                    cntPlayerData_.cirFlop.actThisTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actThisTableAnyNumPlayers, 3),
                    cntPlayerData_.cirFlop.actThisTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][1].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 1, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][1].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][1] = true;
            //frm.chtQuad[intChartSetIndex][1].Invalidate();

            // Any Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers, 0),
                    cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers, 1),
                    cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers, 2),
                    cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers, 3),
                    cntPlayerData_.cirFlop.actAnyTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][2].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 2, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][2].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][2] = true;
            //frm.chtQuad[intChartSetIndex][2].Invalidate();

            // Any Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers, 0),
                    cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers, 1),
                    cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers, 2),
                    cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers, 3),
                    cntPlayerData_.cirFlop.actAnyTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][3].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 3, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][3].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][3] = true;
            //frm.chtQuad[intChartSetIndex][3].Invalidate();
        }

        /// <summary>
        /// Plot the data contained in the DataCounter class on the specified chart set
        /// </summary>
        /// <param name="intChartSetIndex">The index of the set of 4 charts representing the data for one player (an integer 0-9)</param>
        /// <param name="cntPlayerData_">All the player's relevent data</param>
        /// <param name="frm"></param>
        public void PlotPlayerDataTurn(int intChartSetIndex, DataCounter cntPlayerData_, frmDataDisplay frm)
        {
            long lngSamplecount;

            // This Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirTurn.actThisTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableCurrNumPlayers, 0),
                    cntPlayerData_.cirTurn.actThisTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableCurrNumPlayers, 1),
                    cntPlayerData_.cirTurn.actThisTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableCurrNumPlayers, 2),
                    cntPlayerData_.cirTurn.actThisTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableCurrNumPlayers, 3),
                    cntPlayerData_.cirTurn.actThisTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][0].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 0, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][0].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][0] = true;
            //frm.chtQuad[intChartSetIndex][0].Invalidate();

            // This Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirTurn.actThisTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableAnyNumPlayers, 0),
                    cntPlayerData_.cirTurn.actThisTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableAnyNumPlayers, 1),
                    cntPlayerData_.cirTurn.actThisTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableAnyNumPlayers, 2),
                    cntPlayerData_.cirTurn.actThisTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actThisTableAnyNumPlayers, 3),
                    cntPlayerData_.cirTurn.actThisTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][1].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 1, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][1].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][1] = true;
            //frm.chtQuad[intChartSetIndex][1].Invalidate();

            // Any Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers, 0),
                    cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers, 1),
                    cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers, 2),
                    cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers, 3),
                    cntPlayerData_.cirTurn.actAnyTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][2].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 2, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][2].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][2] = true;
            //frm.chtQuad[intChartSetIndex][2].Invalidate();

            // Any Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers, 0),
                    cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers, 1),
                    cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers, 2),
                    cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers, 3),
                    cntPlayerData_.cirTurn.actAnyTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][3].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 3, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][3].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][3] = true;
            //frm.chtQuad[intChartSetIndex][3].Invalidate();
        }

        /// <summary>
        /// Plot the data contained in the DataCounter class on the specified chart set
        /// </summary>
        /// <param name="intChartSetIndex">The index of the set of 4 charts representing the data for one player (an integer 0-9)</param>
        /// <param name="cntPlayerData_">All the player's relevent data</param>
        /// <param name="frm"></param>
        public void PlotPlayerDataRiver(int intChartSetIndex, DataCounter cntPlayerData_, frmDataDisplay frm)
        {
            long lngSamplecount;

            // This Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirRiver.actThisTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][0].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableCurrNumPlayers, 0),
                    cntPlayerData_.cirRiver.actThisTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableCurrNumPlayers, 1),
                    cntPlayerData_.cirRiver.actThisTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableCurrNumPlayers, 2),
                    cntPlayerData_.cirRiver.actThisTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][0].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableCurrNumPlayers, 3),
                    cntPlayerData_.cirRiver.actThisTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][0].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 0, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][0].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][0] = true;
            //frm.chtQuad[intChartSetIndex][0].Invalidate();

            // This Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirRiver.actThisTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][1].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableAnyNumPlayers, 0),
                    cntPlayerData_.cirRiver.actThisTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableAnyNumPlayers, 1),
                    cntPlayerData_.cirRiver.actThisTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableAnyNumPlayers, 2),
                    cntPlayerData_.cirRiver.actThisTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][1].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actThisTableAnyNumPlayers, 3),
                    cntPlayerData_.cirRiver.actThisTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][1].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 1, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][1].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][1] = true;
            //frm.chtQuad[intChartSetIndex][1].Invalidate();

            // Any Table - Current Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][2].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers, 0),
                    cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers, 1),
                    cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers, 2),
                    cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][2].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers, 3),
                    cntPlayerData_.cirRiver.actAnyTableCurrNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][2].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 2, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][2].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][2] = true;
            //frm.chtQuad[intChartSetIndex][2].Invalidate();

            // Any Table - Any Num. Players Chart
            lngSamplecount = DataSampleCount(cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers);
            if (lngSamplecount > 0)
            {
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].Color = Color.SpringGreen;

                frm.chtQuad[intChartSetIndex][3].Series[0].Points[0].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers, 0),
                    cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers.lngFold);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[1].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers, 1),
                    cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers.lngCheck);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[2].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers, 2),
                    cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers.lngCall);
                frm.chtQuad[intChartSetIndex][3].Series[0].Points[3].SetValueXY(GetPiePercentage(cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers, 3),
                    cntPlayerData_.cirRiver.actAnyTableAnyNumPlayers.lngBet);

                frm.lblChartNoData[intChartSetIndex][3].Visible = false;
            }
            else
            {
                NoDataChart(intChartSetIndex, 3, frm);
            }
            frm.lblChartSampleCount[intChartSetIndex][3].Text = lngSamplecount.ToString();

            frm.blnChtUpdated[intChartSetIndex][3] = true;
            //frm.chtQuad[intChartSetIndex][3].Invalidate();
        }

        public DataCounter SumPlayerData(DataCounter[] cntData, int intIndexToSkip)
        {
            DataCounter cntSumedData = new DataCounter();

            for (int i = 0; i < 9; i++)
            {
                if (i != intIndexToSkip)
                {
                    // PreFlop
                    cntSumedData.cirPreFlop.actThisTableCurrNumPlayers.lngFold += cntData[i].cirPreFlop.actThisTableCurrNumPlayers.lngFold;
                    cntSumedData.cirPreFlop.actThisTableCurrNumPlayers.lngCheck += cntData[i].cirPreFlop.actThisTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirPreFlop.actThisTableCurrNumPlayers.lngCall += cntData[i].cirPreFlop.actThisTableCurrNumPlayers.lngCall;
                    cntSumedData.cirPreFlop.actThisTableCurrNumPlayers.lngBet += cntData[i].cirPreFlop.actThisTableCurrNumPlayers.lngBet;

                    cntSumedData.cirPreFlop.actThisTableAnyNumPlayers.lngFold += cntData[i].cirPreFlop.actThisTableAnyNumPlayers.lngFold;
                    cntSumedData.cirPreFlop.actThisTableAnyNumPlayers.lngCheck += cntData[i].cirPreFlop.actThisTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirPreFlop.actThisTableAnyNumPlayers.lngCall += cntData[i].cirPreFlop.actThisTableAnyNumPlayers.lngCall;
                    cntSumedData.cirPreFlop.actThisTableAnyNumPlayers.lngBet += cntData[i].cirPreFlop.actThisTableAnyNumPlayers.lngBet;

                    cntSumedData.cirPreFlop.actAnyTableCurrNumPlayers.lngFold += cntData[i].cirPreFlop.actAnyTableCurrNumPlayers.lngFold;
                    cntSumedData.cirPreFlop.actAnyTableCurrNumPlayers.lngCheck += cntData[i].cirPreFlop.actAnyTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirPreFlop.actAnyTableCurrNumPlayers.lngCall += cntData[i].cirPreFlop.actAnyTableCurrNumPlayers.lngCall;
                    cntSumedData.cirPreFlop.actAnyTableCurrNumPlayers.lngBet += cntData[i].cirPreFlop.actAnyTableCurrNumPlayers.lngBet;

                    cntSumedData.cirPreFlop.actAnyTableAnyNumPlayers.lngFold += cntData[i].cirPreFlop.actAnyTableAnyNumPlayers.lngFold;
                    cntSumedData.cirPreFlop.actAnyTableAnyNumPlayers.lngCheck += cntData[i].cirPreFlop.actAnyTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirPreFlop.actAnyTableAnyNumPlayers.lngCall += cntData[i].cirPreFlop.actAnyTableAnyNumPlayers.lngCall;
                    cntSumedData.cirPreFlop.actAnyTableAnyNumPlayers.lngBet += cntData[i].cirPreFlop.actAnyTableAnyNumPlayers.lngBet;

                    // Flop
                    cntSumedData.cirFlop.actThisTableCurrNumPlayers.lngFold += cntData[i].cirFlop.actThisTableCurrNumPlayers.lngFold;
                    cntSumedData.cirFlop.actThisTableCurrNumPlayers.lngCheck += cntData[i].cirFlop.actThisTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirFlop.actThisTableCurrNumPlayers.lngCall += cntData[i].cirFlop.actThisTableCurrNumPlayers.lngCall;
                    cntSumedData.cirFlop.actThisTableCurrNumPlayers.lngBet += cntData[i].cirFlop.actThisTableCurrNumPlayers.lngBet;

                    cntSumedData.cirFlop.actThisTableAnyNumPlayers.lngFold += cntData[i].cirFlop.actThisTableAnyNumPlayers.lngFold;
                    cntSumedData.cirFlop.actThisTableAnyNumPlayers.lngCheck += cntData[i].cirFlop.actThisTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirFlop.actThisTableAnyNumPlayers.lngCall += cntData[i].cirFlop.actThisTableAnyNumPlayers.lngCall;
                    cntSumedData.cirFlop.actThisTableAnyNumPlayers.lngBet += cntData[i].cirFlop.actThisTableAnyNumPlayers.lngBet;

                    cntSumedData.cirFlop.actAnyTableCurrNumPlayers.lngFold += cntData[i].cirFlop.actAnyTableCurrNumPlayers.lngFold;
                    cntSumedData.cirFlop.actAnyTableCurrNumPlayers.lngCheck += cntData[i].cirFlop.actAnyTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirFlop.actAnyTableCurrNumPlayers.lngCall += cntData[i].cirFlop.actAnyTableCurrNumPlayers.lngCall;
                    cntSumedData.cirFlop.actAnyTableCurrNumPlayers.lngBet += cntData[i].cirFlop.actAnyTableCurrNumPlayers.lngBet;

                    cntSumedData.cirFlop.actAnyTableAnyNumPlayers.lngFold += cntData[i].cirFlop.actAnyTableAnyNumPlayers.lngFold;
                    cntSumedData.cirFlop.actAnyTableAnyNumPlayers.lngCheck += cntData[i].cirFlop.actAnyTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirFlop.actAnyTableAnyNumPlayers.lngCall += cntData[i].cirFlop.actAnyTableAnyNumPlayers.lngCall;
                    cntSumedData.cirFlop.actAnyTableAnyNumPlayers.lngBet += cntData[i].cirFlop.actAnyTableAnyNumPlayers.lngBet;

                    // Turn
                    cntSumedData.cirTurn.actThisTableCurrNumPlayers.lngFold += cntData[i].cirTurn.actThisTableCurrNumPlayers.lngFold;
                    cntSumedData.cirTurn.actThisTableCurrNumPlayers.lngCheck += cntData[i].cirTurn.actThisTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirTurn.actThisTableCurrNumPlayers.lngCall += cntData[i].cirTurn.actThisTableCurrNumPlayers.lngCall;
                    cntSumedData.cirTurn.actThisTableCurrNumPlayers.lngBet += cntData[i].cirTurn.actThisTableCurrNumPlayers.lngBet;

                    cntSumedData.cirTurn.actThisTableAnyNumPlayers.lngFold += cntData[i].cirTurn.actThisTableAnyNumPlayers.lngFold;
                    cntSumedData.cirTurn.actThisTableAnyNumPlayers.lngCheck += cntData[i].cirTurn.actThisTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirTurn.actThisTableAnyNumPlayers.lngCall += cntData[i].cirTurn.actThisTableAnyNumPlayers.lngCall;
                    cntSumedData.cirTurn.actThisTableAnyNumPlayers.lngBet += cntData[i].cirTurn.actThisTableAnyNumPlayers.lngBet;

                    cntSumedData.cirTurn.actAnyTableCurrNumPlayers.lngFold += cntData[i].cirTurn.actAnyTableCurrNumPlayers.lngFold;
                    cntSumedData.cirTurn.actAnyTableCurrNumPlayers.lngCheck += cntData[i].cirTurn.actAnyTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirTurn.actAnyTableCurrNumPlayers.lngCall += cntData[i].cirTurn.actAnyTableCurrNumPlayers.lngCall;
                    cntSumedData.cirTurn.actAnyTableCurrNumPlayers.lngBet += cntData[i].cirTurn.actAnyTableCurrNumPlayers.lngBet;

                    cntSumedData.cirTurn.actAnyTableAnyNumPlayers.lngFold += cntData[i].cirTurn.actAnyTableAnyNumPlayers.lngFold;
                    cntSumedData.cirTurn.actAnyTableAnyNumPlayers.lngCheck += cntData[i].cirTurn.actAnyTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirTurn.actAnyTableAnyNumPlayers.lngCall += cntData[i].cirTurn.actAnyTableAnyNumPlayers.lngCall;
                    cntSumedData.cirTurn.actAnyTableAnyNumPlayers.lngBet += cntData[i].cirTurn.actAnyTableAnyNumPlayers.lngBet;

                    // River
                    cntSumedData.cirRiver.actThisTableCurrNumPlayers.lngFold += cntData[i].cirRiver.actThisTableCurrNumPlayers.lngFold;
                    cntSumedData.cirRiver.actThisTableCurrNumPlayers.lngCheck += cntData[i].cirRiver.actThisTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirRiver.actThisTableCurrNumPlayers.lngCall += cntData[i].cirRiver.actThisTableCurrNumPlayers.lngCall;
                    cntSumedData.cirRiver.actThisTableCurrNumPlayers.lngBet += cntData[i].cirRiver.actThisTableCurrNumPlayers.lngBet;

                    cntSumedData.cirRiver.actThisTableAnyNumPlayers.lngFold += cntData[i].cirRiver.actThisTableAnyNumPlayers.lngFold;
                    cntSumedData.cirRiver.actThisTableAnyNumPlayers.lngCheck += cntData[i].cirRiver.actThisTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirRiver.actThisTableAnyNumPlayers.lngCall += cntData[i].cirRiver.actThisTableAnyNumPlayers.lngCall;
                    cntSumedData.cirRiver.actThisTableAnyNumPlayers.lngBet += cntData[i].cirRiver.actThisTableAnyNumPlayers.lngBet;

                    cntSumedData.cirRiver.actAnyTableCurrNumPlayers.lngFold += cntData[i].cirRiver.actAnyTableCurrNumPlayers.lngFold;
                    cntSumedData.cirRiver.actAnyTableCurrNumPlayers.lngCheck += cntData[i].cirRiver.actAnyTableCurrNumPlayers.lngCheck;
                    cntSumedData.cirRiver.actAnyTableCurrNumPlayers.lngCall += cntData[i].cirRiver.actAnyTableCurrNumPlayers.lngCall;
                    cntSumedData.cirRiver.actAnyTableCurrNumPlayers.lngBet += cntData[i].cirRiver.actAnyTableCurrNumPlayers.lngBet;

                    cntSumedData.cirRiver.actAnyTableAnyNumPlayers.lngFold += cntData[i].cirRiver.actAnyTableAnyNumPlayers.lngFold;
                    cntSumedData.cirRiver.actAnyTableAnyNumPlayers.lngCheck += cntData[i].cirRiver.actAnyTableAnyNumPlayers.lngCheck;
                    cntSumedData.cirRiver.actAnyTableAnyNumPlayers.lngCall += cntData[i].cirRiver.actAnyTableAnyNumPlayers.lngCall;
                    cntSumedData.cirRiver.actAnyTableAnyNumPlayers.lngBet += cntData[i].cirRiver.actAnyTableAnyNumPlayers.lngBet;
                }
            }

            return cntSumedData;
        }

        #endregion

        #region Helper Methods

        private bool ProcessPlayerAciton(DbPlayerHandInfoAll hi_, int intIndex, int[] intChipsInPot_, int intCurrGameId_, 
            int intCurrNumPlayers_, HandCircumstances stg, int intFirstPlayerToActIndex_, int intPlayerIndex_, 
            int intPlayerChipStackStart_, ref bool blnBetMade, ref bool blnFolded_, bool blnPreflop)
        {
            bool blnPlayerAction = false;
            int intOtherPlayerChipsInPot_Prev, intOtherPlayerChipsInPot_Curr, intActionPlayerIndex_;

            intOtherPlayerChipsInPot_Prev = -1;
            intOtherPlayerChipsInPot_Curr = -1;
            intActionPlayerIndex_ = -1;

            // Action made by the player of interest -> log data
            if (hi_.HandInfo.lngHandPlayerId == hi_.lstPlayerActionInfo[intIndex].lngHandPlayerId)
            {
                // Chips put into the pot (call or bet)
                if (hi_.lstPlayerActionInfo[intIndex].intChipCountChange > 0)
                {
                    blnPlayerAction = true;

                    // This is a bet
                    if ((intChipsInPot_[intPlayerIndex_] + hi_.lstPlayerActionInfo[intIndex].intChipCountChange) > 
                        ChipsToCall(intChipsInPot_, hi_.lstHandPlayerInfo.Count))
                    {
                        blnBetMade = true;

                        // This table
                        if (hi_.HandInfo.intGameId == intCurrGameId_)
                        {
                            // Current number of players
                            if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                            {
                                stg.actThisTableCurrNumPlayers.lngBet++;
                                stg.actAnyTableCurrNumPlayers.lngBet++;
                            }
                            stg.actThisTableAnyNumPlayers.lngBet++;
                            stg.actAnyTableAnyNumPlayers.lngBet++;
                        }
                        // Any table
                        else
                        {
                            // Current number of players
                            if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                            {
                                stg.actAnyTableCurrNumPlayers.lngBet++;
                            }
                            stg.actAnyTableAnyNumPlayers.lngBet++;
                        }
                    }
                    // This is a call
                    else
                    {
                        // This table
                        if (hi_.HandInfo.intGameId == intCurrGameId_)
                        {
                            // Current number of players
                            if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                            {
                                stg.actThisTableCurrNumPlayers.lngCall++;
                                stg.actAnyTableCurrNumPlayers.lngCall++;
                            }
                            stg.actThisTableAnyNumPlayers.lngCall++;
                            stg.actAnyTableAnyNumPlayers.lngCall++;
                        }
                        // Any table
                        else
                        {
                            // Current number of players
                            if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                            {
                                stg.actAnyTableCurrNumPlayers.lngCall++;
                            }
                            stg.actAnyTableAnyNumPlayers.lngCall++;
                        }
                    }

                    // Keep track of how many chips this player has in the pot
                    UpdatePlayerChipsInPot(hi_, intChipsInPot_, intIndex, ref intOtherPlayerChipsInPot_Prev, ref intOtherPlayerChipsInPot_Curr);
                }

                // Fold
                else if (hi_.lstPlayerActionInfo[intIndex].intChipCountChange == 0)
                {
                    blnPlayerAction = true;
                    blnFolded_ = true;

                    // This table
                    if (hi_.HandInfo.intGameId == intCurrGameId_)
                    {
                        // Current number of players
                        if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                        {
                            stg.actThisTableCurrNumPlayers.lngFold++;
                            stg.actAnyTableCurrNumPlayers.lngFold++;
                        }
                        stg.actThisTableAnyNumPlayers.lngFold++;
                        stg.actAnyTableAnyNumPlayers.lngFold++;
                    }
                    // Any table
                    else
                    {
                        // Current number of players
                        if (hi_.lstHandPlayerInfo.Count == intCurrNumPlayers_)
                        {
                            stg.actAnyTableCurrNumPlayers.lngFold++;
                        }
                        stg.actAnyTableAnyNumPlayers.lngFold++;
                    }
                }
                // else the player won the pot which we don't need to log here
            }

            // Action made by a player other than the player of interest
            else
            {
                // 1) Keep track of how many chips this player has in the pot
                intActionPlayerIndex_ = UpdatePlayerChipsInPot(hi_, intChipsInPot_, intIndex, ref intOtherPlayerChipsInPot_Prev, 
                    ref intOtherPlayerChipsInPot_Curr);

                // 2) Check to see if the player of interest checked before this player's action
                // 2a) The betting stage is not pre-flop. Pre-flop checks are determined using another method.
                if (!blnPreflop)
                {
                    // 2b) A bet was made
                    if (intOtherPlayerChipsInPot_Curr > intOtherPlayerChipsInPot_Prev)
                    {
                        // 2c) This was the first bet made in this betting round
                        if (!blnBetMade)
                        {
                            // 2d) The player of interest is still in this hand
                            if (!blnFolded_)
                            {
                                // 2e) sequence is correct
                                if ((intFirstPlayerToActIndex_ <= intPlayerIndex_ && intPlayerIndex_ < intActionPlayerIndex_) ||
                                    (intActionPlayerIndex_ < intFirstPlayerToActIndex_ && intFirstPlayerToActIndex_ <= intPlayerIndex_) ||
                                    (intPlayerIndex_ < intActionPlayerIndex_ && intActionPlayerIndex_ < intFirstPlayerToActIndex_))
                                {
                                    // 2f) Call PlayerCheckAction. Note: PlayerCheckAction checks to see if the player is already
                                    //     all in before adding the action to the player's action data counter
                                    PlayerCheckAction(hi_, intCurrGameId_, intCurrNumPlayers_, intPlayerChipStackStart_, intChipsInPot_[intPlayerIndex_], stg);

                                }
                            }
                        }
                        blnBetMade = true;
                    }
                }
            }

            return blnPlayerAction;
        }

        /// <summary>
        /// Adds a "checked" action to the player's action data counter 
        /// </summary>
        /// <param name="hi__"></param>
        /// <param name="intCurrGameId__"></param>
        /// <param name="intCurrNumPlayers__"></param>
        /// <param name="intPlayerChipStackStart__"></param>
        /// <param name="intPlayerChipsInPot"></param>
        /// <param name="stg_"></param>
        private void PlayerCheckAction(DbPlayerHandInfoAll hi__, int intCurrGameId__, int intCurrNumPlayers__, int intPlayerChipStackStart__, 
            int intPlayerChipsInPot, HandCircumstances stg_)
        {
            // The player is only checking if he still has chips available, if not he is already all in
            if (intPlayerChipStackStart__ > intPlayerChipsInPot)
            {
                // The player has checked
                if (hi__.HandInfo.intGameId == intCurrGameId__)
                {
                    // Current number of players
                    if (hi__.lstHandPlayerInfo.Count == intCurrNumPlayers__)
                    {
                        stg_.actThisTableCurrNumPlayers.lngCheck++;
                        stg_.actAnyTableCurrNumPlayers.lngCheck++;
                    }
                    stg_.actThisTableAnyNumPlayers.lngCheck++;
                    stg_.actAnyTableAnyNumPlayers.lngCheck++;
                }
                // Any table
                else
                {
                    // Current number of players
                    if (hi__.lstHandPlayerInfo.Count == intCurrNumPlayers__)
                    {
                        stg_.actAnyTableCurrNumPlayers.lngCheck++;
                    }
                    stg_.actAnyTableAnyNumPlayers.lngCheck++;
                }
            }
        }

        /// <summary>
        /// Finds the most chips that any player currently has in the pot
        /// </summary>
        /// <param name="intChipsBetThisRound"></param>
        /// <param name="intChipsBetThisRoundLength"></param>
        /// <returns></returns>
        private int ChipsToCall(int[] intChipsBetThisRound, int intChipsBetThisRoundLength)
        {
            int intMaxChipsBetThisRound = 0;

            for (int i = 0; i < intChipsBetThisRoundLength; i++)
            {
                if (intChipsBetThisRound[i] > intMaxChipsBetThisRound)
                {
                    intMaxChipsBetThisRound = intChipsBetThisRound[i];
                }
            }

            return intMaxChipsBetThisRound;
        }

        /// <summary>
        /// Update the number of chips the player that acted has in the pot
        /// </summary>
        /// <param name="hi__"></param>
        /// <param name="intChipsInPot__"></param>
        /// <param name="intPlayerActionIndex"></param>
        /// <param name="intChipsPrev"></param>
        /// <param name="intChipsCurr"></param>
        /// <returns></returns>
        private int UpdatePlayerChipsInPot(DbPlayerHandInfoAll hi__, int[] intChipsInPot__, int intPlayerActionIndex, 
            ref int intChipsPrev, ref int intChipsCurr)
        {
            int intActionPlayerIndex = -1;

            // loop through the number of players in the hand
            for (int i = 0; i < hi__.lstHandPlayerInfo.Count; i++)
            {
                // if the lngHandPlayerId for this action equals the lngHandPlayerId in the hi__.lstHandPlayerInfo[i] then "i" is the
                // correct intChipsInPot__ index
                if (hi__.lstPlayerActionInfo[intPlayerActionIndex].lngHandPlayerId == hi__.lstHandPlayerInfo[i].lngHandPlayerId)
                {
                    intChipsPrev = intChipsInPot__[i];
                    intChipsInPot__[i] += hi__.lstPlayerActionInfo[intPlayerActionIndex].intChipCountChange;
                    intChipsCurr = intChipsInPot__[i];

                    intActionPlayerIndex = i;
                    break;
                }
            }

            return intActionPlayerIndex;
        }

        long DataSampleCount(Action act_)
        {
            return act_.lngFold + act_.lngCheck + act_.lngCall + act_.lngBet;
        }

        string GetPiePercentage(Action act_, int intActionIndex)
        {
            string strPercentage = "Error!!!";

            if (intActionIndex == 0)
            {
                if (act_.lngFold > 0)
                {
                    strPercentage = (100.0 * (act_.lngFold) / (act_.lngFold + act_.lngCheck + act_.lngCall + act_.lngBet)).ToString("f0") + "%";
                }
                else
                {
                    strPercentage = "";
                }
            }
            else if (intActionIndex == 1)
            {
                if (act_.lngCheck > 0)
                {
                    strPercentage = (100.0 * (act_.lngCheck) / (act_.lngFold + act_.lngCheck + act_.lngCall + act_.lngBet)).ToString("f0") + "%";
                }
                else
                {
                    strPercentage = "";
                }
            }
            else if (intActionIndex == 2)
            {
                if (act_.lngCall > 0)
                {
                    strPercentage = (100.0 * (act_.lngCall) / (act_.lngFold + act_.lngCheck + act_.lngCall + act_.lngBet)).ToString("f0") + "%";
                }
                else
                {
                    strPercentage = "";
                }
            }
            else if (intActionIndex == 3)
            {
                if (act_.lngBet > 0)
                {
                    strPercentage = (100.0 * (act_.lngBet) / (act_.lngFold + act_.lngCheck + act_.lngCall + act_.lngBet)).ToString("f0") + "%";
                }
                else
                {
                    strPercentage = "";
                }
            }

            return strPercentage;
        }

        private bool RemainingPlayersAllIn(int[] intChipsInPot_, int[] intPlayerChipStackStart_, int intPlayerIndex_, int intNumPlayers)
        {
            bool blnRemainingPlayersAllIn = true;

            for (int i = 0; i < intNumPlayers; i++)
            {
                if (i != intPlayerIndex_)
                {
                    if (intChipsInPot_[i] == intChipsInPot_[intPlayerIndex_])
                    {
                        // The current player has the same number of chips in the pot as the player of interest
                        if (intChipsInPot_[i] < intPlayerChipStackStart_[i])
                        {
                            // The current player is not currently all-in
                            blnRemainingPlayersAllIn = false;
                        }
                    }
                }
            }

            return blnRemainingPlayersAllIn;
        }

        private void NoDataChart(int intChartSetIndex_, int intChartIndex, frmDataDisplay frm_)
        {
            frm_.chtQuad[intChartSetIndex_][intChartIndex].Series[0].Points[0].Color = Color.DarkGray;

            frm_.chtQuad[intChartSetIndex_][intChartIndex].Series[0].Points[0].SetValueXY("", 1);
            frm_.chtQuad[intChartSetIndex_][intChartIndex].Series[0].Points[1].SetValueXY("", 0);
            frm_.chtQuad[intChartSetIndex_][intChartIndex].Series[0].Points[2].SetValueXY("", 0);
            frm_.chtQuad[intChartSetIndex_][intChartIndex].Series[0].Points[3].SetValueXY("", 0);

            frm_.lblChartNoData[intChartSetIndex_][intChartIndex].Visible = true;
        }

        #endregion

        #region Classes

        public class DataCounter
        {
            public HandCircumstances cirPreFlop;
            public HandCircumstances cirFlop;
            public HandCircumstances cirTurn;
            public HandCircumstances cirRiver;

            public DataCounter()
            {
                cirPreFlop = new HandCircumstances();
                cirFlop = new HandCircumstances();
                cirTurn = new HandCircumstances();
                cirRiver = new HandCircumstances();
            }
        }

        public class HandCircumstances
        {
            public Action actThisTableCurrNumPlayers;
            public Action actThisTableAnyNumPlayers;
            public Action actAnyTableCurrNumPlayers;
            public Action actAnyTableAnyNumPlayers;

            public HandCircumstances()
            {
                actThisTableCurrNumPlayers = new Action();
                actThisTableAnyNumPlayers = new Action();
                actAnyTableCurrNumPlayers = new Action();
                actAnyTableAnyNumPlayers = new Action();
            }
        }

        public class Action
        {
            public long lngFold;
            public long lngCheck;
            public long lngCall;
            public long lngBet;

            public Action()
            {
                lngFold = 0;
                lngCheck = 0;
                lngCall = 0;
                lngBet = 0;
            }
        }

        #endregion

    }
}
