using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheDarkVoid
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
				
			}
		}
		public Image image;
		public RectTransform instance;

		//private
		private Beat _beat;

		public void Set(Beat beat)
		{
			_beat = beat;
			duration = beat.duration;
			instance = GetComponent<RectTransform>();
			image = instance.GetComponent<Image>();
		}

		public void Destroy()
		{
			Destroy(instance.gameObject);
		}
	}
}
