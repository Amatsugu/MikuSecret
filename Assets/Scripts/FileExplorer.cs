using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class FileExplorer : MonoBehaviour
{
	//Public
	public Transform canvas;
	public float spacing;
	public GameObject directoryItem;
	public Text curDirText;
	public Text selectedFileText;
	public InputField songNameField;
	//Private
	private List<string> _directories = new List<string>();
	private List<string> _files = new List<string>();
	private List<GameObject> _directoryItems = new List<GameObject>();
	private string _dataPath;
	private float _curLine;
	private List<string> _extentions = new List<string>();
	private float _curPos;
	private AudioClip _song;
	private AudioSource _src;
	private string _selectedFile;
	private RectTransform _scrollView;

	void Start()
	{
		//List Valid File types
		string[] tmp = { ".ogg", ".wav", ".aiff", ".aif", ".mod", ".it", ".s3m", ".xm" };
		_extentions.AddRange(tmp);
		//Cache the scroll view's rect
		_scrollView = canvas.GetComponent<RectTransform>();
		//Cache the audio soruce
		_src = GetComponent<AudioSource>();
		//Set the current directoy
		SetCurrentDirectory(Application.dataPath + "");
		//List the items in the directory
		ListDirectoryItems();
	}

	void Update()
	{
		//Wait for a song to be loaded and play it
		if (_song == null)
			return;
		if (_song.loadState == AudioDataLoadState.Loaded && !_src.isPlaying)
		{
			_src.clip = _song;
			_src.Play();
		}
	}

	//Set the current browsing directory
	void SetCurrentDirectory(string path)
	{
		_dataPath = path;
		_dataPath = _dataPath.Replace('/', Path.DirectorySeparatorChar);
		curDirText.text = _dataPath;
		//Directory.
		Debug.Log(_dataPath);
	}

	//List all directories and valid files in the curreny directory
	void ListDirectoryItems()
	{
		//Reset the UI listing
		ClearOldDirectoryItem();
		//Get the file list
		_files.AddRange(Directory.GetFiles(_dataPath, "*.*"));
		List<string> invalid = new List<string>();
		//Select invalid file types
		foreach (string s in _files)
		{
			if (!_extentions.Contains(Path.GetExtension(s)))
			{
				invalid.Add(s);
			}
		}
		//remove invalid file types
		foreach (string s in invalid)
		{
			_files.Remove(s);
		}
		invalid.Clear();
		//Get Directory List
		_directories.AddRange(Directory.GetDirectories(_dataPath));
		//Create the "go up" directory button
		CreateDirectoryItem("..");
		//Create UI listing of each directory
		foreach (string s in _directories)
		{
			CreateDirectoryItem(s);
		}
		//Create UI listing of each valid file
		foreach (string s in _files)
		{
			CreateDirectoryItem(s);
		}
		//Adjust the size of the scroll area
		_scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (_files.Count + _directories.Count + 1) * spacing);
	}

	//Clear the UI listing of directories and files
	void ClearOldDirectoryItem()
	{
		_files.Clear();
		_directories.Clear();
		foreach (GameObject g in _directoryItems)
			Destroy(g);
		_directoryItems.Clear();
		_curPos = 0;
	}

	//Creates a UI element and sets the parent
	GameObject CreateUIElement(GameObject original, Vector2 position, Transform parent)
	{
		GameObject g = Instantiate(original, position, Quaternion.identity) as GameObject;
		g.transform.SetParent(parent, false);
		g.transform.localPosition = position;
		return g;
	}

	//create a UI element with the default parent
	GameObject CreateUIElement(GameObject original, Vector2 position)
	{
		return CreateUIElement(original, position, canvas);
	}

	//Creates a directory UI listing
	void CreateDirectoryItem(string name)
	{
		GameObject item = CreateUIElement(directoryItem, new Vector2(0, -_curPos));
		string n = name;
        if (name != "..")
		{
			n = Path.GetFileName(name);
		}
		if (_extentions.Contains(Path.GetExtension(n)))
			item.GetComponent<Button>().image.color = Color.cyan;
		item.GetComponentInChildren<Text>().text = n;
		AddListener(item.GetComponent<Button>(), name);
		_directoryItems.Add(item);
		_curPos += spacing;
	}

	//Add the onClick listener to the UI elements
	void AddListener(Button b, string value)
	{
		b.onClick.AddListener(() => ItemClicked(value));
	}

	//Recive the onClick event from UI elements
	void ItemClicked(string item)
	{
		Debug.Log(item);
		if (item == "..")
		{
			MoveUpDirectory();
		}
		else
		{
			if (_extentions.Contains(Path.GetExtension(item)))
			{
				_src.Stop();
				_src.clip = null;
				_song = null;
				StartCoroutine(LoadSongFile(item));
				_selectedFile = item;
				selectedFileText.text = "File: " + Path.GetFileName(item);
				songNameField.text = Path.GetFileNameWithoutExtension(item);
			}
			else
			{
				SetCurrentDirectory(item);
				ListDirectoryItems();
			}
		}
	}

	//Moves up a diectory
	void MoveUpDirectory()
	{
		string[] p = _dataPath.Split(Path.DirectorySeparatorChar);
		Debug.Log(p.Length + "|" + p[0]);
		string newPath = "";
		if (p.Length > 2)
		{
			for (int i = 0; i <= p.Length - 2; i++)
			{
				if (newPath != "")
					newPath += Path.DirectorySeparatorChar + p[i];
				else
					newPath += p[i];
			}
		}
		else
		{
			newPath = p[0] + Path.DirectorySeparatorChar;
		}
		SetCurrentDirectory(newPath);
		ListDirectoryItems();
		Debug.Log(newPath);
	}
	
	//Load an audio file
	IEnumerator LoadSongFile(string path)
	{
		Debug.Log("file:///" + path);
		WWW file = new WWW("file:///" + path);
		_song = file.GetAudioClip(false, false);
		while (_song.loadState != AudioDataLoadState.Loaded)
			yield return file;
	}

	public void ClearSelection()
	{
		songNameField.text = "";
		selectedFileText.text = "File: No File Selected";
		_src.Stop();
		_src.clip = null;
		_song = null;
	}

	public void CreateSong()
	{

	}
}
