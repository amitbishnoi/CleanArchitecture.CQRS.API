using Application.Features.Courses.Dtos;
using Application.Features.Courses.Queries.GetAllCourses;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseByIdQuery, CourseDto?>
    {
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
