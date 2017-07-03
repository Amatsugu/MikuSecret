using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace LuminousVector
{
	public class UIContextMenu
	{
		private List<UIContextMenuItem> menuItems = new List<UIContextMenuItem>();

		public float top, left, bottom, right;

		public UIContextMenu Open(UIContextMenuManager manager)
		{
			foreach(UIContextMenuItem menuItem in menuItems)
			{
				manager.RenderMenuItem(menuItem);
			}
			return this;
		}

		public UIContextMenu AddMenuItem(string name, UnityAction<object> action)
		{
			return AddMenuItem(new UIContextMenuItem(name, action));
		}

		public UIContextMenu AddMenuItem(UIContextMenuItem menuItem)
		{
			menuItems.Add(menuItem);
			return this;
		}

		public UIContextMenu SetMargins(float top, float left, float bottom, float right)
		{
			this.top = top;
			this.left = left;
			this.bottom = bottom;
			this.right = right;
			return this;
		}

		public int size { get { return menuItems.Count; } }
	}
}
