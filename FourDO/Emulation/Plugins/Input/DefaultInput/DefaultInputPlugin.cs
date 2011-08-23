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

namespace FourDO.Emulation.Plugins.Input.DefaultInput
{
    internal class DefaultInputPlugin : IInputPlugin
    {
        GlobalKeyboardHook globalHook = new GlobalKeyboardHook();
        Dictionary<DefaultInputButton, Keys> keys = new Dictionary<DefaultInputButton, Keys>();
        Dictionary<DefaultInputButton, bool> keysDown = new Dictionary<DefaultInputButton, bool>();

        public DefaultInputPlugin()
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
            DefaultInputSettings settingsForm = new DefaultInputSettings(keys);
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
            calculatedByte |= this.CheckDownButton(DefaultInputButton.X) ? (byte)0x10 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.P) ? (byte)0x20 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.C) ? (byte)0x40 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.B) ? (byte)0x80 : (byte)0; // Positive!
            //calculatedByte |= this.CheckDownButton(DefaultInputButton.?) ? (byte)0x01 : (byte)0; // dunno
            //calculatedByte |= this.CheckDownButton(DefaultInputButton.?) ? (byte)0x02 : (byte)0; // dunno
            calculatedByte |= this.CheckDownButton(DefaultInputButton.L) ? (byte)0x04 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.R) ? (byte)0x08 : (byte)0; // Positive!
            data[0x2] = calculatedByte;

            calculatedByte = 0;
            calculatedByte |= this.CheckDownButton(DefaultInputButton.A) ? (byte)0x01 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.Left) ? (byte)0x02 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.Right) ? (byte)0x04 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.Up) ? (byte)0x08 : (byte)0; // Positive!
            calculatedByte |= this.CheckDownButton(DefaultInputButton.Down) ? (byte)0x10 : (byte)0; // Positive!
            //calculatedByte |= this.CheckDownButton(DefaultInputButton.?) ? (byte)0x20 : (byte)0; // dunno
            //calculatedByte |= this.CheckDownButton(DefaultInputButton.?) ? (byte)0x40 : (byte)0; // dunno
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
            this.LoadKeyIfValid(DefaultInputButton.Up, DefaultInput.Default.UpKey);
            this.LoadKeyIfValid(DefaultInputButton.Down, DefaultInput.Default.DownKey);
            this.LoadKeyIfValid(DefaultInputButton.Left, DefaultInput.Default.LeftKey);
            this.LoadKeyIfValid(DefaultInputButton.Right, DefaultInput.Default.RightKey);
            this.LoadKeyIfValid(DefaultInputButton.A, DefaultInput.Default.AKey);
            this.LoadKeyIfValid(DefaultInputButton.B, DefaultInput.Default.BKey);
            this.LoadKeyIfValid(DefaultInputButton.C, DefaultInput.Default.CKey);
            this.LoadKeyIfValid(DefaultInputButton.X, DefaultInput.Default.XKey);
            this.LoadKeyIfValid(DefaultInputButton.P, DefaultInput.Default.PKey);
            this.LoadKeyIfValid(DefaultInputButton.L, DefaultInput.Default.LKey);
            this.LoadKeyIfValid(DefaultInputButton.R, DefaultInput.Default.RKey);
        }

        private void SaveKeys()
        {
            DefaultInput.Default.UpKey = this.GetKeyAsStringIfMapped(DefaultInputButton.Up);
            DefaultInput.Default.DownKey = this.GetKeyAsStringIfMapped(DefaultInputButton.Down);
            DefaultInput.Default.LeftKey = this.GetKeyAsStringIfMapped(DefaultInputButton.Left);
            DefaultInput.Default.RightKey = this.GetKeyAsStringIfMapped(DefaultInputButton.Right);
            DefaultInput.Default.AKey = this.GetKeyAsStringIfMapped(DefaultInputButton.A);
            DefaultInput.Default.BKey = this.GetKeyAsStringIfMapped(DefaultInputButton.B);
            DefaultInput.Default.CKey = this.GetKeyAsStringIfMapped(DefaultInputButton.C);
            DefaultInput.Default.XKey = this.GetKeyAsStringIfMapped(DefaultInputButton.X);
            DefaultInput.Default.PKey = this.GetKeyAsStringIfMapped(DefaultInputButton.P);
            DefaultInput.Default.LKey = this.GetKeyAsStringIfMapped(DefaultInputButton.L);
            DefaultInput.Default.RKey = this.GetKeyAsStringIfMapped(DefaultInputButton.R);
            DefaultInput.Default.Save();
        }

        private void LoadKeyIfValid(DefaultInputButton button, string keyName)
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

        private string GetKeyAsStringIfMapped(DefaultInputButton buttonToFind)
        {
            string returnValue = null;
            if (this.keys.Keys.Contains<DefaultInputButton>(buttonToFind) == true)
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
            foreach (DefaultInputButton button in this.keys.Keys)
            {
                if (this.keys[button] == key)
                {
                    this.keysDown[button] = true;
                }
            }
        }

        private void RemoveDownKey(Keys key)
        {
            foreach (DefaultInputButton button in this.keys.Keys)
            {
                if (this.keys[button] == key)
                {
                    this.keysDown[button] = false;
                }
            }
        }
        
        private bool CheckDownButton(DefaultInputButton button)
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
