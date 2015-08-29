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
	public GameObject beatObject;
	public float hitZoneOffset;
	public Vector3 basePosition;
	public Transform canvas;

	//Private
	private AudioSource _src;
	private Vector3 _progressBarSize;
	private float _songLength;
	private float _curProgress;
	private Song _song;
	private float _playSpeed = 1.0f;

	void Start ()
	{
		_src = GetComponent<AudioSource>();
		_songLength = _src.clip.length;
		_progressBarSize = progressBar.rectTransform.localScale;
		_song = new Song(_src.clip);
		_song.beats.Add(new Beat(5));
		_song.beats.Add(new Beat(6));
		_song.beats.Add(new Beat(7));
		foreach (Beat b in _song.beats)
		{
			Vector3 p = basePosition;
			p.y = b.time + hitZoneOffset;
			Image g = Instantiate(beatObject, Vector3.zero, Quaternion.identity) as Image;
			Debug.Log(g);
			//g.transform.SetParent(canvas, false);
			//g.rectTransform.position = p;
			//b.Create(g);
		}
	}
	
	void Update ()
	{
		return;
		_curProgress = _src.time;
		int m = (int)(_curProgress / 60);
		int s = (int)_curProgress - m * 60;
		timeText.text = m + ":" + s;
		_progressBarSize.x = (_curProgress / _songLength) * Screen.width;
		progressBar.rectTransform.localScale = _progressBarSize;
		float offset = _curProgress + hitZoneOffset;
        foreach (Beat b in _song.beats)
		{
			Vector3 p = b.GetPosition();
			p.y = (b.time - offset) * _playSpeed;
			b.UpdatePosition(p);
		}
	}
}
