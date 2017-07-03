using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuminousVector
{
	public class UITrackConfigWindow : UIWindowManager
	{
		public InputField trackNameField;
		public UIColorPicker colorPicker;

		private UITrackManager track;

		public void Set(UITrackManager track)
		{
			SetHeader(track.track.name);
			this.track = track;
			shouldClose = true;
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
