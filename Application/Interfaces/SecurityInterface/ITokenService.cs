using Domain.Entities;

namespace Application.Interfaces.SecurityInterface
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
