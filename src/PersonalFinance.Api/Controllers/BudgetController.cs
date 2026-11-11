using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Budgets;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.Api.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(
        IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("{year:int}/{month:int}")]
    public async Task<ActionResult<BudgetResponse>> Get(
        int year,
        int month)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var budget =
                await _budgetService.GetAsync(
                    userId,
                    year,
                    month);

            if (budget is null)
            {
                return NotFound();
            }

            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{year:int}/{month:int}")]
    public async Task<ActionResult<BudgetResponse>> Set(
        int year,
        int month,
        SetBudgetRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var budget =
                await _budgetService.SetAsync(
                    userId,
                    year,
                    month,
                    request);

            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{year:int}/{month:int}")]
    public async Task<IActionResult> Delete(
        int year,
        int month)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var deleted =
                await _budgetService.DeleteAsync(
                    userId,
                    year,
                    month);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    private bool TryGetCurrentUserId(
        out Guid userId)
    {
        var userIdClaim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        return Guid.TryParse(
            userIdClaim,
            out userId);
    }
}