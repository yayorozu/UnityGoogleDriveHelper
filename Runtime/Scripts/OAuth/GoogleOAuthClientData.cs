using UnityEditor;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper
{
	/// <summary>
	/// Google認証データ
	/// </summary>
	[CreateAssetMenu]
	public class GoogleOAuthClientData : ScriptableObject, IGoogleOAuthClient
	{
		public string ClientId;
		public string ClientSecret;
		
		string IGoogleOAuthClient.ClientId => ClientId;
		string IGoogleOAuthClient.ClientSecret => ClientSecret;

#if UNITY_EDITOR
		public static GoogleOAuthClientData LoadFromEditor()
		{
			var finds = AssetDatabase.FindAssets($"t:{nameof(GoogleOAuthClientData)}");
			if (finds.Length <= 0)
				return null;

			var path = AssetDatabase.GUIDToAssetPath(finds[0]);
			return AssetDatabase.LoadAssetAtPath<GoogleOAuthClientData>(path);
		}
#endif
	}
}
