mergeInto(LibraryManager.library, {
  RequestMicPermission: function () {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      console.warn("[WebSpeech] getUserMedia not supported.");
      return;
    }

    navigator.mediaDevices.getUserMedia({ audio: true })
      .then(function (stream) {
        console.log("[WebSpeech] Microphone permission granted.");
        stream.getTracks().forEach(track => track.stop()); // Clean up
      })
      .catch(function (err) {
        console.warn("[WebSpeech] Microphone permission denied:", err);
      });
  },

  StartRecognition: function () {
    if (window.recognitionActive || window.recognitionStarting) return;

    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!SpeechRecognition) {
      console.error("[WebSpeech] API not supported.");
      return;
    }

    const recognition = new SpeechRecognition();
    recognition.lang = 'en-US';
    recognition.continuous = false;
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;

    window.recognitionStarting = true;
    window.allowAutoRestart = false;

    recognition.onresult = function (event) {
      const transcript = event.results[0][0].transcript.trim();
      console.log("[WebSpeech] Recognized:", transcript);
      if (transcript.length > 0) {
        SendMessage('SpeechReceiver', 'OnSpeechResult', transcript);
      }
    };

    recognition.onerror = function (event) {
      console.warn("[WebSpeech] Error:", event.error);
      window.recognitionActive = false;
      window.recognitionStarting = false;

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        console.log("[WebSpeech] Retrying after error...");
        setTimeout(() => {
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1000);
      }
    };

    recognition.onend = function () {
      console.log("[WebSpeech] Ended.");
      window.recognitionActive = false;
      window.recognitionStarting = false;

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        console.log("[WebSpeech] Auto-restarting...");
        setTimeout(() => {
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1000);
      }
    };

    try {
      recognition.start();
      window.recognition = recognition;
      window.recognitionActive = true;
      window.recognitionManuallyStopped = false;
      console.log("[WebSpeech] Started.");
    } catch (e) {
      console.error("[WebSpeech] Start failed:", e);
      window.recognitionStarting = false;
    }
  },

  StopRecognition: function () {
    if (window.recognition) {
      try {
        window.recognitionManuallyStopped = true;
        window.allowAutoRestart = false;
        window.recognition.stop();
        console.log("[WebSpeech] Stopping...");
      } catch (e) {
        console.warn("[WebSpeech] Stop error:", e);
      }
      window.recognitionActive = false;
      window.recognitionStarting = false;
    }
  }
});
