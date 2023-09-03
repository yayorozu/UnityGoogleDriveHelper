using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	public class GoogleSpreadSheetApi
	{
		private static readonly string SPREAD_SHEET_SHARE_GET_URL = "https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet=\'{1}\'";
		private static readonly string SPREAD_SHEETS_GET_URL = "https://sheets.googleapis.com/v4/spreadsheets/{0}";
		private static readonly string SPREAD_SHEET_GET_URL = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}";
		private static readonly string SPREAD_SHEET_POST_URL = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}:append?valueInputOption=RAW";
		private static readonly string SPREAD_SHEET_PUT_URL = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/{1}?valueInputOption=RAW";
		
		/// <summary>
		/// 公開されているシートを取得
		/// </summary>
		public static void GetSheet(
			string spreadSheetId,
			string sheetName,
			Action<string[,]> loaded = null,
			Action<ErrorMessage> error = null
		)
		{
			var url = string.Format(SPREAD_SHEET_SHARE_GET_URL, spreadSheetId, sheetName);
			var request = UnityWebRequest.Get(url);

			void Success()
			{
				var csv = request.downloadHandler.text;
				var data = GoogleSpreadSheetUtil.ParseCSV(csv);
				loaded?.Invoke(data);
			}

			// csv形式
			request.Request(Success, error);
		}

		/// <summary>
		/// 取得したデータをコンバート
		/// </summary>
		public static void GetSheet<T>(
			string spreadSheetId,
			string sheetName,
			Action<List<T>> loaded = null,
			Action<ErrorMessage> error = null
		) where T : class
		{
			void Convert(string[,] data)
			{
				var list = GoogleSpreadSheetUtil.ListCast<T>(data);
				loaded?.Invoke(list);
			}

			GetSheet(spreadSheetId, sheetName, Convert, error);
		}

		internal static void GetSpreadSheet(
			string accessToken,
			string spreadSheetId,
			Action<SheetData> loaded = null,
			Action<ErrorMessage> error = null)
		{
			var url = string.Format(SPREAD_SHEETS_GET_URL, spreadSheetId);
			var request = UnityWebRequest.Get(url);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var sheetInfo = JsonUtility.FromJson<SheetData>(request.downloadHandler.text);
				loaded?.Invoke(sheetInfo);
			}

			request.Request(Success, error);
		}

		/// <summary>
		/// シートの中身をロード
		/// </summary>
		internal static void GetSheet(
			string accessToken,
			string sheetId,
			Sheet.SheetProperties prop,
			Action<string[,]> onLoaded = null,
			Action<ErrorMessage> error = null)
		{
			var url = string.Format(SPREAD_SHEET_GET_URL, sheetId, prop.title);

			var request = UnityWebRequest.Get(url);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var sheet = GoogleSpreadSheetUtil.ParseJson(request.downloadHandler.text);
				onLoaded?.Invoke(sheet);
			}

			request.Request(Success, error);
		}
		
		/// <summary>
		/// シート最後に追加
		/// </summary>
		internal static void PostSheetData(
			string accessToken,
			string sheetId,
			Sheet.SheetProperties prop,
			string[,] param,
			Action<PostSheet> success = null,
			Action<ErrorMessage> error = null
			)
		{
			var url = string.Format(SPREAD_SHEET_POST_URL, sheetId, $"\'{prop.title}\'");

			var json = GoogleSpreadSheetUtil.ConvertPostJson(param);

			var request = new UnityWebRequest(url, "POST");
			request.AddJson(json);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var post = JsonUtility.FromJson<PostSheet>(request.downloadHandler.text);
				success?.Invoke(post);
			}

			request.Request(Success, error);
		}

		/// <summary>
		/// データの更新
		/// </summary>
		internal static void UpdateSheetData(
			string accessToken,
			string sheetId,
			Sheet.SheetProperties prop,
			string[,] param,
			string range,
			Action<PutSheet> success = null,
			Action<ErrorMessage> error = null)
		{
			var titleRange = $"\'{prop.title}\'!{range}";
			var url = string.Format(SPREAD_SHEET_PUT_URL, sheetId, titleRange);

			var json = GoogleSpreadSheetUtil.ConvertPostJson(param, titleRange);
			var request = new UnityWebRequest(url, "PUT");
			request.AddJson(json);
			request.SetAccessToken(accessToken);

			void Success()
			{
				var put = JsonUtility.FromJson<PutSheet>(request.downloadHandler.text);
				success?.Invoke(put);
			}

			request.Request(Success, error);
		}
	}
}
