using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheDarkVoid
{
	public class Particle : MonoBehaviour
	{
		//private
		private Vector2 _velocity;
		private float _lifeTime;
		private Vector3 _curPosition;
		private float _curTime;
		private Image _image;
		private Color _startCol;
		private Color _clearCol;

		public void Set(Vector2 velocity, float lifeTime, Color color)
		{
			this._velocity = velocity;
			this._lifeTime = lifeTime;
			this._curPosition = transform.position;
			_image = GetComponent<Image>();
			_image.color = _clearCol = _startCol = color;
			_clearCol.a = 0;
			Destroy(gameObject, _lifeTime);
		}

		public void Set(Vector2 velocity, float lifeTime)
		{
			this._velocity = velocity;
			this._lifeTime = lifeTime;
			this._curPosition = transform.position;
			_image = GetComponent<Image>();
			_startCol = _clearCol = _image.color;
			_clearCol.a = 0;
			Destroy(gameObject, _lifeTime);
		}

		void Update()
		{
			_curPosition.x += _velocity.x * Time.deltaTime;
			_curPosition.y += _velocity.y * Time.deltaTime;
			transform.position = _curPosition;
			_velocity.y -= 98f * Time.deltaTime;
			_image.color = Color.Lerp(_startCol, Color.clear, _curTime / _lifeTime);
			_curTime += Time.deltaTime;

		}


	}
}
