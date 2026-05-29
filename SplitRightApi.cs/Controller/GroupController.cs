using SplitRightApi.cs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SplitRight.API.Models.Entities;
using SplitRight.API.Models;
using SplitRight.API.Data;
using SplitRight.API.Services;
using SplitRightApi.cs.Models;
using Microsoft.AspNetCore.Authorization;

namespace SplitRightApi.cs.Controller
{
    
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private IGroupService _GroupService;

        public GroupController (IGroupService GroupService)
        {
            _GroupService = GroupService;
        }
       

        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            return int.Parse(userId);
        }

        [HttpPost("CreateGroup")]

        public async Task<IActionResult>CreateGroup(CreateGroupDto dto)
        {
            var result =await _GroupService.CreateGroupAsync(dto, GetUserId());

            return Ok(result);
        }

        [HttpGet]

        public async Task<IActionResult> GetMyGroups()
        {
            var result = await _GroupService.GetMyGroupAsync(GetUserId());

            return Ok(result);

        }


        [HttpGet("{Id}")]

        public async Task<IActionResult>GetGroupById(int id)
        {
            var result = await _GroupService.GetGroupByIdAsync(id, GetUserId());

            return Ok(result);
        }

        [HttpPost("{Id}/member")]

        public async Task<IActionResult>AddMemberAsync(int id,AddMemberDto dto)
        {
            var result = await _GroupService.AddMemberAsync(id,GetUserId(),dto);

            return Ok(result);
        }
    }
}
