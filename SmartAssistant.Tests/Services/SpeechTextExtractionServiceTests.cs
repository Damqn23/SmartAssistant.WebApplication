using FluentAssertions;
using SmartAssistant.Shared.Models.Task;
using SmartAssistant.Shared.Services.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class SpeechTextExtractionServiceTests
    {
        private readonly SpeechTextExtractionService _service;

        public SpeechTextExtractionServiceTests()
        {
            _service = new SpeechTextExtractionService();
        }

        [Fact]
        public void ExtractTitle_ShouldReturnTextWithoutDateTime()
        {
            // Arrange
            var input = "Meeting with team tomorrow at 3 PM";
            var expected = "Meeting with team";

            // Act
            var result = _service.ExtractTitle(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ExtractDate_ShouldReturnParsedDateTime()
        {
            // Arrange
            var input = "Meeting tomorrow at 3 PM";
            var expectedDate = DateTime.Now.Date.AddDays(1).AddHours(15); // Assuming tomorrow at 3 PM

            // Act
            var result = _service.ExtractDate(input);

            // Assert
            result.Should().HaveValue();
            result.Value.Should().BeCloseTo(expectedDate, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void ExtractDate_ShouldReturnNull_WhenNoDateTimeFound()
        {
            // Arrange
            var input = "Just a random sentence without a date";

            // Act
            var result = _service.ExtractDate(input);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractEstimatedTime_ShouldReturnParsedTime()
        {
            // Arrange
            var input = "The task should take about two hours to complete";
            var expected = 2;

            // Act
            var result = _service.ExtractEstimatedTime(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ExtractEstimatedTime_ShouldReturnDefault_WhenNoTimeFound()
        {
            // Arrange
            var input = "This task should be done";

            // Act
            var result = _service.ExtractEstimatedTime(input);

            // Assert
            result.Should().Be(1); // Default value
        }

        [Fact]
        public void ExtractPriority_ShouldReturnHigh_WhenTextContainsHigh()
        {
            // Arrange
            var input = "This task has a high priority";

            // Act
            var result = _service.ExtractPriority(input);

            // Assert
            result.Should().Be(PriorityLevel.High);
        }

        [Fact]
        public void ExtractPriority_ShouldReturnMedium_WhenTextContainsMedium()
        {
            // Arrange
            var input = "This task is of medium priority";

            // Act
            var result = _service.ExtractPriority(input);

            // Assert
            result.Should().Be(PriorityLevel.Medium);
        }

        [Fact]
        public void ExtractPriority_ShouldReturnLow_WhenNoPriorityIsSpecified()
        {
            // Arrange
            var input = "This task does not specify a priority";

            // Act
            var result = _service.ExtractPriority(input);

            // Assert
            result.Should().Be(PriorityLevel.Low); // Default value
        }
    }
}
