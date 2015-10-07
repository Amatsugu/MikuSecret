using UnityEngine;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class UITrackManager : MonoBehaviour
	{
		//Public
		public GameObject instance;
		public GameObject beatPrefab;

		//Private
		private List<Transform> _beats = new List<Transform>();

		void Start()
		{
			instance = gameObject;
		}

		public void Destroy()
		{
			Destroy(instance);
		}

		public void AddBeat(float pos)
		{
			Transform beat;
			Utils.CreateUIImage(beatPrefab, new Vector2(pos, 0), instance.transform, out beat);
			_beats.Add(beat);
		}

	}
}
