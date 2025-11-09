using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<DeleteUserCommand, bool>
    {
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
