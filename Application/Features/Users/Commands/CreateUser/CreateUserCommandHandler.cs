using Application.Interfaces;
using Application.Interfaces.SecurityInterface;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

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

            return user.Id;
        }
    }
}
