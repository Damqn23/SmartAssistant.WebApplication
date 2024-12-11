using AutoMapper;
using SmartAssistant.Shared.Data.Entities;
using SmartAssistant.Shared.Models;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApp.Data.Entities;
using SmartAssistant.WebApplication.Data.Entities;
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
            CreateMap<ReminderModel, Reminder>()
              .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ReminderCreateModel, ReminderModel>();

            CreateMap<ReminderModel, ReminderEditModel>();
            CreateMap<ReminderEditModel, ReminderModel>()
             .ForMember(dest => dest.UserId, opt => opt.Ignore());


            CreateMap<ReminderModel, ReminderDeleteModel>();

            CreateMap<UserModel, User>();

            CreateMap<SmartAssistant.WebApp.Data.Entities.Task, TaskModel>();
            CreateMap<TaskModel, SmartAssistant.WebApp.Data.Entities.Task>();

            CreateMap<TaskCreateModel, TaskModel>();
            CreateMap<TaskModel, TaskEditModel>();
            CreateMap<TaskEditModel, TaskModel>();
            CreateMap<TaskModel, TaskDeleteModel>();

            CreateMap<Event, EventModel>();
            CreateMap<EventModel, Event>();

            CreateMap<EventCreateModel, EventModel>();
            CreateMap<EventEditModel, EventModel>().ForMember(dest => dest.UserId, opt => opt.Ignore());
            CreateMap<EventModel, EventEditModel>();

            CreateMap<Team, TeamModel>()
    .ForMember(dest => dest.OwnerUserName, opt => opt.MapFrom(src => src.Owner.UserName));  // Ensure Owner's UserName is mapped





            CreateMap<TeamModel, Team>();
            CreateMap<TeamCreateModel, Team>();

            CreateMap<User, UserModel>().ReverseMap();

            CreateMap<UserTeam, UserTeamModel>().ReverseMap();

            

        }
    }
}
