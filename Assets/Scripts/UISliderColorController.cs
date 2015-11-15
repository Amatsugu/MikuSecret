using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISliderColorController : MonoBehaviour
{
	public Image sliderBackground;
	public Text sliderText;

	private Slider slider;

	public void SetColor(Color color)
	{
		sliderBackground.color = color;
	}

	public void SetSliderText(float value)
	{
		sliderText.text = ((int)(255 * value)).ToString();
	}

	public void SetValue(float value)
	{
		if (slider == null)
			slider = GetComponent<Slider>();
		slider.value = value;
	}
}
