using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Editor : MonoBehaviour
{
	//Public
	public Transform canvas;
	public float spacing;
	public GameObject directoryItem;
	//Private
	private List<string> _directories = new List<string>();
	private List<string> _files = new List<string>();
	private List<GameObject> _directoryItems = new List<GameObject>();
	private string _dataPath;
	private float _curLine;
	private List<string> _ententions = new List<string>();
	private float _curPos;

	void Start()
	{
		string[] tmp = { ".mp3", ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm" };
		_curPos = spacing;
		_ententions.AddRange(tmp);
		_dataPath = Application.dataPath + "";
		_files.AddRange(Directory.GetFiles(_dataPath, "*.*"));
		List<string> invalid = new List<string>();
		foreach (string s in _files)
		{
			if (!_ententions.Contains(Path.GetExtension(s)))
			{
				invalid.Add(s);
			}
		}
		foreach (string s in invalid)
		{
			_files.Remove(s);
		}
		invalid.Clear();
		_directories.AddRange(Directory.GetDirectories(_dataPath));
		CreateDirectoryItem("..");
		foreach (string s in _directories)
		{
			CreateDirectoryItem(s);
		}
		foreach (string s in _files)
		{
			CreateDirectoryItem(s);
		}
	}

	GameObject CreateUIElement(GameObject original, Vector2 position, Transform parent)
	{
		GameObject g = Instantiate(original, position, Quaternion.identity) as GameObject;
		g.transform.SetParent(parent);
		g.transform.localPosition = position;
		return g;
	}

	GameObject CreateUIElement(GameObject original, Vector2 position)
	{
		return CreateUIElement(original, position, canvas);
	}

	void CreateDirectoryItem(string name)
	{
		GameObject item = CreateUIElement(directoryItem, new Vector2(spacing, _curPos));
		item.GetComponentInChildren<Text>().text = name;
		_directoryItems.Add(item);
		_curPos += spacing;
	}

	void BrowseForFile()
	{

	}
}
