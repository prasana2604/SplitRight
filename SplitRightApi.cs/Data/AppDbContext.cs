using Microsoft.EntityFrameworkCore;
using SplitRight.API.Models.Entities;
namespace SplitRight.API.Data;

public class AppDbContext : DbContext

{
    public AppDbContext(DbContextOptions<AppDbContext> Options) : base(Options)
    {

    }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Group> Groups { get; set; }

    public DbSet<User> Users { get; set; }



    public DbSet<GroupMember> GroupMembers { get; set; }

    public DbSet<ExpenseSplit>ExpenseSplits { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupMember>()
        .HasOne(u => u.User)
        .WithMany(g => g.GroupMembers)
        .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupMember>()
        .HasOne(g => g.Group)
        .WithMany(g => g.Members)
        .HasForeignKey(g => g.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
        .HasOne(u => u.Group)
        .WithMany(e => e.Expenses)
        .HasForeignKey(g => g.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
        .HasOne(p => p.PaidBy)
        .WithMany(e => e.Expenses)
        .HasForeignKey(p => p.PaidByUserId)
        .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ExpenseSplit>()
        .HasOne(u => u.User)
        .WithMany()
        .HasForeignKey(u => u.UserId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseSplit>()
        .HasOne(e => e.Expense)
        .WithMany(e => e.Splits)
        .HasForeignKey(e => e.ExpenseId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Group>()
        .HasOne(u => u.CreatedBy)
        .WithMany()
        .HasForeignKey(e => e.CreatedByUserId)
        .OnDelete(DeleteBehavior.Restrict);



}
}
