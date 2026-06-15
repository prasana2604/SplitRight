using SplitRight.API.Services;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
using SplitRight.API.Data;
using SplitRight.API.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
namespace SplitRightApi.cs.Services
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _context;

        public GroupService (AppDbContext context)
        {
            _context = context;
        }
        public async Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto dto, int userId)
        {

            var user = await _context.Users.FindAsync(userId);
            var group = new Group
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now,
            };
            _context.Add(group);
            await _context.SaveChangesAsync();

            var GroupMember = new GroupMember
            {
                Group = group,
                UserId = userId,
                JoinedAt = DateTime.Now,
            };

            _context.Add(GroupMember);
            await _context.SaveChangesAsync();


            return new GroupResponseDto
            {


                Id = group.Id,

                Name = group.Name,

                Description = group.Description,

                CreatedBy = user!.Name!,

                CreatedAt = DateTime.UtcNow,

                MemberCount = 1
            };
        }

        public async Task<List<GroupResponseDto>>GetMyGroupAsync(int userId) { 

            var groups = await _context.GroupMembers.Where(e => e.UserId == userId)
                .Include(em => em.Group)
                .ThenInclude(e => e!.Members)
                .Include(em => em.Group)
                .ThenInclude(e => e!.CreatedBy)
                .Select(gm => new GroupResponseDto
                {
                    Id = gm.Group!.Id,
                    Name = gm.Group.Name!,
                    Description = gm.Group.Description!,
                    CreatedBy = gm.Group.CreatedBy!.Name!,
                    CreatedAt = gm.Group.CreatedAt,
                    MemberCount = gm.Group.Members.Count


                }).ToListAsync();

            return groups;

            
        }

        public async Task<GroupResponseDto> GetGroupByIdAsync(int groupId, int userId)
        {
            {

                var IsMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == groupId && e.UserId == userId);

                if (!IsMember)
                {
                    throw new UnauthorizedAccessException("You are not member of this group");

                }

                var groups = _context.Groups.Include(e => e.Members)
                    .Include(e => e.CreatedBy)
                    .FirstOrDefault(e => e.Id == groupId);


                if (groups == null)
                {
                    throw new KeyNotFoundException("Group Not Found");
                }

                return new GroupResponseDto
                {
                    Id = groups.Id,

                    Name = groups!.Name!,

                    Description = groups!.Description!,

                    CreatedBy = groups!.CreatedBy!.Name!,

                    CreatedAt = groups.CreatedAt,

                    MemberCount = groups!.Members.Count

                };
            }
        }
        
        public async Task<String>AddMemberAsync(int groupId,int userId,AddMemberDto dto)
        {
            var group = _context.Groups.FirstOrDefault(e => e.Id == groupId);

            if(group == null)
            {
                throw new KeyNotFoundException("Group Not Found");
            }

            if(group.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("Only Creator can add the member");
            }

            var AlreadyMember = await _context.GroupMembers.AnyAsync(e => e.GroupId == groupId && e.UserId == dto.UserId);

            if (AlreadyMember)
            {

                return "User is already an Member";

            }

            var ExistingUser = await _context.Users.AnyAsync(e => e.Id == dto.UserId);

            if (!ExistingUser)
            {
                throw new KeyNotFoundException("UserNotFound");
            }

            var member = new GroupMember
            {
                UserId = dto.UserId,

                GroupId = groupId,

                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(member);

            await _context.SaveChangesAsync();

            return "Member Created Successfully";
        }
    }
}
