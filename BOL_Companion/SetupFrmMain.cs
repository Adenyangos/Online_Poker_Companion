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
    /// This class sets the size and location of frmMain and intializes all the form controls, their attributes and defines their locations.
    /// </summary>
    class SetupFrmMain
    {
        private frmMain frm;

        public SetupFrmMain(frmMain frmIn)
        {
            frm = frmIn;

            InitializeForm();
            InitializeControls();
            SetControlLocations();
        }

        private void InitializeForm()
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
            frm.Text = "Control";
            frm.BackColor = frm.clrRtbBackground;
        }

        private void InitializeControls()
        {
            #region Tooltips

            // Note: The same tooltip can be used for many controls
            frm.tipFrmMain = new ToolTip();
            frm.tipFrmMain.AutoPopDelay = 12000;
            frm.tipFrmMain.InitialDelay = 1000;
            frm.tipFrmMain.ReshowDelay = 500;
            frm.tipFrmMain.ShowAlways = true;

            #endregion

            #region GroupBoxes

            frm.grpBasicSettings = new GroupBox();
            frm.grpBasicSettings.Anchor = AnchorStyles.None;
            frm.grpBasicSettings.Text = "Basic Settings";
            frm.grpBasicSettings.BackColor = frm.clrRtbBackground;
            frm.grpBasicSettings.ForeColor = frm.clrNormalText;
            frm.Controls.Add(frm.grpBasicSettings);

            frm.grpProgramControl = new GroupBox();
            frm.grpProgramControl.Anchor = AnchorStyles.None;
            frm.grpProgramControl.Text = "Program Control";
            frm.grpProgramControl.BackColor = frm.clrRtbBackground;
            frm.grpProgramControl.ForeColor = frm.clrNormalText;
            frm.Controls.Add(frm.grpProgramControl);

            frm.grpBitmapLocationTools = new GroupBox();
            frm.grpBitmapLocationTools.Anchor = AnchorStyles.None;
            frm.grpBitmapLocationTools.Text = "Bitmap Location Tools";
            frm.grpBitmapLocationTools.BackColor = frm.clrRtbBackground;
            frm.grpBitmapLocationTools.ForeColor = frm.clrNormalText;
            frm.Controls.Add(frm.grpBitmapLocationTools);

            frm.grpErrorLogging = new GroupBox();
            frm.grpErrorLogging.Anchor = AnchorStyles.None;
            frm.grpErrorLogging.Text = "Error Logging";
            frm.grpErrorLogging.BackColor = frm.clrRtbBackground;
            frm.grpErrorLogging.ForeColor = frm.clrNormalText;
            frm.Controls.Add(frm.grpErrorLogging);

            #endregion

            #region Labels

            frm.lblPlayerOfInterest = new Label();
            frm.lblPlayerOfInterest.Anchor = AnchorStyles.None;
            frm.lblPlayerOfInterest.Text = "Player of Interest";
            frm.tipFrmMain.SetToolTip(frm.lblPlayerOfInterest,
                "This is the player for whom you want data to be displayed. If you are\n" +
                "playing a live game this should be your username.");
            frm.grpBasicSettings.Controls.Add(frm.lblPlayerOfInterest);

            frm.lblRectX = new Label();
            frm.lblRectX.Anchor = AnchorStyles.None;
            frm.lblRectX.Text = "X-Coordinate";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblRectX);

            frm.lblRectY = new Label();
            frm.lblRectY.Anchor = AnchorStyles.None;
            frm.lblRectY.Text = "Y-Coordinate";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblRectY);

            frm.lblRectWidth = new Label();
            frm.lblRectWidth.Anchor = AnchorStyles.None;
            frm.lblRectWidth.Text = "Width";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblRectWidth);

            frm.lblRectHeight = new Label();
            frm.lblRectHeight.Anchor = AnchorStyles.None;
            frm.lblRectHeight.Text = "Height";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblRectHeight);

            frm.lblMouseClickX = new Label();
            frm.lblMouseClickX.Anchor = AnchorStyles.None;
            frm.lblMouseClickX.Text = frm.strMouseLabelX + "N/A";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblMouseClickX);

            frm.lblMouseClickY = new Label();
            frm.lblMouseClickY.Anchor = AnchorStyles.None;
            frm.lblMouseClickY.Text = frm.strMouseLabelY + "N/A";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblMouseClickY);

            frm.lblMouseClickClr = new Label();
            frm.lblMouseClickClr.Anchor = AnchorStyles.None;
            frm.lblMouseClickClr.Text = frm.strMouseLabelClr + "N/A";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblMouseClickClr);

            frm.lblMouseClickBright = new Label();
            frm.lblMouseClickBright.Anchor = AnchorStyles.None;
            frm.lblMouseClickBright.Text = frm.strMouseLabelBright + "N/A";
            frm.grpBitmapLocationTools.Controls.Add(frm.lblMouseClickBright);

            #endregion

            #region CheckBoxes

            frm.chkClearRects = new CheckBox();
            frm.chkClearRects.Anchor = AnchorStyles.None;
            frm.chkClearRects.Text = "Clear Rectangles";
            frm.chkClearRects.Checked = false;
            frm.tipFrmMain.SetToolTip(frm.chkClearRects,
                "Clear all rectangles drawn on the screen (if any) the\n" +
                "next time a screenshot is processed");
            // I am not adding this checkbox to the form. This checkbox has limited, non-critical functionality so i'm removing it for now.
            // Uncomment the line below to re-enable this checkbox. 2020.04.10
            // frm.grpBitmapLocationTools.Controls.Add(frm.chkClearRects);

            frm.chkSaveBitmaps = new CheckBox();
            frm.chkSaveBitmaps.Anchor = AnchorStyles.None;
            frm.chkSaveBitmaps.Text = "Save Bitmap Files";
            frm.chkSaveBitmaps.Checked = false;
            frm.tipFrmMain.SetToolTip(frm.chkSaveBitmaps, 
                "Save a copy of the bitmap files that this program uses to determine\n" +
                "what actions have taken place. This can be helpful for troubleshooting.");
            frm.grpBasicSettings.Controls.Add(frm.chkSaveBitmaps);
            frm.chkSaveBitmaps.CheckedChanged += new EventHandler(frm.chkSaveBitmaps_CheckChanged);

            frm.chkAutoNextScreenShot = new CheckBox();
            frm.chkAutoNextScreenShot.Anchor = AnchorStyles.None;
            frm.chkAutoNextScreenShot.Text = "Auto Continue to Next";
            frm.chkAutoNextScreenShot.Checked = false;
            frm.chkAutoNextScreenShot.Enabled = false;
            frm.chkAutoNextScreenShot.BackColor = frm.clrControlDisabled;
            frm.tipFrmMain.SetToolTip(frm.chkAutoNextScreenShot, 
                "Advance to the next screenshot in the file with the \"Next Screnshot\" button\n" +
                "instead of using the \"Open Screenshot\" button and selecting the file.");
            frm.grpProgramControl.Controls.Add(frm.chkAutoNextScreenShot);
            frm.chkAutoNextScreenShot.EnabledChanged += new EventHandler(frm.chk_EnabledChanged);

            frm.chkShowClickData = new CheckBox();
            frm.chkShowClickData.Anchor = AnchorStyles.None;
            frm.chkShowClickData.Text = "Show Mouse Click Data";
            frm.chkShowClickData.Checked = false;
            frm.tipFrmMain.SetToolTip(frm.chkShowClickData,
                "Show the coordinates (in pixels), the color (in RGB format)\n" +
                "and the brightness value of pixels clicked on with the mouse.\n" +
                "The data will be displayed on the lines below.");
            frm.grpBitmapLocationTools.Controls.Add(frm.chkShowClickData);
            frm.chkShowClickData.CheckedChanged += new EventHandler(frm.chkShowClickData_CheckChanged);
            frm.chkShowClickData.EnabledChanged += new EventHandler(frm.chk_EnabledChanged);

            #endregion

            #region RadioButtons

            frm.rdoModeScreenshotMode = new RadioButton();
            frm.rdoModeScreenshotMode.Anchor = AnchorStyles.None;
            frm.rdoModeScreenshotMode.Text = "Screenshots\nMode";
            frm.rdoModeScreenshotMode.Checked = true;
            frm.tipFrmMain.SetToolTip(frm.rdoModeScreenshotMode,
                "Run this program based on saved screenshots such as screenshots\n" +
                "saved from a previous poker game you were watching or playing.");
            frm.grpBasicSettings.Controls.Add(frm.rdoModeScreenshotMode);
            frm.rdoModeScreenshotMode.CheckedChanged += new EventHandler(frm.rdoModeScreenshotMode_CheckChanged);

            frm.rdoModeLiveGame = new RadioButton();
            frm.rdoModeLiveGame.Anchor = AnchorStyles.None;
            frm.rdoModeLiveGame.Text = "Live Game\nMode";
            frm.tipFrmMain.SetToolTip(frm.rdoModeLiveGame,
                "Process data from a poker game that is being played live. Screenshots\n" +
                "will be automatically taken and processed periodically.");
            frm.grpBasicSettings.Controls.Add(frm.rdoModeLiveGame);
            frm.rdoModeLiveGame.CheckedChanged += new EventHandler(frm.rdoModeLiveGame_CheckChanged);

            frm.rdo9PlayerTable = new RadioButton();
            frm.rdo9PlayerTable.Anchor = AnchorStyles.None;
            frm.rdo9PlayerTable.Text = "9-Player\nTable";
            frm.rdo9PlayerTable.Checked = true;
            // I am not adding this radio button to the form. I decided for simplicity to make this only work with 9-player tables for now.
            // Uncomment the line below to re-enable switching between 9 player and 10 player tables. 2020.03.23
            // frm.grpBasicSettings.Controls.Add(frm.rdo9PlayerTable);
            frm.rdo9PlayerTable.CheckedChanged += new EventHandler(frm.rdo9PlayerTable_CheckChanged);

            frm.rdo10PlayerTable = new RadioButton();
            frm.rdo10PlayerTable.Anchor = AnchorStyles.None;
            frm.rdo10PlayerTable.Text = "10-Player\nTable";
            // I am not adding this radio button to the form. I decided for simplicity to make this only work with 9-player tables for now.
            // Uncomment the line below to re-enable switching between 9 player and 10 player tables. 2020.03.23
            // frm.grpBasicSettings.Controls.Add(frm.rdo10PlayerTable);
            frm.rdo10PlayerTable.CheckedChanged += new EventHandler(frm.rdo10PlayerTable_CheckChanged);

            #endregion

            #region TextBoxes

            frm.txtPlayerOfInterest = new TextBox();
            frm.txtPlayerOfInterest.Anchor = AnchorStyles.None;
            frm.txtPlayerOfInterest.Text = "JabaAdam";
            frm.tipFrmMain.SetToolTip(frm.txtPlayerOfInterest,
                "This is the player for whom you want data to be displayed. If you are\n" +
                "playing a live game this should be your username.");
            frm.grpBasicSettings.Controls.Add(frm.txtPlayerOfInterest);
            frm.txtPlayerOfInterest.TextChanged += new EventHandler(frm.txtPlayerOfInterest_TextChanged);

            frm.txtRectX = new TextBox();
            frm.txtRectX.Anchor = AnchorStyles.None;
            frm.grpBitmapLocationTools.Controls.Add(frm.txtRectX);
            frm.txtRectX.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            frm.txtRectY = new TextBox();
            frm.txtRectY.Anchor = AnchorStyles.None;
            frm.grpBitmapLocationTools.Controls.Add(frm.txtRectY);
            frm.txtRectY.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            frm.txtRectWidth = new TextBox();
            frm.txtRectWidth.Anchor = AnchorStyles.None;
            frm.grpBitmapLocationTools.Controls.Add(frm.txtRectWidth);
            frm.txtRectWidth.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            frm.txtRectHeight = new TextBox();
            frm.txtRectHeight.Anchor = AnchorStyles.None;
            frm.grpBitmapLocationTools.Controls.Add(frm.txtRectHeight);
            frm.txtRectHeight.EnabledChanged += new EventHandler(frm.txt_EnabledChanged);

            frm.txtErrorMessages = new TextBox();
            frm.txtErrorMessages.Anchor = AnchorStyles.None;
            frm.txtErrorMessages.Multiline = true;
            frm.txtErrorMessages.ScrollBars = ScrollBars.Vertical;
            frm.txtErrorMessages.WordWrap = true;
            frm.txtErrorMessages.ReadOnly = true;
            frm.txtErrorMessages.TabStop = false;
            frm.grpErrorLogging.Controls.Add(frm.txtErrorMessages);

            #endregion

            #region Buttons

            frm.btnChangeBitmapSaveLocation = new Button();
            frm.btnChangeBitmapSaveLocation.Anchor = AnchorStyles.None;
            frm.btnChangeBitmapSaveLocation.Text = "Change location where Bitmaps are saved";
            frm.btnChangeBitmapSaveLocation.Enabled = false;
            frm.btnChangeBitmapSaveLocation.BackColor = frm.clrControlDisabled;
            frm.tipFrmMain.SetToolTip(frm.btnChangeBitmapSaveLocation, 
                "Choose where to save a copy of the bitmap files that this program\n" +
                "uses to determine what actions have taken place. This can be helpful\n" +
                "for troubleshooting.");
            frm.grpBasicSettings.Controls.Add(frm.btnChangeBitmapSaveLocation);
            frm.btnChangeBitmapSaveLocation.Click += new EventHandler(frm.btnChangeBitmapSaveLocation_Click);
            frm.btnChangeBitmapSaveLocation.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnClearAllDbData = new Button();
            frm.btnClearAllDbData.Anchor = AnchorStyles.None;
            frm.btnClearAllDbData.Text = "Clear all database data";
            frm.tipFrmMain.SetToolTip(frm.btnClearAllDbData, "Clear all the data in the database (all database tables).");
            frm.grpBasicSettings.Controls.Add(frm.btnClearAllDbData);
            frm.btnClearAllDbData.Click += new EventHandler(frm.btnClearAllDbData_Click);
            frm.btnClearAllDbData.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnStartPokerCompanion = new Button();
            frm.btnStartPokerCompanion.Anchor = AnchorStyles.None;
            frm.btnStartPokerCompanion.Text = "Start Poker Companion";
            frm.btnStartPokerCompanion.Enabled = false;
            frm.btnStartPokerCompanion.BackColor = frm.clrControlDisabled;
            frm.grpProgramControl.Controls.Add(frm.btnStartPokerCompanion);
            frm.btnStartPokerCompanion.Click += new EventHandler(frm.btnStartPokerCompanion_Click);
            frm.btnStartPokerCompanion.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnOpenScreenShotFile = new Button();
            frm.btnOpenScreenShotFile.Anchor = AnchorStyles.None;
            frm.btnOpenScreenShotFile.Text = "Open Screenshot";
            frm.grpProgramControl.Controls.Add(frm.btnOpenScreenShotFile);
            frm.btnOpenScreenShotFile.Click += new EventHandler(frm.btnOpenScreenShotFile_Click);
            frm.btnOpenScreenShotFile.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnNextScreenShot = new Button();
            frm.btnNextScreenShot.Anchor = AnchorStyles.None;
            frm.btnNextScreenShot.Text = "Next Screenshot";
            frm.btnNextScreenShot.Enabled = false;
            frm.btnNextScreenShot.BackColor = frm.clrControlDisabled;
            frm.grpProgramControl.Controls.Add(frm.btnNextScreenShot);
            frm.btnNextScreenShot.Click += new EventHandler(frm.btnNextScreenShot_Click);
            frm.btnNextScreenShot.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnCopyBitmapsForWorkers = new Button();
            frm.btnCopyBitmapsForWorkers.Anchor = AnchorStyles.None;
            frm.btnCopyBitmapsForWorkers.Text = "Copy Bitmaps for Workers";
            // I am not adding this button to the form. I decided for simplicity and for spacing purposes (UI layout) to eliminate this control
            // for now. Uncomment the line below to re-enable this button and it's functionality. 2020.03.29
            // frm.grpProgramControl.Controls.Add(frm.btnCopyBitmapsForWorkers);
            frm.btnCopyBitmapsForWorkers.Click += new EventHandler(frm.btnCopyBitmapsForWorkers_Click);

            frm.btnDrawRect = new Button();
            frm.btnDrawRect.Anchor = AnchorStyles.None;
            frm.btnDrawRect.Text = "Draw Rectangle";
            frm.tipFrmMain.SetToolTip(frm.btnDrawRect,
                "Draw a rectangle on the screen based on the coordinates and width and height\n" +
                "values entered into the above text boxes. All values are in unit of pixels. This\n" +
                "can be helpful for determining what bitmaps need to be copied to process data and\n" +
                "what pixels need to be looked at.");
            frm.grpBitmapLocationTools.Controls.Add(frm.btnDrawRect);
            frm.btnDrawRect.Click += new EventHandler(frm.btnDrawRect_Click);
            frm.btnDrawRect.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            frm.btnDrawAllRects = new Button();
            frm.btnDrawAllRects.Anchor = AnchorStyles.None;
            frm.btnDrawAllRects.Text = "Draw All Rectangles";
            frm.tipFrmMain.SetToolTip(frm.btnDrawAllRects,
                "Draw rectangles around the parts of the screen that this program will use to\n" +
                "determine what actions have taken place. These include the bitmaps that will\n" +
                "be copied to determine the Players' names, chip stacks hold cards etc. as well\n" +
                "as the pixels that will be looked at to determine which player is currently the\n" +
                "dealer, action player etc.");
            frm.grpBitmapLocationTools.Controls.Add(frm.btnDrawAllRects);
            frm.btnDrawAllRects.Click += new EventHandler(frm.btnDrawAllRects_Click);
            frm.btnDrawAllRects.EnabledChanged += new EventHandler(frm.btn_EnabledChanged);

            #endregion

            #region RichTextBoxes

            frm.rtbPotDealerAction = new RichTextBox();
            frm.rtbPotDealerAction.Anchor = AnchorStyles.None;
            frm.rtbPotDealerAction.BackColor = frm.clrRtbBackground;
            frm.rtbPotDealerAction.ForeColor = frm.clrNormalText;
            frm.rtbPotDealerAction.BorderStyle = BorderStyle.None;
            frm.rtbPotDealerAction.ReadOnly = true;
            frm.rtbPotDealerAction.Cursor = Cursors.Default;
            frm.rtbPotDealerAction.TabStop = false;
            frm.rtbPotDealerAction.SelectionTabs = new int[] { 117 };
            frm.Controls.Add(frm.rtbPotDealerAction);

            frm.rtbStatus = new RichTextBox();
            frm.rtbStatus.Anchor = AnchorStyles.None;
            frm.rtbStatus.BackColor = frm.clrRtbBackground;
            frm.rtbStatus.ForeColor = frm.clrNormalText;
            frm.rtbStatus.BorderStyle = BorderStyle.None;
            frm.rtbStatus.ReadOnly = true;
            frm.rtbStatus.Cursor = Cursors.Default;
            frm.rtbStatus.TabStop = false;
            frm.Controls.Add(frm.rtbStatus);

            frm.rtbBoard = new RichTextBox();
            frm.rtbBoard.Anchor = AnchorStyles.None;
            frm.rtbBoard.BackColor = frm.clrRtbBackground;
            frm.rtbBoard.ForeColor = frm.clrNormalText;
            frm.rtbBoard.BorderStyle = BorderStyle.None;
            frm.rtbBoard.ReadOnly = true;
            frm.rtbBoard.Cursor = Cursors.Default;
            frm.rtbBoard.TabStop = false;
            frm.rtbBoard.SelectionTabs = new int[] { 48, 96, 144, 192 };
            frm.Controls.Add(frm.rtbBoard);

            frm.rtbUiIdleTime = new RichTextBox();
            frm.rtbUiIdleTime.Anchor = AnchorStyles.None;
            frm.rtbUiIdleTime.BackColor = frm.clrRtbBackground;
            frm.rtbUiIdleTime.ForeColor = frm.clrNormalText;
            frm.rtbUiIdleTime.BorderStyle = BorderStyle.None;
            frm.rtbUiIdleTime.ReadOnly = true;
            frm.rtbUiIdleTime.Cursor = Cursors.Default;
            frm.rtbUiIdleTime.TabStop = false;
            frm.Controls.Add(frm.rtbUiIdleTime);

            frm.rtbTotalProcessTime = new RichTextBox();
            frm.rtbTotalProcessTime.Anchor = AnchorStyles.None;
            frm.rtbTotalProcessTime.BackColor = frm.clrRtbBackground;
            frm.rtbTotalProcessTime.ForeColor = frm.clrNormalText;
            frm.rtbTotalProcessTime.BorderStyle = BorderStyle.None;
            frm.rtbTotalProcessTime.ReadOnly = true;
            frm.rtbTotalProcessTime.Cursor = Cursors.Default;
            frm.rtbTotalProcessTime.TabStop = false;
            frm.Controls.Add(frm.rtbTotalProcessTime);

            frm.rtbNewHandDetected = new RichTextBox();
            frm.rtbNewHandDetected.Anchor = AnchorStyles.None;
            frm.rtbNewHandDetected.BackColor = frm.clrRtbBackground;
            frm.rtbNewHandDetected.ForeColor = frm.clrDataId;
            frm.rtbNewHandDetected.BorderStyle = BorderStyle.None;
            frm.rtbNewHandDetected.ReadOnly = true;
            frm.rtbNewHandDetected.Cursor = Cursors.Default;
            frm.rtbNewHandDetected.TabStop = false;
            frm.rtbNewHandDetected.Text = "New Hand Detected";
            frm.rtbNewHandDetected.Visible = false;
            frm.Controls.Add(frm.rtbNewHandDetected);

            #endregion

            #region DataGridView

            frm.dgvPlayers = new DataGridView();
            frm.dgvPlayers.Anchor = AnchorStyles.None;
            frm.dgvPlayers.DefaultCellStyle.BackColor = frm.clrRtbBackground;
            //frm.dgvPlayers.AlternatingRowsDefaultCellStyle.BackColor = Color.DarkGray;
            frm.dgvPlayers.ForeColor = frm.clrNormalText;
            frm.dgvPlayers.BorderStyle = BorderStyle.None;
            frm.dgvPlayers.TabStop = false;
            frm.dgvPlayers.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            frm.dgvPlayers.ColumnHeadersVisible = false;
            frm.dgvPlayers.RowHeadersVisible = false;
            frm.dgvPlayers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            frm.dgvPlayers.ReadOnly = true;
            frm.dgvPlayers.AllowDrop = false;
            frm.dgvPlayers.AllowUserToAddRows = false;
            frm.dgvPlayers.AllowUserToDeleteRows = false;
            frm.dgvPlayers.AllowUserToOrderColumns = false;
            frm.dgvPlayers.AllowUserToResizeColumns = false;
            frm.dgvPlayers.AllowUserToResizeRows = false;
            frm.dgvPlayers.RowCount = 10;
            frm.dgvPlayers.ColumnCount = 4;
            frm.dgvPlayers.Columns[0].Width = 31;
            frm.dgvPlayers.Columns[1].Width = 31;
            frm.dgvPlayers.Columns[2].Width = 182;
            frm.dgvPlayers.Columns[3].Width = 97;
            frm.dgvPlayers.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            frm.dgvPlayers.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            frm.dgvPlayers.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            frm.dgvPlayers.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            frm.Controls.Add(frm.dgvPlayers);
            frm.dgvPlayers.ClearSelection();

            frm.dgvTimers = new DataGridView();
            frm.dgvTimers.Anchor = AnchorStyles.None;
            frm.dgvTimers.DefaultCellStyle.BackColor = frm.clrRtbBackground;
            //frm.dgvTimers.AlternatingRowsDefaultCellStyle.BackColor = Color.DarkGray;
            frm.dgvTimers.ForeColor = frm.clrNormalText;
            frm.dgvTimers.BorderStyle = BorderStyle.None;
            frm.dgvTimers.TabStop = false;
            frm.dgvTimers.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            frm.dgvTimers.ColumnHeadersVisible = false;
            frm.dgvTimers.RowHeadersVisible = false;
            frm.dgvTimers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            frm.dgvTimers.ReadOnly = true;
            frm.dgvTimers.AllowDrop = false;
            frm.dgvTimers.AllowUserToAddRows = false;
            frm.dgvTimers.AllowUserToDeleteRows = false;
            frm.dgvTimers.AllowUserToOrderColumns = false;
            frm.dgvTimers.AllowUserToResizeColumns = false;
            frm.dgvTimers.AllowUserToResizeRows = false;
            frm.dgvTimers.RowCount = 9;
            frm.dgvTimers.ColumnCount = 18;
            frm.dgvTimers.Columns[0].Width = 31;
            frm.dgvTimers.Columns[1].Width = 43;
            frm.dgvTimers.Columns[2].Width = 27;
            frm.dgvTimers.Columns[3].Width = 27;
            frm.dgvTimers.Columns[4].Width = 31;
            frm.dgvTimers.Columns[5].Width = 43;
            frm.dgvTimers.Columns[6].Width = 27;
            frm.dgvTimers.Columns[7].Width = 27;
            frm.dgvTimers.Columns[8].Width = 31;
            frm.dgvTimers.Columns[9].Width = 43;
            frm.dgvTimers.Columns[10].Width = 43;
            frm.dgvTimers.Columns[11].Width = 27;
            frm.dgvTimers.Columns[12].Width = 27;
            frm.dgvTimers.Columns[13].Width = 31;
            frm.dgvTimers.Columns[14].Width = 43;
            frm.dgvTimers.Columns[15].Width = 43;
            frm.dgvTimers.Columns[16].Width = 27;
            frm.dgvTimers.Columns[17].Width = 27;
            frm.dgvTimers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            frm.dgvTimers.Columns[3].DividerWidth = 4;
            frm.dgvTimers.Columns[7].DividerWidth = 4;
            frm.dgvTimers.Columns[12].DividerWidth = 4;
            frm.Controls.Add(frm.dgvTimers);
            frm.dgvTimers.ClearSelection();

            #endregion

            #region PictureBox

            frm.picScreenShot = new PictureBox();
            frm.picScreenShot.Anchor = AnchorStyles.Top;
            frm.picScreenShot.Dock = DockStyle.Fill;
            frm.picScreenShot.SizeMode = PictureBoxSizeMode.Normal;
            frm.Controls.Add(frm.picScreenShot);
            frm.picScreenShot.MouseClick += new MouseEventHandler(frm.picScreenShot_MouseClick);

            frm.picMyHand = new PictureBox();
            frm.picMyHand.SizeMode = PictureBoxSizeMode.Normal;
            frm.Controls.Add(frm.picMyHand);

            #endregion;

            #region Fonts

            // Set Font
            Font fntRtbs = frm.rtbPotDealerAction.SelectionFont;
            frm.rtbPotDealerAction.Font = new Font(fntRtbs.FontFamily, 12, FontStyle.Bold);
            frm.rtbStatus.Font = new Font(fntRtbs.FontFamily, 12, FontStyle.Bold);
            frm.rtbBoard.Font = new Font(fntRtbs.FontFamily, 12, FontStyle.Bold);
            frm.rtbUiIdleTime.Font = new Font(fntRtbs.FontFamily, 10, FontStyle.Bold);
            frm.rtbTotalProcessTime.Font = new Font(fntRtbs.FontFamily, 10, FontStyle.Bold);
            frm.rtbNewHandDetected.Font = new Font(fntRtbs.FontFamily, 10, FontStyle.Bold);
            frm.dgvPlayers.Font = new Font(fntRtbs.FontFamily, 8, FontStyle.Bold);
            frm.dgvTimers.Font = new Font(fntRtbs.FontFamily, 8, FontStyle.Bold);

            #endregion
        }

        private void SetControlLocations()
        {
            int intXLocationLeft, intXLocationRight, intGrpIndentX, intGrpIndentY, intGrpSpacingY, intRtbWidth, intRtbWidthBoard, intRtbWidthDgvPlayers;
            Size szeGrpBasicSettings, szeGrpProgramControl, szeGrpBitmapLocationTools, szeGrpErrorLog, szeButton, szeTextBox, szeRtbDealerAction, 
                szeRtbStatus, szeRtbBoard, szeDgvPlayers, szeDgvTimers,szeRtbUiIdleTime, szeRtbTotalProcessTime, szeRtbNewHandDetected, 
                szeTxtErrorMessages;

            intXLocationLeft = 10;
            intXLocationRight = 1677;
            intGrpIndentX = 12;
            intGrpIndentY = 18;
            intGrpSpacingY = 8;
            intRtbWidth = 238;
            intRtbWidthBoard = 256;
            intRtbWidthDgvPlayers = 342;
            szeGrpBasicSettings = new Size(200, 209);
            szeGrpProgramControl = new Size(200, 163);
            szeGrpBitmapLocationTools = new Size(200, 274);
            szeGrpErrorLog = new Size(262, 130);
            szeButton = new Size(155, 35);
            szeTextBox = new Size(75, 25);
            szeRtbDealerAction = new Size(intRtbWidth, 63);
            szeRtbStatus = new Size(intRtbWidth, 585);
            szeRtbBoard = new Size(intRtbWidthBoard, 42);
            szeRtbUiIdleTime = new Size(136, 19);
            szeRtbTotalProcessTime = new Size(209, 19);
            szeRtbNewHandDetected = new Size(142, 19);
            szeDgvPlayers = new Size(intRtbWidthDgvPlayers, 221);
            szeDgvTimers = new Size(599, 199);
            szeTxtErrorMessages = new Size(239, 104);

            frm.grpBasicSettings.Size = szeGrpBasicSettings;
            frm.grpProgramControl.Size = szeGrpProgramControl;
            frm.grpBitmapLocationTools.Size = szeGrpBitmapLocationTools;
            frm.grpErrorLogging.Size = szeGrpErrorLog;
            frm.btnChangeBitmapSaveLocation.Size = szeButton;
            frm.btnClearAllDbData.Size = szeButton;
            frm.btnStartPokerCompanion.Size = szeButton;
            frm.btnOpenScreenShotFile.Size = szeButton;
            frm.btnNextScreenShot.Size = szeButton;
            frm.btnCopyBitmapsForWorkers.Size = szeButton;
            frm.btnDrawRect.Size = szeButton;
            frm.btnDrawAllRects.Size = szeButton;
            frm.lblPlayerOfInterest.AutoSize = true;
            frm.lblRectX.AutoSize = true;
            frm.lblRectY.AutoSize = true;
            frm.lblRectHeight.AutoSize = true;
            frm.lblRectWidth.AutoSize = true;
            frm.lblMouseClickX.AutoSize = true;
            frm.lblMouseClickY.AutoSize = true;
            frm.lblMouseClickClr.AutoSize = true;
            frm.lblMouseClickBright.AutoSize = true;
            frm.chkClearRects.AutoSize = true;
            frm.chkSaveBitmaps.AutoSize = true;
            frm.chkAutoNextScreenShot.AutoSize = true;
            frm.chkShowClickData.AutoSize = true;
            frm.rdoModeScreenshotMode.AutoSize = true;
            frm.rdoModeLiveGame.AutoSize = true;
            frm.rdo9PlayerTable.AutoSize = true;
            frm.rdo10PlayerTable.AutoSize = true;
            frm.txtPlayerOfInterest.Size = new Size(szeButton.Width, szeTextBox.Height);
            frm.txtRectX.Size = szeTextBox;
            frm.txtRectY.Size = szeTextBox;
            frm.txtRectHeight.Size = szeTextBox;
            frm.txtRectWidth.Size = szeTextBox;
            frm.txtErrorMessages.Size = szeTxtErrorMessages;

            #region Left Column

            #region Basic Settings GroupBox

            frm.grpBasicSettings.Location=new Point(intXLocationLeft, 150);

            frm.rdoModeScreenshotMode.Location = new Point(intGrpIndentX, intGrpIndentY);
            frm.rdoModeLiveGame.Location = new Point(intGrpIndentX + 100, intGrpIndentY);
            frm.lblPlayerOfInterest.Location = new Point(intGrpIndentX, frm.rdoModeScreenshotMode.Location.Y + frm.rdoModeScreenshotMode.Height + 5);
            frm.txtPlayerOfInterest.Location = new Point(intGrpIndentX, frm.lblPlayerOfInterest.Location.Y + frm.lblPlayerOfInterest.Height + 2);
            frm.chkSaveBitmaps.Location = new Point(intGrpIndentX, frm.txtPlayerOfInterest.Location.Y + frm.txtPlayerOfInterest.Height + 10);
            frm.btnChangeBitmapSaveLocation.Location = new Point(intGrpIndentX, frm.chkSaveBitmaps.Location.Y + frm.chkSaveBitmaps.Height + 10);
            // frm.rdo9PlayerTable.Location = new Point(intGrpIndentX, intGrpIndentY);
            // frm.rdo10PlayerTable.Location = new Point(intGrpIndentX + 80, intGrpIndentY);
            frm.btnClearAllDbData.Location = new Point(intGrpIndentX, frm.btnChangeBitmapSaveLocation.Location.Y + frm.btnChangeBitmapSaveLocation.Height + 3);

            #endregion

            #region Program Control GroupBox

            frm.grpProgramControl.Location = new Point(intXLocationLeft, frm.grpBasicSettings.Location.Y + frm.grpBasicSettings.Height + intGrpSpacingY);

            frm.btnStartPokerCompanion.Location = new Point(intGrpIndentX, intGrpIndentY + 1);
            frm.btnOpenScreenShotFile.Location = new Point(intGrpIndentX, frm.btnStartPokerCompanion.Location.Y + frm.btnStartPokerCompanion.Height + 3);
            frm.chkAutoNextScreenShot.Location = new Point(intGrpIndentX, frm.btnOpenScreenShotFile.Location.Y + frm.btnOpenScreenShotFile.Height + 5);
            frm.btnNextScreenShot.Location = new Point(intGrpIndentX, frm.chkAutoNextScreenShot.Location.Y + frm.chkAutoNextScreenShot.Height + 3);
            // frm.btnCopyBitmapsForWorkers.Location = new Point(intGrpIndentX, frm.btnNextScreenShot.Location.Y + frm.btnNextScreenShot.Height + 5);

            #endregion

            #region Bitmap Location Tools GroupBox

            frm.grpBitmapLocationTools.Location=new Point(intXLocationLeft, frm.grpProgramControl.Location.Y + frm.grpProgramControl.Height + intGrpSpacingY);

            // frm.chkClearRects.Location = new Point(intGrpIndentX, intGrpIndentY + 1);
            frm.lblRectX.Location = new Point(intXLocationLeft, intGrpIndentY + 1);
            frm.lblRectY.Location = new Point(intXLocationLeft + szeTextBox.Width + 5, frm.lblRectX.Location.Y);
            frm.txtRectX.Location = new Point(intXLocationLeft, frm.lblRectX.Location.Y + frm.lblRectX.Height + 2);
            frm.txtRectY.Location = new Point(intXLocationLeft + szeTextBox.Width + 5, frm.lblRectX.Location.Y + frm.lblRectX.Height + 2);
            frm.lblRectWidth.Location = new Point(intXLocationLeft, frm.txtRectX.Location.Y + frm.txtRectX.Height + 5);
            frm.lblRectHeight.Location = new Point(intXLocationLeft + szeTextBox.Width + 5, frm.txtRectX.Location.Y + frm.txtRectX.Height + 5);
            frm.txtRectWidth.Location = new Point(intXLocationLeft, frm.lblRectWidth.Location.Y + frm.lblRectWidth.Height + 2);
            frm.txtRectHeight.Location = new Point(intXLocationLeft + szeTextBox.Width + 5, frm.lblRectWidth.Location.Y + frm.lblRectWidth.Height + 2);
            frm.btnDrawRect.Location = new Point(intXLocationLeft, frm.txtRectWidth.Location.Y + frm.txtRectWidth.Height + 5);
            frm.btnDrawAllRects.Location = new Point(intXLocationLeft, frm.btnDrawRect.Location.Y + frm.btnDrawRect.Height + 5);
            frm.chkShowClickData.Location = new Point(intXLocationLeft, frm.btnDrawAllRects.Location.Y + frm.btnDrawAllRects.Height + 8);
            frm.lblMouseClickX.Location = new Point(intXLocationLeft, frm.chkShowClickData.Location.Y + frm.chkShowClickData.Height + 5);
            frm.lblMouseClickY.Location = new Point(intXLocationLeft, frm.lblMouseClickX.Location.Y + frm.lblMouseClickX.Height + 2);
            frm.lblMouseClickClr.Location = new Point(intXLocationLeft, frm.lblMouseClickY.Location.Y + frm.lblMouseClickY.Height + 2);
            frm.lblMouseClickBright.Location = new Point(intXLocationLeft, frm.lblMouseClickClr.Location.Y + frm.lblMouseClickClr.Height + 2);

            #endregion

            #region Error Logging GroupBox

            frm.grpErrorLogging.Location=new Point(frm.grpBitmapLocationTools.Location.X + frm.grpBitmapLocationTools.Width + intXLocationLeft, frm.grpBitmapLocationTools.Location.Y + frm.grpBitmapLocationTools.Height - frm.grpErrorLogging.Height);
            frm.txtErrorMessages.Location = new Point(intGrpIndentX, intGrpIndentY);

            #endregion

            #endregion

            #region Right Column

            frm.rtbPotDealerAction.Size = szeRtbDealerAction;
            frm.rtbStatus.Size = szeRtbStatus;
            frm.rtbBoard.Size = szeRtbBoard;
            frm.dgvPlayers.Size = szeDgvPlayers;

            frm.rtbPotDealerAction.Location = new Point(intXLocationRight, 80);
            frm.rtbStatus.Location = new Point(intXLocationRight, frm.rtbPotDealerAction.Location.Y + frm.rtbPotDealerAction.Height + 8);
            frm.rtbBoard.Location = new Point(intXLocationRight - intRtbWidthBoard + intRtbWidth, frm.rtbStatus.Location.Y + frm.rtbStatus.Height + 8);
            frm.dgvPlayers.Location = new Point(intXLocationRight - intRtbWidthDgvPlayers + intRtbWidth, frm.rtbBoard.Location.Y + frm.rtbBoard.Height + 8);

            #endregion

            #region Timers Area

            frm.rtbUiIdleTime.Size = szeRtbUiIdleTime;
            frm.rtbTotalProcessTime.Size = szeRtbTotalProcessTime;
            frm.rtbNewHandDetected.Size = szeRtbNewHandDetected;
            frm.dgvTimers.Size = szeDgvTimers;

            frm.rtbUiIdleTime.Location = new Point(1213, 793);
            frm.rtbTotalProcessTime.Location = new Point(1355, 793);
            frm.rtbNewHandDetected.Location = new Point(1347, 770);
            frm.dgvTimers.Location = new Point(965, 816);

            SetRtbDefaultText();
            SetDgvDefaultText();

            #endregion

            #region Active player hand picturebox

            frm.picMyHand.Location = new Point(1497, 617);

            #endregion
        }

        private void SetRtbDefaultText()
        {
            frm.SetColoredRtbText(frm.rtbPotDealerAction, frm.clrDataId, "Pot: ");
            frm.SetColoredChipCountRtbText(frm.rtbPotDealerAction, frm.clrNormalText, "\t0\n", false);
            frm.SetColoredRtbText(frm.rtbPotDealerAction, frm.clrDataId, "Dealer: ");
            frm.rtbPotDealerAction.AppendText("\tPlayer 01\n");
            frm.SetColoredRtbText(frm.rtbPotDealerAction, frm.clrDataId, "Action Player: ");
            frm.rtbPotDealerAction.AppendText("\tPlayer 01");

            frm.SetColoredRtbText(frm.rtbBoard, frm.clrDataId, "F: ");
            frm.rtbBoard.AppendText(" - -        ");
            frm.SetColoredRtbText(frm.rtbBoard, frm.clrDataId, "\tF: ");
            frm.rtbBoard.AppendText(" - -        ");
            frm.SetColoredRtbText(frm.rtbBoard, frm.clrDataId, "\tF: ");
            frm.rtbBoard.AppendText(" - - \n");
            frm.SetColoredRtbText(frm.rtbBoard, frm.clrDataId, "\tT: ");
            frm.rtbBoard.AppendText(" - -        ");
            frm.SetColoredRtbText(frm.rtbBoard, frm.clrDataId, "\tR: ");
            frm.rtbBoard.AppendText(" - - ");

            frm.SetColoredRtbText(frm.rtbUiIdleTime, frm.clrDataId, "UI Idle Time: ");
            frm.rtbUiIdleTime.AppendText("0.000");

            frm.SetColoredRtbText(frm.rtbTotalProcessTime, frm.clrDataId, "Total Processing Time: ");
            frm.rtbTotalProcessTime.AppendText("0.000");
        }

        private void SetDgvDefaultText()
        { 
            for (int i = 0; i < 10; i++)
            {
                frm.dgvPlayers.Rows[i].Cells[0].Value = " - - ";
                frm.dgvPlayers.Rows[i].Cells[1].Value = " - - ";
                frm.dgvPlayers.Rows[i].Cells[2].Value = "Player " + (i + 1).ToString("D2");
                frm.dgvPlayers.Rows[i].Cells[3].Value = "0";
            }

            for (int i = 0; i < 9; i++)
            {
                frm.dgvTimers.Rows[i].Cells[0].Style = new DataGridViewCellStyle { ForeColor = frm.clrDataId };
                frm.dgvTimers.Rows[i].Cells[0].Value = "c" + (i + 1).ToString();
                frm.dgvTimers.Rows[i].Cells[1].Value = "0.000";
                frm.dgvTimers.Rows[i].Cells[2].Value = (i + 1).ToString();
                frm.dgvTimers.Rows[i].Cells[3].Value = (i + 1).ToString();
                frm.dgvTimers.Rows[i].Cells[4].Style = new DataGridViewCellStyle { ForeColor = frm.clrDataId };
                if (i != 8)
                {
                    frm.dgvTimers.Rows[i].Cells[4].Value = "c" + (i + 10).ToString();
                    frm.dgvTimers.Rows[i].Cells[5].Value = "0.000";
                    frm.dgvTimers.Rows[i].Cells[6].Value = (i + 10).ToString();
                    frm.dgvTimers.Rows[i].Cells[7].Value = (i + 10).ToString();
                }
                else
                {
                    frm.dgvTimers.Rows[i].Cells[4].Value = "SS.";
                    frm.dgvTimers.Rows[i].Cells[5].Value = "0.000";
                    frm.dgvTimers.Rows[i].Cells[6].Value = "-";
                    frm.dgvTimers.Rows[i].Cells[7].Value = "-";
                }
                frm.dgvTimers.Rows[i].Cells[8].Style = new DataGridViewCellStyle { ForeColor = frm.clrDataId };
                frm.dgvTimers.Rows[i].Cells[8].Value = "w" + (i + 1).ToString();
                frm.dgvTimers.Rows[i].Cells[9].Value = "0.000";
                frm.dgvTimers.Rows[i].Cells[10].Value = "0.000";
                frm.dgvTimers.Rows[i].Cells[11].Value = (i + 18).ToString();
                frm.dgvTimers.Rows[i].Cells[12].Value = (i + 18).ToString();
                frm.dgvTimers.Rows[i].Cells[13].Style = new DataGridViewCellStyle { ForeColor = frm.clrDataId };
                if (i != 8)
                {
                    frm.dgvTimers.Rows[i].Cells[13].Value = "w" + (i + 10).ToString();
                    frm.dgvTimers.Rows[i].Cells[14].Value = "0.000";
                    frm.dgvTimers.Rows[i].Cells[15].Value = "0.000";
                    frm.dgvTimers.Rows[i].Cells[16].Value = (i + 27).ToString();
                    frm.dgvTimers.Rows[i].Cells[17].Value = (i + 27).ToString();
                }
                else
                {
                    frm.dgvTimers.Rows[i].Cells[13].Value = "UI";
                    frm.dgvTimers.Rows[i].Cells[14].Value = "-";
                    frm.dgvTimers.Rows[i].Cells[15].Value = "0.000";
                    frm.dgvTimers.Rows[i].Cells[16].Value = "-";
                    frm.dgvTimers.Rows[i].Cells[17].Value = "-";
                }
            }
        }
    }
}
