using AutoMapper;
using SmartAssistant.Shared.Models;
using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Reminder, ReminderModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom<UserResolver>());
            CreateMap<ReminderModel, Reminder>();
            CreateMap<ReminderCreateModel, ReminderModel>();
        }
    }
}
