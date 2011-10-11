using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FourDO.Utilities
{
    public static class DisplayHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags()]
        private enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x16,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [DllImport("user32.dll")]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int ENUM_REGISTRY_SETTINGS = -2;

        public static int GetMaximumRefreshRate()
        {
            int refreshRate = 0;

            DISPLAY_DEVICE device = new DISPLAY_DEVICE();
            DEVMODE deviceMode = new DEVMODE();
            uint deviceNum = 0;

            device.cb = Marshal.SizeOf(device);
            while (EnumDisplayDevices(null, deviceNum, ref device, 0))
            {
                if (EnumDisplaySettings(device.DeviceName, ENUM_CURRENT_SETTINGS, ref deviceMode))
                {
                    if (deviceMode.dmDisplayFrequency > refreshRate)
                        refreshRate = deviceMode.dmDisplayFrequency;
                }
                deviceNum++;
                device.cb = Marshal.SizeOf(device);
            }

            return refreshRate;
        }

        /// <summary>
        /// Try to figure out if the form is Left/Right docked in windows 7.
        /// </summary>
        public static bool IsFormDocked(Form form)
        {
            Screen screen = DisplayHelper.GetCurrentScreen(form);
            
            // Just use the size. I see no API calls that I can use to determine if my window is docked.
            if (form.Height == screen.WorkingArea.Height 
                && (form.Left == screen.WorkingArea.Left || form.Right == screen.WorkingArea.Right)
                && (form.Width == screen.WorkingArea.Width / 2 || form.Width == screen.WorkingArea.Width / 2 + 1))
                return true;

            return false;
        }

        /// <summary>
        /// Try to figure out if the form is vertically bound (in windows 7).
        /// A user can do this by resizing the top of the window to the top of the screen.
        /// I don't know that many people are really aware of this, but I want to ignore
        /// these situations as well.
        /// </summary>
        public static bool IsFormVerticallyBound(Form form)
        {
            Screen screen = DisplayHelper.GetCurrentScreen(form);

            if (form.Height == screen.WorkingArea.Height
                && (form.Left == screen.WorkingArea.Left || form.Right == screen.WorkingArea.Right)
                && (form.Width == screen.WorkingArea.Width / 2 || form.Width == screen.WorkingArea.Width / 2 + 1))
                return true;

            return false;
        }

        /// <summary>
        /// Determine the screen best suited to be considered the parent of the form 
        /// by analyzing its position in relation to the screens.
        /// </summary>
        public static Screen GetCurrentScreen(Form form)
        {
            int screenNumber;
            return DisplayHelper.GetCurrentScreen(form, out screenNumber);
        }

        /// <summary>
        /// Determine the screen best suited to be considered the parent of the form 
        /// by analyzing its position in relation to the screens.
        /// </summary>
        public static Screen GetCurrentScreen(Form form, out int screenNumber)
        {
            ///////////
            // Find the screen we're on (use the middle of the form).
            System.Drawing.Point point = new Point();
            point.X = form.Left + form.Width / 2;
            point.Y = form.Top + form.Height / 2;

            int screenNum = 0;
            Screen screenToUse = null;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(point))
                {
                    screenToUse = screen;
                    break;
                }
                screenNum++;
            }
            if (screenToUse == null)
            {
                screenNum = 0;
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.Bounds.IntersectsWith(form.Bounds))
                    {
                        screenToUse = screen;
                        break;
                    }
                    screenNum++;
                }
            }
            if (screenToUse == null) // Yes, it could happen!
            {
                screenNum = 0;
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.Bounds.IntersectsWith(form.Bounds))
                    {
                        screenToUse = Screen.PrimaryScreen;
                        break;
                    }
                    screenNum++;
                }
            }

            screenNumber = screenNum;
            return screenToUse;
        }
    }
}
