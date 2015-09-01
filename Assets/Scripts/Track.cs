using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class Track
	{
		//public
		public List<Beat> beats = new List<Beat>();

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
