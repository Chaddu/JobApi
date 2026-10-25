using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(ICompanyService companyService) : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await companyService.GetAllAsync();
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await companyService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateCompanyRequest request)
    {
        var result = await companyService.CreateAsync(request, GetUserId());
        return HandleResult(result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(
        int id,
        UpdateCompanyRequest request)
    {
        var result = await companyService.UpdateAsync(
            id, request, GetUserId());

        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await companyService.DeleteAsync(id, GetUserId());
        return HandleResult(result);
    }
}