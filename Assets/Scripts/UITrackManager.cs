using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace com.LuminousVector
{
	public class UITrackManager : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
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
		public Image image { get { return _image; } }

		//Private
		private List<UIBeatManager> _beats = new List<UIBeatManager>();
		private GameObject _instance;
		private Image _image;
		private float _left;
		private float _startPos = Mathf.NegativeInfinity;
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
			Utils.CreateUIImage(beatPrefab, Vector2.zero, _beatParent, out beat);
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
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if(_startPos == Mathf.NegativeInfinity)
			{
				_startPos = eventData.position.x - _left;
				_prevBeat.instance.gameObject.SetActive(true);
			}
			float curPos = (_startPos < eventData.position.x - _left) ? _startPos : eventData.position.x - _left;
			float duration = Utils.TransformToTime(Math.Abs(eventData.position.x - _startPos - _left), 0);
			_prevBeat.positon = new Vector2(curPos, 0);
			_prevBeat.duration = duration;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			float time = (_startPos < eventData.position.x - _left) ? Utils.TransformToTime(_startPos, 0) : Utils.TransformToTime(eventData.position.x, _left);
			float duration = Utils.TransformToTime(Math.Abs(eventData.position.x - _startPos - _left), 0);
            AddBeat(time, duration);
			_prevBeat.instance.gameObject.SetActive(false);
			_startPos = Mathf.NegativeInfinity;
		}
	}
}
