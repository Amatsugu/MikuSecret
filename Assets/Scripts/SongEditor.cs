using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace TheDarkVoid
{
	[RequireComponent(typeof(AudioSource))]
	public class SongEditor : MonoBehaviour
	{
		public enum TimelineMode
		{
			min,
			sec
		}
		//Public
		public Transform timeline;
		public GameObject timeMarker;
		public GameObject trackPrefab;
		public Slider timeScaleSlider;
		public DynamicWidthSlider seekSlider;
		public ClickDragable playHead;
		public Image songProgressBar;
		public RectTransform trackScrollView;
		public GameObject trackConfigWindow;
		public float minThreshold = 20f;
		public float playHeadPos = 0;
		public float padding = 5f;
		public float timeScale
		{
			get { return _timeScale; }
		}
		public float seekPos
		{
			get { return _seekPos; }
		}
		public float songLength
		{
			get { return _songLength; }
		}

		//Private
		private float _timeScale;
		private Song _curSong;
		private string _dataPath;
		private float _songLength;
		private float _seekPos;
		private float _timelineWidth;
		private TimelineMode _mode;
		private List<Transform> _timeMarkers = new List<Transform>();
		private List<UITrackManager> _tracks = new List<UITrackManager>();
		private AudioSource _audio;
		private UITrackManager _trackToConfigure;

		private float _loadStartTime;

		//Load the song and wait for completion 
		void Start()
		{
			trackConfigWindow.SetActive(false);
			_dataPath = Application.dataPath + "/Songs/";
			_mode = TimelineMode.sec;
			_curSong = Song.loadSong(File.ReadAllBytes(_dataPath + "SongName/Song.SongData"));
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
			EventManager.StopListening("songLoaded", SongReady);
			Debug.Log("Song Loaded... in " + (Time.time - _loadStartTime) + "s");
			_songLength = _curSong.song.length;
			_audio.clip = _curSong.song;
			Image tl = timeline.GetComponent<Image>();
			_timelineWidth = tl.rectTransform.rect.width;
			timeScaleSlider.minValue = _timelineWidth / _songLength;
			EventManager.StartListening(seekSlider.eventCallback, TimelineSeek);
			EventManager.StartListening(playHead.eventCallback, SetPlayHead);
			float xOff = tl.rectTransform.position.x;
			playHead.SetMinMax(xOff, _timelineWidth + xOff);
			CreateTimeline(1);
			ZoomTimeline(timeScaleSlider.value);
			RenderTimeline();
			RenderTracks();
			_tracks[0].track = _curSong.tracks[0];
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

		public void UpdateTimeline()
		{
			for (int i = 0; i < _timeMarkers.Count; i++)
			{
				Vector2 pos = _timeMarkers[i].localPosition;
				pos.x = i * _timeScale;
				if (_mode == TimelineMode.min)
					pos.x *= 10f;
				pos.x -= (_seekPos * _timeScale);
				_timeMarkers[i].localPosition = pos;
			}
			foreach(UITrackManager t in _tracks)
			{
				t.UpdateBeats();
			}
			Vector2 p = songProgressBar.rectTransform.localPosition;
			p.x = -(_seekPos * _timeScale);
			songProgressBar.rectTransform.localPosition = p;
			SetPlayHead();
		}

		//Seek the visbile area on the timeline
		public void TimelineSeek()
		{
			_seekPos = seekSlider.value * _songLength;
			UpdateTimeline();
		}

		//Move the playhead and seek
		public void SetPlayHead()
		{
			SetAudioTime();
			SetTimeReadout();
		}

		//Seeks to a specfic position in the song
		public void SetAudioTime()
		{
			playHeadPos = playHead.value;
			float phT = (playHeadPos - playHead.min) / _timelineWidth;
			phT *= (seekSlider.width * _songLength);
			phT += _seekPos;
			_audio.time = phT;
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
			_audio.time += dir;
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
			_audio.Play();
			SetAudioTime();
			CancelInvoke("SetTimeReadout");
			InvokeRepeating("SetTimeReadout", 0, .01f);
			_audio.pitch = 1;
		}

		//Stop the song
		public void Pause()
		{
			_audio.Stop();
			CancelInvoke("SetTimeReadout");
			SetPlayHead();
		}

		//Add a new Track to the Song
		public void AddTrack()
		{
			_curSong.AddTrack(new Track());
			RenderTracks();
		}

		//Render a list of all tracks into the scrollview
		public void RenderTracks()
		{
			if (_curSong.trackCount == 0)
				return;
			DestroyTracks();
			Image trackSample = null;
			float yPos = -padding;
			foreach (Track t in _curSong.tracks)
			{
				Transform track;
				trackSample = Utils.CreateUIImage(trackPrefab, new Vector2(195, yPos), trackScrollView, out track);
				_tracks.Add(track.GetComponent<UITrackManager>());
				_tracks[_tracks.Count - 1].Set(this, t);
				//_tracks[_tracks.Count - 1].UpdateBeats();
				yPos -= trackSample.rectTransform.rect.height + padding;
			}
			trackScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((_tracks[0].GetComponent<Image>().rectTransform.rect.height + padding) * _tracks.Count) + padding);
		}

		public void UpdateTracks()
		{
			foreach(UITrackManager t in _tracks)
			{
				t.UpdateBeats();
			}
		}

		public void RemoveTrack(UITrackManager track)
		{
			track.Destroy();
			_curSong.RemoveTrack(track.track);
			_tracks.Remove(track);
			RenderTracks();
		}

		public void ConfigureTrack(UITrackManager track)
		{
			_trackToConfigure = track;
			Debug.Log(track);
			trackConfigWindow.SetActive(true);
		}
		
		public void CloseTrackConfigWindow()
		{
			_trackToConfigure = null;
			trackConfigWindow.SetActive(false);
		}

		//Destroy exsisting tracks
		public void DestroyTracks()
		{
			foreach (UITrackManager t in _tracks)
				t.Destroy();
			_tracks.Clear();
		}
	}
}
