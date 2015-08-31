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
		public Vector3 basePosition;
		public Transform canvas;
		public float leadTime = 1.0f;
		public float hitZoneSize = 0.1f;
		public float hitRange = 0.5f;
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
		private List<Beat> _beatsToRemove = new List<Beat>();

		void Start()
		{
			_src = GetComponent<AudioSource>();
			_songLength = _src.clip.length;
			CalculatePixelLength();
			_progressBarSize = progressBar.rectTransform.localScale;
			_song = new Song(_src.clip);
			for (float i = 1.64f; i <= 100; i += 0.646f)
			{
				_song.beats.Add(new Beat(i));
			}
			foreach (Beat b in _song.beats)
			{
				basePosition.x = Screen.width / 2;
				basePosition.y = Screen.height;
				b.startPosition = basePosition.y;
				GameObject g = Instantiate(beatObject, basePosition, Quaternion.identity) as GameObject;
				g.transform.SetParent(canvas, true);
				Image B = g.GetComponent<Image>();
				b.Create(B);
			}
		}

		void Update()
		{
			//		return;
			_curProgress = _src.time;
			int m = (int)(_curProgress / 60);
			int s = (int)_curProgress - m * 60;
			timeText.text = m + ":" + s;
			_progressBarSize.x = (_curProgress / _songLength) * Screen.width;
			progressBar.rectTransform.localScale = _progressBarSize;
			foreach (Beat b in _song.beats)
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
					}
					else if (Mathf.Abs(_curProgress - b.time) <= hitRange + hitZoneSize)
						Debug.Log("Miss");
				}
			}
			Beat beat = _song.beats[0];
			float a = Mathf.Abs((_curProgress - beat.time) - hitRange / 2);
			a /= hitRange;
			hitZone.color = accuaracyColor.Evaluate(a);
			foreach (Beat b in _beatsToRemove)
			{
				b.Destroy();
				_song.beats.Remove(b);
			}
			_beatsToRemove.Clear();
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
