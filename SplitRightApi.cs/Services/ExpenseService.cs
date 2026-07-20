using SplitRight.API.Data;
using SplitRight.API.Services;
using System;
using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
namespace SplitRightApi.cs.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        private readonly IMemoryCache _cache;
        public ExpenseService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
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
    

        public async Task<PagedResposneDto<ExpenseResponseDto>>GetGroupExpensesAsync(int userId,int groupId,ExpenseQueryDto dto)
        {
            var IsMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == groupId && e.UserId == userId);

            if (!IsMember)
            {
                throw new UnauthorizedAccessException("You Are Not a Member of the Group");
            }

            var CacheKey = $"GroupExpenses:{groupId}";

            if (_cache.TryGetValue(CacheKey, out PagedResposneDto<ExpenseResponseDto>? cached))

                return cached!;

            var query = _context.Expenses.Where(e => e.GroupId == groupId)
                .Include(e => e.Splits)
                .AsQueryable();

           

                //Filtering

            if (!String.IsNullOrWhiteSpace(dto.Category))
            {
                query = query.Where(e => e.Category == dto.Category);
            }

            if (dto.MaximumAmount.HasValue)
            {
                query = query.Where(e => e.Amount <= dto.MaximumAmount);
            }

            if (dto.MinimumAmount.HasValue)

            {
                query = query.Where(e => e.Amount >= dto.MinimumAmount);
            }

            if (dto.From.HasValue)
            {
                query = query.Where(e => e.CreatedAt >= dto.From);
            }
            if (dto.To.HasValue)
            {
                query = query.Where( e => e.CreatedAt <= dto.To);
            }
           if((bool)dto.IsPaid.HasValue)
            {
                query = query.Where(e => e.Splits.Any(e => e.UserId == userId&& e.IsPaid == dto.IsPaid));
            }
        //Sorting

        query = dto.SortBy!.ToLower() switch
            {
                "amount" => dto.Order == "desc" ? query.OrderByDescending(e => e.Amount) : query.OrderBy(e => e.Amount),

                "category" => dto.Order == "desc" ? query.OrderByDescending(e => e.Category) : query.OrderBy(e => e.Category),

                "createdat" => dto.Order == "desc" ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt),

                _=> query.OrderByDescending(e => e.CreatedAt ).ThenBy(e => e.Splits.Any(s => s.UserId == userId)),
            
    };

            //TotalCount

            var TotalCount = await query.CountAsync();

            //Pagination

            var Page = dto.Page ?? 1;
            var PageSize = dto.PageSize ?? 1;

            query = query.Skip((Page - 1) * PageSize).Take(PageSize);

            //Project

            var expense = await query.Where(e => e.GroupId == groupId)
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

            _cache.Set(CacheKey, expense,TimeSpan.FromMinutes(5));

            return new PagedResposneDto<ExpenseResponseDto>
            {
                Data = expense,
                Page = Page,
                PageSize = PageSize,
                TotalCount = TotalCount,
                
               
            };

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

            var CacheKey = $"expense:{ExpenseId}";

            if (_cache.TryGetValue(CacheKey, out ExpenseResponseDto? cached))
                return cached!;

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

            _cache.Set(CacheKey, expenses, TimeSpan.FromMinutes(5));

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

        public async Task<List<GroupSummaryResponseDto>> GroupSummaryAsync(int UserId, int GroupId)
        {
            var IsMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == GroupId && e.UserId == UserId);

            if (!IsMember)
            {
                throw new UnauthorizedAccessException("User Is Not A Member Of This Group");
            }

            var User = await _context.Users.FindAsync(UserId);

            var CacheKey = $"group-summary-{GroupId}";

            

            if (_cache.TryGetValue(CacheKey, out  List<GroupSummaryResponseDto>? cached ))

                return cached!;
          
            var Summary = await _context.ExpenseSplits
                          .Where(s => s.Expense!.GroupId == GroupId && s.IsPaid == false)
                          .Include(s => s.User)
                          .GroupBy(s => new { s.UserId, s.User!.Name })
                          .Select(s => new GroupSummaryResponseDto
                          {
                              UserId = s.Key.UserId,
                              UserName = s.Key.Name!,
                              TotalOwed = s.Sum(e => e.Amount),
                              IsPaid = false,

                          }).ToListAsync();

            _cache.Set(CacheKey, Summary, TimeSpan.FromMinutes(10));
            

            return Summary;


                          

        }

    }
}


