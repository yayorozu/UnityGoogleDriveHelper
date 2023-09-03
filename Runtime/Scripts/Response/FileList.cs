using System;
using UnityEngine;

namespace Yorozu.GoogleDriveHelper.File
{
	[Serializable]
	public class DriveFileList
	{
		public string kind;
		public bool incompleteSearch;
		public DriveFile[] files;
	}

	[Serializable]
	public class DriveFile
	{
		public string kind;
		public string id;
		public string name;
		public string mimeType;
		public int version;
		public string webViewLink;
		public string iconLink;
		public string createdTime;
		public string modifiedTime;
		public string[] parents;
		/// <summary>
		/// bytes
		/// </summary>
		public long size;

		public DateTime CreateDate => DateTime.Parse(createdTime);
		public DateTime ModifiedDate => DateTime.Parse(modifiedTime);

		public int KByte => Mathf.CeilToInt(size / 1028f);
		public int MByte => Mathf.CeilToInt(size / 1028f / 1028f);

		public bool IsFolder => mimeType == "application/vnd.google-apps.folder";
	}
}
