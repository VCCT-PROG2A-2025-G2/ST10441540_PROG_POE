//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

//Reference: 
//              https://elevenlabs.io/app/speech-synthesis/text-to-speech

using System;
using System.IO;
using System.Media;
using System.Windows;

namespace ST10441540_PROG_POE
{
    //-----------------------------------------------------------------------------------//
    //Class for the ChatBot Voice Greeting
    class VoiceGreeting
    {
        //-----------------------------------------------------------------------------//
        //Method that plays the WAV greeting file and returns a message if an error occurs
        public string Greeting()
        {
            //WAV audio file path
            string audioFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recording.wav");

            //Check if the file exists before trying to play it
            if (File.Exists(audioFilePath))
            {
                //Play the file
                return PlayAudioFile(audioFilePath);
            }
            else
            {
                //Return an error message to be displayed in the WPF UI
                return "Audio file not found! Please check the file path.";
            }
        }

        //-------------------------------------------------------------------------------------//
        //Method that plays the WAV file and returns a message if an error occurs
        private string PlayAudioFile(string filePath)
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer(filePath))
                {
                    //Load the sound synchronously
                    player.Load();
                    //Play the WAV file synchronously
                    player.PlaySync();
                }
                //No error, return null
                return null;
            }
            catch (Exception ex)
            {
                //Return the error message to be displayed in the WPF UI
                return $"Error playing audio file: {ex.Message}";
            }
        }
        //--------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//
}
//---------------------------------------End of Code-------------------------------------------------//