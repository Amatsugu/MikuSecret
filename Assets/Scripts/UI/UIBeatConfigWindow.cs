using UnityEngine;
using System.Collections;

namespace LuminousVector
{
	public class UIBeatConfigWindow : UIWindowManager
	{
		private UIBeatManager _beat;

		public void Set(UIBeatManager beat)
		{
			_beat = beat;
		}

		public void OnChange()
		{
			shouldClose = false;
		}

		public void Close()
		{

		}

		public override void OnWindowClose()
		{
			_beat.parent.UpdateBeats();
			base.OnWindowClose();
		}
	}
}
