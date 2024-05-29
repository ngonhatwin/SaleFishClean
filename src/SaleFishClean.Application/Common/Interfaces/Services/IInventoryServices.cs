using Contract.Interfaces;
using SaleFishClean.Domains.Entities;

namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IInventoryServices : IRepositoryBaseAsync<Inventory, int>
    {
        Task CreateByModelsAsync(Inventory inventory);
    }
}
