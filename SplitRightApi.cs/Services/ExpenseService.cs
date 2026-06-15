using SplitRight.API.Data;
using SplitRight.API.Services;
using System;
using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRightApi.cs.Models.Entities;
using SplitRightApi.cs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
namespace SplitRightApi.cs.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseResponseDto> CreateExpenseAsync(int userId, int groupId, CreateExpenseDto dto)

        {

            var PaidUser = await _context.Users.FindAsync(userId);
            using var transaction = await _context.Database.BeginTransactionAsync();
            try {
               

                var Expense = new Expense
                {
                    Description = dto.Description,
                    Amount = dto.Amount,
                    Category = dto.Category,
                    GroupId = groupId,
                    PaidByUserId = userId
                };

        
                _context.Expenses.Add(Expense);

                await _context.SaveChangesAsync();

                var members = await _context.GroupMembers.Where(e => groupId == e.GroupId).Include(u => u.User).ToListAsync();

                var BaseAmount = Math.Round (dto.Amount / members.Count,2);

                var TotalSplit = BaseAmount * members.Count;

                var remainder = dto.Amount - TotalSplit;

                var memberCount = members.Count;


                var splits = members.Select((m,Index) => new ExpenseSplit
                {
                    ExpenseId = Expense.Id,
                    UserId = m.UserId,
                    IsPaid = m.UserId == userId,
                    CreatedAt = DateTime.UtcNow,

                    Amount = m.UserId == userId ? BaseAmount + remainder :BaseAmount,
                }).ToList();

                
                _context.ExpenseSplits.AddRange(splits);

                await _context.SaveChangesAsync();

                await transaction .CommitAsync();


                return new ExpenseResponseDto
                {

                    Id = Expense.Id,
                    Description = Expense.Description,
                    Amount = Expense.Amount,
                    Category = Expense.Category,

                    PaidBy = PaidUser!.Name!,

                    CreatedAt = DateTime.UtcNow,
                    Splits = splits.Select(s =>
                        new SplitResponseDto
                        {
                            UserId = s.UserId,
                            Amount = s.Amount,
                            UserName = members.First(m => m.UserId == s.UserId).User!.Name!,
                            IsPaid = s.IsPaid
                        }).ToList()

                };

            }

            catch(Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                throw;
            }
        }
    

        public async Task<List<ExpenseResponseDto>>GetGroupExpensesAsync(int userId,int groupId)
        {
            var IsMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == groupId && e.UserId == userId);

            if (!IsMember)
            {
                throw new UnauthorizedAccessException("You Are Not a Member of the Group");
            }

            var expense = await _context.Expenses.Where(e => e.GroupId == groupId)
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .ThenInclude(e => e.User)
                .Select(e => new ExpenseResponseDto
                {

                    Id = e.Id,
                    Description = e.Description!,
                    Category = e.Category!,
                    Amount = e.Amount,
                    PaidBy = e.PaidBy!.Name!,
                    CreatedAt = e.CreatedAt,
                    Splits = e.Splits.Select(e => new SplitResponseDto
                    {
                        UserId = e.UserId,
                        UserName = e.User!.Name!,
                        Amount = e.Amount,
                        IsPaid = e.IsPaid,
                    }).ToList()


                }).ToListAsync();


            return(expense);

        }

        public async Task<ExpenseResponseDto> GetExpenseByIdAsync(int ExpenseId, int userId)
        {

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == ExpenseId);

            var IsMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == expense!.GroupId && e.UserId == userId); 

            if(expense == null)
            
                 throw new KeyNotFoundException("No Excpetion Found");
            

            if (!IsMember)
            {
                throw new UnauthorizedAccessException("Member is a not part of the group");
            }
            var expenses = await _context.Expenses.Where(e => e.Id == ExpenseId)
                .Include(e => e.PaidBy)
                .Include(e => e.Splits)
                .ThenInclude(e => e.User)
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Description = e.Description!,
                    Category = e.Category!,
                    Amount = e.Amount,
                    PaidBy = e.PaidBy!.Name!,
                    CreatedAt = e.CreatedAt,
                    Splits = e.Splits.Select(e => new SplitResponseDto
                    {

                        UserId = e.UserId,
                        UserName = e.User!.Name!,
                        Amount = e.Amount,
                        IsPaid = e.IsPaid,

                    }).ToList()
                }).SingleOrDefaultAsync();

            return expenses!;
            
        }

        public async Task<String>MakePaymentAsync(int userId,int ExpenseId)
        {
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == ExpenseId);

            var IsMember = await _context.GroupMembers.AnyAsync( e => e.GroupId == expense!.GroupId && e.UserId == userId);

            if(expense == null)
            {
                throw new KeyNotFoundException("Expense Not Found");
            }

            if (!IsMember)
            {
                throw new UnauthorizedAccessException("User is not a Member of this Group");
            }

            var result = await _context.ExpenseSplits.FirstOrDefaultAsync(e => e.ExpenseId == ExpenseId && e.UserId == userId);

            if(result == null)
            {
                throw new KeyNotFoundException("Split Not Found");
            }

            if (result!.IsPaid)
            {
                return "It is alraedy paid";
            }

            result.IsPaid = true;



            await _context.SaveChangesAsync();

            return "Payment Done successfully";

            
        }

        

    }

}
