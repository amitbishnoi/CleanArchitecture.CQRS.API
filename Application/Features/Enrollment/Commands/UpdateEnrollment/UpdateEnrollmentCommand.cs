using MediatR;

namespace Application.Features.Enrollment.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }
}
