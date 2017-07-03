using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
namespace LuminousVector
{
	public class UICreateSongWindow : UIWindowManager
	{
		//Public
		public InputField songNameField;
		public Text titleText;
		//Private
		private string _selectedFile;
		private FileExplorer _fe;
		private bool create = false;
		private string songPath;

		public void Set(FileExplorer fe, string file)
		{
			_fe = fe;
			_selectedFile = file;
			titleText.text = Path.GetFileName(file);
		}

		public void CreateSong()
		{
			create = true;
			string dataPath = Application.dataPath + "/Songs";
			songPath = dataPath + "/" + songNameField.text;
			if (Directory.Exists(songPath))
			{
				shouldClose = false;
			}
			CloseWindow();
		}

		public void Cancel()
		{
			shouldClose = true;
			create = false;
			CloseWindow();
		}

		private void Create()
		{
			GameRegistry.SetValue("CUR_SONG", songNameField.text);
			Directory.CreateDirectory(songPath);
			string file = songPath + "/Song" + Path.GetExtension(_selectedFile);
			if (File.Exists(file))
				File.Delete(file);
			File.Copy(_selectedFile, file);
			Song song = new Song("/" + songNameField.text + "/Song" + Path.GetExtension(_selectedFile), 3);
			song.info.title = songNameField.text;
			_fe.CreateSong(song);
		}

		public override void OnWindowClose()
		{
			if (create)
				Create();
			_fe.ClearSelection();
		}
	}
}
