using UnityEngine;
using System.Collections.Generic;

namespace LuminousVector
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
		[SerializeField]
		private Dictionary<string, string> stringStore;
		[SerializeField]
		private Dictionary<string, int> intStore;
		[SerializeField]
		private Dictionary<string, bool> boolStore;
		[SerializeField]
		private Dictionary<string, float> floatStore;

		void Start()
		{
			DontDestroyOnLoad(gameObject);
			if (FindObjectOfType<GameRegistry>() as GameRegistry != this)
				Destroy(gameObject);
		}

		void Init()
		{
			stringStore = new Dictionary<string, string>();
			intStore = new Dictionary<string, int>();
			boolStore = new Dictionary<string, bool>();
			floatStore = new Dictionary<string, float>();
		}

		public static string GetString(string id)
		{
			string value;
			instance.stringStore.TryGetValue(id, out value);
			if (value == null)
				value = "";
			return value;
		}

		public static string GetString(string id, string defaultValue)
		{
			if (instance.floatStore.ContainsKey(id))
				return GetString(id);
			else
				SetValue(id, defaultValue);
			return defaultValue;
		}

		public static int GetInt(string id)
		{
			int value;
			instance.intStore.TryGetValue(id, out value);
			return value;
		}

		public static int GetInt(string id, int defaultValue)
		{
			if (instance.floatStore.ContainsKey(id))
				return GetInt(id);
			else
				SetValue(id, defaultValue);
			return defaultValue;
		}

		public static bool GetBool(string id)
		{
			bool value;
			instance.boolStore.TryGetValue(id, out value);
			return value;
		}

		public static bool GetBool(string id, bool defaultValue)
		{
			if (instance.boolStore.ContainsKey(id))
				return GetBool(id);
			else
				SetValue(id, defaultValue);
			return defaultValue;
		}

		public static float GetFloat(string id)
		{
			float value;
			instance.floatStore.TryGetValue(id, out value);
			return value;
		}

		public static float GetFloat(string id, float defaultValue)
		{
			if (instance.floatStore.ContainsKey(id))
				return GetFloat(id);
			else
				SetValue(id, defaultValue);
			return defaultValue;
		}

		public static void SetValue(string id, string value)
		{
			if (instance.stringStore.ContainsKey(id))
				instance.stringStore[id] = value;
			else
				instance.stringStore.Add(id, value);
		}

		public static void SetValue(string id, int value)
		{
			if (instance.intStore.ContainsKey(id))
				instance.intStore[id] = value;
			else
				instance.intStore.Add(id, value);
		}

		public static void SetValue(string id, bool value)
		{
			if (instance.boolStore.ContainsKey(id))
				instance.boolStore[id] = value;
			else
				instance.boolStore.Add(id, value);
		}

		public static void SetValue(string id, float value)
		{
			if (instance.floatStore.ContainsKey(id))
				instance.floatStore[id] = value;
			else
				instance.floatStore.Add(id, value);
		}

		public static void RemoveEntry<T>(string id)
		{
			if (typeof(T) == typeof(string))
			{
				instance.stringStore.Remove(id);
			}
			else if (typeof(T) == typeof(int))
			{
				instance.intStore.Remove(id);
			}
			else if (typeof(T) == typeof(bool))
			{
				instance.boolStore.Remove(id);
			}
			else if (typeof(T) == typeof(float))
			{
				instance.floatStore.Remove(id);
			}
			else
				Debug.LogError("No such type in Game Registry: " + typeof(T));
		}
	}
}
