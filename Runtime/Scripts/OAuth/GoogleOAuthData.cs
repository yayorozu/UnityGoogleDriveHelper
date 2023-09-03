using System;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper
{
	[Serializable]
	public abstract class GoogleOAuthData
	{
		internal string ClientId { get; }
		internal string ClientSecret { get; }

		protected OAuthToken _token;
		private bool _isReadonly;

		protected GoogleOAuthData(bool isReadonly = true)
		{
			_isReadonly = isReadonly;
		}
		
		protected GoogleOAuthData(IGoogleOAuthClient data, bool isReadonly = true) : this(isReadonly)
		{
			ClientId = data.ClientId;
			ClientSecret = data.ClientSecret;
		}

		/// <summary>
		/// Require Scope
		/// https://developers.google.com/identity/protocols/oauth2/scopes
		/// </summary>
		protected virtual string Scope => "";

		public void GetOAuthCode()
		{
			GoogleOAuthUtil.OpenGoogleOAuth(ClientId, _isReadonly, Scope);
		}

		public bool ValidAccessToken => !string.IsNullOrEmpty(_token.AccessToken) && !_token.IsExpireAccessToken();

		/// <summary>
		/// Tokenをセット
		/// </summary>
		public void SetToken(OAuthToken token)
		{
			_token = token;
		}

		public void SetOAuthCode(string code)
		{
			_token.AuthCode = code;
		}

		public virtual void GetToken(Action<string> success = null, Action<ErrorMessage> error = null)
		{
			if (_token == null)
			{
				Debug.LogError("Require OAuth Token");
				return;
			}

			GoogleOAuthUtil.GetToken(ClientId, ClientSecret, _token, accessToken =>
			{
				success?.Invoke(accessToken);
			}, error);
		}

		public virtual void DeleteToken()
		{
			if (_token == null)
				return;

			_token.RefreshToken = "";
			_token.AccessToken = "";
			_token.Expire = "";
		}

	}
}
