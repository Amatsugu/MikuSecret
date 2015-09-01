using UnityEngine;
using System.Collections;

namespace TheDarkVoid
{
	public class SongInfo
	{
		public string title;
		public string artist;
		public string album;
		public string year;
		public string difficulty;

		public SongInfo(string title, string artist, string album, string year, string difficulty)
		{
			this.title = title;
			this.artist = artist;
			this.album = album;
			this.year = year;
			this.difficulty = difficulty;
		}
	}
}
