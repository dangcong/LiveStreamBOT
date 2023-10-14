namespace BaseSouce.Services.Services.ValidateLink
{
    public interface IValidateLinkService
    {
        Task<KeyValuePair<bool, string>> ValidateLinkAsync(string link, string? name = null);
    }
}
