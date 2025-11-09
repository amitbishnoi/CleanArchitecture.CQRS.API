using Application.Features.Courses.Dtos;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
    {
        public async Task<List<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
            }).ToList();
        }
    }
}
