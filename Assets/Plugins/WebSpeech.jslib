mergeInto(LibraryManager.library, {
  wordList: [],

  SetWordDifficulty: function(levelPtr) {
    var level = UTF8ToString(levelPtr);
    Module.wordList = [];

    if (level === "easy") {
      Module.wordList = [
        "fan","food","fish","fairy","very","vote","van",
        "this","that","then","thin","three","zip",
        "zoo","buzz","rice","sun","moon","king","queen","dragon"
      ];
    } else if (level === "medium") {
      Module.wordList = [
        "fantasy","festival","fortress","fearful","forever","victory",
        "villain","voyage","vanish","velvet","thunder","thousand",
        "brother","mother","gather","another","puzzle","blizzard",
        "frozen","horizon","amazing","discover","adventure","wizard","lantern"
      ];
    } else if (level === "hard") {
      Module.wordList = [
        "responsibility","pronunciation","opportunity","vocabulary",
        "unbelievable","transformation","determination","extraordinary",
        "electricity","imagination","communication","information",
        "celebration","investigation","civilization","federation",
        "verification","visualization","adventurous","victorious",
        "perseverance","bewilderment","appreciation","exaggeration","manifestation"
      ];
    }

    console.log("[WebSpeech] Word difficulty set:", level, "(", Module.wordList.length, "words )");
  },

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
    if (window.recognitionActive || window.recognitionStarting) {
      console.log("[WebSpeech] StartRecognition ignored: already active/starting.");
      return;
    }

    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    const SpeechGrammarList = window.SpeechGrammarList || window.webkitSpeechGrammarList;

    if (!SpeechRecognition) {
      console.error("[WebSpeech] API not supported.");
      return;
    }

    const grammar = "#JSGF V1.0; grammar words; public <word> = " + (Module.wordList.join(" | ") || "") + " ;";
    const recognition = new SpeechRecognition();

    recognition.lang = 'en-US';
    recognition.continuous = false;
    recognition.interimResults = false;
    recognition.maxAlternatives = 1;

    if (SpeechGrammarList && Module.wordList.length > 0) {
      const list = new SpeechGrammarList();
      list.addFromString(grammar, 1);
      recognition.grammars = list;
    }

    // Flags for push-to-talk
    window.recognitionStarting = true;
    window.recognitionActive = false;
    window.recognitionManuallyStopped = false; // user pressed button
    window.allowAutoRestart = false;           // disable auto-restart by default

    recognition.onresult = function (event) {
      try {
        const r = event.results[0][0];
        const transcript = (r.transcript || "").trim().toLowerCase();
        const confidence = r.confidence || 0;
        console.log(`[WebSpeech] Recognized: "${transcript}" (confidence: ${confidence.toFixed(2)})`);
        if (transcript.length > 0) {
          SendMessage('SpeechReceiver', 'OnSpeechResultWithConfidence', transcript + "|" + confidence);
        }
      } catch (e) {
        console.warn("[WebSpeech] onresult handler error:", e);
      }
    };

    recognition.onerror = function (event) {
      console.warn("[WebSpeech] Error:", event && event.error);
      window.recognitionActive = false;
      window.recognitionStarting = false;

      const err = event && event.error ? event.error : "";

      if (err === "aborted" || err === "no-speech" || err === "network") {
        SendMessage('SpeechReceiver', 'OnSpeechTryAgain');
      }

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        setTimeout(() => {
          console.log("[WebSpeech] Auto-restarting after error...");
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1000);
      }
    };

    recognition.onend = function () {
      console.log("[WebSpeech] Recognition ended.");
      window.recognitionActive = false;
      window.recognitionStarting = false;

      if (!window.recognitionManuallyStopped && window.allowAutoRestart) {
        setTimeout(() => {
          console.log("[WebSpeech] Auto-restarting after end...");
          SendMessage('SpeechReceiver', 'RetryRecognition');
        }, 1000);
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
      window.recognitionManuallyStopped = true;
      window.allowAutoRestart = false;

      try {
        window.recognition.stop();
        console.log("[WebSpeech] StopRecognition called.");
      } catch (e) {
        console.warn("[WebSpeech] Stop error:", e);
      }

      window.recognitionActive = false;
      window.recognitionStarting = false;
    } else {
      window.recognitionManuallyStopped = true;
      window.allowAutoRestart = false;
      window.recognitionActive = false;
      window.recognitionStarting = false;
      console.log("[WebSpeech] StopRecognition called but no recognition object present.");
    }
  }
});
