using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CreateCourseCommand, int>
    {
        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var entity = new Course
            {
                Title = request.Title,
                Description = request.Description,
            };

            await _unitOfWork.Courses.AddAsync(entity);
            await _unitOfWork.SaveAsync();
                
            return entity.Id;
        }
    }
}
