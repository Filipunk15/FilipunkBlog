namespace FilipunkBlog.Domain.Entities
{
    public class SiteSettings : EntityBase
    {
        public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Available;
        public string? AvailabilityNote { get; set; }
        public string? AvailabilityNoteEn { get; set; }
    }
}
