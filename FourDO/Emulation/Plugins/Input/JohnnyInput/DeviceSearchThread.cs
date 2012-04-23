using System.Collections.Generic;
using System.Linq;
using SlimDX.DirectInput;
using System.Threading;

namespace FourDO.Emulation.Plugins.Input.JohnnyInput
{
	internal class DeviceSearchThread
	{
		private bool stopSignal;
		private Thread deviceCheckerThread;
		private List<DeviceInstance> deviceCheckerResults;
		
		private readonly object stateSemaphore = new object();
		private readonly object resultSemaphore = new object();

		private DirectInput directInput = new DirectInput();

	    public void Start()
		{
			lock (this.stateSemaphore)
			{
				if (this.deviceCheckerThread != null)
					return;

				this.directInput = new DirectInput();

				this.deviceCheckerThread = new Thread(new ThreadStart(this.CheckerWorkerThread));
				this.deviceCheckerThread.Start();
			}
		}

		public List<DeviceInstance> GetDevices()
		{
			lock (this.resultSemaphore)
			{
				// Make a copy of the last result and return it.
				var returnValue = new List<DeviceInstance>();
				if (this.deviceCheckerResults != null)
					this.deviceCheckerResults.ForEach(returnValue.Add);
				return returnValue;
			}
		}

		public void Stop()
		{
			lock (this.stateSemaphore)
			{
				if (this.deviceCheckerThread != null)
				{
					this.stopSignal = true;
					this.deviceCheckerThread.Abort();
					this.deviceCheckerThread.Join();

					this.directInput.Dispose();

					this.deviceCheckerThread = null;
				}
			}
		}

		private void CheckerWorkerThread()
		{
			do 
			{
				// NOTE: This function call can take over 100 milliseconds on some machines!
				//       In fact, that's why we have it on its own thread.
				var joystickDevices = this.directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).ToList<DeviceInstance>();

				// Use a semaphore here so we don't copy into the results list while it's in use.
				lock (this.resultSemaphore)
				{
					this.deviceCheckerResults = joystickDevices;
				}

				Thread.Sleep(1000);
			} while (this.stopSignal == false);
		}
	}
}
