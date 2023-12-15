using Microsoft.AspNetCore.SignalR;

namespace ACS_API
{
    public class NotifitionsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).ReceiveNotification($"Подключился: {Context.User?.Identity?.Name}");

            await base.OnConnectedAsync();
        }
    }
    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }
}
