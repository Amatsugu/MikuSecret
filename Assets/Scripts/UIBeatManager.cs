using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace com.LuminousVector
{
	public class UIBeatManager : MonoBehaviour
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
		public Image image;
		public Image trail;
		[HideInInspector]
		public RectTransform instance;

		//private
		private Beat _beat;

		public void Set(Beat beat)
		{
			_beat = beat;
			duration = beat.duration;
			instance = GetComponent<RectTransform>();
		}

		public void Destroy()
		{
			Destroy(instance.gameObject);
		}
	}
}
