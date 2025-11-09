using Application.Common.Models;
using Application.Features.Courses.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllCoursesQuery, IReadOnlyList<CourseDto>>
    {
        public async Task<IReadOnlyList<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Courses.GetPagedCoursesAsync(
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                request.Pagination.SearchTerm
            );

            return _mapper.Map<IReadOnlyList<CourseDto>>(result);
        }
    }
}
