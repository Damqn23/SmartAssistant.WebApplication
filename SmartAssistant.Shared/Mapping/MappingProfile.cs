using AutoMapper;
using SmartAssistant.Shared.Models;
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
            CreateMap<ReminderModel, ReminderModel>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
