using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Beat
{
	public float time;
	public float duration;

	private Image _beatImage;

	public Beat(float time)
	{
		this.time = time;
		this.duration = 0;
	}

	public Beat(float time, float duration)
	{
		this.time = time;
		this.duration = duration;
	}

	public void Create(Image beatImage)
	{
		_beatImage = beatImage;
	}

	public void UpdatePosition(Vector3 position)
	{
		_beatImage.rectTransform.position = position;
	}

	public Vector3 GetPosition()
	{
		return _beatImage.rectTransform.position;
	}

}
