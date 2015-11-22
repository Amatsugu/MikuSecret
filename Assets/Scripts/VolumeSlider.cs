using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

namespace com.LuminousVector
{
	public class VolumeSlider : MonoBehaviour
	{
		//Public
		public Text volumeText;
		public AudioMixer mixer;

		public float vol
		{
			get
			{
				return _vol;
			}
			set
			{
				_vol = value;
				volumeText.text = value + " dB";
				mixer.SetFloat("MasterVol", value);
				PlayerPrefs.SetFloat("MasterVol", value);
			}
		}
		private float _vol;

		void Start()
		{
			vol = PlayerPrefs.GetFloat("MasterVol", 0);
		}

	}
}
