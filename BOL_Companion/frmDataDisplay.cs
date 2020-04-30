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
using System.Windows.Forms.DataVisualization.Charting;

namespace BOL_Companion
{
    public partial class frmDataDisplay : Form
    {
        #region Variables and Objects

        int intBoardStatus_tsk, intDbGameId_tsk, intCurrNumPlayers_tsk;
        long lngPlrOfInterestDbHandPlayerId;
        string strPlrOfInterest;
        bool[] blnOpenSeat_tsk, blnPlayerFolded_tsk;
        int[] intDbId_tsk;
        string[] strName_tsk;

        public bool[][] blnChtUpdated;
        public Color clrFold, clrFold_Fadded, clrCheck, clrCheck_Fadded, clrCall, 
            clrCall_Fadded, clrBet, clrBet_Fadded, clrWhite_Fadded;

        Color clrRtbBackground;
        Stopwatch stpDataUpdateTimeOthers, stpDataUpdateTimePlrOfInterest;

        SetupFrmDataDisplay SetupFrmDataDisplay_;
        DbController db_PlayerHandInfoQuery;
        DbController[] db_;
        DataAnalyzer[] daz;
        DataAnalyzer.DataCounter[] cntPlayerData;
        List<DbController.DbPlayerHandInfoAll>[] hiaPlayerHandInfoAll;
        Task[] tskUpdateData;

        #endregion

        #region Controls

        public Label lblFrmTitle, lblLegendFold, lblLegendCheck, lblLegendCall, lblLegendBet, lblTime, 
            lblProcessingTimeOthers, lblProcessingTimePlrOfInterest;
        public Label[] lblPlayerName;
        public Label[][] lblChartNoData, lblChartSampleCount;
        public Button btnPreFlop, btnFlop, btnTurn, btnRiver;
        public Panel pnlData, pnlLegendFold, pnlLegendCheck, pnlLegendCall, pnlLegendBet;
        public Panel[][] pnlDataUpdateStatus, pnlDataUpdateStatusBorder;
        public MyChart[][] chtQuad;

        #endregion

        #region Setup

        public frmDataDisplay(Color clrRtbBackgroundIn)
        {
            InitializeComponent();
            // Getting the background color of the rtbs on the main form to use as the background color for this form
            clrRtbBackground = clrRtbBackgroundIn;
        }

        private void frmDataDisplay_Load(object sender, EventArgs e)
        {
            SetupFrmDataDisplay_ = new SetupFrmDataDisplay(this, clrRtbBackground);

            stpDataUpdateTimeOthers = new Stopwatch();
            stpDataUpdateTimePlrOfInterest = new Stopwatch();
            db_PlayerHandInfoQuery = new DbController();
            db_ = new DbController[10];
            daz = new DataAnalyzer[10];
            cntPlayerData = new DataAnalyzer.DataCounter[10];

            for (int i = 0; i < 10; i++)
            {
                db_[i] = new DbController();
                daz[i] = new DataAnalyzer();
                cntPlayerData[i] = new DataAnalyzer.DataCounter();
            }

            // only 9 tasks (assuming a 9 player table)
            tskUpdateData = new Task[9];
            hiaPlayerHandInfoAll = new List<DbController.DbPlayerHandInfoAll>[9];
        }

        #endregion

        #region Public Methods

        public async void UpdateData(bool[] blnOpenSeat, bool[] blnPlayerFolded, int[] intDbId, string[] strName, int intBoardStatus_, int intDbGameId_)
        {
            // start data processing timers
            stpDataUpdateTimeOthers.Start();
            stpDataUpdateTimePlrOfInterest.Start();

            int intChartSetIndex;
            int intIndexFrom6;
            int intPlrOfInterestIndex;

            // Update time stamp
            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");

            // Note: I am going to assume a 9 player table here
            // Get the current number of players in the hand
            intCurrNumPlayers_tsk = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!blnOpenSeat[i])
                {
                    intCurrNumPlayers_tsk++;
                }
            }

            blnOpenSeat_tsk = blnOpenSeat;
            blnPlayerFolded_tsk = blnPlayerFolded;
            intDbId_tsk = intDbId;
            strName_tsk = strName;
            intBoardStatus_tsk = intBoardStatus_;
            intDbGameId_tsk = intDbGameId_;
            
            ChartDataInvalid();

            if (intBoardStatus_ == 0)
            {
                VisualPreFlop();
            }
            else if (intBoardStatus_ == 1)
            {
                VisualFlop();
            }
            else if (intBoardStatus_ == 2)
            {
                VisualTurn();
            }
            else
            {
                VisualRiver();
            }

            // Create the data update tasks so they are ready to go when the they are called below. Note, I am creating the tasks here instead
            // of re-creating the tasks immediately after they run because I want to use a single DB context to do the substantial amount of 
            // DB querying to get the data that these tasks require. I believe using a singe DB context to do all of this querying, which 
            // involves requesting much of the same data for multiple tasks, will be much more efficient if done with a single DB context.
            CreateUpdateDataTasks();

            intPlrOfInterestIndex = -1;

            // Start tasks for all the players that are still in the hand (not folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                if (strName_tsk[i] == "")
                {
                    strName_tsk[i] = "Player " + (i + 1).ToString();
                }

                intIndexFrom6 = IndexFrom6(i);

                // Find out which player is (if any) is the player of interest. The player of interest's data will be processed last in order to improve 
                // performance because the player of interest may have much more data to look up and process.
                if (strName_tsk[intIndexFrom6].ToUpper() == strPlrOfInterest.ToUpper())
                {
                    intPlrOfInterestIndex = intIndexFrom6;
                }

                if (!blnPlayerFolded_tsk[intIndexFrom6] && intIndexFrom6 != intPlrOfInterestIndex)
                {
                    if (tskUpdateData[intIndexFrom6].Status == TaskStatus.Created && tskUpdateData[intIndexFrom6].Status != TaskStatus.Running)
                    {
                        // Debug.WriteLine("Launching task #" + intIndexFrom6.ToString() + " - " + strName_tsk[intIndexFrom6].ToString() + "(" + intDbId_tsk[intIndexFrom6].ToString() + ")");
                        tskUpdateData[intIndexFrom6].Start();
                    }
                    // Debug.WriteLine(strName_tsk[intIndexFrom6].ToString() + " - charts normal colors");
                    UpdateChartColors(PlayerIndexToChartSetIndex(intIndexFrom6), false);
                }
                else if (intIndexFrom6 != intPlrOfInterestIndex)
                {
                    // Debug.WriteLine(strName_tsk[intIndexFrom6].ToString() + " - charts fadded colors");
                    UpdateChartColors(PlayerIndexToChartSetIndex(intIndexFrom6), true);
                }
            }

            // Start tasks for all the players that are no longer in the hand (folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                intIndexFrom6 = IndexFrom6(i);

                if (blnPlayerFolded_tsk[intIndexFrom6] && intIndexFrom6 != intPlrOfInterestIndex)
                {
                    if (tskUpdateData[intIndexFrom6].Status == TaskStatus.Created && tskUpdateData[intIndexFrom6].Status != TaskStatus.Running)
                    {
                        // Debug.WriteLine("Launching task #" + intIndexFrom6.ToString() + " - " + strName_tsk[intIndexFrom6].ToString() + "(" + intDbId_tsk[intIndexFrom6].ToString() + ")");
                        tskUpdateData[intIndexFrom6].Start();
                    }
                }
            }

            // Start the task for the player of interst last because there might be more information to process and it may take more time
            if (tskUpdateData[intPlrOfInterestIndex].Status == TaskStatus.Created && tskUpdateData[intPlrOfInterestIndex].Status != TaskStatus.Running)
            {
                // Debug.WriteLine("Launching task #" + intPlrOfInterestIndex.ToString() + " - " + strName_tsk[intPlrOfInterestIndex].ToString() + "(" + intDbId_tsk[intPlrOfInterestIndex].ToString() + ")");
                tskUpdateData[intPlrOfInterestIndex].Start();
            }

            // Note: I am assuming a 9 player table here
            for (int i = 0; i < 9; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                if (blnOpenSeat[i])
                {
                    lblPlayerName[intChartSetIndex].Text = "Open Seat";
                }
                else
                {
                    lblPlayerName[intChartSetIndex].Text = strName_tsk[i];
                }
            }

            lblPlayerName[9].Text = "Other Players at This Table";

            // Do everything for all the players still in this hand (not folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                intIndexFrom6 = IndexFrom6(i);

                if (!blnPlayerFolded_tsk[intIndexFrom6] && intIndexFrom6 != intPlrOfInterestIndex)
                {
                    await tskUpdateData[intIndexFrom6];
                    // Debug.WriteLine("Running UI for task #" + intIndexFrom6.ToString() + " - " + strName_tsk[intIndexFrom6].ToString());
                    UpdatePlayersCharts(intIndexFrom6, false);
                }
            }

            // Do everything for all the players not still in this hand (folded) except the player of interest
            for (int i = 0; i < 9; i++)
            {
                intIndexFrom6 = IndexFrom6(i);

                if (blnPlayerFolded_tsk[intIndexFrom6] && intIndexFrom6 != intPlrOfInterestIndex)
                {
                    await tskUpdateData[intIndexFrom6];
                    // Debug.WriteLine("Running UI for task #" + intIndexFrom6.ToString() + " - " + strName_tsk[intIndexFrom6].ToString());
                    UpdatePlayersCharts(intIndexFrom6, true);
                }
            }

            // Now that all the other player data has been updated update the sumation of all other player data charts
            cntPlayerData[9] = daz[9].SumPlayerData(cntPlayerData, intPlrOfInterestIndex);
            UpdatePlayersCharts(9, false);

            // Update data processing timer for other players
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();


            // Update the data for the player of interest last because there might be more information to process and it may take more time
            await tskUpdateData[intPlrOfInterestIndex];
            // Debug.WriteLine("Running UI for task #" + intPlrOfInterestIndex.ToString() + " - " + strName_tsk[intPlrOfInterestIndex].ToString());
            UpdatePlayersCharts(intPlrOfInterestIndex, false);

            stpDataUpdateTimePlrOfInterest.Stop();
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": " + (stpDataUpdateTimePlrOfInterest.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimePlrOfInterest.Reset();
        }

        public void ZeroData()
        {
            // start data processing timers
            stpDataUpdateTimeOthers.Start();

            int intChartSetIndex;

            ChartDataInvalid();

            for (int i = 0; i < 9; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                lblPlayerName[intChartSetIndex].Text = "Player " + (i + 1).ToString();
                cntPlayerData[i] = new DataAnalyzer.DataCounter();
                daz[i].PlotPlayerDataPreFlop(intChartSetIndex, cntPlayerData[i], this);

                // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                UpdateChartColors(intChartSetIndex, false);
            }

            lblPlayerName[9].Text = "Other Players at This Table";
            cntPlayerData[9] = new DataAnalyzer.DataCounter();
            daz[9].PlotPlayerDataPreFlop(9, cntPlayerData[9], this);

            // Update time stamp
            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");

            // Update data processing timer
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();
        }

        public void PlayerOfInterest(string strPlr, long lngPlrDbHandPlayerId)
        {
            strPlrOfInterest = strPlr;
            lngPlrOfInterestDbHandPlayerId = lngPlrDbHandPlayerId;
        }

        public void VisualPreFlop()
        {
            lblFrmTitle.Text = "Pre-Flop";
            lblFrmTitle.Location = new Point(ClientSize.Width / 2 - lblFrmTitle.Width / 2, 10);

            btnPreFlop.Enabled = false;
            btnPreFlop.ForeColor = Color.DarkGray;
            btnPreFlop.BackColor = Color.LightGray;

            btnFlop.Enabled = true;
            btnFlop.ForeColor = Color.Black;
            btnFlop.BackColor = Color.DarkGray;

            btnTurn.Enabled = true;
            btnTurn.ForeColor = Color.Black;
            btnTurn.BackColor = Color.DarkGray;

            btnRiver.Enabled = true;
            btnRiver.ForeColor = Color.Black;
            btnRiver.BackColor = Color.DarkGray;
        }

        public void VisualFlop()
        {
            lblFrmTitle.Text = "Flop";
            lblFrmTitle.Location = new Point(ClientSize.Width / 2 - lblFrmTitle.Width / 2, 10);

            btnPreFlop.Enabled = true;
            btnPreFlop.ForeColor = Color.Black;
            btnPreFlop.BackColor = Color.DarkGray;

            btnFlop.Enabled = false;
            btnFlop.ForeColor = Color.DarkGray;
            btnFlop.BackColor = Color.LightGray;

            btnTurn.Enabled = true;
            btnTurn.ForeColor = Color.Black;
            btnTurn.BackColor = Color.DarkGray;

            btnRiver.Enabled = true;
            btnRiver.ForeColor = Color.Black;
            btnRiver.BackColor = Color.DarkGray;
        }

        public void VisualTurn()
        {
            lblFrmTitle.Text = "Turn";
            lblFrmTitle.Location = new Point(ClientSize.Width / 2 - lblFrmTitle.Width / 2, 10);

            btnPreFlop.Enabled = true;
            btnPreFlop.ForeColor = Color.Black;
            btnPreFlop.BackColor = Color.DarkGray;

            btnFlop.Enabled = true;
            btnFlop.ForeColor = Color.Black;
            btnFlop.BackColor = Color.DarkGray;

            btnTurn.Enabled = false;
            btnTurn.ForeColor = Color.DarkGray;
            btnTurn.BackColor = Color.LightGray;

            btnRiver.Enabled = true;
            btnRiver.ForeColor = Color.Black;
            btnRiver.BackColor = Color.DarkGray;
        }

        public void VisualRiver()
        {
            lblFrmTitle.Text = "River";
            lblFrmTitle.Location = new Point(ClientSize.Width / 2 - lblFrmTitle.Width / 2, 10);

            btnPreFlop.Enabled = true;
            btnPreFlop.ForeColor = Color.Black;
            btnPreFlop.BackColor = Color.DarkGray;

            btnFlop.Enabled = true;
            btnFlop.ForeColor = Color.Black;
            btnFlop.BackColor = Color.DarkGray;

            btnTurn.Enabled = true;
            btnTurn.ForeColor = Color.Black;
            btnTurn.BackColor = Color.DarkGray;

            btnRiver.Enabled = false;
            btnRiver.ForeColor = Color.DarkGray;
            btnRiver.BackColor = Color.LightGray;
        }

        #endregion

        #region Event Handlers

        public void btnPreFlop_Click(object sender, EventArgs e)
        {
            // start data processing timer
            stpDataUpdateTimeOthers.Start();

            int intChartSetIndex;

            ChartDataInvalid();
            VisualPreFlop();

            for (int i = 0; i < 10; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                daz[i].PlotPlayerDataPreFlop(intChartSetIndex, cntPlayerData[i], this);

                // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                UpdateChartColors(intChartSetIndex, blnPlayerFolded_tsk[i]);
            }

            // Update data processing timer
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();
        }

        public void btnFlop_Click(object sender, EventArgs e)
        {
            // start data processing timer
            stpDataUpdateTimeOthers.Start();

            int intChartSetIndex;

            ChartDataInvalid();
            VisualFlop();

            for (int i = 0; i < 10; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                daz[i].PlotPlayerDataFlop(intChartSetIndex, cntPlayerData[i], this);

                // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                UpdateChartColors(intChartSetIndex, blnPlayerFolded_tsk[i]);
            }

            // Update data processing timer
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();
        }

        public void btnTurn_Click(object sender, EventArgs e)
        {
            // start data processing timer
            stpDataUpdateTimeOthers.Start();

            int intChartSetIndex;

            ChartDataInvalid();
            VisualTurn();

            for (int i = 0; i < 10; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                daz[i].PlotPlayerDataTurn(intChartSetIndex, cntPlayerData[i], this);

                // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                UpdateChartColors(intChartSetIndex, blnPlayerFolded_tsk[i]);
            }

            // Update data processing timer
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();
        }

        public void btnRiver_Click(object sender, EventArgs e)
        {
            // start data processing timer
            stpDataUpdateTimeOthers.Start();

            int intChartSetIndex;

            ChartDataInvalid();
            VisualRiver();

            for (int i = 0; i < 10; i++)
            {
                intChartSetIndex = PlayerIndexToChartSetIndex(i);

                daz[i].PlotPlayerDataRiver(intChartSetIndex, cntPlayerData[i], this);

                // Verify the chart colors are correct. This is needed when the chart starts with "No Data" then subsequently has valid data. 
                UpdateChartColors(intChartSetIndex, blnPlayerFolded_tsk[i]);
            }

            // Update data processing timer
            stpDataUpdateTimeOthers.Stop();
            lblProcessingTimeOthers.Text = "Others: " + (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            lblProcessingTimePlrOfInterest.Text = strPlrOfInterest + ": "+ (stpDataUpdateTimeOthers.ElapsedMilliseconds / 1000.0).ToString("f3") + " sec.";
            stpDataUpdateTimeOthers.Reset();
        }

        /// <summary>
        /// Update the Chart Updated Indicator color to green because the chart data has been updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chtQuad_PostPaint(object sender, ChartPaintEventArgs e)
        {
            int i, j;
            MyChart cht = sender as MyChart;

            i = cht.I;
            j = cht.J;

            if (blnChtUpdated[i][j])
            {
                if (blnPlayerFolded_tsk != null && blnPlayerFolded_tsk[IndexFrom6(i)])
                {
                    pnlDataUpdateStatus[i][j].BackColor = clrFold_Fadded;
                }
                else
                {
                    pnlDataUpdateStatus[i][j].BackColor = clrFold;
                }
                blnChtUpdated[i][j] = false;
            }
        }

        /// <summary>
        /// This event is needed for the pnlData control. It is not used here but SetupFrmDataDisplay.cs refers to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmDataDisplay_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Changes the color of the "Data Update Status" squares to red indicating the data has not been updated
        /// </summary>
        private void ChartDataInvalid()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (blnPlayerFolded_tsk != null && blnPlayerFolded_tsk[IndexFrom6(i)])
                    {
                        pnlDataUpdateStatus[i][j].BackColor = clrBet_Fadded;
                    }
                    else
                    {
                        pnlDataUpdateStatus[i][j].BackColor = clrBet;
                    }
                }
            }
        }

        /// <summary>
        /// This method updates the charts of the players
        /// </summary>
        /// <param name="intPlayerIndex"></param>
        private void UpdatePlayersCharts(int intPlayerIndex, bool blnFolded)
        {
            int intChartSetIndex = PlayerIndexToChartSetIndex(intPlayerIndex);

            // Update charts with player data based on what betting stage we are in
            if (intBoardStatus_tsk == 0)
            {
                daz[intPlayerIndex].PlotPlayerDataPreFlop(intChartSetIndex, cntPlayerData[intPlayerIndex], this);
            }
            else if (intBoardStatus_tsk == 1)
            {
                daz[intPlayerIndex].PlotPlayerDataFlop(intChartSetIndex, cntPlayerData[intPlayerIndex], this);
            }
            else if (intBoardStatus_tsk == 2)
            {
                daz[intPlayerIndex].PlotPlayerDataTurn(intChartSetIndex, cntPlayerData[intPlayerIndex], this);
            }
            else
            {
                daz[intPlayerIndex].PlotPlayerDataRiver(intChartSetIndex, cntPlayerData[intPlayerIndex], this);
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
        /// Converts an index used in a for loop to increment from 6 to 5 instead of from 0 to 8
        /// </summary>
        /// <param name="intIndex">The index you wish to convert</param>
        /// <returns>The converted index</returns>
        private int IndexFrom6(int intIndex)
        {
            int j;

            j = intIndex + 6;

            if (j > 8)
            {
                j -= 9;
            }

            return j;
        }

        /// <summary>
        /// Changes the colors of a set of charts to either the normal version or the fadded version
        /// </summary>
        /// <param name="intChartSetIndex_"></param>
        /// <param name="blnFaddedColors"></param>
        private void UpdateChartColors(int intChartSetIndex_, bool blnFaddedColors)
        {
            Color clr0, clr1, clr2, clr3, clr4;

            if (!blnFaddedColors)
            {
                clr0 = clrFold;
                clr1 = clrCheck;
                clr2 = clrCall;
                clr3 = clrBet;
                clr4 = Color.White;
            }
            else
            {
                clr0 = clrFold_Fadded;
                clr1 = clrCheck_Fadded;
                clr2 = clrCall_Fadded;
                clr3 = clrBet_Fadded;
                clr4 = clrWhite_Fadded;
            }

            lblPlayerName[intChartSetIndex_].ForeColor = clr4;

            for (int i = 0; i < 4; i++)
            {
                chtQuad[intChartSetIndex_][i].Titles[0].ForeColor = clr4;

                if (lblChartNoData[intChartSetIndex_][i].Visible == false)
                {
                    chtQuad[intChartSetIndex_][i].Series[0].Points[0].Color = clr0;
                    chtQuad[intChartSetIndex_][i].Series[0].Points[1].Color = clr1;
                    chtQuad[intChartSetIndex_][i].Series[0].Points[2].Color = clr2;
                    chtQuad[intChartSetIndex_][i].Series[0].Points[3].Color = clr3;
                }
            }
        }

        #region Task Related Helper Methods

        /// <summary>
        /// Create the tasks for updating the player data on the Data Display Form. (One task for each of a possible 9 players)
        /// </summary>
        private void CreateUpdateDataTasks()
        {
            PreparePlayerHandInfoForAnalysis();

            for (int i = 0; i < 9; i++)
            {
                int j = i;
                if (tskUpdateData[j] == null || tskUpdateData[j].Status != TaskStatus.Created)
                {
                    tskUpdateData[j] = new Task(() =>
                    {
                        // If this is an open seat don't update the data -> pass an empty cntPlayerData object
                        // Note: If the player is sitting out continue to process data as if he is playing. Things get messed up and
                        //       too complicated when trying to ignore players sitting out, not worth the effort.
                        // Note: intBoardStatus_tsk determines what betting stage we are in (0 = preflop, 1 = flop, 2 = turn, 3 = river)
                        if (blnOpenSeat_tsk[j])
                        {
                            cntPlayerData[j] = new DataAnalyzer.DataCounter();
                        }
                        else
                        {
                            cntPlayerData[j] = daz[j].AnalyzePlayerData(hiaPlayerHandInfoAll[j], intDbGameId_tsk, intCurrNumPlayers_tsk, lngPlrOfInterestDbHandPlayerId);
                        }

                        // Debug.WriteLine("Completing task #" + j.ToString() + " - " + strName_tsk[j].ToString());
                    });
                }
            }
        }

        private void PreparePlayerHandInfoForAnalysis()
        {
            Bol_Model_DBEntities ctxQueryContext;

            ctxQueryContext = db_PlayerHandInfoQuery.GetDbContext();

            using (ctxQueryContext)
            {
                for (int i = 0; i < 9; i++)
                {
                    hiaPlayerHandInfoAll[i] = db_[i].QueryPlayerHandsInfoAll_query(db_[i].QueryPlayerHandInfo_query(intDbId_tsk[i], 300, ctxQueryContext), ctxQueryContext);
                }
            }
        }

        #endregion

        #endregion

        #region Classes

        /// <summary>
        /// A custom chart class. The only thing I added to the Chart class was the ability to save two indicies, "I" and "J".
        /// </summary>
        public class MyChart : Chart
        {
            private int intI, intJ;

            public int I
            {
                get { return intI; }
                set { intI = value; }
            }
            public int J
            {
                get { return intJ; }
                set { intJ = value; }
            }
        }

        #endregion

    }
}
