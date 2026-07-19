using Microsoft.AspNetCore.Identity;
using SplitRight.API.Models;
using SplitRight.API.Models.Entities;
using SplitRightApi.cs.Models;
namespace SplitRightApi.cs.Services
{
    public interface IExpenseService
    {
        Task<ExpenseResponseDto> CreateExpenseAsync(int groupId, int userId, CreateExpenseDto dto);
        Task<PagedResposneDto<ExpenseResponseDto>>GetGroupExpensesAsync(int groupId, int userId,ExpenseQueryDto dto);
        Task<ExpenseResponseDto> GetExpenseByIdAsync(int expenseId, int userId);
        Task<String> MakePaymentAsync(int userId, int expenseId);

        Task<List<GroupSummaryResponseDto>> GroupSummaryAsync(int UserId, int GroupId);
    }
}
