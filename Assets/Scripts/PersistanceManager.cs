using UnityEngine;

namespace LuminousVector
{
	public class PersistanceManager : MonoBehaviour
	{
		void Start()
		{
			if (FindObjectsOfType<PersistanceManager>().Length > 1)
				Destroy(gameObject);
		}
	}
}
