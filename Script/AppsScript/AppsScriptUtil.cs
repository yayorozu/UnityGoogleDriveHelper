using System.Collections.Generic;

namespace Yorozu.GoogleDriveHelper.AppsScript
{
    public static class AppsScriptUtil
    {
        /// <summary>
        /// シート名とRowのJsonに変換
        /// </summary>
        internal static string ConvertPostJson(string sheetName, IList<string> rows, int skipRow = 0)
        {
            var json = new System.Text.StringBuilder();
            json.AppendLine("{");

            json.AppendFormat(" \"sheetName\": \"{0}\",\n", sheetName);
            json.AppendFormat(" \"skip\": {0},\n", skipRow);
            {
                json.AppendLine(" \"rows\": [");
                json.AppendLine("  [");
                for (var x = 0; x < rows.Count; x++)
                {
                    json.AppendFormat("   \"{0}\"", rows[x]);
                    if (x < rows.Count - 1)
                        json.Append(",");

                    json.AppendLine();
                }

                json.AppendLine("  ]");

                json.AppendLine(" ]"); // rows
            }
            json.AppendLine("}");
            return json.ToString();
        }   
    }
}