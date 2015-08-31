using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheDarkVoid
{
	public class Beat
	{
		public float time;
		public float duration;
		public float startPosition;
		public bool hit = false;

		private Image _beatImage;


		public Beat(float time)
		{
			this.time = time;
			this.duration = 0;
		}

		public Beat(float time, float duration)
		{
			this.time = time;
			this.duration = duration;
		}

		public void Create(Image beatImage)
		{
			_beatImage = beatImage;
			Vector3 p = GetPosition();
			p.y = startPosition;
			UpdatePosition(p);
		}

		public void UpdatePosition(Vector3 position)
		{
			_beatImage.transform.position = position;
		}

		public Vector3 GetPosition()
		{
			return _beatImage.transform.position;
		}

		public void Destroy()
		{
			GameObject.Destroy(_beatImage.gameObject);
		}

	}
}
