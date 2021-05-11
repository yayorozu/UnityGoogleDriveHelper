using System;
using System.Text;

namespace Yorozu.GoogleDriveHelper
{
	[Serializable]
	public class ErrorMessage
	{
		public Error error;

		[Serializable]
		public class Error
		{
			public int code;
			public string message;
			public string status;
			public Detail[] details;
		}

		[Serializable]
		public class Detail
		{
			public string type;
			public Link[] links;
			public string reason;
			public string domain;
			public MetaData metadata;
		}

		[Serializable]
		public class Link
		{
			public string description;
			public string url;
		}

		[Serializable]
		public class MetaData
		{
			public string service;
			public string consumer;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendLine($"Error Code: {error.code}");
			builder.AppendLine($"Message: {error.message}");
			builder.AppendLine($"Status: {error.status}");
			if (error.details != null && error.details.Length > 0)
			{
				builder.AppendLine("Details:");
				foreach (var detail in error.details)
				{
					builder.AppendLine($"\tType: {detail.type}");
					if (!string.IsNullOrEmpty(detail.reason))
						builder.AppendLine($"\tReason: {detail.reason}");
					if (!string.IsNullOrEmpty(detail.domain))
						builder.AppendLine($"\tDomain: {detail.domain}");

					if (detail.metadata != null)
					{
						builder.AppendLine("\tMetaData:");
						builder.AppendLine($"\t\tService: {detail.metadata.service}");
						builder.AppendLine($"\t\tConsumer: {detail.metadata.consumer}");
					}

					if (detail.links != null && detail.links.Length > 0)
					{
						builder.AppendLine("\tLinks:");
						foreach (var link in detail.links)
						{
							builder.AppendLine($"\t\tDescription: {link.description}");
							builder.AppendLine($"\t\tUrl {link.url}");
						}
					}
				}
			}

			return builder.ToString();
		}
	}
}
