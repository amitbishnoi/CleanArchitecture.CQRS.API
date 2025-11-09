namespace Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }

        public User User { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }

}
