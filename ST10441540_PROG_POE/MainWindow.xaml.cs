//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;


using System.Printing;

using ST10441540_PROG_POE;
using ST104441540_PROG_POE;

namespace ST10441540_PROG_POE
{
    //----------------------------------------------------------------------------------------------------------------//
    //A window class serving as the main interface for the Cybersecurity Awareness Bot
    public partial class MainWindow : Window
    {
        //Declaration
        //Dependencies for user interaction, voice greeting, ASCII art
        private readonly UserInteraction userInteraction;
        private readonly VoiceGreeting voiceGreeting;
        private readonly ASCIIArt asciiArt;
        //List to store chat messages for display in the UI
        private readonly List<CybersecurityQuiz.ChatMessage> chatMessages;
        //Memory object for managing tasks
        private readonly ChatbotMemory memory;
        //Flag to track if the chatbot is waiting for the user's name
        private bool isWaitingForName;

        //Constructor
        public MainWindow()
        {
            //Initialize the window's UI components 
            InitializeComponent();
            //Initialize the chat messages list
            chatMessages = new List<CybersecurityQuiz.ChatMessage>();
            //Bind the chat messages list to the ChatBox control
            ChatBox.ItemsSource = chatMessages;
            //Initialize the task memory
            memory = new ChatbotMemory();
            //Initialize the user interaction handler with the memory
            userInteraction = new UserInteraction(memory);
            //Initialize the voice greeting component
            voiceGreeting = new VoiceGreeting();
            //Initialize the ASCII art component
            asciiArt = new ASCIIArt();
            //Set flag to indicate the chatbot is waiting for the user's name
            isWaitingForName = true;

            //Event handler for when the window is loaded
            Loaded += (s, e) =>
            {
                //Define the file path for the ASCII art file
                string asciiFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ascii-art (4).txt");
                //Load the ASCII art logo
                string asciiResult = asciiArt.Logo(asciiFilePath);
                //Split the ASCII art into lines for display
                string[] asciiLines = asciiResult.Split(new[] { "\n" }, StringSplitOptions.None);
                foreach (string line in asciiLines)
                {
                    //Trim trailing whitespace from each line
                    string trimmedLine = line.TrimEnd();
                    //Skip empty lines to avoid unnecessary UI entries
                    if (string.IsNullOrEmpty(trimmedLine))
                        continue;

                    //Handle ASCII art lines based on content
                    if (trimmedLine.Contains("ASCII logo file not found") || trimmedLine.Contains("Error reading ASCII logo"))
                        //Display error message
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage($"{trimmedLine}", "Red"));
                    else if (trimmedLine.Contains("Welcome to the Cybersecurity Awareness Bot"))
                        //Add welcome message
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage(trimmedLine));
                    else
                        //Add regular ASCII art lines with dark blue colour
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage(trimmedLine, "DarkBlue"));
                }

                //Generate and display the voice greeting
                string voiceResult = voiceGreeting.Greeting();
                if (!string.IsNullOrEmpty(voiceResult))
                    //Add voice greeting error message in red if applicable
                    chatMessages.Add(new CybersecurityQuiz.ChatMessage($"{voiceResult}", "Red"));

                //Prompt the user for their name
                chatMessages.Add(new CybersecurityQuiz.ChatMessage("What is your name?", "Brown"));

                //Refresh the ChatBox to display all messages
                ChatBox.Items.Refresh();
                //Scroll to the latest message
                ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
            };
        }
        //---------------------------------------------------------------------------------------------------------------------------//
        //Private method for the Send button click
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //Get and trim user input from the InputBox
            string userInput = InputBox.Text.Trim();
            //Ignore empty input
            if (string.IsNullOrEmpty(userInput))
                return;

            //Add the user's input to the chat messages with "User:" prefix
            chatMessages.Add(new CybersecurityQuiz.ChatMessage($"User: {userInput} \n"));

            //Handle input based on the chatbot's state
            if (isWaitingForName)
            {
                //Process the user's name input
                var responses = userInteraction.ProcessInput(userInput, "");
                foreach (var response in responses)
                {
                    //Add each response to the chat messages with "Chatbot:" prefix 
                    chatMessages.Add(new CybersecurityQuiz.ChatMessage($"Chatbot: {response.Text}", response.Color));
                }
                //Update state to indicate the name has been received
                isWaitingForName = false;
            }
            else if (userInput.ToLower() == "show art")
            {
                //Handle the "show art" command to display ASCII art
                string asciiFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ascii-art (4).txt");
                string asciiResult = asciiArt.Logo(asciiFilePath);
                string[] asciiLines = asciiResult.Split(new[] { "\n" }, StringSplitOptions.None);

                foreach (string line in asciiLines)
                {
                    //trim trailing whitespace from each line
                    string trimmedLine = line.TrimEnd();
                    //Skip empty lines
                    if (string.IsNullOrEmpty(trimmedLine))
                        continue;

                    //Handle ASCII art lines based on content
                    if (trimmedLine.Contains("ASCII logo file not found") || trimmedLine.Contains("Error reading ASCII logo"))
                        //Display error message
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage($"{trimmedLine}", "Red"));
                    else if (trimmedLine.Contains("Welcome to the Cybersecurity Awareness Bot"))
                        //Add welcome message
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage(trimmedLine));
                    else
                        //Add regular ASCII art lines with dark blue colour
                        chatMessages.Add(new CybersecurityQuiz.ChatMessage(trimmedLine, "DarkBlue"));
                }
            }
            else
            {
                //Process general user input
                var responses = userInteraction.ProcessInput(userInput, "");
                foreach (var response in responses)
                {
                    //Add each response to the chat messages with "Chatbot:" prefix
                    chatMessages.Add(new CybersecurityQuiz.ChatMessage($"Chatbot: {response.Text}", response.Color));
                }
            }

            //Refresh the ChatBox to display updated messages
            ChatBox.Items.Refresh();
            //Clear the input box
            InputBox.Text = "";
            //Scroll to the latest message
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
        }
        //------------------------------------------------------------------------------------------------------------------//
        //Private method for the Start Quiz button click
        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            //Process the "start quiz" command
            var responses = userInteraction.ProcessInput("start quiz", "");
            foreach (var response in responses)
            {
                //Add each response to the chat messages with specified colour
                chatMessages.Add(new CybersecurityQuiz.ChatMessage($"{response.Text}", response.Color));
            }
            //Refresh the ChatBox to display updated messages
            ChatBox.Items.Refresh();
            //Scroll to the latest message
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);
        }
        //-------------------------------------------------------------------------------------------------------------//
        //Private method for the Manage Tasks button click
        private void ManageTasksButton_Click(object sender, RoutedEventArgs e)
        {
            //Create and show the TaskWindow dialog for managing tasks
            TaskWindow taskWindow = new TaskWindow(memory);
            taskWindow.ShowDialog();
        }
        //------------------------------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//

}
//---------------------------------------End of Code-------------------------------------------------//