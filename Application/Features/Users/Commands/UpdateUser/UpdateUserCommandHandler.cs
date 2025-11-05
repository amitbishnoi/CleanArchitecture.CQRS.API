using Application.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");

            user.Name = request.Name.Trim();
            user.Email = request.Email.Trim();
            user.PasswordHash = request.Password.Trim();
            user.Role = request.Password.Trim();

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
