using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/[controller]")]
[Authorize]
public class JobController(IJobService jobService) : BaseController
{
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await jobService.GetByIdAsync(id);

        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] JobFilterRequest requst)
    {
        var result = await jobService.SearchAsync(requst);

        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var result = await jobService.GetByCompanyIdAsync(companyId, GetUserId());

        return HandleResult(result);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateJobRequest request)
    {
        var result = await jobService.CreateAsync(request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateJobRequest request)
    {
        var result = await jobService.UpdateAsync(id, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPatch("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        var result = await jobService.PublishAsync(id, GetUserId());
        return HandleResult(result);
    }

    [Authorize]
    [HttpPatch("{id:int}/close")]
    public async Task<IActionResult> Close(int id)
    {
        var result = await jobService.CloseAsync(id, GetUserId());

        return HandleResult(result);

    }
}
