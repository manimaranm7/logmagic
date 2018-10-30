using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace LogMagic.Microsoft.Windows.Speech
{
   class WindowsSpeechLogWriter : ILogWriter
   {
      private readonly SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
      private readonly string _whatToSayOnError;
      private readonly bool _speakMessage;

      public WindowsSpeechLogWriter(string whatToSayOnError, bool speakMessage)
      {
         _speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
         _speechSynthesizer.SetOutputToDefaultAudioDevice();
         _whatToSayOnError = whatToSayOnError;
         _speakMessage = speakMessage;
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         foreach(LogEvent e in events)
         {
            if(e.ErrorException != null)
            {
               if (!string.IsNullOrEmpty(_whatToSayOnError))
                  _speechSynthesizer.Speak(_whatToSayOnError);

               if(_speakMessage)
                  _speechSynthesizer.Speak(e.ErrorException.Message);
            }
         }
      }

      public void Dispose()
      {

      }
   }
}
