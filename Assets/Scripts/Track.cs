using UnityEngine;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	[ProtoContract]
	public class Track
	{
		//public
		[ProtoMember(1)]
		public List<Beat> beats = new List<Beat>();
		[ProtoMember(2)]
		public SColor Scol;
		public Color color
		{
			get { return Scol.color; }
		}

		public Track()
		{
		}

		public Track(Color color)
		{
			this.Scol = new SColor(color);
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
