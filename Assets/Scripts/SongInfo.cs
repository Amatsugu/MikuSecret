using UnityEngine;
using ProtoBuf;
using System.Collections;

namespace LuminousVector
{
	[ProtoContract]
	public class SongInfo
	{
		[ProtoMember(1)]
		public string title;
		[ProtoMember(2)]
		public string artist;
		[ProtoMember(3)]
		public string album;
		[ProtoMember(4)]
		public string year;
		[ProtoMember(5)]
		public string difficulty;
		[ProtoMember(6)]
		public string creator;

		public SongInfo()
		{

		}

		public SongInfo(string title, string artist, string album, string year, string difficulty, string creator)
		{
			this.title = title;
			this.artist = artist;
			this.album = album;
			this.year = year;
			this.difficulty = difficulty;
			this.creator = creator;
		}
	}
}
