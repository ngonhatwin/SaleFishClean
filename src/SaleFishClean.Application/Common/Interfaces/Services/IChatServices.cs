using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IChatServices
    {
        Task SaveMessageAsync(ChatMessage message);
        Task<List<MessageResponse>> GetMessagesAsync(string userId, string contactUserId);
    }
}
