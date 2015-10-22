using UnityEngine;
using UnityEngine.UI;
using System.IO;
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
		public GameObject trailImage;
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
		private Vector3 _progressBarSize = new Vector3();
		private float _songLength;
		private float _curProgress;
		private Song _song;
		private List<Vector2> _basePositions = new List<Vector2>();
		private List<Beat> _beatsToRemove = new List<Beat>();
		private List<int> _targetRemovalTrack = new List<int>();
		private int _trackCount;
		private ControlMap _controls;
		private string _dataPath;

		void Start()
		{
			//Set the Data path
			_dataPath = Application.dataPath + "/Songs/";
			//Cache the audio source
			_src = GetComponent<AudioSource>();
			//get the song length
			//Cache the size of the progressBar
			_progressBarSize = progressBar.rectTransform.localScale;
			//Generate the song
			//GenerateSong();
			//_song = null;
			byte[] songData = File.ReadAllBytes(_dataPath + "SongName/Song.SongData");
			_song = Song.loadSong(songData);
			EventManager.StartListening("loadSong", SongReady);
			StartCoroutine(_song.LoadAudioClip("loadSong"));
			//_src.clip = _song.song;
			//_songLength = _src.clip.length;
		}

		void SongReady()
		{
			EventManager.StopListening("loadSong", SongReady);
			_src.clip = _song.song;
			if (_src.clip != null)
				_songLength = _src.clip.length;
			CreatePlayFeild();
			_src.Play();
		}

		void Update()
		{
			if(_song.song.loadState != AudioDataLoadState.Loaded)
			{
				return;
			}
			if (_curProgress > 3)
				_src.Pause();
			//Render Time
			_curProgress = _src.time;
			int m = (int)(_curProgress / 60);
			int s = (int)_curProgress - m * 60;
			timeText.text = m + ":" + s;
			//Render Progress Bar
			progressBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (_curProgress / _songLength) * Screen.width);
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
					if(b.duration <= 0)
						b.UpdatePosition(p);
					else
					{
						//Calculate the trail length
						float pos = b.duration  + b.time;
						pos -= _curProgress;
						pos /= leadTime;
                        float trailHeight = Mathf.Lerp(hitZoneOffset, _basePositions[i].y, pos);
						trailHeight -= b.GetPosition().y;
						b.UpdatePosition(p, trailHeight);
					}

					//Marks missed beats for removal
					if (_curProgress - b.time - b.duration > hitRange + hitZoneSize)
					{
						_beatsToRemove.Add(b);
						_targetRemovalTrack.Add(i);
					}
					//Checks the accuary of an attempted hit
					KeyCode key = _controls.GetKey(i, _trackCount);
					if (b.duration <= 0)
					{
						if (Input.GetKeyDown(key))
						{
							//Checks if there was a hit
							if (Mathf.Abs(_curProgress - b.time) <= hitRange)
							{
								//Marks a beat as being hit
								b.hit = true;
								//Calculates the hit accuracy
								float hitValue = CalculateAcc(b.time);
								//Adds scire
								score += hitValue * beatValue;
								scoreText.text = score.ToString();
								//Creates a particle burst proportional to the point value of the hit
								SpawnParticles((int)(hitValue * hitParticleCount), b.GetPosition(), hitValue, _song.tracks[i].color);
								//Marks the beat for removal
								_beatsToRemove.Add(b);
								_targetRemovalTrack.Add(i);
							}
							//Checks for a miss
							else if (Mathf.Abs(_curProgress - b.time) <= hitRange + hitZoneSize)
								Debug.Log("Miss");
						}
					}
					else
					{
						//Start the long beat
						if (float.IsNegativeInfinity(b.hitStart) && !b.hit)
						{
							if (Mathf.Abs(_curProgress - b.time) <= hitRange)
								if (Input.GetKeyDown(key))
								{
									b.startAcc = CalculateAcc(b.time);
									b.hitStart = _curProgress;
									SpawnParticles((int)(b.startAcc * hitParticleCount), b.GetPosition(), b.startAcc, _song.tracks[i].color);
								}
						}
						else
						{
							//Sustain the long beat
							if (Input.GetKey(key))
							{
								if (_curProgress <= b.time + b.duration)
								{
									score += Mathf.Round(beatValue * Time.deltaTime);
									SpawnParticles((int)Mathf.Round(hitParticleCount * Time.deltaTime * b.startAcc), b.GetPosition(), b.startAcc, _song.tracks[i].color);
									scoreText.text = score.ToString();
								}
							}
							//End the long beat
							if (Input.GetKeyUp(key))
							{
								b.hit = true;
								if (Mathf.Abs(_curProgress - b.time - b.duration) <= hitRange)
								{
									//Calculates hit accuracy
									float hitValue = CalculateAcc(b.time - b.duration);
									//Adds scire
									score += hitValue * beatValue;
									scoreText.text = score.ToString();
								}
								else
								{
									Debug.Log("Miss, Long");
								}
								//Marks the beat for removal 
								_beatsToRemove.Add(b);
								_targetRemovalTrack.Add(i);
							}
						}
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

		//Calculate the hit accuracy
		float CalculateAcc(float t)
		{
			float error = Mathf.Abs((_curProgress - t) - hitRange / 2);
			error /= hitRange;
			//Resolves the point value of a hit based on an error cruve
			float hitValue = errorCurve.Evaluate(error);
			return hitValue;
		}

		void GenerateSong()
		{
//			_song = new Song(_src.clip, 6);
			//Cache trackCount
			_trackCount = _song.trackCount;
			//Assign beats
			int t = 0;
			for (float i = 1.64f; i <= 100; i += 2 * 0.646f)
			{
				if (t >= _trackCount)
					t = 0;
				_song.tracks[t].AddBeat(new Beat(i));
				t++;
			}
			byte[] songData = _song.getSongData();
			if (!Directory.Exists(_dataPath))
				Directory.CreateDirectory(_dataPath);
			File.WriteAllBytes(_dataPath + "Song.SongData", songData);
		}

		void CreatePlayFeild()
		{
			//Cache Track Count
			_trackCount = _song.trackCount;
			//Assign default controls
			_controls = new ControlMap().AddMap(new KeyMap(), _trackCount);
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
					Transform beatP;
					Image B = Utils.CreateUIImage(beatObject, _basePositions[i], beatCanvas, out beatP);
					if (b.duration <= 0)
						b.Create(B);
					else
					{
						//Create long beat
						float trailLenght = Mathf.Lerp(_basePositions[i].y, hitZoneOffset, b.duration / leadTime);
						Image trail = Utils.CreateUIImage(trailImage, new Vector3(0, 0, 0), beatP);
						trail.color = _song.tracks[i].color;
						trail.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, trailLenght);
						b.Create(B, trail);
					}
					B.color = _song.tracks[i].color;
				}
				//Create Track lanes
				Vector2 pos = new Vector2(_basePositions[i].x, hitZoneOffset);
				Image trackI = Utils.CreateUIImage(trackImage, pos, UIcanvas);
				trackI.color = _song.tracks[i].color;
				//Assign default keys to controlmap
				_controls.AddKey(KeyCode.Space, i, _trackCount);
			}
		}
	}
}
