using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song
{
	public SongInfo info;
	public AudioClip song;
	public List<Beat> beats;

	public Song(SongInfo info, AudioClip song)
	{
		this.info = info;
		this.song = song;
		this.beats = new List<Beat>();
	}

	public Song(SongInfo info, AudioClip song, List<Beat> beats)
	{
		this.info = info;
		this.song = song;
		this.beats = beats;
	}

	public Song(AudioClip song)
	{
		this.info = new SongInfo("no title", "no artist", "no album", "no year");
		this.song = song;
		this.beats = new List<Beat>();
	}

	public void SetBeats(List<Beat> beats)
	{
		this.beats = beats;
	}
}
