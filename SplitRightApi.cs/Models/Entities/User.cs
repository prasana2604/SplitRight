using System.Globalization;

namespace SplitRight.API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string? Name { get; set; }    

        public string? PasswordHash { get; set; }

        public string? Role { get; set; }
        
        public string? Email { get; set; }

        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

        public ICollection<Group> Groups { get; set; } = new List<Group>();

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
