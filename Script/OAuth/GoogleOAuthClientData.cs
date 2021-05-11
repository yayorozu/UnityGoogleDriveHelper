using UnityEngine;

namespace Yorozu.GoogleDriveHelper
{
	/// <summary>
	/// Google認証データ
	/// </summary>
	[CreateAssetMenu]
	public class GoogleOAuthClientData : ScriptableObject
	{
		public string ClientId;
		public string ClientSecret;

		public static GoogleOAuthClientData Load(string path)
		{
			var data = Resources.Load<GoogleOAuthClientData>(path);
			return data;
		}
	}
}
