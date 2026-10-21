using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserSkillsController(IUserSkillService userSkillService) : BaseController
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySkills()
    {
        var result = await userSkillService.GetMySkillsAsync(GetUserId());

        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var result = await userSkillService.GetByUserIdAsync(userId);

        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(AddUserSkillRequest request)
    {
        var result = await userSkillService.AddAsync(request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{skillId:int}")]
    public async Task<IActionResult> Update(int skillId,UpdateUserSkillRequest request)
    {
        var result = await userSkillService.UpdateAsync(skillId, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{skillId:int}")]
    public async Task<IActionResult> Remove(int skillId)
    {
        var result = await userSkillService.RemoveAsync(skillId, GetUserId());

        return HandleResult(result);
    }
}