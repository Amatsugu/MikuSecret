using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace LuminousVector
{
	public abstract class UIWindowManager : MonoBehaviour
	{
		//Public
		public Text headerText;
		public string defaultTitle;
		public bool appendMode = false;
		public float blackoutOpacity = .5f;
		public UIWindowManager confirmationWindow;
		public Image window;
		public bool closeConfirmation = false;
		[HideInInspector]
		public bool shouldClose = true;
		//Private
		private Image windowShadow;
		private float windowWidth;
		private bool _hasInit = false;
		private float animationScale = 2f;

		//Protected
		protected float _windowState = 0;
		protected UIWindowManager parent;


		void OnEnable()
		{
			if (!_hasInit)
			{
				animationScale = GameRegistry.GetFloat("windowAnimationScale", animationScale);
				windowShadow = GetComponent<Image>();
				if (windowWidth == 0)
					windowWidth = window.rectTransform.rect.width;
				CenterWindow();
				headerText.text = defaultTitle;
				if (confirmationWindow == null && closeConfirmation)
					confirmationWindow = FindObjectOfType<UIConfirmationWindow>() as UIConfirmationWindow;
				_hasInit = true;
			}
		}

		protected void CenterWindow()
		{
			if (windowShadow == null)
				windowShadow = GetComponent<Image>();
			Vector2 pos = window.rectTransform.position;
			pos.x = (windowShadow.rectTransform.rect.width / 2) - (windowWidth / 2);
			window.rectTransform.position = pos;
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
			shouldClose = true;
			CenterWindow();
			return OpenWindow(null);
		}

		public UIWindowManager OpenWindow(UIWindowManager parent)
		{
			if (_windowState == 1)
				return this;
			this.parent = parent;
			SetActive(true);
			StartCoroutine(AnimateWindow(1));
			return this;
		}

		public void CloseWindow()
		{
			if (!closeConfirmation)
				StartCoroutine(AnimateWindow(-1));
			else
			{
				if (shouldClose)
					StartCoroutine(AnimateWindow(-1));
				else
				{
					confirmationWindow.OpenWindow(this);
				}
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
			if (windowShadow == null)
				OnEnable();
			Color c = windowShadow.color;
			c.a = blackoutOpacity * (_windowState);
			windowShadow.color = c;
			window.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, windowWidth * _windowState);
			if (_windowState == 0)
				gameObject.SetActive(false);
		}

		protected IEnumerator AnimateWindow(float dir)
		{
			while (true)
			{
				//Open
				if (dir >= 0)
				{
					_windowState += Time.deltaTime * animationScale;
					if (_windowState >= 1)
					{
						_windowState = 1;
						break;
					}
				}
				else //Close
				{
					_windowState -= Time.deltaTime * animationScale;
					if (_windowState <= 0)
					{
						_windowState = 0;
						OnWindowClose();
						break;
					}
				}
				UpdateWindow();
				yield return new WaitForEndOfFrame();
			}
			UpdateWindow();
		}

		public virtual void OnWindowClose()
		{

		}
	}

}
