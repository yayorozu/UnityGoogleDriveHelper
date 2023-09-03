using System;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper
{
	[Serializable]
    public class OAuthToken
    {
	    public string AuthCode { get; set; }
	    public virtual string RefreshToken { get; set; }
    	public virtual string AccessToken { get; set; }
    	public virtual string Expire { get; set; }

	    public bool CanRequestToken()
        {
	        if (!string.IsNullOrEmpty(RefreshToken))
		        return true;

	        if (!string.IsNullOrEmpty(AuthCode))
		        return true;

	        return false;
        }

        public bool IsExpireAccessToken()
        {
	        var expire = Expire;
	        if (string.IsNullOrEmpty(expire))
		        return true;

	        var datetime = DateTime.Parse(expire);
	        return datetime.CompareTo(DateTime.Now) <= 0;
        }

        public void Update(string json)
        {
	        var token = JsonUtility.FromJson<Token>(json);
	        if (!string.IsNullOrEmpty(token.access_token))
		        AccessToken = token.access_token;

	        if (!string.IsNullOrEmpty(token.refresh_token))
		        RefreshToken = token.refresh_token;

	        if (token.expires_in != 0)
		        Expire = DateTime.Now.AddSeconds(token.expires_in).ToString();

	        AuthCode = "";
        }

        [Serializable]
        private class Token
        {
	        public string refresh_token;
	        public string access_token;
	        public int expires_in;
        }
    }
}
