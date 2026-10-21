using Application.DTOs.Response;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApi.Controllers;

[Authorize]
public class NotificationsController(INotificationService notificationService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await notificationService.GetMyNotificationsAsync(GetUserId());

        return HandleResult(result);
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetMyUnreadNotifications()
    {
        var result = await notificationService.GetMyUnreadNotificationsAsync(GetUserId());

        return HandleResult(result);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await notificationService.MarkAsReadAsync(id, GetUserId());
        return HandleResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await notificationService.DeleteAsync(id, GetUserId());
        return HandleResult(result);
    }
}
