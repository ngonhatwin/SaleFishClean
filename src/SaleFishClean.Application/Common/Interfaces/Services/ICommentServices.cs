using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface ICommentServices
    {
        Task AddCommentAsync(int productId, string userId, string content);
        Task<List<string>> GetCommentsAsync(int productId);
    }
}
