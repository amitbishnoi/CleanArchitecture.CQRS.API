using MediatR;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQuery : IRequest<List<CourseDto>> { }

    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInHours { get; set; }
    }
}
