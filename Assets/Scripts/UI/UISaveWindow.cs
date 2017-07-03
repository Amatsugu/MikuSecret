using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LuminousVector
{
	public class UISaveWindow : UIWindowManager
	{
		//Public
		public string fileName
		{
			set
			{
				_fileName = value;
			}
			get
			{
				return _fileName;
			}
		}
		//Private
		private string _fileName;
		private bool _shouldSave;

		public void Save()
		{
			_shouldSave = true;
			CloseWindow();
		}

		public override void OnWindowClose()
		{
			if (!_shouldSave)
				return;
			GameRegistry.SetValue("CUR_SONG", _fileName);
			SongEditor.instance.SaveSong(null);
		}
	}
}
