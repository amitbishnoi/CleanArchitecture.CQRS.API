namespace Application.Features.Enrollment.Dtos
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string CourseTitle { get; set; }
    }
}
