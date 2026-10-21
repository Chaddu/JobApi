using Application.DTOs.Request;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;


[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SavedJobsController(ISavedJobService savedJobService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMySavedJobs()
    {
        var result = await savedJobService.GetMySavedJobsAsync(GetUserId());

        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save(SaveJobRequest request)
    {
        var result = await savedJobService.SaveAsync(request, GetUserId());

        return HandleResult(result);
    }

    [HttpDelete("{jobId:int}")]
    public async Task<IActionResult> Remove(int jobId)
    {
        var result = await savedJobService.RemoveAsync(jobId, GetUserId());

        return HandleResult(result);
    }
}
