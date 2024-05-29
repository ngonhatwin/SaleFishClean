using Microsoft.AspNetCore.SignalR;

namespace SaleFishClean.Web
{
    public class CommentHub : Hub
    {
        public async Task SendComment(string userId, string content)
        {
            await Clients.User(userId).SendAsync("ReceiveComment", content);
        }
    }
}
