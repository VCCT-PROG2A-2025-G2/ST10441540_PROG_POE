//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

//References:
//                  https://www.asciiart.eu/image-to-ascii
//                  https://patorjk.com/software/taag/#p=display&f=Star%20Wars&t=Welcome

using System;
using System.IO;

namespace ST104441540_PROG_POE
{
    //---------------------------------------------------------------------------//
    class ASCIIArt
    {
        //------------------------------------------------------------------------------//
        //Method for the ChatBot logo
        //Makes use of ASCII Art and returns it as a string for WPF display
        public string Logo(string filePath)
        {
            try
            {
                string result = "";
                //Check if the ASCII Art file exists
                if (File.Exists(filePath))
                {
                    //Read the ASCII art from the file
                    string asciiArt = File.ReadAllText(filePath);
                    result += asciiArt + "\n\n";

                    //Add the heading
                    result += "                   CYBERSECURITY AWARENESS BOT                                 \n\n\n";

                    //Add the welcome message and additional ASCII art
                    result += "Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.\n";
                    result += @"
                     __        __   _                                     
                     \ \      / /__| | ___ ___  _ __ ___   ___   
                      \ \ /\ / / _ \ |/ __/ _ \| '_ ` _ \ / _ \  
                       \ V  V /  __/ | (_| (_) | | | | | |  __/  
                        \_/\_/ \___|_|\___\___/|_| |_| |_|\___|    
                    " + "\n\n";
                    result += "********************************************************************************** \n\n";

                    return result;
                }
                else
                {
                    //Displaying error message
                    return "ASCII logo file not found.\n";
                }
            }
            catch (Exception ex)
            {
                //Display error message
                return "Error reading ASCII logo: " + ex.Message + "\n";
            }
        }
        //--------------------------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//
}
//---------------------------------------End of Code--------------------------------------------------//