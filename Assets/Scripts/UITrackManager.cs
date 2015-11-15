using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace TheDarkVoid
{
	public class UITrackManager : MonoBehaviour, IPointerClickHandler, IDragHandler, IDropHandler
	{
		//Public
		public GameObject instance
		{
			get { return _instance; }
			set { _instance = value; _image = _instance.GetComponent<Image>(); }
		}
		public GameObject beatPrefab;
		public Track track;
		public Transform _beatParent;
		public Text trackName;

		//Private
		private List<UIBeatManager> _beats = new List<UIBeatManager>();
		private GameObject _instance;
		private Image _image;
		private float _left;
		private float _startPos;
		private UIBeatManager _prevBeat;

		//Set the inital values of the Track
		public void Set(Track track)
		{
			instance = gameObject;
			_left = _beatParent.position.x;
			_image = instance.GetComponent<Image>();
			this.track = track;
			_image.color = track.color;
			trackName.text = track.name;
			RenderBeats();

			Transform beat;
			Utils.CreateUIImage(beatPrefab,
				Vector2.zero,
				_beatParent, out beat);
			_prevBeat = beat.GetComponent<UIBeatManager>();
			_prevBeat.Set(new Beat(0f));
			beat.gameObject.SetActive(false);
		}

		//Destory this object
		public void Destroy()
		{
			Destroy(instance);
		}

		public void RemoveThisTrack()
		{
			SongEditor.instance.RemoveTrack(this);
		}

		public void ConfigureThisTrack()
		{
			SongEditor.instance.ConfigureTrack(this);
		}

		//Add a beat to the track
		public void AddBeat(float time)
		{
			AddBeat(time, 0);
		}

		public void AddBeat(float time, float duration)
		{
			track.AddBeat(new Beat(time, duration));
			RenderBeats();
		}

		//Render all beats to the track
		void RenderBeats()
		{
			DestroyBeats();
			foreach (Beat b in track.beats)
			{
				Transform beat;
				Utils.CreateUIImage(beatPrefab, new Vector2(Utils.TransformToTimelinePos(b.time) , 0), _beatParent, out beat);
				UIBeatManager bm = beat.GetComponent<UIBeatManager>();
				bm.Set(b);
				_beats.Add(bm);
			}
			UpdateBeats();
		}

		//Udate Beats
		public void UpdateBeats()
		{
			foreach (UIBeatManager b in _beats)
			{
				Vector2 pos = b.positon;
				pos.x = (b.time - SongEditor.instance.seekPos) * SongEditor.instance.timeScale;
				b.positon = pos;
			}
		}

		//Destroy exsisting beats
		void DestroyBeats()
		{
			foreach (UIBeatManager b in _beats)
			{
				b.Destroy();
			}
			_beats.Clear();
		}

		//On Click event handler
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (!eventData.dragging)
					AddBeat(Utils.TransformToTime(eventData.position.x, _left));
				else
				{
					_startPos = eventData.position.x;
					_prevBeat.instance.gameObject.SetActive(true);
				}
			}
		}

		public void OnDrop(PointerEventData eventData)
		{
			_prevBeat.instance.gameObject.SetActive(false);
			float time = (_startPos < eventData.position.x) ?
				Utils.TransformToTime(eventData.position.x, _left) : 
				Utils.TransformToTime(_startPos, _left);
			float duration = Utils.TransformToTime(Math.Abs(eventData.position.x - _startPos),
				_left);
            AddBeat(time, duration);
		}

		public void OnDrag(PointerEventData eventData)
		{
			float time = (_startPos < eventData.position.x) ? _startPos : eventData.position.x;
			float duration = Utils.TransformToTime(Math.Abs(eventData.position.x - _startPos),
				_left);
			_prevBeat.positon = new Vector2(time, 0);
			_prevBeat.duration = duration;
		}
	}
}
