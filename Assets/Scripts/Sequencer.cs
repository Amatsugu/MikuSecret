using UnityEngine;
using System.Collections;

namespace com.LuminousVector
{
	[RequireComponent(typeof(AudioSource))]
	public class Sequencer : MonoBehaviour
	{
		//Public
		public AudioClip preLoop;
		public AudioClip loop;

		//Public
		private float _progress;
		private int _loopCount;
		private AudioSource _src;
		private bool _isLooping = false;

		void Start()
		{
			_src = GetComponent<AudioSource>();
			_src.clip = preLoop;
			_src.Play();
		}

		void Update()
		{
			if (!_src.isPlaying && !_isLooping)
			{
				_src.loop = true;
				_src.clip = loop;
				_isLooping = true;
				_src.Play();
			}
		}
	}
}
