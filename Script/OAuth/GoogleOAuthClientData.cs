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

		public static GoogleOAuthClientData ResourcesLoad(string path)
		{
			var data = Resources.Load<GoogleOAuthClientData>(path);
			return data;
		}
	}
}
