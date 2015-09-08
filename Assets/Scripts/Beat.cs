using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheDarkVoid
{
	[ProtoContract]
	public class Beat
	{
		[ProtoMember(1)]
		public float time;
		[ProtoMember(2)]
		public float duration;
		public float startPosition;
		public bool hit = false;
		public float hitStart = float.NegativeInfinity;
		public float startAcc;

		private Image _beatImage;
		private Image _trail;

		public Beat()
		{

		}

		public Beat(float time)
		{
			this.time = time;
			this.duration = .8f;
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

		public void Create(Image beatImage, Image trail)
		{
			Create(beatImage);
			_trail = trail;
		}

		public void UpdatePosition(Vector3 position)
		{
			_beatImage.transform.position = position;
		}

		public void UpdatePosition(Vector3 position, float trailHeight)
		{
			_beatImage.transform.position = position;
			_trail.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, trailHeight);
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
