using System.Linq;
using UnityEngine;
using UnityEditor;
using Yorozu.GoogleDriveHelper.SpreadSheet;

namespace Yorozu.GoogleDriveHelper
{
	public class GoogleSpreadSheetWindow : EditorWindow
	{
		[MenuItem("Tools/GoogleDriveSample/SpreadSheet")]
		private static void ShowWindow()
		{
			var window = GetWindow<GoogleSpreadSheetWindow>();
			window.titleContent = new GUIContent("SpreadSheet");
			window.Show();
		}

		private GoogleSpreadSheetOAuthData _data;
		private SheetData _sheetData;

		private string _spreadSheetId;
		private string[,] _sheet;
		private int[] _maxSize;
		private Vector2 _scrollPos;

		private void OnEnable()
		{
			var data = GoogleOAuthClientData.ResourcesLoad("GoogleAuthData");
			_data = new GoogleSpreadSheetOAuthData(data);
			_data.SetToken(new EditorOAuthToken(EditorApplication.applicationPath));
		}

		private void OnGUI()
		{
			if (!this.DrawOAuthHeader(_data))
			{
				EditorGUILayout.LabelField("Access Token is Invalid");
				return;
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				_spreadSheetId = EditorGUILayout.TextField("SpreadSheet Id", _spreadSheetId);
				if (GUILayout.Button("Load"))
				{
					_data.LoadSpreadSheet(_spreadSheetId, data =>
					{
						_sheetData = data;
						Repaint();
					}, this.DisplayError);
				}
			}

			if (_sheetData == null)
				return;

			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.LabelField("Load SpreadSheets");
				foreach (var sheet in _sheetData.sheets)
				{
					if (GUILayout.Button(sheet.properties.title))
					{
						_data.LoadSheet(_sheetData, sheet, v =>
						{
							_sheet = v;
							_maxSize = new int[_sheet.GetLength(0)];
							for (var x = 0; x < _sheet.GetLength(0); x++)
							{
								_maxSize[x] = Enumerable.Range(0, _sheet.GetLength(1))
									.Select(v => EditorStyles.textField.CalcSize(new GUIContent(_sheet[x, v])))
									.Select(v => Mathf.CeilToInt(v.x))
									.Max();
							}

							Repaint();
						}, this.DisplayError);
					}
				}
			}

			if (_sheet == null)
				return;

			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.LabelField("Sheet");
				using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPos))
				{
					_scrollPos = scroll.scrollPosition;
					using (new EditorGUI.DisabledScope(true))
					{
						for (var y = 0; y < _sheet.GetLength(1); y++)
						{
							using (new EditorGUILayout.HorizontalScope())
							{
								for (var x = 0; x < _sheet.GetLength(0); x++)
								{
									EditorGUILayout.TextArea(
										_sheet[x, y].Replace("\\n", "\n"),
										GUILayout.Width(_maxSize[x]));
								}
							}
						}
					}
				}
			}
		}

	}
}
