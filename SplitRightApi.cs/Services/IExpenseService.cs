using Microsoft.AspNetCore.Identity;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
using SplitRightApi.cs.Models.Entities;
namespace SplitRightApi.cs.Services
{
    public interface IExpenseService
    {
        Task<ExpenseResponseDto> CreateExpenseAsync(int groupId, int userId, CreateExpenseDto dto);
        Task<List<ExpenseResponseDto>> GetGroupExpensesAsync(int groupId, int userId);
        Task<ExpenseResponseDto> GetExpenseByIdAsync(int expenseId, int userId);
        Task<String> MakePaymentAsync(int userId, int expenseId);
    }
}
