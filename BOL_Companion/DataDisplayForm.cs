using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BOL_Companion.DataAnalyzerDataStructures;
using BOL_Companion.DbDataStructures;
using System.Windows.Forms.DataVisualization.Charting;

namespace BOL_Companion
{
    /// <summary>
    /// The form that displays data to assist the user
    /// </summary>
    public partial class DataDisplayForm : Form
    {
        #region Variables

        /// <summary>
        /// The index of the chart for "this table current number of players" within a set of charts.
        /// </summary>
        public static int ThisTableCurrNumPlayersIndex { get; } = 0;

        /// <summary>
        /// The index of the chart for "this table any number of players" within a set of charts.
        /// </summary>
        public static int ThisTableAnyNumPlayersIndex { get; } = 1;

        /// <summary>
        /// The index of the chart for "any table current number of players" within a set of charts.
        /// </summary>
        public static int AnyTableCurrNumPlayersIndex { get; } = 2;

        /// <summary>
        /// The index of the chart for "any table current any of players" within a set of charts.
        /// </summary>
        public static int AnyTableAnyNumPlayersIndex { get; } = 3;

        /// <summary>
        /// The current betting round
        /// </summary>
        BettingRound.Round bettingRound_tsk;

        /// <summary>
        /// The database game ID
        /// </summary>
        int dbGameId_tsk;

        /// <summary>
        /// The current number of players at the table
        /// </summary>
        int currNumPlayers_tsk;

        /// <summary>
        /// The database HandPlayerId of the player of interest
        /// </summary>
        long plrOfInterestDbHandPlayerId;

        /// <summary>
        /// The player of interest's name
        /// </summary>
        string plrOfInterestName;

        /// <summary>
        /// A boolean for each seat indicating if that seat is open (vacant)
        /// </summary>
        bool[] isOpenSeat_tsk;

        /// <summary>
        /// A boolean for each seat indicating if the player in that seat has already folded
        /// </summary>
        bool[] isPlayerFolded_tsk;

        /// <summary>
        /// One boolean variable for each chart indicating whether or not that chart is up to date
        /// </summary>
        bool[][] isChartUpToDate;

        #endregion

        #region Objects

        /// <summary>
        /// The color representing a fold action
        /// </summary>
        Color foldClr;

        /// <summary>
        /// The color representing a check action
        /// </summary>
        Color checkClr;

        /// <summary>
        /// The color representing a call action
        /// </summary>
        Color callClr;

        /// <summary>
        /// The color representing a bet action
        /// </summary>
        Color betClr;

        /// <summary>
        /// The color representing a fold action of an inactive player
        /// </summary>
        Color foldClrFaded;

        /// <summary>
        /// The color representing a check action of an inactive player
        /// </summary>
        Color checkClrFaded;

        /// <summary>
        /// The color representing a call action of an inactive player
        /// </summary>
        Color callClrFaded;

        /// <summary>
        /// The color representing a bet action of an inactive player
        /// </summary>
        Color betClrFaded;

        /// <summary>
        /// A faded white color to use for inactive player labels
        /// </summary>
        Color whiteClrFaded;

        /// <summary>
        /// The background color for rich textboxes
        /// </summary>
        Color rtbBackgroundClr;

        /// <summary>
        /// A stopwatch that tracks how long it took to update the data for all the players excluding the player of interest
        /// </summary>
        Stopwatch dataUpdateStpOthers;

        /// <summary>
        /// A stopwatch that tracks how long it took to update the data for the player of interest
        /// </summary>
        Stopwatch dataUpdateStpPlrOfInterest;

        /// <summary>
        /// An individual data analyzer for each player at the table
        /// </summary>
        DataAnalyzer[] dataAnalyzers;

        /// <summary>
        /// An individual record of a player's actions and hand circumstances for each player at the table
        /// </summary>
        DataCounter[] playerActionData;

        /// <summary>
        /// A task for updating each player's action data
        /// </summary>
        Task[] updateDataTasks;

        #region Controls

        #region Panels

        /// <summary>
        /// The panel that contains all the data on this form
        /// </summary>
        Panel dataPnl;

        /// <summary>
        /// The colored box in the legend showing the fold color
        /// </summary>
        Panel legendFoldClrPnl;

        /// <summary>
        /// The colored box in the legend showing the check color
        /// </summary>
        Panel legendCheckClrPnl;

        /// <summary>
        /// The colored box in the legend showing the call color
        /// </summary>
        Panel legendCallClrPnl;

        /// <summary>
        /// The colored box in the legend showing the bet color
        /// </summary>
        Panel legendBetClrPnl;

        /// <summary>
        /// All of the dataUpdateStatusIndicators (the colored boxes next to the charts indicating if the charts have been updated)
        /// </summary>
        Panel[][] dataUpdateStatusIndicator;

        /// <summary>
        /// All of the dataUpdateStatusIndicatorBorders (the borders of the dataUpdateStatusIndicators)
        /// </summary>
        Panel[][] dataUpdateStatusIndicatorBorder;

        #endregion

        #region Labels

        /// <summary>
        /// The title of this form
        /// </summary>
        Label formTitleLbl;

        /// <summary>
        /// A timestamp displaying the last time the data on the form was updated
        /// </summary>
        Label timeLbl;

        /// <summary>
        /// The fold label inside the legend
        /// </summary>
        Label legendFoldLbl;
        
        /// <summary>
        /// The check label inside the legend
        /// </summary>
        Label legendCheckLbl;
        
        /// <summary>
        /// The call label inside the legend
        /// </summary>
        Label legendCallLbl;

        /// <summary>
        /// The bet lable inside the legend
        /// </summary>
        Label legendBetLbl;

        /// <summary>
        /// The time it took to process the data for all the players except the player of interest
        /// </summary>
        Label processingTimeOthersLbl;
        
        /// <summary>
        /// The time it took to process the data for the player of interest
        /// </summary>
        Label processingTimePlrOfInterestLbl;

        /// <summary>
        /// The labels for the player names
        /// </summary>
        Label[] playerNameLbl;

        /// <summary>
        /// The labels displaying the text "No Data" when there is no data available for a given chart
        /// </summary>
        Label[][] chartNoDataLbl;

        /// <summary>
        /// The labels displaying how many data samples (data points) are included in a given chart
        /// </summary>
        Label[][] chartSampleCountLbl;

        #endregion

        #region Buttons

        /// <summary>
        /// Click this button to see the pre-flop data
        /// </summary>
        Button preFlopBtn;

        /// <summary>
        /// Click this button to see the flop data
        /// </summary>
        Button flopBtn;

        /// <summary>
        /// Click this button to see the turn data
        /// </summary>
        Button turnBtn;

        /// <summary>
        /// Click this button to see the river data
        /// </summary>
        Button riverBtn;

        #endregion

        #region Charts

        /// <summary>
        /// All of the charts on this form (pie charts)
        /// </summary>
        MyChart[][] pieChts;

        #endregion

        #endregion

        #endregion

        #region Setup

        public DataDisplayForm(Color rtbBackgroundClrIn)
        {
            InitializeComponent();

            // Getting the background color of the rich text boxes on the main form to use as the background color for this form
            rtbBackgroundClr = rtbBackgroundClrIn;
        }

        /// <summary>
        /// The load event for the DataDisplayForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataDisplayForm_Load(object sender, EventArgs e)
        {
            // Setup the controls for this form
            SetupDataDisplayForm SetupDataDisplayForm_ = new SetupDataDisplayForm(this);
            SetupDataDisplayForm_.SetFormColors(ref foldClr, ref checkClr, ref callClr, ref betClr,
                ref foldClrFaded, ref checkClrFaded, ref callClrFaded, ref betClrFaded, 
                ref whiteClrFaded, rtbBackgroundClr);
            SetupDataDisplayForm_.InitializeForm();
            SetupDataDisplayForm_.InitializeFormPanels(ref dataPnl, ref legendFoldClrPnl, ref legendCheckClrPnl, ref legendCallClrPnl, 
                ref legendBetClrPnl, ref dataUpdateStatusIndicator, ref dataUpdateStatusIndicatorBorder);
            SetupDataDisplayForm_.InitializeFormLabels(ref formTitleLbl, ref timeLbl, ref legendFoldLbl, ref legendCheckLbl, 
                ref legendCallLbl, ref legendBetLbl, ref processingTimeOthersLbl, ref processingTimePlrOfInterestLbl,
                ref playerNameLbl, ref chartNoDataLbl, ref chartSampleCountLbl);
            SetupDataDisplayForm_.InitializeFormButtons(ref preFlopBtn, ref flopBtn, ref turnBtn, ref riverBtn);
            SetupDataDisplayForm_.InitializeFormCharts(ref pieChts, ref isChartUpToDate);
            SetupDataDisplayForm_.SetControlSizesAndLocations();

            DisplayPreFlopData();

            dataUpdateStpOthers = new Stopwatch();
            dataUpdateStpPlrOfInterest = new Stopwatch();

            dataAnalyzers = new DataAnalyzer[10];
            playerActionData = new DataCounter[10];

            for (int i = 0; i < 10; i++)
            {
                dataAnalyzers[i] = new DataAnalyzer();
                playerActionData[i] = new DataCounter();
            }

            // only 9 tasks (assuming a 9 player table)
            updateDataTasks = new Task[9];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the data on shown on the DataDisplayForm.
        /// </summary>
        /// <param name="isOpenSeat">An array with true values for seats that are open (no player in that seat)</param>
        /// <param name="isPlayerFolded">A boolean indicating if the player in that seat has already folded</param>
        /// <param name="playerDbIds">The players' database IDs</param>
        /// <param name="playerNames">The players' names</param>
        /// <param name="bettingRound_">The current betting round</param>
        /// <param name="dbGameId_">The database game ID</param>
        public void UpdateData(bool[] isOpenSeat, bool[] isPlayerFolded, int[] playerDbIds, string[] playerNames, 
            BettingRound.Round bettingRound_, int dbGameId_)
        {
            // Start the data processing timers
            dataUpdateStpOthers.Start();
            dataUpdateStpPlrOfInterest.Start();

            // Update time stamp
            timeLbl.Text = DateTime.Now.ToString("hh:mm:ss tt");

            currNumPlayers_tsk = GetCurrentNumberOfPlayers(isOpenSeat);

            // Copy method parameters to class level variables to be used later
            isOpenSeat_tsk = isOpenSeat;
            isPlayerFolded_tsk = isPlayerFolded;
            bettingRound_tsk = bettingRound_;
            dbGameId_tsk = dbGameId_;
            
            ChartDataInvalid();

            DisplayCurrentBettingRound(bettingRound_);

            List<DbPlayerHandInfoAll>[] playersDbData_ = QueryDbDataToAnalyze(playerDbIds);

            // Create the data update tasks so they are ready to go when the they are called below
            CreateUpdateDataTasks(playersDbData_);

            // Start all the data analysis tasks
            // plrOfInterestIndex is the index of the player of interest
            int plrOfInterestIndex = StartDataAnalysisTasks(playerNames);

            UpdatePlayersNamesOnCharts(isOpenSeat, playerNames);

            UpdatePlayersData(plrOfInterestIndex);
        }

        /// <summary>
        /// Zero the data on shown on the DataDisplayForm.
        /// </summary>
        public void ZeroData()
        {
            // start data processing timers
            dataUpdateStpOthers.Start();

            ChartDataInvalid();

            // Zero the data for the 9 players at the table
            for (int i = 0; i < 9; i++)
            {
                int chartSetIndex = PlayerIndexToChartSetIndex(i);

                playerNameLbl[chartSetIndex].Text = "Player " + (i + 1).ToString();
                playerActionData[i] = new DataCounter();
                dataAnalyzers[i].PlotChartSet(playerActionData[i].Preflop, pieChts[chartSetIndex], chartNoDataLbl[chartSetIndex], 
                    chartSampleCountLbl[chartSetIndex], isChartUpToDate[chartSetIndex]);

                UpdateChartColors(chartSetIndex, false);
            }

            // Zero the data for the summation of the other players at the table
            playerNameLbl[9].Text = "Other Players at This Table";
            playerActionData[9] = new DataCounter();
            dataAnalyzers[9].PlotChartSet(playerActionData[9].Preflop, pieChts[9], chartNoDataLbl[9], chartSampleCountLbl[9], isChartUpToDate[9]);

            // Update time stamp
            timeLbl.Text = DateTime.Now.ToString("hh:mm:ss tt");

            // Update data processing timer
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();
        }

        /// <summary>
        /// Pass information about the player of interest to the DataDisplayForm.
        /// </summary>
        /// <param name="plrName">The name of the player of interest</param>
        /// <param name="plrDbHandPlayerId">The HandPlayerId of the player of interest</param>
        public void PlayerOfInterest(string plrName, long plrDbHandPlayerId)
        {
            plrOfInterestName = plrName;
            plrOfInterestDbHandPlayerId = plrDbHandPlayerId;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Display the players' preflop data on the DataDisplayForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void preFlopBtn_Click(object sender, EventArgs e)
        {
            // start data processing timer
            dataUpdateStpOthers.Start();

            // If isPlayerFolded_tsk is null the DataDisplayForm has not been updated yet and all data is blank
            if (isPlayerFolded_tsk != null)
            {
                ChartDataInvalid();
                DisplayPreFlopData();

                for (int i = 0; i < 10; i++)
                {
                    int chartSetIndex = PlayerIndexToChartSetIndex(i);

                    dataAnalyzers[i].PlotChartSet(playerActionData[i].Preflop, pieChts[chartSetIndex], chartNoDataLbl[chartSetIndex],
                        chartSampleCountLbl[chartSetIndex], isChartUpToDate[chartSetIndex]);

                    // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                    UpdateChartColors(chartSetIndex, isPlayerFolded_tsk[i]);
                }
            }
            else
            {
                DisplayPreFlopData();
            }

            // Update data processing timer
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();
        }

        /// <summary>
        /// Display the players' flop data on the DataDisplayForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void flopBtn_Click(object sender, EventArgs e)
        {
            // start data processing timer
            dataUpdateStpOthers.Start();

            // If isPlayerFolded_tsk is null the DataDisplayForm has not been updated yet and all data is blank
            if (isPlayerFolded_tsk != null)
            {
                ChartDataInvalid();
                DisplayFlopData();

                for (int i = 0; i < 10; i++)
                {
                    int chartSetIndex = PlayerIndexToChartSetIndex(i);

                    dataAnalyzers[i].PlotChartSet(playerActionData[i].Flop, pieChts[chartSetIndex], chartNoDataLbl[chartSetIndex],
                        chartSampleCountLbl[chartSetIndex], isChartUpToDate[chartSetIndex]);

                    // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                    UpdateChartColors(chartSetIndex, isPlayerFolded_tsk[i]);
                }
            }
            else
            {
                DisplayFlopData();
            }

            // Update data processing timer
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();
        }

        /// <summary>
        /// Display the players' turn data on the DataDisplayForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void turnBtn_Click(object sender, EventArgs e)
        {
            // start data processing timer
            dataUpdateStpOthers.Start();

            // If isPlayerFolded_tsk is null the DataDisplayForm has not been updated yet and all data is blank
            if (isPlayerFolded_tsk != null)
            {
                ChartDataInvalid();
                DisplayTurnData();

                for (int i = 0; i < 10; i++)
                {
                    int chartSetIndex = PlayerIndexToChartSetIndex(i);

                    dataAnalyzers[i].PlotChartSet(playerActionData[i].Turn, pieChts[chartSetIndex], chartNoDataLbl[chartSetIndex],
                        chartSampleCountLbl[chartSetIndex], isChartUpToDate[chartSetIndex]);

                    // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                    UpdateChartColors(chartSetIndex, isPlayerFolded_tsk[i]);
                }
            }
            else
            {
                DisplayTurnData();
            }

            // Update data processing timer
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();
        }

        /// <summary>
        /// Display the players' river data on the DataDisplayForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void riverBtn_Click(object sender, EventArgs e)
        {
            // start data processing timer
            dataUpdateStpOthers.Start();

            // If isPlayerFolded_tsk is null the DataDisplayForm has not been updated yet and all data is blank
            if (isPlayerFolded_tsk != null)
            {
                ChartDataInvalid();
                DisplayRiverData();

                for (int i = 0; i < 10; i++)
                {
                    int chartSetIndex = PlayerIndexToChartSetIndex(i);

                    dataAnalyzers[i].PlotChartSet(playerActionData[i].River, pieChts[chartSetIndex], chartNoDataLbl[chartSetIndex],
                        chartSampleCountLbl[chartSetIndex], isChartUpToDate[chartSetIndex]);

                    // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                    UpdateChartColors(chartSetIndex, isPlayerFolded_tsk[i]);
                }
            }
            else
            {
                DisplayRiverData();
            }

            // Update data processing timer
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": "+ (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();
        }

        /// <summary>
        /// Update the chart updated indicator color to green because the chart data has been updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chartQuad_PostPaint(object sender, ChartPaintEventArgs e)
        {
            MyChart cht = sender as MyChart;

            if (isChartUpToDate[cht.I][cht.J])
            {
                if (isPlayerFolded_tsk != null && isPlayerFolded_tsk[PlayerIndexToDataDisplayFormIndex(cht.I)])
                {
                    dataUpdateStatusIndicator[cht.I][cht.J].BackColor = foldClrFaded;
                }
                else
                {
                    dataUpdateStatusIndicator[cht.I][cht.J].BackColor = foldClr;
                }
                isChartUpToDate[cht.I][cht.J] = false;
            }
        }

        /// <summary>
        /// This event is needed for the dataPnl control. It is not used here but SetupDataDisplayForm.cs refers to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataDisplayForm_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Start the data analysis tasks for all the players.
        /// </summary>
        /// <param name="playerNames_">The player's names</param>
        /// <returns>The index of the player of interest</returns>
        private int StartDataAnalysisTasks(string[] playerNames_)
        {
            // The index of the player of interest
            int plrOfInterestIndex_ = -1;

            // The player's index on the DataDisplayForm where the player of interest, who in position 6 at the poker table, is in position 0 on
            // the DataDisplayForm
            int DataDisplayFormIndex;

            // Start tasks for all the players that are still in the hand (not folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                if (playerNames_[i] == "")
                {
                    playerNames_[i] = "Player " + (i + 1).ToString();
                }

                DataDisplayFormIndex = PlayerIndexToDataDisplayFormIndex(i);

                // Find out which player (if any) is the player of interest.
                // The player of interest's data will be processed last in order to improve performance because the player of interest may have 
                // much more data available to analyze.
                if (playerNames_[DataDisplayFormIndex].ToUpper() == plrOfInterestName.ToUpper())
                {
                    plrOfInterestIndex_ = DataDisplayFormIndex;
                }

                if (!isPlayerFolded_tsk[DataDisplayFormIndex] && DataDisplayFormIndex != plrOfInterestIndex_)
                {
                    // Make sure the task is already created and not currently running to avoid exceptions although this should never happen
                    if (updateDataTasks[DataDisplayFormIndex].Status == TaskStatus.Created && updateDataTasks[DataDisplayFormIndex].Status != TaskStatus.Running)
                    {
                        updateDataTasks[DataDisplayFormIndex].Start();
                    }
                    UpdateChartColors(PlayerIndexToChartSetIndex(DataDisplayFormIndex), false);
                }
                // If the player has already folded
                else if (DataDisplayFormIndex != plrOfInterestIndex_)
                {
                    UpdateChartColors(PlayerIndexToChartSetIndex(DataDisplayFormIndex), true);
                }
            }

            // Start tasks for all the players that are no longer in the hand (folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                DataDisplayFormIndex = PlayerIndexToDataDisplayFormIndex(i);

                if (isPlayerFolded_tsk[DataDisplayFormIndex] && DataDisplayFormIndex != plrOfInterestIndex_)
                {
                    // Make sure the task is already created and not currently running to avoid exceptions although this should never happen
                    if (updateDataTasks[DataDisplayFormIndex].Status == TaskStatus.Created && updateDataTasks[DataDisplayFormIndex].Status != TaskStatus.Running)
                    {
                        updateDataTasks[DataDisplayFormIndex].Start();
                    }
                }
            }

            // Start the task for the player of interst last to improve performance because the player of interest may have much more data
            // available to analyze.

            // Make sure the task is already created and not currently running to avoid exceptions although this should never happen
            if (updateDataTasks[plrOfInterestIndex_].Status == TaskStatus.Created && updateDataTasks[plrOfInterestIndex_].Status != TaskStatus.Running)
            {
                updateDataTasks[plrOfInterestIndex_].Start();
            }

            return plrOfInterestIndex_;
        }

        /// <summary>
        /// Update the player name labels above the chart sets on the DataDisplayForm.
        /// </summary>
        /// <param name="isOpenSeat_">An array with true values for seats that are open (no player in that seat)</param>
        /// <param name="playerNames_">The player's names</param>
        private void UpdatePlayersNamesOnCharts(bool[] isOpenSeat_, string[] playerNames_)
        {
            // The index of the chart set (corresponding to one of the 10 locations for data to be displayed on the DataDisplayForm)
            int chartSetIndex;

            // Note: I am assuming a 9 player table here
            for (int i = 0; i < 9; i++)
            {
                chartSetIndex = PlayerIndexToChartSetIndex(i);

                if (isOpenSeat_[i])
                {
                    playerNameLbl[chartSetIndex].Text = "Open Seat";
                }
                else
                {
                    playerNameLbl[chartSetIndex].Text = playerNames_[i];
                }
            }

            // Chart set number 10 displays the summation of the data for all players at the table excluding the player of interest
            playerNameLbl[9].Text = "Other Players at This Table";
        }

        /// <summary>
        /// Update the player's data on the DataDisplayForm with the data produced by the update data tasks.
        /// </summary>
        /// <param name="plrOfInterestIndex_">The index of the player of interest</param>
        private async void UpdatePlayersData(int plrOfInterestIndex_)
        {
            // A player's DataDisplayForm Index
            int playerDataDisplayFormIndex;

            // Update data for all the players still in this hand (not folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                playerDataDisplayFormIndex = PlayerIndexToDataDisplayFormIndex(i);

                if (!isPlayerFolded_tsk[playerDataDisplayFormIndex] && playerDataDisplayFormIndex != plrOfInterestIndex_)
                {
                    await updateDataTasks[playerDataDisplayFormIndex];
                    UpdatePlayersCharts(playerDataDisplayFormIndex, false);
                }
            }

            // Update data for all the players not still in this hand (folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                playerDataDisplayFormIndex = PlayerIndexToDataDisplayFormIndex(i);

                if (isPlayerFolded_tsk[playerDataDisplayFormIndex] && playerDataDisplayFormIndex != plrOfInterestIndex_)
                {
                    await updateDataTasks[playerDataDisplayFormIndex];
                    UpdatePlayersCharts(playerDataDisplayFormIndex, true);
                }
            }

            // Now that all the data for the other players has been updated update the summation of all other players data
            playerActionData[9] = dataAnalyzers[9].SumPlayerData(playerActionData, plrOfInterestIndex_);
            UpdatePlayersCharts(9, false);

            // Update data processing timer for other players
            dataUpdateStpOthers.Stop();
            processingTimeOthersLbl.Text = "Others: " + (dataUpdateStpOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpOthers.Reset();


            // Update the data for the player of interest last because there might be more information to process and it may take more time
            await updateDataTasks[plrOfInterestIndex_];
            UpdatePlayersCharts(plrOfInterestIndex_, false);

            // Update data processing timer for the player of interest
            dataUpdateStpPlrOfInterest.Stop();
            processingTimePlrOfInterestLbl.Text = plrOfInterestName + ": " + (dataUpdateStpPlrOfInterest.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            dataUpdateStpPlrOfInterest.Reset();
        }

        /// <summary>
        /// Display the players' pre-flop action data
        /// </summary>
        private void DisplayPreFlopData()
        {
            formTitleLbl.Text = "Pre-Flop";
            formTitleLbl.Location = new Point(ClientSize.Width / 2 - formTitleLbl.Width / 2, 10);

            EnableDisableButton(preFlopBtn, false);
            EnableDisableButton(flopBtn, true);
            EnableDisableButton(turnBtn, true);
            EnableDisableButton(riverBtn, true);
        }

        /// <summary>
        /// Display the players' flop action data
        /// </summary>
        private void DisplayFlopData()
        {
            formTitleLbl.Text = "Flop";
            formTitleLbl.Location = new Point(ClientSize.Width / 2 - formTitleLbl.Width / 2, 10);

            EnableDisableButton(preFlopBtn, true);
            EnableDisableButton(flopBtn, false);
            EnableDisableButton(turnBtn, true);
            EnableDisableButton(riverBtn, true);
        }

        /// <summary>
        /// Display the players' turn action data
        /// </summary>
        private void DisplayTurnData()
        {
            formTitleLbl.Text = "Turn";
            formTitleLbl.Location = new Point(ClientSize.Width / 2 - formTitleLbl.Width / 2, 10);

            EnableDisableButton(preFlopBtn, true);
            EnableDisableButton(flopBtn, true);
            EnableDisableButton(turnBtn, false);
            EnableDisableButton(riverBtn, true);
        }

        /// <summary>
        /// Display the players' river action data
        /// </summary>
        private void DisplayRiverData()
        {
            formTitleLbl.Text = "River";
            formTitleLbl.Location = new Point(ClientSize.Width / 2 - formTitleLbl.Width / 2, 10);

            EnableDisableButton(preFlopBtn, true);
            EnableDisableButton(flopBtn, true);
            EnableDisableButton(turnBtn, true);
            EnableDisableButton(riverBtn, false);
        }

        /// <summary>
        /// Display the players' action data for the current betting round.
        /// </summary>
        /// <param name="currentBettingRound">The current betting round</param>
        private void DisplayCurrentBettingRound(BettingRound.Round currentBettingRound)
        {
            if (currentBettingRound == BettingRound.Round.PreFlop)
            {
                DisplayPreFlopData();
            }
            else if (currentBettingRound == BettingRound.Round.Flop)
            {
                DisplayFlopData();
            }
            else if (currentBettingRound == BettingRound.Round.Turn)
            {
                DisplayTurnData();
            }
            else
            {
                DisplayRiverData();
            }
        }

        /// <summary>
        /// Get the current number of players at the table.
        /// </summary>
        /// <param name="isOpenSeat_">An array with true values for seats that are open (no player in that seat)</param>
        /// <returns>The number of players at the table</returns>
        private int GetCurrentNumberOfPlayers(bool[] isOpenSeat_)
        {
            int numPlayers = 0;
            
            // Note: I am going to assume a 9 player table here
            for (int i = 0; i < 9; i++)
            {
                if (!isOpenSeat_[i])
                {
                    numPlayers++;
                }
            }

            return numPlayers;
        }

        /// <summary>
        /// Change the a button's enabled property and colors to the enabled or disabled state.
        /// </summary>
        /// <param name="btn">The button to enable or disable</param>
        /// <param name="enable">The desired state of the button (true = enable, false = disable)</param>
        private void EnableDisableButton(Button btn, bool enable)
        {
            btn.Enabled = enable;

            if (enable)
            {
                btn.ForeColor = Color.Black;
                btn.BackColor = Color.DarkGray;
            }
            else
            {
                btn.ForeColor = Color.DarkGray;
                btn.BackColor = Color.LightGray;
            }
        }

        /// <summary>
        /// Changes the color of the dataUpdateStatusIndicator squares to red indicating the data has not been updated.
        /// </summary>
        private void ChartDataInvalid()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (isPlayerFolded_tsk != null && isPlayerFolded_tsk[PlayerIndexToDataDisplayFormIndex(i)])
                    {
                        // If this player has already folded use a fadded red color
                        dataUpdateStatusIndicator[i][j].BackColor = betClrFaded;
                    }
                    else
                    {
                        dataUpdateStatusIndicator[i][j].BackColor = betClr;
                    }
                }
            }
        }

        /// <summary>
        /// Update a player's four data charts.
        /// </summary>
        /// <param name="intPlayerIndex">The index of the player whose data charts you wish to update</param>
        /// <param name="blnFolded">True if the player has folded</param>
        private void UpdatePlayersCharts(int intPlayerIndex, bool blnFolded)
        {
            int intChartSetIndex = PlayerIndexToChartSetIndex(intPlayerIndex);

            // Update charts with player data based on what betting stage we are in
            if (bettingRound_tsk == BettingRound.Round.PreFlop)
            {
                dataAnalyzers[intPlayerIndex].PlotChartSet(playerActionData[intPlayerIndex].Preflop, pieChts[intChartSetIndex], 
                    chartNoDataLbl[intChartSetIndex], chartSampleCountLbl[intChartSetIndex], isChartUpToDate[intChartSetIndex]);
            }
            else if (bettingRound_tsk == BettingRound.Round.Flop)
            {
                dataAnalyzers[intPlayerIndex].PlotChartSet(playerActionData[intPlayerIndex].Flop, pieChts[intChartSetIndex],
                    chartNoDataLbl[intChartSetIndex], chartSampleCountLbl[intChartSetIndex], isChartUpToDate[intChartSetIndex]);
            }
            else if (bettingRound_tsk == BettingRound.Round.Turn)
            {
                dataAnalyzers[intPlayerIndex].PlotChartSet(playerActionData[intPlayerIndex].Turn, pieChts[intChartSetIndex],
                    chartNoDataLbl[intChartSetIndex], chartSampleCountLbl[intChartSetIndex], isChartUpToDate[intChartSetIndex]);
            }
            else
            {
                dataAnalyzers[intPlayerIndex].PlotChartSet(playerActionData[intPlayerIndex].River, pieChts[intChartSetIndex],
                    chartNoDataLbl[intChartSetIndex], chartSampleCountLbl[intChartSetIndex], isChartUpToDate[intChartSetIndex]);
            }

            // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
            // This could occur when the number of players at the table changes.
            UpdateChartColors(intChartSetIndex, blnFolded);
        }

        /// <summary>
        /// Converts the player index to the chart set index. This is used so the data for the player of interest will always be displayed in the first position. This assumes the player of interest is always seated in the 7th seat (index 6) [just because the 7th seat is a seat I personally like].
        /// </summary>
        /// <param name="intPlayerIndex">The index of the player</param>
        /// <returns>The index of the chart set</returns>
        private int PlayerIndexToChartSetIndex(int intPlayerIndex)
        {
            int intChartSetIndex;

            if (intPlayerIndex != 9)
            {
                intChartSetIndex = intPlayerIndex + 3;

                if (intChartSetIndex > 8)
                {
                    intChartSetIndex -= 9;
                }
            }
            else
            {
                intChartSetIndex = intPlayerIndex;
            }

            return intChartSetIndex;
        }

        /// <summary>
        /// Converts player indicies so they increment from 6 to 5 instead of from 0 to 8.
        /// </summary>
        /// <param name="intPlayerIndex">The index you wish to convert</param>
        /// <returns>The converted index</returns>
        private int PlayerIndexToDataDisplayFormIndex(int intPlayerIndex)
        {
            int j;

            j = intPlayerIndex + 6;

            if (j > 8)
            {
                j -= 9;
            }

            return j;
        }

        /// <summary>
        /// Changes the colors of a set of charts to either the normal version or the faded version.
        /// </summary>
        /// <param name="intChartSetIndex_">The chart set to be updated</param>
        /// <param name="blnFadedColors">True if the chart colors should be fadded</param>
        private void UpdateChartColors(int intChartSetIndex_, bool blnFadedColors)
        {
            Color clr0, clr1, clr2, clr3, clr4;

            if (!blnFadedColors)
            {
                clr0 = foldClr;
                clr1 = checkClr;
                clr2 = callClr;
                clr3 = betClr;
                clr4 = Color.White;
            }
            else
            {
                clr0 = foldClrFaded;
                clr1 = checkClrFaded;
                clr2 = callClrFaded;
                clr3 = betClrFaded;
                clr4 = whiteClrFaded;
            }

            playerNameLbl[intChartSetIndex_].ForeColor = clr4;

            for (int i = 0; i < 4; i++)
            {
                pieChts[intChartSetIndex_][i].Titles[0].ForeColor = clr4;

                if (chartNoDataLbl[intChartSetIndex_][i].Visible == false)
                {
                    pieChts[intChartSetIndex_][i].Series[0].Points[0].Color = clr0;
                    pieChts[intChartSetIndex_][i].Series[0].Points[1].Color = clr1;
                    pieChts[intChartSetIndex_][i].Series[0].Points[2].Color = clr2;
                    pieChts[intChartSetIndex_][i].Series[0].Points[3].Color = clr3;
                }
            }
        }

        #region Task Related Helper Methods

        /// <summary>
        /// Create the tasks for updating the player data on the Data Display Form. (One task for each of a possible 9 players)
        /// </summary>
        private void CreateUpdateDataTasks(List<DbPlayerHandInfoAll>[] playersDbDataToAnalyze)
        {
            for (int i = 0; i < 9; i++)
            {
                int j = i;
                if (updateDataTasks[j] == null || updateDataTasks[j].Status != TaskStatus.Created)
                {
                    updateDataTasks[j] = new Task(() =>
                    {
                        // If this is an open seat don't update the data -> pass an empty cntPlayerData object
                        // Note: If the player is sitting out continue to process data as if he is playing. Things get messed up and
                        //       too complicated when trying to ignore players sitting out, not worth the effort.
                        // Note: intBoardStatus_tsk determines what betting stage we are in (0 = preflop, 1 = flop, 2 = turn, 3 = river)
                        if (isOpenSeat_tsk[j])
                        {
                            playerActionData[j] = new DataCounter();
                        }
                        else
                        {
                            playerActionData[j] = dataAnalyzers[j].AnalyzePlayerData(playersDbDataToAnalyze[j], dbGameId_tsk, currNumPlayers_tsk, plrOfInterestDbHandPlayerId);
                        }

                        // Debug.WriteLine("Completing task #" + j.ToString() + " - " + playerNames_tsk[j].ToString());
                    });
                }
            }
        }

        /// <summary>
        /// Query the database for all the data that will be analzied for all the players at the table.
        /// </summary>
        /// <param name="playerDbIds_">The database Id's of all the player's at the table</param>
        private List<DbPlayerHandInfoAll>[] QueryDbDataToAnalyze(int[] playerDbIds_)
        {
            // I use a single DB context to do the substantial amount of DB querying to get all the data that will be analyzed for all the
            // players. I believe using a singe DB context to do all of this querying, which involves requesting much of the same data for 
            // multiple tasks, will be more efficient if done with a single DB context.

            DbController dbController = new DbController();
            Bol_Model_DBEntities ctxQueryContext = dbController.GetDbContext();
            List<DbPlayerHandInfoAll>[] playersDbData = new List<DbPlayerHandInfoAll>[9];
            int maxNumberOfHandsToLookUp = 300;

            using (ctxQueryContext)
            {
                for (int i = 0; i < 9; i++)
                {
                    playersDbData[i] = dbController.QueryPlayerHandsInfoAll_query(
                        dbController.QueryPlayerHandsInfo(playerDbIds_[i], maxNumberOfHandsToLookUp, ctxQueryContext), ctxQueryContext);
                }
            }

            return playersDbData;
        }

        #endregion

        #endregion
    }
}
