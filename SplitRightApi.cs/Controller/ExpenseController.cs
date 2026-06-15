using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitRight.API.Services;
using SplitRightApi.cs.Models;
using SplitRightApi.cs.Services;
using System.Security.Claims;

namespace SplitRight.API.Controllers;

[ApiController]
[Route("api/Groups/{groupId}/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private int GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        return int.Parse(userId);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense(int groupId, CreateExpenseDto dto)
    {

        var rawUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (rawUserId == null)
            return BadRequest("UserId claim is null — token issue");


        var result = await _expenseService.CreateExpenseAsync(GetUserId(),groupId,dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroupExpenses(int groupId)
    {
        var result = await _expenseService.GetGroupExpensesAsync(groupId, GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetExpenseById(int groupId, int id)
    {
        var result = await _expenseService.GetExpenseByIdAsync(id, GetUserId());
        return Ok(result);
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int groupId, int id)
    {
        var result = await _expenseService.MakePaymentAsync(id, GetUserId());
        return Ok(result);
    }
}