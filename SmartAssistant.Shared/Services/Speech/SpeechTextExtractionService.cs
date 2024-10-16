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
            // Initialize Chronic.NET parser
            var parser = new Parser();

            // Parse the recognized text for date/time
            var parsedDate = parser.Parse(recognizedText);

            // If a date is found, remove the recognized date/time portion
            if (parsedDate != null && parsedDate.Start.HasValue)
            {
                // Remove recognized time-related words from the original text manually
                recognizedText = Regex.Replace(recognizedText, @"\b(tomorrow|next\s+\w+|on\s+\w+|at\s+\d{1,2}(:\d{2})?\s*(AM|PM|am|pm)?)\b", "", RegexOptions.IgnoreCase).Trim();
            }

            // Further remove any time-related parts like "afternoon", "morning", "evening", "p.m."
            recognizedText = Regex.Replace(recognizedText, @"\b(afternoon|morning|evening|night|today|tonight|p\.?m\.?|a\.?m\.?)\b", "", RegexOptions.IgnoreCase).Trim();

            recognizedText = recognizedText.Trim().TrimEnd('.');

            return recognizedText;
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
        // Helper function to preprocess and normalize the time format
        private string NormalizeTime(string input)
        {
            // Normalize phrases like "afternoon" to specific times
            input = Regex.Replace(input, @"\bafternoon\b", "3:00 PM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bmorning\b", "9:00 AM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bevening\b", "6:00 PM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bnight\b", "8:00 PM", RegexOptions.IgnoreCase);

            // Regex to capture common spoken time formats like "2 PM" or "3:30 PM"
            var timePattern = @"(\d{1,2})(:\d{2})?\s*(AM|PM|am|pm)?";
            var match = Regex.Match(input, timePattern);

            if (match.Success)
            {
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
        { "ten", "10" },
        { "eleven", "11" },
        { "twelve", "12" }
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
