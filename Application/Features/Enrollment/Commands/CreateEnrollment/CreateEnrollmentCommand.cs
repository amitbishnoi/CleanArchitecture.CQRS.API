using MediatR;

namespace Application.Features.Enrollment.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }
}
