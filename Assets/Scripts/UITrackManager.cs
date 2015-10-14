using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace TheDarkVoid
{
	public class UITrackManager : MonoBehaviour, IPointerClickHandler
	{
		//Public
		public GameObject instance
		{
			get { return _instance; }
			set { _instance = value; _image = _instance.GetComponent<Image>(); }
		}
		public float timeScale
		{
			set { _timeScale = value; }
		}
		public GameObject beatPrefab;
		public Track track;
		public static SongEditor _SONG_EDITOR;

		//Private
		private List<Transform> _beats = new List<Transform>();
		private GameObject _instance;
		private Image _image;
		private float _seekWidth;
		private float _songLength;
		private float _timeLineWidth;
		private float _timeScale;

		public void Set(float seekWidth, float timeLineWidth, float songLength)
		{
			instance = gameObject;
			_seekWidth = seekWidth;
			_songLength = songLength;
			_timeLineWidth = timeLineWidth;
		}

		public void Destroy()
		{
			Destroy(instance);
		}

		public void AddBeat(float time)
		{
			track.AddBeat(new Beat(time));
			UpdateBeats();
		}

		public void UpdateBeats()
		{
			foreach (Beat b in track.beats)
			{
				Transform beat;
				Utils.CreateUIImage(beatPrefab, new Vector2(b.time * _SONG_EDITOR.timeScale, 0), instance.transform, out beat);
				_beats.Add(beat);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			float left = _image.rectTransform.position.x;
			float pos = eventData.position.x - left;
			float time = pos / _timeLineWidth;
			time *= (_seekWidth * _songLength);
			time += _SONG_EDITOR.seekPos;
			Debug.Log(time);
		}
	}
}
