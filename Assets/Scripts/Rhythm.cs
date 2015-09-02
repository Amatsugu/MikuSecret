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
		public GameObject trackImage;
		public float hitZoneOffset;
		public Transform UIcanvas;
		public Transform beatCanvas;
		public Transform particleCanvas;
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
		private float _curProgress;
		private Song _song;
		private List<Vector2> _basePositions = new List<Vector2>();
		private List<Beat> _beatsToRemove = new List<Beat>();
		private List<int> _targetRemovalTrack = new List<int>();
		private int _trackCount;
		private ControlMap _controls;

		void Start()
		{
			//Cache the audio source
			_src = GetComponent<AudioSource>();
			//get the song length
			_songLength = _src.clip.length;
			//Cache the size of the progressBar
			_progressBarSize = progressBar.rectTransform.localScale;
			//Generate the song
			_song = new Song(_src.clip, 6);
			int t = 0;
			//Cache trackCount
			_trackCount = _song.trackCount;
			//Assign default controls
			_controls = new ControlMap().AddMap(new KeyMap(), _trackCount);
			//Assign beats
			for (float i = 1.64f; i <= 100; i += 0.646f)
			{
				if (t >= _trackCount)
					t = 0;
				_song.tracks[t].AddBeat(new Beat(i));
				t++;
			}
			//Assign track positions
			for (int i = 0; i <= _trackCount; i++)
				_basePositions.Add(Vector2.zero);
			//Get beat width
			float beatWidth = beatObject.GetComponent<Image>().rectTransform.rect.width;
			//Calculate track spacing;
			float fullWidth = beatWidth + trackSpacing;
			//Create all beats
			for (int i = 0; i < _trackCount; i++)
			{
				List<Beat> track = _song.tracks[i].beats;
				_basePositions[i] = new Vector2(((i - ((_trackCount - 1f) / 2f)) * fullWidth), Screen.height);
				foreach (Beat b in track)
				{
					b.startPosition = _basePositions[i].y;
					Image B = CreateUIImage(beatObject, _basePositions[i], beatCanvas);
					B.color = _song.tracks[i].color;
					b.Create(B);
				}
				//Create Track lanes
				Vector2 pos = new Vector2(_basePositions[i].x, hitZoneOffset);
				Image trackI = CreateUIImage(trackImage, pos, UIcanvas);
				trackI.color = _song.tracks[i].color;
				Debug.Log(_trackCount);
				//Assign default keys to controlmap
				_controls.AddKey(KeyCode.Space, i, _trackCount);
				//trackI.rectTransform.localScale = new Vector3(fullWidth, Screen.height-hitZoneOffset);
			}
		}

		void Update()
		{
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
					//Skips beats that are already hit
					if (b.hit)
						continue;
					//Skips beats that aren't within the lead time
					if (b.time - leadTime > _curProgress)
						continue;
					//Move the beats towards the hitzone based on th ecurrent progress of the audio track
					Vector3 p = b.GetPosition();
					p.y = Mathf.Lerp(b.startPosition, hitZoneOffset, 1 - ((b.time - _curProgress) / leadTime));
					b.UpdatePosition(p);
					//Calcuate end of long beat
					float beatEnd = Mathf.Lerp(b.startPosition, hitZoneOffset, 1 - ((b.time + b.duration - _curProgress) / leadTime));
					float beatLength = beatEnd - p.y;
					//Marks missed beats for removal
					if (_curProgress - b.time > hitRange + hitZoneSize)
					{
						_beatsToRemove.Add(b);
						_targetRemovalTrack.Add(i);
					}
					//Checks the accuary of an attempted hit
					if (Input.GetKeyDown(_controls.GetKey(i, _trackCount)))
					{
						//Checks if there was a hit
						if (Mathf.Abs(_curProgress - b.time) <= hitRange)
						{
							//Marks a beat as being hit
							b.hit = true;
							//Calculates the hit accuracy
							float error = Mathf.Abs((_curProgress - b.time) - hitRange / 2);
							error /= hitRange;
							//Resolves the point value of a hit based on an error cruve
							float hitValue = errorCurve.Evaluate(error);
							//Adds scire
							score += hitValue * beatValue;
							scoreText.text = score.ToString();
							//Creates a particle burst proportional to the point value of the hit
							SpawnParticles((int)(hitValue * hitParticleCount), b.GetPosition(), hitValue, _song.tracks[i].color);
							//Marks the for removal beat
							_beatsToRemove.Add(b);
							_targetRemovalTrack.Add(i);
						}
						//Checks for a miss
						else if (Mathf.Abs(_curProgress - b.time) <= hitRange + hitZoneSize)
							Debug.Log("Miss");
					}
				}
			}

			//Remove used and missed beats
			for (int i = 0; i < _beatsToRemove.Count; i++)
			{
				Beat b = _beatsToRemove[i];
				b.Destroy();
				_song.tracks[_targetRemovalTrack[i]].RemoveBeat(b);
			}
			_beatsToRemove.Clear();
			_targetRemovalTrack.Clear();
		}

		//Instantiates a UI Image at a specficed position, and with a spefiied parent
		Image CreateUIImage(Object obj, Vector2 pos, Transform parent)
		{
			GameObject g = Instantiate(obj, pos, Quaternion.identity) as GameObject;
			g.transform.SetParent(parent, false);
			return g.GetComponent<Image>();
		}

		//Creates a burst fo particles
		void SpawnParticles(int ammount, Vector3 pos, float speedRatio, Color color)
		{
			for (int i = 0; i <= ammount; i++)
			{
				GameObject g = Instantiate(particle, pos, Quaternion.identity) as GameObject;
				g.transform.SetParent(particleCanvas);
				Particle p = g.GetComponent<Particle>();
				p.Set(new Vector2(Random.Range(-particleSpeed, particleSpeed), Random.Range(0, particleSpeed)) * speedRatio, particleLifeTime, color);
			}
		}
	}
}
