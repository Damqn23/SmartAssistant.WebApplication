using Chronic;
using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Services.Speech
{
    public class SpeechTextExtractionService
    {
        public string ExtractTitle(string recognizedText)
        {
            // Improved pattern to catch full date/time expressions, including "tomorrow at 2 PM", "next Sunday"
            var datePattern = @"(\b(at\s+\d{1,2}(:\d{2})?\s*(AM|PM|am|pm)?)\b|\b(tomorrow|next\s+\w+|on\s+\w+|\d{1,2}(st|nd|rd|th)?)\b)";

            // Remove the date/time-related part from the recognized text
            var title = Regex.Replace(recognizedText, datePattern, "", RegexOptions.IgnoreCase).Trim();

            // Ensure that we remove trailing single words like "PM" or "Sunday" after date/time cleanup
            var trailingWordsPattern = @"\b(AM|PM|am|pm|Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)\b";
            title = Regex.Replace(title, trailingWordsPattern, "", RegexOptions.IgnoreCase).Trim();

            // If the title ends up too short or is malformed, fallback to using the full recognized text
            if (string.IsNullOrWhiteSpace(title) || title.Split(' ').Length <= 1)
            {
                title = recognizedText;  // Fallback to the full recognized text if the title seems too short
            }

            return title;
        }
        public DateTime? ExtractDate(string recognizedText)
        {
            var parser = new Parser();

            // Preprocess text to ensure it contains recognizable time formats
            recognizedText = NormalizeTime(recognizedText);

            var parsedDate = parser.Parse(recognizedText);

            if (parsedDate != null && parsedDate.Start != null)
            {
                return parsedDate.Start.Value;
            }

            return null;
        }

        // Helper function to preprocess and normalize the time format
        private string NormalizeTime(string input)
        {
            // Regex to capture common spoken time formats like "2 PM" or "3:30 PM"
            var timePattern = @"(\d{1,2})(:\d{2})?\s*(AM|PM|am|pm)?";
            var match = Regex.Match(input, timePattern);

            if (match.Success)
            {
                // Ensure that time is formatted correctly
                string time = match.Value;

                // If no AM/PM is provided, assume PM for convenience (can be adjusted)
                if (!time.Contains("AM", StringComparison.OrdinalIgnoreCase) && !time.Contains("PM", StringComparison.OrdinalIgnoreCase))
                {
                    time += " PM"; // Assuming PM if AM/PM is not specified
                }

                // Replace the time in the input with the normalized version
                input = Regex.Replace(input, timePattern, time);
            }

            return input;
        }

        public int? ExtractEstimatedTime(string recognizedText)
        {
            // Preprocess to replace number words with digits
            recognizedText = ConvertNumberWordsToDigits(recognizedText);

            // Regex pattern to match variations like "2 hours", "two hours", "2 hrs", etc.
            var timePattern = @"(\d+)\s*(hours?|hrs?|hr?)";
            var match = Regex.Match(recognizedText, timePattern);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return 1; //Default value
        }

        // Helper method to convert written numbers to digits
        private string ConvertNumberWordsToDigits(string input)
        {
            var numberWords = new Dictionary<string, string>
            {
                 { "one", "1" },
                 { "two", "2" },
                 { "three", "3" },
                 { "four", "4" },
                 { "five", "5" },
                 { "six", "6" },
                 { "seven", "7" },
                 { "eight", "8" },
                 { "nine", "9" },
                 { "ten", "10" }
            };

            foreach (var numberWord in numberWords)
            {
                input = Regex.Replace(input, @"\b" + numberWord.Key + @"\b", numberWord.Value, RegexOptions.IgnoreCase);
            }

            return input;
        }

        public PriorityLevel ExtractPriority(string recognizedText)
        {
            if (recognizedText.Contains("high", StringComparison.OrdinalIgnoreCase))
            {
                return PriorityLevel.High;
            }
            else if (recognizedText.Contains("medium", StringComparison.OrdinalIgnoreCase))
            {
                return PriorityLevel.Medium;
            }
            else
            {
                return PriorityLevel.Low; //Default value
            }
        }
    }
}
