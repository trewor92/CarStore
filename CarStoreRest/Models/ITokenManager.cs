using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CarStoreRest.Models
{
    public interface ITokenManager
    {
         Task<Token> GenerateToken(IdentityUser user);
         Task<IdentityUser> GetIdentityUserFromExpireTokenAsync(string accessToken);
         Task<string> GetSavedTokenAsync(IdentityUser user);
    }
}
