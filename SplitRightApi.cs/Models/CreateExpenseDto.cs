namespace SplitRight.API.Models;

public class CreateExpenseDto
{
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }


    public string Category { get; set; } = string.Empty;
}
