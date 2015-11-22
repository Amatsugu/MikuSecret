using UnityEngine;
using System.Collections.Generic;

namespace com.LuminousVector
{
	public class GameRegistry : MonoBehaviour
	{
		//Public
		private static GameRegistry GAME_REGISTRY;

		public static GameRegistry instance
		{
			get
			{
				if (!GAME_REGISTRY)
				{
					GAME_REGISTRY = FindObjectOfType<GameRegistry>() as GameRegistry;
					if (!GAME_REGISTRY)
					{
						Debug.LogError("No Game Registry found");
					}
					else
					{
						GAME_REGISTRY.Init();
					}
				}
				return GAME_REGISTRY;
			}
		}
		//Private
		private static Dictionary<string, string> stringStore;
		private static Dictionary<string, int> intStore;
		private static Dictionary<string, bool> boolStore;
		private static Dictionary<string, float> floatStore;

		void Start()
		{
			DontDestroyOnLoad(gameObject);
			if (FindObjectOfType<GameRegistry>() as GameRegistry != gameObject)
				Destroy(gameObject);
		}

		public void Init()
		{
			stringStore = new Dictionary<string, string>();
			intStore = new Dictionary<string, int>();
			boolStore = new Dictionary<string, bool>();
			floatStore = new Dictionary<string, float>();
		}

		public static string GetString(string id)
		{
			string value;
			stringStore.TryGetValue(id, out value);
			return value;
		}

		public static int GetInt(string id)
		{
			int value;
			intStore.TryGetValue(id, out value);
			return value;
		}

		public static bool GetBool(string id)
		{
			bool value;
			boolStore.TryGetValue(id, out value);
			return value;
		}

		public static float GetFloat(string id)
		{
			float value;
			floatStore.TryGetValue(id, out value);
			return value;
		}

		public static void SetValue(string id, string value)
		{
			stringStore.Add(id, value);
		}

		public static void SetValue(string id, int value)
		{
			intStore.Add(id, value);
		}

		public static void SetValue(string id, bool value)
		{
			boolStore.Add(id, value);
		}

		public static void SetValue(string id, float value)
		{
			floatStore.Add(id, value);
		}

		public static void RemoveEntry<T>(string id)
		{
			if (typeof(T) == typeof(string))
			{
				stringStore.Remove(id);
			}
			else if (typeof(T) == typeof(int))
			{
				intStore.Remove(id);
			}
			else if (typeof(T) == typeof(bool))
			{
				boolStore.Remove(id);
			}
			else if (typeof(T) == typeof(float))
			{
				floatStore.Remove(id);
			}
			else
				Debug.LogError("No such type in Game Registry: " + typeof(T));
		}
	}
}
