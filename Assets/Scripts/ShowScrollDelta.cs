using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowScrollDelta : MonoBehaviour
{
	//Public
	public Text text;
	void Update ()
	{
		text.text = "Detected Scolling: " + Input.GetAxis("Mouse ScrollWheel");
	}
}
