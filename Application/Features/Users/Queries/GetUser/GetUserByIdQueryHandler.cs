using Application.Features.Users.Dtos;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Queries.GetUser
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIDQuery, UserDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto?> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUserWithCoursesAsync(request.Id);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
        }
    }
}
