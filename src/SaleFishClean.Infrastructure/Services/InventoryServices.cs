using Arch.EntityFrameworkCore.UnitOfWork;
using Contract;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure.Data;


namespace SaleFishClean.Infrastructure.Services
{
    public class InventoryServices : RepositoryBaseAsync<SaleFishProjectContext, Inventory, int>,  IInventoryServices
    {
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;

        public InventoryServices(IUnitOfWork<SaleFishProjectContext> unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateByModelsAsync(Inventory inventory)
        {
            var Inventory = new Inventory
            {
                ProductId = inventory.ProductId,
                EntryDate = DateTime.Now,
                Quantity = inventory.Quantity,
            };
            await _unitOfWork.DbContext.Inventories.AddAsync(Inventory);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();  
        }
    }
}
