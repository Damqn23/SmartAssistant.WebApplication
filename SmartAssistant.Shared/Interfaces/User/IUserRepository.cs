using SmartAssistant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.User
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserModel> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        System.Threading.Tasks.Task DeleteUserAsync(string userId);
        Task<UserModel> GetUserByUserNameAsync(string userName);

    }
}
