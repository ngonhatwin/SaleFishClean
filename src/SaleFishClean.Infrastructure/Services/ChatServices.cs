using Contract.Common.Interfaces;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Infrastructure.Services
{
    public class ChatServices : IChatServices
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ISerializeService _serializeService;
        private readonly IUserServices _userServices;
        public ChatServices(IConnectionMultiplexer redis, ISerializeService serializeService, IUserServices userServices)
        {
            _redis = redis;
            _serializeService = serializeService;
            _userServices = userServices;
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            var db = _redis.GetDatabase();
            var serializedMessage = _serializeService.Serialize(message);
            await db.ListRightPushAsync($"chat:{message.SenderUserId}:{message.ReceiverUserId}", serializedMessage);
            await db.ListRightPushAsync($"chat:{message.ReceiverUserId}:{message.SenderUserId}", serializedMessage);
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(string userId, string contactUserId)
        {
            var db = _redis.GetDatabase();

            // Lấy tên người dùng từ userId và contactUserId
            var userName = await _userServices.GetUserNameByUserIdAsync(userId);
            var contactUserName = await _userServices.GetUserNameByUserIdAsync(contactUserId);

            var messages = await db.ListRangeAsync($"chat:{userId}:{contactUserId}");

            var messageDtos = new List<MessageResponse>();

            foreach (var message in messages)
            {
                var chatMessage = _serializeService.Deserialize<ChatMessage>(message);
                var senderName = chatMessage.SenderUserId == userId ? userName : contactUserName;
                messageDtos.Add(new MessageResponse
                {
                    UserName = senderName,
                    Content = chatMessage.Message,
                    Timestamp = chatMessage.Timestamp
                });
            }

            return messageDtos;
        }
    }
}
