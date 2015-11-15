using UnityEngine;
using UnityEngine.UI;

namespace TheDarkVoid
{
	public class UITrackConfigWindowManager : UIWindowManager
	{
		public InputField trackNameField;
		public UIColorPicker colorPicker;

		private UITrackManager track;

		public void Set(string header, UITrackManager track)
		{
			SetHeader(header);
			this.track = track;
			colorPicker.color = new SColor(track.track.Scol.color);
		}

		public void UIChange()
		{
			shouldClose = false;
		}


		public void SetTrackName(string trackName)
		{
			SetHeader(trackName);
			track.track.name = trackName;
		}

		public void SaveAndClose()
		{
			shouldClose = true;
			track.track.name = trackNameField.text;
			track.track.Scol = colorPicker.color;
			EventManager.TriggerEvent("UpdateTrackColors");
			CloseWindow();
		}
	}
}
