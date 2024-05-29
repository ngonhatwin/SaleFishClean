using Microsoft.AspNetCore.SignalR;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using System.Security.Claims;

namespace SaleFishClean.Web
{
    public class ChatHub : Hub
    {
        public static List<UserConnectionRequest> userConnectionRequests = new List<UserConnectionRequest>();
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserServices _userService;
        private readonly IChatServices _chatService;
        public ChatHub(IHttpContextAccessor contextAccessor, IUserServices userService, IChatServices chatService)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            // Lấy thông tin đăng nhập của người dùng từ Context.User hoặc bất kỳ nguồn dữ liệu nào khác
            var userId = GetUserIdFromClaim(); // Ví dụ: lấy UserId từ thông tin đăng nhập của người dùng
            var userName = await _userService.GetUserNameByUserIdAsync(userId);
            if (userId != null)
            {
                // Thêm vào danh sách yêu cầu kết nối của người dùng
                userConnectionRequests.Add(new UserConnectionRequest
                {
                    userName = userName,
                    userId = userId,
                    ConnectionId = connectionId
                });

                // Gửi danh sách người dùng trực tuyến cho người dùng hiện tại
                await Clients.All.SendAsync("ReceiveOnlineUsers", userConnectionRequests);
            }
            await base.OnConnectedAsync();
        }

        public async Task LoadMessageHistory(string userId)
        {
            var currentUserId = GetUserIdFromClaim();
            if (currentUserId != null && userId != null)
            {
                var messages = await _chatService.GetMessagesAsync(currentUserId, userId);
                await Clients.Caller.SendAsync("LoadMessages", messages);
            }
        }

        public async Task LogoutUser()
        {
            var connectionId = Context.ConnectionId;

            // Loại bỏ người dùng đăng xuất khỏi danh sách người dùng trực tuyến
            var userToRemove = userConnectionRequests.FirstOrDefault(u => u.ConnectionId == connectionId);
            if (userToRemove != null)
            {
                userConnectionRequests.Remove(userToRemove);

                // Gửi danh sách người dùng trực tuyến đã được cập nhật cho tất cả các người dùng
                await Clients.All.SendAsync("ReceiveOnlineUsers", userConnectionRequests);
            }

            // Ngắt kết nối của người dùng
            await Clients.Client(connectionId).SendAsync("Logout");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Lấy ConnectionId của người dùng hiện tại
            var connectionId = Context.ConnectionId;
            userConnectionRequests.RemoveAll(a => a.ConnectionId == connectionId);
            // Thêm vào danh sách yêu cầu kết nối của người dùng
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            // Lấy ID của người gửi tin nhắn
            var senderUserId = GetUserIdFromClaim();
            var userName = await _userService.GetUserNameByUserIdAsync(senderUserId);
            // Lấy ConnectionId của người nhận tin nhắn
            var receiverConnectionId = userConnectionRequests.FirstOrDefault(x => x.userId == userId)?.ConnectionId;

            var chatMessage = new ChatMessage
            {
                SenderUserId = senderUserId,
                ReceiverUserId = userId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            await _chatService.SaveMessageAsync(chatMessage);
            // Kiểm tra xem người nhận có đang kết nối hay không
            if (receiverConnectionId != null)
            {
                // Gửi tin nhắn tới người nhận
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", userName, message);
            }
            else
            {
                // Xử lý tin nhắn khi người nhận không online
                // (ví dụ: lưu vào cơ sở dữ liệu để gửi sau)
            }
        }


        public string GetUserIdFromClaim()
        {
            ClaimsPrincipal user = _contextAccessor.HttpContext.User;
            string userId = null;
            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = userIdClaim.Value;
                }
            }
            return userId;
        }


    }
}
