using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await userService.GetByIdAsync(id, GetUserId());
        return HandleResult(result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var result = await userService.UpdateAsync(id, request, GetUserId());
        return HandleResult(result);
    }
    [HttpPatch("{id:int}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleRequest request)
    {
        var result = await userService.ChangeRoleAsync(id, request, GetUserId());
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var result = await userService.ChangePasswordAsync(id, request, GetUserId());
        return HandleResult(result);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await userService.DeleteAsync(id, GetUserId());
        return HandleResult(result);
    }
}
