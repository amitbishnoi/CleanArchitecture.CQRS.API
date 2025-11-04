using Application.Features.Courses.Queries.GetAllCourses;
using MediatR;

namespace Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQuery : IRequest<CourseDto?>
    {
        public int Id { get; set; }
        public GetCourseByIdQuery(int id)
        {
            Id = id;
        }
    }
}
