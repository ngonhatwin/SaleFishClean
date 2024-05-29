using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SaleFishClean.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) =>
            services.AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    }
}
