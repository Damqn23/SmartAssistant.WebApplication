using AutoMapper;
using SmartAssistant.Shared.Mapping;
using SmartAssistant.Shared.Models.Event;
using SmartAssistant.Shared.Models.Reminder;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Models.Team;
using SmartAssistant.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests
{
    public class MappingProfileTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            _configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void Team_To_TeamModel_ShouldMapCorrectly()
        {
            var team = new Team
            {
                Id = 2,
                TeamName = "Development Team",
                OwnerId = "owner123",
                Owner = new User { Id = "owner123", UserName = "OwnerName" }
            };

            var result = _mapper.Map<TeamModel>(team);

            Assert.NotNull(result);
            Assert.Equal(team.Id, result.Id);
            Assert.Equal(team.TeamName, result.TeamName);
            Assert.Equal(team.OwnerId, result.OwnerId);
            Assert.Equal(team.Owner.UserName, result.OwnerUserName);
        }

        [Fact]
        public void TaskModel_To_TaskEntity_ShouldMapCorrectly()
        {
            var taskModel = new TaskModel
            {
                Id = 3,
                Description = "Test Task",
                DueDate = DateTime.Now.AddDays(1),
                UserId = "user123"
            };

            var result = _mapper.Map<SmartAssistant.WebApp.Data.Entities.Task>(taskModel);

            Assert.NotNull(result);
            Assert.Equal(taskModel.Id, result.Id);
            Assert.Equal(taskModel.Description, result.Description);
            Assert.Equal(taskModel.DueDate, result.DueDate);
            Assert.Equal(taskModel.UserId, result.UserId);
        }

        [Fact]
        public void Event_To_EventModel_ShouldMapCorrectly()
        {
            var eventEntity = new Event
            {
                Id = 4,
                EventTitle = "Meeting",
                EventDate = DateTime.Now.AddHours(2),
                UserId = "user456"
            };

            var result = _mapper.Map<EventModel>(eventEntity);

            Assert.NotNull(result);
            Assert.Equal(eventEntity.Id, result.Id);
            Assert.Equal(eventEntity.EventTitle, result.EventTitle);
            Assert.Equal(eventEntity.EventDate, result.EventDate);
            Assert.Equal(eventEntity.UserId, result.UserId);
        }
    }
}
