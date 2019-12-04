using RestSharp.Authenticators;

namespace CarStoreWeb.Models
{
    public interface ITokenAuthenticator : IAuthenticator
    {
        void RefreshTokens();
    }
}
