using SmartAssistant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.User
{
    public interface IUserService
    {
        Task<UserModel> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        System.Threading.Tasks.Task AddUserAsync(UserModel user);
        System.Threading.Tasks.Task UpdateUserAsync(UserModel user);
        System.Threading.Tasks.Task DeleteUserAsync(string userId);
    }
}
