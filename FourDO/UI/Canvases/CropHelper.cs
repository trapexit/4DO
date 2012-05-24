using System;
using FourDO.Utilities;
using FourDO.Emulation.FreeDO;

namespace FourDO.UI.Canvases
{
	internal class CropHelper
	{
		private const double ConsiderationTimeInSeconds = 1.0;

		public BitmapCrop CurrentCrop { get; set; }

		private BitmapCrop lastCandidate = null;
		private long lastCandidateConsideredTime;

		public CropHelper()
		{
			CurrentCrop = new BitmapCrop();
		}

		public bool ConsiderAlternateCrop(BitmapCrop newCrop)
		{
			// If the new crop's no different, this is pretty easy!
			if (newCrop.CompareTo(this.CurrentCrop) == 0)
				return false;

			BitmapCrop acceptedCrop = new BitmapCrop();

			if (newCrop.Top < this.CurrentCrop.Top
				|| newCrop.Left < this.CurrentCrop.Left
				|| newCrop.Bottom < this.CurrentCrop.Bottom
				|| newCrop.Right < this.CurrentCrop.Right)
			{
				// In this case, we're going to accept the new crop because it is
				// indicating that the current crop is cropping out pixels that should
				// otherwise be shown.

				// However, we'll only accept the crop sections that "should" be fixed.
				acceptedCrop.Top = Math.Min(newCrop.Top, this.CurrentCrop.Top);
				acceptedCrop.Left = Math.Min(newCrop.Left, this.CurrentCrop.Left);
				acceptedCrop.Bottom = Math.Min(newCrop.Bottom, this.CurrentCrop.Bottom);
				acceptedCrop.Right = Math.Min(newCrop.Right, this.CurrentCrop.Right);
			}
			else
			{
				// Is this a new candidate? If so, we'll put it under consideration.
				if (this.lastCandidate == null || newCrop.CompareTo(lastCandidate) != 0)
				{
					this.lastCandidate = new BitmapCrop();
					this.lastCandidate.Mimic(newCrop);
					this.lastCandidateConsideredTime = PerformanceCounter.Current;
					return false;
				}

				// Has it been long enough to use this new candidate?
				double time = (PerformanceCounter.Current - this.lastCandidateConsideredTime) / (double)PerformanceCounter.Frequency;
				if (time < ConsiderationTimeInSeconds)
					return false;
				
				// We're going to accept this as the new crop.
				acceptedCrop.Mimic(newCrop);
			}

			//////////////////
			// New candidate accepted.
			this.CurrentCrop.Mimic(acceptedCrop);
			this.lastCandidate = null;

			return true;
		}
	}
}
