using System;
using System.Collections.Generic;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	/// <summary>
	/// スプレッドシートのロード処理まわり
	/// </summary>
	[Serializable]
	public class GoogleSpreadSheetOAuthData : GoogleOAuthData
	{
		public GoogleSpreadSheetOAuthData(IGoogleOAuthClient data, bool isReadonly = true) : base(data, isReadonly)
		{
		}

		protected override string Scope => "https://www.googleapis.com/auth/spreadsheets";

		/// <summary>
		/// スプレッドシート情報を取得
		/// </summary>
		public void LoadSpreadSheet(string spreadSheetId, Action<SheetData> load = null, Action<ErrorMessage> error = null)
		{
			void Get(string accessToken)
			{
				GoogleSpreadSheetApi.GetSpreadSheet(
					accessToken, 
					spreadSheetId, 
					data => load?.Invoke(data),
					e => error?.Invoke(e));
			}

			GetToken(Get, error);
		}

		/// <summary>
		/// シートの中身を取得
		/// </summary>
		public void LoadSheet(SheetData data, Sheet sheet, Action<string[,]> load = null, Action<ErrorMessage> error = null)
		{
			if (data.sheets == null)
				return;

			void Get(string accessToken)
			{
				GoogleSpreadSheetApi.GetSheet(
					accessToken,
					data.Id,
					sheet.properties,
					array => load?.Invoke(array),
					e => error?.Invoke(e)
				);
			}

			GetToken(Get, error);
		}

		/// <summary>
		/// クラスに変換
		/// </summary>
		public void LoadSheet<T>(
			SheetData data,
			Sheet sheet,
			Action<List<T>> load = null,
			Action<ErrorMessage> error = null
			) where T : class
		{
			void Convert(string[,] arg)
			{
				var list = GoogleSpreadSheetUtil.ListCast<T>(arg);
				load?.Invoke(list);
			}

			LoadSheet(data, sheet, Convert, error);
		}

		/// <summary>
		/// シートに追加
		/// </summary>
		public void Add(
			SheetData data,
			Sheet sheet,
			string[,] param,
			Action<PostSheet> success = null,
			Action<ErrorMessage> error = null)
		{
			void Add(string accessToken)
			{
				GoogleSpreadSheetApi.PostSheetData(
					accessToken,
					data.Id,
					sheet.properties,
					param,
					post => success?.Invoke(post),
					e => error?.Invoke(e)
				);
			}

			GetToken(Add, error);
		}

		/// <summary>
		/// シートを更新
		/// </summary>
		public void Update(
			SheetData data,
			Sheet sheet,
			string[,] param,
			string range = "A1",
			Action<PutSheet> success = null,
			Action<ErrorMessage> error = null)
		{
			void Update(string accessToken)
			{
				GoogleSpreadSheetApi.UpdateSheetData(
					accessToken,
					data.Id,
					sheet.properties,
					param,
					range,
					put => success?.Invoke(put),
					e => error?.Invoke(e)
				);
			}

			GetToken(Update, error);
		}
	}
}
