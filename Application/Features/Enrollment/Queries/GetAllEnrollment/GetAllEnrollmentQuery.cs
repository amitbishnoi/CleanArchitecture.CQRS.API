using Application.Common.Models;
using Application.Features.Enrollment.Dtos;
using MediatR;

namespace Application.Features.Enrollment.Queries.GetAllEnrollment
{
    public class GetAllEnrollmentQuery : IRequest<List<EnrollmentDto>> 
    {
        public PaginationParams Pagination { get; set; } = new();
    }
}
