using NetFavorite.Models;

namespace NetFavorite.Utilities
{
    public interface ITokenService
    {
        public string CreateToken(Guid userId, string userName, string userRole);
        public LoginUser ReadToken();
    }
}
