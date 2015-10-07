using UnityEngine;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{
	[ProtoContract]
	public class Song
	{
		[ProtoMember(1)]
		public SongInfo info;
		[ProtoMember(2)]
		public List<Track> tracks;
		[ProtoMember(3)]
		public int trackCount;
		public AudioClip song
		{
			get
			{
				if (_song == null)
					LoadAudioClip("");
				return _song;
			}
		}
		public bool isLoaded = false;

		[ProtoMember(4)]
		private string songPath;
		private AudioClip _song;

		public Song()
		{
			
		}

		public Song(SongInfo info, string songPath, int trackCount)
		{
			this.info = info;
			this.songPath = songPath;
			this.trackCount = trackCount;
			FillTracks(trackCount);
		}

		public Song(SongInfo info, string songPath, List<Track> tracks)
		{
			this.info = info;
			this.songPath = songPath;
			this.trackCount = tracks.Count;
			this.tracks = tracks;
		}


		public Song(string songPath, int trackCount)
		{
			this.info = new SongInfo("no title", "no artist", "no album", "no year", "normal");
			this.songPath = songPath;
			this.trackCount = trackCount;
			FillTracks(trackCount);
		}

		public IEnumerator LoadAudioClip(string action)
		{
			//Debug.Log("file:///" + Application.dataPath + "/Music/Song.wav");
            WWW file = new WWW("file:///" + Application.dataPath + "/Songs/SongName/Song.wav");
			_song = file.GetAudioClip(false, false);
			while(_song.loadState != AudioDataLoadState.Loaded)
				yield return file;
			isLoaded = true;
			if(action != "")
				EventManager.TriggerEvent(action);
		}

		public void AddTrack(Track track)
		{
			tracks.Add(track);
			trackCount = tracks.Count;
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
				this.tracks.Add(new Track(new Color(Random.Range(0f, 1f), Random.Range(0f, 0f), Random.Range(0f, 1f), 1f)));
		}

		public static Song loadSong(byte[] songData)
		{
			return DataSerializer.deserializeData<Song>(songData);
		}

		public byte[] getSongData()
		{
			return DataSerializer.serializeData<Song>(this);
		}
	}
}
