using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class Song
	{
		public SongInfo info;
		public AudioClip song;
		public List<Track> tracks;
		public int trackCount;

		public Song(SongInfo info, AudioClip song, int trackCount)
		{
			this.info = info;
			this.song = song;
			this.trackCount = trackCount;
			FillTracks(trackCount);
		}

		public Song(SongInfo info, AudioClip song, List<Track> tracks)
		{
			this.info = info;
			this.song = song;
			this.trackCount = tracks.Count;
			this.tracks = tracks;
		}

		public Song(AudioClip song, int trackCount)
		{
			this.info = new SongInfo("no title", "no artist", "no album", "no year", "normal");
			this.song = song;
			this.trackCount = trackCount;
			FillTracks(trackCount);
		}

		public void SetTrack(List<Beat> beats, int track)
		{
			tracks[track].beats = beats;
		}

		public void SetTracks(List<Track> tracks)
		{
			this.tracks = tracks;
		}

		void FillTracks(int count)
		{
			this.tracks = new List<Track>(trackCount);
			for(int i = 0; i < count; i++)
				this.tracks.Add(new Track());
		}
	}
}
