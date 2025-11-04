using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
