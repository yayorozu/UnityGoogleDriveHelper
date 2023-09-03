using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Yorozu.GoogleDriveHelper.File
{
	public class GoogleDriveFileApi
	{
		private static readonly string FILE_LIST_URL = "https://www.googleapis.com/drive/v3/files";
		private static readonly string FILE_GET_URL = "https://www.googleapis.com/drive/v3/files/{0}";
		private static readonly string FILE_LOAD_URL = "https://www.googleapis.com/drive/v3/files/{0}?alt=media";
		private static readonly string FIELDS = "?fields=files(kind, id, name, mimeType, version, webViewLink, iconLink, createdTime, modifiedTime, size, parents)";
		private static readonly string FIELDS_SINGLE = "?fields=kind, id, name, mimeType, version, webViewLink, iconLink, createdTime, modifiedTime, size, parents";

		/// <summary>
		/// フィルター
		/// </summary>
		private static readonly string FILE_IMAGE_FILTER = "q=mimeType contains \'image/\'";
		private static readonly string FILE_SPREAD_SHEET_FILTER = "q=mimeType=\'application/vnd.google-apps.spreadsheet\'";
		private static readonly string QUERY_TARGET_DIRECTORY_FILES = "q=\'{0}\' in parents";

		/// <summary>
		/// リンクを公開していればDLできる
		/// </summary>
		public static void LoadShareTexture(
			string hash,
			Action<Texture2D> success = null,
			Action<ErrorMessage> error = null)
		{
			LoadTexture("", hash, success, error);
		}

		internal static void LoadTexture(
			string accessToken,
			string hash,
			Action<Texture2D> success = null,
			Action<ErrorMessage> error = null)
		{
			var url = string.Format(FILE_LOAD_URL, hash);
			var request = UnityWebRequestTexture.GetTexture(url);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var texture = DownloadHandlerTexture.GetContent(request);
				success?.Invoke(texture);
			}

			request.Request(Success, error);
		}

		internal static void LoadFile(
			string accessToken,
			string hash,
			Action<DriveFile> success = null,
			Action<ErrorMessage> error = null)
		{
			var url = string.Format(FILE_GET_URL, hash) + FIELDS_SINGLE;
			var request = UnityWebRequest.Get(url);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var file = JsonUtility.FromJson<DriveFile>(request.downloadHandler.text);
				success?.Invoke(file);
			}

			request.Request(Success, error);
		}

		private static void GetFileList(
			string accessToken,
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null,
			params string[] queries)
		{
			var url = FILE_LIST_URL + FIELDS;
			if (queries.Length > 0)
			{
				url += "&" + string.Join("&", queries);
			}

			var request = UnityWebRequest.Get(url);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var json = request.downloadHandler.text;
				var fileList = JsonUtility.FromJson<DriveFileList>(json);
				success?.Invoke(fileList);
			}

			request.Request(Success, error);
		}

		internal static void GetImages(
			string accessToken,
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null,
			params string[] queries)
		{
			GetFileList(accessToken, success, error, FILE_IMAGE_FILTER);
		}

		internal static void GetSpreadSheets(
			string accessToken,
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null)
		{
			GetFileList(accessToken, success, error, FILE_SPREAD_SHEET_FILTER);
		}

		internal static void GetTargetDirectoryFiles(
			string accessToken,
			string hash,
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null)
		{
			GetFileList(accessToken, success, error, string.Format(QUERY_TARGET_DIRECTORY_FILES, hash));
		}
	}
}
