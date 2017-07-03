using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using Mp3Sharp;
//using TagLib;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace LuminousVector
{
	public class FileExplorer : MonoBehaviour
	{
		//Public
		public Transform canvas;
		public float spacing;
		public GameObject directoryItem;
		public Text curDirText;
		public UICreateSongWindow createWindow;
		public UILoadSongWindow loadWindow;
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
		//private Mp3Stream _mp3Stream;

		void Start()
		{
			//List Valid File types
			string[] tmp = { ".ogg", ".wav", ".mod", ".it", ".s3m", ".xm" };
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

		//Set the current browsing directory
		void SetCurrentDirectory(string path)
		{
			_dataPath = path;
			_dataPath = _dataPath.Replace('/', Path.DirectorySeparatorChar);
			curDirText.text = _dataPath;
			//Directory.
		}

		//List all directories and valid files in the curreny directory
		void ListDirectoryItems()
		{
			//Reset the UI listing
			ClearOldDirectoryItem();
			try
			{
				//Get the file list
				_files.AddRange(Directory.GetFiles(_dataPath, "*.*"));
			}
			catch
			{
				//Catch permission errors and recover
				MoveUpDirectory();
				Debug.Log("Errored");
				return;
			}
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
		void CreateDirectoryItem(string path)
		{
			GameObject item = CreateUIElement(directoryItem, new Vector2(0, -_curPos));
			string n = path;
			if (path != "..")
			{
				n = Path.GetFileName(path);
			}
			if (_extentions.Contains(Path.GetExtension(n)) || CheckForSongData(path))
				item.GetComponent<Button>().image.color = Color.cyan;
			item.GetComponentInChildren<Text>().text = n;
			AddListener(item.GetComponent<Button>(), path);
			_directoryItems.Add(item);
			_curPos += spacing;
		}

		//Check for songData
		bool CheckForSongData(string path)
		{
			string[] files = Directory.GetFiles(path);
			bool file = false, data = false;
			foreach(string s in files)
			{
				data = (data) ? data : Path.GetFileName(s) == "Song.SongData";
				file = (file) ? file : _extentions.Contains(Path.GetExtension(s)) && (Path.GetFileNameWithoutExtension(s) == "Song");
			}
			return file && data;
		}

		//Find the song file
		string GetSongFile(string path)
		{
			string[] files = Directory.GetFiles(path);
			string file = null;
			foreach (string s in files)
			{
				if (_extentions.Contains(Path.GetExtension(s)) && (Path.GetFileNameWithoutExtension(s) == "Song"))
					file = s;
			}
			return file;
		}

		//Add the onClick listener to the UI elements
		void AddListener(Button b, string value)
		{
			b.onClick.AddListener(() => ItemClicked(value));
		}

		//Recive the onClick event from UI elements
		void ItemClicked(string item)
		{
			if (item == "..")
			{
				MoveUpDirectory();
			}
			else
			{
				string ext = Path.GetExtension(item);
				if (_extentions.Contains(ext))
				{
					ClearSelection();
					if (ext != ".mp3")
						StartCoroutine(LoadSongFile(item));
					else
					{
						//LoadMp3File(item);
					}
					_selectedFile = item;
					createWindow.OpenWindow();
					createWindow.Set(this, _selectedFile);
				}
				else if(CheckForSongData(item))
				{
					string file = GetSongFile(item);
					ext = Path.GetExtension(file);
					ClearSelection();
					if (ext != ".mp3")
						StartCoroutine(LoadSongFile(file));
					else
					{
						//LoadMp3File(item);
					}
					loadWindow.OpenWindow();
					loadWindow.Set(this, item);
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
		}

		//Load an audio file
		IEnumerator LoadSongFile(string path)
		{
			WWW file = new WWW("file:///" + path);
			_song = file.GetAudioClip(false, false);
			while (_song.loadState != AudioDataLoadState.Loaded)
				yield return file;
			_src.clip = _song;
			_src.Play();
		}

		//Load Mp3
		/*void LoadMp3File(string path)
		{
			_mp3Stream = new Mp3Stream(path);
			Debug.Log(_mp3Stream.Length);
			Debug.Log(_mp3Stream.Format);
			//return;
			byte[] mp3Bytes = new byte[_mp3Stream.Length];

			int bytesReturned = -1;
			int totalBytes = 0;
			Debug.Log("load start");
			while (bytesReturned != 0)
			{
				bytesReturned = _mp3Stream.Read(mp3Bytes, 0, mp3Bytes.Length);
				totalBytes += bytesReturned;
			}
			
			Debug.Log(totalBytes);
			Debug.Log("load end");
			if(bytesReturned == 0)
			{
				_song = new AudioClip();
				float[] fData = BytesToFloats(mp3Bytes);
				_src.clip.SetData(fData, 0);
			}
		}*/

		float[] BytesToFloats(byte[] bytes)
		{
			float[] floatArr = new float[bytes.Length / 4];
			for (int i = 0; i < floatArr.Length; i++)
			{
				if (BitConverter.IsLittleEndian)
					Array.Reverse(bytes, i * 4, 4);
				floatArr[i] = BitConverter.ToSingle(bytes, i * 4) / 0x80000000;
			}
			return floatArr;
		}

		public void ClearSelection()
		{
			//songNameField.text = "";
			//selectedFileText.text = "File: No File Selected";
			_selectedFile = null;
			_src.Stop();
			_src.clip = null;
			_song = null;
		}

		public void CreateSong(Song song)
		{
			string curPath = _dataPath + "/" + GameRegistry.GetString("CUR_SONG");
			Directory.CreateDirectory(curPath);
			if (!File.Exists(_dataPath + "/" + GameRegistry.GetString("CUR_SONG") + "/" + Path.GetFileName(song.songPath)))
			{
				string newPath = GameRegistry.GetString("CUR_SONG") + "/" + Path.GetFileName(song.songPath);
				File.Copy(_dataPath + "/" + song.songPath, _dataPath + "/" + newPath);
				song.songPath = newPath;
			}
			File.WriteAllBytes(curPath + "/Song.SongData", song.getSongData());
			SceneManager.LoadScene("songEditor");
		}
	}
}
