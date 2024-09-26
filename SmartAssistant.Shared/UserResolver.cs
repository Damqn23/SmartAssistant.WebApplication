using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared
{
    public class UserResolver : IValueResolver<Reminder, ReminderModel, UserModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserModel Resolve(Reminder source, ReminderModel destination, UserModel destMember, ResolutionContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                // Handle non-HTTP or unauthenticated cases (e.g., return null or some default user)
                return null;
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                return new UserModel
                {
                    UserId = userId,
                    // Populate other user properties as needed
                };
            }

            return null;
        }

    }
}
