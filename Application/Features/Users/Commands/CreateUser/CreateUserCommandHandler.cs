using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(IUnitOfWork _unitOfWork,IPasswordHasher _passwordHasher, IEmailService _emailService) : IRequestHandler<CreateUserCommand, int>
    {
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = request.Role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Welcome to RemoteLMS 🎉",
                $"<h3>Hello {user.Name},</h3><p>Welcome aboard! Your account has been created successfully in my new LMS.</p>"
            );

            return user.Id;
        }
    }
}
