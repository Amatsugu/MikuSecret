using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIVolumeSlider : MonoBehaviour
{
	//Public
	public Image slider;
	public Image sliderFrame;
	public Text volumeText;
	public float sensitivity = 1;
	public float volumeSnapSpeed = 1;
	public float fadeSpeed = 1;
	public float fadeDelay = 2;
	//Private
	private float _fadeProgess = 1;
	private float _fadeAmmount
	{
		set
		{
			_opacity.a = value;
			slider.color = sliderFrame.color = volumeText.color = _opacity;
		}
		get
		{
			return _opacity.a;
		}
	}
	private float _timeout;
	private float _targetVol;
	private float _volLerpProgress;
	private Color _opacity = Color.white;

	void Start ()
	{
		_fadeAmmount = 0;
		slider.fillAmount = _targetVol = AudioListener.volume;
		volumeText.text = (int)(AudioListener.volume * 100) + "%";
	}

	void Update ()
	{
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			float deltaS = Input.GetAxis("Mouse ScrollWheel");
			Debug.Log(deltaS);
            if (deltaS != 0)
			{
				_timeout = fadeDelay + Time.time;
				_targetVol += deltaS * sensitivity;
				_targetVol = (_targetVol > 1) ? 1 : _targetVol;
				_targetVol = (_targetVol < 0) ? 0 : _targetVol;
				_volLerpProgress = 0;
				
			}
		}
		if(AudioListener.volume != _targetVol)
		{
			AudioListener.volume = Mathf.Lerp(AudioListener.volume, _targetVol, _volLerpProgress);
			_volLerpProgress += Time.deltaTime * volumeSnapSpeed;
			slider.fillAmount = AudioListener.volume;
			volumeText.text = (int)(AudioListener.volume * 100) + "%";
		}
		if(Time.time >= _timeout)
		{
			if (_fadeAmmount > 0)
			{
				_fadeAmmount = Mathf.Lerp(1, 0, _fadeProgess);
				_fadeProgess += fadeSpeed * Time.deltaTime;
			}
		}else
		{
			if (_fadeAmmount < 1)
			{
				_fadeAmmount = Mathf.Lerp(1, 0, _fadeProgess);
				_fadeProgess -= 4 * Time.deltaTime;
			}
		}

	}
}
