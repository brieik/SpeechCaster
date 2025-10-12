mergeInto(LibraryManager.library, {
  RequestMicPermission: function () {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      console.warn("[WebSpeech] getUserMedia not supported.");
      return;
    }

    navigator.mediaDevices.getUserMedia({ audio: true })
      .then(stream => {
        console.log("[WebSpeech] Microphone permission granted.");
        stream.getTracks().forEach(track => track.stop());
      })
      .catch(err => {
        console.warn("[WebSpeech] Microphone permission denied:", err);
      });
  },

  StartRecognition: function () {
    if (window.recognitionActive || window.recognitionStarting) return;

    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    const SpeechGrammarList = window.SpeechGrammarList || window.webkitSpeechGrammarList;

    if (!SpeechRecognition) {
      console.error("[WebSpeech] API not supported.");
      return;
    }

    const allWords = [
      "cat","sun","hat","dog","book","pen","fish","milk","tree","ball","cup","run","star","bird","rain","apple","leaf","moon","door",
      "planet","school","teacher","window","garden","market","forest","pencil","rocket","river","butterfly","school bag",
      "chocolate","family","elephant","picture","kitchen","monster","summer",
      "stranger","through","rhythm","architecture","consequence","temperature","extraordinary",
      "phenomenon","squirrel","environment","hypothesis","psychology","mathematics","vegetable",
      "university","literature","accommodation","transformation"
    ];

    const grammar = "#JSGF V1.0; grammar words; public <word> = " + allWords.join(" | ") + " ;";

    const recognition = new SpeechRecognition();
    recognition.lang = 'en-US';
    recognition.continuous = false;
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;

    if (SpeechGrammarList) {
      const speechRecognitionList = new SpeechGrammarList();
      speechRecognitionList.addFromString(grammar, 1);
      recognition.grammars = speechRecognitionList;
      console.log("[WebSpeech] Grammar loaded with " + allWords.length + " words.");
    }

    window.recognitionStarting = true;
    window.allowAutoRestart = true;
    window.recognitionManuallyStopped = false;

    recognition.onresult = function (event) {
      const result = event.results[0][0];
      const transcript = result.transcript.trim().toLowerCase();
      const confidence = result.confidence || 0;

      console.log(`[WebSpeech] Recognized: "${transcript}" (confidence: ${confidence.toFixed(2)})`);

      if (transcript.length > 0) {
        SendMessage('SpeechReceiver', 'OnSpeechResultWithConfidence', transcript + "|" + confidence);
      }
    };

    recognition.onerror = function (event) {
      console.warn("[WebSpeech] Error:", event.error);
      window.recognitionActive = false;
      window.recognitionStarting = false;

      // 🩵 Handle "aborted" or "no-speech" gracefully
      if (event.error === "aborted" || event.error === "no-speech" || event.error === "network") {
        console.log("[WebSpeech] Speech aborted or no input detected. Sending Try Again to Unity...");
        SendMessage('SpeechReceiver', 'OnSpeechTryAgain'); // 👈 Unity message
      }

      // Auto-restart for recoverable errors
      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        setTimeout(() => {
          console.log("[WebSpeech] Auto-restarting after error...");
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1500);
      }
    };

    recognition.onend = function () {
      console.log("[WebSpeech] Ended.");
      window.recognitionActive = false;
      window.recognitionStarting = false;

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        setTimeout(() => {
          console.log("[WebSpeech] Auto-restarting after end...");
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1500);
      }
    };

    try {
      recognition.start();
      window.recognition = recognition;
      window.recognitionActive = true;
      console.log("[WebSpeech] Started recognition.");
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
