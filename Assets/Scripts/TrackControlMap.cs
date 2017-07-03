using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackControlMap
{
	private Dictionary<int, KeyMap> _kayMaps = new Dictionary<int, KeyMap>();

	public KeyMap GetMap(int trackCount)
	{
		KeyMap value;
		_kayMaps.TryGetValue(trackCount, out value);
		return value;
	}

	public TrackControlMap AddMap(KeyMap keyMap, int trackCount)
	{
		_kayMaps.Add(trackCount, keyMap);
		return this;
	}
	
	public KeyCode GetKey(int track, int trackCount)
	{
		return GetMap(trackCount).GetKey(track);
	}

	public TrackControlMap AddKey(KeyCode key, int track, int trackCount)
	{
		GetMap(trackCount).AddKey(track, key);
		return this;
	}
}
