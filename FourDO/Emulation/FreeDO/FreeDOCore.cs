using System;
using System.Runtime.InteropServices;
using FourDO.Properties;

namespace FourDO.Emulation.FreeDO
{
	public enum FixMode
	{
		FIX_BIT_TIMING_1 = 0x00000001,
		FIX_BIT_TIMING_2 = 0x00000002,
		FIX_BIT_TIMING_3 = 0x00000004,
		FIX_BIT_TIMING_4 = 0x00000008,
		FIX_BIT_TIMING_5 = 0x00000010,
		FIX_BIT_TIMING_6 = 0x00000020,
		FIX_BIT_TIMING_7 = 0x00000040,
		FIX_BIT_GRAPHICS_STEP_Y = 0x00080000,
	}



	internal static class FreeDOCore
	{
		#region Public Delegates

		public delegate void ReadRomDelegate(IntPtr romPointer);
		public static ReadRomDelegate ReadRomEvent { get; set; }

		public delegate void ReadNvramDelegate(IntPtr nvramPointer);
		public static ReadNvramDelegate ReadNvramEvent { get; set; }

		public delegate void WriteNvramDelegate(IntPtr nvramPointer);
		public static WriteNvramDelegate WriteNvramEvent { get; set; }

		public delegate IntPtr SwapFrameDelegate(IntPtr currentFrame);
		public static SwapFrameDelegate SwapFrameEvent { get; set; }

		public delegate void PushSampleDelegate(uint dspSample);
		public static PushSampleDelegate PushSampleEvent { get; set; }

		public delegate int GetPbusLengthDelegate();
		public static GetPbusLengthDelegate GetPbusLengthEvent { get; set; }

		public delegate IntPtr GetPbusDataDelegate();
		public static GetPbusDataDelegate GetPbusDataEvent { get; set; }

		public delegate void KPrintDelegate(IntPtr value);
		public static KPrintDelegate KPrintEvent { get; set; }

		public delegate void DebugPrintDelegate(IntPtr value);
		public static DebugPrintDelegate DebugPrintEvent { get; set; }

		public delegate void FrameTriggerDelegate();
		public static FrameTriggerDelegate FrameTriggerEvent { get; set; }

		public delegate void Read2048Delegate(IntPtr buffer);
		public static Read2048Delegate Read2048Event { get; set; }

		public delegate int GetDiscSizeDelegate();
		public static GetDiscSizeDelegate GetDiscSizeEvent { get; set; }

		public delegate void OnSectorDelegate(int sectorNumber);
		public static OnSectorDelegate OnSectorEvent { get; set; }

		public delegate void ArmSyncDelegate(int armSyncValue);
		public static ArmSyncDelegate ArmSync { get; set; }

		#endregion // Public Types

		#region Private Types

		private enum ExternalFunction
		{
			EXT_READ_ROMS = 1,
			EXT_READ_NVRAM = 2,
			EXT_WRITE_NVRAM = 3,
			EXT_SWAPFRAME = 5, //frame swap (in mutlithreaded) or frame draw(single treaded)
			EXT_PUSH_SAMPLE = 6, //sends sample to the buffer
			EXT_GET_PBUSLEN = 7,
			EXT_GETP_PBUSDATA = 8,
			EXT_KPRINT = 9,
			EXT_DEBUG_PRINT = 10,
			EXT_FRAMETRIGGER_MT = 12, //multitasking
			EXT_READ2048 = 14, //for XBUS Plugin
			EXT_GET_DISC_SIZE = 15,
			EXT_ON_SECTOR = 16,
			EXT_ARM_SYNC = 17,
			FDP_GET_FRAME_BITMAP = 18
		}

		private enum InterfaceFunction
		{
			FDP_FREEDOCORE_VERSION = 0,
			FDP_INIT = 1,  //set ext_interface
			FDP_DESTROY = 2,
			FDP_DO_EXECFRAME = 3,  //execute 1/60 of second
			FDP_DO_FRAME_MT = 4,  //multitasking
			FDP_DO_EXECFRAME_MT = 5,  //multitasking
			FDP_DO_LOAD = 6,  //load state from buffer, returns !NULL if everything went smooth
			FDP_GET_SAVE_SIZE = 7,  //return size of savestatemachine
			FDP_DO_SAVE = 8,  //save state to buffer
			FDP_GETP_NVRAM = 9,  //returns ptr to NVRAM 32K
			FDP_GETP_RAMS = 10, //returns ptr to RAM 3M
			FDP_GETP_ROMS = 11, //returns ptr to ROM 2M
			FDP_GETP_PROFILE = 12, //returns profile pointer, sizeof = 3M/4
			FDP_BUGTEMPORALFIX = 13, // JMK NOTE: Unused?
			FDP_SET_ARMCLOCK = 14,
			FDP_SET_TEXQUALITY = 15,
			FDP_GETP_WRCOUNT = 16, // JMK NOTE: Unused?
			FDP_SET_FIX_MODE = 17,
			FDP_GET_FRAME_BITMAP = 18,
            FDP_GET_BIOS_TYPE = 19,
            FDP_SET_ANVIL = 20
		}

		#endregion // Private Types

		[DllImport(@"FreeDOCore.dll", EntryPoint = "_freedo_Interface")]
		private static extern IntPtr FreeDoInterface(int procedure, IntPtr datum);

		public static int GetCoreVersion()
		{
			return (int)FreeDoInterface((int)InterfaceFunction.FDP_FREEDOCORE_VERSION, (IntPtr)0);
		}

		public static uint GetSaveSize()
		{
			return (uint)FreeDoInterface((int)InterfaceFunction.FDP_GET_SAVE_SIZE, (IntPtr)0);
		}

		public static IntPtr GetPointerNVRAM()
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_GETP_NVRAM, (IntPtr)0);
		}

		public static IntPtr GetPointerRAM()
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_GETP_RAMS, (IntPtr)0);
		}

		public static IntPtr GetPointerROM()
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_GETP_ROMS, (IntPtr)0);
		}

		public static IntPtr GetPointerProfile()
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_GETP_PROFILE, (IntPtr)0);
		}

		public static int Initialize()
		{
			return (int)FreeDoInterface(
					(int)InterfaceFunction.FDP_INIT,
					Marshal.GetFunctionPointerForDelegate(externalInterfaceDelegate));
		}

        public static int GetBiosType()
        {
            return (int)FreeDoInterface((int)InterfaceFunction.FDP_GET_BIOS_TYPE, (IntPtr)0);

        }

        public static int SetAnvilFix(int flag)
        {
            return (int)FreeDoInterface((int)InterfaceFunction.FDP_SET_ANVIL, (IntPtr)flag);

        }

		public static void Destroy()
		{
			FreeDoInterface((int)InterfaceFunction.FDP_DESTROY, (IntPtr)0);
		}

		public static void DoExecuteFrame(IntPtr VDLFrame)
		{
			FreeDoInterface((int)InterfaceFunction.FDP_DO_EXECFRAME, VDLFrame);
		}

		public static void DoExecuteFrameMultitask(IntPtr VDLFrame)
		{
			FreeDoInterface((int)InterfaceFunction.FDP_DO_EXECFRAME_MT, VDLFrame);
		}

		public static void DoFrameMultitask(IntPtr VDLFrame)
		{
			FreeDoInterface((int)InterfaceFunction.FDP_DO_FRAME_MT, VDLFrame);
		}

		public static bool DoLoad(IntPtr loadBuffer)
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_DO_LOAD, loadBuffer).ToInt32() != 0;
		}

		public static void DoSave(IntPtr saveBuffer)
		{
			FreeDoInterface((int)InterfaceFunction.FDP_DO_SAVE, saveBuffer);
		}

		public static IntPtr SetArmClock(int clock)
		{
			// TODO: Untested!
			return FreeDoInterface((int)InterfaceFunction.FDP_SET_ARMCLOCK, new IntPtr(clock));
		}

		public static IntPtr SetFixMode(int fixMode)
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_SET_FIX_MODE, new IntPtr(fixMode));
		}

		public static IntPtr SetTextureQuality(int textureScalar)
		{
			return FreeDoInterface((int)InterfaceFunction.FDP_SET_TEXQUALITY, new IntPtr(textureScalar));
		}

		public static void GetFrameBitmap(
			IntPtr sourceFrame,
			IntPtr destinationBitmap,
			int destinationBitmapWidthPixels,
			BitmapCrop resultingBitmapCrop,
			int copyWidthPixels,
			int copyHeightPixels,
			bool addBlackBorder,
			bool copyPointlessAlphaByte,
			bool allowCrop,
			ScalingAlgorithm scalingAlgorithm,
			out int resultingWidth,
			out int resultingHeight)
		{
			var crop = new GetFrameBitmapCrop();
			GCHandle cropHandle;
			RawSerialize(crop, out cropHandle);

			var param = new GetFrameBitmapParams();
			param.sourceFrame = sourceFrame;
			param.destinationBitmap = destinationBitmap;
			param.destinationBitmapWidthPixels = destinationBitmapWidthPixels;
			param.bitmapCrop = cropHandle.AddrOfPinnedObject();
			param.copyWidthPixels = copyWidthPixels;
			param.copyHeightPixels = copyHeightPixels;
			param.addBlackBorder = (byte)(addBlackBorder ? 1 : 0);
			param.copyPointlessAlphaByte = (byte)(copyPointlessAlphaByte ? 1 : 0);
			param.allowCrop = (byte)(allowCrop ? 1 : 0);
			param.scalingAlgorithm = (int)scalingAlgorithm;

			// Copy into locked structure.
			GCHandle paramHandle;
			RawSerialize(param, out paramHandle);

			/////////////////////
			// Run!
			FreeDoInterface((int)InterfaceFunction.FDP_GET_FRAME_BITMAP, paramHandle.AddrOfPinnedObject());
			Marshal.PtrToStructure(cropHandle.AddrOfPinnedObject(), crop);
			Marshal.PtrToStructure(paramHandle.AddrOfPinnedObject(), param);
			// Get resulting crop.
            
            if (!Properties.Settings.Default.WindowScale)
            {
                resultingBitmapCrop.Top = crop.top;
                resultingBitmapCrop.Left = crop.left;
                resultingBitmapCrop.Right = crop.right;
                resultingBitmapCrop.Bottom = crop.bottom;
            }
            else if (param.scalingAlgorithm == 1 || Properties.Settings.Default.RenderHighResolution == true)
            {
                resultingBitmapCrop.Top = crop.top + 32;
                resultingBitmapCrop.Left = crop.left + 32;
                resultingBitmapCrop.Right = crop.right + 32;
                resultingBitmapCrop.Bottom = crop.bottom + 32;
            }
            else if (param.scalingAlgorithm == 2)
            {
                resultingBitmapCrop.Top = crop.top + 48;
                resultingBitmapCrop.Left = crop.left + 48;
                resultingBitmapCrop.Right = crop.right + 48;
                resultingBitmapCrop.Bottom = crop.bottom + 48;
            }
            else if (param.scalingAlgorithm == 3)
            {
                resultingBitmapCrop.Top = crop.top + 64;
                resultingBitmapCrop.Left = crop.left + 64;
                resultingBitmapCrop.Right = crop.right + 64;
                resultingBitmapCrop.Bottom = crop.bottom + 64;
            }
            else
            {
                resultingBitmapCrop.Top = crop.top + 16;
                resultingBitmapCrop.Left = crop.left + 16;
                resultingBitmapCrop.Right = crop.right + 16;
                resultingBitmapCrop.Bottom = crop.bottom + 16;
            }

			resultingWidth = param.resultingWidth;
			resultingHeight = param.resultingHeight;

			// Unlock structures.
			paramHandle.Free();
			cropHandle.Free();
		}

		private delegate IntPtr ExternalInterfaceDelegate(int procedure, IntPtr data);
		private static readonly ExternalInterfaceDelegate externalInterfaceDelegate = new ExternalInterfaceDelegate(PrivateExternalInterface);

		private static IntPtr PrivateExternalInterface(int procedure, IntPtr data)
		{
			switch (procedure)
			{
				case (int)ExternalFunction.EXT_READ_ROMS:
					if (ReadRomEvent != null)
						ReadRomEvent(data);
					break;

				case (int)ExternalFunction.EXT_READ_NVRAM:
					if (ReadNvramEvent != null)
						ReadNvramEvent(data);
					break;

				case (int)ExternalFunction.EXT_WRITE_NVRAM:
					if (WriteNvramEvent != null)
						WriteNvramEvent(data);
					break;

				case (int)ExternalFunction.EXT_SWAPFRAME:
					if (SwapFrameEvent != null)
						return SwapFrameEvent(data);
					break;

				case (int)ExternalFunction.EXT_PUSH_SAMPLE:
					if (PushSampleEvent != null)
						PushSampleEvent((uint)data);
					break;

				case (int)ExternalFunction.EXT_GET_PBUSLEN:
					if (GetPbusLengthEvent != null)
						return (IntPtr)GetPbusLengthEvent();
					break;

				case (int)ExternalFunction.EXT_GETP_PBUSDATA:
					if (GetPbusDataEvent != null)
						return GetPbusDataEvent();
					break;

				case (int)ExternalFunction.EXT_KPRINT:
					if (KPrintEvent != null)
						KPrintEvent(data);
					break;

				case (int)ExternalFunction.EXT_DEBUG_PRINT:
					if (DebugPrintEvent != null)
						DebugPrintEvent(data);
					break;

				case (int)ExternalFunction.EXT_FRAMETRIGGER_MT:
					if (FrameTriggerEvent != null)
						FrameTriggerEvent();
					break;

				case (int)ExternalFunction.EXT_READ2048:
					if (Read2048Event != null)
						Read2048Event(data);
					break;

				case (int)ExternalFunction.EXT_GET_DISC_SIZE:
					if (GetDiscSizeEvent != null)
						return (IntPtr)GetDiscSizeEvent();
					break;

				case (int)ExternalFunction.EXT_ON_SECTOR:
					if (OnSectorEvent != null)
						OnSectorEvent((int)data);
					break;

				case (int)ExternalFunction.EXT_ARM_SYNC:
					if (ArmSync != null)
						ArmSync((int)data);
					break;
			}
			return new IntPtr(0);
		}

		private static byte[] RawSerialize(object anything, out GCHandle newHandle)
		{
			int rawsize = Marshal.SizeOf(anything);
			byte[] rawdata = new byte[rawsize];

			newHandle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
			Marshal.StructureToPtr(anything, newHandle.AddrOfPinnedObject(), false);
			return rawdata;
		}

      /*  internal static void SetScale(bool p)
        {
            BitmapCrop rBitCrop = new BitmapCrop();
            rBitCrop.Scaled = 20;

        }*/
    }
}
