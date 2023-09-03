using System;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	[Serializable]
	public class PutSheet
	{
		public string spreadsheetId;
		public string updatedRange;
		public int updatedRows;
		public int updatedColumns;
		public int updatedCells;
	}
}
