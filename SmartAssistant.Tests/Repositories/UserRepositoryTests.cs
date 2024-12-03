using AutoMapper;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Repositories.User;
using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Repositories
{
    public class UserRepositoryTests : RepositoryTestBase
    {
        private readonly IMapper _mapper;

        public UserRepositoryTests()
        {
            // Initialize AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, User>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async System.Threading.Tasks.Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);
            var user = new UserModel { Id = "user1", UserName = "TestUser" };

            // Act
            await repository.AddAsync(user);

            // Assert
            var addedUser = context.Users.FirstOrDefault(u => u.Id == "user1");
            Assert.NotNull(addedUser);
            Assert.Equal("TestUser", addedUser.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);

            // Add a user with an ID that matches the numeric string expectation
            var user = new SmartAssistant.WebApp.Data.Entities.User
            {
                Id = "1", // Use "1" as string to match the int-to-string conversion in GetByIdAsync
                UserName = "TestUser"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1); // Pass int ID; it will be converted to string internally

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);
            context.Users.AddRange(
                new User { Id = "user1", UserName = "User1" },
                new User { Id = "user2", UserName = "User2" });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);
            var user = new User { Id = "user1", UserName = "OldUserName" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var updatedUserModel = new UserModel { Id = "user1", UserName = "UpdatedUserName" };

            // Act
            await repository.UpdateAsync(updatedUserModel);

            // Assert
            var updatedUser = context.Users.FirstOrDefault(u => u.Id == "user1");
            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedUserName", updatedUser.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);
            var user = new User { Id = "user1", UserName = "TestUser" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userModel = _mapper.Map<UserModel>(user);

            // Act
            await repository.DeleteAsync(userModel);

            // Assert
            var deletedUser = context.Users.FirstOrDefault(u => u.Id == "user1");
            Assert.Null(deletedUser);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserByUserNameAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context, _mapper);
            var user = new User { Id = "user1", UserName = "TestUser" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetUserByUserNameAsync("TestUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.UserName);
        }
    }
}
