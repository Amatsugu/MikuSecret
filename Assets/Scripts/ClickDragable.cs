using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LuminousVector
{
	public class ClickDragable : MonoBehaviour, IPointerClickHandler, IDragHandler
	{
		//Public
		public float min, max;
		public float value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				Vector2 p = dragable.rectTransform.position;
				p.x = Mathf.Clamp(value, min, max); ;
				dragable.rectTransform.position = p;
			}
		}
		public float offsetValue
		{
			get { return _value - min; }
		}
		public string eventCallback;
		public Image dragable;

		private float _value;

		void Start()
		{
			if(eventCallback == "" || eventCallback == " ")
			{
				eventCallback = gameObject.name;
			}
		}

		public void SetMinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
			_value = min;
		}

		public void OnDrag(PointerEventData eventData)
		{
			MoveDragable(eventData.position.x);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			MoveDragable(eventData.position.x);
		}

		void MoveDragable(float pos)
		{
			Debug.Log(pos);
			value = pos;
			//Vector2 p = dragable.rectTransform.position;
			//_value = p.x = Mathf.Clamp(pos, min, max);
			//dragable.rectTransform.position = p;
			EventManager.TriggerEvent(eventCallback);
		}
	}
}
