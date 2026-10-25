using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Authorize]
[Route("api/[controller]")]
public class JobApplicationsController(IJobApplicationService jobApplicationService) : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await jobApplicationService.GetByIdAsync(id, GetUserId());

        return HandleResult(result);
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications()
    {
        var result = await jobApplicationService.GetMyApplicationsAsync(GetUserId());
        return HandleResult(result);
    }
    [HttpGet("job/{jobId:int}")]
    public async Task<IActionResult> GetByJobId(int jobId)
    {
        var result = await jobApplicationService.GetByJobIdAsync(jobId, GetUserId());
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Apply(CreateJobApplicationRequest request)
    {
        var result = await jobApplicationService.ApplyAsync(request, GetUserId());
        return HandleResult(result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateJobApplicationRequest request)
    {
        var result = await jobApplicationService.UpdateAsync(id, request, GetUserId());
        return HandleResult(result);
    }

    [HttpPatch("{id:int}/review")]
    public async Task<IActionResult> StartReview(int id)
    {
        var result = await jobApplicationService.StartReviewAsync(id, GetUserId());

        return HandleResult(result);
    }
    [HttpPatch("{id:int}/interview")]
    public async Task<IActionResult> ScheduleInterview(int id)
    {
        var result = await jobApplicationService.ScheduleInterviewAsync(id, GetUserId());

        return HandleResult(result);
    }

    [HttpPatch("{id:int}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var result = await jobApplicationService.AcceptAsync(id, GetUserId());

        return HandleResult(result);
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await jobApplicationService.RejectAsync(id, GetUserId());

        return HandleResult(result);
    }

    [HttpPatch("{id:int}/withdraw")]
    public async Task<IActionResult> Withdraw(int id)
    {
        var result = await jobApplicationService.WithdrawAsync(id, GetUserId());

        return HandleResult(result);
    }
}
