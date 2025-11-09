using Application.Features.Courses.Dtos;
using MediatR;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<List<CourseDto>> { }    
}
