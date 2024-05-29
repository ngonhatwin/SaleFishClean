using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Infrastructure.Services;

namespace SaleFishClean.Web.Controllers.WebApp
{
    public class CommentController : Controller
    {
        private readonly ICommentServices _commentService;
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly IProductServices _productService;
        public CommentController(ICommentServices commentService, IHubContext<CommentHub> hubContext, IProductServices productService)
        {
            _commentService = commentService;
            _hubContext = hubContext;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment(string userId, string content, int id)
        {
            // Assume you need to pass productId here
            var productId = id; // Replace this with the actual productId
            var productName = await _productService.GetProductNameByProductIdAsync(productId);
            var message = $"Có bình luận mới tại sản phẩm {productName}: {content}";
            await _commentService.AddCommentAsync(productId, userId, content);
            await _hubContext.Clients.All.SendAsync("ReceiveComment", message); // Gửi thông báo qua SignalR
            return Ok();
        }

        [HttpGet("get-comment")]
        public async Task<IActionResult> GetComments(int productId)
        {
            var comments = await _commentService.GetCommentsAsync(productId);
            return Json(comments);
        }
    }
}
