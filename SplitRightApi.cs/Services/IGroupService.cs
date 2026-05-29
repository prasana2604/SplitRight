using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRightApi.cs.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace SplitRightApi.cs.Services
{
    public interface IGroupService
    {
        Task<GroupResponseDto> CreateGroupAsync(CreateGroupDto dto, int userId);
        Task<List<GroupResponseDto>> GetMyGroupAsync(int userId);

        Task<GroupResponseDto> GetGroupByIdAsync(int groupId, int userId);

        Task<String> AddMemberAsync(int groupId, int userId, AddMemberDto dto);

    }
}
