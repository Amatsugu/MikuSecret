using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LuminousVector
{
	public class UISongConfigurationWindow : UIWindowManager
	{
		//Public
		public InputField songNameField;
		public InputField artistNameField;
		public InputField albumNameField;
		public InputField releasedYearField;
		public InputField creatorNameField;
		//Private
		private SongInfo _songInfo;
		public void Set(string header)
		{
			_songInfo = SongEditor.SONG_EDITOR.curSong.info;
            songNameField.text = _songInfo.title;
			artistNameField.text = _songInfo.artist;
			albumNameField.text = _songInfo.album;
			releasedYearField.text = _songInfo.year;
			creatorNameField.text = _songInfo.creator;
			SetHeader(header);
		}

		public void UIChange()
		{
			shouldClose = false;
		}

		public void SetSongName(string name)
		{
			SetHeader(name);
		}

		public void SaveAndClose()
		{
			shouldClose = true;
			_songInfo.title = songNameField.text;
			_songInfo.artist = artistNameField.text;
			_songInfo.album = albumNameField.text;
			_songInfo.year = releasedYearField.text;
			_songInfo.creator = creatorNameField.text;
			CloseWindow();
		}
	}
}
