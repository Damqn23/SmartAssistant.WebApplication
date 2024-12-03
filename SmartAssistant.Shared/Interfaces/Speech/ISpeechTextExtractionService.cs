using SmartAssistant.Shared.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Speech
{
    public interface ISpeechTextExtractionService
    {
        string ExtractTitle(string text);
        DateTime? ExtractDate(string text);
        double ExtractEstimatedTime(string text);
        PriorityLevel ExtractPriority(string text);
    }
}
