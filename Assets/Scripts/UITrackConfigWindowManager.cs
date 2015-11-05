using UnityEngine;

namespace TheDarkVoid
{
	public class UITrackConfigWindowManager : UIWindowManager
	{
		private UITrackManager track;
		public void Set(string header, UITrackManager track)
		{
			SetHeader(header);
			this.track = track;
		}

		public void SetTrackName(string trackName)
		{
			SetHeader(trackName);
			track.track.name = trackName;
		}
	}
}
