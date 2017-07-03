using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace LuminousVector
{
	public class UIContextMenuManager : MonoBehaviour
	{
		public GameObject menuItemPrefab;
		public string openMenu { get { return _openMenu; } }
		public Rect curRect { set
			{
				_curRect = value;
				_curRect.x = instance._thisTransform.position.x;
				_curRect.y = instance._thisTransform.position.y;
				Debug.Log(_curRect);
			} }
		private static UIContextMenuManager CONTEXT_MENU;

		public static UIContextMenuManager instance
		{
			get
			{
				if (!CONTEXT_MENU)
				{
					CONTEXT_MENU = FindObjectOfType<UIContextMenuManager>() as UIContextMenuManager;
					if (!CONTEXT_MENU)
					{
						Debug.LogError("No Event Manager found");
					}
					else
						CONTEXT_MENU.Init();
				}
				return CONTEXT_MENU;
			}
		}

		private Dictionary<string, UIContextMenu> _menus = new Dictionary<string, UIContextMenu>();
		private Image _menuBackground;
		private List<Transform> _menuItems = new List<Transform>();
		private Transform _thisTransform;
		private Rect _curRect;
		private bool _isOpen = false;
		private string _openMenu = "";
		private object _target;

		void Init()
		{
			_menuBackground = GetComponent<Image>();
			_thisTransform = transform;
			CloseContextMenu();
		}

		void Update()
		{
			if (instance._isOpen && Input.GetKeyUp(KeyCode.Mouse0))
			{
				Vector2 mPos = Input.mousePosition;
				mPos.y = Screen.height - mPos.y;
				Rect curRect = instance._curRect;
				UIContextMenu curMenu = _menus[_openMenu];
				Debug.Log(curRect);
				curRect.x -= curMenu.left;
				curRect.width += curMenu.left + curMenu.right;
				curRect.y += curMenu.top;
				curRect.height += curMenu.top + curMenu.bottom;
				Debug.Log(curRect);
				if (!curRect.Contains(mPos))
				{
					CloseContextMenu();
					Debug.Log("Closing menu");
				}
			}
		}

		public static void OpenContextMenu(string id, Vector2 pos, object target)
		{
			Debug.Log("Opening menu: " + id);
			if (instance._isOpen)
			{
				if (instance._openMenu == id)
				{
					instance._thisTransform.position = pos;
					instance._target = target;
					instance.curRect = instance._thisTransform.GetComponent<RectTransform>().rect;
				}
				else
				{
					CloseContextMenu();
					OpenContextMenu(id, pos, target);
				}
			}
			else
			{
				UIContextMenu menu;
				instance._menus.TryGetValue(id, out menu);
				if (menu != null)
				{
					Debug.Log("Menu found, opening... Menu Size: " + menu.size);
					instance._isOpen = true;
					instance._target = target;
					instance._openMenu = id;
					instance._thisTransform.position = pos;
					instance._menuBackground.color = Color.white;
					menu.Open(instance);
					instance._menuBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, menu.size * 25);
					instance.curRect = instance._thisTransform.GetComponent<RectTransform>().rect;
				}else
				{
					Debug.LogError("Menu not found: " + id);
				}
			}
		}

		public static void CloseContextMenu()
		{
			instance._isOpen = false;
			instance._openMenu = null;
			instance._target = null;
			instance._menuBackground.color = Color.clear;
			instance._menuBackground.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
			foreach(Transform t in instance._menuItems)
			{
				Destroy(t.gameObject);
			}
			instance._menuItems.Clear();
		}


		public static UIContextMenu AddContextMenu(string id)
		{
			UIContextMenu menu = new UIContextMenu();
			instance._menus.Add(id, menu);
			return menu;
		}

		public UIContextMenuManager RenderMenuItem(UIContextMenuItem mItem)
		{
			Transform item;
			Utils.CreateUIImage(menuItemPrefab, new Vector2(0, _menuItems.Count * -25), _thisTransform, out item);
			_menuItems.Add(item);
			item.gameObject.AddComponent<ContextMenuItem>().Set(mItem, _target);
			item.GetComponentInChildren<Text>().text = mItem.name;
			return this;
		}

	}
}
