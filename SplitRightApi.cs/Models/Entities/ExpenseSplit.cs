namespace SplitRight.API.Models.Entities
{
    public class ExpenseSplit
    {
        public int Id { get; set; }

        public int ExpenseId { get; set; } 

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; }
        public User? User { get; set; }

        public Expense? Expense { get; set; }
    }
}
