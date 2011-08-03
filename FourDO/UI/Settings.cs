using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Emulation.Plugins;

namespace FourDO.UI
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.GraphicsPluginComboBox.SelectedIndex = 0;
            this.AudioPluginComboBox.SelectedIndex = 0;
            this.InputPluginComboBox.SelectedIndex = 0;

            this.ConfigureAudioButton.Enabled = PluginLoader.GetAudioPlugin().GetHasSettings();
            this.ConfigureInputButton.Enabled = PluginLoader.GetInputPlugin().GetHasSettings();
        }

        private void ConfigureAudioButton_Click(object sender, EventArgs e)
        {
            PluginLoader.GetAudioPlugin().ShowSettings(this);
        }

        private void ConfigureInputButton_Click(object sender, EventArgs e)
        {
            PluginLoader.GetInputPlugin().ShowSettings(this);
        }
    }
}

