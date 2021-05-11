using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Yorozu.GoogleDriveHelper
{
	public static class GoogleDriveSampleUtil
	{

#if UNITY_EDITOR
		/// <summary>
		/// 認証用のヘッダー記述
		/// </summary>
		public static bool DrawOAuthHeader(this EditorWindow self, GoogleOAuthData data)
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				if (GUILayout.Button("認証コードを取得", EditorStyles.toolbarButton))
				{
					data.GetOAuthCode();
				}

				GUILayout.Label("認証コード:", EditorStyles.toolbarButton);
				using (var check = new EditorGUI.ChangeCheckScope())
				{
					var code = "";
					code = GUILayout.TextField(code, GUILayout.Width(200));
					if (check.changed)
					{
						if (!string.IsNullOrEmpty(code))
						{
							data.DeleteToken();
							data.SetOAuthCode(code);
							data.GetToken();
						}
					}
				}

				if (GUILayout.Button("トークン取得", EditorStyles.toolbarButton))
				{
					data.GetToken(token => self.Repaint());
				}

				if (GUILayout.Button("トークン削除", EditorStyles.toolbarButton))
				{
					data.DeleteToken();
					self.Repaint();
				}

				GUILayout.FlexibleSpace();
			}

			return data.ValidAccessToken;
		}
#endif

		public static void DisplayError(this ErrorMessage self)
		{
			Debug.LogError(self.ToString());
		}
	}
}
