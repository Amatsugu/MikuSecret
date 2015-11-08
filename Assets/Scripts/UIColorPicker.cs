using UnityEngine;
using UnityEngine.UI;

namespace TheDarkVoid
{
	public class UIColorPicker : MonoBehaviour
	{
		//Public
		public Color color { set { _color = new SColor(value); } get { return _color.color; } }
		public UISliderColorController slider1;
		public UISliderColorController slider2;
		public UISliderColorController slider3;
		public int mode { set { _mode = value; } }
		public float s1
		{
			set
			{
				if (_mode == 0)
				{
					_color.r = value;
					UpdateSliders(value, _color.g, _color.b);
				}
				else
				{
					float h, s, v;
					h = s = v = 0;
					_color.GetHSV(out h, out s, out v);
					h = value;
					_color.SetHSV(h, s, v);
					UpdateSliders(h, s, v);
				}
			}
		}
		public float s2
		{
			set
			{
				if (_mode == 0)
				{
					_color.g = value;
					UpdateSliders(_color.r, value, _color.b);
				}
				else
				{
					float h, s, v;
					h = s = v = 0;
					_color.GetHSV(out h, out s, out v);
					s = value;
					_color.SetHSV(h, s, v);
					UpdateSliders(h, s, v);
				}
			}
		}
		public float s3
		{
			set
			{
				if (_mode == 0)
				{
					_color.b = value;
					UpdateSliders(_color.r, _color.g, value);			
				}
				else
				{
					float h, s, v;
					h = s = v = 0;
					_color.GetHSV(out h, out s, out v);
					v = value;
					_color.SetHSV(h, s, v);
					UpdateSliders(h, s, v);
				}
			}
		}

		//Private
		private int _mode;
		private SColor _color = new SColor();

		public void UpdateSliders()
		{
			if (_mode == 0)
				UpdateSliders(_color.r, _color.g, _color.b);
			else
				UpdateSliders(_color.h, _color.s, _color.v);
		}

		void UpdateSliders(float s1, float s2, float s3)
		{
			if (_mode == 0)
			{
				slider1.SetColor(new Color(s1, 0, 0));
				slider2.SetColor(new Color(0, s2, 0));
				slider3.SetColor(new Color(0, 0, s3));
			}
			else
			{
				slider1.SetColor(SColor.HSVToRGB(s1, 1, 1));
				slider2.SetColor(SColor.HSVToRGB(s1, s2, 1));
				slider3.SetColor(SColor.HSVToRGB(s1, 1, s3));
			}
		}

	}
}
