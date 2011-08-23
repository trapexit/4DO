//////////////////////////////////////////////////////////
// JMK NOTES:
// I figured the quickest way to get controls in would just be
// to use global keyboard hooks. This is a bit lazy, especially
// because I don't realy think I want them global. Oh well.
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FourDO.Utilities;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
    internal class JohnnyInputPlugin : IInputPlugin
    {
        GlobalKeyboardHook globalHook = new GlobalKeyboardHook();
        Dictionary<JohnnyInputButton, Keys> keys = new Dictionary<JohnnyInputButton, Keys>();
        Dictionary<JohnnyInputButton, bool> keysDown = new Dictionary<JohnnyInputButton, bool>();

        public JohnnyInputPlugin()
        {
            this.LoadKeys();
            this.UpdateHookedKeys();

            globalHook.KeyDown += new KeyEventHandler(GlobalHook_KeyDown);
            globalHook.KeyUp += new KeyEventHandler(GlobalHook_KeyUp);
        }

        #region IInputPlugin Implementation

        public void Destroy() { }

        public bool GetHasSettings()
        {
            return true;
        }

        public void ShowSettings(IWin32Window owner)
        {
            JohnnyInputSettings settingsForm = new JohnnyInputSettings(keys);
            settingsForm.ShowDialog(owner);
            this.SaveKeys();
            this.ClearDownKeys();
            this.UpdateHookedKeys();
        }

        public byte[] GetPbusData()
        {
            byte[] data = new byte[16];
            byte calculatedByte = 0;

            data[0x0] = 0x00;
            data[0x1] = 0x49;

            calculatedByte = 0;
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.X) ? (byte)0x10 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.P) ? (byte)0x20 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.C) ? (byte)0x40 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.B) ? (byte)0x80 : (byte)0; // Positive!
            //calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x01 : (byte)0; // dunno
            //calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x02 : (byte)0; // dunno
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.L) ? (byte)0x04 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.R) ? (byte)0x08 : (byte)0; // Positive!
            data[0x2] = calculatedByte;

            calculatedByte = 0;
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.A) ? (byte)0x01 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.Left) ? (byte)0x02 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.Right) ? (byte)0x04 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.Up) ? (byte)0x08 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(JohnnyInputButton.Down) ? (byte)0x10 : (byte)0; // Positive!
            //calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x20 : (byte)0; // dunno
            //calculatedByte |= this.CheckDownButton(JohnnyInputButton.?) ? (byte)0x40 : (byte)0; // dunno
            calculatedByte |= (byte)0x80;
            data[0x3] = calculatedByte;

            data[0x4] = 0xFF;
            data[0x5] = 0xFF;
            data[0x6] = 0x00;
            data[0x7] = 0x00;
            data[0x8] = 0xFF;
            data[0x9] = 0xFF;
            data[0xA] = 0xFF;
            data[0xB] = 0xFF;
            data[0xC] = 0xFF;
            data[0xD] = 0xFF;
            data[0xE] = 0xFF;
            data[0xF] = 0xFF;

            return data;
        }

        #endregion // IInputPlugin Implementation

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            this.AddDownKey(e.KeyCode);
        }

        private void GlobalHook_KeyUp(object sender, KeyEventArgs e)
        {
            this.RemoveDownKey(e.KeyCode);
        }

        private void LoadKeys()
        {
            this.keys.Clear();
            this.LoadKeyIfValid(JohnnyInputButton.Up, JohnnyInput.Default.UpKey);
            this.LoadKeyIfValid(JohnnyInputButton.Down, JohnnyInput.Default.DownKey);
            this.LoadKeyIfValid(JohnnyInputButton.Left, JohnnyInput.Default.LeftKey);
            this.LoadKeyIfValid(JohnnyInputButton.Right, JohnnyInput.Default.RightKey);
            this.LoadKeyIfValid(JohnnyInputButton.A, JohnnyInput.Default.AKey);
            this.LoadKeyIfValid(JohnnyInputButton.B, JohnnyInput.Default.BKey);
            this.LoadKeyIfValid(JohnnyInputButton.C, JohnnyInput.Default.CKey);
            this.LoadKeyIfValid(JohnnyInputButton.X, JohnnyInput.Default.XKey);
            this.LoadKeyIfValid(JohnnyInputButton.P, JohnnyInput.Default.PKey);
            this.LoadKeyIfValid(JohnnyInputButton.L, JohnnyInput.Default.LKey);
            this.LoadKeyIfValid(JohnnyInputButton.R, JohnnyInput.Default.RKey);
        }

        private void SaveKeys()
        {
            JohnnyInput.Default.UpKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.Up);
            JohnnyInput.Default.DownKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.Down);
            JohnnyInput.Default.LeftKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.Left);
            JohnnyInput.Default.RightKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.Right);
            JohnnyInput.Default.AKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.A);
            JohnnyInput.Default.BKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.B);
            JohnnyInput.Default.CKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.C);
            JohnnyInput.Default.XKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.X);
            JohnnyInput.Default.PKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.P);
            JohnnyInput.Default.LKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.L);
            JohnnyInput.Default.RKey = this.GetKeyAsStringIfMapped(JohnnyInputButton.R);
            JohnnyInput.Default.Save();
        }

        private void LoadKeyIfValid(JohnnyInputButton button, string keyName)
        {
            Keys? key = this.GetKeyByKeyName(keyName);
            if (key != null)
                this.keys.Add(button, key.Value);
        }

        private Keys? GetKeyByKeyName(string keyName)
        {
            Keys? returnValue = null;
            Keys outResult;
            if (Enum.TryParse<Keys>(keyName, out outResult))
                returnValue = outResult;
            return returnValue;
        }

        private string GetKeyAsStringIfMapped(JohnnyInputButton buttonToFind)
        {
            string returnValue = null;
            if (this.keys.Keys.Contains<JohnnyInputButton>(buttonToFind) == true)
            {
                returnValue = this.keys[buttonToFind].ToString();
            }
            return returnValue;
        }

        private void ClearDownKeys()
        {
            this.keysDown.Clear();
        }

        private void AddDownKey(Keys key)
        {
            foreach (JohnnyInputButton button in this.keys.Keys)
            {
                if (this.keys[button] == key)
                {
                    this.keysDown[button] = true;
                }
            }
        }

        private void RemoveDownKey(Keys key)
        {
            foreach (JohnnyInputButton button in this.keys.Keys)
            {
                if (this.keys[button] == key)
                {
                    this.keysDown[button] = false;
                }
            }
        }
        
        private bool CheckDownButton(JohnnyInputButton button)
        {
            if (this.keysDown.ContainsKey(button))
            {
                return this.keysDown[button];
            }
            return false;
        }

        private void UpdateHookedKeys()
        {
            globalHook.HookedKeys.Clear();
            foreach (Keys key in keys.Values)
            {
                if (globalHook.HookedKeys.Contains(key) == false)
                    globalHook.HookedKeys.Add(key);
            }
        }
    }
}
