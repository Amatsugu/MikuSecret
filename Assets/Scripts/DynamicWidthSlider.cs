using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TheDarkVoid
{
	public class DynamicWidthSlider : MonoBehaviour, IDragHandler, IBeginDragHandler
	{
		//Public
		public Image handle;
		public float value;
		public float width
		{
			set
			{
				_width = Mathf.Clamp(value, 0, 1);
				handle.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _xMax * value);
				Vector2 pos = handle.rectTransform.localPosition;
				pos.x = Mathf.Clamp(pos.x, 0, _xMax - (_width * _xMax));
				handle.rectTransform.localPosition = pos;
				this.value = pos.x / _xMax;
				EventManager.TriggerEvent(eventCallback);
			}
			get
			{
				return _width;
			}
		}
		public string eventCallback;
		//Private
		private float _width = 0;
		private float _clickOffset = -1;
		private float _xMin, _xMax;

		void Start()
		{
			if (eventCallback == "" || eventCallback == " ")
			{
				eventCallback = gameObject.name;
			}
			RectTransform rt = GetComponent<Image>().rectTransform;
			_xMin = rt.position.x;
			_xMax = rt.rect.width;
			width = 0.2f;
		}

		public void OnDrag(PointerEventData eventData)
		{

			Vector2 pos = handle.rectTransform.localPosition;
			pos.x = Mathf.Clamp(Input.mousePosition.x - _xMin -_clickOffset, 0, _xMax - (_width * _xMax));
			handle.rectTransform.localPosition = pos;
			value = pos.x / _xMax;
			EventManager.TriggerEvent(eventCallback);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			Vector2 pos = handle.rectTransform.position;
			_clickOffset = eventData.position.x - pos.x;
		}
	}
}
