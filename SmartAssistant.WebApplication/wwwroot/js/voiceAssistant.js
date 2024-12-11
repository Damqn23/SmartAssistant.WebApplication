document.getElementById("voiceCommandButton").addEventListener("click", function () {
    const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
    recognition.lang = 'en-US';
    recognition.start();

    recognition.onresult = function (event) {
        const spokenCommand = event.results[0][0].transcript.toLowerCase();
        processVoiceCommand(spokenCommand);
    };

    recognition.onerror = function (event) {
        console.log('Speech recognition error:', event.error);
    };
});

function processVoiceCommand(command) {
    if (command.includes("go to tasks")) {
        window.location.href = "/Task/Index";  // Navigate to Task list
    } else if (command.includes("go to events")) {
        window.location.href = "/Event/Index";  // Navigate to Event list
    } else if (command.includes("go to teams")) {
        window.location.href = "/Team/Index";  // Navigate to Team list
    } else if (command.includes("go to reminders")) {
        window.location.href = "/Reminder/Index";  // Navigate to Reminders list
    } else if (command.includes("open calendar")) {
        window.location.href = "/Calendar/Index";  // Navigate to Calendar view
    } else {
        alert("Command not recognized. Please try again.");  // Handle unknown commands
    }
}
