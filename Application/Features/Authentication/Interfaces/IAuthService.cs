using Application.Features.Authentication.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> AuthenticateAsync(LoginRequest request);
    }
}
