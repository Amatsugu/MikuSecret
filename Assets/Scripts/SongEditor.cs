using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace TheDarkVoid
{
	public class SongEditor : MonoBehaviour
	{
		//Public
		public float timeScale;
		public Transform timeline;
		public GameObject secondsMarker;

		//Private
		private Song _curSong;
		private string _dataPath;
		private float _songLength;
		private List<Transform> _secondsMarkers = new List<Transform>();

		void Start()
		{
			_dataPath = Application.dataPath + "/Songs/";
			_curSong = Song.loadSong(File.ReadAllBytes(_dataPath + "Song.SongData"));
			EventManager.StartListening("songLoaded", SongReady);
			StartCoroutine(_curSong.LoadAudioClip("songLoaded"));
		}

		void SongReady()
		{
			EventManager.StopListening("songLoaded", SongReady);
			Debug.Log("loaded");
			_songLength = _curSong.song.length;
			CreateTimeline();
		}

		void CreateTimeline()
		{
			for (int i = 1; i <= (int)_songLength; i++)
			{
				Transform marker;
				Utils.CreateUIImage(secondsMarker, new Vector2(i * timeScale, 0), timeline, out marker);
				marker.GetComponent<Text>().text = i.ToString();
                _secondsMarkers.Add(marker);
            }
			UpdateTimeline(timeScale);
		}

		public void UpdateTimeline(float timeScale)
		{
			this.timeScale = timeScale;
			for(int i = 1; i <= _secondsMarkers.Count; i++)
			{
				Vector2 pos = _secondsMarkers[i - 1].localPosition;
				pos.x = i * timeScale;
				_secondsMarkers[i - 1].localPosition = pos;
			}
		}
	}
}
