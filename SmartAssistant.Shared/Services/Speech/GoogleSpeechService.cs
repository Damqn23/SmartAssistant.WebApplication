using Google.Cloud.Speech.V1;
using System;

namespace SmartAssistant.Shared.Services.Speech
{
    public class GoogleSpeechService
    {
        public string RecognizeSpeech(byte[] audioBytes)
        {
            try
            {
                // Create a speech client
                var speech = SpeechClient.Create();

                // Get the sample rate from the WAV file header
                int sampleRate = GetWavFileSampleRate(audioBytes);

                var response = speech.Recognize(new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = sampleRate,
                    LanguageCode = "en-US"
                }, RecognitionAudio.FromBytes(audioBytes));

                foreach (var result in response.Results)
                {
                    return result.Alternatives[0].Transcript;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Log the actual error for debugging
                return string.Empty;
            }
        }

        private int GetWavFileSampleRate(byte[] wavBytes)
        {
            try
            {
                // WAV file header has sample rate at byte offset 24 (4 bytes, little-endian)
                return BitConverter.ToInt32(wavBytes, 24);
            }
            catch (Exception)
            {
                return 16000; // Return default sample rate if unable to read from header
            }
        }
    }
}