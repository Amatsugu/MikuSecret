using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace TheDarkVoid
{
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
		public Slider timeScaleSlider;
		public DynamicWidthSlider seekSlider;
		public ClickDragable playHead;
		public Image songLengthBar;
		public float minThreshold = 20f;
		public float playHeadPos = 0;

		//Private
		private float _timeScale;
		private Song _curSong;
		private string _dataPath;
		private float _songLength;
		private float _seekPos;
		private float _timelineWidth;
		private TimelineMode _mode;
		private List<Transform> _timeMarkers = new List<Transform>();

		void Start()
		{
			_dataPath = Application.dataPath + "/Songs/";
			_mode = TimelineMode.sec;
			_curSong = Song.loadSong(File.ReadAllBytes(_dataPath + "Song.SongData"));
			EventManager.StartListening("songLoaded", SongReady);
			StartCoroutine(_curSong.LoadAudioClip("songLoaded"));
		}

		void SongReady()
		{
			EventManager.StopListening("songLoaded", SongReady);
			Debug.Log("loaded");
			_songLength = _curSong.song.length;
			Debug.Log(_songLength);
			Image tl = timeline.GetComponent<Image>();
			_timelineWidth = tl.rectTransform.rect.width;
			Debug.Log(_timelineWidth);
			timeScaleSlider.minValue = _timelineWidth / _songLength;
			EventManager.StartListening(seekSlider.eventCallback, TimelineSeek);
			EventManager.StartListening(playHead.eventCallback, SetPlayHead);
			float xO = tl.rectTransform.position.x;
            playHead.SetMinMax(xO, _timelineWidth + xO);
			CreateTimelineSeconds();
			ZoomTimeline(timeScaleSlider.value);
			UpdateTimeline();
		}

		void CreateTimelineSeconds()
		{
			DestroyMarkers();
			int min = 0;
			int sec = 0;
			for (int i = 0; i < (int)_songLength; i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(i * _timeScale, 0), timeline, out marker);
                marker.GetComponent<Text>().text = min + ":" + sec;
                _timeMarkers.Add(marker);
				sec++;
				if (sec >= 59)
				{
					min += 1;
					sec = 0;
				}
            }
		}

		void CreateTimelineMinutes()
		{
			DestroyMarkers();
			int min = 0;
			int sec = 0;
			for (int i = 0; i < (int)(_songLength / 10); i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(), timeline, out marker);
				marker.GetComponent<Text>().text = min + ":" + sec;
				_timeMarkers.Add(marker);
				sec += 10;
				if(sec >= 59)
				{
					min += 1;
					sec = 0;
				}
			}
		}

		void DestroyMarkers()
		{
			foreach(Transform m in _timeMarkers)
			{
				Destroy(m.gameObject);
			}
			_timeMarkers.Clear();
		}

		public void ZoomTimeline(float zoomLevel)
		{
			//Debug.Log(zoomLevel);
			_timeScale = zoomLevel;
			seekSlider.width = (_timelineWidth/(_songLength*_timeScale));
			UpdateTimeline();
		}

		public void UpdateTimeline()
		{
			if(_timeScale <= minThreshold && _mode == TimelineMode.sec)
			{
				_mode = TimelineMode.min;
				CreateTimelineMinutes();
			}else if(_timeScale >= minThreshold && _mode == TimelineMode.min)
			{
				_mode = TimelineMode.sec;
				CreateTimelineSeconds();
			}
			for(int i = 0; i < _timeMarkers.Count; i++)
			{
				Vector2 pos = _timeMarkers[i].localPosition;
				pos.x = i * _timeScale;
				if (_mode == TimelineMode.min)
					pos.x *= 10f;
				pos.x -= (_seekPos * _timeScale);
				_timeMarkers[i].localPosition = pos;
			}
			songLengthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _songLength * _timeScale);
			SetPlayHead();
		}

		public void TimelineSeek()
		{
			_seekPos = seekSlider.value * _songLength;
			UpdateTimeline();
		}

		public void SetPlayHead()
		{
			playHeadPos = playHead.value;
			int m, s, ns;
			float phT = (playHeadPos-playHead.min) / _timelineWidth;
			phT *= (seekSlider.width * _songLength);
			phT += _seekPos;
			m = (int)(phT / 60);
			s = ((int)phT- m*60);
			ns = (int)((phT- m*60 - s) * 100); 
			string time = m + ":" + s + ":" + ns;
			playHead.dragable.GetComponentInChildren<Text>().text = time;
		}
	}
}
