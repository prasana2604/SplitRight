namespace SplitRight.API.Models.Entities
{
    public class ExpenseQueryDto
    {
        public string? Category {  get; set; }

        public decimal ? MaximumAmount  { get; set; }

        public decimal ? MinimumAmount { get; set; }

        public bool? IsPaid { get; set; } 
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string? SortBy { get; set; } = "CreatedAt";

        public string? Order { get; set; } = "desc";

        public int? PageSize { get; set; } = 10;

        public int? Page { get; set; } = 1;
    }
}
