using MediatR;

namespace Application.Features.Enrollment.Commands.DeleteEnrollment
{
    public class DeleteEnrollmentCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
