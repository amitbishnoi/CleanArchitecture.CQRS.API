using MediatR;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInHours { get; set; }
    }
}
