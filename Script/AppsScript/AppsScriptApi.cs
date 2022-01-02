using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Yorozu.GoogleDriveHelper.SpreadSheet;

namespace Yorozu.GoogleDriveHelper.AppsScript
{
    public class AppsScriptApi
    {
	    private static readonly string APPS_SCRIPT_DEPLOY_URL = "https://script.google.com/macros/s/{0}/exec";
		
		/// <summary>
		/// AppScript へ Post リクエスト
		/// 最後追加スクリプトを利用した場合
		/// </summary>
		public static void PostAppsScript(
			string deployId,
			string sheetName,
			IList<string> rows,
			int skip = 0,
			Action success = null,
			Action<string> error = null
		)
		{
			void Success(string json)
			{
				var response = JsonUtility.FromJson<GASResponse>(json);
				if (!string.IsNullOrEmpty(response.message))
				{
					error?.Invoke(response.message);
					return;
				}
				success?.Invoke();
			}
			
			var json = AppsScriptUtil.ConvertPostJson(sheetName, rows, skip);
			PostAppsScript(deployId, json, Success, e => error?.Invoke(e.ToString()));
		}
		
		/// <summary>
		/// AppScript へ Post リクエスト
		/// </summary>
		public static void PostAppsScript(
			string deployId,
			string json,
			Action<string> success = null,
			Action<ErrorMessage> error = null
		)
		{
			var url = string.Format(APPS_SCRIPT_DEPLOY_URL, deployId);
			var request = new UnityWebRequest(url, "POST");
			request.AddJson(json);
			
			void Success()
			{
				success?.Invoke(request.downloadHandler.text);
			}

			request.Request(Success, error);
		}
		
		/// <summary>
		/// AppScript への Get リクエスト
		/// </summary>
		public static void GetAppsScript(
			string deployId,
			Dictionary<string, string> getParams,
			Action<string> success = null,
			Action<ErrorMessage> error = null
		)
		{
			var url = string.Format(APPS_SCRIPT_DEPLOY_URL, deployId);
			var ps = getParams.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).ToArray();
			if (ps.Length > 0)
			{
				var gp = string.Join("&", ps);
				url += "?" + gp;
			}
				
			var request = UnityWebRequest.Get(url);
			
			void Success()
			{
				success?.Invoke(request.downloadHandler.text);
			}

			request.Request(Success, error);
		}
		
		/// <summary>
		/// AppScript への Get リクエスト
		/// 戻り値のCSV を string[,] にパースして返す
		/// </summary>
		public static void GetAppsScriptCSV(
			string deployId,
			Dictionary<string, string> getParams,
			Action<string[,]> success = null,
			Action<ErrorMessage> error = null
		)
		{
			void Success(string csv)
			{
				var data = GoogleSpreadSheetUtil.ParseCSV(csv);
				success?.Invoke(data);
			}
			
			GetAppsScript(deployId, getParams, Success, e => error?.Invoke(e));
		}
		
		/// <summary>
		/// AppScript への Get リクエスト
		/// 戻り値のCSV をクラスにパースして返す
		/// </summary>
		public static void GetAppsScript<T>(
			string deployId,
			Dictionary<string, string> getParams,
			Action<List<T>> success = null,
			Action<ErrorMessage> error = null
		) where T : class
		{
			void Convert(string[,] data)
			{
				var list = GoogleSpreadSheetUtil.ListCast<T>(data);
				success?.Invoke(list);
			}
			
			GetAppsScriptCSV(deployId, getParams, Convert, e => error?.Invoke(e));
		}
    }
}