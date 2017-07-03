using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace LuminousVector
{
	public class UIBeatManager : MonoBehaviour, IPointerClickHandler
	{
		//public
		public float time
		{
			get { return _beat.time; }
		}
		public Vector2 positon
		{
			get { return instance.localPosition; }
			set { instance.localPosition = value; }
		}
		public float duration
		{
			set
			{
				_beat.duration = value;
				trail.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Utils.TransformToTimelinePos(value));
			}
		}
		public Beat beat { get { return _beat; } }
		public UITrackManager parent { get { return _parent; } }
		public Image image;
		public Image trail;
		[HideInInspector]
		public RectTransform instance;

		//private
		private Beat _beat;
		private UITrackManager _parent;

		public void Set(Beat beat, UITrackManager parent)
		{
			_beat = beat;
			duration = beat.duration;
			instance = GetComponent<RectTransform>();
			_parent = parent;
		}

		public void Destroy()
		{
			Destroy(instance.gameObject);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if(eventData.button == PointerEventData.InputButton.Right)
			{
				Debug.Log("Open Menu");
				UIContextMenuManager.OpenContextMenu("beatMenu", eventData.position, this);
			}
		}
	}
}
