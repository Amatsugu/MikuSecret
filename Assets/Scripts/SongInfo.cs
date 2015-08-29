using UnityEngine;
using System.Collections;

public class SongInfo
{
	public string title;
	public string artist;
	public string album;
	public string year;

	public SongInfo(string title, string artist, string album, string year)
	{
		this.title = title;
		this.artist = artist;
		this.album = album;
		this.year = year;
	}
}
