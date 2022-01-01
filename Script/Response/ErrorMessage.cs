using System;
using System.Text;

namespace Yorozu.GoogleDriveHelper
{
	[Serializable]
	public class ErrorMessage
	{
		public Error error = new Error();

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
					if (!string.IsNullOrEmpty(detail.reason) || !string.IsNullOrEmpty(detail.domain))
					{
						builder.AppendLine($"   Type: {detail.type}");
						if (!string.IsNullOrEmpty(detail.reason))
							builder.AppendLine($"   Reason: {detail.reason}");
						if (!string.IsNullOrEmpty(detail.domain))
							builder.AppendLine($"   Domain: {detail.domain}");
					}

					if (detail.metadata != null && 
					    !string.IsNullOrEmpty(detail.metadata.service) || !string.IsNullOrEmpty(detail.metadata.consumer))
					{
						builder.AppendLine("   MetaData:");
						if (!string.IsNullOrEmpty(detail.metadata.service))
							builder.AppendLine($"      Service: {detail.metadata.service}");
						if (!string.IsNullOrEmpty(detail.metadata.consumer))
							builder.AppendLine($"      Consumer: {detail.metadata.consumer}");
					}

					if (detail.links != null && detail.links.Length > 0)
					{
						builder.AppendLine("   Links:");
						foreach (var link in detail.links)
						{
							builder.AppendLine($"      Description: {link.description}");
							builder.AppendLine($"      Url {link.url}");
						}
					}
				}
			}

			return builder.ToString();
		}
	}
}
