using System;

#if UNITY_EDITOR

using UnityEditor;

namespace Yorozu.GoogleDriveHelper
{
	[Serializable]
	public class EditorOAuthToken : OAuthToken
	{
		private string _refreshTokenKey;
		private string _accessTokenKey;
		private string _accessTokenExpireKey;

		public EditorOAuthToken(string key)
		{
			_refreshTokenKey = "GoogleAuthRefreshToken" + key;
			_accessTokenKey = "GoogleAuthAccessToken" + key;
			_accessTokenExpireKey = "GoogleAuthAccessExpire:" + key;
		}

		// OAuth Code が使えるのは一度切りなので保存しない
		public override string RefreshToken
		{
			get => EditorPrefs.GetString(_refreshTokenKey);
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					EditorPrefs.DeleteKey(_refreshTokenKey);
					return;
				}
				EditorPrefs.SetString(_refreshTokenKey, value);
			}
		}
		public override string AccessToken
		{
			get => EditorPrefs.GetString(_accessTokenKey);
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					EditorPrefs.DeleteKey(_accessTokenKey);
					return;
				}
				EditorPrefs.SetString(_accessTokenKey, value);
			}
		}

		public override string Expire
		{
			get => EditorPrefs.GetString(_accessTokenExpireKey);
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					EditorPrefs.DeleteKey(_accessTokenExpireKey);
					return;
				}
				EditorPrefs.SetString(_accessTokenExpireKey, value);
			}
		}
	}
}
#endif
