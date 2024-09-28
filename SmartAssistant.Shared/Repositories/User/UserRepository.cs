using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartAssistant.Shared.Interfaces.User;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApplication.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserModel> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return _mapper.Map<UserModel>(user);
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task AddUserAsync(UserModel userModel)
        {
            var user = _mapper.Map<SmartAssistant.WebApp.Data.Entities.User>(userModel);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserModel userModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userModel.Id);
            if (user != null)
            {
                _mapper.Map(userModel, user);  // Updates the user entity with the model's data
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserModel>>(users);
        }

        public async Task<UserModel> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            return _mapper.Map<UserModel>(user);
        }

        public async Task AddAsync(UserModel entity)
        {
            var user = _mapper.Map<SmartAssistant.WebApp.Data.Entities.User>(entity);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserModel entity)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);
            if (user != null)
            {
                _mapper.Map(entity, user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(UserModel entity)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> GetUserByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return _mapper.Map<UserModel>(user);
        }

    }
}
