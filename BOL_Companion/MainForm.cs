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
    public partial class MainForm : Form
    {

        #region Variable Definitions

        /// <summary>
        /// A boolean variable indicating if the Bitmap and Graphics objects containing the current screenshot should be disposed before their values are updated
        /// </summary>
        bool disposeScreenShotResources;

        /// <summary>
        /// A boolean variable indicating if the bitmaps used to process the information in the current screenshot should be saved outside this app
        /// </summary>
        bool saveBitmaps;

        /// <summary>
        /// A boolean variable indicating if the process timers should be run
        /// </summary>
        bool isRunTimers;

        /// <summary>
        /// A boolean variable indicating the size of the pot has changed
        /// </summary>
        bool isPotSizeChange;

        /// <summary>
        /// A boolean variable indicating if there is a new dealer
        /// </summary>
        bool isNewDealer;

        /// <summary>
        /// A boolean variable indicating if there was a new dealer on the previously processed screenshot
        /// </summary>
        bool isNewDealerPrev;

        /// <summary>
        /// A boolean variable indicating if is a new action player
        /// </summary>
        bool isNewActionPlayer;

        /// <summary>
        /// A boolean variable indicating if there was a new action player on the previously processed screenshot
        /// </summary>
        bool isNewActionPlayerPrev;

        /// <summary>
        /// A boolean variable indicating if processing the "next" screenshot a vaild option
        /// </summary>
        bool IsNextScreenShotOptionValid;

        /// <summary>
        /// A boolean variable indicating if the pot size changed on the previously processed screenshot
        /// </summary>
        bool isPotChangePrev;

        /// <summary>
        /// A boolean variable indicating if there was a board card change on the previously processed screenshot
        /// </summary>
        bool isBoardCardChangePrev;

        /// <summary>
        /// A boolean variable indicating that the task workers need to be re-run because a new hand without a dealer change has occured
        /// </summary>
        bool reRunTasks;

        /// <summary>
        /// A boolean variable indicating the current game is taking place at a 9-player table - else, it is a 10-player table
        /// </summary>
        bool is9PlayerTable;

        /// <summary>
        /// A boolean variable indicating that there is a showdown meaning the remaining players are showing their cards to determine the winner
        /// </summary>
        bool isShowdown;

        /// <summary>
        /// A boolean variable indicating that this is the first screenshot this app is processing
        /// </summary>
        bool isFirstScreenshot;

        /// <summary>
        /// A boolean variable indicating that data should not be updated based on the current screenshot because the hand has not started yet
        /// </summary>
        bool dontUpdateData;

        /// <summary>
        /// A boolean variable indicating that the dealer chip was not found
        /// </summary>
        bool dealerChipNotFound;

        /// <summary>
        /// A boolean variable indicating that this app is currently in live game mode
        /// </summary>
        bool isLiveGameMode;

        /// <summary>
        /// A boolean variable indicating that current chip stacks have been corrected to account for all-in bets
        /// </summary>
        bool allInChipStacksCorrected;

        /// <summary>
        /// An integer representing the current screenshot file number (applicable when this app is run in screenshots mode)
        /// </summary>
        int currentScreenShotFileNum;

        /// <summary>
        /// An integer that tracks the order in which the jobs required for processing a screenshot were started
        /// </summary>
        int jobStartedCounter;

        /// <summary>
        /// An integer that tracks the order in which the jobs required for processing a screenshot were completed
        /// </summary>
        int jobCompletionCounter;

        /// <summary>
        /// The index of the big blind player
        /// </summary>
        int bbPlayerIndex;

        /// <summary>
        /// The index of the small blind player
        /// </summary>
        int sbPlayerIndex;

        /// <summary>
        /// The integer value of the corrected all-in bet
        /// </summary>
        int correctedAllInBet;

        /// <summary>
        /// The database game id value
        /// </summary>
        int dbGameId;

        /// <summary>
        /// The database hand action number
        /// </summary>
        int dbHandActionNum;

        /// <summary>
        /// The width of the screen in pixels
        /// </summary>
        int screenWidth;

        /// <summary>
        /// The height of the screen in pixels
        /// </summary>
        int screenHeight;

        /// <summary>
        /// The x-coordinate of the pixel location of the screen
        /// </summary>
        int screenLocX;

        /// <summary>
        /// The y-coordinate of the pixel location of the screen
        /// </summary>
        int screenLocY;

        /// <summary>
        /// The current value of the pot
        /// </summary>
        int potCurr;

        /// <summary>
        /// The previous value of the pot based on the previously processed screenshot
        /// </summary>
        int potPrev;

        /// <summary>
        /// The index of the player who currently has the dealer chip
        /// </summary>
        int dealerIndexCurr;

        /// <summary>
        /// The index of the player who had the dealer chip based on the previously processed screenshot
        /// </summary>
        int dealerIndexPrev;

        /// <summary>
        /// The index of the current action player
        /// </summary>
        int actionPlayerIndexCurr;

        /// <summary>
        /// The index of the action player based on the previously processed screenshot
        /// </summary>
        int actionPlayerIndexPrev;

        /// <summary>
        /// The number of tasks this app uses to process screenshot data
        /// </summary>
        const int taskCount = 17;

        /// <summary>
        /// The database hand id value
        /// </summary>
        long dbHandId;

        /// <summary>
        /// The path to the bitmap screenshot file to be processed (applicable when this app is run in screenshots mode)
        /// </summary>
        string screenShotFilePath;

        /// <summary>
        /// The path to the location where the bitmaps used to process the screenshot data should be saved (applicable when saveBitmaps is enabled)
        /// </summary>
        string bitmapSavePath;

        /// <summary>
        /// The name of the player of interest
        /// </summary>
        string playerOfInterestName;

        /// <summary>
        /// The text of the mouseClickLocationX_Label
        /// </summary>
        string mouseClickLocationX_Str;

        /// <summary>
        /// The text of the mouseClickLocationY_Label
        /// </summary>
        string mouseClickLocationY_Str;

        /// <summary>
        /// The text of the mouseClickLocationColorLabel
        /// </summary>
        string mouseClickLocationColorStr;

        /// <summary>
        /// The text of the mouseClickLocationBrightnessLabel
        /// </summary>
        string mouseClickLocationBrightnessStr;

        /// <summary>
        /// The paths to the screenshot files to be processed (applicable when this app is run in screenshots mode)
        /// </summary>
        string[] screenShotFilePaths;

        /// <summary>
        /// The color of normal text
        /// </summary>
        Color normalTextClr;

        /// <summary>
        /// The color used to indicate a change in the data
        /// </summary>
        Color dataChangeClr;

        /// <summary>
        /// The color used for data descriptions
        /// </summary>
        Color dataIdClr;

        /// <summary>
        /// The color used for the background of rich text boxes
        /// </summary>
        Color rtbBackgroundClr;

        /// <summary>
        /// The color used to indicate a control is disabled
        /// </summary>
        Color controlDisabledClr;

        /// <summary>
        /// A white pen object
        /// </summary>
        Pen whitePen;

        /// <summary>
        /// A black pen object
        /// </summary>
        Pen blackPen;

        /// <summary>
        /// A red pen object
        /// </summary>
        Pen redPen;

        /// <summary>
        /// A green pen object
        /// </summary>
        Pen greenPen;

        /// <summary>
        /// A blue pen object
        /// </summary>
        Pen bluePen;

        /// <summary>
        /// A purple pen object;
        /// </summary>
        Pen purplePen;

        /// <summary>
        /// The bitmap screenshot of the poker table
        /// </summary>
        Bitmap screenShotBmp;

        /// <summary>
        /// The bitmap image of the player of interest's hand
        /// </summary>
        Bitmap playerOfInterestHandBmp;

        /// <summary>
        /// The graphics drawing surface of the screenshot of the poker table
        /// </summary>
        Graphics screenShotGfx;

        /// <summary>
        /// The stopwatch used to measure the time it takes to copy the bimaps that the worker tasks need to process the screenshot data
        /// </summary>
        Stopwatch bitmapCopiesStp;

        /// <summary>
        /// The stopwatch used to measure the time it takes to process a screenshot from beginning to end
        /// </summary>
        Stopwatch overallTimeStp;

        /// <summary>
        /// The stopwatch used to measure the UI thread idle time during the processing of a screenshot
        /// </summary>
        Stopwatch uiThreadIdleTimeStp;

        /// <summary>
        /// The stopwatch used to measure the time it takes to update the UI after the data is processed
        /// </summary>
        Stopwatch uiUpdateStp;

        /// <summary>
        /// An array of stopwatches used to measure the time it takes for each worker thread to complete it's task
        /// </summary>
        Stopwatch[] workersStp;

        /// <summary>
        /// An array of tasks to be completed to process the data in a screenshot
        /// </summary>
        Task[] workerTasks;

        /// <summary>
        /// A database controller for interacting with this application's database
        /// </summary>
        DbController db_;

        /// <summary>
        /// An array of PlayerSeat objects representing the player seats at the poker table
        /// </summary>
        PlayerSeat[] seat;

        /// <summary>
        /// An array of BoardCard objects that represent the possible board card positions
        /// </summary>
        BoardCard[] boardCard;

        /// <summary>
        /// A ProcessScreenShots object used to process screenshots and extract and return the relevant data
        /// </summary>
        ProcessScreenShots[] screenShotProcessor;

        #endregion

        #region Control Definitions

        #region ToolTips

        /// <summary>
        /// The tooltip control shared across MainForm
        /// </summary>
        ToolTip formTip;

        #endregion

        #region GroupBoxes

        /// <summary>
        /// The basic settings groupbox
        /// </summary>
        GroupBox basicSettingsGrp;

        /// <summary>
        /// The program control groupbox
        /// </summary>
        GroupBox programControlGrp;

        /// <summary>
        /// The bitmap location tools groupbox
        /// </summary>
        GroupBox bitmapLocationToolsGrp;

        /// <summary>
        /// The error logging groupbox
        /// </summary>
        GroupBox errorLoggingGrp;

        #endregion

        #region Labels

        /// <summary>
        /// The player of interest Label
        /// </summary>
        Label playerOfInterestLbl;

        /// <summary>
        /// The rectangle X value label
        /// </summary>
        Label rectX_Lbl;

        /// <summary>
        /// The rectangle Y value label
        /// </summary>
        Label rectY_Lbl;

        /// <summary>
        /// The rectangle width value label
        /// </summary>
        Label rectWidthLbl;

        /// <summary>
        /// The rectangle height value label
        /// </summary>
        Label rectHeightLbl;

        /// <summary>
        /// The mouse click X location label
        /// </summary>
        Label mouseClickX_Lbl;

        /// <summary>
        /// The mouse click Y location label
        /// </summary>
        Label mouseClickY_Lbl;

        /// <summary>
        /// The mouse click location color label
        /// </summary>
        Label mouseClickColorLbl;

        /// <summary>
        /// The mouse click location brightness value label
        /// </summary>
        Label mouseClickBrightnessLbl;

        #endregion

        #region CheckBoxes

        /// <summary>
        /// The check box indicating that rectangles drawn on MainForm should be cleared on the next MainForm screenshot update
        /// </summary>
        CheckBox clearRectanglesChk;

        /// <summary>
        /// The check box indicating that used on the next MainForm screenshot update should be saved
        /// </summary>
        CheckBox saveBitmapsChk;

        /// <summary>
        /// The check box indicating that the application should automatically proceed to upload and process the next sequential screenshot when the current screenshot has finished processing
        /// </summary>
        CheckBox autoNextScreenShotChk;

        /// <summary>
        /// The check box indicating that the aplication should display the data about the pixel that was clicked
        /// </summary>
        CheckBox showClickDataChk;

        #endregion

        #region RadioButtons

        /// <summary>
        /// The radio button to select to run the application in live game mode
        /// </summary>
        RadioButton liveGameModeRdo;

        /// <summary>
        /// The radio button to select to run the application in screenshots mode
        /// </summary>
        RadioButton screenshotsModeRdo;

        /// <summary>
        /// The radio button to select to run the application based on a 9 player table
        /// </summary>
        RadioButton _9PlayerTableRdo;

        /// <summary>
        /// The radio button to select to run the application based on a 10 player table
        /// </summary>
        RadioButton _10PlayerTableRdo;

        #endregion

        #region TextBoxes

        /// <summary>
        /// The text box to enter the player of interest's name
        /// </summary>
        TextBox playerOfInterestTxt;

        /// <summary>
        /// The text box to enter the X value of the rectangle to be drawn
        /// </summary>
        TextBox rectX_Txt;

        /// <summary>
        /// The text box to enter the Y value of the rectangle to be drawn
        /// </summary>
        TextBox rectY_Txt;

        /// <summary>
        /// The text box to enter the width value of the rectangle to be drawn
        /// </summary>
        TextBox rectWidthTxt;

        /// <summary>
        /// The text box to enter the height value of the rectangle to be drawn
        /// </summary>
        TextBox rectHeightTxt;

        /// <summary>
        /// The text box where error messages are displayed
        /// </summary>
        TextBox errorMessagesTxt;

        #endregion

        #region Buttons

        /// <summary>
        /// The button to click to change the save location of the bitmap files that the application uses
        /// </summary>
        Button changeBitmapSaveLocation;

        /// <summary>
        /// The button to click to start the application
        /// </summary>
        Button startPokerCompanion;

        /// <summary>
        /// The button to click to open a screen shot file
        /// </summary>
        Button openScreenShotFile;

        /// <summary>
        /// The button to click to process the next screen shot
        /// </summary>
        Button nextScreenShot;

        /// <summary>
        /// The button to click to make a copy of the bitmap images on screen for the application's bitmap workers
        /// </summary>
        Button copyBitmapsForWorkers;

        /// <summary>
        /// The button to click to draw a rectangle on screen based on the values defined by the user
        /// </summary>
        Button drawRect;

        /// <summary>
        /// The button to click to draw all the rectangles used to define the bitmap image locations on screen that the application uses for data processing
        /// </summary>
        Button drawAllRects;

        /// <summary>
        /// The button to click to clear all the rectangles drawn on screen the next time a new screen shot is processed
        /// </summary>
        Button clearAllDbData;

        #endregion

        #region RichTextBoxes

        /// <summary>
        /// The rich text box listing the chips in the pot, the dealer and the action player
        /// </summary>
        RichTextBox potDealerAction;

        /// <summary>
        /// The rich text box listing the actions taking place in the game
        /// </summary>
        RichTextBox gameActions;

        /// <summary>
        /// The rich text box displaying the status of the board cards
        /// </summary>
        RichTextBox boardStatus;

        /// <summary>
        /// The rich text box displaying the length of time the UI thread was idle during data processing
        /// </summary>
        RichTextBox UiIdleTimeRtb;

        /// <summary>
        /// The rich text box displaying the total length of time it took to process the data in a screenshot
        /// </summary>
        RichTextBox totalProcessingTimeRtb;

        /// <summary>
        /// The rich text box that tells the user the special case where a new hand is detected without the dealer changing has occured
        /// </summary>
        RichTextBox newHandDetectedRtb;

        #endregion

        #region DataGridViews

        /// <summary>
        /// The data grid view displaying all the players and their information for the current hand
        /// </summary>
        DataGridView playersDgv;

        /// <summary>
        /// The data grid view displaying the data for the processing timers
        /// </summary>
        DataGridView timersDgv;

        #endregion

        #region PictureBoxes

        /// <summary>
        /// The picture box containing the screenshot of the poker table
        /// </summary>
        PictureBox screenShotPic;

        /// <summary>
        /// The picture box containing the screenshot of the player of interest's hand
        /// </summary>
        PictureBox myHandPic;

        #endregion

        DataDisplayForm dataDisplayForm;

        #endregion

        #region Form Initialization

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The load event for the MainForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeVariables();
            InitializeThisForm();
            InitializePlayerSeats();
            InitializeBoard();
            InitializeScreenShotProcessors();
            InitializeStopWatches();
            InitializeTasks();
            CreateDataDisplayForm();
            DetectScreenCount();
        }

        /// <summary>
        /// Initialize this form, it's controls, their locations as well as some default text and some additional objects.
        /// </summary>
        private void InitializeThisForm()
        {
            SetupMainForm setupMainForm;

            setupMainForm = new SetupMainForm(this);
            setupMainForm.InitializeColors(ref normalTextClr, ref dataChangeClr, ref dataIdClr, ref rtbBackgroundClr, ref controlDisabledClr);
            setupMainForm.InitializeForm();
            setupMainForm.PassFormVariables(mouseClickLocationX_Str, mouseClickLocationY_Str, mouseClickLocationColorStr,
                mouseClickLocationBrightnessStr);
            setupMainForm.InitializeToolTips(ref formTip);
            setupMainForm.InitializeGroupBoxes(ref basicSettingsGrp, ref programControlGrp, ref bitmapLocationToolsGrp, ref errorLoggingGrp);
            setupMainForm.InitializeLabels(ref playerOfInterestLbl, ref rectX_Lbl, ref rectY_Lbl, ref rectWidthLbl, ref rectHeightLbl,
                ref mouseClickX_Lbl, ref mouseClickY_Lbl, ref mouseClickColorLbl, ref mouseClickBrightnessLbl);
            setupMainForm.InitializeCheckBoxes(ref clearRectanglesChk, ref saveBitmapsChk, ref autoNextScreenShotChk,
                ref showClickDataChk);
            setupMainForm.InitializeRadioButtons(ref liveGameModeRdo, ref screenshotsModeRdo, ref _9PlayerTableRdo, ref _10PlayerTableRdo);
            setupMainForm.InitializeTextBoxes(ref playerOfInterestTxt, ref rectX_Txt, ref rectY_Txt, ref rectWidthTxt,
                ref rectHeightTxt, ref errorMessagesTxt);
            setupMainForm.InitializeButtons(ref changeBitmapSaveLocation, ref startPokerCompanion, ref openScreenShotFile,
                ref nextScreenShot, ref copyBitmapsForWorkers, ref drawRect, ref drawAllRects, ref clearAllDbData);
            setupMainForm.InitializeRichTextBoxes(ref potDealerAction, ref gameActions, ref boardStatus, ref UiIdleTimeRtb, ref totalProcessingTimeRtb,
                ref newHandDetectedRtb);
            setupMainForm.InitializeDataGridViews(ref playersDgv, ref timersDgv);
            setupMainForm.InitializePictureBoxes(ref screenShotPic, ref myHandPic);
            setupMainForm.InitializeFonts();

            setupMainForm.SetControlLocations();
            setupMainForm.SetRtbDefaultText();
            setupMainForm.SetDgvDefaultText();
        }

        /// <summary>
        /// Initialize this form's variables and set default values.
        /// </summary>
        private void InitializeVariables()
        {
            disposeScreenShotResources = false;
            saveBitmaps = false;
            isRunTimers = true;
            isPotSizeChange = false;
            isNewDealer = false;
            isNewDealerPrev = false;
            isNewActionPlayer = false;
            isNewActionPlayerPrev = false;
            isPotChangePrev = false;
            isBoardCardChangePrev = false;
            IsNextScreenShotOptionValid = false;
            reRunTasks = false;
            is9PlayerTable = true;
            isShowdown = false;
            isFirstScreenshot = true;
            dontUpdateData = false;
            dealerChipNotFound = false;
            allInChipStacksCorrected = false;
            jobStartedCounter = 1;
            jobCompletionCounter = 1;
            bbPlayerIndex = -1;
            sbPlayerIndex = -1;
            dbGameId = -1;
            dbHandId = -1;
            dbHandActionNum = -1;
            correctedAllInBet = 0;
            potCurr = -1;
            potPrev = -1;
            dealerIndexCurr = -1;
            dealerIndexPrev = -1;
            actionPlayerIndexCurr = -1;
            actionPlayerIndexPrev = -1;
            screenWidth = Screen.AllScreens[0].WorkingArea.Width;
            screenHeight = Screen.AllScreens[0].WorkingArea.Height;
            screenLocX = Screen.AllScreens[0].WorkingArea.Location.X;
            screenLocY = Screen.AllScreens[0].WorkingArea.Location.Y + SystemInformation.CaptionHeight;
            screenShotFilePath = "";
            bitmapSavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            playerOfInterestName = "JabaAdam";
            mouseClickLocationX_Str = "Mouse Click X Coordinate: ";
            mouseClickLocationY_Str = "Mouse Click Y Coordinate: ";
            mouseClickLocationColorStr = "Mouse Click Color: ";
            mouseClickLocationBrightnessStr = "Mouse Click Brightness: ";

            whitePen = new Pen(Color.White, 1);
            blackPen = new Pen(Color.Black, 1);
            redPen = new Pen(Color.Red, 1);
            greenPen = new Pen(Color.Green, 1);
            bluePen = new Pen(Color.Blue, 1);
            purplePen = new Pen(Color.Purple, 1);

            db_ = new DbController();
        }

        /// <summary>
        /// Detect the number of displays on the machine that is running this app.
        /// </summary>
        private void DetectScreenCount()
        {
            if (Screen.AllScreens.Count() < 2)
            {
                MessageBox.Show("Warning! Only one monitor has been detected. This application was designed to be run with at least two monitors. This application will contiue to run but be aware that some information may not be displayed properly or hidden on windows that are not visible.", "Single monitor detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Initialize and create the worker tasks.
        /// </summary>
        async private void InitializeTasks()
        {
            workerTasks = new Task[taskCount];

            // Create the worker tasks so they are ready to run when called
            CreateTasks(true);

            if (is9PlayerTable)
            {
                // If this is a 9-player table run the task for the 10th worker (index of 9)
                // Note: The 10th task was created above so it would not be null but it is run here because it's not needed
                workerTasks[9].Start();
                await workerTasks[9];
            }
        }

        #region Initialize Objects

        /// <summary>
        /// Initialize player seat objects.
        /// </summary>
        private void InitializePlayerSeats()
        {
            seat = new PlayerSeat[10];

            for (int i = 0; i < 10; i++)
            {
                seat[i] = new PlayerSeat();
            }
        }

        /// <summary>
        /// Initialize board card objects.
        /// </summary>
        private void InitializeBoard()
        {
            boardCard = new BoardCard[5];

            for (int i = 0; i < 5; i++)
            {
                boardCard[i] = new BoardCard();
            }
        }

        /// <summary>
        /// Initialize screenshot processor objects.
        /// </summary>
        private void InitializeScreenShotProcessors()
        {
            DataLocations.InitializeDataLocations();

            screenShotProcessor = new ProcessScreenShots[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                screenShotProcessor[i] = new ProcessScreenShots(i, bitmapSavePath);
            }
        }

        /// <summary>
        /// Initialize stopwatch objects.
        /// </summary>
        private void InitializeStopWatches()
        {
            bitmapCopiesStp = new Stopwatch();
            overallTimeStp = new Stopwatch();
            uiThreadIdleTimeStp = new Stopwatch();
            uiUpdateStp = new Stopwatch();

            workersStp = new Stopwatch[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                workersStp[i] = new Stopwatch();
            }
        }

        #endregion

        /// <summary>
        /// Insert a new game entry into the database.
        /// </summary>
        private void CreateNewGameInDb()
        {
            // Insert new game entry into DB and get GameId
            dbGameId = db_.InsertGame(true, 9, errorMessagesTxt);
        }

        /// <summary>
        /// Create an instance of DataDisplayForm.
        /// </summary>
        private void CreateDataDisplayForm()
        {
            // Pass rtbBackgroundClr to DataDisplayForm form so it can be used as the background color of DataDisplayForm
            dataDisplayForm = new DataDisplayForm(rtbBackgroundClr);
            dataDisplayForm.PlayerOfInterest(playerOfInterestName, -1);
            dataDisplayForm.Show();
            dataDisplayForm.ZeroData();
        }

        #endregion

        #region Control Events

        /// <summary>
        /// The open screenshot file click event. This will open the selected screenshot file, copy it to the window background and process the data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void openScreenShotFile_Click(object sender, EventArgs e)
        {
            // Temporarily disable controls that could conflict with this process
            openScreenShotFile.Enabled = false;
            autoNextScreenShotChk.Enabled = false;
            nextScreenShot.Enabled = false;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "E:\\Documents\\Visual Studio 2015\\Projects\\Online Poker\\BetOnline\\Screen Shot Analysis";
            openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Title = "Select Screen Shot to Open";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (isRunTimers)
                {
                    overallTimeStp.Start();
                }

                // Disable the clear all database data button to prevent clearing the database in the middle of a hand as this would cause 
                // problems with the data.
                clearAllDbData.Enabled = false;

                try
                {
                    if ((screenShotFilePath = openFileDialog.FileName) != null)
                    {
                        // Get all the .bmp files in the selected directory
                        string directoryPath = Path.GetDirectoryName(screenShotFilePath);
                        screenShotFilePaths = Directory.GetFiles(directoryPath, "*.bmp", SearchOption.TopDirectoryOnly);

                        // Find the index of the selected .bmp file
                        currentScreenShotFileNum = Array.IndexOf(screenShotFilePaths, screenShotFilePath);

                        if (showClickDataChk.Checked)
                        {
                            mouseClickX_Lbl.Text = mouseClickLocationX_Str;
                            mouseClickY_Lbl.Text = mouseClickLocationY_Str;
                            mouseClickColorLbl.Text = mouseClickLocationColorStr;
                            mouseClickBrightnessLbl.Text = mouseClickLocationBrightnessStr;
                        }

                        if (disposeScreenShotResources)
                        {
                            screenShotBmp.Dispose();
                            screenShotGfx.Dispose();
                        }

                        screenShotBmp = (Bitmap)Image.FromFile(screenShotFilePath);
                        screenShotGfx = Graphics.FromImage(screenShotBmp);

                        screenShotPic.Image = screenShotBmp;
                        disposeScreenShotResources = true;

                        // Must create a new game in the Db here for the subsequent processes to refer to
                        CreateNewGameInDb();
                        ProcessAndUpdateAllData();
                    }
                    IsNextScreenShotOptionValid = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else if (IsNextScreenShotOptionValid)
            {
                // Re-enable temporarily disabled controls
                openScreenShotFile.Enabled = true;
                autoNextScreenShotChk.Enabled = true;
                nextScreenShot.Enabled = true;
                openScreenShotFile.Focus();
            }
            else
            {
                // Re-enable open screenshot file button
                openScreenShotFile.Enabled = true;
                openScreenShotFile.Focus();
            }
        }

        /// <summary>
        /// The start poker companion click event. This method starts taking screenshots of the poker game, processes the data and produces/updates the graphs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void startPokerCompanion_Click(object sender, EventArgs e)
        {
            // Disable the clear all database data button to prevent clearing the database in the middle of a hand as this would cause 
            // problems with the data.
            clearAllDbData.Enabled = false;

            // sender == null when this method is called from the UpdateAllData method, continually repeating the screenshot capture and data processing
            // procedure. 
            // sender != null if the button is actually clicked by the user.
            if (sender != null && isLiveGameMode)
            {
                // Stop live game mode
                isLiveGameMode = false;
                openScreenShotFile.Enabled = true;
                autoNextScreenShotChk.Enabled = true;
                nextScreenShot.Enabled = true;

                startPokerCompanion.Text = "Start Poker Companion";
            }
            else
            {
                if (sender != null && !isLiveGameMode)
                {
                    // Start live game mode
                    isLiveGameMode = true;
                    openScreenShotFile.Enabled = false;
                    autoNextScreenShotChk.Enabled = false;
                    nextScreenShot.Enabled = false;

                    startPokerCompanion.Text = "Stop Poker Companion";

                    // Must create a new game in the Db here for the subsequent processes to refer to
                    CreateNewGameInDb();
                }

                if (isLiveGameMode)
                {
                    // Live game mode is already running, continue
                    if (disposeScreenShotResources)
                    {
                        screenShotBmp.Dispose();
                        screenShotGfx.Dispose();
                    }

                    screenShotBmp = new Bitmap(screenWidth, screenHeight - SystemInformation.CaptionHeight);

                    screenShotGfx = Graphics.FromImage(screenShotBmp as Image);
                    screenShotGfx.CopyFromScreen(screenLocX, screenLocY, 0, 0, screenShotBmp.Size);
                    screenShotPic.Image = screenShotBmp;

                    disposeScreenShotResources = true;

                    if (isRunTimers)
                    {
                        overallTimeStp.Start();
                    }

                    ProcessAndUpdateAllData();
                }
            }
        }

        /// <summary>
        /// The next screenshot click event. This method opens the next screenshot in the screenshot directory, processes the data and produces/updates the graphs as necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void nextScreenShot_Click(object sender, EventArgs e)
        {
            // Temporarily disable controls that could conflict with this process
            openScreenShotFile.Enabled = false;
            nextScreenShot.Enabled = false;

            if (isRunTimers)
            {
                overallTimeStp.Start();
            }

            try
            {
                if (screenShotFilePaths.Length > 0)
                {
                    // Screenshot file paths exist
                    if (currentScreenShotFileNum < screenShotFilePaths.Length - 1)
                    {
                        // We have not reached the end of the screenShotFilePaths array
                        currentScreenShotFileNum++;

                        screenShotBmp.Dispose();
                        screenShotGfx.Dispose();

                        screenShotBmp = (Bitmap)Image.FromFile(screenShotFilePaths[currentScreenShotFileNum]);
                        screenShotGfx = Graphics.FromImage(screenShotBmp);

                        screenShotPic.Image = screenShotBmp;

                        ProcessAndUpdateAllData();
                    }
                    else
                    {
                        // We have reached the end of the screenShotFilePaths array
                        MessageBox.Show("No screenshot files left to process. You have reached the last screenshot file in the selected directory",
                            "No Next File");
                    }
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

        /// <summary>
        /// The copy bitmaps for workers click event. This method copies and passes all the bitmaps the screenShotProcessor objects need to process the currently loaded screenshot. The bitmap files are also saved outside of this app. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void copyBitmapsForWorkers_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < taskCount; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    screenShotProcessor[i].NewScreenShot(new Bitmap(screenShotBmp.Clone(screenShotProcessor[i].BitmapRect(i, 0),
                        screenShotBmp.PixelFormat)));
                    screenShotProcessor[i].SaveBitmap();
                }
            }
        }

        /// <summary>
        /// The change bitmap save location click event. Change the location where the bitmap files that the screenShotProcessor objects use interpret the state of the game are saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void changeBitmapSaveLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            folderBrowser.SelectedPath = bitmapSavePath;
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Select Location to Save Bitmap Files";

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                bitmapSavePath = folderBrowser.SelectedPath;

                for (int i = 0; i < taskCount; i++)
                {
                    screenShotProcessor[i].ChangeBitmapSaveLocation(bitmapSavePath);
                }
            }
        }

        /// <summary>
        /// The clear all database data click event. Clear all the data in all the tables of the database and restart the Id numbering at 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void clearAllDbData_Click(object sender, EventArgs e)
        {
            db_.ClearAllDbData();
        }

        /// <summary>
        /// The draw rectangle click event. Draw a rectangle inside the MainForm window according to the parameters entered in the textboxes inside the Bitmap Location Tools groupbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void drawRect_Click(object sender, EventArgs e)
        {
            int rectX, rectY, rectWidth, rectHeight;

            // High level data validation
            if (int.TryParse(rectX_Txt.Text, out rectX) && int.TryParse(rectY_Txt.Text, out rectY) &&
                int.TryParse(rectWidthTxt.Text, out rectWidth) && int.TryParse(rectHeightTxt.Text, out rectHeight) && screenShotFilePath != "")
            {
                DrawRect(whitePen, rectX, rectY, rectWidth, rectHeight, clearRectanglesChk.Checked);
            }
        }

        /// <summary>
        /// The draw all rectangles click event. Draw rectangles around all the areas used for data processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void drawAllRects_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < taskCount; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    Rectangle rctToDraw = screenShotProcessor[i].BitmapRect(i, 0);
                    DrawRect(whitePen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                    // Specaial data locations for players (poker table seats)
                    if (i < 10)
                    {
                        // Player's first hold card
                        rctToDraw = screenShotProcessor[i].BitmapRect(i, 1);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player's second hold card
                        rctToDraw = screenShotProcessor[i].BitmapRect(i, 2);
                        DrawRect(bluePen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Note the 4th row of DataLocations array inside of DataLocations.cs is ActionBarX, ActionBarY, DealerChipX, DealerChipY

                        // Player's action bar
                        rctToDraw = screenShotProcessor[i].BitmapRect(i, 3);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, 0, 0, false);

                        // Player's dealer chip
                        rctToDraw = screenShotProcessor[i].BitmapRect(i, 3);
                        DrawRect(redPen, rctToDraw.Width, rctToDraw.Height, 0, 0, false);

                        // Player avatar present check
                        rctToDraw = screenShotProcessor[i].AvatarPresentCheckRect(i);
                        DrawRect(greenPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face down check location
                        rctToDraw = screenShotProcessor[i].Hc1FaceDownCheckRect(i);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face up check location 1
                        rctToDraw = screenShotProcessor[i].Hc1FaceUpCheckRect_1(i);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player first hold card face up check location 2
                        rctToDraw = screenShotProcessor[i].Hc1FaceUpCheckRect_2(i);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face down check location
                        rctToDraw = screenShotProcessor[i].Hc2FaceDownCheckRect(i);
                        DrawRect(greenPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face up check location 1
                        rctToDraw = screenShotProcessor[i].Hc2FaceUpCheckRect_1(i);
                        DrawRect(bluePen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Player second hold card face up check location 2
                        rctToDraw = screenShotProcessor[i].Hc2FaceUpCheckRect_2(i);
                        DrawRect(bluePen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Open seat check
                        rctToDraw = screenShotProcessor[i].OpenSeatCheckRect(i);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Banner present check
                        rctToDraw = screenShotProcessor[i].BannerPresentCheckRect(i);
                        DrawRect(redPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Hold card 1 suit check location
                        rctToDraw = screenShotProcessor[i].Hc1SuitCheckRect(i);
                        DrawRect(greenPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);

                        // Hold card 2 suit check location
                        rctToDraw = screenShotProcessor[i].Hc2SuitCheckRect(i);
                        DrawRect(greenPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);
                    }

                    // Specaial data locations for board cards
                    if (i > 9 && i < 15)
                    {
                        // Board card suit check location
                        rctToDraw = screenShotProcessor[i].BoardCardSuitCheckRect(i);
                        DrawRect(greenPen, rctToDraw.X, rctToDraw.Y, rctToDraw.Width, rctToDraw.Height, false);
                    }
                }
            }
        }

        /// <summary>
        /// A generic event handler for Button_EnabledChanged event. Update button background colors to enabled or disabled colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Enabled)
            {
                btn.BackColor = rtbBackgroundClr;
            }
            else
            {
                btn.BackColor = controlDisabledClr;
            }
        }

        /// <summary>
        /// The save bitmaps checkbox changed event. Enable and disable the button for choosing the save location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void saveBitmapsChk_CheckChanged(object sender, EventArgs e)
        {
            if (saveBitmapsChk.Checked)
            {
                changeBitmapSaveLocation.Enabled = true;
            }
            else
            {
                changeBitmapSaveLocation.Enabled = false;
            }
        }

        /// <summary>
        /// A generic event handler for CheckBox_EnabledChanged event. Update checkbox background colors to enabled or disabled colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void checkBox_EnabledChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            if (chk.Enabled)
            {
                chk.BackColor = rtbBackgroundClr;
            }
            else
            {
                chk.BackColor = controlDisabledClr;
            }
        }

        /// <summary>
        /// The show click data checkbox changed event. Alternate between showing and not showing the data about locations clicked inside the MainForm window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void showClickDataChk_CheckChanged(object sender, EventArgs e)
        {
            if (showClickDataChk.Checked)
            {
                mouseClickX_Lbl.Text = mouseClickLocationX_Str;
                mouseClickY_Lbl.Text = mouseClickLocationY_Str;
                mouseClickColorLbl.Text = mouseClickLocationColorStr;
                mouseClickBrightnessLbl.Text = mouseClickLocationBrightnessStr;
            }
            else
            {
                mouseClickX_Lbl.Text = mouseClickLocationX_Str + "N/A";
                mouseClickY_Lbl.Text = mouseClickLocationY_Str + "N/A";
                mouseClickColorLbl.Text = mouseClickLocationColorStr + "N/A";
                mouseClickBrightnessLbl.Text = mouseClickLocationBrightnessStr + "N/A";
            }
        }

        /// <summary>
        /// The screenshots mode radio check changed event handler. Enable and disable controls as appropriate for the selected mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void screenshotsModeRdo_CheckChanged(object sender, EventArgs e)
        {
            if (screenshotsModeRdo.Checked)
            {
                startPokerCompanion.Enabled = false;
                openScreenShotFile.Enabled = true;
                bitmapLocationToolsGrp.Enabled = true;
            }
        }

        /// <summary>
        /// The live game mode radio check changed event handler. Enable and disable controls as appropriate for the selected mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void liveGameModeRdo_CheckChanged(object sender, EventArgs e)
        {
            if (liveGameModeRdo.Checked)
            {
                startPokerCompanion.Enabled = true;
                openScreenShotFile.Enabled = false;
                bitmapLocationToolsGrp.Enabled = false;
            }
        }

        /* I have commented out these two event handlers because I decided for simplicity to make this app only work with 9-player tables for now.
         * These radio buttons were intended to facilitate this application functioning with both 9 and 10 player poker tables but I have not
         * finished adding that functionality.
        public async void _9PlayerTableRdo_CheckChanged(object sender, EventArgs e)
        {
            if (_9PlayerTableRdo.Checked)
            {
                is9PlayerTable = true;
                CreateTasks(true);

                for (int i = 0; i < taskCount; i++)
                {
                    screenShotProcessor[i].PlayerCount9();
                }

                if (workerTasks[9].IsCompleted != true)
                {
                    workerTasks[9].Start();
                    await workerTasks[9];
                }
            }
        }

        public void _10PlayerTableRdo_CheckChanged(object sender, EventArgs e)
        {
            if (_10PlayerTableRdo.Checked)
            {
                is9PlayerTable = false;
                CreateTasks(true);

                for (int i = 0; i < taskCount; i++)
                {
                    screenShotProcessor[i].PlayerCount10();
                }
            }
        }
        */

        /// <summary>
        /// The player of interest textbox text changed event handler. Change the player of interest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void playerOfInterestTxt_TextChanged(object sender, EventArgs e)
        {
            playerOfInterestName = playerOfInterestTxt.Text.Trim();
            long playerOfInterestDbHandPlayerId = -1;

            // Cycle through all the players at the table to try to find a matching name
            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    if (playerOfInterestName.ToUpper() == seat[i].Name.ToUpper())
                    {
                        playerOfInterestDbHandPlayerId = seat[i].DbHandPlayerId;
                        break;
                    }
                }
            }

            dataDisplayForm.PlayerOfInterest(playerOfInterestName, playerOfInterestDbHandPlayerId);
        }

        /// <summary>
        /// A generic event handler for TextBox_EnabledChanged event. Update textbox background colors to enabled or disabled colors.
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
                txt.BackColor = controlDisabledClr;
            }
        }

        /// <summary>
        /// The event handler for the screenshot picturebox click event. Find and display the data about the mouse click location if showClickDataChk is checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void screenShotPic_MouseClick(object sender, MouseEventArgs e)
        {
            if (showClickDataChk.Checked)
            {
                DisplayClickLocationData(e.Location);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method processes all the data contained in the bitmap image inside of the screenShotPic picturebox.
        /// </summary>
        private async void ProcessAndUpdateAllData()
        {
            // Check to see if the "Save Bitmaps" checkbox is checked. If so save bitmaps
            if (saveBitmapsChk.Checked)
            {
                saveBitmaps = true;
            }
            else
            {
                saveBitmaps = false;
            }

            StartDealerActionPlayerAndPotTasks();
            StartPlayerAndBoardCardTasks();

            isFirstScreenshot = false;

            await Task.WhenAll(workerTasks);

            // Check for secondary new hand signal which is...
            // When a player didn't have cards before (folded) but has cards now without the dealer changing or
            // if a player showed his cards but is not showing them now. Either of these events indicate a new 
            // hand has started.
            // This happens when the player who was the first to act player (player to the left of the dealer) 
            // gets eliminated. This is done so the next first to act player doesn't skip his blinds.
            if (reRunTasks && !dealerChipNotFound && !dontUpdateData)
            {
                // A secondary new hand event has occured
                RunSecondaryNewHandTasks();
                await Task.WhenAll(workerTasks);
            }
            else
            {
                newHandDetectedRtb.Visible = false;

                if (dealerChipNotFound)
                {
                    isNewDealer = false;
                    reRunTasks = false;

                    // Reset the pot data which may have been updated
                    potCurr = potPrev;
                    isPotSizeChange = false;
                }

                if (dontUpdateData)
                {
                    dontUpdateData = false;
                    potCurr = potPrev;
                    isPotSizeChange = false;
                }
            }

            if (isRunTimers)
            {
                UpdateTimersUiUpdateStart();
            }

            UpdateUiWithTaskData();

            // Re-Create tasks to avoid getting the "Start may not be called on a task that has completed." exception
            CreateTasks(true);

            if (isRunTimers)
            {
                UpdateTimersUpdateAllComplete();
            }

            if (!isLiveGameMode)
            {
                if (!autoNextScreenShotChk.Checked)
                {
                    openScreenShotFile.Enabled = true;
                    autoNextScreenShotChk.Enabled = true;
                    nextScreenShot.Enabled = true;
                }
                else
                {
                    nextScreenShot_Click(null, null);
                }
            }
            else
            {
                startPokerCompanion_Click(null, null);
            }
        }

        /// <summary>
        /// Copy bitmaps synchronously and run tasks asynchronously for task 17 [index 16] (find dealer and action player) and task 16 [index 15] (find pot)
        /// </summary>
        private void StartDealerActionPlayerAndPotTasks()
        {
            // Run these tasks first so we know if there is a new hand before we start processing the players' data and board cards data. This way we
            // can make decisions about what data needs to processed for the players and board cards

            // Start task 16 first (find dealer and action player) then start task 15 (find pot)
            for (int i = 16; i > taskCount - 3; i--)
            {
                int[] dgvIndicies = CopyBitmapsForProcessing(i);

                // Start the stopwatch to time how long it takes to process the bitmap (extract the relevant data)
                workersStp[i].Start();

                // Start the task to process the bitmap (extract the relevant data) 
                workerTasks[i].Start();

                // Log the sequence in which this job (processing the bitmap) was started
                timersDgv.Rows[dgvIndicies[0]].Cells[dgvIndicies[1] + 2].Value = jobStartedCounter;
                jobStartedCounter++;
            }
        }

        /// <summary>
        /// Copy bitmaps synchronously and run tasks asynchronously for each of the 9 or 10 players and 5 possilbe board cards
        /// </summary>
        private void StartPlayerAndBoardCardTasks()
        {
            // Start the player tasks first then the board card tasks
            for (int i = 0; i < taskCount - 2; i++)
            {
                CopyBitmapsForProcessing(i);

                if (i != 9 || !is9PlayerTable)
                {
                    Task tsk = StartProcessorTask(i);
                }
                else
                {
                    if (isRunTimers)
                    {
                        // This is a 9 player table and this is task #9 (player 10) so don't start the task just update timersDgv
                        timersDgv.Rows[0].Cells[16].Value = jobStartedCounter;
                        jobStartedCounter++;
                        timersDgv.Rows[0].Cells[14].Value = string.Format("{0:0.000}", 0);
                        timersDgv.Rows[0].Cells[15].Value = string.Format("{0:0.000}", 0);
                        timersDgv.Rows[0].Cells[17].Value = jobCompletionCounter;
                        jobCompletionCounter++;
                    }
                }
            }
        }

        /// <summary>
        /// Copy and pass the bitmap needed for the screenShotProcessor object to process the data it is responsible for.
        /// </summary>
        /// <param name="index">The index of the screenShotProcessor to copy and pass the bitmap to</param>
        /// <returns>The timersDgv row and worker cell where performance data for this screenShotProcessor should be placed</returns>
        private int[] CopyBitmapsForProcessing(int index)
        {
            // Variables to hold the locations where performance data for these jobs should be placed inside timersDgv
            int timersDgvRow = 0;
            int timersDgvBitmapCell = 0;
            int timersDgvWorkerCell = 0;

            if (isRunTimers)
            {
                if (index < 9)
                {
                    timersDgvRow = index;
                    timersDgvBitmapCell = 1;
                    timersDgvWorkerCell = timersDgvBitmapCell + 8;
                }
                else
                {
                    timersDgvRow = index - 9;
                    timersDgvBitmapCell = 5;
                    timersDgvWorkerCell = timersDgvBitmapCell + 9;
                }

                // Log the sequence in which this job (copying the bitmap) was started
                timersDgv.Rows[timersDgvRow].Cells[timersDgvBitmapCell + 1].Value = jobStartedCounter;
                jobStartedCounter++;

                if (index != 9 || !is9PlayerTable)
                {
                    // Start the stopwatch to time how long it takes to copy the bitmap
                    bitmapCopiesStp.Start();
                }
            }

            if (index != 9 || !is9PlayerTable)
            {
                // Copy the bitmap image for the screenShotProcessor object to work with
                screenShotProcessor[index].NewScreenShot(new Bitmap(screenShotBmp.Clone(screenShotProcessor[index].BitmapRect(index, 0),
                    screenShotBmp.PixelFormat)));
            }

            if (isRunTimers)
            {
                // Stop the stopwatch to time how long it takes to copy the bitmap 
                bitmapCopiesStp.Stop();

                // Log time it took to copy the bitmap
                timersDgv.Rows[timersDgvRow].Cells[timersDgvBitmapCell].Value = string.Format("{0:0.000}", bitmapCopiesStp.ElapsedMilliseconds / 1000.0);

                // Log the sequence in which this job (copying the bitmap) was completed
                timersDgv.Rows[timersDgvRow].Cells[timersDgvBitmapCell + 2].Value = jobCompletionCounter;
                jobCompletionCounter++;

                // Reset the stopwatch used to time how long it takes to copy the bitmap 
                bitmapCopiesStp.Reset();
            }

            // Return timersDgvRow and timersDgvWorkerCell to be used by the caller
            int[] dgvIndices_ = { timersDgvRow, timersDgvWorkerCell };

            return dgvIndices_;
        }

        /// <summary>
        /// Tasks started by this method will await the completion of task #16 before they start. If task #16 has not been completed yet this method will wait asynchronously to start the new task.
        /// </summary>
        /// <param name="TaskIndex">The index of the task to start</param>
        /// <returns>The task to start</returns>
        private async Task StartProcessorTask(int TaskIndex)
        {
            // Start the "StartTask" method here. It is asynchronous and will await the completion of task #16 before it starts
            // any other tasks.

            // Wait for workerTasks #16 to complete so we know if we have a new hand so we know what conditions we need to check for
            await workerTasks[16];

            // Prevent starting this task twice as this method may be called recursively. See longer note at the end of this method 
            // for more info.
            // If the status of this workerTask is "Created" it hasn't started running yet
            if (workerTasks[TaskIndex].Status == TaskStatus.Created)
            {
                if (isRunTimers)
                {
                    // Start the stopwatch to time how long it takes to process the bitmap (extract the relevant data)
                    workersStp[TaskIndex].Start();
                }

                // Start the task to process the bitmap (extract the relevant data) 
                workerTasks[TaskIndex].Start();

                if (isRunTimers)
                {
                    // Variables to hold the locations where performance data for these jobs should be placed inside timersDgv
                    int timersDgvRow, timersDgvWorkerCell;

                    if (TaskIndex < 9)
                    {
                        timersDgvRow = TaskIndex;
                        timersDgvWorkerCell = 9;
                    }
                    else
                    {
                        timersDgvRow = TaskIndex - 9;
                        timersDgvWorkerCell = 14;
                    }

                    // Log the sequence in which this job (processing the bitmap) was started
                    timersDgv.Rows[timersDgvRow].Cells[timersDgvWorkerCell + 2].Value = jobStartedCounter;
                    jobStartedCounter++;

                    // Reset and restart the UI thread idle time timer after each task is started to guarentee it will start  
                    // right after the last task is started so that the UI idle time is measured correctly
                    ResetAndStartUiThreadTimer();
                }

                // Check to see if the previous workerTasks object has already started. If it hasn't that means the bitmap for 
                // the previous task has already been copied but the dealer player hadn't been determined yet so that task was  
                // not started yet. If this is the case start that task because the earlier tasks usually take longer than the 
                // later tasks so getting all the earlier tasks started ASAP will increase the overall processing speed.
                //
                // Check for TaskStatus.Created because if the task has only been created and is not "WaitingToRun", "Running" 
                // or "RanToCompletion" it hasn't started yet.
                // Note: StartProcessorTask(int) is being called recursively here
                if (TaskIndex > 0 && workerTasks[TaskIndex - 1].Status == TaskStatus.Created)
                {
                    await StartProcessorTask(TaskIndex - 1);
                }
            }
        }

        /// <summary>
        /// Run the tasks required to process a secondary new hand event.
        /// </summary>
        private void RunSecondaryNewHandTasks()
        {
            reRunTasks = false;
            newHandDetectedRtb.Visible = true;

            // Re-create the tasks needed to process the secondary new hand event
            CreateTasks(false);

            // Run tasks asynchronously for each of the 10 players and 5 possilbe board cards
            for (int i = 0; i < taskCount - 2; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    if (isRunTimers)
                    {
                        // Start the stopwatch to time how long it takes to process the bitmap (extract the relevant data)
                        workersStp[i].Start();
                    }

                    // Start the task to process the bitmap (extract the relevant data) 
                    workerTasks[i].Start();

                    if (isRunTimers)
                    {
                        // Variables to hold the locations where performance data for these tasks should be placed
                        int timersDgvRow, timersDgvWorkerCell;

                        if (i < 9)
                        {
                            timersDgvRow = i;
                            timersDgvWorkerCell = 9;
                        }
                        else
                        {
                            timersDgvRow = i - 9;
                            timersDgvWorkerCell = 14;
                        }

                        // Log the sequence in which this job (processing the bitmap) was started
                        timersDgv.Rows[timersDgvRow].Cells[timersDgvWorkerCell + 2].Value = jobStartedCounter;
                        jobStartedCounter++;

                        if (i == taskCount - 3)
                        {
                            // This is the last task to run under the secondary new hand event. Reset and start the UI
                            // thread idle time timer to measure the UI thread idle time
                            ResetAndStartUiThreadTimer();
                        }
                    }
                }
                else
                {
                    // This is a 9 player table and this is task #9 (player 10) so don't start the task just update timersDgv
                    if (isRunTimers)
                    {
                        // Log the sequence in which this job (processing the bitmap) was started
                        timersDgv.Rows[0].Cells[16].Value = jobStartedCounter;
                        jobStartedCounter++;
                    }
                }
            }
        }

        /// <summary>
        /// Stop, reset and restart the UI thread timer.
        /// </summary>
        private void ResetAndStartUiThreadTimer()
        {
            uiThreadIdleTimeStp.Stop();
            uiThreadIdleTimeStp.Reset();
            uiThreadIdleTimeStp.Start();
        }

        /// <summary>
        /// Stop the uiThreadIdleTimeStp timer and start the uiUpdateStp timer. Update the UI with the uiThreadIdleTimeStp timer value.
        /// </summary>
        private void UpdateTimersUiUpdateStart()
        {
            // Start the stopwatch to time how long it takes to update the UI
            uiUpdateStp.Start();

            // Stop the stopwatch that measures the UI thread idle time
            uiThreadIdleTimeStp.Stop();

            // Update the UI idle time values
            UiIdleTimeRtb.Clear();
            AppendColoredRtbText(UiIdleTimeRtb, dataIdClr, "UI Idle Time: ");
            UiIdleTimeRtb.AppendText(string.Format("{0:0.000}", uiThreadIdleTimeStp.ElapsedMilliseconds / 1000.0));
        }

        /// <summary>
        /// Update the UI with the new data from the completed tasks.
        /// </summary>
        private void UpdateUiWithTaskData()
        {
            // Update potDealerAction here so the new hand is processed before player (and play chip count) changes. If there is a new hand
            // also handle the UI update it here.
            if (isPotSizeChange || isNewDealer || isNewActionPlayer ||
                isPotChangePrev || isNewDealerPrev || isNewActionPlayerPrev)
            {
                UpdateUiPotDealerActionAndUiForNewPot(isPotSizeChange, isNewDealer, isNewActionPlayer);
            }

            if (PlayerInfoChange())
            {
                UpdatePlayersActionsUi();
            }

            if (isNewDealer || isNewDealerPrev || isBoardCardChangePrev || BoardChange())
            {
                UpdateBoardStatusUi();
            }

            bool isNewBoardCard = false;

            // Find out if a new card has come onto the board
            if (boardCard[0].CardCurrInt != boardCard[0].CardPrevInt || boardCard[3].CardCurrInt != boardCard[3].CardPrevInt ||
                boardCard[4].CardCurrInt != boardCard[4].CardPrevInt)
            {
                isNewBoardCard = true;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    if (actionPlayerIndexCurr == i && (actionPlayerIndexPrev != i || isNewActionPlayer || isNewBoardCard) &&
                        seat[i].Name.ToLower() == playerOfInterestName.ToLower())
                    {
                        // The player of interest is the action player and was not previously the action player, or the player of interest is the  
                        // action player and a new card has come onto the board -> update the DataDisplayForm window
                        UpdateDataDisplayForm();

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Update the potDealerAction rich textbox. Also, if there is a new hand update the UI accordingly.
        /// </summary>
        /// <param name="isPotChange">True if the pot size has changed</param>
        /// <param name="isDealerChange">True if the dealer player has changed</param>
        /// <param name="isActionPlayerChange">True if the action player has changed</param>
        private void UpdateUiPotDealerActionAndUiForNewPot(bool isPotChange, bool isDealerChange, bool isActionPlayerChange)
        {
            potDealerAction.Clear();

            UpdateUiPot(isPotChange);

            AppendColoredRtbText(potDealerAction, dataIdClr, "Dealer:\t");
            string dealerName = GetDealerName();

            if (isDealerChange)
            {
                UpdateUiNewHand(dealerName);
            }
            else
            {
                potDealerAction.AppendText(dealerName + "\n");
            }

            UpdateUiActionPlayer(isActionPlayerChange);
        }

        /// <summary>
        /// Update the pot value in the potDealerAction rich text box.
        /// </summary>
        /// <param name="isPotChange_">True if the pot size has changed</param>
        private void UpdateUiPot(bool isPotChange_)
        {
            AppendColoredRtbText(potDealerAction, dataIdClr, "Pot:\t");

            if (isPotChange_)
            {
                SetColoredChipCountRtbText(potDealerAction, dataChangeClr, potCurr.ToString() + "\n", false);
                isPotChangePrev = true;
            }
            else
            {
                SetColoredChipCountRtbText(potDealerAction, normalTextClr, potCurr.ToString() + "\n", false);
                isPotChangePrev = false;
            }
        }

        /// <summary>
        /// Gets the name of the dealer player.
        /// </summary>
        /// <returns>The name of the dealer player</returns>
        private string GetDealerName()
        {
            if (dealerIndexCurr != -1)
            {
                return GetPlayerName(dealerIndexCurr);
            }
            else
            {
                return "None";
            }
        }

        /// <summary>
        /// Update the action player name in the potDealerAction rich text box.
        /// </summary>
        /// <param name="isActionPlayerChange_">True if the action player has changed</param>
        private void UpdateUiActionPlayer(bool isActionPlayerChange_)
        {
            AppendColoredRtbText(potDealerAction, dataIdClr, "Action Player:\t");
            string actionPlayerName;

            if (actionPlayerIndexCurr != -1)
            {
                actionPlayerName = GetPlayerName(actionPlayerIndexCurr);
            }
            else
            {
                actionPlayerName = "None";
            }

            if (isActionPlayerChange_)
            {
                AppendColoredRtbText(potDealerAction, dataChangeClr, actionPlayerName);
                isNewActionPlayerPrev = true;
            }
            else
            {
                potDealerAction.AppendText(actionPlayerName);
                isNewActionPlayerPrev = false;
            }
        }

        /// <summary>
        /// Update the UI for a new hand -> Announce the winner(s) of the previous hand, announce the number of chips each player is starting this hand with, announce the ante for this hand, announce the big blind and small blind for this hand.
        /// </summary>
        /// <param name="dealerName_">The name of the dealer player</param>
        private void UpdateUiNewHand(string dealerName_)
        {
            gameActions.Clear();

            isShowdown = false;
            int[] chipsWon = new int[10];
            bool[] isWinner = new bool[10];

            int winnersCount = UpdateUiPreviousHandWinner(chipsWon, isWinner);

            UpdateUiNewDealer(dealerName_);

            // Set intHandActionNum = 1 for DB logging purposes because this is the start of a new hand
            dbHandActionNum = 1;

            // Reset the bbPlayer and sbPlayer indices for the new hand
            bbPlayerIndex = -1;
            sbPlayerIndex = -1;

            // Insert new hand into DB and get HandId
            dbHandId = db_.InsertHand(dbGameId, 0, errorMessagesTxt);

            int playersInHand = 0;
            int bbChips = 0;
            int sbChips = 0;

            UpdatePlayerNamesChipsBbSb(ref playersInHand, ref bbChips, ref sbChips, chipsWon, isWinner);

            // Is there a small blind player in this hand? (There could be no small blind player under special circumstances)
            bool smallBlindExists = false;
            int anteAmount = 0;

            FindAnteAndSmallBlind(ref anteAmount, ref smallBlindExists, playersInHand, sbChips);

            UpdateUiAntesAndBlinds(playersInHand, anteAmount, sbChips, bbChips, smallBlindExists);

            // DB stuff: Update "Ante" field in "Hand" table
            db_.UpdateHand_Ante(dbHandId, anteAmount);

            if (sbPlayerIndex >= 0)
            {
                // DB stuff: Update "Blind" field in "HandPlayer" table
                db_.UpdateHandPlayer_Blind(seat[sbPlayerIndex].DbHandPlayerId, (sbChips - anteAmount));
            }
            if (bbPlayerIndex >= 0)
            {
                // DB stuff: Update "Blind" field in "HandPlayer" table
                db_.UpdateHandPlayer_Blind(seat[bbPlayerIndex].DbHandPlayerId, (bbChips - anteAmount));
            }
        }

        /// <summary>
        /// Update the UI with the previous hand winner or winners.
        /// </summary>
        /// <param name="chipsWon_">An array containing the number of chips each player won in the hand</param>
        /// <param name="isWinner_">An Array containing true for each player that was a winner in the hand and false for each player that was a loser in the hand</param>
        /// <returns>The number of winners in the hand (the number of players that won or split the pot)</returns>
        private int UpdateUiPreviousHandWinner(int[] chipsWon_, bool[] isWinner_)
        {
            int sumOfAllBets = FindSumOfAllBets();

            // Make sure the pot amount is equal to the sum of the all the players' bets (sumOfAllBets). If it isn't
            // someone called a bet that put them all in (someone bet more chips than another player had and that player
            // called).
            if (sumOfAllBets != potPrev)
            {
                CorrectAllInChipCounts(sumOfAllBets);
            }

            int winnersCount_ = FindAllHandWinners(chipsWon_, isWinner_);

            int winnerWithLeastChipsInIndex = -1;
            int winnerWithLeastChipsInChips = 0;

            bool isEqualSplit = DetermineWinCircumstances(winnersCount_, ref winnerWithLeastChipsInChips, ref winnerWithLeastChipsInIndex,
                chipsWon_, isWinner_);

            if (winnersCount_ > 1)
            {
                // We have more than one winner

                if (isEqualSplit)
                {
                    // Winners split the winnings evenly
                    UpdateUiEqualSplit(winnersCount_, chipsWon_, isWinner_);
                }
                else
                {
                    // Winners do not split the winnings evenly
                    UpdateUiUnEqualSplit(winnersCount_, winnerWithLeastChipsInChips, winnerWithLeastChipsInIndex, chipsWon_, isWinner_);
                }
            }

            if (winnersCount_ > 0)
            {
                gameActions.AppendText("\n");
            }

            return winnersCount_;
        }

        /// <summary>
        /// Find the sum of all bets from the previous hand.
        /// </summary>
        /// <returns>The sum of all bets from the previous hand</returns>
        private int FindSumOfAllBets()
        {
            int sumOfBets = 0;

            // Find the sum of all bets made in the last hand
            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    sumOfBets += seat[i].ChipsInPot;
                }
            }

            return sumOfBets;
        }

        /// <summary>
        /// Correct for bets made by one player and called by another player who did not have enough chips to call the bet.
        /// </summary>
        /// <param name="sumOfAllBets_">The sum of all the bets made by the players in the hand</param>
        private void CorrectAllInChipCounts(int sumOfAllBets_)
        {
            int mostChipsInPot = 0;
            int secondMostChipsInPot = 0;
            int playerMostChipsInPotIndex = -1;
            int playerSecondMostChipsInPotIndex = -1;

            // Find the two players who put the most chips into the pot
            for (int i = 0; i < 10; i++)
            {
                if (seat[i].ChipsInPot > mostChipsInPot)
                {
                    secondMostChipsInPot = mostChipsInPot;
                    playerSecondMostChipsInPotIndex = playerMostChipsInPotIndex;

                    mostChipsInPot = seat[i].ChipsInPot;
                    playerMostChipsInPotIndex = i;
                }
                else if (seat[i].ChipsInPot > secondMostChipsInPot)
                {
                    secondMostChipsInPot = seat[i].ChipsInPot;
                    playerSecondMostChipsInPotIndex = i;
                }
            }


            // If playerMostChipsInPotIndex > -1 is false no players have any chips in the pot which would indicate an incomplete/invalid hand
            // Also if bbPlayerIndex == -1 this is an incomplete hand (we don't have the data from the beginning of the hand)
            // -> don't run the code below for verifying chip counts
            if (playerMostChipsInPotIndex > -1 && bbPlayerIndex != -1)
            {
                // If we have reached this point we have a valid hand

                // Fix the ChipsInPot and ChipStackPrev values for the player with the most chips in the pot. This player made a bet that the
                // player with the second most chips in the pot called but that player didn't have enough chips to cover the bet
                seat[playerMostChipsInPotIndex].ChipsInPot -= sumOfAllBets_ - potPrev;
                seat[playerMostChipsInPotIndex].ChipStackPrev += sumOfAllBets_ - potPrev;

                // This if statement is only to verify that everything was done correctly. If all goes well this if statement will
                // never be true.
                if (seat[playerMostChipsInPotIndex].ChipsInPot != seat[playerSecondMostChipsInPotIndex].ChipsInPot)
                {
                    if (errorMessagesTxt.Text.Length != 0)
                    {
                        errorMessagesTxt.AppendText("\r\n");
                    }
                    errorMessagesTxt.AppendText("Houston we have a problem!!!, A Chip Accounting Error has Occcured - " +
                        DateTime.Now.ToLongTimeString());

                    // Try to fix the error here
                    seat[playerMostChipsInPotIndex].ChipsInPot = seat[playerSecondMostChipsInPotIndex].ChipsInPot;
                }
            }
        }

        /// <summary>
        /// Find all the winners of the previous hand and the total number of winners (not how much they won, just who they are).
        /// </summary>
        /// <param name="chipsWon__">An array containing the number of chips each player won in the hand</param>
        /// <param name="isWinner__">An Array containing true for each player that was a winner in the hand and false for each player that was a loser in the hand</param>
        /// <returns>The number of winners in the hand (the number of players that won or split the pot)</returns>
        private int FindAllHandWinners(int[] chipsWon__, bool[] isWinner__)
        {
            int winnersCount__ = 0;

            for (int i = 0; i < 10; i++)
            {
                // Initialize the arrays
                chipsWon__[i] = 0;
                isWinner__[i] = false;

                if (i != 9 || !is9PlayerTable)
                {
                    if (seat[i].ChipStackPrev != -1)
                    {
                        if (seat[i].ChipStackCurr > seat[i].ChipStackPrev)
                        {
                            isWinner__[i] = true;
                            winnersCount__++;
                        }
                    }
                }
            }

            return winnersCount__;
        }

        /// <summary>
        /// If there was one winner update the UI accordingly. If there was more than one winner detrmine if they split the pot evenly.
        /// </summary>
        /// <param name="winnersCount__">The number of winners in the hand (the number of players that won or split the pot)</param>
        /// <param name="winnerWithLeastChipsInChips_">The number of chips the winner with the least chips in the pot put into the pot</param>
        /// <param name="winnerWithLeastChipsInIndex_">The index of the winner with the least chips in the pot</param>
        /// <param name="chipsWon__">An array containing the number of chips each player won in the hand</param>
        /// <param name="isWinner__">An Array containing true for each player that was a winner in the hand and false for each player that was a loser in the hand</param>
        /// <returns>True if the pot was split evenly (or if there was one winner), false otherwise</returns>
        private bool DetermineWinCircumstances(int winnersCount__, ref int winnerWithLeastChipsInChips_, ref int winnerWithLeastChipsInIndex_,
            int[] chipsWon__, bool[] isWinner__)
        {
            bool isEqualSplit_ = true;

            for (int i = 0; i < 10; i++)
            {
                if (isWinner__[i])
                {
                    if (winnersCount__ == 1)
                    {
                        // One winner -> update the UI accordingly
                        chipsWon__[i] = potPrev;
                        AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(i) + ":");
                        gameActions.AppendText(" won " + (chipsWon__[i]).ToString() + "\n");

                        // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                        db_.InsertPlayerAction(seat[i].DbHandPlayerId, -1 * chipsWon__[i], dbHandActionNum, errorMessagesTxt);
                        dbHandActionNum++;
                    }
                    else
                    {
                        // More than one winner -> figure out if all the winners put the same number of chips into the pot

                        if (winnerWithLeastChipsInChips_ == 0)
                        {
                            // Set the first winner to be the winner with the least chips in the pot
                            winnerWithLeastChipsInChips_ = seat[i].ChipsInPot;
                            winnerWithLeastChipsInIndex_ = i;
                        }
                        else
                        {
                            if (winnerWithLeastChipsInChips_ != seat[i].ChipsInPot)
                            {
                                isEqualSplit_ = false;

                                if (seat[i].ChipsInPot < winnerWithLeastChipsInChips_)
                                {
                                    // Update the winner with the least chips in the pot
                                    winnerWithLeastChipsInChips_ = seat[i].ChipsInPot;
                                    winnerWithLeastChipsInIndex_ = i;
                                }
                            }
                        }
                    }
                }
            }

            return isEqualSplit_;
        }

        /// <summary>
        /// Update the UI for an equal split (each winner put the same number of chips into the pot although the pot might not divide up equally between the winners).
        /// </summary>
        /// <param name="winnersCount__">The number of winners in the hand (the number of players that won or split the pot)</param>
        /// <param name="chipsWon__">An array containing the number of chips each player won in the hand</param>
        /// <param name="isWinner__">An Array containing true for each player that was a winner in the hand and false for each player that was a loser in the hand</param>
        private void UpdateUiEqualSplit(int winnersCount__, int[] chipsWon__, bool[] isWinner__)
        {
            // The number of chips that don't split evenly between the winners
            int equalSplitRemainderChips = potPrev % winnersCount__;

            int winnerIndex;

            for (int z = 0; z < 10; z++)
            {
                // Handle the winners in order starting with the first winner to the left of the dealer
                winnerIndex = dealerIndexPrev + 1 + z;

                if (winnerIndex > 9)
                {
                    winnerIndex -= 10;
                }

                chipsWon__[winnerIndex] = 0;

                if (isWinner__[winnerIndex])
                {
                    if (equalSplitRemainderChips != 0)
                    {
                        // Deal with the special case where the pot doesn't split evenly amongst the winners. There are different ways
                        // to handle this case but it looks like Bet Online's poker site handles this case by giving the remaining
                        // chips that don't split evenly amongst the winners (usually one chip) to the the winners to the left of the
                        // dealer chip one each until there are none left. 

                        // This is the explanation I found that Bet Online's site seems to follow:
                        // In games with blinds (like Texas Holdem and Draw games), extra chips are given to the players in order after 
                        // the button. That is, eldest hand gets first odd chip, second eldest hand gets the next one, etc. 
                        chipsWon__[winnerIndex]++;
                        equalSplitRemainderChips--;
                    }

                    // Split the pot into equal parts for each of the winners
                    chipsWon__[winnerIndex] += potPrev / winnersCount__;
                    AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(winnerIndex) + ":");
                    gameActions.AppendText(" won " + (chipsWon__[winnerIndex]).ToString() + "\n");

                    // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                    db_.InsertPlayerAction(seat[winnerIndex].DbHandPlayerId, -1 * chipsWon__[winnerIndex], dbHandActionNum,
                        errorMessagesTxt);
                    dbHandActionNum++;
                }
            }
        }

        /// <summary>
        /// Update the UI for an un-equal split (each winner did not put the same number of chips into the pot)
        /// </summary>
        /// <param name="winnersCount__">The number of winners in the hand (the number of players that won or split the pot)</param>
        /// <param name="winnerWithLeastChipsInChips_">The number of chips the winner with the least chips in the pot put into the pot</param>
        /// <param name="winnerWithLeastChipsInIndex_">The index of the winner with the least chips in the pot</param>
        /// <param name="chipsWon__">An array containing the number of chips each player won in the hand</param>
        /// <param name="isWinner__">An Array containing true for each player that was a winner in the hand and false for each player that was a loser in the hand</param>
        private void UpdateUiUnEqualSplit(int winnersCount__, int winnerWithLeastChipsInChips_, int winnerWithLeastChipsInIndex_,
            int[] chipsWon__, bool[] isWinner__)
        {
            // Ok, we have more than one winner and one or more winners (but not all) went all-in.
            // We know that players that didn't go all-in must be splitting the pot with at least one other player.
            // The all-in player could have won his side pot or could have split it.
            // If the winning player who put in the least chips has a current chip stack of more than half his share of 
            // the pot (calculated by summing what everyone put in up to how much this player put in) then he won the whole
            // side pot. If not I will assume he is splitting his share of the side pot with all other players and they are 
            // all splitting the main pot (note, I am effectively assuming there is only one side pot). I know that there 
            // could be other more complicated cases but I believe this will address the vast majority of cases. I also note 
            // that the way to get these numers correct every time is to read the chips passed to each player from the pot 
            // at a showdown, but that would be a lot of work for a very rare occurrence so I'm not going to deal with it 
            // right now.

            int mainPot = 0;
            int sidePot = 0;

            // Find the values of the main pot and the side pot
            for (int i = 0; i < 10; i++)
            {
                if (seat[i].ChipsInPot <= winnerWithLeastChipsInChips_)
                {
                    mainPot += seat[i].ChipsInPot;
                }
                else
                {
                    mainPot += winnerWithLeastChipsInChips_;
                    sidePot += seat[i].ChipsInPot - winnerWithLeastChipsInChips_;
                }
            }

            if (seat[winnerWithLeastChipsInIndex_].ChipStackCurr > mainPot / 2)
            {
                // The winner with the least chips in won the entire main pot
                chipsWon__[winnerWithLeastChipsInIndex_] = mainPot;
                AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(winnerWithLeastChipsInIndex_) + ":");
                gameActions.AppendText(" won " + (chipsWon__[winnerWithLeastChipsInIndex_]).ToString() + "\n");

                // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                db_.InsertPlayerAction(seat[winnerWithLeastChipsInIndex_].DbHandPlayerId, -1 * chipsWon__[winnerWithLeastChipsInIndex_],
                    dbHandActionNum, errorMessagesTxt);
                dbHandActionNum++;

                for (int i = 0; i < 10; i++)
                {
                    if (isWinner__[i] && i != winnerWithLeastChipsInIndex_)
                    {
                        // A winner other than the winner with the least chips in the pot is splitting the side pot
                        chipsWon__[i] = sidePot / (winnersCount__ - 1);
                        AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(i) + ":");
                        gameActions.AppendText(" won " + (chipsWon__[i]).ToString() + "\n");

                        // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                        db_.InsertPlayerAction(seat[i].DbHandPlayerId, -1 * chipsWon__[i], dbHandActionNum, errorMessagesTxt);
                        dbHandActionNum++;
                    }
                }
            }
            else
            {
                // The winner with the least chips in the pot splits the side pot with everyone else and everyone else splits 
                // the side pot.
                for (int i = 0; i < 10; i++)
                {
                    if (isWinner__[i])
                    {
                        AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(i) + ":");

                        if (i == winnerWithLeastChipsInIndex_)
                        {
                            chipsWon__[i] = mainPot / winnersCount__;
                            gameActions.AppendText(" won " + (chipsWon__[i]).ToString() + "\n");

                            // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                            db_.InsertPlayerAction(seat[i].DbHandPlayerId, -1 * chipsWon__[i], dbHandActionNum, errorMessagesTxt);
                            dbHandActionNum++;
                        }
                        else
                        {
                            chipsWon__[i] = (mainPot / winnersCount__) + (sidePot / (winnersCount__ - 1));
                            gameActions.AppendText(" won " + (chipsWon__[i]).ToString() + "\n");

                            // (DB stuff) Update the DB with a negative chip count change for the winner, indicating a win
                            db_.InsertPlayerAction(seat[i].DbHandPlayerId, -1 * chipsWon__[i], dbHandActionNum, errorMessagesTxt);
                            dbHandActionNum++;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Update the new dealer name in the potDealerAction rich text box.
        /// </summary>
        /// <param name="dealerName__">The name of the new dealer player</param>
        private void UpdateUiNewDealer(string dealerName__)
        {
            AppendColoredRtbText(potDealerAction, dataChangeClr, dealerName__ + "\n");
            AppendColoredRtbText(gameActions, dataChangeClr, "New Dealer: ");
            AppendColoredRtbText(gameActions, dataIdClr, dealerName__ + "\n\n");
        }

        /// <summary>
        /// Update player names (add new players to the DB), display the players' starting chip stacks and find the big blind and small blind chip values.
        /// </summary>
        /// <param name="playersInHand_">The number of players in the new hand</param>
        /// <param name="bbChips_">The value of the big blind (how many chips)</param>
        /// <param name="sbChips_">The value of the small blind (how many chips)</param>
        /// <param name="chipsWon_">An array containing the number of chips each player won in the previous hand</param>
        /// <param name="isWinner_">An Array containing true for each player that was a winner in the previous hand and false for each player that was a loser in the previous hand</param>
        private void UpdatePlayerNamesChipsBbSb(ref int playersInHand_, ref int bbChips_, ref int sbChips_, int[] chipsWon_, bool[] isWinner_)
        {
            string currentPlayer;

            for (int i = 0; i < 10; i++)
            {
                // Zero out the chips in pot for each player
                seat[i].ChipsInPot = 0;

                if (i != 9 || !is9PlayerTable)
                {
                    if (seat[i].ChipStackCurr > 0 || seat[i].ChipStackPrev > 0)
                    {
                        currentPlayer = GetPlayerName(i);

                        // Check if player exists in the DB (if result is < 1 player does not exist in DB -> add the player)
                        seat[i].DbPlayerId = db_.QueryPlayerExists(currentPlayer);

                        if (seat[i].DbPlayerId < 1)
                        {
                            seat[i].DbPlayerId = db_.InsertPlayer(currentPlayer, errorMessagesTxt);
                        }

                        // Document how many chips each player is starting with and what they have in the pot
                        AppendColoredRtbText(gameActions, dataChangeClr, currentPlayer + ":");
                        if (!isWinner_[i])
                        {
                            if (seat[i].ChipStackPrev != -1)
                            {
                                // This player was a loser in the previous hand
                                seat[i].ChipsInPot = seat[i].ChipStackPrev - seat[i].ChipStackCurr;
                                gameActions.AppendText(" has " + seat[i].ChipStackPrev.ToString() + "\n");
                                playersInHand_++;
                                seat[i].DbHandPlayerId = db_.InsertHandPlayer(dbHandId, seat[i].DbPlayerId, seat[i].ChipStackPrev,
                                    0, 0, 0, errorMessagesTxt);
                            }
                            else
                            {
                                // This is this player's first hand
                                gameActions.AppendText(" has " + seat[i].ChipStackCurr.ToString() + "\n");
                                playersInHand_++;
                                seat[i].DbHandPlayerId = db_.InsertHandPlayer(dbHandId, seat[i].DbPlayerId, seat[i].ChipStackCurr,
                                    0, 0, 0, errorMessagesTxt);
                            }
                        }
                        else
                        {
                            // This player was a winner in the previous hand
                            seat[i].ChipsInPot = seat[i].ChipStackPrev + chipsWon_[i] - seat[i].ChipStackCurr;
                            gameActions.AppendText(" has " + (seat[i].ChipStackPrev + chipsWon_[i]).ToString() + "\n");
                            playersInHand_++;
                            seat[i].DbHandPlayerId = db_.InsertHandPlayer(dbHandId, seat[i].DbPlayerId,
                                seat[i].ChipStackPrev + chipsWon_[i], 0, 0, 0, errorMessagesTxt);
                        }

                        // Update player of interest data in DataDisplayForm.cs
                        if (playerOfInterestName == seat[i].Name)
                        {
                            dataDisplayForm.PlayerOfInterest(seat[i].Name, seat[i].DbHandPlayerId);
                        }

                        // Find the big blind and small blind
                        if (seat[i].ChipsInPot > bbChips_)
                        {
                            sbChips_ = bbChips_;
                            sbPlayerIndex = bbPlayerIndex;

                            bbChips_ = seat[i].ChipsInPot;
                            bbPlayerIndex = i;
                        }
                        else if (seat[i].ChipsInPot > sbChips_)
                        {
                            sbChips_ = seat[i].ChipsInPot;
                            sbPlayerIndex = i;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find the ante amount and check to make sure a small blind player exits (there could be no small blind player under special circumstances). If a small blind player exists find the value of the small blind.
        /// </summary>
        /// <param name="anteAmount_">The value of the ante (number of chips)</param>
        /// <param name="smallBlindExists_">Does a small blind player exist</param>
        /// <param name="playersInHand_">The number of players in this hand</param>
        /// <param name="sbChips_">The value of the small blind (number of chips)</param>
        /// <returns></returns>
        private void FindAnteAndSmallBlind(ref int anteAmount_, ref bool smallBlindExists_, int playersInHand_, int sbChips_)
        {
            // If there are only two players left assume there is no ante and one player is paying the big blind and the other is paying the small
            // blind although this is not strictly true
            if (playersInHand_ > 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i != bbPlayerIndex && i != sbPlayerIndex && !smallBlindExists_)
                    {
                        if (seat[i].ChipsInPot > 0)
                        {
                            anteAmount_ = seat[i].ChipsInPot;
                            if (sbChips_ > anteAmount_)
                            {
                                smallBlindExists_ = true;
                            }
                        }
                    }
                }
            }

            if (anteAmount_ > 0)
            {
                gameActions.AppendText("\n");
            }
        }

        /// <summary>
        /// Announce the ante players, the small blind player and the big blind player.
        /// </summary>
        /// <param name="playersInHand_">The number of players in this hand</param>
        /// <param name="anteAmount_">The value of the ante (number of chips)</param>
        /// <param name="sbChips_">The value of the small blind (number of chips)</param>
        /// <param name="bbChips_">The value of the big blind (number of chips)</param>
        /// <param name="smallBlindExists_">Does a small blind player exist</param>
        private void UpdateUiAntesAndBlinds(int playersInHand_, int anteAmount_, int sbChips_, int bbChips_, bool smallBlindExists_)
        {
            // Announce Ante Players
            if (playersInHand_ > 2)
            {
                string currentPlayer;
                for (int i = 0; i < 10; i++)
                {
                    if (seat[i].ChipsInPot > 0)
                    {
                        currentPlayer = GetPlayerName(i);

                        AppendColoredRtbText(gameActions, dataChangeClr, currentPlayer + ":");
                        gameActions.AppendText(" Ante " + (anteAmount_) + "\n");
                    }
                }
            }

            // Announce Sb Player
            if (smallBlindExists_)
            {
                string sbPlayerName = GetPlayerName(sbPlayerIndex);

                AppendColoredRtbText(gameActions, dataChangeClr, "\n" + sbPlayerName + ":");
                gameActions.AppendText(" SB " + (sbChips_ - anteAmount_) + "\n");
            }
            else
            {
                sbPlayerIndex = -1;
                gameActions.AppendText("\n");
            }

            // Announce Bb Player
            if (bbPlayerIndex > -1)
            {
                string bbPlayerName = GetPlayerName(bbPlayerIndex);

                AppendColoredRtbText(gameActions, dataChangeClr, bbPlayerName + ":");
                gameActions.AppendText(" BB " + (bbChips_ - anteAmount_) + "\n");
            }

            gameActions.AppendText("\n");
        }

        /// <summary>
        /// Has the data changed for any of the players?
        /// </summary>
        /// <returns>True if one of more of the players' data has been updated</returns>
        private bool PlayerInfoChange()
        {
            bool isChange = false;

            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    if (seat[i].PlayerChange)
                    {
                        // The player in this seat has changed
                        isChange = true;
                        seat[i].PlayerChange = false;
                    }

                    if (!isChange && seat[i].ChipStackCurr != seat[i].ChipStackPrev &&
                        seat[i].ChipStackCurr != ProcessScreenShots.ChipStackValueCoverdByBanner)
                    {
                        // The player's chip stack has changed
                        isChange = true;
                    }

                    if (!isChange && seat[i].OpenSeatCurr != seat[i].OpenSeatPrev)
                    {
                        // A new player is at the table or the previous player has left
                        isChange = true;
                    }

                    if (!isChange && seat[i].ChipStackChangePrev || seat[i].OpenSeatChangePrev)
                    {
                        // There was a data change on the previous UI update
                        isChange = true;
                    }
                    if (seat[i].HoldCardsCurr[0] != seat[i].HoldCardsPrev[0] || seat[i].HoldCardsCurr[1] != seat[i].HoldCardsPrev[1])
                    {
                        // The status of the player's hold cards has changed
                        isChange = true;
                        seat[i].HoldCardChange = true;
                    }
                }
            }

            return isChange;
        }

        /// <summary>
        /// Update the UI to reflect any new player actions.
        /// </summary>
        private void UpdatePlayersActionsUi()
        {
            // firstPlayerToActIndex is intended to be the first player to act during this screenshot capture. This variable is  
            // important because if there are two players that perform an aciton in the same screenshot we want the actions to  
            // be announced (and saved to the DB) in the order they actually happened
            int firstPlayerToActIndex = FindFirstPlayerToActIndex();
            int i;

            for (int z = 0; z < 10; z++)
            {
                // Iterate through the players in order starting with the firstPlayerToActIndex
                i = z + firstPlayerToActIndex;

                if (i > 9)
                {
                    i -= 10;
                }

                if (i != 9 || !is9PlayerTable)
                {
                    seat[i].ChipStackChangePrev = false;

                    if (!seat[i].OpenSeatCurr)
                    {
                        // This player's seat is NOT open

                        if (seat[i].NewPlayer)
                        {
                            UpdateUiAndDbForNewPlayer(i);
                        }

                        if (isNewDealer && seat[i].FirstActionFastFold)
                        {
                            // Address a special condition where the player folds immediately at the begining of a hand and there is no screenshot
                            // that shows this player having hold cards

                            AppendColoredRtbText(gameActions, dataChangeClr, seat[i].Name + ":");
                            gameActions.AppendText(" folded\n");
                            gameActions.ScrollToCaret();

                            // DB stuff: Use "Chip Count Change" of 0 to mean the player folded
                            db_.InsertPlayerAction(seat[i].DbHandPlayerId, 0, dbHandActionNum, errorMessagesTxt);
                            dbHandActionNum++;
                        }
                        else if (seat[i].HoldCardChange)
                        {
                            seat[i].HoldCardChange = false;
                            UpdateUiAndDbForHoldCardChange(i);
                        }

                        seat[i].FirstActionFastFold = false;

                        string chipStackStr = UpdatePlayerChipStack(i);

                        UpdatePlayerChipActionUi(i, chipStackStr);
                    }
                }
            }
        }

        /// <summary>
        /// Find the first player to act on the current screen capture.
        /// </summary>
        /// <returns>The index of the first player to act on the current screen capture.</returns>
        private int FindFirstPlayerToActIndex()
        {
            int firstPlayerToActIndex_;

            if (isNewDealer || (isNewDealerPrev && actionPlayerIndexCurr == -1))
            {
                firstPlayerToActIndex_ = bbPlayerIndex + 1;
            }
            else
            {
                firstPlayerToActIndex_ = actionPlayerIndexPrev;
            }

            if (firstPlayerToActIndex_ > 9)
            {
                firstPlayerToActIndex_ -= 10;
            }

            return firstPlayerToActIndex_;
        }

        /// <summary>
        /// Update the UI and DB with the data for a new player at the table.
        /// </summary>
        /// <param name="playerIndex">The index of the new player</param>
        private void UpdateUiAndDbForNewPlayer(int playerIndex)
        {
            // DB stuff: Check to see if this player exists in the DB (if result is < 1 player does not exist in DB)
            seat[playerIndex].DbPlayerId = db_.QueryPlayerExists(seat[playerIndex].Name);
            if (seat[playerIndex].DbPlayerId < 1)
            {
                // This player does not exist in the DB -> add the player
                seat[playerIndex].DbPlayerId = db_.InsertPlayer(seat[playerIndex].Name, errorMessagesTxt);
            }

            // DB stuff: Update the HandPlayerID entry with the new player's DbPlayerId
            db_.UpdateHandPlayer_Player(seat[playerIndex].DbHandPlayerId, seat[playerIndex].DbPlayerId);

            // Announce new player's name
            AppendColoredRtbText(gameActions, dataChangeClr, seat[playerIndex].Name + ":");
            gameActions.AppendText(" is Player " + (playerIndex + 1).ToString() + "\n");
            gameActions.ScrollToCaret();

            seat[playerIndex].NewPlayer = false;
        }

        /// <summary>
        /// Update the UI and database to reflect a hold card(s) change. This could be a fold, showing cards after betting is complete or the player of interest's cards being visible.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose hold cards have changed</param>
        private void UpdateUiAndDbForHoldCardChange(int playerIndex)
        {
            if (seat[playerIndex].HoldCardsCurr[0] == CardTranslator.NoCardPresent && !isShowdown)
            {
                // Check to make sure other players are left before calling it a fold.
                // This is necessary because the winner's cards disappear before the hand is over which would be misinterpreted as a fold.
                // Also if two players have cards at the end it was a showdown and there are no folds after a showdown.
                for (int j = 0; j < 10; j++)
                {
                    if (j != playerIndex && seat[j].HoldCardsCurr[0] != CardTranslator.NoCardPresent && !seat[j].OpenSeatCurr)
                    {
                        // Other players are still in the hand -> The player we are checking has folded
                        if (seat[playerIndex].Name != "")
                        {
                            AppendColoredRtbText(gameActions, dataChangeClr, seat[playerIndex].Name + ":");
                        }
                        else
                        {
                            AppendColoredRtbText(gameActions, dataChangeClr, "Player " + (playerIndex + 1).ToString() + ":");
                        }
                        gameActions.AppendText(" folded\n");
                        gameActions.ScrollToCaret();

                        // DB stuff (use "Chip Count Change" of 0 to mean the player folded)
                        db_.InsertPlayerAction(seat[playerIndex].DbHandPlayerId, 0, dbHandActionNum, errorMessagesTxt);
                        dbHandActionNum++;

                        // If the player we are checking folded break out of the loop. We only need to show this once
                        break;
                    }
                }
            }
            else if ((seat[playerIndex].HoldCardsCurr[0] != CardTranslator.CardFaceDown &&
                seat[playerIndex].HoldCardsCurr[0] != CardTranslator.NoCardPresent) ||
                (seat[playerIndex].HoldCardsCurr[1] != CardTranslator.CardFaceDown &&
                seat[playerIndex].HoldCardsCurr[1] != CardTranslator.NoCardPresent))
            {
                // Player has shown at least one hold card
                if (seat[playerIndex].Name.ToLower() != playerOfInterestName.ToLower() &&
                    seat[playerIndex].NameTemp.ToLower() != playerOfInterestName.ToLower())
                {
                    // Not the player of interest
                    AppendColoredRtbText(gameActions, dataChangeClr, GetPlayerName(playerIndex) + ":");
                    gameActions.AppendText(" shows " + CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[0]) + " " +
                        CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[1]) + "\n");
                    gameActions.ScrollToCaret();

                    // DB stuff
                    if (seat[playerIndex].DbHandPlayerId != -1)
                    {
                        // Log the cards the player showed
                        if (seat[playerIndex].HoldCardsCurr[0] != CardTranslator.CardFaceDown &&
                            seat[playerIndex].HoldCardsCurr[1] != CardTranslator.CardFaceDown)
                        {
                            // Player shows both cards
                            db_.UpdateHandPlayer_HcBoth(seat[playerIndex].DbHandPlayerId, (short)seat[playerIndex].HoldCardsCurr[0],
                                (short)seat[playerIndex].HoldCardsCurr[1]);
                        }
                        else if (seat[playerIndex].HoldCardsCurr[0] != CardTranslator.CardFaceDown)
                        {
                            // Player only shows their first card
                            db_.UpdateHandPlayer_Hc1(seat[playerIndex].DbHandPlayerId, (short)seat[playerIndex].HoldCardsCurr[0]);
                        }
                        else if (seat[playerIndex].HoldCardsCurr[1] != CardTranslator.CardFaceDown)
                        {
                            // Player only shows their second card
                            db_.UpdateHandPlayer_Hc2(seat[playerIndex].DbHandPlayerId, (short)seat[playerIndex].HoldCardsCurr[1]);
                        }
                    }

                    // Cards are only shown on hand in progress if we have a showdown
                    isShowdown = true;

                    // Look for the "All-In" text over a player's name. Run this method now, on the main thread.
                    // It doesn't make sense to start up a bunch of tasks running on other threads to handle this work. There
                    // are most likely only two players whose names need to be checked for the text "All-In".
                    CheckForAndProcessAllInPlayer(playerIndex);
                }
                else
                {
                    // The player showing their cards is the player of interest
                    AppendColoredRtbText(gameActions, dataChangeClr, "You have: ");
                    gameActions.AppendText(CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[0]) + " " +
                        CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[1]) + "\n");
                    gameActions.ScrollToCaret();

                    // DB stuff
                    if (seat[playerIndex].DbHandPlayerId != -1)
                    {
                        // Log the cards the player showed
                        db_.UpdateHandPlayer_HcBoth(seat[playerIndex].DbHandPlayerId, (short)seat[playerIndex].HoldCardsCurr[0],
                            (short)seat[playerIndex].HoldCardsCurr[1]);
                    }

                    DisplayPlayerOfInterestHandUi(playerIndex);
                }
            }
        }

        /// <summary>
        /// Check to see if a player has the words "All-In" displayed where their name would normally be shown. If so put that player all-in (reduce his chipstack to 0) and make sure all other players remaining in the hand have either put in as many chips as this all-in player or are all-in themselves.
        /// </summary>
        /// <param name="playerIndex">The index of the player to be checked</param>
        private void CheckForAndProcessAllInPlayer(int playerIndex)
        {
            // Loop through all the players
            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    if (seat[i].HoldCardsCurr[0] != CardTranslator.NoCardPresent)
                    {
                        // The player is still in the hand
                        string newPlayerName = screenShotProcessor[i].GetPlayerName(false);

                        if (newPlayerName.ToUpper() == "ALL-IN")
                        {
                            // First check to make sure the chip counts aren't already correct. This could happen if a player has gone
                            // all-in and only one other player is left in the hand and that other player has more than enough chips to
                            // call but clicks "all-in" instead of call. This is effectively a call because the player that has already  
                            // gone all-in has no means to call this larger bet.
                            int sumOfChipsInPot = 0;

                            for (int j = 0; j < 10; j++)
                            {
                                if (j != 9 || !is9PlayerTable)
                                {
                                    sumOfChipsInPot += seat[j].ChipsInPot;
                                }
                            }

                            if (sumOfChipsInPot != potCurr)
                            {
                                // This is the case where a player has gone all-in and the remaining players' hold cards are shown.
                                // Note: The issue here (the reason for this section of code) is when this happen's the remaining players'
                                // chances of winning are shown over their chip count so the players' chip counts cannot be read. Therefore if  
                                // newPlayerName == "All-In" update that player's chip count accordingly. 
                                seat[i].ChipStackCurr = 0;

                                if (playerIndex > i)
                                {
                                    seat[i].SkipInfoCheck = true;
                                }

                                // Also check the chip counts of the remaining players to make sure they have either put in as many chips as the 
                                // all-in player or they have gone all-in themselves. This is for the case where a player bets more chips than 
                                // the other player has and that other player instantly goes all in before the first player's bet could be 
                                // registered. 
                                for (int j = 0; j < 10; j++)
                                {
                                    if (j != 9 || !is9PlayerTable)
                                    {
                                        if (seat[j].HoldCardsCurr[0] != CardTranslator.NoCardPresent)
                                        {
                                            // The player is still in the hand
                                            if (seat[j].ChipsInPot < seat[i].ChipsInPot + seat[i].ChipStackPrev)
                                            {
                                                // The player has less chips in the pot than the all-in player
                                                if (seat[j].ChipStackCurr > 0)
                                                {
                                                    // The player has some chips left in his chipstack
                                                    if (seat[j].ChipsInPot + seat[j].ChipStackCurr > seat[i].ChipsInPot + seat[i].ChipStackPrev)
                                                    {
                                                        // The player has more than enough chips to cover the bet

                                                        // My thinking here is the current player's chipstack is equal to what he had previously minus 
                                                        // the everything that the all-in player put in minus what the current player had already put
                                                        // in the pot.
                                                        seat[j].ChipStackCurr = seat[j].ChipStackPrev -
                                                            (seat[i].ChipsInPot + seat[i].ChipStackPrev - seat[i].ChipsInPot);

                                                        if (playerIndex > j)
                                                        {
                                                            seat[j].SkipInfoCheck = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // The player's chip stack is less than or equal to the amount needed to cover the bet
                                                        seat[j].ChipStackCurr = 0;

                                                        if (playerIndex > j)
                                                        {
                                                            seat[j].SkipInfoCheck = true;
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

        /// <summary>
        /// Display this player's hold cards on the UI in the myHandPic picturebox.
        /// </summary>
        /// <param name="playerIndex_">The index of the player whose hold cards are to be displayed</param>
        private void DisplayPlayerOfInterestHandUi(int playerIndex_)
        {
            Bitmap playerHandbmp = screenShotProcessor[playerIndex_].ShareScreenShot();
            // Crop the bitmap so that only the two hold cards are left
            playerOfInterestHandBmp = new Bitmap(playerHandbmp.Clone(new Rectangle(54, 0, 155, playerHandbmp.Height), playerHandbmp.PixelFormat));
            myHandPic.Size = new Size(playerOfInterestHandBmp.Width, playerOfInterestHandBmp.Height);
            myHandPic.Image = playerOfInterestHandBmp;
            myHandPic.BringToFront();
        }

        /// <summary>
        /// Update the player's chips stack value.
        /// </summary>
        /// <param name="playerIndex">The index of the player whose chip stack is to be updated</param>
        /// <returns>The string value of the player's updated chip stack</returns>
        private string UpdatePlayerChipStack(int playerIndex)
        {
            string updatedPlayerChipStackStr;

            if (seat[playerIndex].ChipStackCurr != ProcessScreenShots.ChipStackValueCoverdByBanner)
            {
                updatedPlayerChipStackStr = seat[playerIndex].ChipStackCurr.ToString();
            }
            else
            {
                if (potCurr > potPrev && actionPlayerIndexPrev == playerIndex)
                {
                    if (seat[playerIndex].ChipStackPrev > 0)
                    {
                        // This player's chips stack is covered by a banner and cannot be read but they are calling an all-in bet
                        correctedAllInBet = AllInBetCalled(playerIndex);
                        allInChipStacksCorrected = true;
                    }
                    updatedPlayerChipStackStr = seat[playerIndex].ChipStackPrev.ToString();
                }
                else
                {
                    // Pot hasn't change so the player's chipstack hasn't changed
                    seat[playerIndex].ChipStackCurr = seat[playerIndex].ChipStackPrev;
                    updatedPlayerChipStackStr = seat[playerIndex].ChipStackCurr.ToString();
                }
            }

            return updatedPlayerChipStackStr;
        }

        /// <summary>
        /// An all-in bet has been called. Verify that we have the correct chip counts for the players.
        /// </summary>
        /// <param name="playerIndex_">The index of the player whose chip stack is to be updated</param>
        /// <returns>The amount the player being examined added to the pot</returns>
        private int AllInBetCalled(int playerIndex_)
        {
            // The player chip counts could be off in an all-in situation because in all-in situaitons banners pop up and cover the players' chip 
            // counts. In the case where a player with more chips goes all-in and another player or players call his bet chip counts will be off 
            // because the player with more chips will have all his chips in the pot and the player(s) that called will not have the same number
            // of chips in the pot.

            // Verify chip counts
            int plr1stMostChipsInPot = -1;
            int plr2ndMostChipsInPot = -1;
            int _1stMostChipsInPot = -1;
            int _2ndMostChipsInPot = -1;

            // Find the two players with the most chips in the pot (find both the player indices and chips in pot values)
            for (int i = 0; i < 10; i++)
            {
                if (i != 9 || !is9PlayerTable)
                {
                    // Check if player is still in the game (all remaining players have both of their hold cards shown at this point)
                    if (seat[i].HoldCardsCurr[0] > 0)
                    {
                        if (seat[i].ChipsInPot > _1stMostChipsInPot)
                        {
                            plr2ndMostChipsInPot = plr1stMostChipsInPot;
                            _2ndMostChipsInPot = _1stMostChipsInPot;

                            plr1stMostChipsInPot = i;
                            _1stMostChipsInPot = seat[i].ChipsInPot;
                        }
                        else if (seat[i].ChipsInPot > _2ndMostChipsInPot)
                        {
                            plr2ndMostChipsInPot = i;
                            _2ndMostChipsInPot = seat[i].ChipsInPot;
                        }
                    }
                }
            }

            int plr1PrevChips = seat[plr1stMostChipsInPot].ChipStackPrev;
            int plr2PrevChips = seat[plr2ndMostChipsInPot].ChipStackPrev;

            // Correct the chip counts
            if (_1stMostChipsInPot != _2ndMostChipsInPot)
            {
                if (seat[plr2ndMostChipsInPot].ChipStackPrev > 0)
                {
                    if (plr2PrevChips + _2ndMostChipsInPot >= _1stMostChipsInPot)
                    {
                        // The player with the second most chips in the pot is calling and has enough chips to cover the bet
                        seat[plr2ndMostChipsInPot].ChipStackPrev -= (_1stMostChipsInPot - _2ndMostChipsInPot);
                        seat[plr2ndMostChipsInPot].ChipsInPot = _1stMostChipsInPot;
                    }
                    else
                    {
                        // The player with the second most chips in the pot is calling but doesn't have enough chips to cover the bet
                        seat[plr2ndMostChipsInPot].ChipsInPot += seat[plr2ndMostChipsInPot].ChipStackPrev;
                        seat[plr2ndMostChipsInPot].ChipStackPrev = 0;

                        seat[plr1stMostChipsInPot].ChipStackPrev += seat[plr1stMostChipsInPot].ChipsInPot - seat[plr2ndMostChipsInPot].ChipsInPot;
                        seat[plr1stMostChipsInPot].ChipsInPot = seat[plr2ndMostChipsInPot].ChipsInPot;
                    }
                }
            }

            // Find bet amount
            int betAmount = 0;
            if (playerIndex_ == plr1stMostChipsInPot)
            {
                betAmount = plr1PrevChips - seat[plr1stMostChipsInPot].ChipStackPrev;
            }
            else if (playerIndex_ == plr2ndMostChipsInPot)
            {
                betAmount = plr2PrevChips - seat[plr2ndMostChipsInPot].ChipStackPrev;
            }

            return betAmount;
        }

        /// <summary>
        /// Update the UI to reflect player chip actions (any change in the player's chip count).
        /// </summary>
        /// <param name="playerIndex">The index of the player whose chip stack is to be updated</param>
        /// <param name="playerChipStackStr">The string representation of the player's updated chipstack</param>
        private void UpdatePlayerChipActionUi(int playerIndex, string playerChipStackStr)
        {
            if (seat[playerIndex].ChipStackCurr != seat[playerIndex].ChipStackPrev)
            {
                playersDgv.Rows[playerIndex].Cells[3].Style = new DataGridViewCellStyle { ForeColor = dataChangeClr };

                if (playerChipStackStr != ProcessScreenShots.PlayerChipStackParseError.ToString())
                {
                    playersDgv.Rows[playerIndex].Cells[3].Value = playerChipStackStr;
                }
                else
                {
                    playersDgv.Rows[playerIndex].Cells[3].Value = "???";
                }

                if (seat[playerIndex].ChipStackPrev != -1)
                {
                    if (!isNewDealer)
                    {
                        if (seat[playerIndex].ChipStackCurr < seat[playerIndex].ChipStackPrev)
                        {
                            AppendColoredRtbText(gameActions, dataChangeClr, seat[playerIndex].Name + ":");
                            if (seat[playerIndex].ChipStackCurr != ProcessScreenShots.ChipStackValueCoverdByBanner)
                            {
                                seat[playerIndex].ChipsInPot += seat[playerIndex].ChipStackPrev - seat[playerIndex].ChipStackCurr;
                                gameActions.AppendText(" bet " + (seat[playerIndex].ChipStackPrev - seat[playerIndex].ChipStackCurr).ToString() +
                                    " - " + seat[playerIndex].ChipStackCurr.ToString() + "\n");
                                gameActions.ScrollToCaret();

                                // DB stuff (update player chip action)
                                db_.InsertPlayerAction(seat[playerIndex].DbHandPlayerId,
                                    (seat[playerIndex].ChipStackPrev - seat[playerIndex].ChipStackCurr), dbHandActionNum, errorMessagesTxt);
                                dbHandActionNum++;
                            }
                            else
                            {
                                // Player chip stack covered by banner (cannot be read)
                                if (!allInChipStacksCorrected)
                                {
                                    seat[playerIndex].ChipsInPot = seat[playerIndex].ChipsInPot + (potCurr - potPrev);
                                    gameActions.AppendText(" bet " + (potCurr - potPrev).ToString() +
                                        " - " + playerChipStackStr + "\n");
                                    seat[playerIndex].ChipStackPrev -= (potCurr - potPrev);
                                    gameActions.ScrollToCaret();

                                    // DB stuff (update player chip action)
                                    db_.InsertPlayerAction(seat[playerIndex].DbHandPlayerId, (potCurr - potPrev), dbHandActionNum,
                                        errorMessagesTxt);
                                }
                                else
                                {
                                    gameActions.AppendText(" bet " + (correctedAllInBet).ToString() +
                                        " - " + playerChipStackStr + "\n");
                                    gameActions.ScrollToCaret();

                                    // DB stuff (update player chip action)
                                    db_.InsertPlayerAction(seat[playerIndex].DbHandPlayerId, (correctedAllInBet), dbHandActionNum,
                                        errorMessagesTxt);

                                    allInChipStacksCorrected = false;
                                }
                                dbHandActionNum++;
                            }
                        }
                        else
                        {
                            seat[playerIndex].ChipsInPot += seat[playerIndex].ChipStackPrev - seat[playerIndex].ChipStackCurr;
                        }
                    }
                }

                seat[playerIndex].ChipStackChangePrev = true;
            }
            else
            {
                // Chip stack has not changed since the last screenshot
                playersDgv.Rows[playerIndex].Cells[3].Style = new DataGridViewCellStyle { ForeColor = normalTextClr };
                if (playerChipStackStr != ProcessScreenShots.PlayerChipStackParseError.ToString())
                {
                    playersDgv.Rows[playerIndex].Cells[3].Value = playerChipStackStr;
                }
                else
                {
                    playersDgv.Rows[playerIndex].Cells[3].Value = "???";
                }
            }
        }

        /// <summary>
        /// Have any of the board cards changed? (new cards on board)
        /// </summary>
        /// <returns>True if one of more of the board cards have changed</returns>
        private bool BoardChange()
        {
            bool isChange;

            if (boardCard[0].CardCurrInt != boardCard[0].CardPrevInt || boardCard[1].CardCurrInt != boardCard[1].CardPrevInt ||
                boardCard[2].CardCurrInt != boardCard[2].CardPrevInt || boardCard[3].CardCurrInt != boardCard[3].CardPrevInt ||
                boardCard[4].CardCurrInt != boardCard[4].CardPrevInt)
            {
                isChange = true;
            }
            else
            {
                isChange = false;
            }

            return isChange;
        }

        /// <summary>
        /// Update the boardStatus rich text box with the current board card values.
        /// </summary>
        private void UpdateBoardStatusUi()
        {
            boardStatus.Clear();

            isBoardCardChangePrev = false;

            // Flop #1 
            UpdateBoardCardUi(0, "F: ", "Flop : ", "       \t", " - ", "Flop card #1 read incorrectly!");

            // Flop #2
            UpdateBoardCardUi(1, "F: ", "", "       \t", " - ", "Flop card #2 read incorrectly!");

            // Flop #3
            UpdateBoardCardUi(2, "F: ", "", "\n\t", "\n", "Flop card #3 read incorrectly!");

            // Turn
            UpdateBoardCardUi(3, "T: ", "Turn : ", "       \t", "\n", "Turn card read incorrectly!");

            // River
            UpdateBoardCardUi(4, "R: ", "River : ", "", "\n", "River card read incorrectly!");
        }

        /// <summary>
        /// Update the UI with a single board card.
        /// </summary>
        /// <param name="boardCardIndex">The index of the board card</param>
        /// <param name="boardCardIdentifierStr1">The abbreviated string identifier for the board card</param>
        /// <param name="boardCardIdentifierStr2">The string identifier for the board card</param>
        /// <param name="spacerStr1">The 1st spacer string for spacing in the boardStatus rich text box</param>
        /// <param name="spacerStr2">The 2nd spacer string for spacing in the boardStatus rich text box</param>
        /// <param name="cardReadErrorStr">The card read error statement for the board card</param>
        private void UpdateBoardCardUi(int boardCardIndex, string boardCardIdentifierStr1, string boardCardIdentifierStr2, string spacerStr1,
            string spacerStr2, string cardReadErrorStr)
        {
            AppendColoredRtbText(boardStatus, dataIdClr, boardCardIdentifierStr1);
            if (boardCard[boardCardIndex].CardCurrInt != boardCard[boardCardIndex].CardPrevInt)
            {
                AppendColoredRtbText(boardStatus, dataChangeClr, CardTranslator.CardIntToString(boardCard[boardCardIndex].CardCurrInt) + spacerStr1);
                isBoardCardChangePrev = true;

                if (boardCard[boardCardIndex].CardCurrInt != CardTranslator.NoCardPresent)
                {
                    if (boardCardIdentifierStr2.ToUpper() != "")
                    {
                        AppendColoredRtbText(gameActions, dataChangeClr, boardCardIdentifierStr2);
                    }
                    gameActions.AppendText(CardTranslator.CardIntToString(boardCard[boardCardIndex].CardCurrInt) + spacerStr2);
                    gameActions.ScrollToCaret();

                    // DB stuff (add this board card action to the database)
                    boardCard[boardCardIndex].PrevBoardActionId = db_.InsertBoardAction(dbHandId, (short)boardCard[boardCardIndex].CardCurrInt,
                        dbHandActionNum, errorMessagesTxt);
                    dbHandActionNum++;
                }
                else
                {
                    // This board card was read incorrectly and the information must be corrected
                    AppendColoredRtbText(gameActions, dataChangeClr, cardReadErrorStr);
                    db_.DeleteBoardAction(boardCard[boardCardIndex].PrevBoardActionId);
                    dbHandActionNum--;
                }
            }
            else
            {
                boardStatus.AppendText(CardTranslator.CardIntToString(boardCard[boardCardIndex].CardCurrInt) + spacerStr1);
            }
        }

        /// <summary>
        /// Gather the data needed to update the DataDisplayForm and call the method to update the DataDisplayForm.
        /// </summary>
        private void UpdateDataDisplayForm()
        {
            bool[] isOpenSeat = new bool[10];
            bool[] isPlrFolded = new bool[10];
            int[] plrDbId = new int[10];
            string[] plrName = new string[10];

            // Copy the relevant data from the plr object and put it into arrays to pass to the DataDisplayForm
            for (int j = 0; j < 10; j++)
            {
                if (seat[j].HoldCardsCurr[0] == CardTranslator.NoCardPresent)
                {
                    isPlrFolded[j] = true;
                }
                else
                {
                    isPlrFolded[j] = false;
                }

                isOpenSeat[j] = seat[j].OpenSeatCurr;
                plrDbId[j] = seat[j].DbPlayerId;
                plrName[j] = seat[j].Name;
            }

            BettingRound.Round bettingRound;

            // Determine the board status (0 = pre-flop, 1 = flop, 2 = turn, 3 = river)
            if (boardCard[0].CardCurrInt == CardTranslator.NoCardPresent)
            {
                bettingRound = BettingRound.Round.PreFlop;
            }
            else if (boardCard[3].CardCurrInt == CardTranslator.NoCardPresent)
            {
                bettingRound = BettingRound.Round.Flop;
            }
            else if (boardCard[4].CardCurrInt == CardTranslator.NoCardPresent)
            {
                bettingRound = BettingRound.Round.Turn;
            }
            else
            {
                bettingRound = BettingRound.Round.River;
            }

            // Update DataDisplayForm
            dataDisplayForm.UpdateData(isOpenSeat, isPlrFolded, plrDbId, plrName, bettingRound, dbGameId);
        }

        /// <summary>
        /// Stop the process timers for the UpdateAll method, update the UI with the timer values and reset the timers to prepare for the next update.
        /// </summary>
        private void UpdateTimersUpdateAllComplete()
        {
            uiUpdateStp.Stop();
            overallTimeStp.Stop();

            timersDgv.Rows[8].Cells[15].Value = string.Format("{0:0.000}", uiUpdateStp.ElapsedMilliseconds / 1000.0);
            totalProcessingTimeRtb.Clear();
            AppendColoredRtbText(totalProcessingTimeRtb, dataIdClr, "Total Processing Time: ");
            totalProcessingTimeRtb.AppendText(string.Format("{0:0.000}", overallTimeStp.ElapsedMilliseconds / 1000.0));

            jobCompletionCounter = 1;
            jobStartedCounter = 1;

            overallTimeStp.Reset();
            uiUpdateStp.Reset();
        }

        /// <summary>
        /// Get the player's name (string).
        /// </summary>
        /// <param name="playerIndex_">The player's index</param>
        /// <returns></returns>
        private string GetPlayerName(int playerIndex_)
        {
            string playerName_;

            if (seat[playerIndex_].Name != "")
            {
                playerName_ = seat[playerIndex_].Name;
            }
            else
            {
                playerName_ = "Player " + (playerIndex_ + 1).ToString();
            }
            return playerName_;
        }

        /// <summary>
        /// Draw a rectangle inside the MainForm window.
        /// </summary>
        /// <param name="penColor">Stroke color for the rectangle</param>
        /// <param name="rectX_">X coordinate of the left side of the rectangle</param>
        /// <param name="rectY_">Y coordinate of the top of the rectangle</param>
        /// <param name="rectWidth_">Width of the rectangle</param>
        /// <param name="rectHeight_">Height of the rectangle</param>
        /// <param name="clearPreviousRects">True to clear all previously drawn rectangles from the screen</param>
        private void DrawRect(Pen penColor, int rectX_, int rectY_, int rectWidth_, int rectHeight_, bool clearPreviousRects)
        {
            if (clearPreviousRects)
            {
                screenShotBmp = (Bitmap)Image.FromFile(screenShotFilePath);
                screenShotGfx = Graphics.FromImage(screenShotBmp);
            }

            screenShotGfx.DrawRectangle(penColor, rectX_ - 1, rectY_ - 1, rectWidth_ + 2, rectHeight_ + 2);
            screenShotPic.Image = screenShotBmp;
        }

        /// <summary>
        /// Append colored text to the current text of a rich text box.
        /// </summary>
        /// <param name="rtb">The rich text box to append colored text to</param>
        /// <param name="clr">The desired text color</param>
        /// <param name="str">The desired text</param>
        public void AppendColoredRtbText(RichTextBox rtb, Color clr, string str)
        {
            Font currentFont = rtb.SelectionFont;

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            // Create a new font (same as the old with a different color)
            rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold);
            rtb.SelectionColor = clr;

            rtb.AppendText(str);

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            // Set rich text box font back to the original font (and color)
            rtb.SelectionFont = currentFont;
            rtb.SelectionColor = rtb.ForeColor;
        }

        /// <summary>
        /// Append colored, strikethrough text to the current text of a rich text box.
        /// </summary>
        /// <param name="rtb">The rich text box to append colored, strikethrough text to</param>
        /// <param name="clr">The desired text color</param>
        /// <param name="str">The desired text</param>
        public void AppendColoredStrikeThroughRtbText(RichTextBox rtb, Color clr, string str)
        {
            Font currentFont = rtb.SelectionFont;

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            // Create a new font (same as the old with a different color and strikeout applied)
            rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold | FontStyle.Strikeout);
            rtb.SelectionColor = clr;

            rtb.AppendText(str);

            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;

            // Set rich text box font back to the original font (and color)
            rtb.SelectionFont = currentFont;
            rtb.SelectionColor = rtb.ForeColor;
        }

        /// <summary>
        /// Set colored chip count text in a rich text box with the option to right justify the text.
        /// </summary>
        /// <param name="rtb">The rich text box to add the chipcount text to</param>
        /// <param name="clr">The desired text color</param>
        /// <param name="str">The desired text</param>
        /// <param name="isRightJustify">True to right justify the text</param>
        public void SetColoredChipCountRtbText(RichTextBox rtb, Color clr, string str, bool isRightJustify)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = clr;

            if (str == "-1")
            {
                str = "-";
            }

            if (isRightJustify)
            {
                while (str.Length < 10)
                {
                    str = " " + str;
                }
                rtb.SelectionBackColor = Color.FromArgb(90, 90, 90);
            }

            rtb.AppendText(str);

            // Set rich text box font back to the original font (and color)
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = rtb.ForeColor;
        }

        /// <summary>
        /// Find and display the data about the mouse click location.
        /// </summary>
        /// <param name="clickLocation">The location of the mouse click</param>
        private void DisplayClickLocationData(Point clickLocation)
        {
            // Get X and Y coordinates of the click location
            int xLocation = clickLocation.X;
            int yLocation = clickLocation.Y;

            mouseClickX_Lbl.Text = mouseClickLocationX_Str + xLocation.ToString();
            mouseClickY_Lbl.Text = mouseClickLocationY_Str + yLocation.ToString();

            if (screenShotBmp != null)
            {
                // Get color
                Color pixelClr = screenShotBmp.GetPixel(xLocation, yLocation);
                int rComponent = pixelClr.R;
                int gComponent = pixelClr.G;
                int bComponent = pixelClr.B;
                float brightness = pixelClr.GetBrightness();

                mouseClickColorLbl.Text = mouseClickLocationColorStr + "(" + rComponent.ToString() + ", " + gComponent.ToString() + ", " +
                    bComponent.ToString() + ")";
                mouseClickBrightnessLbl.Text = mouseClickLocationBrightnessStr + brightness.ToString("f4");
            }
            else
            {
                mouseClickColorLbl.Text = mouseClickLocationColorStr + "N/A";
                mouseClickBrightnessLbl.Text = mouseClickLocationBrightnessStr + "N/A";
            }
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Create the tasks that will be run asynchronously by the ProcessScreenShots objects.
        /// </summary>
        /// <param name="createAllTasks">True to create all tasks, false to only create the tasks needed in the case of a secondary new hand event</param>
        private void CreateTasks(bool createAllTasks)
        {
            CreatePlayerInfoTasks(createAllTasks);
            CreateBoardCardInfoTasks();

            // Create these two tasks under normal conditions. Don't create these two tasks if tasks are being re-run due to a secondary new hand event
            if (createAllTasks)
            {
                CreatePotInfoTask();
                CreateDealerAndActionPlayerInfoTask();
            }
        }

        /// <summary>
        /// Create the tasks that retrieve information about the players (player names, chip stacks, hold cards etc.) [Workers 0 - 9].
        /// </summary>
        /// <param name="createAllTasks_">True to create all tasks, false to only create the tasks needed in the case of a secondary new hand event</param>
        private void CreatePlayerInfoTasks(bool createAllTasks_)
        {
            for (int i = 0; i < 10; i++)
            {
                int j = i;

                // Do not create task 9 if it's a 9 player table
                if (j != 9 || !is9PlayerTable || workerTasks[9] == null)
                {
                    workerTasks[j] = new Task(() =>
                    {
                        // Do not add the code for task 9 if it will not actually be run
                        if (j != 9 || !is9PlayerTable)
                        {
                            long threadCreationTime = 0;

                            if (isRunTimers)
                            {
                                StartRunTimersPlayerTask(j, ref threadCreationTime);
                            }

                            // Only run the below code if we don't have either of the following conditions
                            // 1) A condition where the dealer chip is temporarily covered or doesn't exist yet.
                            // 2) A condition where there is a new dealer but the players don't have cards yet (if the players haven't recieved their
                            //    cards yet then the antes and blinds haven't been posted yet).
                            // The code below is the processing of the data (bitmap processing).
                            if (!dealerChipNotFound && !dontUpdateData)
                            {
                                if (dealerIndexCurr != -1)
                                {
                                    // Only find player info if a dealer exists. There is always a dealer during gameplay.
                                    // Not finding player info when there is no dealer will avoid errors before game starts and after
                                    // game is over while still allowing the whole game to be captured.
                                    if (!screenShotProcessor[j].IsOpenSeat())
                                    {
                                        HoldCardState.PairOfHoldCards hcPairState = GetPlayerHoldCardsTask(j, createAllTasks_);

                                        // Secondary new hand check
                                        if (!isNewDealer && !isNewDealerPrev && hcPairState == HoldCardState.PairOfHoldCards.BothCardsFaceDown &&
                                        (seat[j].HoldCardsPrev[0] != 0 || seat[j].HoldCardsPrev[1] != 0))
                                        {
                                            // We have a secondary new hand event
                                            isNewDealer = true;
                                            reRunTasks = true;
                                        }

                                        if (!reRunTasks)
                                        {
                                            // Only do this if this task is not going to be re-run because of the secondary check for new hand flag
                                            if (!seat[j].SkipInfoCheck)
                                            {
                                                GetPlayerChipStackTask(j);
                                                GetPlayerNameTask(j);
                                            }
                                            else
                                            {
                                                seat[j].SkipInfoCheck = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        OpenSeatTask(j);
                                    }

                                    UpdatePlayerDgvTask(j);
                                }
                            }

                            if (saveBitmaps)
                            {
                                screenShotProcessor[j].SaveBitmap();
                            }

                            if (isRunTimers)
                            {
                                StopRunTimersPlayerTask(j, threadCreationTime);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Start the run timers for the player info task and get the thread creation time.
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StartRunTimersPlayerTask(int playerIndex, ref long threadCreationTime_)
        {
            workersStp[playerIndex].Stop();
            threadCreationTime_ = workersStp[playerIndex].ElapsedMilliseconds;
            workersStp[playerIndex].Reset();
            workersStp[playerIndex].Start();
        }

        /// <summary>
        /// Find the values and states of the player's hold cards.
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        /// <param name="createAllTasks__">False if this is a secondary new hand event, true otherwise</param>
        /// <returns>The state of the player's pair of hold cards</returns>
        private HoldCardState.PairOfHoldCards GetPlayerHoldCardsTask(int playerIndex, bool createAllTasks__)
        {
            if (createAllTasks__)
            {
                // Update player hold cards. If this is a secondary new hand don't do this because the hold card values will all
                // be the same because we are evaluating the same screenshot.
                seat[playerIndex].HoldCardsPrev[0] = seat[playerIndex].HoldCardsCurr[0];
                seat[playerIndex].HoldCardsPrev[1] = seat[playerIndex].HoldCardsCurr[1];
            }

            // Always check the hold card status (as long as it's not an open seat on a continuing hand)
            // Use this as a secondary check for a new hand --> If a player didn't have cards before (folded)
            // but has cards now without the dealer changing we have a new hand.
            // This happens when the player who was the first to act player (player to the left of the dealer) 
            // gets eliminated. This is done so the next first to act player doesn't skip his blinds.

            HoldCardState.PairOfHoldCards hcPairState_ = screenShotProcessor[playerIndex].HcState();

            if (hcPairState_ == HoldCardState.PairOfHoldCards.BothCardsFaceDown)
            {
                seat[playerIndex].HoldCardsCurr[0] = CardTranslator.CardFaceDown;
                seat[playerIndex].HoldCardsCurr[1] = CardTranslator.CardFaceDown;
            }
            else if (hcPairState_ == HoldCardState.PairOfHoldCards.BothCardsShown)
            {
                // only find the hold card if this is a new hold card
                if (seat[playerIndex].HoldCardsCurr[0] < CardTranslator.LowestFaceUpCardValue || isNewDealer)
                {
                    seat[playerIndex].HoldCardsCurr[0] = screenShotProcessor[playerIndex].FindHc(true, saveBitmaps);
                }

                // only find the hold card if this is a new hold card
                if (seat[playerIndex].HoldCardsCurr[1] < CardTranslator.LowestFaceUpCardValue || isNewDealer)
                {
                    seat[playerIndex].HoldCardsCurr[1] = screenShotProcessor[playerIndex].FindHc(false, saveBitmaps);
                }
            }
            else if (hcPairState_ == HoldCardState.PairOfHoldCards.OnlyCard1Shown)
            {
                // only find the hold card if this is a new hold card
                if (seat[playerIndex].HoldCardsCurr[0] < CardTranslator.LowestFaceUpCardValue)
                {
                    seat[playerIndex].HoldCardsCurr[0] = screenShotProcessor[playerIndex].FindHc(true, saveBitmaps);
                }

                seat[playerIndex].HoldCardsCurr[1] = CardTranslator.CardFaceDown;
            }
            else if (hcPairState_ == HoldCardState.PairOfHoldCards.OnlyCard2Shown)
            {
                seat[playerIndex].HoldCardsCurr[0] = CardTranslator.CardFaceDown;

                // only find the hold card if this is a new hold card
                if (seat[playerIndex].HoldCardsCurr[1] < CardTranslator.LowestFaceUpCardValue)
                {
                    seat[playerIndex].HoldCardsCurr[1] = screenShotProcessor[playerIndex].FindHc(false, saveBitmaps);
                }
            }
            else
            {
                seat[playerIndex].HoldCardsCurr[0] = CardTranslator.NoCardPresent;
                seat[playerIndex].HoldCardsCurr[1] = CardTranslator.NoCardPresent;
            }

            return hcPairState_;
        }

        /// <summary>
        /// Get the player's chip stack.
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        private void GetPlayerChipStackTask(int playerIndex)
        {
            // Update intPlayerChipStacks array (save current value to the old value if criteria is met)
            if (seat[playerIndex].ChipStackCurr != ProcessScreenShots.ChipStackValueCoverdByBanner)
            {
                seat[playerIndex].ChipStackPrev = seat[playerIndex].ChipStackCurr;
            }

            // Get new player chip stack value if new hand or player has not yet folded
            if (isNewDealer || isNewDealerPrev || seat[playerIndex].HoldCardsCurr[0] != CardTranslator.NoCardPresent)
            {
                // HERE HERE HERE!!! print both values when these are not equal??? (also print index)
                seat[playerIndex].ChipStackCurr = screenShotProcessor[playerIndex].GetChipStack(saveBitmaps);
            }
        }

        /// <summary>
        /// Get the player's name.
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        private void GetPlayerNameTask(int playerIndex)
        {
            if (isNewDealer || seat[playerIndex].NameTemp != seat[playerIndex].Name || seat[playerIndex].Name == "")
            {
                // If new hand or previous player name does not match current player name get player name
                string newPlayerName = screenShotProcessor[playerIndex].GetPlayerName(saveBitmaps);

                // If newPlayerName = to the player's name nothing needs to be done
                if (newPlayerName != seat[playerIndex].Name)
                {
                    // Make sure newPlayerName isn't a tempory action status before changing anything
                    if (newPlayerName.ToUpper() != "ALL-IN" && newPlayerName.ToUpper() != "BET" &&
                        newPlayerName.ToUpper() != "CALL" && newPlayerName.ToUpper() != "CHECK" &&
                        newPlayerName.ToUpper() != "RAISE" && newPlayerName.ToUpper() != "FOLD" &&
                        newPlayerName.ToUpper() != "TIME" && newPlayerName != "")
                    {
                        // If the same new player's name is shown for two screen captures in a row or if this is the first time
                        // checking for the players name(\f) change it
                        if (newPlayerName == seat[playerIndex].NameTemp)
                        {
                            // If this is more than a character case change we have a new player, if not don't make a fuss
                            if (newPlayerName.ToUpper() != seat[playerIndex].Name.ToUpper())
                            {
                                // This variable is used to signal to use a different color for the player's name to highlight
                                // that this is a new player
                                seat[playerIndex].PlayerChange = true;
                                // This variable is used so we can announce when there's a new player
                                seat[playerIndex].NewPlayer = true;
                            }

                            seat[playerIndex].Name = newPlayerName;
                        }
                        else
                        {
                            // If not just save it to NameTemp
                            seat[playerIndex].NameTemp = newPlayerName;

                            if (seat[playerIndex].Name == "\f")
                            {
                                seat[playerIndex].Name = "";
                            }
                        }
                    }
                    else
                    {
                        seat[playerIndex].NameTemp = "\f";

                        if (seat[playerIndex].Name == "\f")
                        {
                            seat[playerIndex].Name = "";
                        }

                        if (newPlayerName.ToUpper() == "FOLD")
                        {
                            if (isNewDealer && !seat[playerIndex].FirstActionFastFold)
                            {
                                seat[playerIndex].HoldCardsPrev[0] = CardTranslator.CardFaceDown;
                                seat[playerIndex].HoldCardsPrev[1] = CardTranslator.CardFaceDown;

                                seat[playerIndex].FirstActionFastFold = true;
                            }
                        }
                    }
                }
            }

            // If this point has been reached it is not an Open Seat
            // Update OpenSeatPrev status (save current value to old value)
            seat[playerIndex].OpenSeatPrev = seat[playerIndex].OpenSeatCurr;
            seat[playerIndex].OpenSeatCurr = false;
        }

        /// <summary>
        /// Set this seat to an open seat.
        /// </summary>
        /// <param name="seatIndex">The index of the seat</param>
        private void OpenSeatTask(int seatIndex)
        {
            seat[seatIndex].Name = "Open Seat";
            seat[seatIndex].ChipStackCurr = 0;
            seat[seatIndex].OpenSeatPrev = seat[seatIndex].OpenSeatCurr;
            seat[seatIndex].OpenSeatCurr = true;

            seat[seatIndex].HoldCardsCurr[0] = CardTranslator.NoCardPresent;
            seat[seatIndex].HoldCardsCurr[1] = CardTranslator.NoCardPresent;
            seat[seatIndex].HoldCardsPrev[0] = CardTranslator.NoCardPresent;
            seat[seatIndex].HoldCardsPrev[1] = CardTranslator.NoCardPresent;
        }

        /// <summary>
        /// Update the playersDgv with the new player information (done from within the worker task).
        /// </summary>
        /// <param name="playerIndex">The index of the player</param>
        private void UpdatePlayerDgvTask(int playerIndex)
        {
            seat[playerIndex].OpenSeatChangePrev = false;

            if (seat[playerIndex].OpenSeatCurr)
            {
                if (!seat[playerIndex].OpenSeatPrev)
                {
                    // New open seat
                    playersDgv.Rows[playerIndex].Cells[0].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = dataChangeClr
                    };
                    playersDgv.Rows[playerIndex].Cells[0].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[0]);

                    playersDgv.Rows[playerIndex].Cells[1].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = dataChangeClr
                    };
                    playersDgv.Rows[playerIndex].Cells[1].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[1]);

                    playersDgv.Rows[playerIndex].Cells[2].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = dataChangeClr
                    };
                    playersDgv.Rows[playerIndex].Cells[2].Value = "Open Seat";

                    playersDgv.Rows[playerIndex].Cells[3].Style = new DataGridViewCellStyle { ForeColor = dataChangeClr };
                    playersDgv.Rows[playerIndex].Cells[3].Value = "- - -";

                    seat[playerIndex].OpenSeatChangePrev = true;
                }
                else
                {
                    // Continuing open seat
                    playersDgv.Rows[playerIndex].Cells[2].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = normalTextClr
                    };
                    playersDgv.Rows[playerIndex].Cells[2].Value = seat[playerIndex].Name;

                    playersDgv.Rows[playerIndex].Cells[0].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = normalTextClr
                    };
                    playersDgv.Rows[playerIndex].Cells[0].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[0]);

                    playersDgv.Rows[playerIndex].Cells[1].Style = new DataGridViewCellStyle
                    {
                        Font = new Font(playersDgv.Font.FontFamily, playersDgv.Font.Size, FontStyle.Bold),
                        ForeColor = normalTextClr
                    };
                    playersDgv.Rows[playerIndex].Cells[1].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[1]);

                    playersDgv.Rows[playerIndex].Cells[3].Style = new DataGridViewCellStyle { ForeColor = normalTextClr };
                    playersDgv.Rows[playerIndex].Cells[3].Value = "- - -";
                }
            }
            else
            {
                if (seat[playerIndex].HoldCardsCurr[0] != seat[playerIndex].HoldCardsPrev[0] || 
                    seat[playerIndex].HoldCardsCurr[1] != seat[playerIndex].HoldCardsPrev[1])
                {
                    // Hold card change
                    playersDgv.Rows[playerIndex].Cells[0].Style = new DataGridViewCellStyle { ForeColor = dataChangeClr };
                    playersDgv.Rows[playerIndex].Cells[1].Style = new DataGridViewCellStyle { ForeColor = dataChangeClr };  
                }
                else
                {
                    // Same hold cards
                    playersDgv.Rows[playerIndex].Cells[0].Style = new DataGridViewCellStyle { ForeColor = normalTextClr };
                    playersDgv.Rows[playerIndex].Cells[1].Style = new DataGridViewCellStyle { ForeColor = normalTextClr };
                }

                playersDgv.Rows[playerIndex].Cells[0].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[0]);
                playersDgv.Rows[playerIndex].Cells[1].Value = CardTranslator.CardIntToString(seat[playerIndex].HoldCardsCurr[1]);

                seat[playerIndex].SittingOutCurr = screenShotProcessor[playerIndex].IsSittingOut();

                string playerStr;
                if (seat[playerIndex].SittingOutCurr)
                {
                    if (seat[playerIndex].SittingOutPrev)
                    {
                        // Player continuing to sit out
                        if (seat[playerIndex].Name != "")
                        {
                            playerStr = "(" + seat[playerIndex].Name + ")";
                        }
                        else
                        {
                            playerStr = "(Player " + (playerIndex + 1).ToString() + ")";
                        }
                    }
                    else
                    {
                        // Player newly sitting out
                        playerStr = GetPlayerName(playerIndex);
                        seat[playerIndex].SittingOutPrev = true;
                    }
                }
                else
                {
                    // Player not sitting out
                    playerStr = GetPlayerName(playerIndex);
                    seat[playerIndex].SittingOutPrev = false;
                }

                playersDgv.Rows[playerIndex].Cells[2].Value = playerStr;
            }
        }

        /// <summary>
        /// Stop the player task run timer and update the UI with the thread creation time and thread run time.
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="threadCreationTime_"></param>
        private void StopRunTimersPlayerTask(int playerIndex, long threadCreationTime_)
        {
            workersStp[playerIndex].Stop();

            int dgvRow, dgvCell;

            if (playerIndex < 9)
            {
                dgvRow = playerIndex;
                dgvCell = 9;
            }
            else
            {
                dgvRow = playerIndex - 9;
                dgvCell = 14;
            }

            timersDgv.Rows[dgvRow].Cells[dgvCell].Value = string.Format("{0:0.000}", threadCreationTime_ / 1000.0);
            timersDgv.Rows[dgvRow].Cells[dgvCell + 1].Value = string.Format("{0:0.000}", workersStp[playerIndex].ElapsedMilliseconds / 1000.0);
            timersDgv.Rows[dgvRow].Cells[dgvCell + 3].Value = jobCompletionCounter;
            jobCompletionCounter++;

            workersStp[playerIndex].Reset();
        }

        /// <summary>
        /// Create the tasks that retrieve information about the board cards (board card states and values) [Workers 10 - 14].
        /// </summary>
        private void CreateBoardCardInfoTasks()
        {
            for (int i = 10; i < 15; i++)
            {
                int j = i;

                workerTasks[j] = new Task(() =>
                {
                    long threadCreationTime = 0;

                    if (isRunTimers)
                    {
                        StartRunTimersBoardCardTask(j, ref threadCreationTime);
                    }

                    // Only run the below code if we don't have either of the following conditions
                    // 1) A condition where the dealer chip is temporarily covered.
                    // 2) A condition where there is a new dealer but the players don't have cards yet (if the players haven't recieved their
                    //    cards yet then the antes and blinds haven't been posted yet).
                    // The code below is the processing of the data (bitmap processing).
                    if (!dealerChipNotFound && !dontUpdateData)
                    {
                        // Only find player info if a dealer exists. There is always a dealer during gameplay.
                        // Not finding player info when there is no dealer will avoid errors before game starts and after
                        // game is over while still allowing the whole game to be captured.
                        if (dealerIndexCurr != -1)
                        {
                            GetBoardCardTask(j);
                        }
                    }

                    if (saveBitmaps)
                    {
                        screenShotProcessor[j].SaveBitmap();
                    }

                    if (isRunTimers)
                    {
                        StopRunTimersBoardCardTask(j, threadCreationTime);                        
                    }
                });
            }
        }

        /// <summary>
        /// Start the run timers for the board card info task and get the thread creation time.
        /// </summary>
        /// <param name="boardCardIndex">The index of the board card</param>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StartRunTimersBoardCardTask(int boardCardIndex, ref long threadCreationTime_)
        {
            workersStp[boardCardIndex].Stop();
            threadCreationTime_ = workersStp[boardCardIndex].ElapsedMilliseconds;
            workersStp[boardCardIndex].Reset();
            workersStp[boardCardIndex].Start();
        }

        /// <summary>
        /// Get the value of the board card (what card).
        /// </summary>
        /// <param name="BoardCardIndex">The index of the board card</param>
        private void GetBoardCardTask(int BoardCardIndex)
        {
            // Only search for the board card if:
            // 1) It's a new hand or
            // 2) The board card hasn't already been shown and one subsequent screenshot to verify the board card seen on the first screenshot was 
            // read correctly. 

            // Take an extra screenshot to verify the card was read correctly because I had an instance when a player folded and the animation
            // showing that player’s hold cards being moved to the middle of the table (an animation which I cannot shut off) was interpreted 
            // as a hold card. What I plan to do is read the board card, announce it and save it to the DB, then double check on the next screenshot 
            // and if it was misread erase the previous record from the DB, decrement the dbHandActionNum counter and announce the mistake. This
            // double check functionality is performed inside of UpdateBoardCardUi.
            if (isNewDealer || isNewDealerPrev || boardCard[BoardCardIndex - 10].CardCurrInt == CardTranslator.NoCardPresent || 
                boardCard[BoardCardIndex - 10].CardPrevInt == CardTranslator.NoCardPresent)
            {
                if (!isNewDealer && !isNewDealerPrev)
                {
                    // Update intBoardCards array (save current value to the old value)
                    boardCard[BoardCardIndex - 10].CardPrevInt = boardCard[BoardCardIndex - 10].CardCurrInt;

                    // Get new intBoardCards value
                    boardCard[BoardCardIndex - 10].CardCurrInt = screenShotProcessor[BoardCardIndex].GetBoardCard();

                    if (boardCard[BoardCardIndex - 10].CardPrevInt != CardTranslator.NoCardPresent && 
                        boardCard[BoardCardIndex - 10].CardCurrInt != boardCard[BoardCardIndex - 10].CardPrevInt)
                    {
                        // An error was made in reading the board card
                        boardCard[BoardCardIndex - 10].CardCurrInt = CardTranslator.NoCardPresent;
                    }
                }
                else
                {
                    boardCard[BoardCardIndex - 10].CardCurrInt = CardTranslator.NoCardPresent;
                    boardCard[BoardCardIndex - 10].CardPrevInt = CardTranslator.NoCardPresent;
                }
            }
        }

        /// <summary>
        /// Stop the board card info task run timer and update the UI with the thread creation time and thread run time.
        /// </summary>
        /// <param name="playerIndex">The index of the board card</param>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StopRunTimersBoardCardTask(int BoardCardIndex, long threadCreationTime_)
        {
            workersStp[BoardCardIndex].Stop();
            timersDgv.Rows[BoardCardIndex - 9].Cells[14].Value = string.Format("{0:0.000}", threadCreationTime_ / 1000.0);
            timersDgv.Rows[BoardCardIndex - 9].Cells[15].Value = string.Format("{0:0.000}", workersStp[BoardCardIndex].ElapsedMilliseconds / 1000.0);
            timersDgv.Rows[BoardCardIndex - 9].Cells[17].Value = jobCompletionCounter;
            jobCompletionCounter++;

            workersStp[BoardCardIndex].Reset();
        }

        /// <summary>
        /// Create the task that finds the value of the pot (number of chips in the pot) [Worker 15].
        /// </summary>
        private void CreatePotInfoTask()
        {
            workerTasks[15] = new Task(() =>
            {
                long threadCreationTime = 0;

                if (isRunTimers)
                {
                    StartRunTimersPotInfoTask(ref threadCreationTime);
                }

                isPotSizeChange = GetPotTask();

                if (saveBitmaps)
                {
                    screenShotProcessor[15].SaveBitmap();
                }

                if (isRunTimers)
                {
                    StopRunTimersPotInfoTask(threadCreationTime);
                }
            });
        }

        /// <summary>
        /// Start the run timers for the pot info task and get the thread creation time.
        /// </summary>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StartRunTimersPotInfoTask(ref long threadCreationTime_)
        {
            workersStp[15].Stop();
            threadCreationTime_ = workersStp[15].ElapsedMilliseconds;
            workersStp[15].Reset();
            workersStp[15].Start();
        }

        /// <summary>
        /// Get the value of the pot (number of chips in the pot).
        /// </summary>
        /// <returns>True if the pot size has changed</returns>
        private bool GetPotTask()
        {
            potPrev = potCurr;
            int newPot = screenShotProcessor[15].GetPot();

            if (newPot > -1)
            {
                potCurr = newPot;
            }

            bool isPotChange;

            if (potCurr == potPrev)
            {
                isPotChange = false;
            }
            else
            {
                isPotChange = true;
            }

            return isPotChange;
        }

        /// <summary>
        /// Stop the pot info task run timer and update the UI with the thread creation time and thread run time.
        /// </summary>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StopRunTimersPotInfoTask(long threadCreationTime_)
        {
            workersStp[15].Stop();
            timersDgv.Rows[6].Cells[15].Value = string.Format("{0:0.000}", threadCreationTime_ / 1000.0);
            timersDgv.Rows[6].Cells[15].Value = string.Format("{0:0.000}", workersStp[15].ElapsedMilliseconds / 1000.0);
            timersDgv.Rows[6].Cells[17].Value = jobCompletionCounter;
            jobCompletionCounter++;
            workersStp[15].Reset();
        }

        /// <summary>
        /// Create the task that finds which player is the dealer and which player is the action player (the player whose turn it is to act) [Worker 16].
        /// </summary>
        private void CreateDealerAndActionPlayerInfoTask()
        {
            workerTasks[16] = new Task(() =>
            {
                long threadCreationTime = 0;

                if (isRunTimers)
                { 
                    StartRuntTimersDealerActionPlayerTask(ref threadCreationTime);
                }

                isNewDealer = GetDealerTask();
                isNewActionPlayer = GetActionPlayerTask();

                if (saveBitmaps)
                {
                    // Draw the relevant rectangles so the saved bitmap can be interpreted
                    screenShotProcessor[16].DrawDealerRects();
                    screenShotProcessor[16].DrawHoldCardCheckRects(dealerIndexCurr);
                    screenShotProcessor[16].DrawActionBarRects();

                    screenShotProcessor[16].SaveBitmap();
                }

                if (isRunTimers)
                {
                    StopRunTimersDealerActionPlayerTask(threadCreationTime);
                }
            });
        }

        /// <summary>
        /// Start the run timers for the dealer and action player info task and get the thread creation time.
        /// </summary>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StartRuntTimersDealerActionPlayerTask(ref long threadCreationTime_)
        {
            workersStp[16].Stop();
            threadCreationTime_ = workersStp[16].ElapsedMilliseconds;
            workersStp[16].Reset();
            workersStp[16].Start();
        }

        /// <summary>
        /// Find which player is the dealer and identify new hands.
        /// </summary>
        /// <returns>True if there is a new dealer</returns>
        private bool GetDealerTask()
        {
            isNewDealerPrev = isNewDealer;

            int tempDealerIndex = screenShotProcessor[16].FindDealer();
            bool isNewDealer_ = false;

            // Don't do anything if there is no dealer
            if (tempDealerIndex != -1)
            {
                dealerChipNotFound = false;

                if (tempDealerIndex != dealerIndexCurr)
                {
                    // Check to make sure the dealer has hold cards If he doesn't the hand hasn't really started yet (blinds have not been posted yet).
                    // Wait for a screenshot where the new dealer has hold cards and blinds have been posted before declaring the start of a new hand.
                    if (screenShotProcessor[16].DealerHasHoldCards(tempDealerIndex) || isFirstScreenshot)
                    {
                        dealerIndexPrev = dealerIndexCurr;
                        dealerIndexCurr = tempDealerIndex;
                        isNewDealer_ = true;
                    }
                    else
                    {
                        dontUpdateData = true;
                    }
                }
            }
            else if (dealerIndexCurr != -1)
            {
                dealerChipNotFound = true;
            }

            return isNewDealer_;
        }

        /// <summary>
        /// Find which player is the action player (the player whose turn it is to act).
        /// </summary>
        /// <returns></returns>
        private bool GetActionPlayerTask()
        {
            int tempActionPlayerIndex = screenShotProcessor[16].FindActionPlayer();
            bool isNewActionPlayer_;

            if (tempActionPlayerIndex == actionPlayerIndexCurr)
            {
                isNewActionPlayer_ = false;
            }
            else
            {
                isNewActionPlayer_ = true;
            }

            // Never let the previous action player become -1 (-1 means no action player)
            if (actionPlayerIndexCurr != -1)
            {
                actionPlayerIndexPrev = actionPlayerIndexCurr;
            }

            // If this is a new hand set the current and previous action player to the current action player
            if ((isNewDealer || (isNewDealerPrev && actionPlayerIndexCurr == -1)) && tempActionPlayerIndex != -1)
            {
                actionPlayerIndexPrev = tempActionPlayerIndex;
            }

            actionPlayerIndexCurr = tempActionPlayerIndex;

            return isNewActionPlayer_;
        }

        /// <summary>
        /// Stop the dealer and action player info task run timer and update the UI with the thread creation time and thread run time.
        /// </summary>
        /// <param name="threadCreationTime_">The time it took for this worker thread to be created</param>
        private void StopRunTimersDealerActionPlayerTask(long threadCreationTime_)
        {
            workersStp[16].Stop();
            timersDgv.Rows[7].Cells[14].Value = string.Format("{0:0.000}", threadCreationTime_ / 1000.0);
            timersDgv.Rows[7].Cells[15].Value = string.Format("{0:0.000}", workersStp[16].ElapsedMilliseconds / 1000.0);
            timersDgv.Rows[7].Cells[17].Value = jobCompletionCounter;
            jobCompletionCounter++;
            workersStp[16].Reset();
        }

        #endregion
    }
}
