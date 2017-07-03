using UnityEngine.UI;
using System.Collections;

namespace LuminousVector
{
	public class UIConfirmationWindow : UIWindowManager
	{
		public void Confirm()
		{
			shouldClose = true;
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
