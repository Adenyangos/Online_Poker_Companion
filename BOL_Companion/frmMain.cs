using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOL_Companion
{
    public partial class frmMain : Form
    {

        #region Variable Definitions

        bool blnDisposeScreenShotResources, blnSaveBitmaps, blnRunTimers;
        bool blnNewPot, blnNewDealer, blnNewDealerPrev, blnNewActionPlayer, blnNextSsEnabled;
        bool blnPotChangePrevious, blnDealerChangePrevious, blnActionPlayerChangePrevious, blnBoardChangePrevious;
        bool blnReRunTasks, bln9PlayerTable, blnShowdown, blnFirstScreenShot, blnDontUpdateData, blnDealerChipNotFound;
        bool blnPokerCompanionRunning, blnAllInChipStacksCorrected;
        int intCurrentSsFile, intJobCompletionCounter, intJobStartedCounter, intBbPlayer, intSbPlayer, intCorrectedAllInBet;
        int intDbGameId, intDbHandActionNum, intScreenWidth, intScreenHeight, intScreenLocX, intScreenLocY;
        long lngDbHandId;
        const int intTaskCount = 17;
        int[] intPot, intDealer, intActionPlayer;
        string strScreenshotFilePath, strBitmapSavePath, strPlayerOfInterest;
        public string strMouseLabelX, strMouseLabelY, strMouseLabelClr, strMouseLabelBright;
        string[] strSsFiles;
        public Color clrNormalText, clrDataChange, clrDataId, clrRtbBackground, clrControlDisabled;
        Pen penWhite, penBlack, penRed, penGreen, penBlue, PenPurple;
        Bitmap bmpScreenShot, bmpMyHand;
        Graphics gfxScreenShot;
        Stopwatch stpBitmapCopies, stpOverallTime, stpUiIdleTime, stpUiUpdate;

        SetupFrmMain SetupFrmMain_;
        Players[] plr;
        Board[] brd;
        DbController Db_;
        ProcessScreenShots[] pssWorkers;
        Task[] tskWorkers;
        Stopwatch[] stpWorkers;

        #endregion

        #region Control Definitions

        public ToolTip tipFrmMain;
        public GroupBox grpBasicSettings, grpProgramControl, grpBitmapLocationTools, grpErrorLogging;
        public Label lblPlayerOfInterest, lblRectX, lblRectY, lblRectWidth, lblRectHeight, lblMouseClickX, lblMouseClickY, lblMouseClickClr, lblMouseClickBright;
        public CheckBox chkClearRects, chkSaveBitmaps, chkAutoNextScreenShot, chkShowClickData;
        public RadioButton rdoModeLiveGame, rdoModeScreenshotMode, rdo9PlayerTable, rdo10PlayerTable;
        public TextBox txtPlayerOfInterest, txtRectX, txtRectY, txtRectWidth, txtRectHeight, txtErrorMessages;
        public Button btnChangeBitmapSaveLocation, btnStartPokerCompanion, btnOpenScreenShotFile, btnNextScreenShot, btnCopyBitmapsForWorkers, btnDrawRect, btnDrawAllRects, btnClearAllDbData;
        public RichTextBox rtbPotDealerAction, rtbStatus, rtbBoard, rtbUiIdleTime, rtbTotalProcessTime, rtbNewHandDetected;
        public DataGridView dgvPlayers, dgvTimers;
        public PictureBox picScreenShot, picMyHand;
        frmDataDisplay frmDataDisplay;

        #endregion

        #region Form Initialization

        public frmMain()
        {
            InitializeComponent();
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeVariables();
            InitializeThisForm();
            InitializePlayers();
            InitializeBoard();
            InitializeScreenShotHandlers();
            InitializeStopWatches();
            InitializeTasks();
            CreateFrmDataDisplay();
            DetectScreenCount();
        }

        private void InitializeThisForm()
        {
            SetupFrmMain_ = new SetupFrmMain(this);
        }

        private void InitializeVariables()
        {
            blnDisposeScreenShotResources = false;
            blnSaveBitmaps = false;
            blnRunTimers = true;
            blnNewPot = false;
            blnNewDealer = false;
            blnNewDealerPrev = false;
            blnNewActionPlayer = false;
            blnPotChangePrevious = false;
            blnDealerChangePrevious = false;
            blnActionPlayerChangePrevious = false;
            blnBoardChangePrevious = false;
            blnNextSsEnabled = false;
            blnReRunTasks = false;
            bln9PlayerTable = true;
            blnShowdown = false;
            blnFirstScreenShot = true;
            blnDontUpdateData = false;
            blnDealerChipNotFound = false;
            blnAllInChipStacksCorrected = false;
            intJobCompletionCounter = 1;
            intJobStartedCounter = 1;
            intBbPlayer = -1;
            intSbPlayer = -1;
            intDbGameId = -1;
            lngDbHandId = -1;
            intDbHandActionNum = -1;
            intCorrectedAllInBet = 0;
            intScreenWidth = Screen.AllScreens[0].WorkingArea.Width;
            intScreenHeight = Screen.AllScreens[0].WorkingArea.Height;
            intScreenLocX = Screen.AllScreens[0].WorkingArea.Location.X;
            intScreenLocY = Screen.AllScreens[0].WorkingArea.Location.Y + SystemInformation.CaptionHeight;
            strScreenshotFilePath = "";
            strBitmapSavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            strPlayerOfInterest = "JabaAdam";
            strMouseLabelX = "Mouse Click X Coordinate: ";
            strMouseLabelY = "Mouse Click Y Coordinate: ";
            strMouseLabelClr = "Mouse Click Color: ";
            strMouseLabelBright = "Mouse Click Brightness: ";

            intPot = new int[2] { -1, -1 };
            intDealer = new int[2] { -1, -1 };
            intActionPlayer = new int[2] { -1, -1 };

            clrNormalText = Color.White;
            clrDataChange = Color.Blue;
            clrDataId = Color.SpringGreen;
            clrRtbBackground = Color.DimGray;
            clrControlDisabled = Color.FromArgb(85, 85, 85);

            penWhite = new Pen(Color.White, 1);
            penBlack = new Pen(Color.Black, 1);
            penRed = new Pen(Color.Red, 1);
            penGreen = new Pen(Color.Green, 1);
            penBlue = new Pen(Color.Blue, 1);
            PenPurple = new Pen(Color.Purple, 1);

            Db_ = new DbController(this);
        }

        private void DetectScreenCount()
        {
            if (Screen.AllScreens.Count() < 2)
            {
                MessageBox.Show("Warning! Only one monitor has been detected. This application was designed to be run with at least two monitors. This application will contiue to run but be aware that some information may not be displayed properly or hidden on windows that are not visible.", "Single monitor detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        async private void InitializeTasks()
        {
            tskWorkers = new Task[intTaskCount];
            CreateTasks(true);

            if (bln9PlayerTable)
            {
                tskWorkers[9].Start();
                await tskWorkers[9];
            }
        }

        #region Initialize Objects

        private void InitializePlayers()
        {
            plr = new Players[10];

            for (int i = 0; i < 10; i++)
            {
                plr[i] = new Players();
            }
        }

        private void InitializeBoard()
        {
            brd = new Board[5];

            for (int i = 0; i < 5; i++)
            {
                brd[i] = new Board();
            }
        }

        private void InitializeScreenShotHandlers()
        {
            pssWorkers = new ProcessScreenShots[intTaskCount];

            for (int i = 0; i < intTaskCount; i++)
            {
                pssWorkers[i] = new ProcessScreenShots(i, strBitmapSavePath);
            }
        }

        private void InitializeStopWatches()
        {
            stpBitmapCopies = new Stopwatch();
            stpOverallTime = new Stopwatch();
            stpUiIdleTime = new Stopwatch();
            stpUiUpdate = new Stopwatch();

            stpWorkers = new Stopwatch[intTaskCount];

            for (int i = 0; i < intTaskCount; i++)
            {
                stpWorkers[i] = new Stopwatch();
            }
        }

        #endregion

        private void CreateNewGameInDb()
        {
            // insert new game into DB and get GameId
            intDbGameId = Db_.InsertGame(true, 9);
        }

        private void CreateFrmDataDisplay()
        {
            // Pass clrRtbBackground to the frmDataDisplay form to use it as the background color
            frmDataDisplay = new frmDataDisplay(clrRtbBackground);
            frmDataDisplay.PlayerOfInterest(strPlayerOfInterest, -1);
            frmDataDisplay.Show();
            frmDataDisplay.ZeroData();
        }

        #endregion

        #region Control Events

        /// <summary>
        /// This is the main event that does everyting important
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnOpenScreenShotFile_Click(object sender, EventArgs e)
        {
            btnStartPokerCompanion.Enabled = false;
            btnOpenScreenShotFile.Enabled = false;
            chkAutoNextScreenShot.Enabled = false;
            btnNextScreenShot.Enabled = false;

            OpenFileDialog ofdOpenFileDialog = new OpenFileDialog();

            ofdOpenFileDialog.InitialDirectory = "E:\\Documents\\Visual Studio 2015\\Projects\\Online Poker\\BetOnline\\Screen Shot Analysis";
            ofdOpenFileDialog.Filter = "BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
            ofdOpenFileDialog.FilterIndex = 0;
            ofdOpenFileDialog.RestoreDirectory = false;
            ofdOpenFileDialog.Title = "Select Screen Shot to Open";

            if (ofdOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (blnRunTimers)
                {
                    stpOverallTime.Start();
                }

                // Disable the clear all database data button to prevent clearing the database in the middle of a hand as this would cause problems with the 
                // data.
                btnClearAllDbData.Enabled = false;

                try
                {
                    if ((strScreenshotFilePath = ofdOpenFileDialog.FileName) != null)
                    {
                        // get all the .bmp files in the selected directory
                        string strDirectoryPath = Path.GetDirectoryName(strScreenshotFilePath);
                        strSsFiles = Directory.GetFiles(strDirectoryPath, "*.bmp", SearchOption.TopDirectoryOnly);

                        // find the index of the selected .bmp file
                        intCurrentSsFile = Array.IndexOf(strSsFiles, strScreenshotFilePath);

                        if (blnDisposeScreenShotResources)
                        {
                            bmpScreenShot.Dispose();
                            gfxScreenShot.Dispose();
                        }

                        if (chkShowClickData.Checked)
                        {
                            lblMouseClickX.Text = strMouseLabelX;
                            lblMouseClickY.Text = strMouseLabelY;
                            lblMouseClickClr.Text = strMouseLabelClr;
                            lblMouseClickBright.Text = strMouseLabelBright;
                        }

                        bmpScreenShot = (Bitmap)Bitmap.FromFile(strScreenshotFilePath);
                        gfxScreenShot = Graphics.FromImage(bmpScreenShot);

                        picScreenShot.Image = bmpScreenShot;
                        blnDisposeScreenShotResources = true;

                        // Must create a new game in the Db here for the subsequent processes to refer to
                        CreateNewGameInDb();
                        UpdateAllData();
                    }
                    blnNextSsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else if (blnNextSsEnabled)
            {
                btnStartPokerCompanion.Enabled = true;
                btnOpenScreenShotFile.Enabled = true;
                chkAutoNextScreenShot.Enabled = true;
                btnNextScreenShot.Enabled = true;
                btnOpenScreenShotFile.Focus();
            }
            else
            {
                btnOpenScreenShotFile.Enabled = true;
                btnOpenScreenShotFile.Focus();
            }
        }

        /// <summary>
        /// Click Event for btnStartPokerCompanion. This method starts taking screenshots of the poker game, saves and processes the data and produces graphs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnStartPokerCompanion_Click(object sender, EventArgs e)
        {
            // Disable the clear all database data button to prevent clearing the database in the middle of a hand as this would cause problems with the 
            // data.
            btnClearAllDbData.Enabled = false;

            // sender == null when this method is called from UpdateAllData to repeat the process. sender != null if the button is actually clicked by the user
            if (sender != null && blnPokerCompanionRunning)
            {
                blnPokerCompanionRunning = false;
                btnOpenScreenShotFile.Enabled = true;
                chkAutoNextScreenShot.Enabled = true;
                btnNextScreenShot.Enabled = true;

                btnStartPokerCompanion.Text = "Start Poker Companion";
            }
            else
            {
                if (sender != null && !blnPokerCompanionRunning)
                {
                    blnPokerCompanionRunning = true;
                    btnOpenScreenShotFile.Enabled = false;
                    chkAutoNextScreenShot.Enabled = false;
                    btnNextScreenShot.Enabled = false;

                    btnStartPokerCompanion.Text = "Stop Poker Companion";

                    // Must create a new game in the Db here for the subsequent processes to refer to
                    CreateNewGameInDb();
                }

                if (blnPokerCompanionRunning)
                {
                    if (blnDisposeScreenShotResources)
                    {
                        bmpScreenShot.Dispose();
                        gfxScreenShot.Dispose();
                    }

                    bmpScreenShot = new Bitmap(intScreenWidth, intScreenHeight - SystemInformation.CaptionHeight);

                    gfxScreenShot = Graphics.FromImage(bmpScreenShot as Image);
                    gfxScreenShot.CopyFromScreen(intScreenLocX, intScreenLocY, 0, 0, bmpScreenShot.Size);
                    picScreenShot.Image = bmpScreenShot;

                    blnDisposeScreenShotResources = true;

                    stpOverallTime.Start();
                    UpdateAllData();
                }
            }
        }

        public void btnNextScreenShot_Click(object sender, EventArgs e)
        {
            btnStartPokerCompanion.Enabled = false;
            btnOpenScreenShotFile.Enabled = false;
            btnNextScreenShot.Enabled = false;

            if (blnRunTimers)
            {
                stpOverallTime.Start();
            }

            try
            {
                if (strSsFiles.Length > 0)
                {
                    if (intCurrentSsFile < strSsFiles.Length)
                    {
                        intCurrentSsFile++;
                    }
                    else
                    {
                        intCurrentSsFile = 0;
                    }

                    bmpScreenShot.Dispose();
                    gfxScreenShot.Dispose();

                    bmpScreenShot = (Bitmap)Bitmap.FromFile(strSsFiles[intCurrentSsFile]);
                    gfxScreenShot = Graphics.FromImage(bmpScreenShot);

                    picScreenShot.Image = bmpScreenShot;

                    UpdateAllData();
                }
                else
                {
                    MessageBox.Show("No next file available!!!", "No Next File");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        public void btnCopyBitmapsForWorkers_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < intTaskCount; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    pssWorkers[i].NewScreenShot(new Bitmap((Bitmap)bmpScreenShot.Clone(pssWorkers[i].BitmapRect(i, 0), bmpScreenShot.PixelFormat)));
                    pssWorkers[i].SaveBitmap();
                }
            }
        }

        /// <summary>
        /// Change the location where the Bitmap files that are used to read the state of the game are saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnChangeBitmapSaveLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbdOpenDirectoryDialog = new FolderBrowserDialog();

            fbdOpenDirectoryDialog.SelectedPath = strBitmapSavePath;
            fbdOpenDirectoryDialog.ShowNewFolderButton = true;
            fbdOpenDirectoryDialog.Description = "Select Location to Save Bitmap Files";

            if (fbdOpenDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                strBitmapSavePath = fbdOpenDirectoryDialog.SelectedPath;

                for (int i = 0; i < intTaskCount; i++)
                {
                    pssWorkers[i].ChangeBitmapSaveLocation(strBitmapSavePath);
                }
            }
        }

        /// <summary>
        /// Clear all data in all the tables of the database and restart the Id numbering at 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnClearAllDbData_Click(object sender, EventArgs e)
        {
            Db_.ClearAllDbData();
        }

        public void btnDrawRect_Click(object sender, EventArgs e)
        {
            int intX, intY, intWidth, intHeight;

            if (int.TryParse(txtRectX.Text, out intX) && int.TryParse(txtRectY.Text, out intY) &&
                int.TryParse(txtRectWidth.Text, out intWidth) && int.TryParse(txtRectHeight.Text, out intHeight) && strScreenshotFilePath != "")
            {
                DrawRect(penWhite, intX, intY, intWidth, intHeight, chkClearRects.Checked);
            }
        }

        public void btnDrawAllRects_Click(object sender, EventArgs e)
        {
            Rectangle rctToDraw;
            for (int i = 0; i < intTaskCount; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    rctToDraw = pssWorkers[i].BitmapRect(i, 0);
                    DrawRect(penWhite, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                    if (i < 10)
                    {
                        // Player's first hold card
                        rctToDraw = pssWorkers[i].BitmapRect(i, 1);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player's second hold card
                        rctToDraw = pssWorkers[i].BitmapRect(i, 2);
                        DrawRect(penBlue, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Note the 4th row of intLocations array inside of pssWorkers is ActionBarX, ActionBarY, DealerChipX, DealerChipY

                        // Player's action bar
                        rctToDraw = pssWorkers[i].BitmapRect(i, 3);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, 0, 0, false);

                        // Player's dealer chip
                        rctToDraw = pssWorkers[i].BitmapRect(i, 3);
                        DrawRect(penRed, rctToDraw.Width, rctToDraw.Height, 0, 0, false);

                        // Player avitar present check
                        rctToDraw = pssWorkers[i].AvitarPresentCheckRect(i);
                        DrawRect(penGreen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face down check location
                        rctToDraw = pssWorkers[i].Hc1FaceDownCheckRect(i);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face up check location 1
                        rctToDraw = pssWorkers[i].Hc1FaceUpCheckRect_1(i);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face up check location 2
                        rctToDraw = pssWorkers[i].Hc1FaceUpCheckRect_2(i);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face down check location
                        rctToDraw = pssWorkers[i].Hc2FaceDownCheckRect(i);
                        DrawRect(penGreen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face up check location 1
                        rctToDraw = pssWorkers[i].Hc2FaceUpCheckRect_1(i);
                        DrawRect(penBlue, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face up check location 2
                        rctToDraw = pssWorkers[i].Hc2FaceUpCheckRect_2(i);
                        DrawRect(penBlue, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Open seat check
                        rctToDraw = pssWorkers[i].OpenSeatCheckRect(i);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Banner present check
                        rctToDraw = pssWorkers[i].BannerPresentCheckRect(i);
                        DrawRect(penRed, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Hold card 1 suit check location
                        rctToDraw = pssWorkers[i].Hc1SuitCheckRect(i);
                        DrawRect(penGreen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Hold card 2 suit check location
                        rctToDraw = pssWorkers[i].Hc2SuitCheckRect(i);
                        DrawRect(penGreen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);
                    }

                    // Board cards
                    if (i > 9 && i < 15)
                    {
                        // Board card suit check location
                        rctToDraw = pssWorkers[i].BoardCardSuitCheckRect(i);
                        DrawRect(penGreen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);
                    }
                }
            }
        }

        /// <summary>
        /// Generic event handler for Button_EnabledChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Enabled)
            {
                btn.BackColor = clrRtbBackground;
            }
            else
            {
                btn.BackColor = clrControlDisabled;
            }
        }

        public void chkSaveBitmaps_CheckChanged(object sender, EventArgs e)
        {
            if (chkSaveBitmaps.Checked)
            {
                btnChangeBitmapSaveLocation.Enabled = true;
            }
            else
            {
                btnChangeBitmapSaveLocation.Enabled = false;
            }
        }

        /// <summary>
        /// Generic event handler for CheckBox_EnabledChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chk_EnabledChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            if (chk.Enabled)
            {
                chk.BackColor = clrRtbBackground;
            }
            else
            {
                chk.BackColor = clrControlDisabled;
            }
        }

        public void chkShowClickData_CheckChanged(object sender, EventArgs e)
        {
            if (chkShowClickData.Checked)
            {
                lblMouseClickX.Text = strMouseLabelX;
                lblMouseClickY.Text = strMouseLabelY;
                lblMouseClickClr.Text = strMouseLabelClr;
                lblMouseClickBright.Text = strMouseLabelBright;
            }
            else
            {
                lblMouseClickX.Text = strMouseLabelX + "N/A";
                lblMouseClickY.Text = strMouseLabelY + "N/A";
                lblMouseClickClr.Text = strMouseLabelClr + "N/A";
                lblMouseClickBright.Text = strMouseLabelBright + "N/A";
            }
        }

        public void rdoModeScreenshotMode_CheckChanged(object sender, EventArgs e)
        {
            if (rdoModeScreenshotMode.Checked)
            {
                btnStartPokerCompanion.Enabled = false;
                btnOpenScreenShotFile.Enabled = true;
                grpBitmapLocationTools.Enabled = true;
            }
        }

        public void rdoModeLiveGame_CheckChanged(object sender, EventArgs e)
        {
            if (rdoModeLiveGame.Checked)
            {
                btnStartPokerCompanion.Enabled = true;
                btnOpenScreenShotFile.Enabled = false;
                grpBitmapLocationTools.Enabled = false;
            }
        }

        public async void rdo9PlayerTable_CheckChanged(object sender, EventArgs e)
        {
            if (rdo9PlayerTable.Checked)
            {
                bln9PlayerTable = true;
                CreateTasks(true);

                for (int i = 0; i < intTaskCount; i++)
                {
                    pssWorkers[i].PlayerCount9();
                }

                if (tskWorkers[9].IsCompleted != true)
                {
                    tskWorkers[9].Start();
                    await tskWorkers[9];
                }
            }
        }

        public void rdo10PlayerTable_CheckChanged(object sender, EventArgs e)
        {
            if (rdo10PlayerTable.Checked)
            {
                bln9PlayerTable = false;
                CreateTasks(true);

                for (int i = 0; i < intTaskCount; i++)
                {
                    pssWorkers[i].PlayerCount10();
                }
            }
        }

        public void txtPlayerOfInterest_TextChanged(object sender, EventArgs e)
        {
            long lngPlayerOfInterestDbHandPlayerId;

            lngPlayerOfInterestDbHandPlayerId = -1;
            strPlayerOfInterest = txtPlayerOfInterest.Text.Trim();

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    if (strPlayerOfInterest.ToUpper() == plr[i].strName.ToUpper())
                    {
                        lngPlayerOfInterestDbHandPlayerId = plr[i].lngDbHandPlayerId;
                        break;
                    }
                }
            }

            frmDataDisplay.PlayerOfInterest(strPlayerOfInterest, lngPlayerOfInterestDbHandPlayerId);
        }

        /// <summary>
        /// Generic event handler for TextBox_EnabledChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void txt_EnabledChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (txt.Enabled)
            {
                txt.BackColor = Color.White;
            }
            else
            {
                txt.BackColor = clrControlDisabled;
            }
        }

        public void picScreenShot_MouseClick(object sender, MouseEventArgs e)
        {
            if (chkShowClickData.Checked)
            {
                DisplayClickLocationData(e.Location);
            }
        }

        #endregion

        #region Helper Methods

        // This is the main helper method that processes all the data on the screen
        private async void UpdateAllData()
        {
            bool blnNewBoardCard = false;

            // Check to see if the "Save Bitmaps" checkbox is checked. If so save bitmaps
            if (chkSaveBitmaps.Checked)
            {
                blnSaveBitmaps = true;
            }
            else
            {
                blnSaveBitmaps = false;
            }

            // Copy bitmaps synchronously and run tasks asynchronously for tasks 17 [index 16] (find dealer and action player)  
            // and 16 [index 15] (find pot) first
            // Do these first so we know if there is a new hand before we start processing the players and hold cards so that we
            // can make decisions about what data needs to processed
            for (int i = 16; i > intTaskCount - 3; i--)
            {
                int intRow, intCellBitmap, intCellWorker;
                intRow = intCellBitmap = intCellWorker = 0;

                if (blnRunTimers)
                {
                    if (i < 9)
                    {
                        intRow = i;
                        intCellBitmap = 1;
                        intCellWorker = intCellBitmap + 8;
                    }
                    else
                    {
                        intRow = i - 9;
                        intCellBitmap = 5;
                        intCellWorker = intCellBitmap + 9;
                    }

                    dgvTimers.Rows[intRow].Cells[intCellBitmap + 1].Value = intJobStartedCounter;
                    intJobStartedCounter++;
                    // Log time it takes to copy the bitmap
                    stpBitmapCopies.Start();
                }

                // Debug.WriteLine("C" + (i + 1).ToString() + " started " + DateTime.Now.ToString("mm:ss.fffff"));
                pssWorkers[i].NewScreenShot(new Bitmap((Bitmap)bmpScreenShot.Clone(pssWorkers[i].BitmapRect(i, 0), bmpScreenShot.PixelFormat)));
                // Debug.WriteLine("C" + (i + 1).ToString() + " completed " + DateTime.Now.ToString("mm:ss.fffff"));

                if (blnRunTimers)
                {
                    stpBitmapCopies.Stop();

                    // Log time it takes to copy the bitmap
                    dgvTimers.Rows[intRow].Cells[intCellBitmap].Value = string.Format("{0:0.000}", stpBitmapCopies.ElapsedMilliseconds / 1000.0);
                    dgvTimers.Rows[intRow].Cells[intCellBitmap + 2].Value = intJobCompletionCounter;
                    intJobCompletionCounter++;
                    stpBitmapCopies.Reset();
                }

                stpWorkers[i].Start();
                tskWorkers[i].Start();
                dgvTimers.Rows[intRow].Cells[intCellWorker + 2].Value = intJobStartedCounter;
                intJobStartedCounter++;
            }

            // Copy bitmaps synchronously and run tasks asynchronously for each of the 10 players and 5 possilbe board cards
            for (int i = 0; i < intTaskCount - 2; i++)
            {
                int intRow, intCellBitmap;
                intRow = intCellBitmap = 0;

                if (blnRunTimers)
                {
                    if (i < 9)
                    {
                        intRow = i;
                        intCellBitmap = 1;
                    }
                    else
                    {
                        intRow = i - 9;
                        intCellBitmap = 5;
                    }

                    dgvTimers.Rows[intRow].Cells[intCellBitmap + 1].Value = intJobStartedCounter;
                    intJobStartedCounter++;

                    if (i != 9 || !bln9PlayerTable)
                    {
                        // Log time it takes to copy the bitmap
                        stpBitmapCopies.Start();
                    }
                }

                if (i != 9 || !bln9PlayerTable)
                {
                    pssWorkers[i].NewScreenShot(new Bitmap((Bitmap)bmpScreenShot.Clone(pssWorkers[i].BitmapRect(i, 0), bmpScreenShot.PixelFormat)));
                }

                if (blnRunTimers)
                {                    
                    stpBitmapCopies.Stop();

                    // Log time it takes to copy the bitmap
                    dgvTimers.Rows[intRow].Cells[intCellBitmap].Value = string.Format("{0:0.000}", stpBitmapCopies.ElapsedMilliseconds / 1000.0);
                    dgvTimers.Rows[intRow].Cells[intCellBitmap + 2].Value = intJobCompletionCounter;
                    intJobCompletionCounter++;
                    stpBitmapCopies.Reset();
                }

                if (i != 9 || !bln9PlayerTable)
                {
                    // Start the "StartTask" method here. It is asynchronous and will await the completion of task #16 before it starts
                    // any other tasks.
                    Task tsk = StartTask(i);
                }
                else
                {
                    if (blnRunTimers)
                    {
                        // If this is a 9 player table and this is task 9 don't start the task just update dgvTimers
                        dgvTimers.Rows[0].Cells[16].Value = intJobStartedCounter;
                        intJobStartedCounter++;
                        dgvTimers.Rows[0].Cells[14].Value = string.Format("{0:0.000}", 0);
                        dgvTimers.Rows[0].Cells[15].Value = string.Format("{0:0.000}", 0);
                        dgvTimers.Rows[0].Cells[17].Value = intJobCompletionCounter;
                        intJobCompletionCounter++;

                        // reset and restart this timer every time so that when the last iteration of this method fires it starts the
                        // "UI Idle time" timer =)
                        stpUiIdleTime.Stop();
                        stpUiIdleTime.Reset();
                        stpUiIdleTime.Start();
                    }
                }
            }
            blnFirstScreenShot = false;

            await Task.WhenAll(tskWorkers);
            // Debug.WriteLine("***All tasks completed has been called!!!***");

            // Check for secondary new hand signal which is...
            // When a player didn't have cards before (folded) but has cards now without the dealer changing or
            // if a player showed his cards but is not showing them now a new hand has started.
            // This happens when the player who was the first to act player (player to the left of the dealer) 
            // gets eliminated on the previous hand. This is done so the next first to act player doesn't skip
            // his blinds
            if (blnReRunTasks && !blnDealerChipNotFound && !blnDontUpdateData)
            {
                blnReRunTasks = false;
                rtbNewHandDetected.Visible = true;

                CreateTasks(false);

                // Run tasks asynchronously for each of the 10 players and 5 possilbe board cards
                for (int i = 0; i < intTaskCount - 2; i++)
                {
                    if (i != 9 || !bln9PlayerTable)
                    {
                        if (blnRunTimers)
                        {
                            stpWorkers[i].Start();
                        }

                        tskWorkers[i].Start();

                        if (blnRunTimers)
                        {
                            int intRow, intCellWorker;

                            if (i < 9)
                            {
                                intRow = i;
                                intCellWorker = 9;
                            }
                            else
                            {
                                intRow = i - 9;
                                intCellWorker = 14;
                            }

                            dgvTimers.Rows[intRow].Cells[intCellWorker + 2].Value = intJobStartedCounter;
                            intJobStartedCounter++;

                            // reset and restart this timer every time so that when the last iteration of this method fires it starts the
                            // "UI Idle time" timer =)
                            stpUiIdleTime.Stop();
                            stpUiIdleTime.Reset();
                            stpUiIdleTime.Start();
                        }
                    }
                    else
                    {
                        // if this is a 9 player table and task 9 just update dgvTimers
                        if (blnRunTimers)
                        {
                            dgvTimers.Rows[0].Cells[16].Value = intJobStartedCounter;
                            intJobStartedCounter++;

                            // reset and restart this timer every time so that when the last iteration of this method fires it starts the
                            // "UI Idle time" timer =)
                            stpUiIdleTime.Stop();
                            stpUiIdleTime.Reset();
                            stpUiIdleTime.Start();
                        }
                    }
                }

                await Task.WhenAll(tskWorkers);
            }
            else 
            {
                if (rtbNewHandDetected.Visible == true)
                {
                    rtbNewHandDetected.Visible = false;
                }

                if (blnDealerChipNotFound)
                {
                    blnNewDealer = false;
                    blnReRunTasks = false;

                    // Reset the pot data which may have been updated
                    intPot[0] = intPot[1];
                    blnNewPot = false;
                }

                if (blnDontUpdateData)
                {
                    blnDontUpdateData = false;
                    intPot[0] = intPot[1];
                    blnNewPot = false;
                }
            }

            // Update UI with new data
            stpUiUpdate.Start();
            stpUiIdleTime.Stop();

            rtbUiIdleTime.Clear();
            SetColoredRtbText(rtbUiIdleTime, clrDataId, "UI Idle Time: ");
            rtbUiIdleTime.AppendText(string.Format("{0:0.000}", stpUiIdleTime.ElapsedMilliseconds / 1000.0));

            // Start processing the data produced by the work that the tskWorkers performed

            // Update rtbPotDealerAction here so that new hand is processed before player (and play chip count) changes
            if (blnNewPot || blnNewDealer || blnNewActionPlayer ||
                blnPotChangePrevious || blnDealerChangePrevious || blnActionPlayerChangePrevious)
            {
                UpdateRtbPotDealerAction(blnNewPot, blnNewDealer, blnNewActionPlayer);
            }

            // Included blnPlrChange, blnChipStackChangePrev and blnOpenSeatChangePrev check into PlayerInfoChange()
            if (PlayerInfoChange())
            {
                UpdateDgvPlayers();
            }

            // Update RtbBoard
            if (blnNewDealer || blnNewDealerPrev || blnBoardChangePrevious || BoardChange())
            {
                UpdateRtbBoard();
            }

            // Update frmDisplay window (if necessary)
            // Find out if a new card has come onto the board
            if (brd[0].intCardCurr != brd[0].intCardPrev || brd[3].intCardCurr != brd[3].intCardPrev || brd[4].intCardCurr != brd[4].intCardPrev)
            {
                blnNewBoardCard = true;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    // If the player of interest is the action player and was not previously the action player, or if the player of interest is the action 
                    // player and a new card has come onto the board update the frmDisplayData window
                    if (intActionPlayer[0] == i && (intActionPlayer[1] != i || blnNewActionPlayer || blnNewBoardCard) && plr[i].strName.ToLower() == strPlayerOfInterest.ToLower())
                    {
                        bool[] blnPlrOpenSeat = new bool[10];
                        bool[] blnPlrFolded = new bool[10];
                        int intBoardStatus;
                        int[] intPlrDbId = new int[10];
                        string[] strPlrName = new string[10];

                        // Copy the relevant data from the plr array and put it into arrays to pass to frmDataDisplay
                        for (int j = 0; j < 10; j++)
                        {
                            if (plr[j].intHoldCardsCurr[0] == -1)
                            {
                                blnPlrFolded[j] = true;
                            }
                            else
                            {
                                blnPlrFolded[j] = false;
                            }

                            blnPlrOpenSeat[j] = plr[j].blnOpenSeatCurr;
                            intPlrDbId[j] = plr[j].intDbPlayerId;
                            strPlrName[j] = plr[j].strName;
                        }

                        // Determine the board status (0 = pre-flop, 1 = flop, 2 = turn, 3 = river)
                        if (brd[0].intCardCurr == -1)
                        {
                            intBoardStatus = 0;
                        }
                        else if (brd[3].intCardCurr == -1)
                        {
                            intBoardStatus = 1;
                        }
                        else if (brd[4].intCardCurr == -1)
                        {
                            intBoardStatus = 2;
                        }
                        else
                        {
                            intBoardStatus = 3;
                        }

                        // Update frmDataDisplay
                        frmDataDisplay.UpdateData(blnPlrOpenSeat, blnPlrFolded, intPlrDbId, strPlrName, intBoardStatus, intDbGameId);

                        break;
                    }
                }
            }

            // Re-Create Tasks to avoid getting the "Start may not be called on a task that has completed." exception
            CreateTasks(true);

            if (blnRunTimers)
            {
                stpUiUpdate.Stop();
                stpOverallTime.Stop();
                dgvTimers.Rows[8].Cells[15].Value = string.Format("{0:0.000}", stpUiUpdate.ElapsedMilliseconds / 1000.0);

                rtbTotalProcessTime.Clear();
                SetColoredRtbText(rtbTotalProcessTime, clrDataId, "Total Processing Time: ");
                rtbTotalProcessTime.AppendText(string.Format("{0:0.000}", stpOverallTime.ElapsedMilliseconds / 1000.0));
                intJobCompletionCounter = 1;
                intJobStartedCounter = 1;
                stpOverallTime.Reset();
                stpUiUpdate.Reset();
            }

            if (!blnPokerCompanionRunning)
            {
                if (!chkAutoNextScreenShot.Checked)
                {
                    btnStartPokerCompanion.Enabled = true;
                    btnOpenScreenShotFile.Enabled = true;
                    chkAutoNextScreenShot.Enabled = true;
                    btnNextScreenShot.Enabled = true;
                }
                else
                {
                    btnNextScreenShot_Click(null, null);
                }
            }
            else
            {
                btnStartPokerCompanion_Click(null, null);
            }
        }

        private async Task StartTask(int intTask)
        {
            // wait for taskworkers #16 to complete so we know if we have a new hand so we know what we need to check for
            await tskWorkers[16];

            // Prevent starting this task twice as this method may be called recursively. See longer note approximately 35 lines
            // below this one for more info.
            if (tskWorkers[intTask].Status == TaskStatus.Created)
            {
                if (blnRunTimers)
                {
                    stpWorkers[intTask].Start();
                }

                tskWorkers[intTask].Start();

                if (blnRunTimers)
                {
                    int intRow, intCellWorker;

                    if (intTask < 9)
                    {
                        intRow = intTask;
                        intCellWorker = 9;
                    }
                    else
                    {
                        intRow = intTask - 9;
                        intCellWorker = 14;
                    }

                    dgvTimers.Rows[intRow].Cells[intCellWorker + 2].Value = intJobStartedCounter;
                    intJobStartedCounter++;

                    // reset and restart this timer every time so that when the last iteration of this method fires it starts the
                    // "UI Idle time" timer =)
                    stpUiIdleTime.Stop();
                    stpUiIdleTime.Reset();
                    stpUiIdleTime.Start();
                }

                // Check to see if the previous task has already started. If it hasn't that means the bitmap for the previous
                // task has already been copied but the dealer player hadn't been determined yet so that task was not started 
                // yet. If this is the case start that task because the earlier tasks usually take longer than the later tasks
                // so getting all the earlier tasks started ASAP will increase the overall processing speed.
                //
                // Check for TaskStatus.Created because if the task has only been created and is not "WaitingToRun", "Running" 
                // or "RanToCompletion" it hasn't started yet.
                // Note: StartTask(int) is being called recursively here
                if (intTask > 0 && tskWorkers[intTask - 1].Status == TaskStatus.Created)
                {
                    await StartTask(intTask - 1);
                }
            }
        }

        /// <summary>
        /// Verify that we have correct chip counts for the players. The player chip counts could be off in an all-in situation because in all-in situaitons banners pop up and cover the players' chip counts. In the case where a player with more chips goes all-in and another player or players call his bet chip counts will be off because the player with more chips will have all his chips in the pot and the player(s) that called will not.
        /// </summary>
        /// <param name="intCurrPlr">The index of the current player being processed</param>
        /// <returns>The amount the current player bet</returns>
        private int CorrectAllInChipCounts(int intCurrPlr)
        {
            int intBetAmount, intPlr1PrevChips, intPlr2PrevChips;
            intBetAmount = 0;

            // Verify chip counts
            int intPlr1stMostChipsInPot, intPlr2ndMostChipsInPot, int1stMostChipsInPot, int2ndMostChipsInPot;
            intPlr1stMostChipsInPot = -1;
            intPlr2ndMostChipsInPot = -1;
            int1stMostChipsInPot = -1;
            int2ndMostChipsInPot = -1;

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    // check if player is still in the game (assuming all remaining players have both of their hold cards shown at this point)
                    if (plr[i].intHoldCardsCurr[0] > 0)
                    {
                        if (plr[i].intChipsInPot > int1stMostChipsInPot)
                        {
                            intPlr2ndMostChipsInPot = intPlr1stMostChipsInPot;
                            int2ndMostChipsInPot = int1stMostChipsInPot;

                            intPlr1stMostChipsInPot = i;
                            int1stMostChipsInPot = plr[i].intChipsInPot;
                        }
                        else if (plr[i].intChipsInPot > int2ndMostChipsInPot)
                        {
                            intPlr2ndMostChipsInPot = i;
                            int2ndMostChipsInPot = plr[i].intChipsInPot;
                        }
                    }
                }
            }

            intPlr1PrevChips = plr[intPlr1stMostChipsInPot].intChipStackPrev;
            intPlr2PrevChips = plr[intPlr2ndMostChipsInPot].intChipStackPrev;

            // Correct the chip counts
            if (int1stMostChipsInPot != int2ndMostChipsInPot)
            {
                if (plr[intPlr2ndMostChipsInPot].intChipStackPrev > 0)
                {
                    // The player with the second most chips in the pot is calling and has enough chips to cover the bet
                    if (intPlr2PrevChips + int2ndMostChipsInPot >= int1stMostChipsInPot)
                    {
                        plr[intPlr2ndMostChipsInPot].intChipStackPrev -= (int1stMostChipsInPot - int2ndMostChipsInPot);
                        plr[intPlr2ndMostChipsInPot].intChipsInPot = int1stMostChipsInPot;
                    }
                    // The player with the second most chips in the pot is calling but doesn't have enough chips to cover the bet
                    else
                    {
                        plr[intPlr2ndMostChipsInPot].intChipsInPot += plr[intPlr2ndMostChipsInPot].intChipStackPrev;
                        plr[intPlr2ndMostChipsInPot].intChipStackPrev = 0;

                        plr[intPlr1stMostChipsInPot].intChipStackPrev = plr[intPlr1stMostChipsInPot].intChipsInPot - plr[intPlr2ndMostChipsInPot].intChipsInPot;
                        plr[intPlr1stMostChipsInPot].intChipsInPot = plr[intPlr2ndMostChipsInPot].intChipsInPot;
                    }
                }
            }

            if (intCurrPlr == intPlr1stMostChipsInPot)
            {
                intBetAmount = intPlr1PrevChips - plr[intPlr1stMostChipsInPot].intChipStackPrev;
            }
            else if (intCurrPlr == intPlr2ndMostChipsInPot)
            {
                intBetAmount = intPlr2PrevChips - plr[intPlr2ndMostChipsInPot].intChipStackPrev;
            }

            return intBetAmount;
        }

        private void UpdateRtbPotDealerAction(bool blnPotChange, bool blnDealerChange, bool blnActionPlayerChange)
        {
            string strDealer, strActionPlayer, strCurrPlayer, strSb, strBb;
            rtbPotDealerAction.Clear();

            SetColoredRtbText(rtbPotDealerAction, clrDataId, "Pot:\t");
            if (blnPotChange)
            {
                SetColoredChipCountRtbText(rtbPotDealerAction, clrDataChange, intPot[0].ToString() + "\n", false);
                blnPotChangePrevious = true;
            }
            else
            {
                SetColoredChipCountRtbText(rtbPotDealerAction, clrNormalText, intPot[0].ToString() + "\n", false);
                blnPotChangePrevious = false;
            }

            SetColoredRtbText(rtbPotDealerAction, clrDataId, "Dealer:\t");
            if (intDealer[0] != -1)
            {
                strDealer = GetPlayerName(intDealer[0]);
            }
            else
            {
                strDealer = "None";
            }
            if (blnDealerChange)
            {
                bool blnEqualSplit = true, blnSbExists = false;
                int intWinnersCount = 0, intBbChips = 0, intSbChips = 0, intChipsInLeast = 0, intPlayerChipsInLeast = -1;
                int intAnte = 0, intActivePlayers = 0, intSumOfBets = 0;
                int intMostChipsInPot = 0, intPlayerMostChipsInPot = -1, intSecondMostChipsInPot = 0, intPlayerSecondMostChipsInPot = -1;
                int[] intChipsWon = new int[10];
                bool[] blnWinners = new bool[10];

                blnShowdown = false;
                rtbStatus.Clear();

                // Find the sum of all bets made in the last hand
                for (int i = 0; i < 10; i++)
                {
                    if (i != 9 || !bln9PlayerTable)
                    {
                        intSumOfBets += plr[i].intChipsInPot;
                    }
                }

                // Make sure the pot amount is equal to the sum of the all the players' bets (intSumOfBets). If it isn't
                // someone called a bet that put them all in (someone bet more chips than another player had and that player
                // called).
                if (intSumOfBets != intPot[1])
                {
                    // Find the player who put in the most chips
                    for (int i = 0; i < 10; i++)
                    {
                        if (plr[i].intChipsInPot > intMostChipsInPot)
                        {
                            intSecondMostChipsInPot = intMostChipsInPot;
                            intPlayerSecondMostChipsInPot = intPlayerMostChipsInPot;

                            intMostChipsInPot = plr[i].intChipsInPot;
                            intPlayerMostChipsInPot = i;
                        }
                        else if (plr[i].intChipsInPot > intSecondMostChipsInPot)
                        {
                            intSecondMostChipsInPot = plr[i].intChipsInPot;
                            intPlayerSecondMostChipsInPot = i;
                        }
                    }

                    // If intBbPlayer == -1 this is an incomplete hand -> don't run the code below for verifying chip counts
                    if (intPlayerMostChipsInPot > -1 && intBbPlayer != -1)
                    {
                        // If the above is not true no players have any chips in the pot which would indicate an invalid hand
                        // -> If we have reached this point we have a valid hand
                        plr[intPlayerMostChipsInPot].intChipsInPot -= intSumOfBets - intPot[1];
                        plr[intPlayerMostChipsInPot].intChipStackPrev += intSumOfBets - intPot[1];

                        // This if statement is only to verify that everything was done correctly. If all goes well this if statement will
                        // never be true.
                        if (plr[intPlayerMostChipsInPot].intChipsInPot != plr[intPlayerSecondMostChipsInPot].intChipsInPot)
                        {
                            //MessageBox.Show("Houston we have a problem!!!", "A Chip Accounting Error has Occcured");
                            if (txtErrorMessages.Text.Length != 0)
                            {
                                txtErrorMessages.AppendText("\r\n");
                            }
                            txtErrorMessages.AppendText("Houston we have a problem!!!, A Chip Accounting Error has Occcured - " + DateTime.Now.ToLongTimeString());

                            plr[intPlayerMostChipsInPot].intChipsInPot = plr[intPlayerSecondMostChipsInPot].intChipsInPot;
                        }
                    }
                }

                intBbPlayer = -1;
                intSbPlayer = -1;

                // Find all winners from last hand and total number of winners (not how much they won, who they are)
                for (int i = 0; i < 10; i++)
                {
                    intChipsWon[i] = 0;
                    if (i != 9 || !bln9PlayerTable)
                    {
                        blnWinners[i] = false;

                        if (plr[i].intChipStackPrev != -1)
                        {
                            if (plr[i].intChipStackCurr > plr[i].intChipStackPrev)
                            {
                                blnWinners[i] = true;
                                intWinnersCount++;
                            }
                        }
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if (blnWinners[i])
                    {
                        // One winner
                        if (intWinnersCount == 1)
                        {
                            strCurrPlayer = GetPlayerName(i);

                            intChipsWon[i] = intPot[1];
                            SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                            rtbStatus.AppendText(" won " + (intChipsWon[i]).ToString() + "\n");

                            // DB stuff
                            Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, -1 * intChipsWon[i], intDbHandActionNum);
                            intDbHandActionNum++;
                        }
                        else
                        {
                            // if more than one winner figure out if they all put the same number of chips in the pot
                            if (intChipsInLeast == 0)
                            {
                                intChipsInLeast = plr[i].intChipsInPot;
                                intPlayerChipsInLeast = i;
                            }
                            else
                            {
                                if (intChipsInLeast != plr[i].intChipsInPot)
                                {
                                    blnEqualSplit = false;

                                    if (plr[i].intChipsInPot < intChipsInLeast)
                                    {
                                        intChipsInLeast = plr[i].intChipsInPot;
                                        intPlayerChipsInLeast = i;
                                    }
                                }
                            }
                        }
                    }
                }

                // More than one winner
                if (intWinnersCount > 1)
                {
                    // Winners split the winnings evenly
                    if (blnEqualSplit)
                    {
                        // Handle the winners in order starting with the first winner to the left of the dealer
                        int intWinnerIndex;
                        int intExtraChips;

                        intExtraChips = intPot[1] % intWinnersCount;

                        for (int z = 0; z < 10; z++)
                        {
                            intWinnerIndex = intDealer[1] + 1 + z;

                            if (intWinnerIndex > 9)
                            {
                                intWinnerIndex -= 10;
                            }

                            intChipsWon[intWinnerIndex] = 0;

                            if (blnWinners[intWinnerIndex])
                            {
                                if (intExtraChips != 0)
                                {
                                    // Deal with the special case where the pot doesn't split evenly amongst the winners. There are different ways
                                    // to handle this case but it looks like Bet Online's poker site handles this case by giving the remaining
                                    // chips that don't split evenly amongst the winners (usually one chip) to the the winners to the left of the
                                    // dealer chip one each until there are none left. This is the explination I found that Bet Online's site seems
                                    // to follow:
                                    // In games with blinds (like Texas Holdem and Draw games), extra chips are given to the players in order after 
                                    // the button. That is, eldest hand gets first odd chip, second eldest hand gets the next one, etc. 
                                    intChipsWon[intWinnerIndex]++;
                                    intExtraChips--;
                                }

                                // Split the pot into equal parts for each of the winners
                                strCurrPlayer = GetPlayerName(intWinnerIndex);

                                intChipsWon[intWinnerIndex] += intPot[1] / intWinnersCount;
                                SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                                rtbStatus.AppendText(" won " + (intChipsWon[intWinnerIndex]).ToString() + "\n");

                                // DB stuff
                                Db_.InsertPlayerAction(plr[intWinnerIndex].lngDbHandPlayerId, -1 * intChipsWon[intWinnerIndex], intDbHandActionNum);
                                intDbHandActionNum++;
                            }
                        }
                    }
                    // Ok, we have more than one winner and one or more winners (but not all) went all-in.
                    // I know that players that didn't go all-in must be splitting the pot with at least one other player.
                    // The all-in player could have won his side pot or could have split it.
                    // If the winning player who put in the least chips has a current chip stack of more than half his share of 
                    // the pot (calculated by summing what everyone put in up to how much this player put in) then he won the whole
                    // side pot. If not I will assume he is splitting his share of the side pot with all other players and they are 
                    // all splitting the main pot (note, I am effectively assuming there is only one side pot). I know that there 
                    // could be other more complicated cases but I believe this will address the vast majority of cases. I also note 
                    // that the way to get these numers correct every time is to read the chips passed to each player from the pot 
                    // at a showdown, but that would be a lot of work for a very rare occurrence so I'm not going to deal with it 
                    // right now.
                    else
                    {
                        int intMainPot = 0, intSidePot = 0;

                        for (int i = 0; i < 10; i++)
                        {
                            if (plr[i].intChipsInPot <= intChipsInLeast)
                            {
                                intMainPot += plr[i].intChipsInPot;
                            }
                            else
                            {
                                intMainPot += intChipsInLeast;
                                intSidePot += plr[i].intChipsInPot - intChipsInLeast;
                            }
                        }
                        // If the winner with the least chips in won the entire side pot
                        if (plr[intPlayerChipsInLeast].intChipStackCurr > intMainPot / 2)
                        {
                            strCurrPlayer = GetPlayerName(intPlayerChipsInLeast);

                            intChipsWon[intPlayerChipsInLeast] = intMainPot;
                            SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                            rtbStatus.AppendText(" won " + (intChipsWon[intPlayerChipsInLeast]).ToString() + "\n");

                            // DB stuff
                            Db_.InsertPlayerAction(plr[intPlayerChipsInLeast].lngDbHandPlayerId, -1 * intChipsWon[intPlayerChipsInLeast], intDbHandActionNum);
                            intDbHandActionNum++;

                            for (int i = 0; i < 10; i++)
                            {
                                if (blnWinners[i] && i != intPlayerChipsInLeast)
                                {
                                    strCurrPlayer = GetPlayerName(i);

                                    intChipsWon[i] = intSidePot / (intWinnersCount - 1);
                                    SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                                    rtbStatus.AppendText(" won " + (intChipsWon[i]).ToString() + "\n");

                                    // DB stuff
                                    Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, -1 * intChipsWon[i], intDbHandActionNum);
                                    intDbHandActionNum++;
                                }
                            }
                        }
                        // Assume the winner with the least chips splits the side pot with everyone else and everyone else
                        // splits the rest of the pot.
                        else
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                if (blnWinners[i])
                                {
                                    strCurrPlayer = GetPlayerName(i);

                                    SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");

                                    if (i == intPlayerChipsInLeast)
                                    {
                                        intChipsWon[i] = intMainPot / intWinnersCount;
                                        rtbStatus.AppendText(" won " + (intChipsWon[i]).ToString() + "\n");

                                        // DB stuff
                                        Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, -1 * intChipsWon[i], intDbHandActionNum);
                                        intDbHandActionNum++;
                                    }
                                    else
                                    {
                                        intChipsWon[i] = (intMainPot / intWinnersCount) + (intSidePot / (intWinnersCount - 1));
                                        rtbStatus.AppendText(" won " + (intChipsWon[i]).ToString() + "\n");

                                        // DB stuff
                                        Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, -1 * intChipsWon[i], intDbHandActionNum);
                                        intDbHandActionNum++;
                                    }

                                }
                            }
                        }
                    }
                }

                if (intWinnersCount > 0)
                {
                    rtbStatus.AppendText("\n");
                }

                SetColoredRtbText(rtbPotDealerAction, clrDataChange, strDealer + "\n");
                blnDealerChangePrevious = true;
                SetColoredRtbText(rtbStatus, clrDataChange, "New Dealer: ");
                SetColoredRtbText(rtbStatus, clrDataId, strDealer + "\n\n");

                // Set intHandActionNum = 1 for DB logging purposes because this is the start of a new hand
                intDbHandActionNum = 1;

                // insert new hand into DB and get HandId
                lngDbHandId = Db_.InsertHand(intDbGameId, 0);

                // This loop also does two things in order to reduce the number of loops that need to be run
                for (int i = 0; i < 10; i++)
                {
                    plr[i].intChipsInPot = 0;

                    if (i != 9 || !bln9PlayerTable)
                    {
                        // Document how many chips each player is starting with and what they have in the pot
                        if (plr[i].intChipStackCurr > 0 || plr[i].intChipStackPrev > 0)
                        {
                            strCurrPlayer = GetPlayerName(i);

                            // Check if player exists in the DB (if result is < 1 player does not exist in DB, add the player)
                            plr[i].intDbPlayerId = Db_.QueryPlayerExists(strCurrPlayer);

                            if (plr[i].intDbPlayerId < 1)
                            {
                                plr[i].intDbPlayerId = Db_.InsertPlayer(strCurrPlayer);
                            }

                            SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                            if (!blnWinners[i])
                            {
                                if (plr[i].intChipStackPrev != -1)
                                {
                                    plr[i].intChipsInPot = plr[i].intChipStackPrev - plr[i].intChipStackCurr;
                                    rtbStatus.AppendText(" has " + plr[i].intChipStackPrev.ToString() + "\n");
                                    intActivePlayers++;
                                    plr[i].lngDbHandPlayerId = Db_.InsertHandPlayer(lngDbHandId, plr[i].intDbPlayerId, plr[i].intChipStackPrev, 0, 0, 0);
                                }
                                else
                                {
                                    // First hand
                                    rtbStatus.AppendText(" has " + plr[i].intChipStackCurr.ToString() + "\n");
                                    intActivePlayers++;
                                    plr[i].lngDbHandPlayerId = Db_.InsertHandPlayer(lngDbHandId, plr[i].intDbPlayerId, plr[i].intChipStackCurr, 0, 0, 0);
                                }
                            }
                            else
                            {
                                plr[i].intChipsInPot = plr[i].intChipStackPrev + intChipsWon[i] - plr[i].intChipStackCurr;

                                rtbStatus.AppendText(" has " + (plr[i].intChipStackPrev + (intPot[1] / intWinnersCount)).ToString()
                                    + "\n");
                                intActivePlayers++;
                                plr[i].lngDbHandPlayerId = Db_.InsertHandPlayer(lngDbHandId, plr[i].intDbPlayerId, plr[i].intChipStackPrev + (intPot[1] / intWinnersCount), 0, 0, 0);
                            }

                            // Update player of interest data in frmDataDisplay.cs
                            if (strPlayerOfInterest == plr[i].strName)
                            {
                                frmDataDisplay.PlayerOfInterest(plr[i].strName, plr[i].lngDbHandPlayerId);
                            }

                            // Figure out the big blind and small blind
                            if (plr[i].intChipsInPot > intBbChips)
                            {
                                intSbChips = intBbChips;
                                intSbPlayer = intBbPlayer;

                                intBbChips = plr[i].intChipsInPot;
                                intBbPlayer = i;
                            }
                            else if (plr[i].intChipsInPot > intSbChips)
                            {
                                intSbChips = plr[i].intChipsInPot;
                                intSbPlayer = i;
                            }
                        }
                    }
                }

                //Find ante and check to make sure Sb exists (could be no Sb player under special circumstances)
                if (intActivePlayers > 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i != intBbPlayer && i != intSbPlayer && !blnSbExists)
                        {
                            if (plr[i].intChipsInPot > 0)
                            {
                                intAnte = plr[i].intChipsInPot;
                                if (intSbChips > intAnte)
                                {
                                    blnSbExists = true;
                                }
                            }
                        }
                    }
                }

                if (intAnte > 0)
                {
                    rtbStatus.AppendText("\n");
                }

                // Announce Ante Players
                if (intActivePlayers > 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (plr[i].intChipsInPot > 0)
                        {
                            strCurrPlayer = GetPlayerName(i);

                            SetColoredRtbText(rtbStatus, clrDataChange, strCurrPlayer + ":");
                            rtbStatus.AppendText(" Ante " + (intAnte) + "\n");
                        }
                    }
                }

                // Announce Sb Player
                if (blnSbExists)
                {
                    strSb = GetPlayerName(intSbPlayer);

                    SetColoredRtbText(rtbStatus, clrDataChange, "\n" + strSb + ":");
                    rtbStatus.AppendText(" SB " + (intSbChips - intAnte) + "\n");
                }
                else
                {
                    intSbPlayer = -1;
                    rtbStatus.AppendText("\n");
                }

                // Announce Bb Player
                if (intBbPlayer > -1)
                {
                    strBb = GetPlayerName(intBbPlayer);

                    SetColoredRtbText(rtbStatus, clrDataChange, strBb + ":");
                    rtbStatus.AppendText(" BB " + (intBbChips - intAnte) + "\n");
                }

                rtbStatus.AppendText("\n");

                // DB stuff: 
                // 1. Update "Ante" field in "Hand" table
                // 2. up date "Blind" field in "HandPlayer" table

                Db_.UpdateHand_Ante(lngDbHandId, intAnte);

                if (intSbPlayer >= 0)
                {
                    Db_.UpdateHandPlayer_Blind(plr[intSbPlayer].lngDbHandPlayerId, (intSbChips - intAnte));
                }
                if (intBbPlayer >= 0)
                {
                    Db_.UpdateHandPlayer_Blind(plr[intBbPlayer].lngDbHandPlayerId, (intBbChips - intAnte));
                }
            }
            else
            {
                rtbPotDealerAction.AppendText(strDealer + "\n");
                blnDealerChangePrevious = false;
            }
            
            SetColoredRtbText(rtbPotDealerAction, clrDataId, "Action Player:\t");
            if (intActionPlayer[0] != -1)
            {
                strActionPlayer = GetPlayerName(intActionPlayer[0]);
            }
            else
            {
                strActionPlayer = "None";
            }
            if (blnActionPlayerChange)
            {
                SetColoredRtbText(rtbPotDealerAction, clrDataChange, strActionPlayer);
                blnActionPlayerChangePrevious = true;
            }
            else
            {
                rtbPotDealerAction.AppendText(strActionPlayer);
                blnActionPlayerChangePrevious = false;
            }
        }

        private void UpdateDgvPlayers()
        {
            int intFirstPlayer, i;
            string strPlayer, strChipStack;

            // intFirstPlayer is intended to be the first player to act during this screenshot capture. This variable is important 
            // because if there are two players that perform an aciton in the same screen shot i want the actions to be announced 
            // (and saved to the DB) in the order they actually happened
            if (blnNewDealer || (blnNewDealerPrev && intActionPlayer[0] == -1))
            {
                intFirstPlayer = intBbPlayer + 1;
            }
            else
            {
                intFirstPlayer = intActionPlayer[1];
            }

            if (intFirstPlayer > 9)
            {
                intFirstPlayer -= 10;
            }

            for (int z = 0; z < 10; z++)
            {
                i = z + intFirstPlayer;

                if (i > 9)
                {
                    i -= 10;
                }

                if (i != 9 || !bln9PlayerTable)
                {
                    plr[i].blnChipStackChangePrev = false;

                    // if the player's seat is NOT open
                    if (!plr[i].blnOpenSeatCurr)
                    {
                        // Announce player's name if player's name was previously unknow
                        if (plr[i].blnNewPlr)
                        {
                            // DB stuff
                            // Check if player exists in the DB (if result is < 1 player does not exist in DB, add the player)
                            plr[i].intDbPlayerId = Db_.QueryPlayerExists(plr[i].strName);
                            if (plr[i].intDbPlayerId < 1)
                            {
                                plr[i].intDbPlayerId = Db_.InsertPlayer(plr[i].strName);
                            }

                            Db_.UpdateHandPlayer_Player(plr[i].lngDbHandPlayerId, plr[i].intDbPlayerId);

                            SetColoredRtbText(rtbStatus, clrDataChange, plr[i].strName + ":");
                            rtbStatus.AppendText(" is Player " + (i + 1).ToString() + "\n");
                            rtbStatus.ScrollToCaret();

                            plr[i].blnNewPlr = false;
                        }

                        // Address a special condition where the player folds immediately at the begining of a hand and there is no screenshot
                        // that shows this player having hold cards
                        if (blnNewDealer && plr[i].blnFirstFold)
                        {
                            SetColoredRtbText(rtbStatus, clrDataChange, plr[i].strName + ":");
                            rtbStatus.AppendText(" folded\n");
                            rtbStatus.ScrollToCaret();

                            // DB stuff (use "Chip Count Change" of 0 to mean the player folded)
                            Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, 0, intDbHandActionNum);
                            intDbHandActionNum++;
                        }
                        else if (plr[i].blnHoldCardChange)
                        {
                            string strHc1, strHc2;
                            strHc1 = pssWorkers[i].CardIntToString(plr[i].intHoldCardsCurr[0]);
                            strHc2 = pssWorkers[i].CardIntToString(plr[i].intHoldCardsCurr[1]);

                            plr[i].blnHoldCardChange = false;

                            if (strHc1 == " - - " && !blnShowdown)
                            {
                                // Check to make sure other players are left before calling it a fold
                                // I am doing this because the winner's cards disappear before the hand is over
                                // If two players have cards at the end it was a showdown, there are no folds after a showdown
                                // Note: I haven't explicitly taken mucks into account
                                for (int j = 0; j < 10; j++)
                                {
                                    if (j != i && pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[0]) != " - - " && !plr[j].blnOpenSeatCurr)
                                    {
                                        if (plr[i].strName != "")
                                        {
                                            SetColoredRtbText(rtbStatus, clrDataChange, plr[i].strName + ":");
                                        }
                                        else
                                        {
                                            SetColoredRtbText(rtbStatus, clrDataChange, "Player " + (i + 1).ToString() + ":");
                                        }
                                        rtbStatus.AppendText(" folded\n");
                                        rtbStatus.ScrollToCaret();
                                        // if folded break out of the loop. Only need to show this once

                                        // DB stuff (use "Chip Count Change" of 0 to mean the player folded)
                                        Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, 0, intDbHandActionNum);
                                        intDbHandActionNum++;

                                        break;
                                    }
                                }
                            }
                            else if ((strHc1 != "XX" && strHc1 != " - - ") || (strHc2 != "XX" && strHc2 != " - - "))
                            {
                                if (plr[i].strName.ToLower() != strPlayerOfInterest.ToLower() && plr[i].strNameTemp.ToLower() != strPlayerOfInterest.ToLower())
                                {
                                    strPlayer = GetPlayerName(i);

                                    SetColoredRtbText(rtbStatus, clrDataChange, strPlayer + ":");
                                    rtbStatus.AppendText(" shows " + strHc1 + " " + strHc2 + "\n");
                                    rtbStatus.ScrollToCaret();

                                    // DB stuff
                                    if (plr[i].lngDbHandPlayerId != -1)
                                    {
                                        if (strHc1 != "XX" && strHc2 != "XX")
                                        {
                                            Db_.UpdateHandPlayer_HcBoth(plr[i].lngDbHandPlayerId, (short)plr[i].intHoldCardsCurr[0], (short)plr[i].intHoldCardsCurr[1]);
                                        }
                                        else if (strHc1 != "XX")
                                        {
                                            Db_.UpdateHandPlayer_Hc1(plr[i].lngDbHandPlayerId, (short)plr[i].intHoldCardsCurr[0]);
                                        }
                                        else if (strHc2 != "XX")
                                        {
                                            Db_.UpdateHandPlayer_Hc2(plr[i].lngDbHandPlayerId, (short)plr[i].intHoldCardsCurr[1]);
                                        }
                                    }

                                    blnShowdown = true;

                                    // Run this method now, on the main thread:
                                    // I am running this method now because the "All-In" text over a player's name may have fadded by the  
                                    // next screenshot. If so that data will never have been registered and processed. It doesn't make sense
                                    // to start up a bunch of tasks running on other threads to handle this work. There are most likely 
                                    // only two players whose names need to be checked for the text "All-In".
                                    CheckForAndProcessAllInPlayer(i);
                                }
                                // if the palyer showing his cards is me!
                                else
                                {
                                    SetColoredRtbText(rtbStatus, clrDataChange, "You have: ");
                                    rtbStatus.AppendText(strHc1 + " " + strHc2 + "\n");
                                    rtbStatus.ScrollToCaret();

                                    // DB stuff
                                    if (plr[i].lngDbHandPlayerId != -1)
                                    {
                                        Db_.UpdateHandPlayer_HcBoth(plr[i].lngDbHandPlayerId, (short)plr[i].intHoldCardsCurr[0], (short)plr[i].intHoldCardsCurr[1]);
                                    }

                                    Bitmap bmpTemp;
                                    bmpTemp = pssWorkers[i].ShareScreenShot();
                                    bmpMyHand = new Bitmap((Bitmap)bmpTemp.Clone(new Rectangle(54, 0, 155, bmpTemp.Height), bmpTemp.PixelFormat));
                                    picMyHand.Size = new Size(bmpMyHand.Width, bmpMyHand.Height);
                                    picMyHand.Image = bmpMyHand;
                                    picMyHand.BringToFront();
                                }
                            }
                        }

                        plr[i].blnFirstFold = false;

                        if (plr[i].intChipStackCurr != -2)
                        {
                            strChipStack = plr[i].intChipStackCurr.ToString();
                        }
                        else
                        {
                            if (intPot[0] > intPot[1] && intActionPlayer[1] == i)
                            {
                                // Only run this code for the player that is calling the all-in bet
                                if (plr[i].intChipStackPrev > 0)
                                {
                                    intCorrectedAllInBet = CorrectAllInChipCounts(i);

                                    blnAllInChipStacksCorrected = true;
                                }
                                strChipStack = plr[i].intChipStackPrev.ToString();
                            }
                            else
                            {
                                strChipStack = plr[i].intChipStackPrev.ToString();
                                plr[i].intChipStackCurr = plr[i].intChipStackPrev;
                            }
                        }

                        if (plr[i].intChipStackCurr != plr[i].intChipStackPrev)
                        {
                            // Leave these dgvPlayers.Rows[].Cells[].Value calls because they depend on the chips in the pot
                            // which may not be determined yet in the background thread tasks
                            dgvPlayers.Rows[i].Cells[3].Style = new DataGridViewCellStyle { ForeColor = clrDataChange };
                            // -3 means there was a problem reading the number
                            if (strChipStack != "-3")
                            {
                                dgvPlayers.Rows[i].Cells[3].Value = strChipStack;
                            }
                            else
                            {
                                dgvPlayers.Rows[i].Cells[3].Value = "???";
                            }

                            if (plr[i].intChipStackPrev != -1)
                            {
                                if (!blnNewDealer)
                                {
                                    if (plr[i].intChipStackCurr < plr[i].intChipStackPrev)
                                    {
                                        SetColoredRtbText(rtbStatus, clrDataChange, plr[i].strName + ":");
                                        if (plr[i].intChipStackCurr != -2)
                                        {
                                            plr[i].intChipsInPot += plr[i].intChipStackPrev - plr[i].intChipStackCurr;
                                            rtbStatus.AppendText(" bet " + (plr[i].intChipStackPrev - plr[i].intChipStackCurr).ToString() +
                                                " - " + plr[i].intChipStackCurr.ToString() + "\n");
                                            rtbStatus.ScrollToCaret();

                                            // DB stuff
                                            Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, (plr[i].intChipStackPrev - plr[i].intChipStackCurr), intDbHandActionNum);
                                            intDbHandActionNum++;
                                        }
                                        else
                                        {
                                            if (!blnAllInChipStacksCorrected)
                                            {
                                                plr[i].intChipsInPot = plr[i].intChipsInPot + (intPot[0] - intPot[1]);
                                                rtbStatus.AppendText(" bet " + (intPot[0] - intPot[1]).ToString() +
                                                    " - " + strChipStack + "\n");
                                                plr[i].intChipStackPrev -= (intPot[0] - intPot[1]);
                                                rtbStatus.ScrollToCaret();

                                                // DB stuff
                                                Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, (intPot[0] - intPot[1]), intDbHandActionNum);
                                            }
                                            else
                                            {
                                                rtbStatus.AppendText(" bet " + (intCorrectedAllInBet).ToString() +
                                                    " - " + strChipStack + "\n");
                                                rtbStatus.ScrollToCaret();

                                                // DB stuff
                                                Db_.InsertPlayerAction(plr[i].lngDbHandPlayerId, (intCorrectedAllInBet), intDbHandActionNum);

                                                blnAllInChipStacksCorrected = false;
                                            }
                                            intDbHandActionNum++;
                                        }
                                    }
                                    else
                                    {
                                        plr[i].intChipsInPot += plr[i].intChipStackPrev - plr[i].intChipStackCurr;
                                    }
                                }
                            }

                            plr[i].blnChipStackChangePrev = true;
                        }
                        else
                        {
                            // Leave these dgvPlayers.Rows[].Cells[].Value calls because they depend on the chips in the pot
                            // which may not be determined yet in the background thread tasks
                            dgvPlayers.Rows[i].Cells[3].Style = new DataGridViewCellStyle { ForeColor = clrNormalText };
                            // -3 means there was a problem reading the number
                            if (strChipStack != "-3")
                            {
                                dgvPlayers.Rows[i].Cells[3].Value = strChipStack;
                            }
                            else
                            {
                                dgvPlayers.Rows[i].Cells[3].Value = "???";
                            }
                        }
                    }
                }
            }
        }

        private void UpdateRtbBoard()
        {
            rtbBoard.Clear();

            blnBoardChangePrevious = false;

            SetColoredRtbText(rtbBoard, clrDataId, "F: ");
            if (brd[0].intCardCurr != brd[0].intCardPrev)
            {
                SetColoredRtbText(rtbBoard, clrDataChange, pssWorkers[10].CardIntToString(brd[0].intCardCurr) + "       \t");
                blnBoardChangePrevious = true;

                if (pssWorkers[10].CardIntToString(brd[0].intCardCurr) != " - - ")
                {
                    SetColoredRtbText(rtbStatus, clrDataChange, "Flop : ");
                    rtbStatus.AppendText(pssWorkers[10].CardIntToString(brd[0].intCardCurr) + " - ");

                    // DB stuff
                    brd[0].lngPrevBoardActionId = Db_.InsertBoardAction(lngDbHandId, (short)brd[0].intCardCurr, intDbHandActionNum);
                    intDbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    SetColoredRtbText(rtbStatus, clrDataChange, "Flop card #1 read incorrectly!");
                    Db_.DeleteBoardAction(brd[0].lngPrevBoardActionId);
                    intDbHandActionNum--;
                }
            }
            else
            {
                rtbBoard.AppendText(pssWorkers[10].CardIntToString(brd[0].intCardCurr) + "       \t");
            }

            SetColoredRtbText(rtbBoard, clrDataId, "F: ");
            if (brd[1].intCardCurr != brd[1].intCardPrev)
            {
                SetColoredRtbText(rtbBoard, clrDataChange, pssWorkers[11].CardIntToString(brd[1].intCardCurr) + "       \t");
                blnBoardChangePrevious = true;

                if (pssWorkers[11].CardIntToString(brd[1].intCardCurr) != " - - ")
                {
                    rtbStatus.AppendText(pssWorkers[11].CardIntToString(brd[1].intCardCurr) + " - ");

                    // DB stuff
                    brd[1].lngPrevBoardActionId = Db_.InsertBoardAction(lngDbHandId, (short)brd[1].intCardCurr, intDbHandActionNum);
                    intDbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    SetColoredRtbText(rtbStatus, clrDataChange, "Flop card #2 read incorrectly!");
                    Db_.DeleteBoardAction(brd[1].lngPrevBoardActionId);
                    intDbHandActionNum--;
                }
            }
            else
            {
                rtbBoard.AppendText(pssWorkers[11].CardIntToString(brd[1].intCardCurr) + "       \t");
            }

            SetColoredRtbText(rtbBoard, clrDataId, "F: ");
            if (brd[2].intCardCurr != brd[2].intCardPrev)
            {
                SetColoredRtbText(rtbBoard, clrDataChange, pssWorkers[12].CardIntToString(brd[2].intCardCurr) + "\n\t");
                blnBoardChangePrevious = true;

                if (pssWorkers[12].CardIntToString(brd[2].intCardCurr) != " - - ")
                {
                    rtbStatus.AppendText(pssWorkers[12].CardIntToString(brd[2].intCardCurr) + "\n");
                    rtbStatus.ScrollToCaret();

                    // DB stuff
                    brd[2].lngPrevBoardActionId = Db_.InsertBoardAction(lngDbHandId, (short)brd[2].intCardCurr, intDbHandActionNum);
                    intDbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    SetColoredRtbText(rtbStatus, clrDataChange, "Flop card #3 read incorrectly!");
                    Db_.DeleteBoardAction(brd[2].lngPrevBoardActionId);
                    intDbHandActionNum--;
                }
            }
            else
            {
                rtbBoard.AppendText(pssWorkers[12].CardIntToString(brd[2].intCardCurr) + "\n\t");
            }

            SetColoredRtbText(rtbBoard, clrDataId, "T: ");
            if (brd[3].intCardCurr != brd[3].intCardPrev)
            {
                SetColoredRtbText(rtbBoard, clrDataChange, pssWorkers[13].CardIntToString(brd[3].intCardCurr) + "       \t");
                blnBoardChangePrevious = true;

                if (pssWorkers[13].CardIntToString(brd[3].intCardCurr) != " - - ")
                {
                    SetColoredRtbText(rtbStatus, clrDataChange, "Turn : ");
                    rtbStatus.AppendText(pssWorkers[13].CardIntToString(brd[3].intCardCurr) + "\n");
                    rtbStatus.ScrollToCaret();

                    // DB stuff
                    brd[3].lngPrevBoardActionId = Db_.InsertBoardAction(lngDbHandId, (short)brd[3].intCardCurr, intDbHandActionNum);
                    intDbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    SetColoredRtbText(rtbStatus, clrDataChange, "Turn card read incorrectly!");
                    Db_.DeleteBoardAction(brd[3].lngPrevBoardActionId);
                    intDbHandActionNum--;
                }
            }
            else
            {
                rtbBoard.AppendText(pssWorkers[13].CardIntToString(brd[3].intCardCurr) + "       \t");
            }

            SetColoredRtbText(rtbBoard, clrDataId, "R: ");
            if (brd[4].intCardCurr != brd[4].intCardPrev)
            {
                SetColoredRtbText(rtbBoard, clrDataChange, pssWorkers[14].CardIntToString(brd[4].intCardCurr));
                blnBoardChangePrevious = true;

                if (pssWorkers[14].CardIntToString(brd[4].intCardCurr) != " - - ")
                {
                    SetColoredRtbText(rtbStatus, clrDataChange, "River : ");
                    rtbStatus.AppendText(pssWorkers[14].CardIntToString(brd[4].intCardCurr) + "\n");
                    rtbStatus.ScrollToCaret();

                    // DB stuff
                    brd[4].lngPrevBoardActionId = Db_.InsertBoardAction(lngDbHandId, (short)brd[4].intCardCurr, intDbHandActionNum);
                    intDbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    SetColoredRtbText(rtbStatus, clrDataChange, "River card read incorrectly!");
                    Db_.DeleteBoardAction(brd[4].lngPrevBoardActionId);
                    intDbHandActionNum--;
                }
            }
            else
            {
                rtbBoard.AppendText(pssWorkers[14].CardIntToString(brd[4].intCardCurr));
            }
        }

        private bool UpdatePot()
        {
            bool blnPotChange;
            int intNewPot;

            intPot[1] = intPot[0];

            intNewPot = pssWorkers[15].GetPot();

            if (intNewPot > -1)
            {
                intPot[0] = intNewPot;
            }

            if (intPot[0] == intPot[1])
            {
                blnPotChange = false;
            }
            else
            {
                blnPotChange = true;
            }

            return blnPotChange;
        }

        private bool UpdateDealer()
        {
            bool blnNewDealr = false;
            int intDealerTemp;

            intDealerTemp = pssWorkers[16].FindDealer();

            // Don't do anything if there is no dealer. I added this code because there was an instance where the dealer chip was covered by a player
            // folding animation (the folded cards were covering the dealer chip).
            if (intDealerTemp != -1)
            {
                blnDealerChipNotFound = false;

                if (intDealerTemp != intDealer[0])
                {
                    if (pssWorkers[16].DealerHasHoldCards(intDealerTemp) || blnFirstScreenShot)
                    {
                        // DealerHasHoldCards(int) checks to make sure the new dealer has hold cards. If he doesn't the hand hasn't really started yet (blinds
                        // have not been posted yet). Wait for a screenshot where the new dealer has hold cards and blinds have been posted before declaring
                        // the start of a new hand.
                        intDealer[1] = intDealer[0];
                        intDealer[0] = intDealerTemp;

                        blnNewDealr = true;
                    }
                    else
                    {
                        blnDontUpdateData = true;
                    }
                }
            }
            else if (intDealer[0] != -1)
            {
                blnDealerChipNotFound = true;
            }

            return blnNewDealr;
        }

        private bool UpdateActionPlayer()
        {
            bool blnNewActionPlayr;
            int intTempActionPlayer;

            intTempActionPlayer = pssWorkers[16].FindActionPlayer();

            if (intTempActionPlayer == intActionPlayer[0])
            {
                blnNewActionPlayr = false;
            }
            else
            {
                blnNewActionPlayr = true;
            }

            // never let the previous action player become -1 (-1 means no action player)
            if (intActionPlayer[0] != -1)
            {
                intActionPlayer[1] = intActionPlayer[0];
            }

            // if new hand set current and previous action player to the current action player
            if ((blnNewDealer || (blnNewDealerPrev && intActionPlayer[0] == -1)) && intTempActionPlayer != -1)
            {
                intActionPlayer[1] = intTempActionPlayer;
            }

            intActionPlayer[0] = intTempActionPlayer;

            return blnNewActionPlayr;
        }

        private void GetBoardCard(int intId)
        {
            // Only search for board card if it's a new hand, the board card hasn't already been shown and one subsequent screenshot to verify the 
            // board card seen on the first screenshot was read correctly. I am doing this because I had an instance when a player folded and the 
            // animation showing that player’s hold cards being moved to the middle of the table (an animation which I cannot shut off) was interpreted 
            // as a hold card. What I plan to do is read the board card, announce it and save it to the DB, then double check on the next screen shot 
            // and if it was misread erase the previous record from the DB, decrement the intDbHandActionNum counter and announce the mistake.
            if (blnNewDealer || blnNewDealerPrev || brd[intId - 10].intCardCurr == -1 || brd[intId - 10].intCardPrev == -1)
            {
                if (!blnNewDealer && !blnNewDealerPrev)
                {
                    // Update intBoardCards array (save current value to the old value)
                    brd[intId - 10].intCardPrev = brd[intId - 10].intCardCurr;

                    // Get new intBoardCards value
                    brd[intId - 10].intCardCurr = pssWorkers[intId].GetBoardCard();

                    if (brd[intId - 10].intCardPrev != -1 && brd[intId - 10].intCardCurr != brd[intId - 10].intCardPrev)
                    {
                        // An error was made in reading the board card
                        brd[intId - 10].intCardCurr = -1;
                    }
                }
                else
                {
                    brd[intId - 10].intCardCurr = -1;
                    brd[intId - 10].intCardPrev = -1;
                }
            }
        }

        /// <summary>
        /// This method gets the player's name and chip stack
        /// </summary>
        /// <param name="intId"></param>
        private void GetPlayerInfo(int intId)
        {
            #region Get player chip stack no matter what 
            
            // Update intPlayerChipStacks array (save current value to the old value if criteria is met)
            if (plr[intId].intChipStackCurr != -2)
            {
                plr[intId].intChipStackPrev = plr[intId].intChipStackCurr;
            }

            // Get new intPlayerChipStacks value if new hand or player has not yet folded
            if (blnNewDealer || blnNewDealerPrev || plr[intId].intHoldCardsCurr[0] != -1)
            {
                plr[intId].intChipStackCurr = pssWorkers[intId].GetChipStack(blnSaveBitmaps);
            }

            #endregion

            #region If new hand or previous player name does not match current player name get player name

            if (blnNewDealer || plr[intId].strNameTemp != plr[intId].strName || plr[intId].strName == "")
            {
                string strNewPlayerName;

                strNewPlayerName = pssWorkers[intId].GetPlayerName(blnSaveBitmaps);

                // if strNewPlayerName = to the player's name nothing needs to be done
                if (strNewPlayerName != plr[intId].strName)
                {
                    // Make sure strNewPlayerName isn't a tempory action status before changing anything
                    if (strNewPlayerName.ToUpper() != "ALL-IN" && strNewPlayerName.ToUpper() != "BET" &&
                        strNewPlayerName.ToUpper() != "CALL" && strNewPlayerName.ToUpper() != "CHECK" &&
                        strNewPlayerName.ToUpper() != "RAISE" && strNewPlayerName.ToUpper() != "FOLD" &&
                        strNewPlayerName.ToUpper() != "TIME" && strNewPlayerName != "")
                    {
                        // if the same new player's name is shown for two screen captures in a row or if this is the first time
                        // checking for the players name(\f) change it
                        if (strNewPlayerName == plr[intId].strNameTemp)
                        {
                            // if this is more than a character case change we have a new player, if not don't make a fuss
                            if (strNewPlayerName.ToUpper() != plr[intId].strName.ToUpper())
                            {
                                // This variable is used to signal that to use a different color for the player's name to highlight
                                // that this is a new player
                                plr[intId].blnPlrChange = true;
                                // This variable is used so i can announce when there's a new player
                                plr[intId].blnNewPlr = true;
                            }

                            plr[intId].strName = strNewPlayerName;
                        }
                        // if not just save it to strNameTemp
                        else
                        {
                            plr[intId].strNameTemp = strNewPlayerName;

                            if (plr[intId].strName == "\f")
                            {
                                plr[intId].strName = "";
                            }
                        }
                    }
                    else
                    {
                        plr[intId].strNameTemp = "\f";

                        if (plr[intId].strName == "\f")
                        {
                            plr[intId].strName = "";
                        }

                        if (strNewPlayerName.ToUpper() == "FOLD")
                        {
                            if (blnNewDealer && !plr[intId].blnFirstFold)
                            {
                                plr[intId].intHoldCardsPrev[0] = 0;
                                plr[intId].intHoldCardsPrev[1] = 0;

                                plr[intId].blnFirstFold = true;
                            }
                        }
                    }
                }
            }

            #endregion

            #region If this point has been reached it is not an Open Seat

            // Update blnOpenSeatPrev status (save current value to old value)
            plr[intId].blnOpenSeatPrev = plr[intId].blnOpenSeatCurr;

            plr[intId].blnOpenSeatCurr = false;

            #endregion
        }

        private bool BoardChange()
        {
            bool blnChange;

            if (brd[0].intCardCurr != brd[0].intCardPrev || brd[1].intCardCurr != brd[1].intCardPrev ||
                brd[2].intCardCurr != brd[2].intCardPrev || brd[3].intCardCurr != brd[3].intCardPrev ||
                brd[4].intCardCurr != brd[4].intCardPrev)
            {
                blnChange = true;
            }
            else
            {
                blnChange = false;
            }

            return blnChange;
        }

        private bool PlayerInfoChange()
        {
            bool blnChange;

            blnChange = false;

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    if (plr[i].blnPlrChange)
                    {
                        blnChange = true;
                        plr[i].blnPlrChange = false;
                    }

                    if (!blnChange && plr[i].intChipStackCurr != plr[i].intChipStackPrev && plr[i].intChipStackCurr != -2)
                    {
                        blnChange = true;
                    }

                    if (!blnChange && plr[i].blnOpenSeatCurr != plr[i].blnOpenSeatPrev)
                    {
                        blnChange = true;
                    }

                    if (!blnChange && plr[i].blnChipStackChangePrev || plr[i].blnOpenSeatChangePrev)
                    {
                        blnChange = true;
                    }
                    if (plr[i].intHoldCardsCurr[0] != plr[i].intHoldCardsPrev[0] || plr[i].intHoldCardsCurr[1] != plr[i].intHoldCardsPrev[1])
                    {
                        blnChange = true;
                        plr[i].blnHoldCardChange = true;
                    }
                }
            }

            return blnChange;
        }

        private void DrawRect(Pen penColor, int intX, int intY, int intWidth, int intHeight, bool blnClearRects)
        {
            if (blnClearRects)
            {
                bmpScreenShot = (Bitmap)Bitmap.FromFile(strScreenshotFilePath);
                gfxScreenShot = Graphics.FromImage(bmpScreenShot);
            }

            gfxScreenShot.DrawRectangle(penColor, intX - 1, intY - 1, intWidth + 2, intHeight + 2);
            picScreenShot.Image = bmpScreenShot;
        }

        private string GetPlayerName(int intPlayer)
        {
            string strPlayerName;

            if (plr[intPlayer].strName != "")
            {
                strPlayerName = plr[intPlayer].strName;
            }
            else
            {
                strPlayerName = "Player " + (intPlayer + 1).ToString();
            }
            return strPlayerName;
        }

        public void SetColoredRtbText(RichTextBox rtb, Color clr, string str)
        {
            Font fntCurrent = rtb.SelectionFont;

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont = new Font(fntCurrent.FontFamily, fntCurrent.Size, FontStyle.Bold);
            rtb.SelectionColor = clr;
            rtb.AppendText(str);

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont = fntCurrent;
            rtb.SelectionColor = rtb.ForeColor;
        }

        public void SetColoredAndStrikeThroughRtbText(RichTextBox rtb, Color clr, string str)
        {
            Font fntCurrent = rtb.SelectionFont;

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont = new Font(fntCurrent.FontFamily, fntCurrent.Size, FontStyle.Bold | FontStyle.Strikeout);
            rtb.SelectionColor = clr;
            rtb.AppendText(str);

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionFont = fntCurrent;
            rtb.SelectionColor = rtb.ForeColor;
        }

        public void SetColoredChipCountRtbText(RichTextBox rtb, Color clr, string str, bool blnRightJustify)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = clr;

            if (str == "-1")
            {
                str = "-";
            }

            if (blnRightJustify)
            {
                while (str.Length < 10)
                {
                    str = " " + str;
                }
                rtb.SelectionBackColor = Color.FromArgb(90, 90, 90);
            }

            rtb.AppendText(str);

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = rtb.ForeColor;
        }

        private void DisplayClickLocationData(Point pntClick)
        {
            int intX, intY, intR, intG, intB;
            float fltBrightness;
            // Point pntClick;
            Color clrPixel;

            // Get X and Y coordinates from position
            // pntClick = this.PointToClient(Cursor.Position);
            intX = pntClick.X;
            intY = pntClick.Y;

            lblMouseClickX.Text = strMouseLabelX + intX.ToString();
            lblMouseClickY.Text = strMouseLabelY + intY.ToString();

            // Get color
            if (bmpScreenShot != null)
            {
                clrPixel = bmpScreenShot.GetPixel(intX, intY);
                intR = clrPixel.R;
                intG = clrPixel.G;
                intB = clrPixel.B;
                fltBrightness = clrPixel.GetBrightness();

                lblMouseClickClr.Text = strMouseLabelClr + "(" + intR.ToString() + ", " + intG.ToString() + ", " + intB.ToString() + ")";
                lblMouseClickBright.Text = strMouseLabelBright + fltBrightness.ToString("f4");
            }
            else
            {
                lblMouseClickClr.Text = strMouseLabelClr + "N/A";
                lblMouseClickBright.Text = strMouseLabelBright + "N/A";
            }
        }

        /// <summary>
        /// Checks to see if a player has the words "All-In" displayed where his name would usually be shown. If so put that player all-in (reduce his chipstack to 0) and make sure all other players remaining in the hand have either put in as many chips as this all in player or are all in themselves.
        /// </summary>
        /// <param name="intPlayerId"></param>
        private void CheckForAndProcessAllInPlayer(int intPlayerId)
        {
            string strNewPlayerName;

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !bln9PlayerTable)
                {
                    if (plr[i].intHoldCardsCurr[0] != -1)
                    {
                        // The player is still in the hand

                        strNewPlayerName = pssWorkers[i].GetPlayerName(false);

                        if (strNewPlayerName.ToUpper() == "ALL-IN")
                        {
                            // First check to make sure the chip counts aren't already correct. This could happen if a player has gone
                            // all in and only one other player is left in the hand and that other player has more than enough chips to
                            // call but clicks "all-in" instead of call. This is really a call because the player that has already gone 
                            // all in has no means to call this larger bet.
                            int intSumOfChipsInPot = 0;

                            for (int j = 0; j < 10; j++)
                            {
                                if (j != 9 || !bln9PlayerTable)
                                {
                                    intSumOfChipsInPot += plr[j].intChipsInPot;
                                }
                            }

                            if (intSumOfChipsInPot != intPot[0])
                            {

                                // This is the case where a player has gone all in and the remaining players' hold cards are shown.
                                // Note: The issue here (the reason for this section of code) is when this happen's the remaining players'
                                // chances of winning are shown over their chip count so the players' chip counts cannot be read. Therefore if  
                                // strNewPlayerName == "All-In" update that player's chip count accordingly. 
                                plr[i].intChipStackCurr = 0;

                                if (intPlayerId > i)
                                {
                                    plr[i].blnSkipInfoCheck = true;
                                }

                                // Also check the chip counts of the remaining players and make sure they have either put in as many chips as the 
                                // all in player or they have gone all in themselves. This is for the case where a player bets more chips than 
                                // the other player has and that other player instantly goes all in before the first player's bet could be 
                                // registered. 

                                for (int j = 0; j < 10; j++)
                                {
                                    if (j != 9 || !bln9PlayerTable)
                                    {
                                        if (plr[j].intHoldCardsCurr[0] != -1)
                                        {
                                            // The player is still in the hand
                                            if (plr[j].intChipsInPot < plr[i].intChipsInPot + plr[i].intChipStackPrev)
                                            {
                                                // The player has less chips in the pot than the all-in player
                                                if (plr[j].intChipStackCurr > 0)
                                                {
                                                    // The player has some chips left in his chipstack
                                                    if (plr[j].intChipsInPot + plr[j].intChipStackCurr > plr[i].intChipsInPot + plr[i].intChipStackPrev)
                                                    {
                                                        // The player has more than enough chips to cover the bet

                                                        // My thinking here is the current player's chipstack is equal to what he had previously minus 
                                                        // the quantity everything that the all in player put in minus what the current player had
                                                        // already put in the pot.
                                                        plr[j].intChipStackCurr = plr[j].intChipStackPrev - (plr[i].intChipsInPot + plr[i].intChipStackPrev - plr[i].intChipsInPot);

                                                        if (intPlayerId > j)
                                                        {
                                                            plr[j].blnSkipInfoCheck = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // The player's chip stack is less than or equal to the amount needed to cover the bet
                                                        plr[j].intChipStackCurr = 0;

                                                        if (intPlayerId > j)
                                                        {
                                                            plr[j].blnSkipInfoCheck = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Tasks

        private void CreateTasks(bool blnAllTasks)
        {
            #region Player info (name and chip stack) [Workers 0 - 9]

            for (int i = 0; i < 10; i++)
            {
                int j = i;

                // Do not create task 9 if it's a 9 player table
                if (j != 9 || !bln9PlayerTable || tskWorkers[9] == null)
                {
                    tskWorkers[j] = new Task(() =>
                    {
                        if (j != 9 || !bln9PlayerTable)
                        {
                            long lngThreadCreationTime = 0;

                            if (blnRunTimers)
                            {
                                stpWorkers[j].Stop();
                                lngThreadCreationTime = stpWorkers[j].ElapsedMilliseconds;
                                stpWorkers[j].Reset();
                                stpWorkers[j].Start();
                            }

                            // Only run the below code if we don't have either of the following conditions
                            // 1) A condition where the dealer chip is temporarily covered.
                            // 2) A condition where there is a new dealer but the players don't have cards yet (if the players haven't recieved their
                            //    cards yet then the antes and blinds haven't been posted yet).
                            // The code below is the processing of the data (bitmap processing).
                            if (!blnDealerChipNotFound && !blnDontUpdateData)
                            {
                                // Only find player info if a dealer exists. It seems there is always a dealer during gameplay.
                                // Not finding player info when there is no dealer will avoid errors before game starts and after
                                // game is over while still allowing the whole game to be captured.
                                if (intDealer[0] != -1)
                                {
                                    if (!pssWorkers[j].OpenSeat())
                                    {
                                        int intHcStatus;

                                        if (blnAllTasks)
                                        {
                                            // Update player hold cards
                                            plr[j].intHoldCardsPrev[0] = plr[j].intHoldCardsCurr[0];
                                            plr[j].intHoldCardsPrev[1] = plr[j].intHoldCardsCurr[1];
                                        }

                                        // Always check the hold card status (as long as it's not an open seat on a continuing hand)
                                        // Use this as a secondary check for a new hand --> if a player didn't have cards before (folded)
                                        // but has cards now without the dealer changing.
                                        // This happens when the player who was the first to act player (player to the left of the dealer) 
                                        // gets eliminated on the previous hand. This is done so the next first to act player doesn't skip
                                        // his blinds.
                                        // Additionally, if blnAllTasks is true this means this method is being re-run because a secondary
                                        // new hand was detected. In this case no need to check the player hold cards because they were
                                        // just checked and not updated.

                                        // intHcStatus (Hold Card Status) == 
                                        // -1 no hold card (no cards, folded or not playing)
                                        // 0 both hold cards face down (the hand has not been shown)
                                        // 1 both hold cards shown
                                        // 2 only first hold card shown
                                        // 3 only second hold card shown
                                        intHcStatus = pssWorkers[j].HcStatus();

                                        if (intHcStatus == 0)
                                        {
                                            plr[j].intHoldCardsCurr[0] = 0;
                                            plr[j].intHoldCardsCurr[1] = 0;
                                        }
                                        else if (intHcStatus == 1)
                                        {
                                            // only find the hold card if this is a new hold card
                                            if (plr[j].intHoldCardsCurr[0] < 1 || blnNewDealer)
                                            {
                                                plr[j].intHoldCardsCurr[0] = pssWorkers[j].FindHc(true, blnSaveBitmaps);
                                            }
                                            // only find the hold card if this is a new hold card
                                            if (plr[j].intHoldCardsCurr[1] < 1 || blnNewDealer)
                                            {
                                                plr[j].intHoldCardsCurr[1] = pssWorkers[j].FindHc(false, blnSaveBitmaps);
                                            }
                                        }
                                        else if (intHcStatus == 2)
                                        {
                                            // only find the hold card if this is a new hold card
                                            if (plr[j].intHoldCardsCurr[0] < 1)
                                            {
                                                plr[j].intHoldCardsCurr[0] = pssWorkers[j].FindHc(true, blnSaveBitmaps);
                                            }
                                            plr[j].intHoldCardsCurr[1] = 0;
                                        }
                                        else if (intHcStatus == 3)
                                        {
                                            plr[j].intHoldCardsCurr[0] = 0;
                                            // only find the hold card if this is a new hold card
                                            if (plr[j].intHoldCardsCurr[1] < 1)
                                            {
                                                plr[j].intHoldCardsCurr[1] = pssWorkers[j].FindHc(false, blnSaveBitmaps);
                                            }
                                        }
                                        else
                                        {
                                            plr[j].intHoldCardsCurr[0] = -1;
                                            plr[j].intHoldCardsCurr[1] = -1;
                                        }

                                        // secondary new hand check
                                        if (!blnNewDealer && !blnNewDealerPrev && intHcStatus == 0 && (plr[j].intHoldCardsPrev[0] != 0 || plr[j].intHoldCardsPrev[1] != 0))
                                        {
                                            blnNewDealer = true;
                                            blnReRunTasks = true;
                                        }

                                        if (!blnReRunTasks)
                                        {
                                            // only do this if this task is not going to be re-run because of the secondary check for new hand flag
                                            if (!plr[j].blnSkipInfoCheck)
                                            {
                                                GetPlayerInfo(j);
                                            }
                                            else
                                            {
                                                plr[j].blnSkipInfoCheck = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        plr[j].strName = "Open Seat";
                                        plr[j].intChipStackCurr = 0;
                                        plr[j].blnOpenSeatPrev = plr[j].blnOpenSeatCurr;
                                        plr[j].blnOpenSeatCurr = true;

                                        plr[j].intHoldCardsCurr[0] = -1;
                                        plr[j].intHoldCardsCurr[1] = -1;
                                        plr[j].intHoldCardsPrev[0] = -1;
                                        plr[j].intHoldCardsPrev[1] = -1;
                                    }

                                    #region Update DGVs

                                    plr[j].blnOpenSeatChangePrev = false;

                                    // if the player's seat is open
                                    if (plr[j].blnOpenSeatCurr)
                                    {
                                        if (!plr[j].blnOpenSeatPrev)
                                        {
                                            dgvPlayers.Rows[j].Cells[0].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrDataChange
                                            };
                                            dgvPlayers.Rows[j].Cells[0].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[0]);

                                            dgvPlayers.Rows[j].Cells[1].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrDataChange
                                            };
                                            dgvPlayers.Rows[j].Cells[1].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[1]);

                                            dgvPlayers.Rows[j].Cells[2].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrDataChange
                                            };
                                            dgvPlayers.Rows[j].Cells[2].Value = "Open Seat";

                                            dgvPlayers.Rows[j].Cells[3].Style = new DataGridViewCellStyle { ForeColor = clrDataChange };
                                            dgvPlayers.Rows[j].Cells[3].Value = "- - -";

                                            plr[j].blnOpenSeatChangePrev = true;
                                        }
                                        else
                                        {
                                            dgvPlayers.Rows[j].Cells[2].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrNormalText
                                            };
                                            dgvPlayers.Rows[j].Cells[2].Value = plr[j].strName;

                                            dgvPlayers.Rows[j].Cells[0].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrNormalText
                                            };
                                            dgvPlayers.Rows[j].Cells[0].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[0]);

                                            dgvPlayers.Rows[j].Cells[1].Style = new DataGridViewCellStyle
                                            {
                                                Font = new Font(dgvPlayers.Font.FontFamily,
                                                dgvPlayers.Font.Size, FontStyle.Bold),
                                                ForeColor = clrNormalText
                                            };
                                            dgvPlayers.Rows[j].Cells[1].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[1]);

                                            dgvPlayers.Rows[j].Cells[3].Style = new DataGridViewCellStyle { ForeColor = clrNormalText };
                                            dgvPlayers.Rows[j].Cells[3].Value = "- - -";
                                        }
                                    }
                                    else
                                    {
                                        string strPlayer;

                                        if (plr[j].intHoldCardsCurr[0] != plr[j].intHoldCardsPrev[0] || plr[j].intHoldCardsCurr[1] != plr[j].intHoldCardsPrev[1])
                                        {
                                            string strHc1, strHc2;
                                            strHc1 = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[0]);
                                            strHc2 = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[1]);
                                            dgvPlayers.Rows[j].Cells[0].Style = new DataGridViewCellStyle { ForeColor = clrDataChange };
                                            dgvPlayers.Rows[j].Cells[1].Style = new DataGridViewCellStyle { ForeColor = clrDataChange };
                                            dgvPlayers.Rows[j].Cells[0].Value = strHc1;
                                            dgvPlayers.Rows[j].Cells[1].Value = strHc2;
                                        }
                                        else
                                        {
                                            dgvPlayers.Rows[j].Cells[0].Style = new DataGridViewCellStyle { ForeColor = clrNormalText };
                                            dgvPlayers.Rows[j].Cells[1].Style = new DataGridViewCellStyle { ForeColor = clrNormalText };
                                            dgvPlayers.Rows[j].Cells[0].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[0]);
                                            dgvPlayers.Rows[j].Cells[1].Value = pssWorkers[j].CardIntToString(plr[j].intHoldCardsCurr[1]);
                                        }

                                        plr[j].blnSittingOutCurr = pssWorkers[j].SittingOut();
                                        if (plr[j].blnSittingOutCurr)
                                        {
                                            if (plr[j].blnSittingOutPrev)
                                            {
                                                if (plr[j].strName != "")
                                                {
                                                    strPlayer = "(" + plr[j].strName + ")";
                                                }
                                                else
                                                {
                                                    strPlayer = "(Player " + (j + 1).ToString() + ")";
                                                }
                                            }
                                            else
                                            {
                                                strPlayer = GetPlayerName(j);
                                                plr[j].blnSittingOutPrev = true;
                                            }
                                        }
                                        else
                                        {
                                            strPlayer = GetPlayerName(j);
                                            plr[j].blnSittingOutPrev = false;
                                        }

                                        dgvPlayers.Rows[j].Cells[2].Value = strPlayer;
                                    }

                                    #endregion
                                }
                            }

                            if (blnSaveBitmaps)
                            {
                                pssWorkers[j].SaveBitmap();
                            }

                            if (blnRunTimers)
                            {
                                int intRow, intCellWorker;
                                stpWorkers[j].Stop();

                                if (j < 9)
                                {
                                    intRow = j;
                                    intCellWorker = 9;
                                }
                                else
                                {
                                    intRow = j - 9;
                                    intCellWorker = 14;
                                }

                                dgvTimers.Rows[intRow].Cells[intCellWorker].Value = string.Format("{0:0.000}", lngThreadCreationTime / 1000.0);
                                dgvTimers.Rows[intRow].Cells[intCellWorker + 1].Value = string.Format("{0:0.000}", stpWorkers[j].ElapsedMilliseconds / 1000.0);
                                dgvTimers.Rows[intRow].Cells[intCellWorker + 3].Value = intJobCompletionCounter;
                                intJobCompletionCounter++;

                                stpWorkers[j].Reset();
                            }                          
                        }
                    });
                }
            }
            
            #endregion

            #region Board Cards [Workers 10 - 14]

            for (int i = 10; i < 15; i++)
            {
                int j = i;

                tskWorkers[j] = new Task(() =>
                {
                    long lngThreadCreationTime = 0;

                    if (blnRunTimers)
                    {
                        stpWorkers[j].Stop();
                        lngThreadCreationTime = stpWorkers[j].ElapsedMilliseconds;
                        stpWorkers[j].Reset();
                        stpWorkers[j].Start();
                    }

                    // Only run the below code if we don't have either of the following conditions
                    // 1) A condition where the dealer chip is temporarily covered.
                    // 2) A condition where there is a new dealer but the players don't have cards yet (if the players haven't recieved their
                    //    cards yet then the antes and blinds haven't been posted yet).
                    // The code below is the processing of the data (bitmap processing).
                    if (!blnDealerChipNotFound && !blnDontUpdateData)
                    {
                        // Only find board cards if a dealer exists. It seems there is always a dealer during gameplay.
                        // Not finding board cards when there is no dealer will avoid errors before game starts and after
                        // game is over while still allowing the whole game to be captured.
                        if (intDealer[0] != -1)
                        {
                            GetBoardCard(j);
                        }
                    }

                    if (blnSaveBitmaps)
                    {
                        pssWorkers[j].SaveBitmap();
                    }

                    if (blnRunTimers)
                    {
                        stpWorkers[j].Stop();
                        dgvTimers.Rows[j - 9].Cells[14].Value = string.Format("{0:0.000}", lngThreadCreationTime / 1000.0);
                        dgvTimers.Rows[j - 9].Cells[15].Value = string.Format("{0:0.000}", stpWorkers[j].ElapsedMilliseconds / 1000.0);
                        dgvTimers.Rows[j - 9].Cells[17].Value = intJobCompletionCounter;
                        intJobCompletionCounter++;

                        stpWorkers[j].Reset();
                    }
                    // Debug.WriteLine("W" + (j + 1).ToString() + " completed " + DateTime.Now.ToString("mm:ss.fffff"));
                });
            }

            #endregion

            #region Pot [Worker 15]

            // Create these two tasks under normal conditions. Don't create these two tasks if tasks are being re-run due to
            // secondary method of detecting a new hand
            if (blnAllTasks)
            {
                tskWorkers[15] = new Task(() =>
                {
                    long lngThreadCreationTime = 0;

                    if (blnRunTimers)
                    {
                        stpWorkers[15].Stop();
                        lngThreadCreationTime = stpWorkers[15].ElapsedMilliseconds;
                        stpWorkers[15].Reset();
                        stpWorkers[15].Start();
                    }

                    blnNewPot = UpdatePot();

                    // For troubleshooting only
                    if (blnSaveBitmaps)
                    {
                        pssWorkers[15].SaveBitmap();
                    }

                    if (blnRunTimers)
                    {
                        stpWorkers[15].Stop();
                        dgvTimers.Rows[6].Cells[15].Value = string.Format("{0:0.000}", lngThreadCreationTime / 1000.0);
                        dgvTimers.Rows[6].Cells[15].Value = string.Format("{0:0.000}", stpWorkers[15].ElapsedMilliseconds / 1000.0);
                        dgvTimers.Rows[6].Cells[17].Value = intJobCompletionCounter;
                        intJobCompletionCounter++;
                        stpWorkers[15].Reset();
                    }
                });

                #endregion

                #region Dealer and Action Player [Worker 16]

                tskWorkers[16] = new Task(() =>
                {
                    long lngThreadCreationTime = 0;

                    if (blnRunTimers)
                    {
                        stpWorkers[16].Stop();
                        lngThreadCreationTime = stpWorkers[16].ElapsedMilliseconds;
                        stpWorkers[16].Reset();
                        stpWorkers[16].Start();
                    }

                    blnNewDealerPrev = blnNewDealer;
                    blnNewDealer = UpdateDealer();
                    blnNewActionPlayer = UpdateActionPlayer();

                    // For troubleshooting only
                    if (blnSaveBitmaps)
                    {
                        pssWorkers[16].DrawDealerRects();
                        pssWorkers[16].DrawHoldCardCheckRects(intDealer[0]);
                        pssWorkers[16].DrawActionBarRects();
                        pssWorkers[16].SaveBitmap();
                    }

                    if (blnRunTimers)
                    {
                        stpWorkers[16].Stop();
                        dgvTimers.Rows[7].Cells[14].Value = string.Format("{0:0.000}", lngThreadCreationTime / 1000.0);
                        dgvTimers.Rows[7].Cells[15].Value = string.Format("{0:0.000}", stpWorkers[16].ElapsedMilliseconds / 1000.0);
                        dgvTimers.Rows[7].Cells[17].Value = intJobCompletionCounter;
                        intJobCompletionCounter++;
                        stpWorkers[16].Reset();
                    }
                });
            }
            #endregion
        }

        #endregion
    }
}
