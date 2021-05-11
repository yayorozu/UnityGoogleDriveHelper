using System.Linq;
using UnityEngine;
using UnityEditor;
using Yorozu.GoogleDriveHelper.SpreadSheet;

namespace Yorozu.GoogleDriveHelper
{
	public class GoogleShareSpreadSheetWindow : EditorWindow
	{
		[MenuItem("Tools/GoogleDrive/ShareSpreadSheet")]
		private static void ShowWindow()
		{
			var window = GetWindow<GoogleShareSpreadSheetWindow>();
			window.titleContent = new GUIContent("ShareSpreadSheet");
			window.Show();
		}

		private string _spreadSheetId;
		private string _sheetName;
		private string[,] _sheet;
		private int[] _maxSize;
		private Vector2 _scrollPos;

		private void OnGUI()
		{
			_spreadSheetId = EditorGUILayout.TextField("SpreadSheet Id", _spreadSheetId);
			_sheetName = EditorGUILayout.TextField("Sheet Name", _sheetName);
			if (GUILayout.Button("Load"))
			{
				GoogleSpreadSheetApi.GetSheet(_spreadSheetId, _sheetName, v =>
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
				});
			}

			if (_sheet == null)
				return;

			using (new EditorGUILayout.VerticalScope("box"))
			{
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
									EditorGUILayout.TextArea(_sheet[x, y].Replace("\\n", "\n"),
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
