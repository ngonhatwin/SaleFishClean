using SaleFishClean.Application.Common.Interfaces.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Infrastructure.Services
{
    public class CommentServices : ICommentServices
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IUserServices _userService;

        public CommentServices(IConnectionMultiplexer redis, IProductServices productService, IUserServices userService)
        {
            _redis = redis;
            _userService = userService;
        }

        public async Task AddCommentAsync(int productId, string userId, string content)
        {
            var db = _redis.GetDatabase();
            var commentKey = $"Comments:{productId}";
            var comment = $"{userId}:{content}";
            await db.ListRightPushAsync(commentKey, comment);
        }

        public async Task<List<string>> GetCommentsAsync(int productId)
        {
            var db = _redis.GetDatabase();
            var commentKey = $"Comments:{productId}";
            var comments = await db.ListRangeAsync(commentKey);
            var commentList = new List<string>();
            foreach (var comment in comments)
            {
                var parts = comment.ToString().Split(':');
                var userId = parts[0];
                var content = parts[1];
                // Thực hiện lọc userName từ userId
                var userName = await _userService.GetUserNameByUserIdAsync(userId); // Đây là phần cần thay đổi để lấy userName từ userId
               // Thêm userName và content vào danh sách bình luận
                commentList.Add($"{userName}: {content}");
            }
            return commentList;
        }
    }
}
