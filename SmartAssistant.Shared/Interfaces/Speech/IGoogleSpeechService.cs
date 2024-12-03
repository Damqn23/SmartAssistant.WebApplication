using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Shared.Interfaces.Speech
{
    public interface IGoogleSpeechService
    {
        string RecognizeSpeech(byte[] audioBytes);
    }
}
