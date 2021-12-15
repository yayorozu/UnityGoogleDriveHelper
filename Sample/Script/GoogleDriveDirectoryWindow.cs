using System.Linq;
using UnityEngine;
using UnityEditor;
using Yorozu.GoogleDriveHelper.File;

namespace Yorozu.GoogleDriveHelper
{
	public class GoogleDriveDirectoryWindow : EditorWindow
	{
		[MenuItem("Tools/GoogleDriveSample/Folder")]
		private static void ShowWindow()
		{
			var window = GetWindow<GoogleDriveDirectoryWindow>();
			window.titleContent = new GUIContent("GoogleDriveFolder");
			window.Show();
		}

		private Rect rect;

		[SerializeField]
		private GoogleDriveFileOAuthData _data;
		[SerializeField]
		private DriveFileList _list;

		private void OnEnable()
		{
			var data = GoogleOAuthClientData.ResourcesLoad("GoogleAuthData");
			_data = new GoogleDriveFileOAuthData(data);
			_data.SetToken(new EditorOAuthToken(EditorApplication.applicationPath));
		}

		private void OnGUI()
		{
			if (!this.DrawOAuthHeader(_data))
			{
				EditorGUILayout.LabelField("Access Token is Invalid");
				return;
			}

			if (_list == null)
			{
				// GetRoot
				if (GUILayout.Button("GetRoot"))
				{
					_data.GetDirectoryFiles("root", list => _list = list);
				}
				return;
			}

			if (GUILayout.Button("Back"))
			{
				var parents = _list.files[0].parents;
				if (parents != null && parents.Length > 0)
				{
					void Load(DriveFile file)
					{
						if (file.parents == null || file.parents.Length <= 0)
							return;

						UpdateFileList(file.parents.First());
					}

					_data.LoadFile(parents.First(), Load, this.DisplayError);
				}
			}

			foreach (var file in _list.files)
			{
				if (file.IsFolder)
				{
					if (GUILayout.Button(file.name))
					{
						UpdateFileList(file.id);
					}
				}
				else
				{
					GUILayout.Label($"{file.name} ({file.KByte}KB)", GUI.skin.button);
				}
			}
		}

		private void UpdateFileList(string hash)
		{
			_data.GetDirectoryFiles(hash, list =>
			{
				_list = list;
				Repaint();
			}, this.DisplayError);
		}
	}
}
