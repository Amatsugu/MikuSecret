using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISliderColorController : MonoBehaviour
{
	public Image sliderBackground;
	public Text sliderText;

	public void SetColor(Color color)
	{
		sliderBackground.color = color;
	}

	public void SetSliderText(float value)
	{
		sliderText.text = ((int)(255 * value)).ToString();
	}
}
