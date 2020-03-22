using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BOL_Companion
{
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
            frm.Location = Screen.AllScreens[3].WorkingArea.Location;
            frm.WindowState = FormWindowState.Maximized;
            frm.Text = "Adam's Bet Online Poker Companion";
        }

        private void InitializeControls()
        {
            #region Labels

            frm.lblRectX = new Label();
            frm.lblRectX.Anchor = AnchorStyles.None;
            frm.lblRectX.Text = "X Coordinate";
            frm.Controls.Add(frm.lblRectX);

            frm.lblRectY = new Label();
            frm.lblRectY.Anchor = AnchorStyles.None;
            frm.lblRectY.Text = "Y Coordinate";
            frm.Controls.Add(frm.lblRectY);

            frm.lblRectWidth = new Label();
            frm.lblRectWidth.Anchor = AnchorStyles.None;
            frm.lblRectWidth.Text = "Width";
            frm.Controls.Add(frm.lblRectWidth);

            frm.lblRectHeight = new Label();
            frm.lblRectHeight.Anchor = AnchorStyles.None;
            frm.lblRectHeight.Text = "Height";
            frm.Controls.Add(frm.lblRectHeight);

            frm.lblMouseClickX = new Label();
            frm.lblMouseClickX.Anchor = AnchorStyles.None;
            frm.lblMouseClickX.Text = frm.strMouseLabelX + "N/A";
            frm.Controls.Add(frm.lblMouseClickX);

            frm.lblMouseClickY = new Label();
            frm.lblMouseClickY.Anchor = AnchorStyles.None;
            frm.lblMouseClickY.Text = frm.strMouseLabelY + "N/A";
            frm.Controls.Add(frm.lblMouseClickY);

            frm.lblMouseClickClr = new Label();
            frm.lblMouseClickClr.Anchor = AnchorStyles.None;
            frm.lblMouseClickClr.Text = frm.strMouseLabelClr + "N/A";
            frm.Controls.Add(frm.lblMouseClickClr);

            frm.lblMouseClickBright = new Label();
            frm.lblMouseClickBright.Anchor = AnchorStyles.None;
            frm.lblMouseClickBright.Text = frm.strMouseLabelBright + "N/A";
            frm.Controls.Add(frm.lblMouseClickBright);

            #endregion

            #region CheckBoxes

            frm.chkClearRects = new CheckBox();
            frm.chkClearRects.Anchor = AnchorStyles.None;
            frm.chkClearRects.Text = "Clear Rectangles";
            frm.chkClearRects.Checked = false;
            frm.Controls.Add(frm.chkClearRects);

            frm.chkSaveBitmaps = new CheckBox();
            frm.chkSaveBitmaps.Anchor = AnchorStyles.None;
            frm.chkSaveBitmaps.Text = "Save Bitmap Files";
            frm.chkSaveBitmaps.Checked = false;
            frm.Controls.Add(frm.chkSaveBitmaps);

            frm.chkAutoNextScreenShot = new CheckBox();
            frm.chkAutoNextScreenShot.Anchor = AnchorStyles.None;
            frm.chkAutoNextScreenShot.Text = "Auto Continue to Next";
            frm.chkAutoNextScreenShot.Checked = false;
            frm.chkAutoNextScreenShot.Enabled = false;
            frm.Controls.Add(frm.chkAutoNextScreenShot);

            frm.chkShowClickData = new CheckBox();
            frm.chkShowClickData.Anchor = AnchorStyles.None;
            frm.chkShowClickData.Text = "Show Mouse Click Data";
            frm.chkShowClickData.Checked = false;
            frm.Controls.Add(frm.chkShowClickData);
            frm.chkShowClickData.CheckedChanged += new EventHandler(frm.chkShowClickData_CheckChanged);

            #endregion

            #region RadioButtons

            frm.rdo9PlayerTable = new RadioButton();
            frm.rdo9PlayerTable.Anchor = AnchorStyles.None;
            frm.rdo9PlayerTable.Text = "9-Player\nTable";
            frm.rdo9PlayerTable.Checked = true;
            frm.Controls.Add(frm.rdo9PlayerTable);
            frm.rdo9PlayerTable.CheckedChanged += new EventHandler(frm.rdo9PlayerTable_CheckChanged);

            frm.rdo10PlayerTable = new RadioButton();
            frm.rdo10PlayerTable.Anchor = AnchorStyles.None;
            frm.rdo10PlayerTable.Text = "10-Player\nTable";
            frm.Controls.Add(frm.rdo10PlayerTable);
            frm.rdo10PlayerTable.CheckedChanged += new EventHandler(frm.rdo10PlayerTable_CheckChanged);

            #endregion

            #region TextBoxes

            frm.txtRectX = new TextBox();
            frm.txtRectX.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.txtRectX);

            frm.txtRectY = new TextBox();
            frm.txtRectY.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.txtRectY);

            frm.txtRectWidth = new TextBox();
            frm.txtRectWidth.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.txtRectWidth);

            frm.txtRectHeight = new TextBox();
            frm.txtRectHeight.Anchor = AnchorStyles.None;
            frm.Controls.Add(frm.txtRectHeight);

            frm.txtErrorMessages = new TextBox();
            frm.txtErrorMessages.Anchor = AnchorStyles.None;
            frm.txtErrorMessages.Multiline = true;
            frm.txtErrorMessages.ScrollBars = ScrollBars.Vertical;
            frm.txtErrorMessages.WordWrap = true;
            frm.txtErrorMessages.ReadOnly = true;
            frm.txtErrorMessages.TabStop = false;
            frm.Controls.Add(frm.txtErrorMessages);

            #endregion

            #region Buttons

            frm.btnStartPokerCompanion = new Button();
            frm.btnStartPokerCompanion.Anchor = AnchorStyles.None;
            frm.btnStartPokerCompanion.Text = "Start Poker Companion";
            frm.Controls.Add(frm.btnStartPokerCompanion);
            frm.btnStartPokerCompanion.Click += new EventHandler(frm.btnStartPokerCompanion_Click);

            frm.btnOpenScreenShotFile = new Button();
            frm.btnOpenScreenShotFile.Anchor = AnchorStyles.None;
            frm.btnOpenScreenShotFile.Text = "Open ScreenShot";
            frm.Controls.Add(frm.btnOpenScreenShotFile);
            frm.btnOpenScreenShotFile.Click += new EventHandler(frm.btnOpenScreenShotFile_Click);

            frm.btnNextScreenShot = new Button();
            frm.btnNextScreenShot.Anchor = AnchorStyles.None;
            frm.btnNextScreenShot.Text = "Next Sceen Shot";
            frm.btnNextScreenShot.Enabled = false;
            frm.Controls.Add(frm.btnNextScreenShot);
            frm.btnNextScreenShot.Click += new EventHandler(frm.btnNextScreenShot_Click);

            frm.btnCopyBitmapsForWorkers = new Button();
            frm.btnCopyBitmapsForWorkers.Anchor = AnchorStyles.None;
            frm.btnCopyBitmapsForWorkers.Text = "Copy Bitmaps for Workers";
            frm.Controls.Add(frm.btnCopyBitmapsForWorkers);
            frm.btnCopyBitmapsForWorkers.Click += new EventHandler(frm.btnCopyBitmapsForWorkers_Click);

            frm.btnDrawRect = new Button();
            frm.btnDrawRect.Anchor = AnchorStyles.None;
            frm.btnDrawRect.Text = "Draw Rectangle";
            frm.Controls.Add(frm.btnDrawRect);
            frm.btnDrawRect.Click += new EventHandler(frm.btnDrawRect_Click);

            frm.btnDrawAllRects = new Button();
            frm.btnDrawAllRects.Anchor = AnchorStyles.None;
            frm.btnDrawAllRects.Text = "Draw All Rectangles";
            frm.Controls.Add(frm.btnDrawAllRects);
            frm.btnDrawAllRects.Click += new EventHandler(frm.btnDrawAllRects_Click);

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
            frm.picMyHand.Location = new Point(1531, 641);
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
            int intXLocationLeft, intXLocationRight, intRtbWidth;
            Size szeButton, szeTextBox, szeRtbDealerAction, szeRtbStatus, szeRtbBoard, szeDgvPlayers, szeDgvTimers,
                szeRtbUiIdleTime, szeRtbTotalProcessTime, szeRtbNewHandDetected, szeTxtErrorMessages;

            intXLocationLeft = 10;
            intXLocationRight = 1652;
            intRtbWidth = 256;
            szeButton = new Size(155, 35);
            szeTextBox = new Size(75, 25);
            szeRtbDealerAction = new Size(intRtbWidth, 63);
            szeRtbStatus = new Size(intRtbWidth, 585);
            szeRtbBoard = new Size(intRtbWidth, 42);
            szeRtbUiIdleTime = new Size(136, 19);
            szeRtbTotalProcessTime = new Size(209, 19);
            szeRtbNewHandDetected = new Size(142, 19);
            szeDgvPlayers = new Size(342, 221);
            szeDgvTimers = new Size(599, 199);
            szeTxtErrorMessages = new Size(300, 69);

            frm.btnStartPokerCompanion.Size = szeButton;
            frm.btnOpenScreenShotFile.Size = szeButton;
            frm.btnNextScreenShot.Size = szeButton;
            frm.btnCopyBitmapsForWorkers.Size = szeButton;
            frm.btnDrawRect.Size = szeButton;
            frm.btnDrawAllRects.Size = szeButton;
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
            frm.rdo9PlayerTable.AutoSize = true;
            frm.rdo10PlayerTable.AutoSize = true;
            frm.txtRectX.Size = szeTextBox;
            frm.txtRectY.Size = szeTextBox;
            frm.txtRectHeight.Size = szeTextBox;
            frm.txtRectWidth.Size = szeTextBox;
            frm.txtErrorMessages.Size = szeTxtErrorMessages;

            // Left Column
            frm.rdo9PlayerTable.Location = new Point(intXLocationLeft, 159);
            frm.rdo10PlayerTable.Location = new Point(intXLocationLeft + 80, 159);

            frm.btnStartPokerCompanion.Location = new Point(intXLocationLeft, frm.rdo9PlayerTable.Location.Y + frm.rdo9PlayerTable.Height + 3);

            frm.chkSaveBitmaps.Location = new Point(intXLocationLeft, frm.btnStartPokerCompanion.Location.Y + frm.btnStartPokerCompanion.Height + 4);

            frm.btnOpenScreenShotFile.Location = new Point(intXLocationLeft, frm.chkSaveBitmaps.Location.Y + frm.chkSaveBitmaps.Height + 3);

            frm.chkAutoNextScreenShot.Location = new Point(intXLocationLeft, frm.btnOpenScreenShotFile.Location.Y + frm.btnOpenScreenShotFile.Height + 5);

            frm.btnNextScreenShot.Location = new Point(intXLocationLeft, frm.chkAutoNextScreenShot.Location.Y + frm.chkAutoNextScreenShot.Height + 3);
            frm.btnCopyBitmapsForWorkers.Location = new Point(intXLocationLeft, frm.btnNextScreenShot.Location.Y + frm.btnNextScreenShot.Height + 5);

            frm.chkClearRects.Location = new Point(intXLocationLeft, frm.btnCopyBitmapsForWorkers.Location.Y + frm.btnCopyBitmapsForWorkers.Height + 8);

            frm.lblRectX.Location = new Point(intXLocationLeft, frm.chkClearRects.Location.Y + frm.chkClearRects.Height + 5);
            frm.lblRectY.Location = new Point(intXLocationLeft + szeTextBox.Width + 5, frm.chkClearRects.Location.Y + frm.chkClearRects.Height + 5);

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

            frm.txtErrorMessages.Location = new Point(intXLocationLeft, frm.lblMouseClickBright.Location.Y + frm.lblMouseClickBright.Height + 6);

            // Right Column
            frm.rtbPotDealerAction.Size = szeRtbDealerAction;
            frm.rtbStatus.Size = szeRtbStatus;
            frm.rtbBoard.Size = szeRtbBoard;
            frm.dgvPlayers.Size = szeDgvPlayers;

            frm.rtbPotDealerAction.Location = new Point(intXLocationRight, 80);
            frm.rtbStatus.Location = new Point(intXLocationRight, frm.rtbPotDealerAction.Location.Y + frm.rtbPotDealerAction.Height + 8);
            frm.rtbBoard.Location = new Point(intXLocationRight, frm.rtbStatus.Location.Y + frm.rtbStatus.Height + 8);
            frm.dgvPlayers.Location = new Point(intXLocationRight - 85, frm.rtbBoard.Location.Y + frm.rtbBoard.Height + 8);

            // Timers Area
            frm.rtbUiIdleTime.Size = szeRtbUiIdleTime;
            frm.rtbTotalProcessTime.Size = szeRtbTotalProcessTime;
            frm.rtbNewHandDetected.Size = szeRtbNewHandDetected;
            frm.dgvTimers.Size = szeDgvTimers;

            frm.rtbUiIdleTime.Location = new Point(1208, 793);
            frm.rtbTotalProcessTime.Location = new Point(1350, 793);
            frm.rtbNewHandDetected.Location = new Point(1417, 770);
            frm.dgvTimers.Location = new Point(960, 816);

            SetRtbDefaultText();
            SetDgvDefaultText();
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
