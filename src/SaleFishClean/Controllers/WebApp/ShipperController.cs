using Microsoft.AspNetCore.Mvc;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Infrastructure.Services;

namespace SaleFishClean.Web.Controllers.WebApp
{
    public class ShipperController : Controller
    {
        private readonly IOrderServices _orderServices;
        public ShipperController(IOrderServices orderService_)
        {
            _orderServices = orderService_;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            var order = await _orderServices.GetOrderForUser(id);
            if (order == null)
            {
                return NotFound();
            }
            return Json(order);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderForShipper()
        {
            var result = await _orderServices.GetOrderForShipper();
            return View("Index", result);
        }
        [HttpGet("GetOrderShippingForShipper")]
        public async Task<IActionResult> GetOrderShippingForShipper()
        {
            var result = await _orderServices.GetOrderShippingForShipper();
            return View("Shipping", result);
        }
        [HttpGet("GetOrderShippedForShipper")]
        public async Task<IActionResult> GetOrderShippedForShipper()
        {
            var result = await _orderServices.GetOrderShippedForShipper();
            return View("Shipped", result);
        }
        [HttpGet("GetOrderDestroyForShipper")]
        public async Task<IActionResult> GetOrderDestroyForShipper()
        {
            var result = await _orderServices.GetOrderDestroyForShipper();
            return View("Destroy", result);
        }
        [HttpGet]
        public async Task<IActionResult> ChangeModeShipping( string orderId)
        {
            await _orderServices.ChangeShipping(orderId);
            return NoContent();
        }
        //[HttpGet("ChangeModeShipped")]
        public async Task<IActionResult> ChangeModeShipped( string orderId)
        {
            await _orderServices.ChangeShipped(orderId);
            return NoContent();
        }
        //[HttpGet("ChangeModeDestroy")]
        public async Task<IActionResult> ChangeModeDestroy( string orderId)
        {
            await _orderServices.ChangeDestroy(orderId);
            return NoContent();
        }
    }
}
