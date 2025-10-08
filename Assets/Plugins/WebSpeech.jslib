mergeInto(LibraryManager.library, {
  RequestMicPermission: function () {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      console.warn("[WebSpeech] getUserMedia not supported.");
      return;
    }

    navigator.mediaDevices.getUserMedia({ audio: true })
      .then(function (stream) {
        console.log("[WebSpeech] Microphone permission granted.");
        stream.getTracks().forEach(track => track.stop());
      })
      .catch(function (err) {
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

    // ✅ COMBINED GRAMMAR LIST — all your words from WordLists.cs
    const allWords = [
      // Easy words
      "cat","sun","hat","dog","book","pen","fish","milk","tree","ball","cup","run","star","bird","rain","apple","leaf","moon","door",
      // Medium words
      "planet","school","teacher","window","garden","market","forest","pencil","rocket","river","butterfly","schoolbag",
      "chocolate","family","elephant","picture","kitchen","monster","summer",
      // Hard words
      "stranger","through","rhythm","architecture","consequence","temperature","extraordinary",
      "phenomenon","squirrel","environment","hypothesis","psychology","mathematics","vegetable",
      "university","literature","accommodation","transformation"
    ];

    // Create grammar string
    const grammar = "#JSGF V1.0; grammar words; public <word> = " + allWords.join(" | ") + " ;";

    const recognition = new SpeechRecognition();
    recognition.lang = 'en-US';
    recognition.continuous = false;
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;

    // 🔹 Attach the grammar
    if (SpeechGrammarList) {
      const speechRecognitionList = new SpeechGrammarList();
      speechRecognitionList.addFromString(grammar, 1);
      recognition.grammars = speechRecognitionList;
      console.log("[WebSpeech] Grammar loaded with " + allWords.length + " words.");
    }

    window.recognitionStarting = true;
    window.allowAutoRestart = false;

    recognition.onresult = function (event) {
      const transcript = event.results[0][0].transcript.trim().toLowerCase();
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
        setTimeout(() => SendMessage('SpeechReceiver', 'RetryRecognition'), 1000);
      }
    };

    recognition.onend = function () {
      console.log("[WebSpeech] Ended.");
      window.recognitionActive = false;
      window.recognitionStarting = false;

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        setTimeout(() => SendMessage('SpeechReceiver', 'RetryRecognition'), 1000);
      }
    };

    try {
      recognition.start();
      window.recognition = recognition;
      window.recognitionActive = true;
      window.recognitionManuallyStopped = false;
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
