using UnityEngine;
using System.Collections;

namespace TheDarkVoid
{
	public class UIConfirmationWindow : UIWindowManager
	{

		public void Confirm()
		{
			parent.shouldClose = true;
			parent.CloseWindow();
			CloseWindow();
		}

		public void Cancel()
		{
			shouldClose = true;
			CloseWindow();
		}
	}
}
