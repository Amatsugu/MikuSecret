using UnityEngine;
using UnityEngine.Events;


namespace LuminousVector
{
	public class UIContextMenuItem
	{
		public string name;
		public UnityAction<object> action;

		public UIContextMenuItem(string name, UnityAction<object> action)
		{
			this.name = name;
			this.action = action;
		}
	}
}
