using Application.Features.Authentication.Dtos;
using Application.Features.Authentication.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Command
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse?>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.AuthenticateAsync(new LoginRequest
            {
                Email = request.Email,
                Password = request.Password
            });
        }
    }

}
