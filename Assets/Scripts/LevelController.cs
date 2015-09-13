using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
	public void Reset()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void GoToPlayer()
	{
		Application.LoadLevel("main");
	}

	public void GoToEditor()
	{
		Application.LoadLevel("songEditor");
	}
}
