using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class Track
	{
		//public
		public List<Beat> beats = new List<Beat>();
		public Color color;

		public Track()
		{
			this.color = new Color(Random.Range(0f,1f), Random.Range(0f,0f), Random.Range(0f,1f), 1f);
			Debug.Log(this.color);
		}

		public Track(Color color)
		{
			this.color = color;
		}

		public void AddBeat(Beat beat)
		{
			beats.Add(beat);
		}

		public void RemoveBeat(Beat beat)
		{
			beats.Remove(beat);
		}
	}
}
