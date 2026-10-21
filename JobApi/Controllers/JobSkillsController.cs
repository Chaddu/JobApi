using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/jobs/{jobId:int}/skills")]
[ApiController]
public class JobSkillsController(IJobSkillService jobSkillService) : BaseController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetByJobId(int jobId)
    {
        var result = await jobSkillService.GetByJobIdAsync(jobId);

        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(int jobId,AddJobSkillRequest request)
    {
        var result = await jobSkillService.AddAsync(jobId, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{skillId:int}")]
    public async Task<IActionResult> Update(int jobId,int skillId,UpdateJobSkillRequest request)
    {
        var result = await jobSkillService.UpdateAsync(jobId, skillId, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{skillId:int}")]
    public async Task<IActionResult> Remove(int jobId,int skillId)
    {
        var result = await jobSkillService.RemoveAsync(jobId, skillId, GetUserId());

        return HandleResult(result);
    }
}