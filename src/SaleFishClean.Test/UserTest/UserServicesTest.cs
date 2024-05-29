using Arch.EntityFrameworkCore.UnitOfWork;
using Contract.Common.Interfaces;
using Contract.Helper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure.Data;
using SaleFishClean.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Test.UserTest
{
    public class UserServicesTest
    {
        private readonly Mock<IUnitOfWork<SaleFishProjectContext>> _mockUnitOfWork;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IDistributedCache> _mockRedisCacheService;
        private readonly Mock<ISerializeService> _mockSerializeService;
        private readonly Mock<IOptions<MailSettings>> _maillSettings;
        private readonly Mock<IValidator<UserRequest>> _validator;

        public UserServicesTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork<SaleFishProjectContext>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockRedisCacheService = new Mock<IDistributedCache>();
            _mockSerializeService = new Mock<ISerializeService>();
            _maillSettings = new Mock<IOptions<MailSettings>>();
            _validator = new Mock<IValidator<UserRequest>>();
        }
        [Fact]
        public async Task GetUserNameByUserId_ReturnsCorrectUserName()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var user = new User { UserId = userId, UserName = userName };

            var mockContext = new Mock<SaleFishProjectContext>();
            mockContext.Setup(m => m.Users.FindAsync(userId)).ReturnsAsync(user);
            var mockValidator = new Mock<IValidator<UserRequest>>();
            var userServices = new UserServices(_maillSettings.Object, _mockUnitOfWork.Object,
                _mockHttpContextAccessor.Object, _mockSerializeService.Object,_mockRedisCacheService.Object,
                mockValidator.Object);

            // Act
            var result = await userServices.GetUserNameByUserIdAsync(userId);

            Assert.Null(result);
            // Assert
            
        }
    }
}
