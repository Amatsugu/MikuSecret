using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TheDarkVoid
{
	public class ClickDragable : MonoBehaviour, IPointerClickHandler, IDragHandler
	{
		//Public
		public float min, max;
		public float value;
		public string eventCallback;
		public Image dragable;

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
			this.value = min;
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
			Vector2 p = dragable.rectTransform.position;
			value = p.x = Mathf.Clamp(pos, min, max);
			dragable.rectTransform.position = p;
			EventManager.TriggerEvent(eventCallback);
		}
	}
}
