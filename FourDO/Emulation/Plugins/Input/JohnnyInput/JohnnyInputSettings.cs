using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	public partial class JohnnyInputSettings : Form
	{
		[Serializable]
		public class GridItem
		{
			public GridItem() {}

			public GridItem (string control, string binding1, string binding2) 
			{
				this.Control = control;
				this.Binding1 = binding1;
				this.Binding2 = binding2;
			}

			[XmlIgnore]
			public string Control { get; set; }
			public string Binding1 { get; set; }
			public string Binding2 { get; set; }
			public JohnnyInputButton InputButton { get; set; }
		}
		
		Dictionary<JohnnyInputButton, Keys> keys;
		private List<GridItem> gridItems;

		public JohnnyInputSettings(Dictionary<JohnnyInputButton, Keys> keys)
		{
			this.keys = keys;

			this.DialogResult = DialogResult.Cancel;

			this.InitializeComponent();
		}

		#region Event Handlers

		private void JohnnyInputSettings_Load(object sender, EventArgs e)
		{
			this.InitializeGrid();

			this.DeviceTypeComboBox.SelectedIndex = 0;

			FileStream stream = new FileStream(@"C:\jmk\teststream.txt", FileMode.Create);
			XmlSerializer serializer = new XmlSerializer(this.gridItems.GetType());
			serializer.Serialize(stream, this.gridItems);

			stream.Close();
			stream = new FileStream(@"C:\jmk\teststream.txt", FileMode.Open);

			this.gridItems = (List<GridItem>)(serializer.Deserialize(stream));
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		#endregion // Event Handlers

		#region Private Functions

		private void InitializeGrid()
		{
			this.gridItems = new List<GridItem>();

			this.AddGridItem(JohnnyInputButton.Up, "Up");
			this.AddGridItem(JohnnyInputButton.Down, "Down");
			this.AddGridItem(JohnnyInputButton.Left, "Left");
			this.AddGridItem(JohnnyInputButton.Right, "Right");
			this.AddGridItem(JohnnyInputButton.A, "A");
			this.AddGridItem(JohnnyInputButton.B, "B");
			this.AddGridItem(JohnnyInputButton.C, "C");
			this.AddGridItem(JohnnyInputButton.X, "X (Stop button)");
			this.AddGridItem(JohnnyInputButton.P, "P (Play/Pause button)");
			this.AddGridItem(JohnnyInputButton.L, "Left Shoulder");
			this.AddGridItem(JohnnyInputButton.R, "Right Shoulder");

			this.ControlsGridView.DataSource = gridItems;
			this.ControlsGridView.Columns["Binding1"].HeaderText = "Main_Binding";
			this.ControlsGridView.Columns["Binding2"].HeaderText = "Alternate_Binding";
			this.ControlsGridView.Columns["InputButton"].Visible = false;

			this.ControlsGridView.AutoResizeColumns();

			this.ControlsGridView.CurrentCell = this.ControlsGridView[0, 0];

		}

		private void AddGridItem(JohnnyInputButton button, string friendlyCaption)
		{
			GridItem newGridItem = new GridItem();
			newGridItem.Control = friendlyCaption;
			this.gridItems.Add(newGridItem);
		}

		#endregion // Private Functions
	}
}
