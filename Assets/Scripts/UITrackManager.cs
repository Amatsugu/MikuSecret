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
		public SongEditor _SONG_EDITOR;
		public Transform _beatParent;

		//Private
		private List<UIBeatManager> _beats = new List<UIBeatManager>();
		private GameObject _instance;
		private Image _image;
		private float _seekWidth;
		private float _songLength;
		private float _timeLineWidth;
		private float _timeScale;
		private float _beatWidth;
		private float _left;

		//Set the inital values of the Track
		public void Set(SongEditor SE, float seekWidth, float timeLineWidth, float songLength, Track track)
		{
			instance = gameObject;
			_left = _beatParent.localPosition.x;
			_SONG_EDITOR = SE;
			_image = instance.GetComponent<Image>();
			this.track = track;
			_image.color = track.color;
			_seekWidth = seekWidth;
			_songLength = songLength;
			_timeLineWidth = timeLineWidth;
			_beatWidth = beatPrefab.GetComponent<Image>().rectTransform.rect.width;
			RenderBeats();
			AddBeat(0);
		}

		//Destory this object
		public void Destroy()
		{
			Destroy(instance);
		}

		//Add a beat to the track
		public void AddBeat(float time)
		{
			track.AddBeat(new Beat(time));
			RenderBeats();
		}

		//Render all beats to the track
		void RenderBeats()
		{
			DestroyBeats();
			foreach (Beat b in track.beats)
			{
				Transform beat;
				Utils.CreateUIImage(beatPrefab, new Vector2((b.time - _SONG_EDITOR.seekPos) * _SONG_EDITOR.timeScale, 0), _beatParent, out beat);
				UIBeatManager bm = beat.GetComponent<UIBeatManager>();
				bm.Set(b);
                _beats.Add(bm);
			}
			UpdateBeats();
		}

		//Udate Beats
		public void UpdateBeats()
		{
			foreach(UIBeatManager b in _beats)
			{
				Vector2 pos = b.positon;
				pos.x = (b.time - _SONG_EDITOR.seekPos) * _SONG_EDITOR.timeScale;
				pos.x -= _left;
				pos.x += _beatWidth / 2;
				b.positon = pos;
			}
		}

		//Destroy exsisting beats
		void DestroyBeats()
		{
			foreach(UIBeatManager b in _beats)
			{
				b.Destroy();
			}
			_beats.Clear();
		}

		//On Click event handler
		public void OnPointerClick(PointerEventData eventData)
		{
			//TODO: Fix this!
			float pos = eventData.position.x;
			pos = (pos < 0) ? 0 : pos;
			float time = pos / _timeLineWidth;
			time += _SONG_EDITOR.seekSlider.value;
			time *= _songLength;
			AddBeat(time);
			Debug.Log(time);
		}
	}
}
