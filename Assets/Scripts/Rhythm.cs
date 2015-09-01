using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TheDarkVoid
{

	[RequireComponent(typeof(AudioSource))]
	public class Rhythm : MonoBehaviour
	{
		//Public
		public Image progressBar;
		public Image hitZone;
		public Text timeText;
		public Text scoreText;
		public GameObject beatObject;
		public GameObject particle;
		public float hitZoneOffset;
		public Transform canvas;
		public float leadTime = 1.0f;
		public float hitZoneSize = 0.1f;
		public float hitRange = 0.5f;
		public float trackSpacing = 20;
		public AnimationCurve errorCurve;
		public float score;
		public float beatValue = 300;
		public float hitParticleCount = 300;
		public float particleSpeed = 10;
		public float particleLifeTime = 3;
		public Gradient accuaracyColor;

		//Private
		private AudioSource _src;
		private Vector3 _progressBarSize;
		private float _songLength;
		private float _songPixelLength;
		private float _curProgress;
		private Song _song;
		private List<Vector2> _basePositions = new List<Vector2>();
		private List<Beat> _beatsToRemove = new List<Beat>();
		private List<int> _targetRemovalTrack = new List<int>();
		private int _trackCount;

		void Start()
		{
			_src = GetComponent<AudioSource>();
			_songLength = _src.clip.length;
			CalculatePixelLength();
			_progressBarSize = progressBar.rectTransform.localScale;
			_song = new Song(_src.clip, 6);
			int t = 0;
			for (float i = 1.64f; i <= 100; i += 0.646f)
			{
				if (t >= _song.trackCount)
					t = 0;
				_song.tracks[t].AddBeat(new Beat(i));
				t++;
			}
			_trackCount = _song.tracks.Count;
			for (int i = 0; i <= _trackCount; i++)
				_basePositions.Add(Vector2.zero);
			float beatWidth = beatObject.GetComponent<Image>().rectTransform.rect.width;
			float fullWidth = beatWidth + trackSpacing;
			for (int i = 0; i < _trackCount; i++)
			{
				List<Beat> track = _song.tracks[i].beats;
				foreach (Beat b in track)
				{
					_basePositions[i] = new Vector2((Screen.width / 2) + ((i - ((_trackCount-1)/2)) * fullWidth), Screen.height);
					b.startPosition = _basePositions[i].y;
					GameObject g = Instantiate(beatObject, _basePositions[i], Quaternion.identity) as GameObject;
					g.transform.SetParent(canvas, true);
					Image B = g.GetComponent<Image>();
					b.Create(B);
				}
			}
		}

		void Update()
		{
			//		return;
			//Render Time
			_curProgress = _src.time;
			int m = (int)(_curProgress / 60);
			int s = (int)_curProgress - m * 60;
			timeText.text = m + ":" + s;
			//Render Progress Bar
			_progressBarSize.x = (_curProgress / _songLength) * Screen.width;
			progressBar.rectTransform.localScale = _progressBarSize;
			//Render beat tracks
			for (int i = 0; i < _trackCount; i++)
			{
				List<Beat> track = _song.tracks[i].beats;
				foreach (Beat b in track)
				{
					if (b.hit)
						continue;
					if (b.time - leadTime > _curProgress)
						continue;
					Vector3 p = b.GetPosition();
					p.y = Mathf.Lerp(b.startPosition, hitZoneOffset, 1 - ((b.time - _curProgress) / leadTime));
					b.UpdatePosition(p);
					if (_curProgress - b.time > hitRange + hitZoneSize)
					{
						_beatsToRemove.Add(b);
						_targetRemovalTrack.Add(i);
					}
					if (Input.GetKeyDown(KeyCode.Space))
					{
						if (Mathf.Abs(_curProgress - b.time) <= hitRange)
						{
							b.hit = true;
							float error = Mathf.Abs((_curProgress - b.time) - hitRange / 2);
							error /= hitRange;
							float hitValue = errorCurve.Evaluate(error);
							score += hitValue * beatValue;
							scoreText.text = score.ToString();
							SpawnParticles((int)(hitValue * hitParticleCount), b.GetPosition(), hitValue);
							_beatsToRemove.Add(b);
							_targetRemovalTrack.Add(i);
						}
						else if (Mathf.Abs(_curProgress - b.time) <= hitRange + hitZoneSize)
							Debug.Log("Miss");
					}
				}
			}
			/*Beat beat = _song.beats[0];
			float a = Mathf.Abs((_curProgress - beat.time) - hitRange / 2);
			a /= hitRange;
			hitZone.color = accuaracyColor.Evaluate(a);
			*/
			for (int i = 0; i < _beatsToRemove.Count; i++)
			{
				Beat b = _beatsToRemove[i];
				b.Destroy();
				_song.tracks[_targetRemovalTrack[i]].RemoveBeat(b);
			}
			_beatsToRemove.Clear();
			_targetRemovalTrack.Clear();
		}

		void CalculatePixelLength()
		{
			_songPixelLength = _songLength * leadTime;
			_songPixelLength -= Screen.height - hitZoneOffset;
		}

		void SpawnParticles(int ammount, Vector3 pos, float speedRatio)
		{
			for (int i = 0; i <= ammount; i++)
			{
				GameObject g = Instantiate(particle, pos, Quaternion.identity) as GameObject;
				g.transform.SetParent(canvas);
				Particle p = g.GetComponent<Particle>();
				p.Set(new Vector2(Random.Range(-particleSpeed, particleSpeed), Random.Range(0, particleSpeed)) * speedRatio, particleLifeTime);
			}
		}
	}
}
