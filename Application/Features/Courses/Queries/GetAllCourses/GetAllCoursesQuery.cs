using Application.Common.Models;
using Application.Features.Courses.Dtos;
using MediatR;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<IReadOnlyList<CourseDto>> 
    {
        public PaginationParams Pagination { get; set; } = new();

    }
}
