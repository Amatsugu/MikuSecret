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
		public Image songLengthBar;
		public float minThreshold = 20f;
		public float scrollPos = 0f;

		//Private
		private float _timeScale;
		private Song _curSong;
		private string _dataPath;
		private float _songLength;
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
			CreateTimelineSeconds();
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
				if (sec >= 60)
				{
					min += 1;
					sec = 0;
				}
				else
					sec++;
            }
		}

		void CreateTimelineMinutes()
		{
			DestroyMarkers();
			int sec = 0;
			int min = 0;
			for (int i = 0; i < (int)(_songLength / 10); i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(), timeline, out marker);
				marker.GetComponent<Text>().text = min + ":" + sec;
				_timeMarkers.Add(marker);
				if(sec >= 60)
				{
					min += 1;
					sec = 0;
				}else
					sec += 10;
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
			_timeScale = zoomLevel;
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
				pos.x -= (scrollPos * _timeScale);
				_timeMarkers[i].localPosition = pos;
			}
			songLengthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _songLength * _timeScale);
		}

		public void TimelineSeek(float position)
		{


			UpdateTimeline();
		}
	}
}
