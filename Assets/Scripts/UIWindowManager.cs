using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class UIWindowManager : MonoBehaviour
	{
		//Public
		public Text headerText;
		public string defaultTitle;
		public bool appendMode = false;
		public float animationScale = .5f;
		public float blackoutOpacity = .75f;

		//Private
		private Image window;

		//Protected
		protected float _windowState = 0;

		void Start()
		{
			window = GetComponent<Image>();
			headerText.text = defaultTitle;
		}

		protected void SetHeader(string header)
		{
			if (appendMode)
				headerText.text = defaultTitle + ": \"" + header + "\"";
			else 
				headerText.text = header;
		}

		public void OpenWindow()
		{
			gameObject.SetActive(true);
			StartCoroutine(AnimateWindow(1));
		}

		public void CloseWindow()
		{
			StartCoroutine(AnimateWindow(-1));
		}

		void Update()
		{
			if (_windowState == 0)
				return;
			if (Input.GetKeyUp(KeyCode.Escape))
				CloseWindow();
		}

		protected void UpdateWindow()
		{
			Color c = window.color;
			c.a = blackoutOpacity * (_windowState);
			window.color = c;
			if (_windowState == 0)
				gameObject.SetActive(false);
		}

		IEnumerator AnimateWindow(float dir)
		{
			Debug.Log("Window Animation Start: " + dir);
			while (true)
			{
				if (dir >= 0)
				{
					_windowState += Time.deltaTime * animationScale;
					if (_windowState >= 1)
					{
						_windowState = 1;
						break;
					}
				}
				else
				{
					_windowState -= Time.deltaTime * animationScale;
					if (_windowState <= 0)
					{
						_windowState = 0;
						break;
					}
				}
				UpdateWindow();
				yield return new WaitForEndOfFrame();
			}
			UpdateWindow();
			Debug.Log("Window Animation End");
		}
	}
}
