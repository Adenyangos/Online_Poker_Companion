using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static BOL_Companion.DataDisplayForm;

namespace BOL_Companion
{
    /// <summary>
    /// This class sets the size and location of frmDataDisplay and initializes all the form controls, their attributes and defines their locations.
    /// </summary>
    class SetupDataDisplayForm
    {
        #region Variables and Objects

        /// <summary>
        /// The font of the chart titles
        /// </summary>
        Font chartTitlesFnt;

        /// <summary>
        /// The font of the player names
        /// </summary>
        Font playerNameFnt;

        /// <summary>
        /// The font for "No Data" labels
        /// </summary>
        Font noDataFnt;

        /// <summary>
        /// The font for sample count labels
        /// </summary>
        Font sampleCountFnt;

        /// <summary>
        /// The font for chart labels
        /// </summary>
        Font chartLabelsFnt;

        /// <summary>
        /// The font for legend lables
        /// </summary>
        Font legendFnt;

        /// <summary>
        /// The font for button text
        /// </summary>
        Font buttonFnt;

        /// <summary>
        /// The pen used for drawing rectangles
        /// </summary>
        Pen rectPen; 
            
        /// <summary>
        /// The pen used for drawing lines
        /// </summary>
        Pen linePen;

        /// <summary>
        /// The color representing a fold action
        /// </summary>
        Color foldClr_;

        /// <summary>
        /// The color representing a check action
        /// </summary>
        Color checkClr_;

        /// <summary>
        /// The color representing a call action
        /// </summary>
        Color callClr_;

        /// <summary>
        /// The color representing a bet action
        /// </summary>
        Color betClr_;

        /// <summary>
        /// The color representing a fold action of an inactive player
        /// </summary>
        Color foldClrFaded_;

        /// <summary>
        /// The color representing a check action of an inactive player
        /// </summary>
        Color checkClrFaded_;

        /// <summary>
        /// The color representing a call action of an inactive player
        /// </summary>
        Color callClrFaded_;

        /// <summary>
        /// The color representing a bet action of an inactive player
        /// </summary>
        Color betClrFaded_;

        /// <summary>
        /// A faded white color to use for inactive player labels
        /// </summary>
        Color whiteClrFaded_;

        /// <summary>
        /// The background color for rich textboxes
        /// </summary>
        Color rtbBackgroundClr_;

        /// <summary>
        /// A custom grey color
        /// </summary>
        Color customGreyClr;

        #endregion

        #region Controls

        #region Form

        /// <summary>
        /// The DataDisplayForm to setup
        /// </summary>
        DataDisplayForm frm;

        #endregion

        #region Panels

        /// <summary>
        /// The panel that contains all the data on the form
        /// </summary>
       Panel dataPnl_;

        /// <summary>
        /// The colored box in the legend showing the fold color
        /// </summary>
       Panel legendFoldClrPnl_;

        /// <summary>
        /// The colored box in the legend showing the check color
        /// </summary>
       Panel legendCheckClrPnl_;

        /// <summary>
        /// The colored box in the legend showing the call color
        /// </summary>
       Panel legendCallClrPnl_;

        /// <summary>
        /// The colored box in the legend showing the bet color
        /// </summary>
       Panel legendBetClrPnl_;

        /// <summary>
        /// All of the dataUpdateStatusIndicators (the colored boxes next to the charts indicating if the charts have been updated)
        /// </summary>
       Panel[][] dataUpdateStatusIndicator_;

        /// <summary>
        /// All of the dataUpdateStatusIndicatorBorders (the borders of the dataUpdateStatusIndicators)
        /// </summary>
       Panel[][] dataUpdateStatusIndicatorBorder_;

        #endregion

        #region Labels

        /// <summary>
        /// The title of the DataDisplayForm
        /// </summary>
       Label formTitleLbl_;

        /// <summary>
        /// A timestamp displaying the last time the data on the DataDisplayForm was updated
        /// </summary>
       Label timeLbl_;

        /// <summary>
        /// The fold label inside the legend
        /// </summary>
       Label legendFoldLbl_;

        /// <summary>
        /// The check label inside the legend
        /// </summary>
       Label legendCheckLbl_;

        /// <summary>
        /// The call label inside the legend
        /// </summary>
       Label legendCallLbl_;

        /// <summary>
        /// The bet lable inside the legend
        /// </summary>
       Label legendBetLbl_;

        /// <summary>
        /// The time it took to process the data for all the players except the player of interest
        /// </summary>
       Label processingTimeOthersLbl_;

        /// <summary>
        /// The time it took to process the data for the player of interest
        /// </summary>
       Label processingTimePlrOfInterestLbl_;

        /// <summary>
        /// The labels for the player names
        /// </summary>
       Label[] playerNameLbl_;

        /// <summary>
        /// The labels displaying the text "No Data" when there is no data available for a given chart
        /// </summary>
       Label[][] chartNoDataLbl_;

        /// <summary>
        /// The labels displaying how many data samples (data points) are included in a given chart
        /// </summary>
       Label[][] chartSampleCountLbl_;

        #endregion

        #region Buttons

        /// <summary>
        /// Click this button to see the pre-flop data
        /// </summary>
       Button preFlopBtn_;

        /// <summary>
        /// Click this button to see the flop data
        /// </summary>
       Button flopBtn_;

        /// <summary>
        /// Click this button to see the turn data
        /// </summary>
       Button turnBtn_;

        /// <summary>
        /// Click this button to see the river data
        /// </summary>
       Button riverBtn_;

        #endregion

        #region Charts

        /// <summary>
        /// All of the charts on the DataDislayForm (pie charts)
        /// </summary>
        MyChart[][] pieChts_;

        #endregion

        #endregion

        public SetupDataDisplayForm(DataDisplayForm frmIn)
        {
            frm = frmIn;
            InitializeLocalVariables();
        }

        /// <summary>
        /// Initialize this class's private variables
        /// </summary>
        private void InitializeLocalVariables()
        {
            chartTitlesFnt = new Font(frm.Font.FontFamily, 10, FontStyle.Bold);
            playerNameFnt = new Font(frm.Font.FontFamily, 20, FontStyle.Bold | FontStyle.Underline);
            noDataFnt = new Font(frm.Font.FontFamily, 18, FontStyle.Bold);
            sampleCountFnt = new Font(frm.Font.FontFamily, 10);
            chartLabelsFnt = new Font(frm.Font.FontFamily, 12, FontStyle.Bold);
            legendFnt = new Font(frm.Font.FontFamily, 12, FontStyle.Bold);
            buttonFnt = new Font(frm.Font.FontFamily, 18, FontStyle.Bold);
        }

        /// <summary>
        /// Set the colors for the DataDisplayForm.
        /// </summary>
        /// <param name="foldClrIn">foldClr</param>
        /// <param name="checkClrIn">checkClr</param>
        /// <param name="callClrIn">callClr</param>
        /// <param name="betClrIn">betClr</param>
        /// <param name="foldClrFadedIn">foldClrFaded</param>
        /// <param name="checkClrFadedIn">checkClrFaded</param>
        /// <param name="callClrFadedIn">callClrFaded</param>
        /// <param name="betClrFadedIn">betClrFaded</param>
        /// <param name="whiteClrFadedIn">whiteClrFaded</param>
        /// <param name="rtbBackgroundClrIn">rtbBackgroundClr</param>
        public void SetFormColors(ref Color foldClrIn, ref Color checkClrIn, ref Color callClrIn, ref Color betClrIn,
            ref Color foldClrFadedIn, ref Color checkClrFadedIn, ref Color callClrFadedIn, ref Color betClrFadedIn,
            ref Color whiteClrFadedIn, Color rtbBackgroundClrIn)
        {
            foldClrIn = Color.SpringGreen;
            foldClr_ = foldClrIn;

            checkClrIn = Color.DodgerBlue;
            checkClr_ = checkClrIn;

            callClrIn= Color.FromArgb(185, 100, 255);
            callClr_ = callClrIn;

            betClrIn = Color.FromArgb(255, 35, 105);
            betClr_ = betClrIn;

            int colorFadeConst = -125;

            foldClrFadedIn = FadeColor(foldClr_, colorFadeConst);
            foldClrFaded_ = foldClrFadedIn;

            checkClrFadedIn= FadeColor(checkClr_, colorFadeConst);
            checkClrFaded_ = checkClrFadedIn;

            callClrFadedIn= FadeColor(callClr_, colorFadeConst);
            callClrFaded_ = callClrFadedIn;

            betClrFadedIn = FadeColor(betClr_, colorFadeConst);
            betClrFaded_ = betClrFadedIn;

            whiteClrFadedIn = FadeColor(Color.White, -100);
            whiteClrFaded_ = whiteClrFadedIn;

            rtbBackgroundClr_ = rtbBackgroundClrIn;

            customGreyClr = Color.FromArgb(135, 135, 135);

            rectPen = new Pen(Color.FromArgb(255, 255, 255), 2);
            linePen = new Pen(customGreyClr, 1);
        }

        /// <summary>
        /// Set the form location, size, title and background color
        /// </summary>
        public void InitializeForm()
        {
            if (Screen.AllScreens.Count() < 3)
            {
                frm.Location = Screen.AllScreens[0].WorkingArea.Location;
            }
            else
            {
                frm.Location = Screen.AllScreens[2].WorkingArea.Location;
            }
            frm.WindowState = FormWindowState.Maximized;
            frm.Text = "Data Display";
            frm.BackColor = rtbBackgroundClr_;
        }

        /// <summary>
        /// Initialize all the panels on the form
        /// </summary>
        /// <param name="dataPnlIn">dataPnl</param>
        /// <param name="legendFoldClrPnlIn">legendFoldClrPnl</param>
        /// <param name="legendCheckClrPnlIn">legendCheckClrPnl</param>
        /// <param name="legendCallClrPnlIn">legendCallClrPnl</param>
        /// <param name="legendBetClrPnlIn">legendBetClrPnl</param>
        /// <param name="dataUpdateStatusIndicatorIn">dataUpdateStatusIndicator</param>
        /// <param name="dataUpdateStatusIndicatorBorderIn">dataUpdateStatusIndicatorBorder</param>
        public void InitializeFormPanels(ref Panel dataPnlIn, ref Panel legendFoldClrPnlIn, ref Panel legendCheckClrPnlIn,
            ref Panel legendCallClrPnlIn, ref Panel legendBetClrPnlIn,
            ref Panel[][] dataUpdateStatusIndicatorIn, ref Panel[][] dataUpdateStatusIndicatorBorderIn)
        {
            dataPnlIn = new Panel();
            dataPnl_ = dataPnlIn;
            dataPnl_.BackColor = Color.Transparent;
            dataPnl_.Anchor = AnchorStyles.None;
            dataPnl_.BorderStyle = BorderStyle.FixedSingle;
            frm.Controls.Add(dataPnl_);
            dataPnl_.Paint += dataPnl_Paint;

            legendFoldClrPnlIn = new Panel();
            legendFoldClrPnl_ = legendFoldClrPnlIn;
            legendFoldClrPnl_.BackColor = foldClr_;
            legendFoldClrPnl_.Anchor = AnchorStyles.None;
            frm.Controls.Add(legendFoldClrPnl_);

            legendCheckClrPnlIn = new Panel();
            legendCheckClrPnl_ = legendCheckClrPnlIn;
            legendCheckClrPnl_.BackColor = checkClr_;
            legendCheckClrPnl_.Anchor = AnchorStyles.None;
            frm.Controls.Add(legendCheckClrPnl_);

            legendCallClrPnlIn = new Panel();
            legendCallClrPnl_ = legendCallClrPnlIn;
            legendCallClrPnl_.BackColor = callClr_;
            legendCallClrPnl_.Anchor = AnchorStyles.None;
            frm.Controls.Add(legendCallClrPnl_);

            legendBetClrPnlIn = new Panel();
            legendBetClrPnl_ = legendBetClrPnlIn;
            legendBetClrPnl_.BackColor = betClr_;
            legendBetClrPnl_.Anchor = AnchorStyles.None;
            frm.Controls.Add(legendBetClrPnl_);

            dataUpdateStatusIndicatorIn = new Panel[10][];
            dataUpdateStatusIndicator_ = dataUpdateStatusIndicatorIn;

            dataUpdateStatusIndicatorBorderIn = new Panel[10][];
            dataUpdateStatusIndicatorBorder_ = dataUpdateStatusIndicatorBorderIn;

            for (int i = 0; i < 10; i++)
            {
                dataUpdateStatusIndicator_[i] = new Panel[4];
                dataUpdateStatusIndicatorBorder_[i] = new Panel[4];

                for (int j = 0; j < 4; j++)
                {
                    // Data Update Status Background Panels (a border for Data Update Staus Panels)
                    dataUpdateStatusIndicatorBorder_[i][j] = new Panel();
                    dataUpdateStatusIndicatorBorder_[i][j].BackColor = Color.Black;
                    dataUpdateStatusIndicatorBorder_[i][j].BorderStyle = BorderStyle.None;
                    dataUpdateStatusIndicatorBorder_[i][j].Anchor = AnchorStyles.None;
                    dataPnl_.Controls.Add(dataUpdateStatusIndicatorBorder_[i][j]);
                    dataUpdateStatusIndicatorBorder_[i][j].BringToFront();

                    // Data Update Status Panels (colored squares that show if the data is up to date)
                    dataUpdateStatusIndicator_[i][j] = new Panel();
                    dataUpdateStatusIndicator_[i][j].BackColor = betClr_;
                    dataUpdateStatusIndicator_[i][j].BorderStyle = BorderStyle.None;
                    dataUpdateStatusIndicator_[i][j].Anchor = AnchorStyles.None;
                    dataPnl_.Controls.Add(dataUpdateStatusIndicator_[i][j]);
                    dataUpdateStatusIndicator_[i][j].BringToFront();
                }
            }
        }

        /// <summary>
        /// Initialize all the labels on the form
        /// </summary>
        /// <param name="formTitleLblIn">formTitleLbl</param>
        /// <param name="timeLblIn">timeLbl</param>
        /// <param name="legendFoldLblIn">legendFoldLbl</param>
        /// <param name="legendCheckLblIn">legendCheckLbl</param>
        /// <param name="legendCallLblIn">legendCallLbl</param>
        /// <param name="legendBetLblIn">legendBetLbl</param>
        /// <param name="processingTimeOthersLblIn">processingTimeOthersLbl</param>
        /// <param name="processingTimePlrOfInterestLblIn">processingTimePlrOfInterestLbl</param>
        /// <param name="playerNameIn">playerName</param>
        /// <param name="chartNoDataLblIn">chartNoDataLbl</param>
        /// <param name="chartSampleCountLblIn">chartSampleCountLbl</param>
        public void InitializeFormLabels(ref Label formTitleLblIn, ref Label timeLblIn, ref Label legendFoldLblIn, ref Label legendCheckLblIn,
            ref Label legendCallLblIn, ref Label legendBetLblIn, ref Label processingTimeOthersLblIn, ref Label processingTimePlrOfInterestLblIn,
            ref Label[] playerNameLblIn, ref Label[][] chartNoDataLblIn, ref Label[][] chartSampleCountLblIn)
        {
            formTitleLblIn = new Label();
            formTitleLbl_ = formTitleLblIn;
            formTitleLbl_.Font = new Font(frm.Font.FontFamily, 36, FontStyle.Bold);
            formTitleLbl_.ForeColor = Color.White;
            formTitleLbl_.Anchor = AnchorStyles.None;
            formTitleLbl_.Text = "No Data to Display... Yet!";
            frm.Controls.Add(formTitleLbl_);

            timeLblIn = new Label();
            timeLbl_ = timeLblIn;
            timeLbl_.Font = new Font("Courier New", 24, FontStyle.Bold);
            timeLbl_.ForeColor = Color.White;
            timeLbl_.Anchor = AnchorStyles.None;
            timeLbl_.Text = "00:00:00 AM";
            frm.Controls.Add(timeLbl_);

            legendFoldLblIn = new Label();
            legendFoldLbl_ = legendFoldLblIn;
            legendFoldLbl_.Font = legendFnt;
            legendFoldLbl_.ForeColor = Color.White;
            legendFoldLbl_.Anchor = AnchorStyles.None;
            legendFoldLbl_.Text = "Fold";
            frm.Controls.Add(legendFoldLbl_);
  
            legendCheckLblIn = new Label();
            legendCheckLbl_ = legendCheckLblIn;
            legendCheckLbl_.Font = legendFnt;
            legendCheckLbl_.ForeColor = Color.White;
            legendCheckLbl_.Anchor = AnchorStyles.None;
            legendCheckLbl_.Text = "Check";
            frm.Controls.Add(legendCheckLbl_);

            legendCallLblIn = new Label();
            legendCallLbl_ = legendCallLblIn;
            legendCallLbl_.Font = legendFnt;
            legendCallLbl_.ForeColor = Color.White;
            legendCallLbl_.Anchor = AnchorStyles.None;
            legendCallLbl_.Text = "Call";
            frm.Controls.Add(legendCallLbl_);

            legendBetLblIn = new Label();
            legendBetLbl_ = legendBetLblIn;
            legendBetLbl_.Font = legendFnt;
            legendBetLbl_.ForeColor = Color.White;
            legendBetLbl_.Anchor = AnchorStyles.None;
            legendBetLbl_.Text = "Bet";
            frm.Controls.Add(legendBetLbl_);

            processingTimeOthersLblIn = new Label();
            processingTimeOthersLbl_ = processingTimeOthersLblIn;
            processingTimeOthersLbl_.Font = new Font("Courier New", 10, FontStyle.Bold);
            processingTimeOthersLbl_.ForeColor = Color.White;
            processingTimeOthersLbl_.Anchor = AnchorStyles.None;
            processingTimeOthersLbl_.Text = "Others: 0.000 sec.";
            frm.Controls.Add(processingTimeOthersLbl_);

            processingTimePlrOfInterestLblIn = new Label();
            processingTimePlrOfInterestLbl_ = processingTimePlrOfInterestLblIn;
            processingTimePlrOfInterestLbl_.Font = new Font("Courier New", 10, FontStyle.Bold);
            processingTimePlrOfInterestLbl_.ForeColor = Color.White;
            processingTimePlrOfInterestLbl_.Anchor = AnchorStyles.None;
            processingTimePlrOfInterestLbl_.Text = "JabaAdam: 0.000 sec.";
            frm.Controls.Add(processingTimePlrOfInterestLbl_);

            playerNameLblIn = new Label[10];
            playerNameLbl_ = playerNameLblIn;

            chartNoDataLblIn = new Label[10][];
            chartNoDataLbl_ = chartNoDataLblIn;

            chartSampleCountLblIn = new Label[10][];
            chartSampleCountLbl_ = chartSampleCountLblIn;

            for (int i = 0; i < 10; i++)
            {
                // Player Name Labels
                playerNameLbl_[i] = new Label();
                playerNameLbl_[i].Font = playerNameFnt;
                playerNameLbl_[i].ForeColor = Color.White;
                playerNameLbl_[i].Anchor = AnchorStyles.None;
                playerNameLbl_[i].BackColor = Color.Transparent;
                if (i < 9)
                {
                    playerNameLbl_[i].Text = "Player " + (i + 1).ToString();
                }
                else
                {
                    playerNameLbl_[i].Text = "Other Players at This Table";
                }
                playerNameLbl_[i].TextAlign = ContentAlignment.TopCenter;
                dataPnl_.Controls.Add(playerNameLbl_[i]);

                // No Chart Data Labels and Chart Sample Count Labels
                chartNoDataLbl_[i] = new Label[4];
                chartSampleCountLbl_[i] = new Label[4];
                for (int j = 0; j < 4; j++)
                {
                    chartNoDataLbl_[i][j] = new Label();
                    chartNoDataLbl_[i][j].Font = noDataFnt;
                    chartNoDataLbl_[i][j].Anchor = AnchorStyles.None;
                    chartNoDataLbl_[i][j].BackColor = Color.DarkGray;
                    chartNoDataLbl_[i][j].Text = "No Data";
                    chartNoDataLbl_[i][j].Visible = false;
                    dataPnl_.Controls.Add(chartNoDataLbl_[i][j]);

                    chartSampleCountLbl_[i][j] = new Label();
                    chartSampleCountLbl_[i][j].Font = sampleCountFnt;
                    chartSampleCountLbl_[i][j].Anchor = AnchorStyles.None;
                    chartSampleCountLbl_[i][j].BackColor = Color.Transparent;
                    chartSampleCountLbl_[i][j].Text = "XXX";
                    dataPnl_.Controls.Add(chartSampleCountLbl_[i][j]);
                }
            }
        }

        /// <summary>
        /// Initialize all the buttons on the form
        /// </summary>
        /// <param name="preFlopBtnIn">preFlopBtn</param>
        /// <param name="flopBtnIn">flopBtn</param>
        /// <param name="turnBtnIn">turnBtn</param>
        /// <param name="riverBtnIn">riverBtn</param>
        public void InitializeFormButtons(ref Button preFlopBtnIn, ref Button flopBtnIn, ref Button turnBtnIn, ref Button riverBtnIn)
        {
            preFlopBtnIn = new Button();
            preFlopBtn_ = preFlopBtnIn;
            preFlopBtn_.Font = buttonFnt;
            preFlopBtn_.ForeColor = Color.DarkGray;
            preFlopBtn_.BackColor = Color.LightGray;
            preFlopBtn_.Anchor = AnchorStyles.None;
            preFlopBtn_.Text = "Pre-Flop";
            preFlopBtn_.Enabled = false;
            frm.Controls.Add(preFlopBtn_);
            preFlopBtn_.Click += new EventHandler(frm.preFlopBtn_Click);

            flopBtnIn = new Button();
            flopBtn_ = flopBtnIn;
            flopBtn_.Font = buttonFnt;
            flopBtn_.ForeColor = Color.Black;
            flopBtn_.BackColor = Color.DarkGray;
            flopBtn_.Anchor = AnchorStyles.None;
            flopBtn_.Text = "Flop";
            flopBtn_.Enabled = true;
            frm.Controls.Add(flopBtn_);
            flopBtn_.Click += new EventHandler(frm.flopBtn_Click);

            turnBtnIn = new Button();
            turnBtn_ = turnBtnIn;
            turnBtn_.Font = buttonFnt;
            turnBtn_.ForeColor = Color.Black;
            turnBtn_.BackColor = Color.DarkGray;
            turnBtn_.Anchor = AnchorStyles.None;
            turnBtn_.Text = "Turn";
            turnBtn_.Enabled = true;
            frm.Controls.Add(turnBtn_);
            turnBtn_.Click += new EventHandler(frm.turnBtn_Click);

            riverBtnIn = new Button();
            riverBtn_ = riverBtnIn;
            riverBtn_.Font = buttonFnt;
            riverBtn_.ForeColor = Color.Black;
            riverBtn_.BackColor = Color.DarkGray;
            riverBtn_.Anchor = AnchorStyles.None;
            riverBtn_.Text = "River";
            riverBtn_.Enabled = true;
            frm.Controls.Add(riverBtn_);
            riverBtn_.Click += new EventHandler(frm.riverBtn_Click);
        }

        /// <summary>
        /// Initialize all the charts on the form
        /// </summary>
        /// <param name="pieChtsIn">pieChts</param>
        /// <param name="isChartUpToDateIn">isChartUpToDate</param>
        public void InitializeFormCharts(ref MyChart[][] pieChtsIn, ref bool[][] isChartUpToDateIn)
        {
            // One set of 4 charts for each of the 10 data display locations on the DataDisplayForm
            pieChtsIn = new MyChart[10][];
            pieChts_ = pieChtsIn;

            for (int i = 0; i < 10; i++)
            {
                pieChts_[i] = new MyChart[4];
            }

            // Create the 4 chart titles
            string[] chartTitles = new string[4];
            chartTitles[0] = "This Table -\nCurrent Num. Players";
            chartTitles[1] = "This Table -\nAny Num. Players";
            chartTitles[2] = "Any Table -\nCurrent Num. Players";
            chartTitles[3] = "Any Table -\nAny Num. Players";

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    pieChts_[i][j] = new MyChart();
                    ChartArea pieChartArea = new ChartArea();

                    pieChts_[i][j].Series.Add(chartTitles[j]);
                    pieChts_[i][j].Series[0].ChartType = SeriesChartType.Pie;
                    pieChts_[i][j].Series[0].Points.AddXY("25%", 2);
                    pieChts_[i][j].Series[0].Points.AddXY("25%", 2);
                    pieChts_[i][j].Series[0].Points.AddXY("25%", 2);
                    pieChts_[i][j].Series[0].Points.AddXY("25%", 2);
                    pieChts_[i][j].Series[0].Points[0].Color = foldClr_;
                    pieChts_[i][j].Series[0].Points[1].Color = checkClr_;
                    pieChts_[i][j].Series[0].Points[2].Color = callClr_;
                    pieChts_[i][j].Series[0].Points[3].Color = betClr_;
                    pieChts_[i][j].Series[0].Font = chartLabelsFnt;
                    pieChts_[i][j].Series[0]["PieStartAngle"] = "270";
                    pieChts_[i][j].ChartAreas.Add(pieChartArea);
                    pieChts_[i][j].ChartAreas[0].BackColor = Color.Transparent;
                    pieChts_[i][j].Titles.Add(chartTitles[j]);
                    pieChts_[i][j].Titles[0].ForeColor = Color.White;
                    pieChts_[i][j].Titles[0].Font = chartTitlesFnt;
                    pieChts_[i][j].Titles[0].DockingOffset = 3;
                    pieChts_[i][j].BackColor = Color.Transparent;
                    pieChts_[i][j].I = i;
                    pieChts_[i][j].J = j;
                    dataPnl_.Controls.Add(pieChts_[i][j]);

                    pieChts_[i][j].PostPaint += frm.chartQuad_PostPaint;
                }
            }

            isChartUpToDateIn = new bool[10][];
            for (int i = 0; i < 10; i++)
            {
                isChartUpToDateIn[i] = new bool[4];
                for (int j = 0; j < 4; j++)
                {
                    isChartUpToDateIn[i][j] = false;
                }
            }
        }

        /// <summary>
        /// Set the sizes and locations of the controls on the form.
        /// </summary>
        public void SetControlSizesAndLocations()
        {
            formTitleLbl_.AutoSize = true;
            timeLbl_.AutoSize = true;
            processingTimeOthersLbl_.AutoSize = true;
            processingTimePlrOfInterestLbl_.AutoSize = true;
            legendFoldLbl_.AutoSize = true;
            legendCheckLbl_.AutoSize = true;
            legendCallLbl_.AutoSize = true;
            legendBetLbl_.AutoSize = true;

            Size buttonSize = new Size(130, 45);
            preFlopBtn_.Size = buttonSize;
            flopBtn_.Size = buttonSize;
            turnBtn_.Size = buttonSize;
            riverBtn_.Size = buttonSize;

            int formWidth = frm.ClientSize.Width;
            int formHeight = frm.ClientSize.Height;
            dataPnl_.Size = new Size(formWidth - 30, formHeight - 90);

            Size legendPanelSize = new Size(32, 14);
            legendFoldClrPnl_.Size = legendPanelSize;
            legendCheckClrPnl_.Size = legendPanelSize;
            legendCallClrPnl_.Size = legendPanelSize;
            legendBetClrPnl_.Size = legendPanelSize;

            for (int i = 0; i < 10; i++)
            {
                playerNameLbl_[i].Size = new Size(dataPnl_.Width / 5, 32);

                for (int j = 0; j < 4; j++)
                {
                    dataUpdateStatusIndicator_[i][j].Size = new Size(14, 14);
                    dataUpdateStatusIndicatorBorder_[i][j].Size = new Size(18, 18);
                    chartNoDataLbl_[i][j].AutoSize = true;
                    chartSampleCountLbl_[i][j].AutoSize = true;
                    pieChts_[i][j].Size = new Size(dataPnl_.Width / 10, (dataPnl_.Height - 40) / 4);
                }
            }

            formTitleLbl_.Location = new Point(formWidth / 2 - formTitleLbl_.Width / 2, 10);
            timeLbl_.Location = new Point(1683, 8);
            preFlopBtn_.Location = new Point(15, 25);
            flopBtn_.Location = new Point(155, 25);
            turnBtn_.Location = new Point(295, 25);
            riverBtn_.Location = new Point(435, 25);
            dataPnl_.Location = new Point(formWidth / 2 - dataPnl_.Width / 2, (formHeight - 60) / 2 - dataPnl_.Height / 2 + 60);
            processingTimeOthersLbl_.Location = new Point(dataPnl_.Location.X + dataPnl_.Width - processingTimeOthersLbl_.Width + 4, 1001);
            processingTimePlrOfInterestLbl_.Location = new Point(dataPnl_.Location.X, 1001);

            legendBetLbl_.Location = new Point(1867, 48);
            legendBetClrPnl_.Location = new Point(legendBetLbl_.Location.X - 33, 50);
            legendCallLbl_.Location = new Point(legendBetClrPnl_.Location.X - legendCallLbl_.Width - 19, 48);
            legendCallClrPnl_.Location = new Point(legendCallLbl_.Location.X - 33, 50);
            legendCheckLbl_.Location = new Point(legendCallClrPnl_.Location.X - legendCheckLbl_.Width - 19, 48);
            legendCheckClrPnl_.Location = new Point(legendCheckLbl_.Location.X - 33, 50);
            legendFoldLbl_.Location = new Point(legendCheckClrPnl_.Location.X - legendFoldLbl_.Width - 19, 48);
            legendFoldClrPnl_.Location = new Point(legendFoldLbl_.Location.X - 33, 50);

            for (int i = 0; i < 5; i++)
            {
                playerNameLbl_[i].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 - playerNameLbl_[i].Width / 2, 4);
                playerNameLbl_[i + 5].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 - playerNameLbl_[i + 5].Width / 2, dataPnl_.Height / 2 + 4);

                dataUpdateStatusIndicator_[i][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 21, 85);
                dataUpdateStatusIndicator_[i][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 21, 85);
                dataUpdateStatusIndicator_[i][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 21, dataPnl_.Height / 4 + 69);
                dataUpdateStatusIndicator_[i][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 21, dataPnl_.Height / 4 + 69);

                dataUpdateStatusIndicator_[i + 5][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 21, dataPnl_.Height / 2 + 85);
                dataUpdateStatusIndicator_[i + 5][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 21, dataPnl_.Height / 2 + 85);
                dataUpdateStatusIndicator_[i + 5][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 21, 3 * dataPnl_.Height / 4 + 69);
                dataUpdateStatusIndicator_[i + 5][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 21, 3 * dataPnl_.Height / 4 + 69);

                dataUpdateStatusIndicatorBorder_[i][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 19, 83);
                dataUpdateStatusIndicatorBorder_[i][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 19, 83);
                dataUpdateStatusIndicatorBorder_[i][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 19, dataPnl_.Height / 4 + 67);
                dataUpdateStatusIndicatorBorder_[i][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 19, dataPnl_.Height / 4 + 67);

                dataUpdateStatusIndicatorBorder_[i + 5][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 19, dataPnl_.Height / 2 + 83);
                dataUpdateStatusIndicatorBorder_[i + 5][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 19, dataPnl_.Height / 2 + 83);
                dataUpdateStatusIndicatorBorder_[i + 5][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 19, 3 * dataPnl_.Height / 4 + 67);
                dataUpdateStatusIndicatorBorder_[i + 5][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 19, 3 * dataPnl_.Height / 4 + 67);

                pieChts_[i][0].Location = new Point(2 * i * dataPnl_.Width / 10, 33);
                pieChts_[i][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10, 33);
                pieChts_[i][2].Location = new Point(2 * i * dataPnl_.Width / 10, dataPnl_.Height / 4 + 17);
                pieChts_[i][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10, dataPnl_.Height / 4 + 17);

                pieChts_[i + 5][0].Location = new Point(2 * i * dataPnl_.Width / 10, dataPnl_.Height / 2 + 33);
                pieChts_[i + 5][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10, dataPnl_.Height / 2 + 33);
                pieChts_[i + 5][2].Location = new Point(2 * i * dataPnl_.Width / 10, 3 * dataPnl_.Height / 4 + 17);
                pieChts_[i + 5][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10, 3 * dataPnl_.Height / 4 + 17);

                chartNoDataLbl_[i][0].Location = new Point((4 * i + 1) * dataPnl_.Width / 20 - chartNoDataLbl_[i][0].Width / 2, 147);
                chartNoDataLbl_[i][1].Location = new Point((4 * i + 3) * dataPnl_.Width / 20 - chartNoDataLbl_[i][1].Width / 2, 147);
                chartNoDataLbl_[i][2].Location = new Point((4 * i + 1) * dataPnl_.Width / 20 - chartNoDataLbl_[i][2].Width / 2, dataPnl_.Height / 4 + 131);
                chartNoDataLbl_[i][3].Location = new Point((4 * i + 3) * dataPnl_.Width / 20 - chartNoDataLbl_[i][3].Width / 2, dataPnl_.Height / 4 + 131);

                chartNoDataLbl_[i + 5][0].Location = new Point((4 * i + 1) * dataPnl_.Width / 20 - chartNoDataLbl_[i + 5][0].Width / 2, dataPnl_.Height / 2 + 147);
                chartNoDataLbl_[i + 5][1].Location = new Point((4 * i + 3) * dataPnl_.Width / 20 - chartNoDataLbl_[i + 5][1].Width / 2, dataPnl_.Height / 2 + 147);
                chartNoDataLbl_[i + 5][2].Location = new Point((4 * i + 1) * dataPnl_.Width / 20 - chartNoDataLbl_[i + 5][2].Width / 2, 3 * dataPnl_.Height / 4 + 131);
                chartNoDataLbl_[i + 5][3].Location = new Point((4 * i + 3) * dataPnl_.Width / 20 - chartNoDataLbl_[i + 5][3].Width / 2, 3 * dataPnl_.Height / 4 + 131);

                chartSampleCountLbl_[i][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 140, 228);
                chartSampleCountLbl_[i][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 140, 228);
                chartSampleCountLbl_[i][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 140, dataPnl_.Height / 4 + 212);
                chartSampleCountLbl_[i][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 140, dataPnl_.Height / 4 + 212);

                chartSampleCountLbl_[i + 5][0].Location = new Point(2 * i * dataPnl_.Width / 10 + 140, dataPnl_.Height / 2 + 228);
                chartSampleCountLbl_[i + 5][1].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 140, dataPnl_.Height / 2 + 228);
                chartSampleCountLbl_[i + 5][2].Location = new Point(2 * i * dataPnl_.Width / 10 + 140, 3 * dataPnl_.Height / 4 + 212);
                chartSampleCountLbl_[i + 5][3].Location = new Point((2 * i + 1) * dataPnl_.Width / 10 + 140, 3 * dataPnl_.Height / 4 + 212);
            }
        }

        #region Events

        /// <summary>
        /// Draw seperator lines inside of the dataPnl_
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataPnl_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = ((Panel)sender);
            if (panel.Visible)
            {
                int panelMiddle = (panel.Height - 2) / 2;

                // horizontal lines
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 0 / 5 + 15, panelMiddle), new Point(panel.Width * 1 / 5 - 15, panelMiddle));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 1 / 5 + 15, panelMiddle), new Point(panel.Width * 2 / 5 - 15, panelMiddle));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 2 / 5 + 15, panelMiddle), new Point(panel.Width * 3 / 5 - 15, panelMiddle));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 3 / 5 + 15, panelMiddle), new Point(panel.Width * 4 / 5 - 15, panelMiddle));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 4 / 5 + 15, panelMiddle), new Point(panel.Width * 5 / 5 - 15, panelMiddle));

                // vertical lines
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 1 / 5, 15), new Point(panel.Width * 1 / 5, panelMiddle * 1 - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 1 / 5, panelMiddle + 15), new Point(panel.Width * 1 / 5, panel.Height - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 2 / 5, 15), new Point(panel.Width * 2 / 5, panelMiddle * 1 - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 2 / 5, panelMiddle + 15), new Point(panel.Width * 2 / 5, panel.Height - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 3 / 5, 15), new Point(panel.Width * 3 / 5, panelMiddle * 1 - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 3 / 5, panelMiddle + 15), new Point(panel.Width * 3 / 5, panel.Height - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 4 / 5, 15), new Point(panel.Width * 4 / 5, panelMiddle * 1 - 15));
                e.Graphics.DrawLine(linePen, new Point(panel.Width * 4 / 5, panelMiddle + 15), new Point(panel.Width * 4 / 5, panel.Height - 15));

                // Panel border
                e.Graphics.DrawRectangle(rectPen, 0, 0, panel.Width - 2, panel.Height - 2);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts a color to the faded version of that color.
        /// </summary>
        /// <param name="inputColor">The inputColor to be modified</param>
        /// <param name="colorFadeConstant">The constant specifying how faded the new color will be</param>
        /// <returns></returns>
        private Color FadeColor(Color inputColor, int colorFadeConstant)
        {
            // Add a constant to the red, green and blue components of the color to make it appear faded
            int colorR = inputColor.R + colorFadeConstant;
            int colorG = inputColor.G + colorFadeConstant;
            int colorB = inputColor.B + colorFadeConstant;

            if (colorR > 255)
            {
                colorR = 255;
            }
            else if (colorR < 0)
            {
                colorR = 0;
            }

            if (colorG > 255)
            {
                colorG = 255;
            }
            else if (colorG < 0)
            {
                colorG = 0;
            }

            if (colorB > 255)
            {
                colorB = 255;
            }
            else if (colorB < 0)
            {
                colorB = 0;
            }

            return Color.FromArgb(colorR, colorG, colorB);
        }

        #endregion
    }
}
