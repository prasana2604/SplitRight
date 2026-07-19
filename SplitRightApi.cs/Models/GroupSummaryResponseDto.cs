namespace SplitRightApi.cs.Models
{
    public class GroupSummaryResponseDto
    {
        public int UserId { get; set; }

        public String UserName { get; set; } = string.Empty;

        public decimal TotalOwed { get; set; }

        public bool IsPaid { get; set; }
    }
}
