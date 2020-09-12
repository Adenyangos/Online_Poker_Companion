using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOL_Companion.DbDataStructures;
using BOL_Companion.DataAnalyzerDataStructures;
using static BOL_Companion.DbController;
using System.Windows.Forms;

namespace BOL_Companion
{
    /// <summary>
    /// A class that translates data saved in the database into poker actions and plots that data.
    /// </summary>
    class DataAnalyzer
    {
        public DataAnalyzer()
        {

        }

        #region Public Methods

        /// <summary>
        /// Translate the data saved in the database into a collection poker actions organized by the hand circumstances and the betting rounds in which those actions took place.
        /// </summary>
        /// <param name="allPlayerDataList">A list of all the DbPlayerHandInfoAll collections to be analyzed for a single player</param>
        /// <param name="currGameId">The game Id of the current game</param>
        /// <param name="currNumPlayers">The current number of players at the poker table</param>
        /// <param name="playerOfInterestHandPlayerId">The current HandPlayerId of the player running this app (the user)</param>
        /// <returns>The collection of all the poker actions taken by the player organized by the hand circumstances and the betting rounds in which those actions took place</returns>
        public DataCounter AnalyzePlayerData(List<DbPlayerHandInfoAll> allPlayerDataList, int currGameId, int currNumPlayers, long playerOfInterestHandPlayerId)
        {
            // action log is the record of all the player of interest's actions
            DataCounter actionLog = new DataCounter();

            // A single DbPlayerHandInfoAll represents all the info about one hand for one player. 
            // The List<DbPlayerHandInfoAll> (allPlayerDataList) represents all the hands that player has been involved in therefore each iteration 
            // of this loop represents one hand.
            foreach (DbPlayerHandInfoAll hand in allPlayerDataList)
            {
                // If no blinds were posted this is an incomplete hand -> ignore it (we don't have the data starting from the beginning of the hand)
                if (BlindExists(hand))
                {
                    // A blind player exists (we have the full hand data available in the DB) -> move foward processing the data for this hand)

                    // The hand action number of the flop
                    int flopActionNum;
                    // The hand action number of the turn
                    int turnActionNum;
                    // The hand action number of the river
                    int riverActionNum;

                    // The default number for the board card action numbers. Setting the board card action number to a high number
                    // translates to the board card never being dealt.
                    int defaultBoardCardActionNum = 100000;

                    GetBoardCardActionNumbers(hand, defaultBoardCardActionNum, out flopActionNum, out turnActionNum, out riverActionNum);

                    // Assuming 10 players per poker table max
                    int maxPlayersPerTable = 10;

                    // playerChipsInPot is the number of chips each player has in the pot
                    int[] playerChipsInPot = new int[maxPlayersPerTable];

                    // playerChipStackStart is the number of chips each player has at the beginning of the hand (before blinds and antes)
                    int[] playerChipStackStart = new int[maxPlayersPerTable];

                    // blindPlayerIndices are the indices of the two blind players
                    int[] blindPlayerIndices = { -1, -1 };

                    // playerOfInterestIndex is the index of the player of interest
                    int playerOfInterestIndex = InitializeHandStartingConditions(hand, playerChipsInPot, playerChipStackStart, blindPlayerIndices);

                    if (UnfinishedHand(hand))
                    {
                        for (int i = 0; i < hand.HandPlayerInfoList.Count; i++)
                        {
                            // Check to see if this is the hand currently in progress (use playerOfInterestHandPlayerId which is the user's
                            // HandPlayerId for the hand in progress)
                            if (hand.HandPlayerInfoList[i].HandPlayerId == playerOfInterestHandPlayerId)
                            {
                                ModifyDataToProcessHandInProgress(hand, actionLog, flopActionNum, turnActionNum, riverActionNum, playerOfInterestHandPlayerId);
                                break;
                            }
                        }
                    }

                    // firstPlayerToActIndex is the index of the first player to act in this hand
                    int firstPlayerToActIndex = FindFirstPlayerToActIndex(hand, blindPlayerIndices);

                    // Has the player of interested folded
                    bool playerOfInterestFolded = false;

                    // Does a preflop check need to be logged for the player of interest
                    bool logPlayerOfInterestPreFlopCheck = true;

                    // This is the first bet, call or fold action on the flop
                    bool isFirstFlopAction = true;
                    // This is the first bet, call or fold action on the turn
                    bool isFirstTurnAction = true;
                    // This is the first bet, call or fold action on the river
                    bool isFirstRiverAction = true;

                    // Did any player bet preflop
                    bool preFlopBetMade = false;
                    // Has there been at least one player action (bet, call, check or fold) logged for the flop
                    bool flopActionTaken = false;
                    // Has there been at least one player action (bet, call, check or fold) logged for the turn
                    bool turnActionTaken = false;
                    // Has there been at least one player action (bet, call, check or fold) logged for the river
                    bool riverActionTaken = false;

                    // Go through each player action in this hand but only log actions for plotting if the action was performed by the player of interest
                    // (hand.HandInfo.HandPlayerId == hand.PlayerActionInfoList[i].HandPlayerId)
                    // else just keep track of how many chips each player has in the pot.
                    for (int i = 0; i < hand.PlayerActionInfoList.Count; i++)
                    {
                        // No need to continue processing player actions if the player of interest has already folded because we are only tracking the
                        // actions of the player of interest
                        if (!playerOfInterestFolded)
                        {
                            // Pre-Flop action
                            if (hand.PlayerActionInfoList[i].HandActionNumber < flopActionNum)
                            {
                                if (ProcessPlayerAciton(hand, i, playerChipsInPot, currGameId, currNumPlayers, actionLog.Preflop,
                                    firstPlayerToActIndex, playerOfInterestIndex, playerChipStackStart[playerOfInterestIndex], ref preFlopBetMade, ref playerOfInterestFolded, true))
                                {
                                    logPlayerOfInterestPreFlopCheck = false;
                                }
                            }

                            // Flop action
                            else if (hand.PlayerActionInfoList[i].HandActionNumber < turnActionNum)
                            {
                                // This is a bet, call or fold action on the flop
                                if (isFirstFlopAction)
                                {
                                    TestForPreflopCheckAction(hand, actionLog, ref logPlayerOfInterestPreFlopCheck, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    isFirstFlopAction = false;
                                }

                                ProcessPlayerAciton(hand, i, playerChipsInPot, currGameId, currNumPlayers, actionLog.Flop, firstPlayerToActIndex,
                                    playerOfInterestIndex, playerChipStackStart[playerOfInterestIndex], ref flopActionTaken, ref playerOfInterestFolded, false);
                            }

                            // Turn action
                            else if (hand.PlayerActionInfoList[i].HandActionNumber < riverActionNum)
                            {
                                // This is a bet, call or fold action on the turn
                                if (isFirstTurnAction)
                                {
                                    TestForPreflopCheckAction(hand, actionLog, ref logPlayerOfInterestPreFlopCheck, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    TestForFlopCheckAction(hand, actionLog, ref flopActionTaken, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    isFirstTurnAction = false;
                                }

                                ProcessPlayerAciton(hand, i, playerChipsInPot, currGameId, currNumPlayers, actionLog.Turn, firstPlayerToActIndex,
                                    playerOfInterestIndex, playerChipStackStart[playerOfInterestIndex], ref turnActionTaken, ref playerOfInterestFolded, false);
                            }

                            // River action
                            else
                            {
                                // This is a bet, call, fold or pot winning action on the river
                                if (isFirstRiverAction)
                                {
                                    TestForPreflopCheckAction(hand, actionLog, ref logPlayerOfInterestPreFlopCheck, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    TestForFlopCheckAction(hand, actionLog, ref flopActionTaken, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    TestForTurnCheckAction(hand, actionLog, ref turnActionTaken, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);

                                    isFirstRiverAction = false;
                                }

                                // This is the last action of the hand (also note that this player action is occuring on the river)
                                if (i == hand.PlayerActionInfoList.Count - 1)
                                {
                                    TestForRiverCheckAction(hand, actionLog, ref riverActionTaken, playerChipsInPot, playerChipStackStart,
                                        playerOfInterestIndex, currGameId, currNumPlayers);
                                }

                                ProcessPlayerAciton(hand, i, playerChipsInPot, currGameId, currNumPlayers, actionLog.River, firstPlayerToActIndex,
                                    playerOfInterestIndex, playerChipStackStart[playerOfInterestIndex], ref riverActionTaken, ref playerOfInterestFolded, false);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return actionLog;
        }

        /// <summary>
        /// Plot a set of charts (4 charts) with the action data contained in the HandCircumstances object.
        /// </summary>
        /// <param name="bettingRoundData">All the action data from a single betting round for a single player</param>
        /// <param name="chartSetToUpdate_">The set of charts to plot the data on</param>
        /// <param name="chartSetNoDataLabels_">The set of "No Data" lables for the chart set</param>
        /// <param name="chartSetSampleCount_">The set of sample count labels for the chart set</param>
        /// <param name="isChartSetUpdated_">The set of boolean chart updated variables for the chart set</param>
        public void PlotChartSet(HandCircumstances bettingRoundData, MyChart[] chartSetToUpdate_, Label[] chartSetNoDataLabels_,
            Label[] chartSetSampleCount_, bool[] isChartSetUpdated_)
        {
            // Chart for "This Table Current Number of Players"
            PlotChart(bettingRoundData.ThisTableCurrNumPlayers, chartSetToUpdate_[DataDisplayForm.ThisTableCurrNumPlayersIndex],
                chartSetNoDataLabels_[DataDisplayForm.ThisTableCurrNumPlayersIndex], chartSetSampleCount_[DataDisplayForm.ThisTableCurrNumPlayersIndex],
                ref isChartSetUpdated_[DataDisplayForm.ThisTableCurrNumPlayersIndex]);

            // Chart for "This Table Any Number of Players"
            PlotChart(bettingRoundData.ThisTableAnyNumPlayers, chartSetToUpdate_[DataDisplayForm.ThisTableAnyNumPlayersIndex],
                chartSetNoDataLabels_[DataDisplayForm.ThisTableAnyNumPlayersIndex], chartSetSampleCount_[DataDisplayForm.ThisTableAnyNumPlayersIndex],
                ref isChartSetUpdated_[DataDisplayForm.ThisTableAnyNumPlayersIndex]);

            // Chart for "Any Table Current Number of Players"
            PlotChart(bettingRoundData.AnyTableCurrNumPlayers, chartSetToUpdate_[DataDisplayForm.AnyTableCurrNumPlayersIndex],
                chartSetNoDataLabels_[DataDisplayForm.AnyTableCurrNumPlayersIndex], chartSetSampleCount_[DataDisplayForm.AnyTableCurrNumPlayersIndex],
                ref isChartSetUpdated_[DataDisplayForm.AnyTableCurrNumPlayersIndex]);

            // Chart for "Any Table Any Number of Players"
            PlotChart(bettingRoundData.AnyTableAnyNumPlayers, chartSetToUpdate_[DataDisplayForm.AnyTableAnyNumPlayersIndex],
                chartSetNoDataLabels_[DataDisplayForm.AnyTableAnyNumPlayersIndex], chartSetSampleCount_[DataDisplayForm.AnyTableAnyNumPlayersIndex],
                ref isChartSetUpdated_[DataDisplayForm.AnyTableAnyNumPlayersIndex]);
        }

        /// <summary>
        /// Sum up the poker actions of all players except the player running this application.
        /// </summary>
        /// <param name="playerData">The player data for all the players</param>
        /// <param name="indexToSkip">The index of the player running this application</param>
        /// <returns>The sum of the poker actions of all players except the player running this application</returns>
        public DataCounter SumPlayerData(DataCounter[] playerData, int indexToSkip)
        {
            DataCounter sumedData = new DataCounter();

            for (int i = 0; i < 9; i++)
            {
                if (i != indexToSkip)
                {
                    // PreFlop
                    AddPokerActionsToSum(sumedData.Preflop.ThisTableCurrNumPlayers, playerData[i].Preflop.ThisTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Preflop.ThisTableAnyNumPlayers, playerData[i].Preflop.ThisTableAnyNumPlayers);
                    AddPokerActionsToSum(sumedData.Preflop.AnyTableCurrNumPlayers, playerData[i].Preflop.AnyTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Preflop.AnyTableAnyNumPlayers, playerData[i].Preflop.AnyTableAnyNumPlayers);

                    // Flop
                    AddPokerActionsToSum(sumedData.Flop.ThisTableCurrNumPlayers, playerData[i].Flop.ThisTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Flop.ThisTableAnyNumPlayers, playerData[i].Flop.ThisTableAnyNumPlayers);
                    AddPokerActionsToSum(sumedData.Flop.AnyTableCurrNumPlayers, playerData[i].Flop.AnyTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Flop.AnyTableAnyNumPlayers, playerData[i].Flop.AnyTableAnyNumPlayers);

                    // Turn
                    AddPokerActionsToSum(sumedData.Turn.ThisTableCurrNumPlayers, playerData[i].Turn.ThisTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Turn.ThisTableAnyNumPlayers, playerData[i].Turn.ThisTableAnyNumPlayers);
                    AddPokerActionsToSum(sumedData.Turn.AnyTableCurrNumPlayers, playerData[i].Turn.AnyTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.Turn.AnyTableAnyNumPlayers, playerData[i].Turn.AnyTableAnyNumPlayers);

                    // River
                    AddPokerActionsToSum(sumedData.River.ThisTableCurrNumPlayers, playerData[i].River.ThisTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.River.ThisTableAnyNumPlayers, playerData[i].River.ThisTableAnyNumPlayers);
                    AddPokerActionsToSum(sumedData.River.AnyTableCurrNumPlayers, playerData[i].River.AnyTableCurrNumPlayers);
                    AddPokerActionsToSum(sumedData.River.AnyTableAnyNumPlayers, playerData[i].River.AnyTableAnyNumPlayers);
                }
            }
            return sumedData;
        }

        #endregion

        #region Helper Methods

        #region Processing database data (player actions)

        /// <summary>
        /// Check to make sure at least one player posted a blind this hand.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <returns>True if at least one player posted a blind</returns>
        private bool BlindExists(DbPlayerHandInfoAll hand_)
        {
            foreach (DbHandPlayerInfo handPlayer in hand_.HandPlayerInfoList)
            {
                if (handPlayer.Blind > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine the actions numbers for pre-flop, flop, turn and river actions. (flopActionNum, turnActionNum and riverActionNum)
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="defaultBoardCardActionNum_">The default value for the board card action numbers</param>
        /// <param name="flopActionNum_">The action number for the first flop card</param>
        /// <param name="turnActionNum_">The ation number for the turn card</param>
        /// <param name="riverActionNum_">The action number for the river card</param>
        private void GetBoardCardActionNumbers(DbPlayerHandInfoAll hand_, int defaultBoardCardActionNum_, out int flopActionNum_, out int turnActionNum_, out int riverActionNum_)
        {
            // Set the board card action numbers to their default value
            flopActionNum_ = defaultBoardCardActionNum_;
            turnActionNum_ = defaultBoardCardActionNum_;
            riverActionNum_ = defaultBoardCardActionNum_;

            // Update the board card action numbers if the board cards were dealt this hand
            for (int i = 0; i < hand_.BoardActionInfoList.Count; i++)
            {
                if (i == 0)
                {
                    flopActionNum_ = hand_.BoardActionInfoList[i].HandActionNumber;
                }
                else if (i == 3)
                {
                    turnActionNum_ = hand_.BoardActionInfoList[i].HandActionNumber;
                }
                else if (i == 4)
                {
                    riverActionNum_ = hand_.BoardActionInfoList[i].HandActionNumber;
                }
            }  
        }

        /// <summary>
        /// Update the playerChipsInPot array with antes and blinds, find the indices of the blind players and find the index of the player of interest.
        /// </summary>
        /// <remarks>
        /// <para>Only updating the elements of the playerChipsInPot array for the players who are actually in the hand.</para>
        /// <para>The indices of the blind players will be used later to determine the first player to act.</para>
        /// </remarks>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="blindPlayerIndices_">The indices of the two blind players [array]</param>
        /// <returns>The index of the player of interest for this hand</returns>
        private int InitializeHandStartingConditions(DbPlayerHandInfoAll hand_, int[] playerChipsInPot_, int[] playerChipStackStart_, int[] blindPlayerIndices_)
        {
            int playerOfInterestIndex_ = -1;

            for (int i = 0; i < hand_.HandPlayerInfoList.Count; i++)
            {
                playerChipsInPot_[i] = hand_.HandPlayerInfoList[i].Blind + hand_.HandInfo.Ante;
                playerChipStackStart_[i] = hand_.HandPlayerInfoList[i].ChipCountStart;

                if (hand_.HandInfo.HandPlayerId == hand_.HandPlayerInfoList[i].HandPlayerId)
                {
                    playerOfInterestIndex_ = i;
                }

                if (hand_.HandPlayerInfoList[i].Blind > 0)
                {
                    if (blindPlayerIndices_[0] == -1)
                    {
                        blindPlayerIndices_[0] = i;
                    }
                    else
                    {
                        blindPlayerIndices_[1] = i;
                    }
                }
            }

            return playerOfInterestIndex_;
        }

        /// <summary>
        /// Check to see if this hand has started and has a winner.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <returns>True if this hand has started and has no winner</returns>
        private bool UnfinishedHand(DbPlayerHandInfoAll hand_)
        {
            // Check to make sure this hand has a winner by checking to see if the hand ended with someone gaining chips which is denoted 
            // by a negative chipCountChange number. First I will check to make sure the hand has started by checking 
            // hand.PlayerActionInfoList.Count > 0. If hand.PlayerActionInfoList.Count == 0 it means this is pre-flop and the player of 
            // interest is the first player to act so nothing has happened yet.
            return (hand_.PlayerActionInfoList.Count > 0 && hand_.PlayerActionInfoList[hand_.PlayerActionInfoList.Count - 1].ChipCountChange >= 0);
        }

        /// <summary>
        /// Modify the hand data so that check actions immediately before the current oppertunity to act are processed properly. 
        /// </summary>
        /// <remarks>
        /// At this point we have a hand in progress that has not been completed. It is the player of interest's turn to act. If one or more 
        /// players checked immediately before this point those actions would not be processed correctly because checks are not saved to the 
        /// database. Once subsequent actions are taken checks are identified by skipped opportunities to act.
        /// </remarks>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionLog_">The record of all the player of interest's actions</param>
        /// <param name="flopActionNum_">The hand action number of the flop</param>
        /// <param name="turnActionNum_">The hand action number of the turn</param>
        /// <param name="riverActionNum_">The hand action number of the river</param>
        /// <param name="playerOfInterestHandPlayerId_">The current HandPlayerId of the player of interest</param>
        private void ModifyDataToProcessHandInProgress(DbPlayerHandInfoAll hand_, DataCounter actionLog_, int flopActionNum_, int turnActionNum_,
            int riverActionNum_, long playerOfInterestHandPlayerId_)
        {
            int largeBet = 1000000000;
            int nextHandActionNum = FindNextHandActionNum(hand_, flopActionNum_, turnActionNum_, riverActionNum_);

            // It is the player of interest's (playerOfInterestHandPlayerId_) turn to act in the hand currently in progress.
            // Add a large bet action to the PlayerActionInfoList for the player of interest so that players who checked
            // immediately before the player of interest will have there actions counted.
            AddBetActionToHand(hand_, playerOfInterestHandPlayerId_, largeBet, nextHandActionNum);

            // Is the player currently being analyzed the player of interest (the app user)
            if (hand_.HandInfo.HandPlayerId == playerOfInterestHandPlayerId_)
            {
                // Subtract 1 bet from the appropriate DataCounter betting round tally for the player of interest (the user)

                // Find out if this is a pre-flop, flop, turn or river fold
                if (riverActionNum_ < 100000)
                {
                    // We already know this is the current hand in progress so it is "This Table" and it is also
                    // "Curr Number of Players"

                    AddBetActionToActionLog(actionLog_.River, -1);
                }
                else if (turnActionNum_ < 100000)
                {
                    // We already know this is the current hand in progress so it is "This Table" and it is also
                    // "Curr Number of Players"

                    AddBetActionToActionLog(actionLog_.Turn, -1);
                }
                else if (flopActionNum_ < 100000)
                {
                    // We already know this is the current hand in progress so it is "This Table" and it is also
                    // "Curr Number of Players"

                    AddBetActionToActionLog(actionLog_.Flop, -1);
                }
                else
                {
                    // We already know this is the current hand in progress so it is "This Table" and it is also
                    // "Curr Number of Players"

                    AddBetActionToActionLog(actionLog_.Preflop, -1);
                }
            }
        }

        /// <summary>
        /// Find the next hand action number.
        /// </summary>
        /// <param name="hand__">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="flopActionNum__">The hand action number of the flop</param>
        /// <param name="turnActionNum__">The hand action number of the turn</param>
        /// <param name="riverActionNum__">The hand action number of the river</param>
        /// <returns>The next hand action number</returns>
        private int FindNextHandActionNum(DbPlayerHandInfoAll hand__, int flopActionNum__, int turnActionNum__, int riverActionNum__)
        {
            int nextHandActionNum = 0;

            // If the last action was a board card being shown then the next hand action number is either flopActionNumber__ + 1, 
            // turnActionNumber__ + 1 or riverActionNumber__ + 1.

            // Find latest board action
            if (riverActionNum__ < 100000)
            {
                nextHandActionNum = riverActionNum__ + 1;
            }
            else if (turnActionNum__ < 100000)
            {
                nextHandActionNum = turnActionNum__ + 1;
            }
            else if (flopActionNum__ < 100000)
            {
                nextHandActionNum = flopActionNum__ + 1;
            }

            // If the last action was a player action the next hand action number will be PlayerActionInfoList.Count.

            // Check to see if the latest hand action was more recent than the latest board action.
            if (hand__.PlayerActionInfoList.Count > nextHandActionNum)
            {
                nextHandActionNum = hand__.PlayerActionInfoList.Count;
            }

            return nextHandActionNum;
        }

        /// <summary>
        /// Add a bet action to the PlayerActionInfoList for a given player.
        /// </summary>
        /// <param name="hand__">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="handPlayerId">The handPlayerId of the player you wish to add the bet action to</param>
        /// <param name="bet">The value of the bet action you wish to add</param>
        /// <param name="handActionNum">The handActionNumber of the bet you wish to add</param>
        private void AddBetActionToHand(DbPlayerHandInfoAll hand__, long handPlayerId, int bet, int handActionNum)
        {
            hand__.PlayerActionInfoList.Add(new DbPlayerActionInfo
            {
                HandPlayerId = handPlayerId,
                ChipCountChange = bet,
                HandActionNumber = handActionNum
            });

            for (int i = 0; i < hand__.HandPlayerInfoList.Count; i++)
            {
                if (hand__.HandPlayerInfoList[i].HandPlayerId == handPlayerId)
                {
                    // Add the same bet to the player's initial chip count to prevent issues that might arise with pre-flop
                    // call/bet minimums and all-in issues in the code
                    hand__.HandPlayerInfoList[i].ChipCountStart += bet;
                    break;
                }
            }
        }

        /// <summary>
        /// Add bet actions to the ActionLog for a given round of betting. The bet actions will apply to all four of the possible 
        /// HandCircumstances (ThisTableCurrNumPlayers, ThisTableAnyNumPlayers, AnyTableCurrNumPlayers and AnyTableAnyNumPlayers).
        /// </summary>
        /// <param name="bettingRound">The DataCounter.HandCircumstances object representing the betting round of the hand you wish to add a bet action to</param>
        /// <param name="numBets">The number of bet actions you wish to add</param>
        private void AddBetActionToActionLog(HandCircumstances bettingRound, int numBets)
        {
            bettingRound.ThisTableCurrNumPlayers.Bet += numBets;
            bettingRound.ThisTableAnyNumPlayers.Bet += numBets;
            bettingRound.AnyTableCurrNumPlayers.Bet += numBets;
            bettingRound.AnyTableAnyNumPlayers.Bet += numBets;
        }

        /// <summary>
        /// Find the index of the first player to act in this hand.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="blindPlayerIndices_">The indices of the two blind players [array]</param>
        /// <returns>The index of the first player to act</returns>
        private int FindFirstPlayerToActIndex(DbPlayerHandInfoAll hand_, int[] blindPlayerIndices_)
        {
            int firstPlayerToActIndex_;

            // If only one player posted a blind that player was the first to act
            if (blindPlayerIndices_[1] == -1)
            {
                firstPlayerToActIndex_ = blindPlayerIndices_[0];
            }
            // More than one player posted a blind
            else
            {
                // If there are more than two players left
                if (hand_.HandPlayerInfoList.Count > 2)
                {
                    // The player with the lowest index is always the first to act unless the player in the first seat at the table and the player
                    // in the last seat at the table were the two blinds. In that case the player with the higher index was the first to act.
                    if (!(blindPlayerIndices_[0] == 0 && blindPlayerIndices_[1] == hand_.HandPlayerInfoList.Count - 1))
                    {
                        firstPlayerToActIndex_ = blindPlayerIndices_[0];
                    }
                    else
                    {
                        firstPlayerToActIndex_ = blindPlayerIndices_[1];
                    }
                }
                // only two players are left
                else
                {
                    // Check to see if the blinds posted were the same. If so one of the players is all-in
                    if (hand_.HandPlayerInfoList[0].Blind == hand_.HandPlayerInfoList[1].Blind)
                    {
                        // In this extremely rare case it doesn't matter who the first player to act is because no poker actions will
                        // take place because one of the two remaining players is already all-in
                        firstPlayerToActIndex_ = blindPlayerIndices_[0];
                    }
                    // With two players left the player who posted the bigger blind is the dealer -> the other player is the first to act. 
                    if (hand_.HandPlayerInfoList[0].Blind > hand_.HandPlayerInfoList[1].Blind)
                    {
                        firstPlayerToActIndex_ = blindPlayerIndices_[1];
                    }
                    else
                    {
                        firstPlayerToActIndex_ = blindPlayerIndices_[0];
                    }
                }
            }

            return firstPlayerToActIndex_;
        }

        /// <summary>
        /// Process a player action from the hand.PlayerActionInfoList.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionIndex">The index of the player action to update</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="currGameId_">The game Id of the current game</param>
        /// <param name="currNumPlayers_">The current number of players at the poker table</param>
        /// <param name="bettingRound">The DataCounter.HandCircumstances object representing the betting round this action took place in</param>
        /// <param name="intFirstPlayerToActIndex_">The index of the action player for this hand</param>
        /// <param name="playerOfInterestIndex_">The index of the player of interest for this hand</param>
        /// <param name="playerOfInterestChipStackStart_">The number of chips the player of interest had at the beggining of the hand (before blinds and antes)</param>
        /// <param name="actionTaken_">Has there been at least one player action (bet) in this round of betting</param>
        /// <param name="playerOfInterestFolded_">Has the player of interested folded</param>
        /// <param name="isPreflop">Is this action taking place pre-flop</param>
        /// <returns>Did the player of interest bet, call or fold</returns>
        private bool ProcessPlayerAciton(DbPlayerHandInfoAll hand_, int actionIndex, int[] playerChipsInPot_, int currGameId_, 
            int currNumPlayers_, HandCircumstances bettingRound, int intFirstPlayerToActIndex_, int playerOfInterestIndex_, 
            int playerOfInterestChipStackStart_, ref bool actionTaken_, ref bool playerOfInterestFolded_, bool isPreflop)
        {
            // Did the player of interest bet, call or fold
            bool playerOfInterestBetCallOrFoldAction = false;

            // The number of chips the player had in the pot before the player acted
            int playerChipsInPotPrev = -1;
            // The number of chips the player has in the pot after the player acted
            int playerChipsInPotCurr = -1;

            // Action made by the player of interest -> log data
            if (hand_.HandInfo.HandPlayerId == hand_.PlayerActionInfoList[actionIndex].HandPlayerId)
            {
                // Chips put into the pot (call or bet)
                if (hand_.PlayerActionInfoList[actionIndex].ChipCountChange > 0)
                {
                    playerOfInterestBetCallOrFoldAction = true;

                    // This is a bet
                    if ((playerChipsInPot_[playerOfInterestIndex_] + hand_.PlayerActionInfoList[actionIndex].ChipCountChange) > 
                        ChipsToCall(playerChipsInPot_, hand_.HandPlayerInfoList.Count))
                    {
                        actionTaken_ = true;
                        AddPokerActionBasedOnHandCircumstances(hand_, bettingRound, PokerActions.PossibleActions.Bet, currGameId_, currNumPlayers_);
                    }
                    // This is a call
                    else
                    {
                        AddPokerActionBasedOnHandCircumstances(hand_, bettingRound, PokerActions.PossibleActions.Call, currGameId_, currNumPlayers_);
                    }

                    // Keep track of how many chips this player has in the pot
                    UpdatePlayerChipsInPot(hand_, playerChipsInPot_, actionIndex, ref playerChipsInPotPrev, ref playerChipsInPotCurr);
                }

                // Fold
                else if (hand_.PlayerActionInfoList[actionIndex].ChipCountChange == 0)
                {
                    playerOfInterestBetCallOrFoldAction = true;
                    playerOfInterestFolded_ = true;

                    AddPokerActionBasedOnHandCircumstances(hand_, bettingRound, PokerActions.PossibleActions.Fold, currGameId_, currNumPlayers_);
                }
                // else the player won the pot which we don't need to log here
            }

            // Action taken by a player other than the player of interest
            else
            {
                // 1) Keep track of how many chips this player has in the pot
                
                // actionPlayerIndex is the index of the aciton player in this hand (hand_.HandPlayerInfoList[index])
                int actionPlayerIndex_ = UpdatePlayerChipsInPot(hand_, playerChipsInPot_, actionIndex, ref playerChipsInPotPrev, 
                    ref playerChipsInPotCurr);

                // 2) Check to see if the player of interest checked before this player's action so that we can log that check action

                // 2a) The betting stage is not pre-flop. Pre-flop checks are determined using another method
                if (!isPreflop)
                {
                    // 2b) A bet was made
                    if (playerChipsInPotCurr > playerChipsInPotPrev)
                    {
                        // 2c) This was the first bet made in this betting round
                        if (!actionTaken_)
                        {
                            // 2d) The player of interest is still in this hand
                            if (!playerOfInterestFolded_)
                            {
                                // 2e) The sequence of actions is correct
                                if ((intFirstPlayerToActIndex_ <= playerOfInterestIndex_ && playerOfInterestIndex_ < actionPlayerIndex_) ||
                                    (actionPlayerIndex_ < intFirstPlayerToActIndex_ && intFirstPlayerToActIndex_ <= playerOfInterestIndex_) ||
                                    (playerOfInterestIndex_ < actionPlayerIndex_ && actionPlayerIndex_ < intFirstPlayerToActIndex_))
                                {
                                    // 2f) Check to make sure the player of interest isn't already all-in (a player is only checking if that 
                                    // player still has chips available)
                                    if (playerChipsInPot_[playerOfInterestIndex_] < playerOfInterestChipStackStart_)
                                    {
                                        // 2g) Call AddPokerActionBasedOnHandCircumstances to add a check action for the player of interest
                                        AddPokerActionBasedOnHandCircumstances(hand_, bettingRound, PokerActions.PossibleActions.Check, 
                                            currGameId_, currNumPlayers_);
                                    }
                                }
                            }
                        }
                        actionTaken_ = true;
                    }
                }
            }

            return playerOfInterestBetCallOrFoldAction;
        }

        /// <summary>
        /// Find the largest number of chips any player has in the pot.
        /// </summary>
        /// <param name="playerChipsInPot__">The number of chips each player has in the pot [array]</param>
        /// <param name="playerInHand">The number of players in the current hand</param>
        /// <returns>The largest number of chips any player has in the pot</returns>
        private int ChipsToCall(int[] playerChipsInPot__, int playerInHand)
        {
            int maxChipsBetThisRound = 0;

            for (int i = 0; i < playerInHand; i++)
            {
                if (playerChipsInPot__[i] > maxChipsBetThisRound)
                {
                    maxChipsBetThisRound = playerChipsInPot__[i];
                }
            }

            return maxChipsBetThisRound;
        }

        /// <summary>
        /// Add a poker action to appropriate HandCircumstances and betting round of the ActionLog. 
        /// </summary>
        /// <param name="hand__">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="bettingRound_">The DataCounter.HandCircumstances object representing the betting round of the hand you wish to add an action to</param>
        /// <param name="pokerAction">The poker action you wish to add (bet, call, check, fold)</param>
        /// <param name="currGameId__">The game Id of the current game</param>
        /// <param name="currNumPlayers__">The current number of players at the poker table</param>
        private void AddPokerActionBasedOnHandCircumstances(DbPlayerHandInfoAll hand__, HandCircumstances bettingRound_,
            PokerActions.PossibleActions pokerAction, int currGameId__, int currNumPlayers__)
        {
            bool incrementThisTableCurrNumPlayers = false;
            bool incrementThisTableAnyNumPlayers = false;
            bool incrementAnyTableCurrNumPlayers = false;
            bool incrementAnyTableAnyNumPlayers = false;

            // This is the current table (current game being played)
            if (hand__.HandInfo.GameId == currGameId__)
            {
                // This action took place with the same number of players at the table as are currently at the table
                if (hand__.HandPlayerInfoList.Count == currNumPlayers__)
                {
                    incrementThisTableCurrNumPlayers = true;
                    incrementAnyTableCurrNumPlayers = true;
                }
                incrementThisTableAnyNumPlayers = true;
                incrementAnyTableAnyNumPlayers = true;
            }
            // Any table
            else
            {
                // This action took place with the same number of players at the table as are currently at the current table
                if (hand__.HandPlayerInfoList.Count == currNumPlayers__)
                {
                    incrementAnyTableCurrNumPlayers = true;
                }
                incrementAnyTableAnyNumPlayers = true;
            }

            // Apply these actions to the correct hand circumstances and betting round
            if (pokerAction == PokerActions.PossibleActions.Bet)
            {
                if (incrementThisTableCurrNumPlayers)
                {
                    bettingRound_.ThisTableCurrNumPlayers.Bet++;
                }
                if (incrementThisTableAnyNumPlayers)
                {
                    bettingRound_.ThisTableAnyNumPlayers.Bet++;
                }
                if (incrementAnyTableCurrNumPlayers)
                {
                    bettingRound_.AnyTableCurrNumPlayers.Bet++;
                }
                if (incrementAnyTableAnyNumPlayers)
                {
                    bettingRound_.AnyTableAnyNumPlayers.Bet++;
                }
            }
            else if (pokerAction == PokerActions.PossibleActions.Call)
            {
                if (incrementThisTableCurrNumPlayers)
                {
                    bettingRound_.ThisTableCurrNumPlayers.Call++;
                }
                if (incrementThisTableAnyNumPlayers)
                {
                    bettingRound_.ThisTableAnyNumPlayers.Call++;
                }
                if (incrementAnyTableCurrNumPlayers)
                {
                    bettingRound_.AnyTableCurrNumPlayers.Call++;
                }
                if (incrementAnyTableAnyNumPlayers)
                {
                    bettingRound_.AnyTableAnyNumPlayers.Call++;
                }
            }
            else if (pokerAction == PokerActions.PossibleActions.Check)
            {
                if (incrementThisTableCurrNumPlayers)
                {
                    bettingRound_.ThisTableCurrNumPlayers.Check++;
                }
                if (incrementThisTableAnyNumPlayers)
                {
                    bettingRound_.ThisTableAnyNumPlayers.Check++;
                }
                if (incrementAnyTableCurrNumPlayers)
                {
                    bettingRound_.AnyTableCurrNumPlayers.Check++;
                }
                if (incrementAnyTableAnyNumPlayers)
                {
                    bettingRound_.AnyTableAnyNumPlayers.Check++;
                }
            }
            else if (pokerAction == PokerActions.PossibleActions.Fold)
            {
                if (incrementThisTableCurrNumPlayers)
                {
                    bettingRound_.ThisTableCurrNumPlayers.Fold++;
                }
                if (incrementThisTableAnyNumPlayers)
                {
                    bettingRound_.ThisTableAnyNumPlayers.Fold++;
                }
                if (incrementAnyTableCurrNumPlayers)
                {
                    bettingRound_.AnyTableCurrNumPlayers.Fold++;
                }
                if (incrementAnyTableAnyNumPlayers)
                {
                    bettingRound_.AnyTableAnyNumPlayers.Fold++;
                }
            }
        }

        /// <summary>
        /// Update the number of chips the player that acted has in the pot.
        /// </summary>
        /// <param name="hand__">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="playerChipsInPot__">The number of chips each player has in the pot [array]</param>
        /// <param name="actionIndex_">The index of the player action to update</param>
        /// <param name="chipsPrev">The number of chips the action player had before the action</param>
        /// <param name="chipsCurr">The number of chips the action player currently has (after the action was completed)</param>
        /// <returns>The index of the action player</returns>
        private int UpdatePlayerChipsInPot(DbPlayerHandInfoAll hand__, int[] playerChipsInPot__, int actionIndex_,
            ref int chipsPrev, ref int chipsCurr)
        {
            // The index of the action player
            int actionPlayerIndex = -1;

            // Loop through all the players in the hand
            for (int i = 0; i < hand__.HandPlayerInfoList.Count; i++)
            {
                // If the HandPlayerId for this action equals the HandPlayerId in the hand__.HandPlayerInfoList[i] then "i" is the
                // correct playerChipsInPot__ index
                if (hand__.PlayerActionInfoList[actionIndex_].HandPlayerId == hand__.HandPlayerInfoList[i].HandPlayerId)
                {
                    chipsPrev = playerChipsInPot__[i];
                    playerChipsInPot__[i] += hand__.PlayerActionInfoList[actionIndex_].ChipCountChange;
                    chipsCurr = playerChipsInPot__[i];

                    actionPlayerIndex = i;
                    break;
                }
            }

            return actionPlayerIndex;
        }

        /// <summary>
        /// Test to see if there was a preflop check that needs to be logged. If there was log that check action for the player of interest.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionLog_">The record of all the player of interest's actions</param>
        /// <param name="logPlayerOfInterestPreFlopCheck_">Does a preflop check need to be logged for the player of interest</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="playerOfInterestIndex_">The index of the player of interest for this hand</param>
        /// <param name="currGameId_">The game Id of the current game</param>
        /// <param name="currNumPlayers_">The current number of players at the poker table</param>
        private void TestForPreflopCheckAction(DbPlayerHandInfoAll hand_, DataCounter actionLog_, ref bool logPlayerOfInterestPreFlopCheck_, 
            int[] playerChipsInPot_, int[] playerChipStackStart_, int playerOfInterestIndex_, int currGameId_, int currNumPlayers_)
        {
            // The player of interest didn't bet, call or fold preflop. Other player(s) remain in the hand who are not all-in
            if (logPlayerOfInterestPreFlopCheck_ && 
                !RemainingPlayersAllIn(playerChipsInPot_, playerChipStackStart_, playerOfInterestIndex_, hand_.HandPlayerInfoList.Count))
            {
                // The player of interest is not all-in
                if (playerChipsInPot_[playerOfInterestIndex_] < playerChipStackStart_[playerOfInterestIndex_])
                {
                    // The player of interest checked pre-flop
                    AddPokerActionBasedOnHandCircumstances(hand_, actionLog_.Preflop, PokerActions.PossibleActions.Check,
                        currGameId_, currNumPlayers_);
                }
                logPlayerOfInterestPreFlopCheck_ = false;
            }
        }

        /// <summary>
        /// Test to see if there was a check on the flop that needs to be logged. If there was log that check action for the player of interest.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionLog_">The record of all the player of interest's actions</param>
        /// <param name="flopActionTaken_">Has there been at least one player action (bet, call, check or fold) logged for the flop</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="playerOfInterestIndex_">The index of the player of interest for this hand</param>
        /// <param name="currGameId_">The game Id of the current game</param>
        /// <param name="currNumPlayers_">The current number of players at the poker table</param>
        private void TestForFlopCheckAction(DbPlayerHandInfoAll hand_, DataCounter actionLog_, ref bool flopActionTaken_,
            int[] playerChipsInPot_, int[] playerChipStackStart_, int playerOfInterestIndex_, int currGameId_, int currNumPlayers_)
        {
            // No player bet on the flop. Other player(s) remain in the hand who are not all-in
            if (!flopActionTaken_ && 
                !RemainingPlayersAllIn(playerChipsInPot_, playerChipStackStart_, playerOfInterestIndex_, hand_.HandPlayerInfoList.Count))
            {
                // The player of interest is not all-in
                if (playerChipsInPot_[playerOfInterestIndex_] < playerChipStackStart_[playerOfInterestIndex_])
                {
                    // The player of interest checked on the flop
                    AddPokerActionBasedOnHandCircumstances(hand_, actionLog_.Flop, PokerActions.PossibleActions.Check,
                        currGameId_, currNumPlayers_);
                }
                flopActionTaken_ = true;
            }
        }

        /// <summary>
        /// Test to see if there was a check on the turn that needs to be logged. If there was log that check action for the player of interest.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionLog_">The record of all the player of interest's actions</param>
        /// <param name="turnActionTaken_">Has there been at least one player action (bet, call, check or fold) logged for the turn</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="playerOfInterestIndex_">The index of the player of interest for this hand</param>
        /// <param name="currGameId_">The game Id of the current game</param>
        /// <param name="currNumPlayers_">The current number of players at the poker table</param>
        private void TestForTurnCheckAction(DbPlayerHandInfoAll hand_, DataCounter actionLog_, ref bool turnActionTaken_,
            int[] playerChipsInPot_, int[] playerChipStackStart_, int playerOfInterestIndex_, int currGameId_, int currNumPlayers_)
        {
            // No player bet on the turn. Other player(s) remain in the hand who are not all-in
            if (!turnActionTaken_ && 
                !RemainingPlayersAllIn(playerChipsInPot_, playerChipStackStart_, playerOfInterestIndex_, hand_.HandPlayerInfoList.Count))
            {
                // The player of interest is not all-in
                if (playerChipsInPot_[playerOfInterestIndex_] < playerChipStackStart_[playerOfInterestIndex_])
                {
                    // The player of interest checked on the turn
                    AddPokerActionBasedOnHandCircumstances(hand_, actionLog_.Turn, PokerActions.PossibleActions.Check,
                        currGameId_, currNumPlayers_);
                }
                turnActionTaken_ = true;
            }
        }

        /// <summary>
        /// Test to see if there was a check on the river that needs to be logged. If there was log that check action for the player of interest.
        /// </summary>
        /// <param name="hand_">The DbPlayerHandInfoAll for the hand of interest</param>
        /// <param name="actionLog_">The record of all the player of interest's actions</param>
        /// <param name="riverActionTaken_">>Has there been at least one player action (bet, call, check or fold) logged for the river</param>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="playerOfInterestIndex_">The index of the player of interest for this hand</param>
        /// <param name="currGameId_">The game Id of the current game</param>
        /// <param name="currNumPlayers_">The current number of players at the poker table</param>
        private void TestForRiverCheckAction(DbPlayerHandInfoAll hand_, DataCounter actionLog_, ref bool riverActionTaken_,
            int[] playerChipsInPot_, int[] playerChipStackStart_, int playerOfInterestIndex_, int currGameId_, int currNumPlayers_)
        {
            // No player bet on the river. Other player(s) remain in the hand who are not all-in
            if (!riverActionTaken_ && 
                !RemainingPlayersAllIn(playerChipsInPot_, playerChipStackStart_, playerOfInterestIndex_, hand_.HandPlayerInfoList.Count))
            {
                // The player of interest is not all-in
                if (playerChipsInPot_[playerOfInterestIndex_] < playerChipStackStart_[playerOfInterestIndex_])
                {
                    // The player of interest checked on the river
                    AddPokerActionBasedOnHandCircumstances(hand_, actionLog_.River, PokerActions.PossibleActions.Check,
                        currGameId_, currNumPlayers_);
                }
                riverActionTaken_ = true;
            }
        }

        /// <summary>
        /// Check to see if all other players, excluding the specified player, that remain in the hand are all-in.
        /// </summary>
        /// <param name="playerChipsInPot_">The number of chips each player has in the pot [array]</param>
        /// <param name="playerChipStackStart_">The number of chips each player had at the beginning of the hand (before blinds and antes) [array]</param>
        /// <param name="playerIndex_">The index of the player that is excluded from the all-in check</param>
        /// <param name="numPlayers">The number of players in the hand at the beginning of the hand</param>
        /// <returns>True if all the remaining players, excluding the specified player, are all-in</returns>
        private bool RemainingPlayersAllIn(int[] playerChipsInPot__, int[] playerChipStackStart__, int playerIndex__, int numPlayers)
        {
            bool remainingPlayersAllIn = true;

            for (int i = 0; i < numPlayers; i++)
            {
                if (i != playerIndex__)
                {
                    // The current player has the same number of chips in the pot as the player of interest
                    if (playerChipsInPot__[i] == playerChipsInPot__[playerIndex__])
                    {
                        // The current player is not all-in
                        if (playerChipsInPot__[i] < playerChipStackStart__[i])
                        {
                            remainingPlayersAllIn = false;
                        }
                    }
                }
            }

            return remainingPlayersAllIn;
        }

        #endregion

        #region Chart Stuff

        /// <summary>
        /// Plot a single pie chart with the action data contained in the PokerActions object.
        /// </summary>
        /// <param name="roundAndCircumstancesData">The action data for a given betting round and set of circumstances</param>
        /// <param name="chartToUpdate">The chart the data will be plotted on</param>
        /// <param name="chartNoDataLabel">The "No Data" label of the chart the data will be plotted on</param>
        /// <param name="chartSampleCount">The number of data points contained in the PokerActions object</param>
        /// <param name="isChartUpdated">The variable that indicates that the chart data has been updated</param>
        private void PlotChart(PokerActions roundAndCircumstancesData, MyChart chartToUpdate, Label chartNoDataLabel, 
            Label chartSampleCount, ref bool isChartUpdated)
        {
            // The number of data points to be plotted for this betting round and set of circumstances(this table, any table, this number of players, any number of players)
            long dataPointCount = DataSampleCount(roundAndCircumstancesData);

            if (dataPointCount > 0)
            {
                chartToUpdate.Series[0].Points[0].Color = Color.SpringGreen;

                chartToUpdate.Series[0].Points[0].SetValueXY(GetPiePercentage(roundAndCircumstancesData, PokerActions.PossibleActions.Fold),
                    roundAndCircumstancesData.Fold);
                chartToUpdate.Series[0].Points[1].SetValueXY(GetPiePercentage(roundAndCircumstancesData, PokerActions.PossibleActions.Check),
                    roundAndCircumstancesData.Check);
                chartToUpdate.Series[0].Points[2].SetValueXY(GetPiePercentage(roundAndCircumstancesData, PokerActions.PossibleActions.Call),
                    roundAndCircumstancesData.Call);
                chartToUpdate.Series[0].Points[3].SetValueXY(GetPiePercentage(roundAndCircumstancesData, PokerActions.PossibleActions.Bet),
                    roundAndCircumstancesData.Bet);

                chartNoDataLabel.Visible = false;
            }
            else
            {
                NoDataChart(chartToUpdate, chartNoDataLabel);
            }

            chartSampleCount.Text = dataPointCount.ToString();
            isChartUpdated = true;
        }

        /// <summary>
        /// The total number of data points for this betting round and set of circumstances (this table, any table, this number of players, any number of players).
        /// </summary>
        /// <param name="roundAndCircumstancesData">The action data for a given betting round and set of circumstances</param>
        /// <returns>The total number of data points for this betting round and set of circumstances</returns>
        private long DataSampleCount(PokerActions roundAndCircumstancesData)
        {
            return roundAndCircumstancesData.Fold + roundAndCircumstancesData.Check + roundAndCircumstancesData.Call + roundAndCircumstancesData.Bet;
        }

        /// <summary>
        /// Get the percentage occurrence of a specific action type.
        /// </summary>
        /// <param name="act_">The object containing all the poker actions</param>
        /// <param name="intActionIndex">The specific poker action to find the percentage occurrence of</param>
        /// <returns>The percentage occurrence of a specific poker action</returns>
        private string GetPiePercentage(PokerActions act_, PokerActions.PossibleActions actionType)
        {
            string percentageOccurrence = "Error!!!";

            // The total number of poker actions
            long totalNumActions = act_.Fold + act_.Check + act_.Call + act_.Bet;

            if (actionType == PokerActions.PossibleActions.Fold)
            {
                if (act_.Fold > 0)
                {
                    percentageOccurrence = (100.0 * (act_.Fold) / totalNumActions).ToString("f0") + "%";
                }
                else
                {
                    percentageOccurrence = "";
                }
            }
            else if (actionType == PokerActions.PossibleActions.Check)
            {
                if (act_.Check > 0)
                {
                    percentageOccurrence = (100.0 * (act_.Check) / totalNumActions).ToString("f0") + "%";
                }
                else
                {
                    percentageOccurrence = "";
                }
            }
            else if (actionType == PokerActions.PossibleActions.Call)
            {
                if (act_.Call > 0)
                {
                    percentageOccurrence = (100.0 * (act_.Call) / totalNumActions).ToString("f0") + "%";
                }
                else
                {
                    percentageOccurrence = "";
                }
            }
            else if (actionType == PokerActions.PossibleActions.Bet)
            {
                if (act_.Bet > 0)
                {
                    percentageOccurrence = (100.0 * (act_.Bet) / totalNumActions).ToString("f0") + "%";
                }
                else
                {
                    percentageOccurrence = "";
                }
            }

            return percentageOccurrence;
        }

        /// <summary>
        /// Create a pie chart for a betting round and set of circumstances where no data exists.
        /// </summary>
        /// <param name="chartToUpdate_">The chart the data will be plotted on</param>
        /// <param name="chartNoDataLabel_">The "No Data" label of the chart the data will be plotted on</param>
        private void NoDataChart(MyChart chartToUpdate_, Label chartNoDataLabel_)
        {
            chartToUpdate_.Series[0].Points[0].Color = Color.DarkGray;

            chartToUpdate_.Series[0].Points[0].SetValueXY("", 1);
            chartToUpdate_.Series[0].Points[1].SetValueXY("", 0);
            chartToUpdate_.Series[0].Points[2].SetValueXY("", 0);
            chartToUpdate_.Series[0].Points[3].SetValueXY("", 0);

            chartNoDataLabel_.Visible = true;
        }

        /// <summary>
        /// Add the poker actions from a single player to the sum of poker actions.
        /// </summary>
        /// <param name="sumOfActions">The sum of poker actions</param>
        /// <param name="singlePlayerActions">The poker actions of a single player</param>
        private void AddPokerActionsToSum(PokerActions sumOfActions, PokerActions singlePlayerActions)
        {
            sumOfActions.Fold += singlePlayerActions.Fold;
            sumOfActions.Check += singlePlayerActions.Check;
            sumOfActions.Call += singlePlayerActions.Call;
            sumOfActions.Bet += singlePlayerActions.Bet;
        }

        #endregion

        #endregion
    }
}
