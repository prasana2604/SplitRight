namespace SplitRight.API.Models.Entities
{

    public class ExpenseResponseDto
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        public string PaidBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<SplitResponseDto> Splits { get; set; } = new List<SplitResponseDto>();
    }
}