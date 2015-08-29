using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Rhythm : MonoBehaviour
{
	//Public
	public Image progressBar;
	public Text timeText;
	public Text scoreText;
	public GameObject beatObject;
	public float hitZoneOffset;
	public Vector3 basePosition;
	public Transform canvas;
	public float leadTime = 1.0f;
	public float hitZoneSize = 0.1f;
	public float hitRange = 0.5f;
	public AnimationCurve errorCurve;
	public float score;
	public float beatValue = 300;

	//Private
	private AudioSource _src;
	private Vector3 _progressBarSize;
	private float _songLength;
	private float _songPixelLength;
	private float _curProgress;
	private Song _song;
	private List<Beat> _beatsToRemove = new List<Beat>();

	void Start ()
	{
		_src = GetComponent<AudioSource>();
		_songLength = _src.clip.length;
		CalculatePixelLength();
		_progressBarSize = progressBar.rectTransform.localScale;
		_song = new Song(_src.clip);
		for (int i = 1; i <= 100; i++)
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
	
	void Update ()
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
			if (b.time - leadTime > _curProgress)
				continue;
			Vector3 p = b.GetPosition();
            p.y = Mathf.Lerp(b.startPosition, hitZoneOffset, 1-((b.time-_curProgress)/leadTime));
			b.UpdatePosition(p);
			if(_curProgress - b.time > hitRange + hitZoneSize)
			{
				b.Destroy();
				_beatsToRemove.Add(b);
			}
			if(Input.GetKeyDown(KeyCode.Space))
			{ 
				if (Mathf.Abs(_curProgress-b.time) <= hitRange)
				{
					b.hit = true;
					float error = (_curProgress - b.time) + hitRange;
					score += errorCurve.Evaluate(error) * beatValue;
					scoreText.text = score.ToString();
				}
				else if(Mathf.Abs(_curProgress-b.time) <= hitRange+hitZoneSize)
					Debug.Log("Miss");
			}
		}
		foreach(Beat b in  _beatsToRemove)
		{
			_song.beats.Remove(b);
		}
		_beatsToRemove.Clear();
	}

	void CalculatePixelLength()
	{
		_songPixelLength = _songLength * leadTime;
		_songPixelLength -= Screen.height - hitZoneOffset;
	}
}
