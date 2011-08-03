using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FourDO.UI
{
    public partial class NagBox : UserControl
    {
        public event EventHandler CloseClicked;
        public event EventHandler LinkClicked;

        public string MessageText
        {
            get
            {
                return MessageLabel.Text;
            }
            set
            {
                MessageLabel.Text = value;
            }
        }

        public string LinkText
        {
            get
            {
                return LinkLabel.Text;
            }
            set
            {
                LinkLabel.Text = value;
            }
        }

        public NagBox()
        {
            InitializeComponent();
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.LinkClicked != null)
            {
                this.LinkClicked(this, new EventArgs());
            }
        }

        private void HideLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.CloseClicked != null)
            {
                this.CloseClicked(this, new EventArgs());
            }
        }
    }
}
