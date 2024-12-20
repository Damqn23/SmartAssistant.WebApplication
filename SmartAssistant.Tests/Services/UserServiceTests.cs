﻿using FluentAssertions;
using Moq;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            var userId = "user123";
            var user = new UserModel { Id = userId, UserName = "John Doe" };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(userId);

            result.Should().BeEquivalentTo(user);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "user1", UserName = "John Doe" },
                new UserModel { Id = "user2", UserName = "Jane Smith" }
            };

            _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            result.Should().BeEquivalentTo(users);
            _mockUserRepository.Verify(r => r.GetAllUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser()
        {
            var user = new UserModel { Id = "user123", UserName = "John Doe" };

            await _userService.AddUserAsync(user);

            _mockUserRepository.Verify(r => r.AddAsync(It.Is<UserModel>(u => u.Id == user.Id && u.UserName == user.UserName)), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            var user = new UserModel { Id = "user123", UserName = "John Updated" };

            await _userService.UpdateUserAsync(user);

            _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<UserModel>(u => u.Id == user.Id && u.UserName == user.UserName)), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            var userId = "user123";

            await _userService.DeleteUserAsync(userId);

            _mockUserRepository.Verify(r => r.DeleteUserAsync(userId), Times.Once);
        }
    }
}
