using System;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;

namespace Yorozu.GoogleDriveHelper
{
	/// <summary>
    /// Token まわりの取得処理
    /// </summary>
	internal class GoogleOAuthUtil
    {
	    private static readonly string GOOGLE_AUTH_URL = "https://www.googleapis.com/oauth2/v4/token";

		/// <summary>
		/// ブラウザで認証ページを開く
		/// </summary>
		internal static void OpenGoogleOAuth(string clientId, bool isReadOnly = true, string scope = "")
		{
			var builder = new StringBuilder();

			builder.Append("https://accounts.google.com/o/oauth2/auth");
			builder.Append("?response_type=code");
			builder.Append("&access_type=offline");
			builder.AppendFormat("&client_id={0}", clientId);
			builder.Append("&redirect_uri=urn%3Aietf%3Awg%3Aoauth%3A2.0%3Aoob&state");
			builder.Append("&approval_prompt=force");
			if (!string.IsNullOrEmpty(scope))
			{
				builder.AppendFormat("&scope={0}", scope);
				if (isReadOnly)
					builder.Append(".readonly");
			}

			var url = builder.ToString();
			Application.OpenURL(url);
		}

		/// <summary>
		/// Token の更新
		/// </summary>
		internal static void GetToken(
			string clientId,
			string clientSecret,
			OAuthToken token,
			Action<string> onLoaded = null,
			Action<ErrorMessage> error = null
		)
		{
			if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
			{
				Debug.LogError("Client Data is Empty");
				return;
			}

			// AccessToken が有効な場合
			if (!string.IsNullOrEmpty(token.AccessToken) && !token.IsExpireAccessToken())
			{
				onLoaded?.Invoke(token.AccessToken);
				return;
			}

			if (!token.CanRequestToken())
			{
				Debug.Log("Set OAuth Code");
				return;
			}

			var form = new WWWForm();
			form.AddField("client_id", clientId);
			form.AddField("client_secret", clientSecret);
			form.AddField("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
			form.AddField("access_type", "offline");

			// リフレッシュトークンがなければ取得
			if (string.IsNullOrEmpty(token.RefreshToken))
			{
				form.AddField("code", token.AuthCode);
				form.AddField("grant_type", "authorization_code");
			}
			else
			{
				form.AddField("refresh_token", token.RefreshToken);
				form.AddField("grant_type", "refresh_token");
			}

			var request = UnityWebRequest.Post(GOOGLE_AUTH_URL, form);
			request.Request(() =>
			{
				token.Update(request.downloadHandler.text);
				onLoaded?.Invoke(token.AccessToken);
			}, e =>
			{
				error?.Invoke(e);
			});
		}
    }
}
