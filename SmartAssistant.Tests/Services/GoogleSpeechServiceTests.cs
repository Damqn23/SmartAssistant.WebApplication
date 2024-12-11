using Google.Cloud.Speech.V1;
using Moq;
using SmartAssistant.Shared.Services.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAssistant.Tests.Services
{
    public class GoogleSpeechServiceTests
    {
        private readonly Mock<SpeechClient> _mockSpeechClient;
        private readonly GoogleSpeechService _googleSpeechService;

        public GoogleSpeechServiceTests()
        {
            _mockSpeechClient = new Mock<SpeechClient>();
            _googleSpeechService = new GoogleSpeechService(_mockSpeechClient.Object);
        }

        [Fact]
        public void RecognizeSpeech_ShouldReturnTranscript_WhenAudioIsRecognized()
        {
            var audioBytes = new byte[100]; // Mock audio bytes
            var mockResponse = new RecognizeResponse
            {
                Results =
                {
                    new SpeechRecognitionResult
                    {
                        Alternatives =
                        {
                            new SpeechRecognitionAlternative { Transcript = "Hello World" }
                        }
                    }
                }
            };

            _mockSpeechClient
                .Setup(client => client.Recognize(
                    It.IsAny<RecognitionConfig>(),
                    It.IsAny<RecognitionAudio>(),
                    null)) // Optional argument explicitly set
                .Returns(mockResponse);

            var result = _googleSpeechService.RecognizeSpeech(audioBytes);

            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void RecognizeSpeech_ShouldReturnEmptyString_WhenNoResults()
        {
            var audioBytes = new byte[100]; // Mock audio bytes
            var mockResponse = new RecognizeResponse();

            _mockSpeechClient
                .Setup(client => client.Recognize(
                    It.IsAny<RecognitionConfig>(),
                    It.IsAny<RecognitionAudio>(),
                    null))
                .Returns(mockResponse);

            var result = _googleSpeechService.RecognizeSpeech(audioBytes);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetWavFileSampleRate_ShouldReturnDefaultSampleRate_WhenInvalidWavBytes()
        {
            var invalidWavBytes = new byte[10]; // Invalid WAV header

            var result = _googleSpeechService.GetWavFileSampleRate(invalidWavBytes);

            Assert.Equal(16000, result); // Default sample rate
        }
    }
}
