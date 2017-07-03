using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LuminousVector
{
	public class UIFileMenu : MonoBehaviour, IPointerClickHandler
	{
		private Vector2 _pos;

		void Start()
		{
			_pos = transform.position;
			Rect img = GetComponent<Image>().rectTransform.rect;
			_pos.y -= img.height;
			_pos.x -= img.width;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (UIContextMenuManager.instance.openMenu == "fileMenu")
				UIContextMenuManager.CloseContextMenu();
			else
				UIContextMenuManager.OpenContextMenu("fileMenu", _pos, null);
		}
	}
}
