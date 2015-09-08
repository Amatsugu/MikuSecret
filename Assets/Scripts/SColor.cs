using UnityEngine;
using ProtoBuf;
using System.Collections;

namespace TheDarkVoid
{
	[ProtoContract]
	public class SColor
	{
		//Public
		[ProtoMember(1)]
		public float r;
		[ProtoMember(2)]
		public float g;
		[ProtoMember(3)]
		public float b;
		[ProtoMember(4)]
		public float a;
		public Color color
		{
			get
			{
				return new Color(r, g, b, a);
			}
		}

		public SColor()
		{
		}

		public SColor(Color col)
		{
			this.r = col.r;
			this.g = col.g;
			this.b = col.b;
			this.a = col.a;
		}

		public SColor(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public SColor(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1;
		}
	}
}
