using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace com.LuminousVector
{
	public class UIWindowManager : MonoBehaviour
	{
		//Public
		public Text headerText;
		public string defaultTitle;
		public bool appendMode = false;
		public float animationScale = 2f;
		public float blackoutOpacity = .5f;
		public UIWindowManager confirmationWindow;
		[HideInInspector]
		public bool shouldClose = true;

		//Private
		private Image window;

		//Protected
		protected float _windowState = 0;
		protected UIWindowManager parent;

		void Start()
		{
			window = GetComponent<Image>();
			headerText.text = defaultTitle;
		}

		void OnDisable()
		{
			if (confirmationWindow == null)
				return;
			confirmationWindow.SetActive(false);
		}

		protected void SetActive(bool state)
		{
			gameObject.SetActive(state);
		}

		protected UIWindowManager SetHeader(string header)
		{
			if (appendMode)
				headerText.text = defaultTitle + ": \"" + header + "\"";
			else 
				headerText.text = header;
			return this;
		}

		public UIWindowManager OpenWindow()
		{
			return OpenWindow(null);
		}

		public UIWindowManager OpenWindow(UIWindowManager parent)
		{
			this.parent = parent;
			gameObject.SetActive(true);
			StartCoroutine(AnimateWindow(1));
			return this;
		}

		public void CloseWindow()
		{
			if(shouldClose)
				StartCoroutine(AnimateWindow(-1));
			else
			{
				confirmationWindow.OpenWindow(this);
			}
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
			if (window == null)
				Start();
			Color c = window.color;
			c.a = blackoutOpacity * (_windowState);
			window.color = c;
			if (_windowState == 0)
				gameObject.SetActive(false);
		}

		protected IEnumerator AnimateWindow(float dir)
		{
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
		}
	}
}
