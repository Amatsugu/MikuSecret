using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPS : MonoBehaviour
{
	private Text _fpsText;
	private float _deltaTime;

	void Start()
	{
		_fpsText = GetComponent<Text>();
	}

	void Update()
	{
		_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
		float msec = _deltaTime * 1000.0f;
		float fps = 1.0f / _deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		_fpsText.text = text;
	}
}
