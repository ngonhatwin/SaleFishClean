using Contract.Interfaces;
using Contract;
using Microsoft.Extensions.DependencyInjection;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SaleFishClean.Infrastructure.Repositories;
using Contract.Common.Implement;
using Contract.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace SaleFishClean.Infrastructure
{
    public static class ConfigureServices
    {
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentNullException("Redis Connection string is not configured.");
            }
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = redisConnectionString;
            });
        }
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
                .AddScoped<IProductServices, ProductServices>()
                .AddScoped<IUserServices, UserServices>()
                .AddScoped<IJwtRepository, JwtRepository>()
                .AddScoped<ITokenRepository, TokenRepository>()
                .AddScoped<IOrderServices, OrderServices>()
                .AddScoped<IInventoryServices, InventoryServices>()
                .AddScoped<IShoppingCartServices, ShoppingCartServices>()
                .AddScoped<ICommentServices, CommentServices>()
                .AddScoped<IChatServices, ChatServices>()
                .AddSingleton<IVnPayServices, VnPayServices>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<ISerializeService, SerializeService>()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
            ;
        }
    }
}
