using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyMap
{
	private Dictionary<int, KeyCode> _keys = new Dictionary<int, KeyCode>();
	
	public KeyMap AddKey(int track, KeyCode key)
	{
		_keys.Add(track, key);
		return this;
	}

	public KeyCode GetKey(int track)
	{
		KeyCode value;
		_keys.TryGetValue(track, out value);
		return value;
	}
}
