using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOL_Companion
{
    /// <summary>
    /// This class sets the size and location of MainForm and intializes all the form controls, their attributes and defines their locations.
    /// </summary>
    class SetupMainForm
    {
        #region Variables

        /// <summary>
        /// The text of the mouseClickX_Lbl
        /// </summary>
        string mouseClickLocationX_Str_;

        /// <summary>
        /// The text of the mouseClickY_Lbl
        /// </summary>
        string mouseClickLocationY_Str_;

        /// <summary>
        /// The text of the mouseClickColorLbl
        /// </summary>
        string mouseClickLocationColorStr_;

        /// <summary>
        /// The text of the mouseClickBrightnessLbl
        /// </summary>
        string mouseClickLocationBrightnessStr_;

        #endregion

        #region Colors

        /// <summary>
        /// The color of normal text
        /// </summary>
        Color normalTextClr_;

        /// <summary>
        /// The color used to indicate a change in the data
        /// </summary>
        Color dataChangeClr_;

        /// <summary>
        /// The color used for data descriptions
        /// </summary>
        Color dataIdClr_;

        /// <summary>
        /// The color used for the background of rich text boxes
        /// </summary>
        Color rtbBackgroundClr_;

        /// <summary>
        /// The color used to indicate a control is disabled
        /// </summary>
        Color controlDisabledClr_;

        #endregion

        #region Controls

        #region Form

        /// <summary>
        /// The MainForm to setp
        /// </summary>
        MainForm frm;

        #endregion

        #region ToolTips

        /// <summary>
        /// The tooltip control shared across MainForm
        /// </summary>
        ToolTip formTip_;

        #endregion

        #region GroupBoxes

        /// <summary>
        /// The basic settings groupbox
        /// </summary>
        GroupBox basicSettingsGrp_;

        /// <summary>
        /// The program control groupbox
        /// </summary>
        GroupBox programControlGrp_;

        /// <summary>
        /// The bitmap location tools groupbox
        /// </summary>
        GroupBox bitmapLocationToolsGrp_;

        /// <summary>
        /// The error logging groupbox
        /// </summary>
        GroupBox errorLoggingGrp_;

        #endregion

        #region Labels

        /// <summary>
        /// The player of interest Label
        /// </summary>
        Label playerOfInterestLbl_;

        /// <summary>
        /// The rectangle X value label
        /// </summary>
        Label rectX_Lbl_;

        /// <summary>
        /// The rectangle Y value label
        /// </summary>
        Label rectY_Lbl_;

        /// <summary>
        /// The rectange width value label
        /// </summary>
        Label rectWidthLbl_;

        /// <summary>
        /// The rectangle height value label
        /// </summary>
        Label rectHeightLbl_;

        /// <summary>
        /// The mouse click X location label
        /// </summary>
        Label mouseClickX_Lbl_;

        /// <summary>
        /// The mouse click Y location label
        /// </summary>
        Label mouseClickY_Lbl_;

        /// <summary>
        /// The mouse click location color label
        /// </summary>
        Label mouseClickColorLbl_;

        /// <summary>
        /// The mouse click location brightness value label
        /// </summary>
        Label mouseClickBrightnessLbl_;

        #endregion

        #region CheckBoxes

        /// <summary>
        /// The check box indicating that rectangles drawn on MainForm should be cleared on the next MainForm screenshot update
        /// </summary>
        CheckBox clearRectanglesChk_;

        /// <summary>
        /// The check box indicating that used on the next MainForm screenshot update should be saved
        /// </summary>
        CheckBox saveBitmapsChk_;

        /// <summary>
        /// The check box indicating that the application should automatically proceed to upload and process the next sequential screenshot when the current screenshot has finished processing
        /// </summary>
        CheckBox autoNextScreenShotChk_;

        /// <summary>
        /// The check box indicating that the aplication should display the data about the pixel that was clicked
        /// </summary>
        CheckBox showClickDataChk_;

        #endregion

        #region RadioButtons

        /// <summary>
        /// The radio button to select to run the application in live game mode
        /// </summary>
        RadioButton liveGameModeRdo_;

        /// <summary>
        /// The radio button to select to run the application in screenshots mode
        /// </summary>
        RadioButton screenshotsModeRdo_;

        /// <summary>
        /// The radio button to select to run the application based on a 9 player table
        /// </summary>
        RadioButton _9PlayerTableRdo_;

        /// <summary>
        /// The radio button to select to run the application based on a 10 player table
        /// </summary>
        RadioButton _10PlayerTableRdo_;

        #endregion

        #region TextBoxes

        /// <summary>
        /// The text box to enter the player of interest's name
        /// </summary>
        TextBox playerOfInterestTxt_;

        /// <summary>
        /// The text box to enter the X value of the rectangle to be drawn
        /// </summary>
        TextBox rectX_Txt_;

        /// <summary>
        /// The text box to enter the Y value of the rectangle to be drawn
        /// </summary>
        TextBox rectY_Txt_;

        /// <summary>
        /// The text box to enter the width value of the rectangle to be drawn
        /// </summary>
        TextBox rectWidthTxt_;

        /// <summary>
        /// The text box to enter the height value of the rectangle to be drawn
        /// </summary>
        TextBox rectHeightTxt_;

        /// <summary>
        /// The text box where error messages are displayed
        /// </summary>
        TextBox errorMessagesTxt_;

        #endregion

        #region Buttons

        /// <summary>
        /// The button to click to change the save location of the bitmap files that the application uses
        /// </summary>
        Button changeBitmapSaveLocation_;

        /// <summary>
        /// The button to click to start the application
        /// </summary>
        Button startPokerCompanion_;

        /// <summary>
        /// The button to click to open a screen shot file
        /// </summary>
        Button openScreenShotFile_;

        /// <summary>
        /// The button to click to process the next screen shot
        /// </summary>
        Button nextScreenShot_;

        /// <summary>
        /// The button to click to make a copy of the bitmap images on screen for the application's bitmap workers
        /// </summary>
        Button copyBitmapsForWorkers_;

        /// <summary>
        /// The button to click to draw a rectangle on screen based on the values defined by the user
        /// </summary>
        Button drawRect_;

        /// <summary>
        /// The button to click to draw all the rectangles used to define the bitmap image locations on screen that the application uses for data processing
        /// </summary>
        Button drawAllRects_;

        /// <summary>
        /// The button to click to clear all the rectangles drawn on screen the next time a new screen shot is processed
        /// </summary>
        Button clearAllDbData_;

        #endregion

        #region RichTextBoxes

        /// <summary>
        /// The rich text box listing the chips in the pot, the dealer and the action player
        /// </summary>
        RichTextBox potDealerAction_;

        /// <summary>
        /// The rich text box listing the actions taking place in the game
        /// </summary>
        RichTextBox gameActions_;

        /// <summary>
        /// The rich text box displaying the status of the board cards
        /// </summary>
        RichTextBox boardStatus_;

        /// <summary>
        /// The rich text box displaying the length of time the UI thread was idle during data processing
        /// </summary>
        RichTextBox UiIdleTimeRtb_;

        /// <summary>
        /// The rich text box displaying the total length of time it took to process the data in a screenshot
        /// </summary>
        RichTextBox totalProcessingTimeRtb_;

        /// <summary>
        /// The rich text box that identifies the special case when a new hand is detected without the dealer changing
        /// </summary>
        RichTextBox newHandDetectedRtb_;

        #endregion

        #region PictureBoxes

        /// <summary>
        /// The picture box containing the screenshot of the poker table
        /// </summary>
        PictureBox screenShotPic_;

        /// <summary>
        /// The picture box containing the screenshot of the player of interest's hand
        /// </summary>
        PictureBox myHandPic_;

        #endregion

        #region DataGridViews

        /// <summary>
        /// The data grid view displaying all the players and their information for the current hand
        /// </summary>
        DataGridView playersDgv_;

        /// <summary>
        /// The data grid view displaying the data for the processing timers
        /// </summary>
        DataGridView timersDgv_;

        #endregion

        #endregion

        public SetupMainForm(MainForm frmIn)
        {
            frm = frmIn;
        }

        /// <summary>
        /// Initialize the colors on MainForm.
        /// </summary>
        /// <param name="normalTextClrIn">normalTextClr</param>
        /// <param name="dataChangeClrIn">dataChangeClr</param>
        /// <param name="dataIdClrIn">dataIdClr</param>
        /// <param name="rtbBackgroundClrIn">rtbBackgroundClr</param>
        /// <param name="controlDisabledClrIn">controlDisabledClr</param>
        public void InitializeColors(ref Color normalTextClrIn, ref Color dataChangeClrIn, ref Color dataIdClrIn, ref Color rtbBackgroundClrIn,
            ref Color controlDisabledClrIn)
        {
            normalTextClrIn = Color.White;
            normalTextClr_ = normalTextClrIn;

            dataChangeClrIn = Color.Blue;
            dataChangeClr_ = dataChangeClrIn;

            dataIdClrIn = Color.SpringGreen;
            dataIdClr_ = dataIdClrIn;

            rtbBackgroundClrIn = Color.DimGray;
            rtbBackgroundClr_ = rtbBackgroundClrIn;

            controlDisabledClrIn = Color.FromArgb(85, 85, 85);
            controlDisabledClr_ = controlDisabledClrIn;
        }

        /// <summary>
        /// Set the location, size, title and background color of MainForm.
        /// </summary>
        public void InitializeForm()
        {
            if (Screen.AllScreens.Count() < 2)
            {
                frm.Location = Screen.AllScreens[0].WorkingArea.Location;
            }
            else
            {
                frm.Location = Screen.AllScreens[1].WorkingArea.Location;
            }
            frm.WindowState = FormWindowState.Maximized;
            frm.Text = "Control Window";
            frm.BackColor = rtbBackgroundClr_;
        }

        /// <summary>
        /// Pass the variables used to initialize MainForm to SetupMainForm.
        /// </summary>
        /// <param name="mouseClickLocationX_StrIn">mouseClickLocationX_Str</param>
        /// <param name="mouseClickLocationY_StrIn">mouseClickLocationY_Str</param>
        /// <param name="mouseClickLocationColorStrIn">mouseClickLocationColorStr</param>
        /// <param name="mouseClickLocationBrightnessStrIn">mouseClickLocationBrightnessStr</param>
        public void PassFormVariables(string mouseClickLocationX_StrIn, string mouseClickLocationY_StrIn, 
            string mouseClickLocationColorStrIn, string mouseClickLocationBrightnessStrIn)
        {
            mouseClickLocationX_Str_ = mouseClickLocationX_StrIn;
            mouseClickLocationY_Str_ = mouseClickLocationY_StrIn;
            mouseClickLocationColorStr_ = mouseClickLocationColorStrIn;
            mouseClickLocationBrightnessStr_ = mouseClickLocationBrightnessStrIn;
        }

        /// <summary>
        /// Initialize the tool tip controls on MainForm.
        /// </summary>
        /// <param name="formTipIn">formTip</param>
        public void InitializeToolTips(ref ToolTip formTipIn)
        {
            // Note: The same tooltip is used for many controls on MainForm
            formTipIn = new ToolTip();
            formTip_ = formTipIn;
            formTip_.AutoPopDelay = 12000;
            formTip_.InitialDelay = 1000;
            formTip_.ReshowDelay = 500;
            formTip_.ShowAlways = true;
        }

        /// <summary>
        /// Initialize the group box controls on MainForm.
        /// </summary>
        /// <param name="basicSettingsGrpIn">basicSettingsGrp</param>
        /// <param name="programControlGrpIn">programControlGrp</param>
        /// <param name="bitmapLocationToolsGrpIn">bitmapLocationToolsGrp</param>
        /// <param name="errorLoggingGrpIn">errorLoggingGrp</param>
        public void InitializeGroupBoxes(ref GroupBox basicSettingsGrpIn, ref GroupBox programControlGrpIn, ref GroupBox bitmapLocationToolsGrpIn, 
            ref GroupBox errorLoggingGrpIn)
        {
            basicSettingsGrpIn = new GroupBox();
            basicSettingsGrp_ = basicSettingsGrpIn;
            basicSettingsGrp_.Anchor = AnchorStyles.None;
            basicSettingsGrp_.Text = "Basic Settings";
            basicSettingsGrp_.BackColor = rtbBackgroundClr_;
            basicSettingsGrp_.ForeColor = normalTextClr_;
            frm.Controls.Add(basicSettingsGrp_);

            programControlGrpIn = new GroupBox();
            programControlGrp_ = programControlGrpIn;
            programControlGrp_.Anchor = AnchorStyles.None;
            programControlGrp_.Text = "Program Control";
            programControlGrp_.BackColor = rtbBackgroundClr_;
            programControlGrp_.ForeColor = normalTextClr_;
            frm.Controls.Add(programControlGrp_);

            bitmapLocationToolsGrpIn = new GroupBox();
            bitmapLocationToolsGrp_ = bitmapLocationToolsGrpIn;
            bitmapLocationToolsGrp_.Anchor = AnchorStyles.None;
            bitmapLocationToolsGrp_.Text = "Bitmap Location Tools";
            bitmapLocationToolsGrp_.BackColor = rtbBackgroundClr_;
            bitmapLocationToolsGrp_.ForeColor = normalTextClr_;
            frm.Controls.Add(bitmapLocationToolsGrp_);

            errorLoggingGrpIn = new GroupBox();
            errorLoggingGrp_ = errorLoggingGrpIn;
            errorLoggingGrp_.Anchor = AnchorStyles.None;
            errorLoggingGrp_.Text = "Error Logging";
            errorLoggingGrp_.BackColor = rtbBackgroundClr_;
            errorLoggingGrp_.ForeColor = normalTextClr_;
            frm.Controls.Add(errorLoggingGrp_);
        }

        /// <summary>
        /// Initialize the label controls on MainForm.
        /// </summary>
        /// <param name="playerOfInterestLblIn">playerOfInterestLbl</param>
        /// <param name="rectX_LblIn">rectX_Lbl</param>
        /// <param name="rectY_LblIn">rectY_Lbl</param>
        /// <param name="rectWidthLblIn">rectWidthLbl</param>
        /// <param name="rectHeightLblIn">rectHeightLbl</param>
        /// <param name="mouseClickX_LblIn">mouseClickX_Lbl</param>
        /// <param name="mouseClickY_LblIn">mouseClickY_Lbl</param>
        /// <param name="mouseClickColorLblIn">mouseClickColorLbl</param>
        /// <param name="mouseClickBrightnessLblIn">mouseClickBrightnessLbl</param>
        public void InitializeLabels(ref Label playerOfInterestLblIn, ref Label rectX_LblIn, ref Label rectY_LblIn, ref Label rectWidthLblIn,
            ref Label rectHeightLblIn, ref Label mouseClickX_LblIn, ref Label mouseClickY_LblIn, ref Label mouseClickColorLblIn, 
            ref Label mouseClickBrightnessLblIn)
        {
            playerOfInterestLblIn = new Label();
            playerOfInterestLbl_ = playerOfInterestLblIn;
            playerOfInterestLbl_.Anchor = AnchorStyles.None;
            playerOfInterestLbl_.Text = "Player of Interest";
            formTip_.SetToolTip(playerOfInterestLbl_,
                "This is the player for whom you want data to be displayed. If you are\n" +
                "playing a live game this should be your username.");
            basicSettingsGrp_.Controls.Add(playerOfInterestLbl_);

            rectX_LblIn = new Label();
            rectX_Lbl_ = rectX_LblIn;
            rectX_Lbl_.Anchor = AnchorStyles.None;
            rectX_Lbl_.Text = "X-Coordinate";
            bitmapLocationToolsGrp_.Controls.Add(rectX_Lbl_);

            rectY_LblIn = new Label();
            rectY_Lbl_ = rectY_LblIn;
            rectY_Lbl_.Anchor = AnchorStyles.None;
            rectY_Lbl_.Text = "Y-Coordinate";
            bitmapLocationToolsGrp_.Controls.Add(rectY_Lbl_);

            rectWidthLblIn = new Label();
            rectWidthLbl_ = rectWidthLblIn;
            rectWidthLbl_.Anchor = AnchorStyles.None;
            rectWidthLbl_.Text = "Width";
            bitmapLocationToolsGrp_.Controls.Add(rectWidthLbl_);

            rectHeightLblIn = new Label();
            rectHeightLbl_ = rectHeightLblIn;
            rectHeightLbl_.Anchor = AnchorStyles.None;
            rectHeightLbl_.Text = "Height";
            bitmapLocationToolsGrp_.Controls.Add(rectHeightLbl_);

            mouseClickX_LblIn = new Label();
            mouseClickX_Lbl_ = mouseClickX_LblIn;
            mouseClickX_Lbl_.Anchor = AnchorStyles.None;
            mouseClickX_Lbl_.Text = mouseClickLocationX_Str_ + "N/A";
            bitmapLocationToolsGrp_.Controls.Add(mouseClickX_Lbl_);

            mouseClickY_LblIn = new Label();
            mouseClickY_Lbl_ = mouseClickY_LblIn;
            mouseClickY_Lbl_.Anchor = AnchorStyles.None;
            mouseClickY_Lbl_.Text = mouseClickLocationY_Str_ + "N/A";
            bitmapLocationToolsGrp_.Controls.Add(mouseClickY_Lbl_);

            mouseClickColorLblIn = new Label();
            mouseClickColorLbl_ = mouseClickColorLblIn;
            mouseClickColorLbl_.Anchor = AnchorStyles.None;
            mouseClickColorLbl_.Text = mouseClickLocationColorStr_ + "N/A";
            bitmapLocationToolsGrp_.Controls.Add(mouseClickColorLbl_);

            mouseClickBrightnessLblIn = new Label();
            mouseClickBrightnessLbl_ = mouseClickBrightnessLblIn;
            mouseClickBrightnessLbl_.Anchor = AnchorStyles.None;
            mouseClickBrightnessLbl_.Text = mouseClickLocationBrightnessStr_ + "N/A";
            bitmapLocationToolsGrp_.Controls.Add(mouseClickBrightnessLbl_);
        }

        /// <summary>
        /// Initialize the check box controls on MainForm.
        /// </summary>
        /// <param name="clearRectanglesChkIn">clearRectanglesChk</param>
        /// <param name="saveBitmapsChkIn">saveBitmapsChk</param>
        /// <param name="autoNextScreenShotChkIn">autoNextScreenShotChk</param>
        /// <param name="showClickDataChkIn">showClickDataChk</param>
        public void InitializeCheckBoxes(ref CheckBox clearRectanglesChkIn, ref CheckBox saveBitmapsChkIn, 
            ref CheckBox autoNextScreenShotChkIn, ref CheckBox showClickDataChkIn)
        {
            clearRectanglesChkIn = new CheckBox();
            clearRectanglesChk_ = clearRectanglesChkIn;
            clearRectanglesChk_.Anchor = AnchorStyles.None;
            clearRectanglesChk_.Text = "Clear Rectangles";
            clearRectanglesChk_.Checked = false;
            formTip_.SetToolTip(clearRectanglesChk_,
                "Clear all rectangles drawn on the screen (if any) the\n" +
                "next time a screenshot is processed");
            // I am not adding this checkbox to the form. This checkbox has limited, non-critical functionality so I'm removing it for now.
            // Uncomment the line below to re-enable this checkbox. 2020.04.10
            // bitmapLocationToolsGrp_.Controls.Add(clearRectanglesChk_);

            saveBitmapsChkIn = new CheckBox();
            saveBitmapsChk_ = saveBitmapsChkIn;
            saveBitmapsChk_.Anchor = AnchorStyles.None;
            saveBitmapsChk_.Text = "Save Bitmap Files";
            saveBitmapsChk_.Checked = false;
            formTip_.SetToolTip(saveBitmapsChk_,
                "Save a copy of the bitmap files that this program uses to determine\n" +
                "what actions have taken place. This can be helpful for troubleshooting.");
            basicSettingsGrp_.Controls.Add(saveBitmapsChk_);
            saveBitmapsChk_.CheckedChanged += new EventHandler(frm.saveBitmapsChk_CheckChanged);

            autoNextScreenShotChkIn = new CheckBox();
            autoNextScreenShotChk_ = autoNextScreenShotChkIn;
            autoNextScreenShotChk_.Anchor = AnchorStyles.None;
            autoNextScreenShotChk_.Text = "Auto Continue to Next";
            autoNextScreenShotChk_.Checked = false;
            autoNextScreenShotChk_.Enabled = false;
            autoNextScreenShotChk_.BackColor = controlDisabledClr_;
            formTip_.SetToolTip(autoNextScreenShotChk_,
                "Advance to the next screenshot in the file with the \"Next Screnshot\" button\n" +
                "instead of using the \"Open Screenshot\" button and selecting the file.");
            programControlGrp_.Controls.Add(autoNextScreenShotChk_);
            autoNextScreenShotChk_.EnabledChanged += new EventHandler(frm.checkBox_EnabledChanged);

            showClickDataChkIn = new CheckBox();
            showClickDataChk_ = showClickDataChkIn;
            showClickDataChk_.Anchor = AnchorStyles.None;
            showClickDataChk_.Text = "Show Mouse Click Data";
            showClickDataChk_.Checked = false;
            formTip_.SetToolTip(showClickDataChk_,
                "Show the coordinates (in pixels), the color (in RGB format)\n" +
                "and the brightness value of pixels clicked on with the mouse.\n" +
                "The data will be displayed on the lines below.");
            bitmapLocationToolsGrp_.Controls.Add(showClickDataChk_);
            showClickDataChk_.CheckedChanged += new EventHandler(frm.showClickDataChk_CheckChanged);
            showClickDataChk_.EnabledChanged += new EventHandler(frm.checkBox_EnabledChanged);
        }

        /// <summary>
        /// Initialize the radio button controls on MainForm.
        /// </summary>
        /// <param name="liveGameModeRdoIn">liveGameModeRdo</param>
        /// <param name="screenshotsModeRdoIn">screenshotsModeRdo</param>
        /// <param name="_9PlayerTableRdoIn">_9PlayerTableRdo</param>
        /// <param name="_10PlayerTableRdoIn">_10PlayerTableRdo</param>
        public void InitializeRadioButtons(ref RadioButton liveGameModeRdoIn, ref RadioButton screenshotsModeRdoIn, 
            ref RadioButton _9PlayerTableRdoIn, ref RadioButton _10PlayerTableRdoIn)
        {
            liveGameModeRdoIn = new RadioButton();
            liveGameModeRdo_ = liveGameModeRdoIn;
            liveGameModeRdo_.Anchor = AnchorStyles.None;
            liveGameModeRdo_.Text = "Live Game\nMode";
            formTip_.SetToolTip(liveGameModeRdo_,
                "Process data from a poker game that is being played live. Screenshots\n" +
                "will be automatically taken and processed periodically.");
            basicSettingsGrp_.Controls.Add(liveGameModeRdo_);
            liveGameModeRdo_.CheckedChanged += new EventHandler(frm.liveGameModeRdo_CheckChanged);

            screenshotsModeRdoIn = new RadioButton();
            screenshotsModeRdo_ = screenshotsModeRdoIn;
            screenshotsModeRdo_.Anchor = AnchorStyles.None;
            screenshotsModeRdo_.Text = "Screenshots\nMode";
            screenshotsModeRdo_.Checked = true;
            formTip_.SetToolTip(screenshotsModeRdo_,
                "Run this program based on saved screenshots such as screenshots\n" +
                "saved from a previous poker game you were watching or playing.");
            basicSettingsGrp_.Controls.Add(screenshotsModeRdo_);
            screenshotsModeRdo_.CheckedChanged += new EventHandler(frm.screenshotsModeRdo_CheckChanged);

            _9PlayerTableRdoIn = new RadioButton();
            _9PlayerTableRdo_ = _9PlayerTableRdoIn;
            _9PlayerTableRdo_.Anchor = AnchorStyles.None;
            _9PlayerTableRdo_.Text = "9-Player\nTable";
            _9PlayerTableRdo_.Checked = true;
            // I am not adding this radio button to the form. I decided for simplicity to make this only work with 9-player tables for now.
            // Uncomment the line below to re-enable switching between 9 player and 10 player tables. 2020.03.23
            // basicSettingsGrp_.Controls.Add(_9PlayerTableRdo_);
            // _9PlayerTableRdo_.CheckedChanged += new EventHandler(frm._9PlayerTableRdo_CheckChanged);

            _10PlayerTableRdoIn = new RadioButton();
            _10PlayerTableRdo_ = _10PlayerTableRdoIn;
            _10PlayerTableRdo_.Anchor = AnchorStyles.None;
            _10PlayerTableRdo_.Text = "10-Player\nTable";
            // I am not adding this radio button to the form. I decided for simplicity to make this only work with 9-player tables for now.
            // Uncomment the line below to re-enable switching between 9 player and 10 player tables. 2020.03.23
            // basicSettingsGrp_.Controls.Add(_10PlayerTableRdo_);
            // _10PlayerTableRdo_.CheckedChanged += new EventHandler(frm._10PlayerTableRdo_CheckChanged);
        }

        /// <summary>
        /// Initialize the text box controls on MainForm.
        /// </summary>
        /// <param name="playerOfInterestTxtIn">playerOfInterestTxt</param>
        /// <param name="rectX_TxtIn">rectX_Txt</param>
        /// <param name="rectY_TxtIn">rectY_Txt</param>
        /// <param name="rectWidthTxtIn">rectWidthTxt</param>
        /// <param name="rectHeightTxtIn">rectHeightTxt</param>
        /// <param name="errorMessagesTxtIn">errorMessagesTxt</param>
        public void InitializeTextBoxes(ref TextBox playerOfInterestTxtIn, ref TextBox rectX_TxtIn, ref TextBox rectY_TxtIn, 
            ref TextBox rectWidthTxtIn, ref TextBox rectHeightTxtIn, ref TextBox errorMessagesTxtIn)
        {
            playerOfInterestTxtIn = new TextBox();
            playerOfInterestTxt_ = playerOfInterestTxtIn;
            playerOfInterestTxt_.Anchor = AnchorStyles.None;
            playerOfInterestTxt_.Text = "JabaAdam";
            formTip_.SetToolTip(playerOfInterestTxt_,
                "This is the player for whom you want data to be displayed. If you are\n" +
                "playing a live game this should be your username.");
            basicSettingsGrp_.Controls.Add(playerOfInterestTxt_);
            playerOfInterestTxt_.TextChanged += new EventHandler(frm.playerOfInterestTxt_TextChanged);

            rectX_TxtIn = new TextBox();
            rectX_Txt_ = rectX_TxtIn;
            rectX_Txt_.Anchor = AnchorStyles.None;
            bitmapLocationToolsGrp_.Controls.Add(rectX_Txt_);
            rectX_Txt_.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            rectY_TxtIn = new TextBox();
            rectY_Txt_ = rectY_TxtIn;
            rectY_Txt_.Anchor = AnchorStyles.None;
            bitmapLocationToolsGrp_.Controls.Add(rectY_Txt_);
            rectY_Txt_.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            rectWidthTxtIn = new TextBox();
            rectWidthTxt_ = rectWidthTxtIn;
            rectWidthTxt_.Anchor = AnchorStyles.None;
            bitmapLocationToolsGrp_.Controls.Add(rectWidthTxt_);
            rectWidthTxt_.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            rectHeightTxtIn = new TextBox();
            rectHeightTxt_ = rectHeightTxtIn;
            rectHeightTxt_.Anchor = AnchorStyles.None;
            bitmapLocationToolsGrp_.Controls.Add(rectHeightTxt_);
            rectHeightTxt_.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            errorMessagesTxtIn = new TextBox();
            errorMessagesTxt_ = errorMessagesTxtIn;
            errorMessagesTxt_.Anchor = AnchorStyles.None;
            errorMessagesTxt_.Multiline = true;
            errorMessagesTxt_.ScrollBars = ScrollBars.Vertical;
            errorMessagesTxt_.WordWrap = true;
            errorMessagesTxt_.ReadOnly = true;
            errorMessagesTxt_.TabStop = false;
            errorLoggingGrp_.Controls.Add(errorMessagesTxt_);
        }

        /// <summary>
        /// Initialize the button controls on MainForm.
        /// </summary>
        /// <param name="changeBitmapSaveLocationIn">changeBitmapSaveLocation</param>
        /// <param name="startPokerCompanionIn">startPokerCompanion</param>
        /// <param name="openScreenShotFileIn">openScreenShotFile</param>
        /// <param name="nextScreenShotIn">nextScreenShot</param>
        /// <param name="copyBitmapsForWorkersIn">copyBitmapsForWorkers</param>
        /// <param name="drawRectIn">drawRect</param>
        /// <param name="drawAllRectsIn">drawAllRects</param>
        /// <param name="clearAllDbDataIn">clearAllDbData</param>
        public void InitializeButtons(ref Button changeBitmapSaveLocationIn, ref Button startPokerCompanionIn, 
            ref Button openScreenShotFileIn, ref Button nextScreenShotIn, ref Button copyBitmapsForWorkersIn,
            ref Button drawRectIn, ref Button drawAllRectsIn, ref Button clearAllDbDataIn)
        {
            changeBitmapSaveLocationIn = new Button();
            changeBitmapSaveLocation_ = changeBitmapSaveLocationIn;
            changeBitmapSaveLocation_.Anchor = AnchorStyles.None;
            changeBitmapSaveLocation_.Text = "Change location where Bitmaps are saved";
            changeBitmapSaveLocation_.Enabled = false;
            changeBitmapSaveLocation_.BackColor = controlDisabledClr_;
            formTip_.SetToolTip(changeBitmapSaveLocation_,
                "Choose where to save a copy of the bitmap files that this program\n" +
                "uses to determine what actions have taken place. This can be helpful\n" +
                "for troubleshooting.");
            basicSettingsGrp_.Controls.Add(changeBitmapSaveLocation_);
            changeBitmapSaveLocation_.Click += new EventHandler(frm.changeBitmapSaveLocation_Click);
            changeBitmapSaveLocation_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            startPokerCompanionIn = new Button();
            startPokerCompanion_ = startPokerCompanionIn;
            startPokerCompanion_.Anchor = AnchorStyles.None;
            startPokerCompanion_.Text = "Start Poker Companion";
            startPokerCompanion_.Enabled = false;
            startPokerCompanion_.BackColor = controlDisabledClr_;
            programControlGrp_.Controls.Add(startPokerCompanion_);
            startPokerCompanion_.Click += new EventHandler(frm.startPokerCompanion_Click);
            startPokerCompanion_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            clearAllDbDataIn = new Button();
            clearAllDbData_ = clearAllDbDataIn;
            clearAllDbData_.Anchor = AnchorStyles.None;
            clearAllDbData_.Text = "Clear all database data";
            formTip_.SetToolTip(clearAllDbData_, "Clear all the data in the database (all database tables).");
            basicSettingsGrp_.Controls.Add(clearAllDbData_);
            clearAllDbData_.Click += new EventHandler(frm.clearAllDbData_Click);
            clearAllDbData_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            openScreenShotFileIn = new Button();
            openScreenShotFile_ = openScreenShotFileIn;
            openScreenShotFile_.Anchor = AnchorStyles.None;
            openScreenShotFile_.Text = "Open Screenshot";
            programControlGrp_.Controls.Add(openScreenShotFile_);
            openScreenShotFile_.Click += new EventHandler(frm.openScreenShotFile_Click);
            openScreenShotFile_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            nextScreenShotIn = new Button();
            nextScreenShot_ = nextScreenShotIn;
            nextScreenShot_.Anchor = AnchorStyles.None;
            nextScreenShot_.Text = "Next Screenshot";
            nextScreenShot_.Enabled = false;
            nextScreenShot_.BackColor = controlDisabledClr_;
            programControlGrp_.Controls.Add(nextScreenShot_);
            nextScreenShot_.Click += new EventHandler(frm.nextScreenShot_Click);
            nextScreenShot_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            copyBitmapsForWorkersIn = new Button();
            copyBitmapsForWorkers_ = copyBitmapsForWorkersIn;
            copyBitmapsForWorkers_.Anchor = AnchorStyles.None;
            copyBitmapsForWorkers_.Text = "Copy Bitmaps for Workers";
            // I am not adding this button to the form. I decided for simplicity and for spacing purposes (UI layout) to eliminate this control
            // for now. Uncomment the line below to re-enable this button and it's functionality. 2020.03.29
            // programControlGrp_.Controls.Add(copyBitmapsForWorkers_);
            copyBitmapsForWorkers_.Click += new EventHandler(frm.copyBitmapsForWorkers_Click);

            drawRectIn = new Button();
            drawRect_ = drawRectIn;
            drawRect_.Anchor = AnchorStyles.None;
            drawRect_.Text = "Draw Rectangle";
            formTip_.SetToolTip(drawRect_,
                "Draw a rectangle on the screen based on the coordinates and width and height\n" +
                "values entered into the above text boxes. All values are in unit of pixels. This\n" +
                "can be helpful for determining what bitmaps need to be copied to process data and\n" +
                "what pixels need to be looked at.");
            bitmapLocationToolsGrp_.Controls.Add(drawRect_);
            drawRect_.Click += new EventHandler(frm.drawRect_Click);
            drawRect_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            drawAllRectsIn = new Button();
            drawAllRects_ = drawAllRectsIn;
            drawAllRects_.Anchor = AnchorStyles.None;
            drawAllRects_.Text = "Draw All Rectangles";
            formTip_.SetToolTip(drawAllRects_,
                "Draw rectangles around the parts of the screen that this program will use to\n" +
                "determine what actions have taken place. These include the bitmaps that will\n" +
                "be copied to determine the Players' names, chip stacks hold cards etc. as well\n" +
                "as the pixels that will be looked at to determine which player is currently the\n" +
                "dealer, action player etc.");
            bitmapLocationToolsGrp_.Controls.Add(drawAllRects_);
            drawAllRects_.Click += new EventHandler(frm.drawAllRects_Click);
            drawAllRects_.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);
        }

        /// <summary>
        /// Initialize the rich text box controls on MainForm.
        /// </summary>
        /// <param name="potDealerActionIn">potDealerAction</param>
        /// <param name="gameActionsIn">gameActions</param>
        /// <param name="boardStatusIn">boardStatus</param>
        /// <param name="UiIdleTimeRtbIn">UiIdleTimeRtb</param>
        /// <param name="totalProcessingTimeRtbIn">totalProcessingTimeRtb</param>
        /// <param name="newHandDetectedRtbIn">newHandDetectedRtb</param>
        public void InitializeRichTextBoxes(ref RichTextBox potDealerActionIn, ref RichTextBox gameActionsIn, ref RichTextBox boardStatusIn,
            ref RichTextBox UiIdleTimeRtbIn, ref RichTextBox totalProcessingTimeRtbIn, ref RichTextBox newHandDetectedRtbIn)
        {
            potDealerActionIn = new RichTextBox();
            potDealerAction_ = potDealerActionIn;
            potDealerAction_.Anchor = AnchorStyles.None;
            potDealerAction_.BackColor = rtbBackgroundClr_;
            potDealerAction_.ForeColor = normalTextClr_;
            potDealerAction_.BorderStyle = BorderStyle.None;
            potDealerAction_.ReadOnly = true;
            potDealerAction_.Cursor = Cursors.Default;
            potDealerAction_.TabStop = false;
            potDealerAction_.SelectionTabs = new int[] { 117 };
            frm.Controls.Add(potDealerAction_);

            gameActionsIn = new RichTextBox();
            gameActions_ = gameActionsIn;
            gameActions_.Anchor = AnchorStyles.None;
            gameActions_.BackColor = rtbBackgroundClr_;
            gameActions_.ForeColor = normalTextClr_;
            gameActions_.BorderStyle = BorderStyle.None;
            gameActions_.ReadOnly = true;
            gameActions_.Cursor = Cursors.Default;
            gameActions_.TabStop = false;
            frm.Controls.Add(gameActions_);

            boardStatusIn = new RichTextBox();
            boardStatus_ = boardStatusIn;
            boardStatus_.Anchor = AnchorStyles.None;
            boardStatus_.BackColor = rtbBackgroundClr_;
            boardStatus_.ForeColor = normalTextClr_;
            boardStatus_.BorderStyle = BorderStyle.None;
            boardStatus_.ReadOnly = true;
            boardStatus_.Cursor = Cursors.Default;
            boardStatus_.TabStop = false;
            boardStatus_.SelectionTabs = new int[] { 48, 96, 144, 192 };
            frm.Controls.Add(boardStatus_);

            UiIdleTimeRtbIn = new RichTextBox();
            UiIdleTimeRtb_ = UiIdleTimeRtbIn;
            UiIdleTimeRtb_.Anchor = AnchorStyles.None;
            UiIdleTimeRtb_.BackColor = rtbBackgroundClr_;
            UiIdleTimeRtb_.ForeColor = normalTextClr_;
            UiIdleTimeRtb_.BorderStyle = BorderStyle.None;
            UiIdleTimeRtb_.ReadOnly = true;
            UiIdleTimeRtb_.Cursor = Cursors.Default;
            UiIdleTimeRtb_.TabStop = false;
            frm.Controls.Add(UiIdleTimeRtb_);

            totalProcessingTimeRtbIn = new RichTextBox();
            totalProcessingTimeRtb_ = totalProcessingTimeRtbIn;
            totalProcessingTimeRtb_.Anchor = AnchorStyles.None;
            totalProcessingTimeRtb_.BackColor = rtbBackgroundClr_;
            totalProcessingTimeRtb_.ForeColor = normalTextClr_;
            totalProcessingTimeRtb_.BorderStyle = BorderStyle.None;
            totalProcessingTimeRtb_.ReadOnly = true;
            totalProcessingTimeRtb_.Cursor = Cursors.Default;
            totalProcessingTimeRtb_.TabStop = false;
            frm.Controls.Add(totalProcessingTimeRtb_);

            newHandDetectedRtbIn = new RichTextBox();
            newHandDetectedRtb_ = newHandDetectedRtbIn;
            newHandDetectedRtb_.Anchor = AnchorStyles.None;
            newHandDetectedRtb_.BackColor = rtbBackgroundClr_;
            newHandDetectedRtb_.ForeColor = dataIdClr_;
            newHandDetectedRtb_.BorderStyle = BorderStyle.None;
            newHandDetectedRtb_.ReadOnly = true;
            newHandDetectedRtb_.Cursor = Cursors.Default;
            newHandDetectedRtb_.TabStop = false;
            newHandDetectedRtb_.Text = "New Hand Detected";
            newHandDetectedRtb_.Visible = false;
            frm.Controls.Add(newHandDetectedRtb_);
        }

        /// <summary>
        /// Initialize the data grid view controls on MainForm.
        /// </summary>
        /// <param name="playersDgvIn">playersDgv</param>
        /// <param name="timersDgvIn">timersDgv</param>
        public void InitializeDataGridViews(ref DataGridView playersDgvIn, ref DataGridView timersDgvIn)
        {
            playersDgvIn = new DataGridView();
            playersDgv_ = playersDgvIn;
            playersDgv_.Anchor = AnchorStyles.None;
            playersDgv_.DefaultCellStyle.BackColor = rtbBackgroundClr_;
            //playersDgv_.AlternatingRowsDefaultCellStyle.BackColor = Color.DarkGray;
            playersDgv_.ForeColor = normalTextClr_;
            playersDgv_.BorderStyle = BorderStyle.None;
            playersDgv_.TabStop = false;
            playersDgv_.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            playersDgv_.ColumnHeadersVisible = false;
            playersDgv_.RowHeadersVisible = false;
            playersDgv_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            playersDgv_.ReadOnly = true;
            playersDgv_.AllowDrop = false;
            playersDgv_.AllowUserToAddRows = false;
            playersDgv_.AllowUserToDeleteRows = false;
            playersDgv_.AllowUserToOrderColumns = false;
            playersDgv_.AllowUserToResizeColumns = false;
            playersDgv_.AllowUserToResizeRows = false;
            playersDgv_.RowCount = 10;
            playersDgv_.ColumnCount = 4;
            playersDgv_.Columns[0].Width = 31;
            playersDgv_.Columns[1].Width = 31;
            playersDgv_.Columns[2].Width = 182;
            playersDgv_.Columns[3].Width = 97;
            playersDgv_.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            playersDgv_.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            playersDgv_.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            playersDgv_.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            frm.Controls.Add(playersDgv_);
            playersDgv_.ClearSelection();

            timersDgvIn = new DataGridView();
            timersDgv_ = timersDgvIn;
            timersDgv_.Anchor = AnchorStyles.None;
            timersDgv_.DefaultCellStyle.BackColor = rtbBackgroundClr_;
            //timersDgv_.AlternatingRowsDefaultCellStyle.BackColor = Color.DarkGray;
            timersDgv_.ForeColor = normalTextClr_;
            timersDgv_.BorderStyle = BorderStyle.None;
            timersDgv_.TabStop = false;
            timersDgv_.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            timersDgv_.ColumnHeadersVisible = false;
            timersDgv_.RowHeadersVisible = false;
            timersDgv_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            timersDgv_.ReadOnly = true;
            timersDgv_.AllowDrop = false;
            timersDgv_.AllowUserToAddRows = false;
            timersDgv_.AllowUserToDeleteRows = false;
            timersDgv_.AllowUserToOrderColumns = false;
            timersDgv_.AllowUserToResizeColumns = false;
            timersDgv_.AllowUserToResizeRows = false;
            timersDgv_.RowCount = 9;
            timersDgv_.ColumnCount = 18;
            timersDgv_.Columns[0].Width = 31;
            timersDgv_.Columns[1].Width = 43;
            timersDgv_.Columns[2].Width = 27;
            timersDgv_.Columns[3].Width = 27;
            timersDgv_.Columns[4].Width = 31;
            timersDgv_.Columns[5].Width = 43;
            timersDgv_.Columns[6].Width = 27;
            timersDgv_.Columns[7].Width = 27;
            timersDgv_.Columns[8].Width = 31;
            timersDgv_.Columns[9].Width = 43;
            timersDgv_.Columns[10].Width = 43;
            timersDgv_.Columns[11].Width = 27;
            timersDgv_.Columns[12].Width = 27;
            timersDgv_.Columns[13].Width = 31;
            timersDgv_.Columns[14].Width = 43;
            timersDgv_.Columns[15].Width = 43;
            timersDgv_.Columns[16].Width = 27;
            timersDgv_.Columns[17].Width = 27;
            timersDgv_.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            timersDgv_.Columns[3].DividerWidth = 4;
            timersDgv_.Columns[7].DividerWidth = 4;
            timersDgv_.Columns[12].DividerWidth = 4;
            frm.Controls.Add(timersDgv_);
            timersDgv_.ClearSelection();
        }

        /// <summary>
        /// Initialize the picture box controls on MainForm.
        /// </summary>
        /// <param name="screenShotPicIn">screenShotPic</param>
        /// <param name="myHandPicIn">myHandPic</param>
        public void InitializePictureBoxes(ref PictureBox screenShotPicIn, ref PictureBox myHandPicIn)
        {
            screenShotPicIn = new PictureBox();
            screenShotPic_ = screenShotPicIn;
            screenShotPic_.Anchor = AnchorStyles.Top;
            screenShotPic_.Dock = DockStyle.Fill;
            screenShotPic_.SizeMode = PictureBoxSizeMode.Normal;
            frm.Controls.Add(screenShotPic_);
            screenShotPic_.MouseClick += new MouseEventHandler(frm.screenShotPic_MouseClick);

            myHandPicIn = new PictureBox();
            myHandPic_ = myHandPicIn;
            myHandPic_.SizeMode = PictureBoxSizeMode.Normal;
            frm.Controls.Add(myHandPic_);
        }

        /// <summary>
        /// Initialize the fonts to be used on MainForm.
        /// </summary>
        public void InitializeFonts()
        {
            Font rtbsFnt = potDealerAction_.SelectionFont;
            potDealerAction_.Font = new Font(rtbsFnt.FontFamily, 12, FontStyle.Bold);
            gameActions_.Font = new Font(rtbsFnt.FontFamily, 12, FontStyle.Bold);
            boardStatus_.Font = new Font(rtbsFnt.FontFamily, 12, FontStyle.Bold);
            UiIdleTimeRtb_.Font = new Font(rtbsFnt.FontFamily, 10, FontStyle.Bold);
            totalProcessingTimeRtb_.Font = new Font(rtbsFnt.FontFamily, 10, FontStyle.Bold);
            newHandDetectedRtb_.Font = new Font(rtbsFnt.FontFamily, 10, FontStyle.Bold);
            playersDgv_.Font = new Font(rtbsFnt.FontFamily, 8, FontStyle.Bold);
            timersDgv_.Font = new Font(rtbsFnt.FontFamily, 8, FontStyle.Bold);
        }

        /// <summary>
        /// Set the sizes and locations of the controls on MainForm.
        /// </summary>
        public void SetControlLocations()
        {             
            // Button sizes
            Size btnSize = new Size(155, 35);
            changeBitmapSaveLocation_.Size = btnSize;
            clearAllDbData_.Size = btnSize;
            startPokerCompanion_.Size = btnSize;
            openScreenShotFile_.Size = btnSize;
            nextScreenShot_.Size = btnSize;
            copyBitmapsForWorkers_.Size = btnSize;
            drawRect_.Size = btnSize;
            drawAllRects_.Size = btnSize;

            // Label sizes
            playerOfInterestLbl_.AutoSize = true;
            rectX_Lbl_.AutoSize = true;
            rectY_Lbl_.AutoSize = true;
            rectHeightLbl_.AutoSize = true;
            rectWidthLbl_.AutoSize = true;
            mouseClickX_Lbl_.AutoSize = true;
            mouseClickY_Lbl_.AutoSize = true;
            mouseClickColorLbl_.AutoSize = true;
            mouseClickBrightnessLbl_.AutoSize = true;

            // Check box sizes
            clearRectanglesChk_.AutoSize = true;
            saveBitmapsChk_.AutoSize = true;
            autoNextScreenShotChk_.AutoSize = true;
            showClickDataChk_.AutoSize = true;

            // Radio button sizes
            screenshotsModeRdo_.AutoSize = true;
            liveGameModeRdo_.AutoSize = true;
            _9PlayerTableRdo_.AutoSize = true;
            _10PlayerTableRdo_.AutoSize = true;

            // Text box sizes
            Size txtSize = new Size(75, 25);
            playerOfInterestTxt_.Size = new Size(btnSize.Width, txtSize.Height);
            rectX_Txt_.Size = txtSize;
            rectY_Txt_.Size = txtSize;
            rectHeightTxt_.Size = txtSize;
            rectWidthTxt_.Size = txtSize;

            // Error messages text box size
            Size txtErrorMessagesSize = new Size(239, 104);
            errorMessagesTxt_.Size = txtErrorMessagesSize;

            #region Left Column

            int xLocationLeft = 10;
            int grpIndentX = 12;
            int grpIndentY = 18;
            int grpSpacingY = 8;

            #region Basic Settings GroupBox

            Size grpBasicSettingsSize = new Size(200, 209);
            basicSettingsGrp_.Size = grpBasicSettingsSize;
            basicSettingsGrp_.Location=new Point(xLocationLeft, 150);

            screenshotsModeRdo_.Location = new Point(grpIndentX, grpIndentY);
            liveGameModeRdo_.Location = new Point(grpIndentX + 100, grpIndentY);
            playerOfInterestLbl_.Location = new Point(grpIndentX, screenshotsModeRdo_.Location.Y + screenshotsModeRdo_.Height + 5);
            playerOfInterestTxt_.Location = new Point(grpIndentX, playerOfInterestLbl_.Location.Y + playerOfInterestLbl_.Height + 2);
            saveBitmapsChk_.Location = new Point(grpIndentX, playerOfInterestTxt_.Location.Y + playerOfInterestTxt_.Height + 10);
            changeBitmapSaveLocation_.Location = new Point(grpIndentX, saveBitmapsChk_.Location.Y + saveBitmapsChk_.Height + 10);
            // _9PlayerTableRdo_.Location = new Point(grpIndentX, grpIndentY);
            // _10PlayerTableRdo_.Location = new Point(grpIndentX + 80, grpIndentY);
            clearAllDbData_.Location = new Point(grpIndentX, changeBitmapSaveLocation_.Location.Y + changeBitmapSaveLocation_.Height + 3);

            #endregion

            #region Program Control GroupBox

            Size grpProgramControlSize = new Size(200, 163);
            programControlGrp_.Size = grpProgramControlSize;
            programControlGrp_.Location = new Point(xLocationLeft, basicSettingsGrp_.Location.Y + basicSettingsGrp_.Height + grpSpacingY);

            startPokerCompanion_.Location = new Point(grpIndentX, grpIndentY + 1);
            openScreenShotFile_.Location = new Point(grpIndentX, startPokerCompanion_.Location.Y + startPokerCompanion_.Height + 3);
            autoNextScreenShotChk_.Location = new Point(grpIndentX, openScreenShotFile_.Location.Y + openScreenShotFile_.Height + 5);
            nextScreenShot_.Location = new Point(grpIndentX, autoNextScreenShotChk_.Location.Y + autoNextScreenShotChk_.Height + 3);
            // copyBitmapsForWorkers_.Location = new Point(grpIndentX, nextScreenShot_.Location.Y + nextScreenShot_.Height + 5);

            #endregion

            #region Bitmap Location Tools GroupBox

            Size grpBitmapLocationToolsSize = new Size(200, 274);
            bitmapLocationToolsGrp_.Size = grpBitmapLocationToolsSize;
            bitmapLocationToolsGrp_.Location=new Point(xLocationLeft, programControlGrp_.Location.Y + programControlGrp_.Height + grpSpacingY);

            // clearRectanglesChk_.Location = new Point(grpIndentX, grpIndentY + 1);
            rectX_Lbl_.Location = new Point(xLocationLeft, grpIndentY + 1);
            rectY_Lbl_.Location = new Point(xLocationLeft + txtSize.Width + 5, rectX_Lbl_.Location.Y);
            rectX_Txt_.Location = new Point(xLocationLeft, rectX_Lbl_.Location.Y + rectX_Lbl_.Height + 2);
            rectY_Txt_.Location = new Point(xLocationLeft + txtSize.Width + 5, rectX_Lbl_.Location.Y + rectX_Lbl_.Height + 2);
            rectWidthLbl_.Location = new Point(xLocationLeft, rectX_Txt_.Location.Y + rectX_Txt_.Height + 5);
            rectHeightLbl_.Location = new Point(xLocationLeft + txtSize.Width + 5, rectX_Txt_.Location.Y + rectX_Txt_.Height + 5);
            rectWidthTxt_.Location = new Point(xLocationLeft, rectWidthLbl_.Location.Y + rectWidthLbl_.Height + 2);
            rectHeightTxt_.Location = new Point(xLocationLeft + txtSize.Width + 5, rectWidthLbl_.Location.Y + rectWidthLbl_.Height + 2);
            drawRect_.Location = new Point(xLocationLeft, rectWidthTxt_.Location.Y + rectWidthTxt_.Height + 5);
            drawAllRects_.Location = new Point(xLocationLeft, drawRect_.Location.Y + drawRect_.Height + 5);
            showClickDataChk_.Location = new Point(xLocationLeft, drawAllRects_.Location.Y + drawAllRects_.Height + 8);
            mouseClickX_Lbl_.Location = new Point(xLocationLeft, showClickDataChk_.Location.Y + showClickDataChk_.Height + 5);
            mouseClickY_Lbl_.Location = new Point(xLocationLeft, mouseClickX_Lbl_.Location.Y + mouseClickX_Lbl_.Height + 2);
            mouseClickColorLbl_.Location = new Point(xLocationLeft, mouseClickY_Lbl_.Location.Y + mouseClickY_Lbl_.Height + 2);
            mouseClickBrightnessLbl_.Location = new Point(xLocationLeft, mouseClickColorLbl_.Location.Y + mouseClickColorLbl_.Height + 2);

            #endregion

            #region Error Logging GroupBox

            Size grpErrorLogSize = new Size(262, 130);
            errorLoggingGrp_.Size = grpErrorLogSize;
            errorLoggingGrp_.Location=new Point(bitmapLocationToolsGrp_.Location.X + bitmapLocationToolsGrp_.Width + xLocationLeft, bitmapLocationToolsGrp_.Location.Y + bitmapLocationToolsGrp_.Height - errorLoggingGrp_.Height);
            errorMessagesTxt_.Location = new Point(grpIndentX, grpIndentY);

            #endregion

            #endregion

            #region Right Column

            int xLocationRight = 1677;
            int rtbWidth = 238;
            int rtbWidthBoard = 256;
            int rtbWidthDgvPlayers = 342;

            Size rtbDealerActionSize = new Size(rtbWidth, 63);
            potDealerAction_.Size = rtbDealerActionSize;

            Size rtbStatusSize = new Size(rtbWidth, 585);
            gameActions_.Size = rtbStatusSize;

            Size rtbBoardSize = new Size(rtbWidthBoard, 42);
            boardStatus_.Size = rtbBoardSize;

            Size dgvPlayersSize = new Size(rtbWidthDgvPlayers, 221);
            playersDgv_.Size = dgvPlayersSize;

            potDealerAction_.Location = new Point(xLocationRight, 80);
            gameActions_.Location = new Point(xLocationRight, potDealerAction_.Location.Y + potDealerAction_.Height + 8);
            boardStatus_.Location = new Point(xLocationRight - rtbWidthBoard + rtbWidth, gameActions_.Location.Y + gameActions_.Height + 8);
            playersDgv_.Location = new Point(xLocationRight - rtbWidthDgvPlayers + rtbWidth, boardStatus_.Location.Y + boardStatus_.Height + 8);

            #endregion

            #region Timers Area

            Size rtbUiIdleTimeSize = new Size(136, 19);
            UiIdleTimeRtb_.Size = rtbUiIdleTimeSize;

            Size rtbTotalProcessTimeSize = new Size(209, 19);
            totalProcessingTimeRtb_.Size = rtbTotalProcessTimeSize;

            Size rtbNewHandDetectedSize = new Size(142, 19);
            newHandDetectedRtb_.Size = rtbNewHandDetectedSize;

            Size dgvTimersSize = new Size(599, 199);
            timersDgv_.Size = dgvTimersSize;

            UiIdleTimeRtb_.Location = new Point(1213, 793);
            totalProcessingTimeRtb_.Location = new Point(1355, 793);
            newHandDetectedRtb_.Location = new Point(1347, 770);
            timersDgv_.Location = new Point(965, 816);

            #endregion

            #region Active player hand picturebox

            myHandPic_.Location = new Point(1497, 617);

            #endregion
        }

        /// <summary>
        /// Set the default text for rich text boxes
        /// </summary>
        public void SetRtbDefaultText()
        {
            frm.AppendColoredRtbText(potDealerAction_, dataIdClr_, "Pot: ");
            frm.SetColoredChipCountRtbText(potDealerAction_, normalTextClr_, "\t0\n", false);
            frm.AppendColoredRtbText(potDealerAction_, dataIdClr_, "Dealer: ");
            potDealerAction_.AppendText("\tPlayer 01\n");
            frm.AppendColoredRtbText(potDealerAction_, dataIdClr_, "Action Player: ");
            potDealerAction_.AppendText("\tPlayer 01");

            frm.AppendColoredRtbText(boardStatus_, dataIdClr_, "F: ");
            boardStatus_.AppendText(" - -        ");
            frm.AppendColoredRtbText(boardStatus_, dataIdClr_, "\tF: ");
            boardStatus_.AppendText(" - -        ");
            frm.AppendColoredRtbText(boardStatus_, dataIdClr_, "\tF: ");
            boardStatus_.AppendText(" - - \n");
            frm.AppendColoredRtbText(boardStatus_, dataIdClr_, "\tT: ");
            boardStatus_.AppendText(" - -        ");
            frm.AppendColoredRtbText(boardStatus_, dataIdClr_, "\tR: ");
            boardStatus_.AppendText(" - - ");

            frm.AppendColoredRtbText(UiIdleTimeRtb_, dataIdClr_, "UI Idle Time: ");
            UiIdleTimeRtb_.AppendText("0.000");

            frm.AppendColoredRtbText(totalProcessingTimeRtb_, dataIdClr_, "Total Processing Time: ");
            totalProcessingTimeRtb_.AppendText("0.000");
        }

        /// <summary>
        /// Set the default text for data grid views
        /// </summary>
        public void SetDgvDefaultText()
        { 
            for (int i = 0; i < 10; i++)
            {
                playersDgv_.Rows[i].Cells[0].Value = " - - ";
                playersDgv_.Rows[i].Cells[1].Value = " - - ";
                playersDgv_.Rows[i].Cells[2].Value = "Player " + (i + 1).ToString("D2");
                playersDgv_.Rows[i].Cells[3].Value = "0";
            }

            for (int i = 0; i < 9; i++)
            {
                timersDgv_.Rows[i].Cells[0].Style = new DataGridViewCellStyle { ForeColor = dataIdClr_ };
                timersDgv_.Rows[i].Cells[0].Value = "c" + (i + 1).ToString();
                timersDgv_.Rows[i].Cells[1].Value = "0.000";
                timersDgv_.Rows[i].Cells[2].Value = (i + 1).ToString();
                timersDgv_.Rows[i].Cells[3].Value = (i + 1).ToString();
                timersDgv_.Rows[i].Cells[4].Style = new DataGridViewCellStyle { ForeColor = dataIdClr_ };
                if (i != 8)
                {
                    timersDgv_.Rows[i].Cells[4].Value = "c" + (i + 10).ToString();
                    timersDgv_.Rows[i].Cells[5].Value = "0.000";
                    timersDgv_.Rows[i].Cells[6].Value = (i + 10).ToString();
                    timersDgv_.Rows[i].Cells[7].Value = (i + 10).ToString();
                }
                else
                {
                    timersDgv_.Rows[i].Cells[4].Value = "SS.";
                    timersDgv_.Rows[i].Cells[5].Value = "0.000";
                    timersDgv_.Rows[i].Cells[6].Value = "-";
                    timersDgv_.Rows[i].Cells[7].Value = "-";
                }
                timersDgv_.Rows[i].Cells[8].Style = new DataGridViewCellStyle { ForeColor = dataIdClr_ };
                timersDgv_.Rows[i].Cells[8].Value = "w" + (i + 1).ToString();
                timersDgv_.Rows[i].Cells[9].Value = "0.000";
                timersDgv_.Rows[i].Cells[10].Value = "0.000";
                timersDgv_.Rows[i].Cells[11].Value = (i + 18).ToString();
                timersDgv_.Rows[i].Cells[12].Value = (i + 18).ToString();
                timersDgv_.Rows[i].Cells[13].Style = new DataGridViewCellStyle { ForeColor = dataIdClr_ };
                if (i != 8)
                {
                    timersDgv_.Rows[i].Cells[13].Value = "w" + (i + 10).ToString();
                    timersDgv_.Rows[i].Cells[14].Value = "0.000";
                    timersDgv_.Rows[i].Cells[15].Value = "0.000";
                    timersDgv_.Rows[i].Cells[16].Value = (i + 27).ToString();
                    timersDgv_.Rows[i].Cells[17].Value = (i + 27).ToString();
                }
                else
                {
                    timersDgv_.Rows[i].Cells[13].Value = "UI";
                    timersDgv_.Rows[i].Cells[14].Value = "-";
                    timersDgv_.Rows[i].Cells[15].Value = "0.000";
                    timersDgv_.Rows[i].Cells[16].Value = "-";
                    timersDgv_.Rows[i].Cells[17].Value = "-";
                }
            }
        }
    }
}
