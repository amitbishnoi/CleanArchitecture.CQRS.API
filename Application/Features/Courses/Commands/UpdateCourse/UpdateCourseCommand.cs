using MediatR;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationInHours { get; set; }
    }
}
