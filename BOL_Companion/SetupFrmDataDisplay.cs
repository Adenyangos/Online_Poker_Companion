using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static BOL_Companion.frmDataDisplay;

namespace BOL_Companion
{
    class SetupFrmDataDisplay
    {
        Font fntChartTitles, fntPlayerName, fntNoData, fntSampleCount, fntChartLabels, fntLegend, fntButton;
        Pen penRect, penLine;
        public Color clrRtbBackground, clrCustomGrey;
        frmDataDisplay frm;

        public SetupFrmDataDisplay(frmDataDisplay frmIn, Color clrRtbBackgroundIn)
        {
            frm = frmIn;
            clrRtbBackground = clrRtbBackgroundIn;

            InitializeForm();
            InitializeVariables();
            InitializeControls();
            SetControlLocations();
            frm.VisualPreFlop();
        }

        private void InitializeForm()
        {
            frm.Location = Screen.AllScreens[1].WorkingArea.Location;
            frm.WindowState = FormWindowState.Maximized;
            frm.Text = "Adam's Bet Online Poker Data Display";
            frm.BackColor = clrRtbBackground;
        }

        private void InitializeVariables()
        {
            int intFadeConst = -125;

            fntChartTitles= new Font(frm.Font.FontFamily, 10, FontStyle.Bold);
            fntPlayerName = new Font(frm.Font.FontFamily, 20, FontStyle.Bold | FontStyle.Underline);
            fntNoData = new Font(frm.Font.FontFamily, 18, FontStyle.Bold);
            fntSampleCount = new Font(frm.Font.FontFamily, 10);
            fntChartLabels = new Font(frm.Font.FontFamily, 12, FontStyle.Bold);
            fntLegend = new Font(frm.Font.FontFamily, 12, FontStyle.Bold);
            fntButton = new Font(frm.Font.FontFamily, 18, FontStyle.Bold);

            frm.clrFold = Color.SpringGreen;
            frm.clrCheck = Color.DodgerBlue;
            frm.clrCall = Color.FromArgb(185, 100, 255);
            frm.clrBet = Color.FromArgb(255, 35, 105);

            frm.clrFold_Fadded = FadeColor(frm.clrFold, intFadeConst);
            frm.clrCheck_Fadded = FadeColor(frm.clrCheck, intFadeConst);
            frm.clrCall_Fadded = FadeColor(frm.clrCall, intFadeConst);
            frm.clrBet_Fadded = FadeColor(frm.clrBet, intFadeConst);
            frm.clrWhite_Fadded = FadeColor(Color.White, -100);

            clrCustomGrey = Color.FromArgb(135, 135, 135);

            penRect = new Pen(Color.FromArgb(255, 255, 255), 2);
            penLine = new Pen(clrCustomGrey, 1);

            frm.blnChtUpdated = new bool[10][];
            for (int i = 0; i < 10; i++)
            {
                frm.blnChtUpdated[i] = new bool[4];
                for (int j = 0; j < 4; j++)
                {
                    frm.blnChtUpdated[i][j] = false;
                }
            }
        }

        private void InitializeControls()
        {
            #region Panels

            frm.pnlData = new Panel();
            frm.pnlData.BackColor = Color.Transparent;
            frm.pnlData.Anchor = AnchorStyles.None;
            frm.pnlData.BorderStyle = BorderStyle.FixedSingle;
            frm.Controls.Add(frm.pnlData);
            frm.pnlData.Paint += pnlData_Paint;

            frm.pnlLegendFold = new Panel();
            frm.pnlLegendFold.BackColor = frm.clrFold;
            frm.pnlLegendFold.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.pnlLegendFold);

            frm.pnlLegendCheck = new Panel();
            frm.pnlLegendCheck.BackColor = frm.clrCheck;
            frm.pnlLegendCheck.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.pnlLegendCheck);

            frm.pnlLegendCall = new Panel();
            frm.pnlLegendCall.BackColor = frm.clrCall;
            frm.pnlLegendCall.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.pnlLegendCall);

            frm.pnlLegendBet = new Panel();
            frm.pnlLegendBet.BackColor = frm.clrBet;
            frm.pnlLegendBet.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.pnlLegendBet);

            frm.pnlDataUpdateStatus = new Panel[10][];
            frm.pnlDataUpdateStatusBorder = new Panel[10][];
            for (int i = 0; i < 10; i++)
            {
                frm.pnlDataUpdateStatus[i] = new Panel[4];
                frm.pnlDataUpdateStatusBorder[i] = new Panel[4];

                for (int j = 0; j < 4; j++)
                {
                    // Data Update Status Background Panels (a border for Data Update Staus Panels)
                    frm.pnlDataUpdateStatusBorder[i][j] = new Panel();
                    frm.pnlDataUpdateStatusBorder[i][j].BackColor = Color.Black;
                    frm.pnlDataUpdateStatusBorder[i][j].BorderStyle = BorderStyle.None;
                    frm.pnlDataUpdateStatusBorder[i][j].Anchor = AnchorStyles.None;
                    frm.pnlData.Controls.Add(frm.pnlDataUpdateStatusBorder[i][j]);
                    frm.pnlDataUpdateStatusBorder[i][j].BringToFront();

                    // Data Update Status Panels (colored squares that show if the data is up to date)
                    frm.pnlDataUpdateStatus[i][j] = new Panel();
                    frm.pnlDataUpdateStatus[i][j].BackColor = frm.clrBet;
                    frm.pnlDataUpdateStatus[i][j].BorderStyle = BorderStyle.None;
                    frm.pnlDataUpdateStatus[i][j].Anchor = AnchorStyles.None;
                    frm.pnlData.Controls.Add(frm.pnlDataUpdateStatus[i][j]);
                    frm.pnlDataUpdateStatus[i][j].BringToFront();
                }
            }

            #endregion

            #region Labels

            frm.lblFrmTitle = new Label();
            frm.lblFrmTitle.Font = new Font(frm.Font.FontFamily, 36, FontStyle.Bold);
            frm.lblFrmTitle.ForeColor = Color.White;
            frm.lblFrmTitle.Anchor = AnchorStyles.None;
            frm.lblFrmTitle.Text = "No Data to Display... Yet!";
            frm.Controls.Add(frm.lblFrmTitle);

            frm.lblTime = new Label();
            frm.lblTime.Font = new Font("Courier New", 24, FontStyle.Bold);
            frm.lblTime.ForeColor = Color.White;
            frm.lblTime.Anchor = AnchorStyles.None;
            frm.lblTime.Text = "00:00:00 AM";
            frm.Controls.Add(frm.lblTime);
            
            frm.lblProcessingTimeOthers = new Label();
            frm.lblProcessingTimeOthers.Font = new Font("Courier New", 10, FontStyle.Bold);
            frm.lblProcessingTimeOthers.ForeColor = Color.White;
            frm.lblProcessingTimeOthers.Anchor = AnchorStyles.None;
            frm.lblProcessingTimeOthers.Text = "Others: 0.000 sec.";
            frm.Controls.Add(frm.lblProcessingTimeOthers);

            frm.lblProcessingTimeJaba = new Label();
            frm.lblProcessingTimeJaba.Font = new Font("Courier New", 10, FontStyle.Bold);
            frm.lblProcessingTimeJaba.ForeColor = Color.White;
            frm.lblProcessingTimeJaba.Anchor = AnchorStyles.None;
            frm.lblProcessingTimeJaba.Text = "JabaAdam: 0.000 sec.";
            frm.Controls.Add(frm.lblProcessingTimeJaba);

            frm.lblLegendFold = new Label();
            frm.lblLegendFold.Font = fntLegend;
            frm.lblLegendFold.ForeColor = Color.White;
            frm.lblLegendFold.Anchor = AnchorStyles.None;
            frm.lblLegendFold.Text = "Fold";
            frm.Controls.Add(frm.lblLegendFold);

            frm.lblLegendCheck = new Label();
            frm.lblLegendCheck.Font = fntLegend;
            frm.lblLegendCheck.ForeColor = Color.White;
            frm.lblLegendCheck.Anchor = AnchorStyles.None;
            frm.lblLegendCheck.Text = "Check";
            frm.Controls.Add(frm.lblLegendCheck);

            frm.lblLegendCall = new Label();
            frm.lblLegendCall.Font = fntLegend;
            frm.lblLegendCall.ForeColor = Color.White;
            frm.lblLegendCall.Anchor = AnchorStyles.None;
            frm.lblLegendCall.Text = "Call";
            frm.Controls.Add(frm.lblLegendCall);

            frm.lblLegendBet = new Label();
            frm.lblLegendBet.Font = fntLegend;
            frm.lblLegendBet.ForeColor = Color.White;
            frm.lblLegendBet.Anchor = AnchorStyles.None;
            frm.lblLegendBet.Text = "Bet";
            frm.Controls.Add(frm.lblLegendBet);

            frm.lblPlayerName = new Label[10];
            frm.lblChartNoData = new Label[10][];
            frm.lblChartSampleCount = new Label[10][];
            for (int i = 0; i < 10; i++)
            {
                // Player Name Labels
                frm.lblPlayerName[i] = new Label();
                frm.lblPlayerName[i].Font = fntPlayerName;
                frm.lblPlayerName[i].ForeColor = Color.White;
                frm.lblPlayerName[i].Anchor = AnchorStyles.None;
                frm.lblPlayerName[i].BackColor = Color.Transparent;
                if (i < 9)
                {
                    frm.lblPlayerName[i].Text = "Player " + (i + 1).ToString();
                }
                else
                {
                    frm.lblPlayerName[i].Text = "Other Players at This Table";
                }
                frm.lblPlayerName[i].TextAlign = ContentAlignment.TopCenter;
                frm.pnlData.Controls.Add(frm.lblPlayerName[i]);

                // Now Chart Data Labels and Chart Sample Count Labels
                frm.lblChartNoData[i] = new Label[4];
                frm.lblChartSampleCount[i] = new Label[4];
                for (int j = 0; j < 4; j++)
                {
                    frm.lblChartNoData[i][j] = new Label();
                    frm.lblChartNoData[i][j].Font = fntNoData;
                    frm.lblChartNoData[i][j].Anchor = AnchorStyles.None;
                    frm.lblChartNoData[i][j].BackColor = Color.DarkGray;
                    frm.lblChartNoData[i][j].Text = "No Data";
                    frm.lblChartNoData[i][j].Visible = false;
                    frm.pnlData.Controls.Add(frm.lblChartNoData[i][j]);

                    frm.lblChartSampleCount[i][j] = new Label();
                    frm.lblChartSampleCount[i][j].Font = fntSampleCount;
                    frm.lblChartSampleCount[i][j].Anchor = AnchorStyles.None;
                    frm.lblChartSampleCount[i][j].BackColor = Color.Transparent;
                    frm.lblChartSampleCount[i][j].Text = "XXX";
                    frm.pnlData.Controls.Add(frm.lblChartSampleCount[i][j]);
                }
            }

            #endregion

            #region Buttons

            frm.btnPreFlop = new Button();
            frm.btnPreFlop.Font = fntButton;
            frm.btnPreFlop.ForeColor = Color.DarkGray;
            frm.btnPreFlop.BackColor = Color.LightGray;
            frm.btnPreFlop.Anchor = AnchorStyles.None;
            frm.btnPreFlop.Text = "Pre-Flop";
            frm.btnPreFlop.Enabled = false;
            frm.Controls.Add(frm.btnPreFlop);
            frm.btnPreFlop.Click += new EventHandler(frm.btnPreFlop_Click);

            frm.btnFlop = new Button();
            frm.btnFlop.Font = fntButton;
            frm.btnFlop.ForeColor = Color.Black;
            frm.btnFlop.BackColor = Color.DarkGray;
            frm.btnFlop.Anchor = AnchorStyles.None;
            frm.btnFlop.Text = "Flop";
            frm.btnFlop.Enabled = true;
            frm.Controls.Add(frm.btnFlop);
            frm.btnFlop.Click += new EventHandler(frm.btnFlop_Click);

            frm.btnTurn = new Button();
            frm.btnTurn.Font = fntButton;
            frm.btnTurn.ForeColor = Color.Black;
            frm.btnTurn.BackColor = Color.DarkGray;
            frm.btnTurn.Anchor = AnchorStyles.None;
            frm.btnTurn.Text = "Turn";
            frm.btnTurn.Enabled = true;
            frm.Controls.Add(frm.btnTurn);
            frm.btnTurn.Click += new EventHandler(frm.btnTurn_Click);

            frm.btnRiver = new Button();
            frm.btnRiver.Font = fntButton;
            frm.btnRiver.ForeColor = Color.Black;
            frm.btnRiver.BackColor = Color.DarkGray;
            frm.btnRiver.Anchor = AnchorStyles.None;
            frm.btnRiver.Text = "River";
            frm.btnRiver.Enabled = true;
            frm.Controls.Add(frm.btnRiver);
            frm.btnRiver.Click += new EventHandler(frm.btnRiver_Click);

            #endregion

            #region Chart Arrays

            // This is one array of 4 charts. 10 of these arrays will exist in the panel
            frm.chtQuad = new MyChart[10][];

            for (int i = 0; i < 10; i++)
            {
                frm.chtQuad[i] = new MyChart[4];
            }

            // Creat the 4 series names
            string[] strChtSeriesNames = new string[4];
            strChtSeriesNames[0] = "This Table -\nCurrent Num. Players";
            strChtSeriesNames[1] = "This Table -\nAny Num. Players";
            strChtSeriesNames[2] = "Any Table -\nCurrent Num. Players";
            strChtSeriesNames[3] = "Any Table -\nAny Num. Players";

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    frm.chtQuad[i][j] = new MyChart();
                    ChartArea cha = new ChartArea();

                    frm.chtQuad[i][j].Series.Add(strChtSeriesNames[j]);
                    frm.chtQuad[i][j].Series[0].ChartType = SeriesChartType.Pie;
                    frm.chtQuad[i][j].Series[0].Points.AddXY("25%", 2);
                    frm.chtQuad[i][j].Series[0].Points.AddXY("25%", 2);
                    frm.chtQuad[i][j].Series[0].Points.AddXY("25%", 2);
                    frm.chtQuad[i][j].Series[0].Points.AddXY("25%", 2);
                    frm.chtQuad[i][j].Series[0].Points[0].Color = frm.clrFold;
                    frm.chtQuad[i][j].Series[0].Points[1].Color = frm.clrCheck;
                    frm.chtQuad[i][j].Series[0].Points[2].Color = frm.clrCall;
                    frm.chtQuad[i][j].Series[0].Points[3].Color = frm.clrBet;
                    frm.chtQuad[i][j].Series[0].Font = fntChartLabels;
                    frm.chtQuad[i][j].Series[0]["PieStartAngle"] = "270";
                    frm.chtQuad[i][j].ChartAreas.Add(cha);                    
                    frm.chtQuad[i][j].ChartAreas[0].BackColor = Color.Transparent;
                    frm.chtQuad[i][j].Titles.Add(strChtSeriesNames[j]);
                    frm.chtQuad[i][j].Titles[0].ForeColor = Color.White;
                    frm.chtQuad[i][j].Titles[0].Font = fntChartTitles;
                    frm.chtQuad[i][j].Titles[0].DockingOffset = 3;
                    frm.chtQuad[i][j].BackColor = Color.Transparent;
                    frm.chtQuad[i][j].I = i;
                    frm.chtQuad[i][j].J = j;
                    frm.pnlData.Controls.Add(frm.chtQuad[i][j]);

                    frm.chtQuad[i][j].PostPaint += frm.chtQuad_PostPaint;
                }
            }

            #endregion
        }

        private void SetControlLocations()
        {
            int intFrmWidth, intFrmHeight;
            Size szeLegendPnl, szeButton;

            intFrmWidth = frm.ClientSize.Width;
            intFrmHeight = frm.ClientSize.Height;

            szeLegendPnl = new Size(32, 14);
            szeButton = new Size(130, 45);

            frm.lblFrmTitle.AutoSize = true;
            frm.lblTime.AutoSize = true;
            frm.lblProcessingTimeOthers.AutoSize = true;
            frm.lblProcessingTimeJaba.AutoSize = true;
            frm.lblLegendFold.AutoSize = true;
            frm.lblLegendCheck.AutoSize = true;
            frm.lblLegendCall.AutoSize = true;
            frm.lblLegendBet.AutoSize = true;
            frm.btnPreFlop.Size = szeButton;
            frm.btnFlop.Size = szeButton;
            frm.btnTurn.Size = szeButton;
            frm.btnRiver.Size = szeButton;
            frm.pnlData.Size = new Size(intFrmWidth - 30, intFrmHeight - 90);
            frm.pnlLegendFold.Size = szeLegendPnl;
            frm.pnlLegendCheck.Size = szeLegendPnl;
            frm.pnlLegendCall.Size = szeLegendPnl;
            frm.pnlLegendBet.Size = szeLegendPnl;

            for (int i = 0; i < 10; i++)
            {
                frm.lblPlayerName[i].Size = new Size(frm.pnlData.Width / 5, 32);

                for (int j = 0; j < 4; j++)
                {
                    frm.pnlDataUpdateStatus[i][j].Size = new Size(14, 14);
                    frm.pnlDataUpdateStatusBorder[i][j].Size = new Size(18, 18);
                    frm.lblChartNoData[i][j].AutoSize = true;
                    frm.lblChartSampleCount[i][j].AutoSize = true;
                    frm.chtQuad[i][j].Size = new Size(frm.pnlData.Width / 10, (frm.pnlData.Height - 40) / 4);
                }
            }

            frm.lblFrmTitle.Location = new Point(intFrmWidth / 2 - frm.lblFrmTitle.Width / 2, 10);
            frm.lblTime.Location = new Point(1683, 8);
            frm.btnPreFlop.Location = new Point(15, 25);
            frm.btnFlop.Location = new Point(155, 25);
            frm.btnTurn.Location = new Point(295, 25);
            frm.btnRiver.Location = new Point(435, 25);
            frm.pnlData.Location = new Point(intFrmWidth / 2 - frm.pnlData.Width / 2, (intFrmHeight - 60) / 2 - frm.pnlData.Height / 2 + 60);
            frm.lblProcessingTimeOthers.Location = new Point(frm.pnlData.Location.X + frm.pnlData.Width - frm.lblProcessingTimeOthers.Width + 4, 1001);
            frm.lblProcessingTimeJaba.Location = new Point(frm.pnlData.Location.X, 1001);

            frm.lblLegendBet.Location = new Point(1867, 48);
            frm.pnlLegendBet.Location = new Point(frm.lblLegendBet.Location.X - 33, 50);
            frm.lblLegendCall.Location = new Point(frm.pnlLegendBet.Location.X - frm.lblLegendCall.Width - 19, 48);
            frm.pnlLegendCall.Location = new Point(frm.lblLegendCall.Location.X - 33, 50);
            frm.lblLegendCheck.Location = new Point(frm.pnlLegendCall.Location.X - frm.lblLegendCheck.Width - 19, 48);
            frm.pnlLegendCheck.Location = new Point(frm.lblLegendCheck.Location.X - 33, 50);
            frm.lblLegendFold.Location = new Point(frm.pnlLegendCheck.Location.X - frm.lblLegendFold.Width - 19, 48);
            frm.pnlLegendFold.Location = new Point(frm.lblLegendFold.Location.X - 33, 50);

            for (int i = 0; i < 5; i++)
            {
                frm.lblPlayerName[i].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 - frm.lblPlayerName[i].Width / 2, 4);
                frm.lblPlayerName[i + 5].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 - frm.lblPlayerName[i + 5].Width / 2, frm.pnlData.Height / 2 + 4);

                frm.pnlDataUpdateStatus[i][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 21, 85);
                frm.pnlDataUpdateStatus[i][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 21, 85);
                frm.pnlDataUpdateStatus[i][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 21, frm.pnlData.Height / 4 + 69);
                frm.pnlDataUpdateStatus[i][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 21, frm.pnlData.Height / 4 + 69);

                frm.pnlDataUpdateStatus[i + 5][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 21, frm.pnlData.Height / 2 + 85);
                frm.pnlDataUpdateStatus[i + 5][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 21, frm.pnlData.Height / 2 + 85);
                frm.pnlDataUpdateStatus[i + 5][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 21, 3 * frm.pnlData.Height / 4 + 69);
                frm.pnlDataUpdateStatus[i + 5][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 21, 3 * frm.pnlData.Height / 4 + 69);

                frm.pnlDataUpdateStatusBorder[i][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 19, 83);
                frm.pnlDataUpdateStatusBorder[i][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 19, 83);
                frm.pnlDataUpdateStatusBorder[i][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 19, frm.pnlData.Height / 4 + 67);
                frm.pnlDataUpdateStatusBorder[i][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 19, frm.pnlData.Height / 4 + 67);

                frm.pnlDataUpdateStatusBorder[i + 5][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 19, frm.pnlData.Height / 2 + 83);
                frm.pnlDataUpdateStatusBorder[i + 5][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 19, frm.pnlData.Height / 2 + 83);
                frm.pnlDataUpdateStatusBorder[i + 5][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 19, 3 * frm.pnlData.Height / 4 + 67);
                frm.pnlDataUpdateStatusBorder[i + 5][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 19, 3 * frm.pnlData.Height / 4 + 67);

                frm.chtQuad[i][0].Location = new Point(2 * i * frm.pnlData.Width / 10, 33);
                frm.chtQuad[i][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10, 33);
                frm.chtQuad[i][2].Location = new Point(2 * i * frm.pnlData.Width / 10, frm.pnlData.Height / 4 + 17);
                frm.chtQuad[i][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10, frm.pnlData.Height / 4 + 17);

                frm.chtQuad[i + 5][0].Location = new Point(2 * i * frm.pnlData.Width / 10, frm.pnlData.Height / 2 + 33);
                frm.chtQuad[i + 5][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10, frm.pnlData.Height / 2 + 33);
                frm.chtQuad[i + 5][2].Location = new Point(2 * i * frm.pnlData.Width / 10, 3 * frm.pnlData.Height / 4 + 17);
                frm.chtQuad[i + 5][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10, 3 * frm.pnlData.Height / 4 + 17);

                frm.lblChartNoData[i][0].Location = new Point((4 * i + 1) * frm.pnlData.Width / 20 - frm.lblChartNoData[i][0].Width / 2, 147);
                frm.lblChartNoData[i][1].Location = new Point((4 * i + 3) * frm.pnlData.Width / 20 - frm.lblChartNoData[i][1].Width / 2, 147);
                frm.lblChartNoData[i][2].Location = new Point((4 * i + 1) * frm.pnlData.Width / 20 - frm.lblChartNoData[i][2].Width / 2, frm.pnlData.Height / 4 + 131);
                frm.lblChartNoData[i][3].Location = new Point((4 * i + 3) * frm.pnlData.Width / 20 - frm.lblChartNoData[i][3].Width / 2, frm.pnlData.Height / 4 + 131);

                frm.lblChartNoData[i + 5][0].Location = new Point((4 * i + 1) * frm.pnlData.Width / 20 - frm.lblChartNoData[i + 5][0].Width / 2, frm.pnlData.Height / 2 + 147);
                frm.lblChartNoData[i + 5][1].Location = new Point((4 * i + 3) * frm.pnlData.Width / 20 - frm.lblChartNoData[i + 5][1].Width / 2, frm.pnlData.Height / 2 + 147);
                frm.lblChartNoData[i + 5][2].Location = new Point((4 * i + 1) * frm.pnlData.Width / 20 - frm.lblChartNoData[i + 5][2].Width / 2, 3 * frm.pnlData.Height / 4 + 131);
                frm.lblChartNoData[i + 5][3].Location = new Point((4 * i + 3) * frm.pnlData.Width / 20 - frm.lblChartNoData[i + 5][3].Width / 2, 3 * frm.pnlData.Height / 4 + 131);

                frm.lblChartSampleCount[i][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 140, 228);
                frm.lblChartSampleCount[i][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 140, 228);
                frm.lblChartSampleCount[i][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 140, frm.pnlData.Height / 4 + 212);
                frm.lblChartSampleCount[i][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 140, frm.pnlData.Height / 4 + 212);

                frm.lblChartSampleCount[i + 5][0].Location = new Point(2 * i * frm.pnlData.Width / 10 + 140, frm.pnlData.Height / 2 + 228);
                frm.lblChartSampleCount[i + 5][1].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 140, frm.pnlData.Height / 2 + 228);
                frm.lblChartSampleCount[i + 5][2].Location = new Point(2 * i * frm.pnlData.Width / 10 + 140, 3 * frm.pnlData.Height / 4 + 212);
                frm.lblChartSampleCount[i + 5][3].Location = new Point((2 * i + 1) * frm.pnlData.Width / 10 + 140, 3 * frm.pnlData.Height / 4 + 212);
            }
        }

        #region Events

        // Draw seperator lines inside of the panel
        private void pnlData_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = ((Panel)sender);
            if (pnl.Visible)
            {
                int intPnlMid;

                intPnlMid = (pnl.Height - 2) / 2;

                // horizontal lines
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 0 / 5 + 15, intPnlMid), new Point(pnl.Width * 1 / 5 - 15, intPnlMid));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 1 / 5 + 15, intPnlMid), new Point(pnl.Width * 2 / 5 - 15, intPnlMid));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 2 / 5 + 15, intPnlMid), new Point(pnl.Width * 3 / 5 - 15, intPnlMid));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 3 / 5 + 15, intPnlMid), new Point(pnl.Width * 4 / 5 - 15, intPnlMid));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 4 / 5 + 15, intPnlMid), new Point(pnl.Width * 5 / 5 - 15, intPnlMid));

                // vertical lines
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 1 / 5, 15), new Point(pnl.Width * 1 / 5, intPnlMid * 1 - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 1 / 5, intPnlMid + 15), new Point(pnl.Width * 1 / 5, pnl.Height - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 2 / 5, 15), new Point(pnl.Width * 2 / 5, intPnlMid * 1 - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 2 / 5, intPnlMid + 15), new Point(pnl.Width * 2 / 5, pnl.Height - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 3 / 5, 15), new Point(pnl.Width * 3 / 5, intPnlMid * 1 - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 3 / 5, intPnlMid + 15), new Point(pnl.Width * 3 / 5, pnl.Height - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 4 / 5, 15), new Point(pnl.Width * 4 / 5, intPnlMid * 1 - 15));
                e.Graphics.DrawLine(penLine, new Point(pnl.Width * 4 / 5, intPnlMid + 15), new Point(pnl.Width * 4 / 5, pnl.Height - 15));

                // Panel border
                e.Graphics.DrawRectangle(penRect, 0, 0, pnl.Width - 2, pnl.Height - 2);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts a color to the fadded version of that color
        /// </summary>
        /// <param name="clrInput"></param>
        /// <param name="intFadeConstant"></param>
        /// <returns></returns>
        private Color FadeColor(Color clrInput, int intFadeConstant)
        {
            int intRed, intGreen, intBlue;
            Color clrOutput;

            intRed = clrInput.R + intFadeConstant;
            intGreen = clrInput.G + intFadeConstant;
            intBlue = clrInput.B + intFadeConstant;

            if (intRed > 255)
            {
                intRed = 255;
            }
            else if (intRed < 0)
            {
                intRed = 0;
            }

            if (intGreen > 255)
            {
                intGreen = 255;
            }
            else if (intGreen < 0)
            {
                intGreen = 0;
            }

            if (intBlue > 255)
            {
                intBlue = 255;
            }
            else if (intBlue < 0)
            {
                intBlue = 0;
            }

            clrOutput = Color.FromArgb(intRed, intGreen, intBlue);

            return clrOutput;
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
