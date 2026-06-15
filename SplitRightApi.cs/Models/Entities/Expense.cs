namespace SplitRight.API.Models.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public String? Description { get; set; }


        public decimal Amount { get; set; } 

        public string? Category  { get; set; }


        public int GroupId { get; set; }

        public int PaidByUserId { get; set; }

        public User? PaidBy { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.Now;

       

        public Group? Group { get; set; }

       public ICollection<ExpenseSplit>Splits { get; set; } = new List<ExpenseSplit>();


    }
}
