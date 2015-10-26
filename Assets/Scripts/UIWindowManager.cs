using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheDarkVoid
{
	public class UIWindowManager : MonoBehaviour
	{
		//Public
		public Text headerText;

		//Private
		private float _windowState = 0;

		protected void SetHeader(string header)
		{
			headerText.text = header;
		}

		public void OpenWindow()
		{
			StartCoroutine(AnimateWindow(1));
		}

		public void CloseWindow()
		{
			StartCoroutine(AnimateWindow(-1));
		}

		void UpdateWindow()
		{

		}

		IEnumerator AnimateWindow(float dir)
		{
			while (true)
			{
				if (dir >= 0)
				{
					_windowState += Time.deltaTime;
					if (_windowState >= 1)
					{
						_windowState = 1;
						break;
					}
				}
				else
				{
					_windowState -= Time.deltaTime;
					if (_windowState <= 0)
					{
						_windowState = 0;
						break;
					}
				}
				UpdateWindow();
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
