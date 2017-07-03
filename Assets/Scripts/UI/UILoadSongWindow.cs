using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections;
namespace LuminousVector
{
	public class UILoadSongWindow : UIWindowManager
	{
		//Public
		public Text title;
		public Text artist;
		public Text tracks;
		public Text creator;
		//Private
		private string _selectedFile;
		private FileExplorer _fe;
		private string songPath;

		public void Set(FileExplorer fe, string file)
		{
			_fe = fe;
			_selectedFile = file;
			Song s = Song.loadSong(File.ReadAllBytes(file + "/Song.SongData"));
			SongInfo song = s.info;
			title.text = "Title: " + song.title;
			artist.text = "Artist: " + song.artist;
			tracks.text = "Tracks: " + s.trackCount;
			creator.text = "Creator: " + song.creator;
		}

		public void LoadSong()
		{
			GameRegistry.SetValue("CUR_SONG", Path.GetFileNameWithoutExtension(_selectedFile));
			SceneManager.LoadScene("songEditor");
			CloseWindow();
		}

		public override void OnWindowClose()
		{
			_fe.ClearSelection();
		}
	}
}
