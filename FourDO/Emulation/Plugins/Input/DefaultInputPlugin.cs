//////////////////////////////////////////////////////////
// JMK NOTES:
// I figured the quickest way to get controls in would just be
// to use global keyboard hooks. This is a bit lazy, especially
// because I don't realy think I want them global. Oh well.
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FourDO.Utilities;

namespace FourDO.Emulation.Plugins.Input
{
    internal class DefaultInputPlugin : IInputPlugin
    {
        GlobalKeyboardHook globalHook = new GlobalKeyboardHook();
        Dictionary<DefaultInputButton, Keys> keys = new Dictionary<DefaultInputButton, Keys>();

        public DefaultInputPlugin()
        {
            this.LoadKeys();
            globalHook.KeyDown += new KeyEventHandler(GlobalHook_KeyDown);
            globalHook.KeyUp += new KeyEventHandler(GlobalHook_KeyUp);
        }

        void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
        }

        void GlobalHook_KeyUp(object sender, KeyEventArgs e)
        {
        }
        
        #region IInputPlugin Implementation

        public void Destroy() {}

        public bool GetHasSettings()
        {
            return true;
        }

        public void ShowSettings(IWin32Window owner)
        {
            DefaultInputSettings settingsForm = new DefaultInputSettings(keys);
            settingsForm.ShowDialog(owner);
            this.SaveKeys();
            this.UpdateHookedKeys();
        }

        public void LoadKeys()
        {
        }

        public void SaveKeys()
        {
        }

        public void UpdateHookedKeys()
        {
            globalHook.HookedKeys.Clear();
            foreach (Keys key in keys.Values)
            {
                if (globalHook.HookedKeys.Contains(key) == false)
                    globalHook.HookedKeys.Add(key);
            }
        }

        #endregion // IInputPlugin Implementation
    }
}
