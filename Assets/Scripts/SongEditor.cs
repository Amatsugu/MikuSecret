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
		public float timeScale;
		public Transform timeline;
		public GameObject timeMarker;
		public float minThreshold = 20f;

		//Private
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
			CreateTimelineSeconds();
			UpdateTimeline(timeScale);
		}

		void CreateTimelineSeconds()
		{
			DestroyMarkers();
			int min = 0;
			for (int i = 0; i < (int)_songLength; i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(i * timeScale, 0), timeline, out marker);
				int sec = (int)(i - (Mathf.Round(i / 60) * 60));
				string val = "";
				if (sec == 0)
				{
					val = "<b>" + min + "m</b>" ;
					min++;
				} else
					val = sec + "s";
                marker.GetComponent<Text>().text = val;
                _timeMarkers.Add(marker);
            }
		}

		void CreateTimelineMinutes()
		{
			DestroyMarkers();
			for (int i = 0; i < (int)(_songLength / 60); i++)
			{
				Transform marker;
				Utils.CreateUIImage(timeMarker, new Vector2(), timeline, out marker);
				marker.GetComponent<Text>().text = i + "m";
				_timeMarkers.Add(marker);
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

		public void UpdateTimeline(float timeScale)
		{
			this.timeScale = timeScale;
			if(timeScale <= minThreshold && _mode == TimelineMode.sec)
			{
				_mode = TimelineMode.min;
				CreateTimelineMinutes();
			}else if(timeScale >= minThreshold && _mode == TimelineMode.min)
			{
				_mode = TimelineMode.sec;
				CreateTimelineSeconds();
			}
			for(int i = 0; i < _timeMarkers.Count; i++)
			{
				Vector2 pos = _timeMarkers[i].localPosition;
				pos.x = i * timeScale;
				if (_mode == TimelineMode.min)
					pos.x *= 60f;
				_timeMarkers[i].localPosition = pos;
			}
		}
	}
}
