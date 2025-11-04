using Application.Features.Courses.Queries.GetAllCourses;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);

            if (course == null)
                return null;

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
            };
        }
    }
}
