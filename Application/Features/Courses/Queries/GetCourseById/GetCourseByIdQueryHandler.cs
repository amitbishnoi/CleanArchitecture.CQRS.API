using Application.Features.Courses.Dtos;
using Application.Features.Courses.Queries.GetAllCourses;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryHandler(IUnitOfWork _unitOfWork, ICacheService _cache) : IRequestHandler<GetCourseByIdQuery, CourseDto?>
    {
        public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"course - {request.Id}";
            var cached = _cache.Get<CourseDto>(cacheKey);
            if (cached is not null)
                return cached;

            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);

            if (course == null)
                return null;

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
            };

            _cache.Set(cacheKey, courseDto, TimeSpan.FromMinutes(5));

            return courseDto;
        }
    }
}
