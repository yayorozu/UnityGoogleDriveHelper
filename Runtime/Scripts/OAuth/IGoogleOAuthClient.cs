namespace Yorozu.GoogleDriveHelper
{
    public interface IGoogleOAuthClient
    {
        string ClientId { get; }
        string ClientSecret { get; }
    }
}