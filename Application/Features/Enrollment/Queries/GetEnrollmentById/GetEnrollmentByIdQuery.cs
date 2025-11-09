using Application.Features.Enrollment.Dtos;
using MediatR;

namespace Application.Features.Enrollment.Queries.GetEnrollmentById
{
    public class GetEnrollmentByIdQuery : IRequest<EnrollmentDto?>
    {
        public int Id { get; set; }
        public GetEnrollmentByIdQuery(int id)
        {
            Id = id;
        }
    }
}