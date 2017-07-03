using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelController : MonoBehaviour
{
	public void Reset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void GoToPlayer()
	{
		SceneManager.LoadScene("main");
	}

	public void GoToEditor()
	{
		SceneManager.LoadScene("songEditor");
	}

	public void GoToExplorer()
	{
		SceneManager.LoadScene("songBrowser");
	}
}
