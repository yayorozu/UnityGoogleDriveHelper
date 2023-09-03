using System;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper.File
{
	/// <summary>
	/// File の読み込み
	/// </summary>
	[Serializable]
	public class GoogleDriveFileOAuthData : GoogleOAuthData
	{
		protected override string Scope => "https://www.googleapis.com/auth/drive";

		public GoogleDriveFileOAuthData(IGoogleOAuthClient data) : base(data) { }

		/// <summary>
		/// Driveにある画像をロード
		/// </summary>
		public void LoadTexture(
			string id,
			Action<Texture2D> success = null,
			Action<ErrorMessage> error = null
		)
		{
			void Get(string accessToken)
			{
				GoogleDriveFileApi.LoadTexture(accessToken, id, data => success?.Invoke(data));
			}

			GetToken(Get, e => error?.Invoke(e));
		}

		/// <summary>
		/// ファイルの情報
		/// </summary>
		public void LoadFile(
			string id,
			Action<DriveFile> success = null,
			Action<ErrorMessage> error = null
		)
		{
			void Get(string accessToken)
			{
				GoogleDriveFileApi.LoadFile(accessToken, id, data => success?.Invoke(data));
			}

			GetToken(Get, e => error?.Invoke(e));
		}

		/// <summary>
		/// Driveにある画像一覧を取得
		/// </summary>
		public void GetImageList(
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null)
		{
			void Get(string accessToken)
			{
				GoogleDriveFileApi.GetImages(accessToken, success, error);
			}

			GetToken(Get);
		}

		/// <summary>
		/// Drive にある SpreadSheet 一覧を取得
		/// </summary>
		public void GetSpreadSheetList(
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null)
		{
			void Get(string accessToken)
			{
				GoogleDriveFileApi.GetSpreadSheets(accessToken, success, error);
			}

			GetToken(Get);
		}

		/// <summary>
		/// 指定したディレクトリにあるファイル一覧を取得
		/// </summary>
		public void GetDirectoryFiles(
			string id = "root",
			Action<DriveFileList> success = null,
			Action<ErrorMessage> error = null)
		{
			void Get(string accessToken)
			{
				GoogleDriveFileApi.GetTargetDirectoryFiles(accessToken, id, success, error);
			}

			GetToken(Get);
		}
	}
}
