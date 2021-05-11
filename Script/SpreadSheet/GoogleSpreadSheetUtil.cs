using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper.SpreadSheet
{
	public static class GoogleSpreadSheetUtil
	{
		private static readonly string SHEET_URL = "https://docs.google.com/spreadsheets/d/{0}/";

		/// <summary>
		/// SpreadSheetを開く
		/// </summary>
		public static void Open(string sheetHash)
		{
			Application.OpenURL(string.Format(SHEET_URL, sheetHash));
		}

		/// <summary>
		/// データ更新用のJsonに変換
		/// </summary>
		internal static string ConvertPostJson(string[,] values, string range = "")
		{
			var json = new System.Text.StringBuilder();
			json.AppendLine("{");
			if (!string.IsNullOrEmpty(range))
			{
				json.AppendFormat(" \"range\": \"{0}\",\n", range);
			}

			json.AppendLine(" \"majorDimension\": \"ROWS\",");
			{
				json.AppendLine(" \"values\": [");
				for (var y = 0; y < values.GetLength(1); y++)
				{
					json.AppendLine("  [");
					for (var x = 0; x < values.GetLength(0); x++)
					{
						json.AppendFormat("   {0}", values[x, y]);
						if (x < values.GetLength(0) - 1)
							json.Append(",");

						json.AppendLine();
					}
					json.Append("  ]");
					if (y < values.GetLength(1) - 1)
						json.Append(",");
					json.AppendLine();
				}

				json.AppendLine(" ]");
			}
			json.AppendLine("}");
			return json.ToString();
		}

		/// <summary>
		/// Jsonを無理やりパース
		/// </summary>
		internal static string[,] ParseJson(string json)
		{
			var split = json.Split('\n');
			var valuesIndex = Array.FindIndex(split, v => v.Trim() == "\"values\": [");

			var dataList = new List<List<string>>();
			for (var i = valuesIndex + 1; i < split.Length; i++)
			{
				var trim = split[i].Trim();
				if (trim != "[")
					continue;

				i++;

				var j = 0;
				var column = new List<string>();
				while (!split[i + j].Trim().StartsWith("]"))
				{
					var begin = split[i + j].IndexOf("\"", StringComparison.Ordinal) + 1;
					var end = split[i + j].LastIndexOf("\"", StringComparison.Ordinal);
					var text = split[i + j].Substring(begin, end - begin);
					column.Add(text);
					j++;
				}

				dataList.Add(column);
				i += j;
			}

			var yCount = dataList.Count;
			var xCount = dataList.Count > 0 ? dataList[0].Count : 0;
			var sheet = new string[xCount, yCount];
			for (var x = 0; x < xCount; x++)
			{
				for (var y = 0; y < yCount; y++)
				{
					sheet[x, y] = dataList[y][x];
				}
			}

			return sheet;
		}

		/// <summary>
		/// csvを解析
		/// </summary>
		internal static string[,] ParseCSV(string csv)
		{
			var split = csv.Split('\n');
			var dataList = new List<List<string>>();
			foreach (var s in split)
			{
				var row = new List<string>();
				foreach (var cell in s.Split(','))
				{
					var begin = cell.IndexOf("\"", StringComparison.Ordinal) + 1;
					var end = cell.LastIndexOf("\"", StringComparison.Ordinal);
					var text = cell.Substring(begin, end - begin);
					row.Add(text);
				}
				dataList.Add(row);
			}

			var yCount = dataList.Count;
			var xCount = dataList.Count > 0 ? dataList[0].Count : 0;
			var sheet = new string[xCount, yCount];
			for (var x = 0; x < xCount; x++)
			{
				for (var y = 0; y < yCount; y++)
				{
					sheet[x, y] = dataList[y][x];
				}
			}

			return sheet;
		}

		internal static List<T> ListCast<T>(string[,] data) where T : class
		{
			var type = typeof(T);
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var list = new List<T>(data.GetLength(1) - 1);

			int FindArgIndex(string name)
			{
				for (var x = 1; x < data.GetLength(0); x++)
				{
					if (data[x, 0] == name)
						return x;
				}

				return -1;
			}

			// 1行目は変数名
			for (var y = 1; y < data.GetLength(1); y++)
			{
				var instance = Activator.CreateInstance(type, true);
				foreach (var field in fields)
				{
					if (string.IsNullOrEmpty(field.Name))
						continue;

					var index = FindArgIndex(field.Name);
					if (index < 0)
						continue;

					field.SetValue(instance, Parse(field, data[index, y]));
				}

				list.Add(instance as T);
			}
			return list;
		}

		private static object Parse(FieldInfo fieldInfo, string value)
		{
			var type = fieldInfo.FieldType;
			try
			{
				if (type.IsEnum)
					return Enum.Parse(type, value);
				if (type == typeof(string))
					return value;
				if (type == typeof(int))
					return int.Parse(value);
				if (type == typeof(uint))
					return uint.Parse(value);
				if (type == typeof(float))
					return float.Parse(value);
				if (type == typeof(double))
					return double.Parse(value);
				if (type == typeof(long))
					return long.Parse(value);
				if (type == typeof(bool))
					return bool.Parse(value);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}

			return null;
		}

	}
}
