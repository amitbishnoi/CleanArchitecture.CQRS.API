namespace Domain.Entities
{
    public class Course : BaseEntity
    {
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public int InstructorId { get; set; }

        public User Instructor { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
