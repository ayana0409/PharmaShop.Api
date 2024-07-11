namespace PharmaShop.Api.Abtract
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}