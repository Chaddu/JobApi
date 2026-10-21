using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyReviewsController(
    ICompanyReviewService reviewService) : BaseController
{
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await reviewService.GetByIdAsync(id);

        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetByCompanyId(int companyId)
    {
        var result = await reviewService.GetByCompanyIdAsync(companyId);

        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCompanyReviewRequest request)
    {
        var result = await reviewService.CreateAsync(request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,UpdateCompanyReviewRequest request)
    {
        var result = await reviewService.UpdateAsync(id, request, GetUserId());

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await reviewService.DeleteAsync(id, GetUserId());

        return HandleResult(result);
    }
}
