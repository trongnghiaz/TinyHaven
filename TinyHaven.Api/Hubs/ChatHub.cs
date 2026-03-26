using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TinyHaven.Api.Data;
using TinyHaven.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace TinyHaven.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _db;

        public ChatHub(AppDbContext db)
        {
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "private-room");
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var userIdRaw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "Ẩn danh";

            if (!Guid.TryParse(userIdRaw, out var userId))
                return;

            var entity = new ChatMessage
            {
                RoomKey = "private-room",
                SenderId = userId,
                Message = message.Trim(),
                CreatedAt = DateTimeOffset.UtcNow
            };

            _db.ChatMessages.Add(entity);
            await _db.SaveChangesAsync();

            await Clients.Group("private-room").SendAsync("receiveMessage", new
            {
                id = entity.Id,
                senderId = userId,
                senderName = userName,
                message = entity.Message,
                createdAt = entity.CreatedAt
            });
        }

        public async Task<List<object>> GetLatestMessages(int take = 50)
        {
            take = Math.Clamp(take, 1, 100);

            return await _db.ChatMessages
                .AsNoTracking()
                .Include(x => x.Sender)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .Select(x => new
                {
                    x.Id,
                    x.SenderId,
                    SenderName = x.Sender != null ? x.Sender.DisplayName : "Ẩn danh",
                    x.Message,
                    x.CreatedAt
                })
                .OrderBy(x => x.CreatedAt)
                .Cast<object>()
                .ToListAsync();
        }
    }

}
