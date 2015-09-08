using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Editor : MonoBehaviour
{
	//Public
	public Transform canvas;
	public float spacing;
	public GameObject Item;
	//Private
	private string[] _directories;
	private List<string> _files;
	private List<GameObject> _directoryItems = new List<GameObject>();
	private string _dataPath;
	private float _curLine;
	private string[] _ententions = { ".mp3", ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm" };

	void Start()
	{
		_dataPath = UnityEngine.Application.dataPath + "/Songs/";
		_files.AddRange(Directory.GetFiles(_dataPath, "*.*"));
		CreateUIElement(Item, new Vector2(0, 0));
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

	void BrowseForFile()
	{

	}
}
