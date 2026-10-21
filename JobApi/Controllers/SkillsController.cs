using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SkillsController(ISkillService skillService) : BaseController
{
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await skillService.GetByIdAsync(id);

        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await skillService.GetAllAsync();

        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSkillRequest request)
    {
        var result = await skillService.CreateAsync(
            request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSkillRequest request)
    {
        var result = await skillService.UpdateAsync(id, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await skillService.DeleteAsync(id, GetUserId());

        return HandleResult(result);
    }
}