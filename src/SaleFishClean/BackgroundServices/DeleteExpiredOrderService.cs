using Microsoft.EntityFrameworkCore;
using SaleFishClean.Infrastructure.Data;

namespace SaleFishClean.Web.BackgroundServices
{
    public class DeleteExpiredOrderService : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromHours(10);
        private readonly IServiceProvider _serviceProvider;

        public DeleteExpiredOrderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(_period);

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SaleFishProjectContext>();

                var orderBele = await context.Orders
                    .Where(c => (c.Status == null || c.Status == 0) && c.OrderDate <= DateTime.Now.AddMinutes(15))
                    .ToListAsync(stoppingToken);

                var ids = orderBele.Select(o => o.OrderId);
                var orderDetail = context.OrderDetails.Where(od => ids.Contains(od.OrderId));

                context.OrderDetails.RemoveRange(orderDetail);
                context.Orders.RemoveRange(orderBele);

                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
