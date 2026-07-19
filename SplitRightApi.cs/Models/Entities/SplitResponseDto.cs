
namespace SplitRight.API.Models.Entities;

public class SplitResponseDto
{
    public int UserId { get; set; } 

    public string UserName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; }

}
