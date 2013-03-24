using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using FourDO.Resources;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class JohnnyInputSettings : Form
	{
		private enum GridColumn
		{
			InputButton = 0,
			ButtonName 
			// The rest are dynamic.
		}

		private bool isEditingButton = false;
		private DataGridViewCell editedCell = null;
		private bool isSettingAll = false;

		private int deviceNumber = 1;

		private readonly JoyInputWatcher joyWatcher = new JoyInputWatcher();
		private readonly JoyInputChecker joyChecker = new JoyInputChecker();

		private InputBindingDevices devices;

		DataGridViewCellStyle linkStyleNormal;
		DataGridViewCellStyle linkStyleBold;
		DataGridViewCellStyle linkStyleBoldWithGrayBack;
		DataGridViewCellStyle editingStyle;
		DataGridViewCellStyle tintedStyle;
		DataGridViewCellStyle disabledStyle;

		[DllImport("user32.dll")]
		private static extern bool LockWindowUpdate(IntPtr hWndLock);

		public JohnnyInputSettings(InputBindingDevices originalDeviceBindings, string bindingsFilePath)
		{
			this.devices = (InputBindingDevices)originalDeviceBindings.Clone();
			this.EnsureMinimumDevices();

			this.BindingsFilePath = bindingsFilePath;

			this.DialogResult = DialogResult.Cancel;

			this.InitializeComponent();
		}

		public string BindingsFilePath { get; private set; }

		#region Event Handlers

		private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.deviceNumber = this.MainTabControl.SelectedIndex;
			this.UpdateUI();
		}

		private void JohnnyInputSettings_Load(object sender, EventArgs e)
		{
			this.Localize();

			this.InitializeGrid();

			// Position controller panel over the tab control.
			var newBounds = this.Player1Tab.Bounds;
			newBounds.X += this.MainTabControl.Left + 3;
			newBounds.Y += this.MainTabControl.Top + 4;
			newBounds.Height -= 7;
			newBounds.Width -= 9;
			this.ControllerPanel.SetBounds(newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height);

			// Move devices label into position.
			this.CurrentDevicesLabel.Bounds = this.controllerPreview.Bounds;
			this.CurrentDevicesLabel.BringToFront();
			this.CurrentDevicesLabel.Visible = false;

			// Select the first tab.
			this.MainTabControl.SelectedIndex = 1;

			this.UpdateUI();

			this.UpdatePreviewBasedOnGrid();
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			bool savedSuccessfully = false;

			try
			{
				this.devices.SaveToFile(this.BindingsFilePath);
				savedSuccessfully = true;
			}
			catch (Exception ex)
			{
				FourDO.UI.Error.ShowError("Unexpected error saving the bindings to file : " + this.BindingsFilePath + "\r\n\r\nDetails: " + ex.ToString());
			}

			if (savedSuccessfully)
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}


		private void controllerInfo_LinkMouseEnter(object sender, EventArgs e)
		{
			this.CurrentDevicesLabel.Text = this.joyWatcher.GetCurrentJoystickList();
			this.CurrentDevicesLabel.Visible = true;
		}

		private void controllerInfo_LinkMouseLeave(object sender, EventArgs e)
		{
			this.CurrentDevicesLabel.Visible = false;
		}

		private JoystickTrigger lastResult = null;
		private void JoystickWatchTimer_Tick(object sender, EventArgs e)
		{
			JoystickTrigger newTrigger = this.joyWatcher.WatchForTrigger();
			bool newTriggerFound = (newTrigger != null && !newTrigger.Equals(lastResult));
			lastResult = newTrigger;

			List<InputButton> glowButtons = null;
			if (this.isEditingButton)
			{
				if (newTriggerFound)
				{
					this.DoStopEditButton(newTrigger);
				}
			}
			else
			{
				// If we're not editing, we can show fun glowy effects!
				this.joyChecker.UpdateValueCache();
				glowButtons = new List<InputButton>();

				InputBindingDevice currentDevice = null;
				if (this.deviceNumber >= 0 && this.devices.Count > this.deviceNumber)
					currentDevice = this.devices[this.deviceNumber];

				if (currentDevice != null)
				{
					var buttons = Enum.GetValues(typeof(InputButton));
					foreach (var buttonEnum in buttons)
					{
						var button = (InputButton)buttonEnum;
						List<InputTrigger> triggers = this.devices.GetTriggers(deviceNumber, button);
						foreach (var trigger in triggers)
						{
							bool triggerHit = false;

							if (trigger is KeyboardInputTrigger)
								triggerHit = JohnnyInputPlugin.IsKeyboardButtonDown(((KeyboardInputTrigger)trigger).Key);
							else if (trigger is JoystickTrigger)
								triggerHit = this.joyChecker.CheckTrigger((JoystickTrigger)trigger);

							if (triggerHit)
							{
								glowButtons.Add(button);
								break;
							}
						}
					}
				}

				if (glowButtons.Count == 0)
					glowButtons = null;
			}
			this.controllerPreview.SetGlowingButtons(glowButtons);
		}

		private void RefreshJoystickListTimer_Tick(object sender, EventArgs e)
		{
			this.joyWatcher.UpdateJoystickListCache();
			this.controllerInfo.DeviceCount = this.joyWatcher.GetCurrentJoystickCount() + 1;
			this.joyChecker.UpdateDeviceCache();
		}

		private void JohnnyInputSettings_KeyDown(object sender, KeyEventArgs e)
		{
			if (this.isEditingButton)
			{
				e.Handled = true;
				if (e.KeyCode == Keys.Escape)
				{
					this.DoStopEditButton();
					return;
				}

				this.DoStopEditButton(new KeyboardInputTrigger(e.KeyCode));
			}
			else
			{
				// We also allow them to clear individual bindings.
				if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
					this.DoClearCellBinding();
			}
		}

		private void ControlsGridView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				e.Handled = true; // I never want the damn enter key to go to the next cell.

			if (this.isEditingButton)
				return;

			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
				this.DoStartEditButton(this.ControlsGridView.CurrentCell);
		}

		private void ControlsGridView_Leave(object sender, EventArgs e)
		{
			if (this.isEditingButton)
				this.DoStopEditButton();
		}

		private static Point lastPoint = new Point(-1, -1);
		private void ControlsGridView_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X == lastPoint.X && e.Y == lastPoint.Y)
				return;

			lastPoint.X = e.X;
			lastPoint.Y = e.Y;
			
			DataGridView.HitTestInfo info = this.ControlsGridView.HitTest(e.X, e.Y);
			DataGridViewCell cell = this.TryGetCell(info.ColumnIndex, info.RowIndex);
			InputButton? highlightedButton = this.FindInputButtonForRow(info.RowIndex);

			// Show a hand cursor over link-styled cells
			if (cell != null && cell.Style.Font != null && cell.Style.Font.Underline)
				this.ControlsGridView.Cursor = Cursors.Hand;
			else
				this.ControlsGridView.Cursor = Cursors.Default;

			// Highlight a specific controller button if the mouse is over a control row.
			this.UpdatePreviewToButton(highlightedButton);
		}

		private void ControlsGridView_MouseLeave(object sender, EventArgs e)
		{
			this.UpdatePreviewBasedOnGrid();
		}

		private void ControlsGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (this.isEditingButton)
				this.DoStopEditButton();
		}

		private void ControlsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			this.ControlsGridView_CellClick(sender, e);
		}

		private void ControlsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridViewCell cell = this.TryGetCell(e.ColumnIndex, e.RowIndex);

			if (cell == null)
				return;

			/////////////////////

			// Was it a link?
			if (cell.Style.Font != null && cell.Style.Font.Underline)
			{
				int setNumber = this.FindSetIndexForColumn(cell.ColumnIndex);

				if ((string)cell.Value == JohnnyInputStrings.DeleteSet)
					this.DoDeleteBinding(setNumber);
				else if ((string)cell.Value == JohnnyInputStrings.AddSet)
					this.DoAddBinding(setNumber);
				else if ((string)cell.Value == JohnnyInputStrings.SetAll)
					this.DoSetAll(setNumber);
				else if ((string)cell.Value == JohnnyInputStrings.ClearAll)
					this.DoClearAll(setNumber);
			}

			// See if they clicked a binding.
			if ((cell.ColumnIndex != this.ControlsGridView.Columns.Count - 1)
				&& cell.ColumnIndex != (int)GridColumn.ButtonName
				&& cell.ColumnIndex != (int)GridColumn.InputButton)
			{
				// They clicked on a binding.
				this.DoStartEditButton(cell);
			}
		}

		private void ControlsGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			this.UpdatePreviewBasedOnGrid();
		}

		private void controllerPreview_MouseHoverButton(MouseHoverButtonEventArgs e)
		{
			if (this.isEditingButton)
				return;
			
			DataGridViewCell newCell = this.FindBestCellForInputButton(e.InputButton);
			if (newCell != null)
				this.ControlsGridView.CurrentCell = newCell;
		}

		private void controllerPreview_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.controllerPreview_MouseClick(sender, e);
		}

		private void controllerPreview_MouseClick(object sender, MouseEventArgs e)
		{
			DataGridViewCell newCell = this.FindBestCellForInputButton(this.controllerPreview.HoveredButton);
			if (newCell != null)
				this.DoStartEditButton(newCell);
		}

		#endregion // Event Handlers

		#region Private Functions

		private void InitializeGrid()
		{
			this.ControlsGridView.Columns.Clear();
			this.ControlsGridView.Rows.Clear();

			this.ControlsGridView.BackgroundColor = SystemColors.Control;

			//////////////////
			// Define styles.
			this.linkStyleNormal = this.ControlsGridView.DefaultCellStyle.Clone();
			this.linkStyleNormal.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.linkStyleNormal.ForeColor = Color.Blue;
			this.linkStyleNormal.Font = new Font(this.linkStyleNormal.Font, FontStyle.Underline);

			this.linkStyleBold = this.linkStyleNormal.Clone();
			this.linkStyleBold.Font = new Font(this.linkStyleBold.Font, FontStyle.Bold | FontStyle.Underline);

			this.linkStyleBoldWithGrayBack = this.linkStyleBold.Clone();
			this.linkStyleBoldWithGrayBack.BackColor = this.ControlsGridView.BackgroundColor;
			this.linkStyleBoldWithGrayBack.Font = new Font(this.linkStyleBoldWithGrayBack.Font, FontStyle.Bold | FontStyle.Underline);

			this.tintedStyle = this.ControlsGridView.DefaultCellStyle.Clone();
			this.tintedStyle.BackColor = SystemColors.Info;

			this.disabledStyle = this.ControlsGridView.DefaultCellStyle.Clone();
			this.disabledStyle.BackColor = this.ControlsGridView.BackgroundColor;

			this.editingStyle = this.ControlsGridView.DefaultCellStyle.Clone();
			this.editingStyle.BackColor = Color.LightPink;
			this.editingStyle.ForeColor = Color.Red;
			this.editingStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.editingStyle.Font = new Font(this.editingStyle.Font, FontStyle.Bold);

			this.ControlsGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
			this.ControlsGridView.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;

			// Done
			this.ControlsGridView.ClearSelection();
		}

		private void UpdateUI()
		{
			LockWindowUpdate(this.ControlsGridView.Handle);
			this.ControlsGridView.SuspendLayout();

			if (this.isEditingButton)
			{
				// Woah! this is unexpected. I will try to salvage the situation.
				this.isEditingButton = false;
			}

			controllerPreview.ViewMode = (this.deviceNumber == 0)
				? ControllerPreview.ViewModeEnum.Console
				: ControllerPreview.ViewModeEnum.Controller;

			// save current grid position and selection
			int? firstColIndex = null;
			int? firstRowIndex = null;
			int xScrollOffset;
			int? selectedColIndex = null;
			int? selectedRowIndex = null;

			xScrollOffset = this.ControlsGridView.HorizontalScrollingOffset;
			var firstCell = this.ControlsGridView.FirstDisplayedCell;
			firstColIndex = firstCell == null ? (int?)null : firstCell.ColumnIndex;
			firstRowIndex = firstCell == null ? (int?)null : firstCell.RowIndex;
			var selectedCell = this.ControlsGridView.CurrentCell;
			selectedColIndex = selectedCell == null ? (int?)null : selectedCell.ColumnIndex;
			selectedRowIndex = selectedCell == null ? (int?)null : selectedCell.RowIndex;

			// Clear everything.
			this.ControlsGridView.Columns.Clear();
			this.ControlsGridView.Rows.Clear();

			//////////////////
			// Set up datagrid columns
			DataGridViewColumn column;
			for (int newColumn = 0; newColumn < Enum.GetValues(typeof(GridColumn)).Length + this.devices[this.deviceNumber].BindingSets.Count + 1; newColumn++)
				this.ControlsGridView.Columns.Add(null, null);

			this.ControlsGridView.Columns[(int)GridColumn.InputButton].Visible = false;

			column = this.ControlsGridView.Columns[(int)GridColumn.ButtonName];
			column.HeaderText = JohnnyInputStrings.Control;
			column.SortMode = DataGridViewColumnSortMode.Programmatic;
			column.DefaultCellStyle = this.tintedStyle.Clone();
			column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

			int columnIndex;
			columnIndex = (int)GridColumn.ButtonName + 1;
			for (int setNumber = 0; setNumber < this.devices[this.deviceNumber].BindingSets.Count; setNumber++)
			{
				column = this.ControlsGridView.Columns[columnIndex];
				column.HeaderText = JohnnyInputStrings.BindingSet.Replace(" ", "_") + "_#" + (setNumber + 1).ToString();
				column.SortMode = DataGridViewColumnSortMode.Programmatic;
				column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.Tag = setNumber;

				columnIndex++;
			}

			column = this.ControlsGridView.Columns[this.ControlsGridView.ColumnCount - 1];
			column.SortMode = DataGridViewColumnSortMode.Programmatic;
			column.DefaultCellStyle = this.disabledStyle.Clone();
			column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			column.Tag = this.devices[this.deviceNumber].BindingSets.Count;

			//////////////////
			// Add some default rows
			int rowIndex;
			DataGridViewCell cell;

			// "delete" row.
			rowIndex = this.ControlsGridView.Rows.Add();
			this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

			columnIndex = (int)GridColumn.ButtonName + 1;
			for (int setNumber = 0; setNumber < this.devices[this.deviceNumber].BindingSets.Count; setNumber++)
			{
				cell = this.ControlsGridView[columnIndex, rowIndex];
				cell.Style = this.linkStyleBold;
				cell.Value = JohnnyInputStrings.DeleteSet;
				columnIndex++;
			}

			cell = this.ControlsGridView[this.ControlsGridView.ColumnCount - 1, rowIndex];
			cell.Style = this.linkStyleBoldWithGrayBack;
			cell.Value = JohnnyInputStrings.AddSet;

			// empty row.
			rowIndex = this.ControlsGridView.Rows.Add();
			this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

			// "set" row
			rowIndex = this.ControlsGridView.Rows.Add();
			this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

			columnIndex = (int)GridColumn.ButtonName + 1;
			for (int setNumber = 0; setNumber < this.devices[this.deviceNumber].BindingSets.Count; setNumber++)
			{
				cell = this.ControlsGridView[columnIndex, rowIndex];
				cell.Style = this.linkStyleNormal;
				cell.Value = JohnnyInputStrings.SetAll;
				columnIndex++;
			}

			// "clear" row
			rowIndex = this.ControlsGridView.Rows.Add();
			this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

			columnIndex = (int)GridColumn.ButtonName + 1;
			for (int setNumber = 0; setNumber < this.devices[this.deviceNumber].BindingSets.Count; setNumber++)
			{
				cell = this.ControlsGridView[columnIndex, rowIndex];
				cell.Style = this.linkStyleNormal;
				cell.Value = JohnnyInputStrings.ClearAll;
				columnIndex++;
			}

			//////////////////
			// Add bindings
			if (this.deviceNumber == 0)
			{
				this.AddGridItem(InputButton.ConsoleStateSave, "(F5) " + JohnnyInputStrings.ButtonConsoleSaveState);
				this.AddGridItem(InputButton.ConsoleStateLoad, "(F8) " + JohnnyInputStrings.ButtonConsoleLoadState);
				this.AddGridItem(InputButton.ConsoleStateSlotPrevious, "(F6) " + JohnnyInputStrings.ButtonConsoleStateSlotPrevious);
				this.AddGridItem(InputButton.ConsoleStateSlotNext, "(F7) " + JohnnyInputStrings.ButtonConsoleStateSlotNext);
				this.AddGridItem(InputButton.ConsoleFullScreen, "(F4) " + JohnnyInputStrings.ButtonConsoleFullScreen);
				this.AddGridItem(InputButton.ConsoleScreenShot, "(F3) " + JohnnyInputStrings.ButtonConsoleScreenshot);
				this.AddGridItem(InputButton.ConsolePause, "(F9) " + JohnnyInputStrings.ButtonConsolePause);
				this.AddGridItem(InputButton.ConsoleAdvanceBySingleFrame, "(F10) " + JohnnyInputStrings.ButtonConsoleAdvanceFrame);
				this.AddGridItem(InputButton.ConsoleReset, "(F12) " + JohnnyInputStrings.ButtonConsoleReset);
			}
			else
			{
				this.AddGridItem(InputButton.Up, JohnnyInputStrings.ButtonUp);
				this.AddGridItem(InputButton.Down, JohnnyInputStrings.ButtonDown);
				this.AddGridItem(InputButton.Left, JohnnyInputStrings.ButtonLeft);
				this.AddGridItem(InputButton.Right, JohnnyInputStrings.ButtonRight);
				this.AddGridItem(InputButton.A, JohnnyInputStrings.ButtonA);
				this.AddGridItem(InputButton.B, JohnnyInputStrings.ButtonB);
				this.AddGridItem(InputButton.C, JohnnyInputStrings.ButtonC);
				this.AddGridItem(InputButton.X, JohnnyInputStrings.ButtonX);
				this.AddGridItem(InputButton.P, JohnnyInputStrings.ButtonP);
				this.AddGridItem(InputButton.L, JohnnyInputStrings.ButtonL);
				this.AddGridItem(InputButton.R, JohnnyInputStrings.ButtonR);
			}

			////////////////////////////////
			// (Done with grid population)

			this.ControlsGridView.AutoResizeColumns();

			// Restore selection and first cell.
			DataGridViewCell newSelectedCell = this.TryGetCell(selectedColIndex, selectedRowIndex);

			if (newSelectedCell != null)
				this.ControlsGridView.CurrentCell = newSelectedCell;
			else
				this.ControlsGridView.ClearSelection();

			DataGridViewCell newFirstCell = null;
			try
			{
				newFirstCell = this.ControlsGridView[firstColIndex.Value, firstRowIndex.Value];
			}
			catch { }
			if (newFirstCell != null)
				this.ControlsGridView.FirstDisplayedCell = newFirstCell;

			try
			{
				this.ControlsGridView.HorizontalScrollingOffset = xScrollOffset;
			}
			catch { }

			this.ControlsGridView.ResumeLayout();
			LockWindowUpdate(IntPtr.Zero);
		}

		private void AddGridItem(InputButton button, string buttonName)
		{
			int rowIndex = this.ControlsGridView.Rows.Add();
			this.ControlsGridView[(int)GridColumn.InputButton, rowIndex].Value = button;
			this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Value = buttonName;
			
			int columnIndex = (int)GridColumn.ButtonName + 1;
			for (int setNumber = 0; setNumber < this.devices[this.deviceNumber].BindingSets.Count; setNumber++)
			{
				InputBindingSet set = this.devices[this.deviceNumber].BindingSets[setNumber];

				InputTrigger trigger = set.GetButtonTrigger(button);
				DataGridViewCell cell = this.ControlsGridView[columnIndex, rowIndex];
				cell.Tag = trigger;
				this.UpdateTriggerText(cell);

				columnIndex++;
			}
		}

		private void DoDeleteBinding(int setNumber)
		{
			this.devices[this.deviceNumber].BindingSets.RemoveSet(setNumber);
			this.UpdateUI();
		}

		private void DoAddBinding(int setNumber)
		{
			this.devices[this.deviceNumber].BindingSets.AddSet();
			this.UpdateUI();
		}

		private void DoSetAll(int setNumber)
		{
			this.isSettingAll = true;

			if (this.devices[this.deviceNumber].BindingSets.Count > 0)
			{
				DataGridViewCell cell = this.GetNextInputButtonCell(null, setNumber);
				this.DoStartEditButton(cell);
			}
		}

		private DataGridViewCell GetNextInputButtonCell(DataGridViewCell startCell, int setNumber)
		{
			return this.GetNextInputButtonCell(startCell == null ? -1 : startCell.RowIndex, setNumber);
		}

		private DataGridViewCell GetNextInputButtonCell(int startRow, int setNumber)
		{
			if (setNumber < 0)
				return null;

			if (setNumber >= this.devices[this.deviceNumber].BindingSets.Count)
				return null;

			// Find the next row that defines an input button.
			InputButton? button;
			do
			{
				startRow++;
				button = this.FindInputButtonForRow(startRow);
			}
			while (!button.HasValue && startRow < this.ControlsGridView.Rows.Count);

			// No other row?
			if (!button.HasValue)
				return null;

			////////////////
			// Get the cell for this set number.
			foreach (DataGridViewColumn column in this.ControlsGridView.Columns)
			{
				if (column.Tag != null && ((int)column.Tag == setNumber))
					return this.ControlsGridView[column.Index, startRow];
			}

			// This shouldn't happen.
			return null;
		}

		private void DoClearAll(int setNumber)
		{
			this.devices[this.deviceNumber].BindingSets[setNumber].Clear();
			this.UpdateUI();
		}

		private void UpdatePreviewBasedOnGrid()
		{
			InputButton? button = this.FindInputButtonForRow(this.ControlsGridView.CurrentCell);

			this.UpdatePreviewToButton(button);
		}

		private void DoStartEditButton(DataGridViewCell cell)
		{
			if (this.isEditingButton)
				return;

			if (cell == null)
				return;

			int setIndex = this.FindSetIndexForColumn(cell);
			if (setIndex < 0 || setIndex >= this.devices[this.deviceNumber].BindingSets.Count)
				return;

			InputButton? button = this.FindInputButtonForRow(cell);
			if (button.HasValue == false)
				return;

			//////////////////
			// Okay! We've got a button to bind.

			this.controllerPreview.HighlightedButton = button;
			this.ControlsGridView.Focus();
			this.ControlsGridView.CurrentCell = cell;

			this.isEditingButton = true;
			this.editedCell = cell;
			this.editedCell.Style = editingStyle;
			this.editedCell.Value = JohnnyInputStrings.Listening;
			this.ControlsGridView.ClearSelection();
		}

	    private void DoStopEditButton(InputTrigger trigger = null)
		{
			if (!this.isEditingButton)
				return;

			int currentRowIndex = this.editedCell.RowIndex;
			var currentSetNumber = this.FindSetIndexForColumn(this.editedCell);

			// If there was a trigger sent in to complete the button editing, then bind it!
			InputButton? button = this.FindInputButtonForRow(this.editedCell);
			if (trigger != null && button != null)
			{
				int setNumber = this.FindSetIndexForColumn(this.editedCell);
				this.devices.SetBinding(deviceNumber, setNumber, button.Value, trigger);
			}
			else
			{
				this.isSettingAll = false;
			}
			
			// Finalize editing here.
			this.editedCell = null;
			this.isEditingButton = false;
			this.UpdateUI();

			// Move on to the next cell down (unless we were forcibly cancelled).
			// Also, if the mouse is over the controller preview, we might not move to
			// the next item.
			DataGridViewCell nextCell = null;
			if (trigger != null)
			{
				nextCell = this.GetNextInputButtonCell(currentRowIndex, currentSetNumber);
				if (nextCell != null)
				{
					if (this.IsMouseOverPreview() && !this.isSettingAll)
					{
						// If the mouse if over the controller preview
						// AND we're not "setting all", it's safe to
						// forego moving to the next item.
					}
					else
						this.ControlsGridView.CurrentCell = nextCell;
				}
			}

			// If we're in "setting all" mode, we also start editing the next item.
			if (this.isSettingAll)
			{
				if (nextCell == null)
					this.isSettingAll = false;
				else
					this.DoStartEditButton(nextCell);
			}
		}

		private bool IsMouseOverPreview()
		{
			Rectangle controllerScreenRect = 
					this.controllerPreview.RectangleToScreen(
						new Rectangle(new Point(0,0), this.controllerPreview.Size));

			bool inBounds = controllerScreenRect.Contains(System.Windows.Forms.Cursor.Position);
			return inBounds;
		}

		private void DoClearCellBinding()
		{
			this.DoClearCellBinding(this.ControlsGridView.CurrentCell);
		}

		private void DoClearCellBinding(DataGridViewCell cell)
		{
			if (cell == null)
				return;

			InputButton? button = this.FindInputButtonForRow(cell);
			int setIndex = this.FindSetIndexForColumn(cell);

			if (!button.HasValue)
				return;

			if (setIndex == -1)
				return;

			this.devices.SetBinding(deviceNumber, setIndex, button.Value, null);
			this.UpdateUI();
		}

		private void UpdatePreviewToButton(InputButton? button)
		{
			if (this.isEditingButton)
				return;

			this.controllerPreview.HighlightedButton = button;
		}

		private int FindSetIndexForColumn(DataGridViewCell cell)
		{
			if (cell == null)
				return -1;

			return this.FindSetIndexForColumn(cell.ColumnIndex);
		}

		private int FindSetIndexForColumn(int columnIndex)
		{
			if (columnIndex == (int)GridColumn.InputButton
				|| columnIndex == (int)GridColumn.ButtonName
				|| columnIndex == (this.ControlsGridView.Columns.Count - 1))
				return -1;

			return (int)this.ControlsGridView.Columns[columnIndex].Tag;
		}

		private DataGridViewCell FindBestCellForInputButton(InputButton? button)
		{
			if (!button.HasValue)
				return null;

			int rowIndex = this.FindRowForInputButton(button.Value);
			if (rowIndex == -1)
				return null;

			int currentSetIndex = this.FindSetIndexForColumn(this.ControlsGridView.CurrentCell);
			if (currentSetIndex == -1)
				currentSetIndex = 0;

			int columnToSelect = this.FindColumnForSetIndex(currentSetIndex);

			return this.TryGetCell(columnToSelect, rowIndex);
		}

		private int FindRowForInputButton(InputButton button)
		{
			for (int row = 0; row < this.ControlsGridView.Rows.Count; row++)
			{
				InputButton? rowButton = this.FindInputButtonForRow(row);
				if (rowButton.HasValue && rowButton.Value == button)
					return row;
			}
			return -1;
		}

		private int FindColumnForSetIndex(int setIndex)
		{
			for (int col = 0; col < this.ControlsGridView.Columns.Count; col++)
			{
				if (this.ControlsGridView.Columns[col].Tag is int)
				{
					int colSetIndex = (int)this.ControlsGridView.Columns[col].Tag;
					if (colSetIndex == setIndex)
						return col;
				}
			}
			return -1;
		}

		private InputButton? FindInputButtonForRow(DataGridViewCell cell)
		{
			if (cell == null)
				return null;

			return this.FindInputButtonForRow(cell.RowIndex);
		}
		
		private InputButton? FindInputButtonForRow(int rowIndex)
		{
			if (rowIndex < 0)
				return null;

			if (rowIndex >= this.ControlsGridView.Rows.Count)
				return null;

			var cell = this.ControlsGridView[(int)GridColumn.InputButton, rowIndex];
			if (cell.Value == null)
				return null;

			return (InputButton)cell.Value;
		}

		private DataGridViewCell TryGetCell(int? colIndex, int? rowIndex)
		{
			if (rowIndex.HasValue == false)
				return null;

			if (colIndex.HasValue == false)
				return null;

			if (rowIndex.Value < 0 || rowIndex.Value >= this.ControlsGridView.Rows.Count)
				return null;

			if (colIndex.Value < 0 || colIndex.Value >= this.ControlsGridView.Columns.Count)
				return null;

			return this.ControlsGridView[colIndex.Value, rowIndex.Value];
		}

		private void UpdateTriggerText(DataGridViewCell cell)
		{
			var trigger = (InputTrigger)cell.Tag;
			if (trigger == null)
				cell.Value = null;
			else
				cell.Value = trigger.FriendlyName;
		}

		#endregion // Private Functions

		private void JohnnyInputSettings_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.joyWatcher.Dispose();
			this.joyChecker.Dispose();
		}

		private void ResetButton_Click(object sender, EventArgs e)
		{
			this.devices = InputBindingDevices.LoadDefault();
			this.EnsureMinimumDevices();
			this.UpdateUI();
		}

		private void EnsureMinimumDevices()
		{
			while (this.devices.Count < 7)
				this.devices.AddDevice();
		}
	}
}
