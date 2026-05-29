namespace SplitRightApi.cs.Models
{
    public class GroupResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } 

        public int MemberCount { get; set; }
    }
}
