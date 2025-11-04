namespace Domain.Entities
{
    public class Course : BaseEntity
    {
        public required string Title { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
