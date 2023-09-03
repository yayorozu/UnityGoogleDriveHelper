using System;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	[Serializable]
	public class PostSheet
	{
		public string spreadsheetId;
		public string tableRange;
		public Update updates;

		[Serializable]
		public class Update
		{
			public string spreadsheetId;
			public string updatedRange;
			public int updatedRows;
			public int updatedColumns;
			public int updatedCells;
		}
	}
}
