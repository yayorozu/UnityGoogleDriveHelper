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
				EditorGUILayout.Space(5);
				
				if (GUILayout.Button("Get OAuth Code", EditorStyles.toolbarButton))
				{
					data.GetOAuthCode();
				}
				
				EditorGUILayout.Space(5);
				
				if (GUILayout.Button("Paste OAuth Code From Clipboard", EditorStyles.toolbarButton))
				{
					var code = GUIUtility.systemCopyBuffer;
					if (!string.IsNullOrEmpty(code))
					{
						data.DeleteToken();
						data.SetOAuthCode(code);
						data.GetToken();
					}
				}
				
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button("Load Access Token", EditorStyles.toolbarButton))
				{
					data.GetToken(token => self.Repaint());
				}

				if (GUILayout.Button("Delete Access Token", EditorStyles.toolbarButton))
				{
					data.DeleteToken();
					self.Repaint();
				}
				EditorGUILayout.Space(5);
			}

			return data != null && data.ValidAccessToken;
		}
#endif

		
		public static void DisplayError(this EditorWindow self, ErrorMessage errorMessage)
		{
			Debug.LogError(errorMessage);
		}
	}
}
