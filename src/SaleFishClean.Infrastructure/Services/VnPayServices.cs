using Contract.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;

namespace SaleFishClean.Infrastructure.Services
{
    public class VnPayServices : IVnPayServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VnPayServices> _logger;
        public VnPayServices(IConfiguration configuration, ILogger<VnPayServices> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequest request)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var _vnpay = new VnPayLibrary();
            _vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            _vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
            _vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            _vnpay.AddRequestData("vnp_Amount", ((long)request.TotalAmount * 100).ToString());
            _vnpay.AddRequestData("vnp_CreateDate", request.CreatedDate.ToString("yyyyMMddHHmmss"));
            _vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            _vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            _vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locate"]);
            _vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng" + request.OrderId.ToString());
            _vnpay.AddRequestData("vnp_OrderType", "other"); //default
            _vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]);
            _vnpay.AddRequestData("vnp_TxnRef", request.OrderId.ToString());

            var PaymentUrl = _vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);
            _logger.LogWarning(PaymentUrl);
            return PaymentUrl;
        }

        public VnPaymentResponse PaymentExecute(IQueryCollection collections)
        {
            var _vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrWhiteSpace(key) && key.StartsWith("vnp_"))
                {
                    _vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_OrderId = long.Parse(_vnpay.GetResponseData("vnp_TxnRef"));
            var TransactionOrderID = Convert.ToInt64(_vnpay.GetResponseData("vnp_TransactionNo"));

            var vnp_SecureHash = _vnpay.GetResponseData("vnp_SecureHash");
            var vnp_ResponseCode = _vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = _vnpay.GetResponseData("vnp_OrderInfo");
            bool checkSignature = _vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);

            if (!checkSignature)
            {
                return new VnPaymentResponse
                {
                    Success = false
                };
            }
            return new VnPaymentResponse
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_OrderId.ToString(),
                TransactionId = TransactionOrderID.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
            };
        }
    }
}
