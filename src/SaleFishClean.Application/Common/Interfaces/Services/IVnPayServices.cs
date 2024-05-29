using Microsoft.AspNetCore.Http;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;

namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequest request);
        VnPaymentResponse PaymentExecute(IQueryCollection collections);
    }
}
