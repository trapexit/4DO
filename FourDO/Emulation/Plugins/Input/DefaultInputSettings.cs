using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.Emulation.Plugins.Input
{
    internal partial class DefaultInputSettings : Form
    {
        Dictionary<DefaultInputButton, Keys> keys;

        public DefaultInputSettings(Dictionary<DefaultInputButton, Keys> keys)
        {
            this.keys = keys;

            this.DialogResult = DialogResult.Cancel;

            this.InitializeComponent();
        }

        private void DefaultInputSettings_Load(object sender, EventArgs e)
        {
            this.ControlsListView.Items.Clear();
            this.AddListItem(DefaultInputButton.Up, "Up");
            this.AddListItem(DefaultInputButton.Down, "Down");
            this.AddListItem(DefaultInputButton.Left, "Left");
            this.AddListItem(DefaultInputButton.Right, "Right");
            this.AddListItem(DefaultInputButton.A, "A");
            this.AddListItem(DefaultInputButton.B, "B");
            this.AddListItem(DefaultInputButton.C, "C");
            this.AddListItem(DefaultInputButton.X, "X (Stop button)");
            this.AddListItem(DefaultInputButton.P, "P (Play/Pause button)");
            this.AddListItem(DefaultInputButton.L, "Left Shoulder");
            this.AddListItem(DefaultInputButton.R, "Right Shoulder");

            this.UpdateUI();
        }

        private void UpdateUI()
        {
            ControlsListView.SuspendLayout(); 
            foreach (ListViewItem item in this.ControlsListView.Items)
            {
                string key = null;
                if (item.Tag != null)
                    key = ((Keys)item.Tag).ToString();

                item.SubItems[1].Text = key;
            }
            ControlsListView.ResumeLayout();
        }

        private void AddListItem(DefaultInputButton button, string friendlyCaption)
        {
            string currentKey = null;
            Keys outValue;
            if (this.keys.TryGetValue(button, out outValue) == true)
                currentKey = outValue.ToString();
            
            ListViewItem newItem = new ListViewItem(friendlyCaption);
            newItem.Name = button.ToString();
            newItem.Tag = currentKey == null ? (Keys?)null : (Keys?)outValue;
            newItem.SubItems.Add("");

            this.ControlsListView.Items.Add(newItem);
        }

        private int lastX = 0;
        private int lastY = 0;

        private void ControlsListView_MouseMove(object sender, MouseEventArgs e)
        {
            lastX = e.X;
            lastY = e.Y;
        }

        private void ControlsListView_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = this.ControlsListView.HitTest(lastX, lastY);
            if (info.Item != null)
            {
                this.CancelSetItemKey();
                this.SetItemKey(info.Item);
            }
        }

        ListViewItem itemSettingKey = null;

        private void SetItemKey(ListViewItem item)
        {
            item.Tag = null; // Clear the key.
            item.SubItems[1].Text = "(Press a key)";
            itemSettingKey = item;
        }

        private void CancelSetItemKey()
        {
            if (itemSettingKey != null)
            {
                itemSettingKey = null;
                this.UpdateUI();
            }
        }

        private void ControlsListView_Leave(object sender, EventArgs e)
        {
            this.CancelSetItemKey();
        }

        private void ControlsListView_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (itemSettingKey == null)
                return;

            if (e.KeyCode == Keys.Escape)
            {
                this.CancelSetItemKey();
                return;
            }

            itemSettingKey.Tag = e.KeyCode;
            itemSettingKey = null;

            this.UpdateUI();
        }

        private void ControlsListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ControlsListView_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.ControlsListView.Items)
            {
                item.Tag = null;
            }
            this.UpdateUI();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            keys.Clear();
            foreach (ListViewItem item in this.ControlsListView.Items)
            {
                DefaultInputButton button = (DefaultInputButton)Enum.Parse(typeof(DefaultInputButton), item.Name);
                if (item.Tag != null)
                    keys[button] = (Keys)item.Tag;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
