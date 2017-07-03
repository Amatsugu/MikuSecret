using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System.Collections.Generic;
using System;

namespace LuminousVector
{
	[RequireComponent(typeof(AudioSource))]
	public class SongEditor : MonoBehaviour
	{
		public enum TimelineMode
		{
			min,
			sec
		}
		//static
		public static SongEditor SONG_EDITOR;
		public static SongEditor instance
		{
			get
			{
				if (!SONG_EDITOR)
				{
					SONG_EDITOR = FindObjectOfType<SongEditor>() as SongEditor;
				}
				return SONG_EDITOR;
			}
		}
		//Public
		public Transform timeline;
		public GameObject timeMarker;
		public GameObject trackPrefab;
		public Slider timeScaleSlider;
		public UIDynamicWidthSlider seekSlider;
		public ClickDragable playHead;
		public Image songProgressBar;
		public RectTransform trackScrollView;
		//Windows
		public UITrackConfigWindow trackConfigWindow;
		public UISongConfigurationWindow songInfoWindow;
		public UISaveWindow saveWindow;
		public UIBeatConfigWindow beatWindow;
		//Formatting
		public float minThreshold = 20f;
		public float playHeadPos = 0;
		public float padding = 5f;
		//Getters
		public float timeScale
		{
			get { return _timeScale; }
		}
		public float scrollPos
		{
			get { return _scrollPos; }
		}
		public float songLength
		{
			get { return _songLength; }
		}
		public Song curSong
		{
			get { return _curSong; }
		}

		//Private
		private float _timeScale;
		private Song _curSong;
		private string _dataPath;
		private float _songLength;
		private float _seekPos;
		private float _scrollPos;
		private float _timelineWidth;
		private TimelineMode _mode;
		private List<Transform> _timeMarkers = new List<Transform>();
		private List<UITrackManager> _tracks = new List<UITrackManager>();
		private AudioSource _audio;

		private float _loadStartTime;

		//Load the song and wait for completion 
		void Start()
		{
			//trackConfigWindow.CloseWindow();
			_dataPath = Application.dataPath + "/Songs";
			//if (GameRegistry.GetString("CUR_SONG") == null)
			//	GameRegistry.SetValue("CUR_SONG", "SongName");
			_mode = TimelineMode.sec;
			string curPath = _dataPath + "/" + GameRegistry.GetString("CUR_SONG", "SongName");
			_curSong = Song.loadSong(File.ReadAllBytes(curPath + "/Song.SongData"));
			Debug.Log(_curSong.songPath);
			//_curSong.songPath = "/SongName/Song.wav";
			//File.WriteAllBytes(_dataPath + "/SongName/Song.SongData", _curSong.getSongData());
			_audio = GetComponent<AudioSource>();
			_audio.playOnAwake = false;
			_audio.Stop();
			EventManager.StartListening("songLoaded", SongReady);
			_loadStartTime = Time.time;
			StartCoroutine(_curSong.LoadAudioClip("songLoaded"));
			Debug.Log("Loading Song...");
		}

		//Prepare the UI once the song is loaded
		void SongReady()
		{
			//Set Events
			EventManager.StopListening("songLoaded", SongReady);
			EventManager.StartListening(seekSlider.eventCallback, TimelineSeek);
			EventManager.StartListening(playHead.eventCallback, SetPlayHead);
			EventManager.StartListening("UpdateTrackColors", RenderTracks);
			//Prepare Song
			Debug.Log("Song Loaded... in " + (Time.time - _loadStartTime) + "s");
			
			_songLength = _curSong.song.length;
			_audio.clip = _curSong.song;
			Image tl = timeline.GetComponent<Image>();
			_timelineWidth = tl.rectTransform.rect.width;
			timeScaleSlider.minValue = _timelineWidth / _songLength;
			float xOff = tl.rectTransform.position.x;
			//Create workspace
			playHead.SetMinMax(xOff, _timelineWidth + xOff);
			CreateTimeline(1);
			ZoomTimeline(timeScaleSlider.value);
			RenderTimeline();
			RenderTracks();
			CreateCOntextMenus();
			//_tracks[0].track = _curSong.tracks[0];
		}

		void CreateCOntextMenus()
		{
			//Beat control
			UIContextMenuManager.AddContextMenu("beatMenu").AddMenuItem(new UIContextMenuItem("Remove", RemoveBeat)).AddMenuItem(new UIContextMenuItem("Edit Beat", ConfigureBeat));
			//Track Control
			UIContextMenuManager.AddContextMenu("trackMenu").AddMenuItem(new UIContextMenuItem("Remove", RemoveTrack)).AddMenuItem(new UIContextMenuItem("Edit Track", ConfigureTrack));
			//File Menu
			UIContextMenuManager.AddContextMenu("fileMenu").AddMenuItem(new UIContextMenuItem("Save", SaveSong)).AddMenuItem(new UIContextMenuItem("Save As", SaveSongAs)).AddMenuItem(new UIContextMenuItem("Play", PlaySong)).AddMenuItem(new UIContextMenuItem("Exit", ExitEditor)).SetMargins(20,0,0,0).AddMenuItem(new UIContextMenuItem("Exit", ExitEditor));
		}

		//Create markers of specified increment
		void CreateTimeline(int incremnt)
		{
			DestroyMarkers();
			int min = 0;
			int sec = 0;
			for (int i = 0; i < (_songLength / incremnt); i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(), timeline, out marker);
				marker.GetComponent<Text>().text = min + ":" + Utils.FormatZeros(sec, 0);
				_timeMarkers.Add(marker);
				sec += incremnt;
				if (sec >= 60)
				{
					min += 1;
					sec = 0;
				}
			}
		}

		//Get rid of the old markers to be replaced
		void DestroyMarkers()
		{
			foreach (Transform m in _timeMarkers)
			{
				Destroy(m.gameObject);
			}
			_timeMarkers.Clear();
		}

		//Adjust the "zoom" level of the timeline
		public void ZoomTimeline(float zoomLevel)
		{
			//Debug.Log(zoomLevel);
			_timeScale = zoomLevel;
			seekSlider.width = (_timelineWidth / (_songLength * _timeScale));
			RenderTimeline();
		}

		//Update the timeline and render the markers in the correct positions
		public void RenderTimeline()
		{
			if (_timeScale <= minThreshold && _mode == TimelineMode.sec)
			{
				_mode = TimelineMode.min;
				CreateTimeline(10);
			}
			else if (_timeScale >= minThreshold && _mode == TimelineMode.min)
			{
				_mode = TimelineMode.sec;
				CreateTimeline(1);
			}
			UpdateTimeline();
		}

		//Update the items on the timeline
		public void UpdateTimeline()
		{
			for (int i = 0; i < _timeMarkers.Count; i++)
			{
				Vector2 pos = _timeMarkers[i].localPosition;
				pos.x = i * _timeScale;
				if (_mode == TimelineMode.min)
					pos.x *= 10f;
				pos.x -= (_scrollPos * _timeScale);
				_timeMarkers[i].localPosition = pos;
			}
			foreach(UITrackManager t in _tracks)
			{
				t.UpdateBeats();
			}
			Vector2 p = songProgressBar.rectTransform.localPosition;
			p.x = -(_scrollPos * _timeScale);
			songProgressBar.rectTransform.localPosition = p;
			//SetPlayHead();
		}

		//Seek the visbile area on the timeline
		public void TimelineSeek()
		{
			_scrollPos = seekSlider.value * _songLength;
			UpdateTimeline();
		}

		//Move the playhead and seek
		public void SetPlayHead()
		{
			playHead.value = Utils.TransformToTimelinePos(_seekPos);
			//SetAudioTime();
			//SetTimeReadout();
		}

		//Seeks to a specfic position in the song
		public void SetAudioTime()
		{
			playHeadPos = playHead.value;
			float phT = Utils.TransformToTime(playHeadPos, playHead.min);
			_audio.time = _seekPos;
		}

		//Update the current progress through the song
		public void SetTimeReadout()
		{
			if (!_audio.isPlaying)
				CancelInvoke("SetTimeReadout");
			float phT = _audio.time;
			int m, s, ns;
			m = (int)(phT / 60);
			s = ((int)phT - m * 60);
			ns = (int)((phT - m * 60 - s) * 100);
			string time = Utils.FormatZeros(m, 2) + ":" + Utils.FormatZeros(s, 2) + ":" + Utils.FormatZeros(ns, 3);
			playHead.dragable.GetComponentInChildren<Text>().text = time;
			songProgressBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, phT * _timeScale);
		}

		//Timeline navigation
		//Make a step in time
		public void TimeStep(int dir)
		{
			dir = (dir >= 0) ? 10 : -10;
			float curTime = _audio.time;
			curTime += dir;
			curTime = (curTime > _songLength) ? _songLength : (curTime < 0) ? 0 : curTime;
			_audio.time = curTime;
		}

		//Change the playback speed
		public void SpeedPlay(int dir)
		{
			dir = (dir >= 0) ? 1 : -1;
			_audio.pitch += dir;
		}

		//Play the song
		public void Play()
		{
			if (_audio.isPlaying)
			{
				Pause();
			}
			else
			{
				_audio.Play();
				SetAudioTime();
				CancelInvoke("SetTimeReadout");
				InvokeRepeating("SetTimeReadout", 0, .01f);
				_audio.pitch = 1;
			}
		}

		//Pause the song
		public void Pause()
		{
			_audio.Pause();
			CancelInvoke("SetTimeReadout");
			SetPlayHead();
		}

		//Stop the song
		public void Stop()
		{
			_audio.Stop();
			CancelInvoke("SetTimeReadout");
			SetPlayHead();
		}

		//Add a new Track to the Song
		public void AddTrack()
		{
			Track t = new Track(SColor.random, "Track " + _curSong.trackCount);
            _curSong.AddTrack(t);
			SpawnTrack(t);
		}

		//Render a list of all tracks into the scrollview
		public void RenderTracks()
		{
			if (_curSong.trackCount == 0)
				return;
			DestroyTracks();
			//Image trackSample = null;
			foreach (Track t in _curSong.tracks)
			{
				SpawnTrack(t);
			}
			//trackScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((_tracks[0].image.rectTransform.rect.height + padding) * _tracks.Count) + padding);
		}

		//Add a new track
		void SpawnTrack(Track t)
		{
			float yPos = -padding;
			yPos -= (_tracks.Count == 0)? 0 : (_tracks[0].image.rectTransform.rect.height + padding)*_tracks.Count;
			Transform track;
			Utils.CreateUIImage(trackPrefab, new Vector2(195, yPos), trackScrollView, out track);
			_tracks.Add(track.GetComponent<UITrackManager>());
			_tracks[_tracks.Count - 1].Set(t);
			trackScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((_tracks[0].image.rectTransform.rect.height + padding) * _tracks.Count) + padding);
		}

		//Update the beat positions
		public void UpdateTracks()
		{
			foreach(UITrackManager t in _tracks)
			{
				t.UpdateBeats();
			}
		}

		//Remove a beat
		public void RemoveBeat(object beat)
		{
			if(beat.GetType() == typeof(UIBeatManager))
			{
				UIBeatManager b = (UIBeatManager)beat;
				b.parent.RemoveBeat(b);
			}
		}

		//Remove a track and ReSort the track list
		public void RemoveTrack(UITrackManager track)
		{
			track.Destroy();
			_curSong.RemoveTrack(track.track);
			_tracks.Remove(track);
			float y = _tracks[0].image.rectTransform.rect.height;
            for (int i = 0; i < _tracks.Count; i++)
			{
				Vector2 pos = _tracks[i].image.rectTransform.localPosition;
				pos.y = padding + ((y + padding)*i);
				pos.y *= -1;
				_tracks[i].image.rectTransform.localPosition = pos;
			}
			trackScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((_tracks[0].image.rectTransform.rect.height + padding) * _tracks.Count) + padding);
		}

		private void RemoveTrack(object track)
		{
			if(track.GetType() == typeof(UITrackManager))
			{
				RemoveTrack((UITrackManager)track);
			}
		}

		//Configure Beat
		private void ConfigureBeat(object beat)
		{
			if(beat.GetType() == typeof(UIBeatManager))
			{
				UIBeatManager b = (UIBeatManager)beat;
				beatWindow.OpenWindow();
				beatWindow.Set(b);
			}
		}

		//Open the configuration window for a selected track
		public void ConfigureTrack(UITrackManager track)
		{
			trackConfigWindow.Set(track);
			trackConfigWindow.OpenWindow();
		}

		private void ConfigureTrack(object track)
		{
			if (track.GetType() == typeof(UITrackManager))
			{
				ConfigureTrack((UITrackManager)track);
			}
		}
		
		//Close the configuration window and cleanup
		public void CloseTrackConfigWindow()
		{
			trackConfigWindow.CloseWindow();
		}

		//Show SongInfo
		public void OpenSongInfoWindow()
		{
			songInfoWindow.Set(_curSong.info.title);
			songInfoWindow.OpenWindow();
		}


		//Destroy exsisting tracks
		public void DestroyTracks()
		{
			foreach (UITrackManager t in _tracks)
				t.Destroy();
			_tracks.Clear();
		}

		//Save
		public void SaveSong(object arg)
		{
			string curPath = _dataPath + "/" + GameRegistry.GetString("CUR_SONG");
			Directory.CreateDirectory(curPath);
			if(!File.Exists(_dataPath + "/" + GameRegistry.GetString("CUR_SONG") + "/" + Path.GetFileName(_curSong.songPath)))
			{
				string newPath = GameRegistry.GetString("CUR_SONG") + "/" + Path.GetFileName(_curSong.songPath);
				File.Copy(_dataPath + "/" + _curSong.songPath, _dataPath + "/" + newPath);
				_curSong.songPath = newPath;
			}
			File.WriteAllBytes(curPath + "/Song.SongData", _curSong.getSongData());
		}


		//Save As
		public void SaveSongAs(object arg)
		{
			saveWindow.OpenWindow();
			saveWindow.fileName = curSong.info.title;
		}

		//Play Song
		public void PlaySong(object arg)
		{
			SaveSong(null);
			GameRegistry.SetValue("isEditing", true);
			SceneManager.LoadScene("main");
		}

		//Exit Editor
		public void ExitEditor(object arg)
		{
			GameRegistry.SetValue("isEditing", false);
			SceneManager.LoadScene(0);
		}
	}
}
