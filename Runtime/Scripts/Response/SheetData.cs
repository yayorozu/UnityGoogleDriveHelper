using System;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	/// <summary>
	/// サーバから返ってくるシートデータ
	/// </summary>
	[Serializable]
	public class SheetData
	{
		[Serializable]
		public class Properties
		{
			public string title;
			public string locale;
			public string timeZone;
			public string autoRecalc;
		}

		public string spreadsheetId;
		public Properties properties;
		public Sheet[] sheets;
		public string spreadsheetUrl;

		public string Title => properties.title;
		public string Id => spreadsheetId;
		public string URL => spreadsheetUrl;
	}

	[Serializable]
	public class Sheet
	{
		[Serializable]
		public class SheetProperties
		{
			public int sheetId;
			public string title;
			public int index;
			public string sheetType;
			public GridProperties gridProperties;

			[Serializable]
			public class GridProperties
			{
				/// <summary>
				/// 行数
				/// </summary>
				public int rowCount;
				/// <summary>
				/// 列数
				/// </summary>
				public int columnCount;
				/// <summary>
				/// 固定行範囲
				/// </summary>
				public int frozenRowCount;
				/// <summary>
				/// 固定列範囲
				/// </summary>
				public int frozenColumnCount;
			}
		}

		[SerializeField]
		public SheetProperties properties;
	}
}
