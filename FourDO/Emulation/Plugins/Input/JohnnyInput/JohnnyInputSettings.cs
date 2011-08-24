using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class JohnnyInputSettings : Form
	{
        private enum GridColumn
        {
            InputButton = 0,
            ButtonName,
            Binding1,
            Binding2,
            AddBinding
        }

        InputBindingSets bindings;

        DataGridViewCellStyle linkStyle1;
        DataGridViewCellStyle linkStyle2;
        DataGridViewCellStyle tintedStyle;
        DataGridViewCellStyle disabledStyle;

		public JohnnyInputSettings(InputBindingSets originalBindings, string bindingsFilePath)
		{
            this.bindings = (InputBindingSets)originalBindings.Clone();
            this.BindingsFilePath = bindingsFilePath;

			this.DialogResult = DialogResult.Cancel;

			this.InitializeComponent();
		}

        public string BindingsFilePath { get; private set; }

		#region Event Handlers

		private void JohnnyInputSettings_Load(object sender, EventArgs e)
		{
			this.InitializeGrid();

			this.DeviceTypeComboBox.SelectedIndex = 0;
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
                this.bindings.SaveToFile(this.BindingsFilePath);
                savedSuccessfully = true;
            }
            catch (Exception ex)
            {
                Utilities.Error.ShowError("Unexpected error saving the bindings to file : " + this.BindingsFilePath + "\r\n\r\nDetails: " + ex.ToString());
            }

            if (savedSuccessfully)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
		}

        private void ControlsGridView_MouseMove(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo info = this.ControlsGridView.HitTest(e.X, e.Y);
            
            DataGridViewCell cell = null;
            InputButton? highlightedButton = null;

            try
            {
                cell = this.ControlsGridView[info.ColumnIndex, info.RowIndex];
                highlightedButton = (InputButton)this.ControlsGridView[(int)GridColumn.InputButton, info.RowIndex].Value;
            }
            catch {}

            // Show a hand cursor over link-styled cells
            if (cell != null 
                    && (cell.Style == this.linkStyle1 || cell.Style == this.linkStyle2))
                this.ControlsGridView.Cursor = Cursors.Hand;
            else
                this.ControlsGridView.Cursor = Cursors.Default;

            // Highlight a specific controller button if the mouse is over a control row.
            this.controllerPreview.HighlightedButton = highlightedButton;
        }

        private void ControlsGridView_MouseLeave(object sender, EventArgs e)
        {
            this.controllerPreview.HighlightedButton = null;
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
            this.linkStyle1 = this.ControlsGridView.DefaultCellStyle.Clone();
            this.linkStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.linkStyle1.ForeColor = Color.Blue;
            this.linkStyle1.Font = new Font(this.linkStyle1.Font, FontStyle.Underline);

            this.linkStyle2 = this.linkStyle1.Clone();
            this.linkStyle2.Font = new Font(this.linkStyle2.Font, FontStyle.Bold | FontStyle.Underline);

            this.tintedStyle = this.ControlsGridView.DefaultCellStyle.Clone();
            this.tintedStyle.BackColor = SystemColors.Info;

            this.disabledStyle = this.ControlsGridView.DefaultCellStyle.Clone();
            this.disabledStyle.BackColor = this.ControlsGridView.BackgroundColor;

            this.ControlsGridView.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
            this.ControlsGridView.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;

            //////////////////
            // Set up datagrid columns
            DataGridViewColumn column;
            for (int newColumn = 0; newColumn < Enum.GetValues(typeof(GridColumn)).Length; newColumn++)
                this.ControlsGridView.Columns.Add(null, null);

            this.ControlsGridView.Columns[(int)GridColumn.InputButton].Visible = false;

            column = this.ControlsGridView.Columns[(int)GridColumn.ButtonName];
            column.HeaderText = "Control";
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.DefaultCellStyle = this.tintedStyle.Clone();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            column = this.ControlsGridView.Columns[(int)GridColumn.Binding1];
            column.HeaderText = "Binding 1";
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            column = this.ControlsGridView.Columns[(int)GridColumn.Binding2];
            column.HeaderText = "Binding 2";
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            column = this.ControlsGridView.Columns[(int)GridColumn.AddBinding];
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            column.DefaultCellStyle = this.disabledStyle.Clone();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //////////////////
            // Add some default rows
            int rowIndex;
            DataGridViewCell cell;

            // "delete" row.
            rowIndex = this.ControlsGridView.Rows.Add();
            cell = this.ControlsGridView[(int)GridColumn.Binding1, rowIndex];
            cell.Style = this.linkStyle2;
            cell.Value = "Delete Binding";

            this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

            cell = this.ControlsGridView[this.ControlsGridView.ColumnCount - 1, rowIndex];
            cell.Style = this.linkStyle2;
            cell.Value = "Add Binding";

            // empty row.
            rowIndex = this.ControlsGridView.Rows.Add();
            this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

            // "set" row
            rowIndex = this.ControlsGridView.Rows.Add();
            cell = this.ControlsGridView[(int)GridColumn.Binding1, rowIndex];
            cell.Style = this.linkStyle1;
            cell.Value = "Set All";

            this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;

            // "clear" row
            rowIndex = this.ControlsGridView.Rows.Add();
            cell = this.ControlsGridView[(int)GridColumn.Binding1, rowIndex];
            cell.Style = this.linkStyle1;
            cell.Value = "Clear All";

            this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Style = this.disabledStyle;
            
            //////////////////
            // Add bindings
            this.AddGridItem(InputButton.Up, "Up");
			this.AddGridItem(InputButton.Down, "Down");
			this.AddGridItem(InputButton.Left, "Left");
			this.AddGridItem(InputButton.Right, "Right");
			this.AddGridItem(InputButton.A, "A");
			this.AddGridItem(InputButton.B, "B");
			this.AddGridItem(InputButton.C, "C");
			this.AddGridItem(InputButton.X, "X (\"Stop\")");
			this.AddGridItem(InputButton.P, "P (\"Play/Pause\")");
			this.AddGridItem(InputButton.L, "L (Left Shoulder)");
			this.AddGridItem(InputButton.R, "R (Right Shoulder)");
            
            this.ControlsGridView.AutoResizeColumns();

			// Done
            this.ControlsGridView.ClearSelection();
 		}

		private void AddGridItem(InputButton button, string buttonName)
		{
            int rowIndex = this.ControlsGridView.Rows.Add();
            this.ControlsGridView[(int)GridColumn.InputButton, rowIndex].Value = button;
            this.ControlsGridView[(int)GridColumn.ButtonName, rowIndex].Value = buttonName;
            this.ControlsGridView[(int)GridColumn.Binding1, rowIndex].Value = null;
            this.ControlsGridView[(int)GridColumn.Binding2, rowIndex].Value = null;
            this.ControlsGridView[(int)GridColumn.AddBinding, rowIndex].Value = null;
		}

		#endregion // Private Functions
	}
}
