using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace LuminousVector
{
	public class ContextMenuItem : MonoBehaviour, IPointerClickHandler
	{
		public UnityAction<object> action;
		public object target;

		public ContextMenuItem Set(UIContextMenuItem item, object target)
		{
			this.action = item.action;
			this.target = target;
			return this;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				action.Invoke(target);
				UIContextMenuManager.CloseContextMenu();
			}
		}
	}
}
