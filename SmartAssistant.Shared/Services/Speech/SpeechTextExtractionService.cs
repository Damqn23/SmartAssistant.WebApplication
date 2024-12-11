using Chronic;
using SmartAssistant.Shared.Interfaces.Speech;
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
            var parser = new Parser();

            var parsedDate = parser.Parse(recognizedText);

            if (parsedDate != null && parsedDate.Start.HasValue)
            {
                recognizedText = Regex.Replace(recognizedText, @"\b(tomorrow|next\s+\w+|on\s+\w+|at\s+\d{1,2}(:\d{2})?\s*(AM|PM|am|pm)?)\b", "", RegexOptions.IgnoreCase).Trim();
            }

            recognizedText = Regex.Replace(recognizedText, @"\b(afternoon|morning|evening|night|today|tonight|p\.?m\.?|a\.?m\.?)\b", "", RegexOptions.IgnoreCase).Trim();

            recognizedText = recognizedText.Trim().TrimEnd('.');

            return recognizedText;
        }

        public DateTime? ExtractDate(string recognizedText)
        {
            var parser = new Parser();

            recognizedText = NormalizeTime(recognizedText);

            var parsedDate = parser.Parse(recognizedText);

            if (parsedDate != null && parsedDate.Start != null)
            {
                return parsedDate.Start.Value;
            }

            return null;
        }

        private string NormalizeTime(string input)
        {
            input = Regex.Replace(input, @"\bafternoon\b", "3:00 PM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bmorning\b", "9:00 AM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bevening\b", "6:00 PM", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\bnight\b", "8:00 PM", RegexOptions.IgnoreCase);

            var timePattern = @"(\d{1,2})(:\d{2})?\s*(AM|PM|am|pm)?";
            var match = Regex.Match(input, timePattern);

            if (match.Success)
            {
                string time = match.Value;

                if (!time.Contains("AM", StringComparison.OrdinalIgnoreCase) && !time.Contains("PM", StringComparison.OrdinalIgnoreCase))
                {
                    time += " PM"; // Assuming PM if AM/PM is not specified
                }

                input = Regex.Replace(input, timePattern, time);
            }

            return input;
        }


        public int? ExtractEstimatedTime(string recognizedText)
        {
            recognizedText = ConvertNumberWordsToDigits(recognizedText);

            var timePattern = @"(\d+)\s*(hours?|hrs?|hr?)";
            var match = Regex.Match(recognizedText, timePattern);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return 1; //Default value
        }

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
