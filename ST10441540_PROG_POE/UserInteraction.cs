//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

//Reference:
//                      https://www.ibm.com/think/topics/natural-language-processing
//                      https://www.geeksforgeeks.org/nlp/nlp-techniques/
//                      https://youtu.be/ioi__WRETk4?si=baYNrcQKP4fRNWVu



using ST10441540_PROG_POE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;

namespace ST10441540_PROG_POE
{
    //--------------------------------------------------------------------------------------------------------//
    //A class to handle user interactions with the chatbot, processing inputs and generating responses
    class UserInteraction
    {
        //Declaration
        //Reference to the chatbot's memory for storing user data and tasks
        private readonly ChatbotMemory memory;
        //Reference to the cybersecurity quiz for quiz related interactions
        private readonly CybersecurityQuiz quiz;
        //Array of supported cybersecurity topics
        private static readonly string[] Topics = { "password", "scam", "privacy", "phishing", "safe browsing" };

        //Flag to track if this is the first interaction 
        private bool isFirstInteraction = true;
        //Flag to track if the chatbot is waiting for a reminder response
        private bool awaitingReminderResponse = false;


        //Constructor for UserInteraction
        public UserInteraction(ChatbotMemory memory)
        {
            //Initialize memory with null checking thrown
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            //Initialize the cybersecurity quiz
            this.quiz = new CybersecurityQuiz();
        }

        //-------------------------------------------------------------------------------------------------------//
        //Processes user input and returns a list of chat messages as responses
        public List<CybersecurityQuiz.ChatMessage> ProcessInput(string input, string sentimentResponse)
        {
            var messages = new List<CybersecurityQuiz.ChatMessage>();

            //Handle the first interaction to capture the user's name
            if (isFirstInteraction)
            {
                if (string.IsNullOrEmpty(input))
                {
                    //Prompt for name if input is empty
                    messages.Add(new CybersecurityQuiz.ChatMessage("Can you please enter your name?\n", "Red"));
                    return messages;
                }
                else
                {
                    //Store the user's name and log the action
                    memory.SetUserName(input);
                    isFirstInteraction = false;
                    memory.LogAction($"User of a name, {input}, logged in");
                    //Welcome the user and prompt for their favourite topic
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Hello, nice to meet you, {input}! How may I help you today?"));
                    messages.Add(new CybersecurityQuiz.ChatMessage($"What’s your favourite cybersecurity topic [password, scam, privacy, phishing, safe browsing]?\n"));
                    return messages;
                }
            }

            //Get the user's name and favourite topic from memory
            string name = memory.GetUserName();
            string favTopic = memory.GetFavoriteTopic();

            //Handle setting the favourite topic if not yet set
            if (string.IsNullOrEmpty(favTopic))
            {
                if (string.IsNullOrEmpty(input))
                {
                    //Display error message
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Sorry, {name}, you didn't enter a topic. Please choose from: password, scam, privacy, phishing, or safe browsing.\n", "Red"));
                    return messages;
                }

                //Check if the input contains a valid topic
                string? matchedTopic = Topics.FirstOrDefault(topic => input.Contains(topic));
                if (matchedTopic != null)
                {
                    //Set and log the favourite topic
                    memory.SetFavoriteTopic(matchedTopic);
                    memory.LogAction($"Favourite topic chosen ({matchedTopic})");
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Great choice, {name}! I’ll remember that you’re interested in {matchedTopic} and noted it as your favourite topic."));
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Would you like to ask a question (or type 'exit' to quit)?\n"));
                    return messages;
                }
                else
                {
                    //Handle invalid topic input
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Sorry, {name}, I don’t recognize that topic.", "Red"));
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Please choose from: password, scam, privacy, phishing, and safe browsing.\n"));
                    return messages;
                }
            }

            //Handle empty input when not expecting a specific response
            if (string.IsNullOrEmpty(input))
            {
                messages.Add(new CybersecurityQuiz.ChatMessage($"{name}, please ask a valid question or give a command.\n", "Red"));
                return messages;
            }

            //Handle task related commands
            if (input.ToLower().StartsWith("add task") ||
                input.ToLower().StartsWith("set up") ||
                input.ToLower().StartsWith("create task") ||
                input.ToLower().StartsWith("remind me to") ||
                input.ToLower().StartsWith("can you remind me to") ||
                input.ToLower().StartsWith("add a task to") ||
                input.ToLower().StartsWith("create a task to") ||
                input.ToLower().StartsWith("set a reminder") ||
                input.ToLower().StartsWith("set a reminder to") ||
                input.ToLower().StartsWith("add task to"))
            {
                //Remove task related keywords from input
                string taskInput = input
                    .Replace("add task -", "")
                    .Replace("add task", "")
                    .Replace("set up", "")
                    .Replace("create task", "")
                    .Replace("remind me to", "")
                    .Replace("can you remind me to", "")
                    .Replace("add a task to", "")
                    .Replace("add a task", "")
                    .Replace("create a task to", "")
                    .Replace("set a reminder", "")
                    .Replace("set a reminder to", "")
                    .Replace("add task to", "")
                    .Trim();
                DateTime? reminderDate = null;

                //Parse time related keywords for reminder dates
                if (taskInput.Contains("today")) reminderDate = DateTime.Today;
                else if (taskInput.Contains("tomorrow")) reminderDate = DateTime.Today.AddDays(1);
                else if (taskInput.Contains("next week")) reminderDate = DateTime.Today.AddDays(7);
                else if (Regex.IsMatch(taskInput, @"in\s*(\d+)\s*week(s)?"))
                {
                    var match = Regex.Match(taskInput, @"(\d+)\s*week(s)?");
                    if (int.TryParse(match.Groups[1].Value, out int weeks))
                    {
                        reminderDate = DateTime.Today.AddDays(weeks * 7);
                    }
                }

                //Remove time related keywords from task title
                taskInput = Regex.Replace(taskInput, @"(today|tomorrow|next week|in\s*\d+\s*week(s)?)", "").Trim();

                //Validate task input
                if (string.IsNullOrEmpty(taskInput))
                {
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Please specify a task, {name} (e.g., 'add task - Review privacy settings')\n.", "Red"));
                    return messages;
                }

                //Format task title in title case, excluding certain words
                string[] wordsToExclude = { "to", "a" };
                string[] words = taskInput.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(word => !wordsToExclude.Contains(word.ToLower()))
                    .ToArray();
                string titleCaseTask = string.Join(" ", words.Select(word =>
                    char.ToUpper(word[0]) + word.Substring(1).ToLower()));
                string description = $"Review Task: {taskInput.ToLower()} to ensure that it is completed.";

                //Add the task to memory and log the action
                memory.AddTask(titleCaseTask, description);
                memory.LogAction($"Task added: {titleCaseTask}");

                if (reminderDate.HasValue)
                {
                    //Set reminder for the task and log it
                    var task = memory.GetTasks().Last();
                    task.ReminderDate = reminderDate.Value;
                    memory.LogAction($"Reminder set for task '{task.Title}' on {task.ReminderDate:yyyy-MM-dd}");
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Task added with the title '{titleCaseTask}' and reminder set for {reminderDate.Value:yyyy-MM-dd}."));
                    messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                }
                else
                {
                    //Prompt for a reminder if none was specified
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Task added with the title '{titleCaseTask}' and description '{description}'. Would you like a reminder?\n"));
                    awaitingReminderResponse = true;
                }
                return messages;
            }

            //Handle reminder response for a previously added task
            if (awaitingReminderResponse)
            {
                input = input.ToLower().Trim();
                int lastTaskIndex = memory.GetTasks().Count - 1;
                if (lastTaskIndex < 0)
                {
                    //Handle case where no task exists
                    messages.Add(new CybersecurityQuiz.ChatMessage($"No task found to respond to, {name}. Would you like to add a task or ask a question?\n", "Red"));
                    awaitingReminderResponse = false;
                    return messages;
                }
                var lastTask = memory.GetTasks()[lastTaskIndex];

                //Handle declining a reminder
                if (input.StartsWith("no") || input.StartsWith("nope") || input.StartsWith("no thanks") || input.StartsWith("no thank you"))
                {
                    memory.LogAction($"Reminder declined for task '{lastTask.Title}'");
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Okay, {name}, no reminder set for the task."));
                    messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                    awaitingReminderResponse = false;
                    return messages;
                }
                //Handle setting a reminder with a timeframe
                else if (input.StartsWith("yes, remind me in") || input.StartsWith("in") || input.StartsWith("remind me in") ||
                         input.Contains("today") || input.Contains("tomorrow") || input.Contains("next week"))
                {
                    //Extract and parse timeframe
                    string timeFrame = input
                        .Replace("yes, remind me in", "")
                        .Replace("remind me in", "")
                        .Replace("in", "")
                        .Trim();
                    DateTime? reminderDate = null;

                    //Different timeframe words and there format
                    if (timeFrame.Contains("today"))
                    {
                        reminderDate = DateTime.Today;
                    }
                    else if (timeFrame.Contains("tomorrow"))
                    {
                        reminderDate = DateTime.Today.AddDays(1);
                    }
                    else if (timeFrame.Contains("next week"))
                    {
                        reminderDate = DateTime.Today.AddDays(7);
                    }
                    else if (Regex.IsMatch(timeFrame, @"(\d+)\s*week(s)?"))
                    {
                        var match = Regex.Match(timeFrame, @"(\d+)\s*week(s)?");
                        if (int.TryParse(match.Groups[1].Value, out int weeks))
                        {
                            reminderDate = DateTime.Today.AddDays(weeks * 7);
                        }
                    }
                    else if (Regex.IsMatch(timeFrame, @"(\d+)\s*day(s)?"))
                    {
                        var match = Regex.Match(timeFrame, @"(\d+)\s*day(s)?");
                        if (int.TryParse(match.Groups[1].Value, out int days))
                        {
                            reminderDate = DateTime.Today.AddDays(days);
                        }
                    }

                    if (reminderDate.HasValue)
                    {
                        //Set and log the reminder
                        lastTask.ReminderDate = reminderDate.Value;
                        memory.LogAction($"Reminder set for task '{lastTask.Title}' on {lastTask.ReminderDate:yyyy-MM-dd}");
                        messages.Add(new CybersecurityQuiz.ChatMessage($"Got it! I'll remind you on {lastTask.ReminderDate:yyyy-MM-dd} for '{lastTask.Title}'."));
                        messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                        awaitingReminderResponse = false;
                        return messages;
                    }

                    //Prompt for a valid timeframe if parsing fails
                    messages.Add(new CybersecurityQuiz.ChatMessage("Please specify a valid timeframe (e.g., 'yes, remind me in 3 days', 'today', 'tomorrow', 'next week', 'in 2 weeks').\n", "Red"));
                    return messages;
                }
                //Handle setting a reminder with a specific date
                else if (input.ToLower().StartsWith("yes, remind me on") || input.ToLower().StartsWith("on") || input.ToLower().StartsWith("remind me on"))
                {
                    string dateStr = input.Replace("yes, remind me on", "").Replace("remind me on", "").Replace("on", "").Trim();
                    if (DateTime.TryParse(dateStr, out DateTime reminderDate))
                    {
                        //Set and log the reminder for a specific date
                        lastTask.ReminderDate = reminderDate;
                        memory.LogAction($"Reminder set for task '{lastTask.Title}' on {lastTask.ReminderDate:yyyy-MM-dd}");
                        messages.Add(new CybersecurityQuiz.ChatMessage($"Got it! I'll remind you on {reminderDate:yyyy-MM-dd} for '{lastTask.Title}'."));
                        messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                        awaitingReminderResponse = false;
                        return messages;
                    }
                    //Prompt for a valid date if parsing fails
                    messages.Add(new CybersecurityQuiz.ChatMessage("Please specify a valid date (e.g., 'yes, remind me on 27 June').\n", "Red"));
                    return messages;
                }
                else
                {
                    //Prompt for a valid reminder response
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Please respond to the reminder question, {name}. Would you like a reminder? (e.g., 'yes, remind me tomorrow' or 'no')\n", "Red"));
                    return messages;
                }
            }

            //Handle quiz initiation
            if (input.Contains("quiz"))
            {
                quiz.StartQuiz();
                memory.LogAction($"Quiz started");
                messages.Add(new CybersecurityQuiz.ChatMessage(quiz.DisplayCurrentQuestion()));
                messages.Add(new CybersecurityQuiz.ChatMessage("\n"));
                return messages;
            }

            //Process quiz answers if the quiz is active
            if (quiz.IsActive())
            {
                var quizMessages = quiz.ProcessAnswer(input);
                if (quiz.IsQuizCompleted())
                {
                    memory.LogAction($"Quiz completed");
                }
                return quizMessages;
            }

            //Handle exit command with sound effect
            if (input.Contains("exit"))
            {
                try
                {
                    //Play an exit sound
                    SoundPlayer player = new SoundPlayer("end.wav");
                    player.Load();
                    player.PlaySync();
                }
                catch (Exception ex)
                {
                    //Handle errors in playing the sound
                    messages.Add(new CybersecurityQuiz.ChatMessage("Error playing sound: " + ex.Message, "Red"));
                    return messages;
                }
                memory.LogAction($"User exited");
                //Display goodbye message
                messages.Add(new CybersecurityQuiz.ChatMessage($"Goodbye, {name}, I hope you have a good day further!\n"));
                return messages;
            }

            //Detect sentiment words in the input and generate a response
            string? sentiment = DetectSentiment(input);
            if (!string.IsNullOrEmpty(sentiment))
            {
                sentimentResponse = sentiment switch
                {
                    "worried" => $"I hear you're {sentiment}, {name}. Let’s address your concern.",
                    "scared" => $"No need to be {sentiment}, {name}. I’m here to help!",
                    "curious" => $"Love that {sentiment} vibe, {name}! Let’s dive in.",
                    "wondering" => $"Great to see you’re {sentiment}, {name}! Here’s some info.",
                    "frustrated" => $"Sorry you’re feeling {sentiment}, {name}. Let’s sort this out.",
                    "annoyed" => $"I get that you’re {sentiment}, {name}. Let’s fix this.",
                    "excited" => $"That’s awesome, {name}! Let’s channel that {sentiment} energy.",
                    "happy" => $"Glad you’re {sentiment}, {name}! Let’s keep the good vibes going.",
                    _ => ""
                };
            }

            //Process general input using the core processing method
            messages.AddRange(ProcessInputCore(input, sentimentResponse));
            return messages;
        }
        //---------------------------------------------------------------------------------------------//
        //Private method to process general user input
        private List<CybersecurityQuiz.ChatMessage> ProcessInputCore(string input, string sentimentResponse)
        {
            var messages = new List<CybersecurityQuiz.ChatMessage>();
            string name = memory.GetUserName();
            string favTopic = memory.GetFavoriteTopic();

            //Handle specific queries or questions
            if (input.Contains("how are you"))
            {
                memory.LogAction($"Question asked how I am");
                messages.Add(new CybersecurityQuiz.ChatMessage($"I'm just a program, {name}, but I'm running smoothly and doing well!"));
                messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                return messages;
            }
            if (input.Contains("what is the purpose") || input.Contains("what is your purpose"))
            {
                memory.LogAction($"Question asked about my purpose");
                messages.Add(new CybersecurityQuiz.ChatMessage($"My purpose is to help you stay safe online by answering your cybersecurity questions, {name}!"));
                messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                return messages;
            }
            if (input.Contains("what can i ask about"))
            {
                memory.LogAction($"Question asked what they can ask about");
                messages.Add(new CybersecurityQuiz.ChatMessage($"{name}, you can ask about:"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- How are you?"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- What is the purpose?"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- Password safety"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- Phishing or scams"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- Privacy"));
                messages.Add(new CybersecurityQuiz.ChatMessage("- Safe browsing"));
                messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                return messages;
            }

            //Check for topic related questions and for favoutire topic
            string? matchedTopic = Topics.FirstOrDefault(topic => input.Contains(topic));
            if (matchedTopic != null)
            {
                memory.SetLastTopic(matchedTopic);
                memory.LogAction($"Question asked about {matchedTopic}");
                string randomResponse = memory.GetRandomResponse(matchedTopic);
                if (!string.IsNullOrEmpty(sentimentResponse))
                {
                    randomResponse = $"{sentimentResponse} {randomResponse}";
                }
                if (!string.IsNullOrEmpty(favTopic) && favTopic == matchedTopic)
                {
                    randomResponse += $" I remember this is your favourite topic, {name}!";
                }
                messages.Add(new CybersecurityQuiz.ChatMessage(randomResponse));
                messages.Add(new CybersecurityQuiz.ChatMessage("Want more or something else? (Type 'exit' to quit)\n"));
                return messages;
            }

            //Handle requests for more information on the last topic
            if (input.Contains("more") || input.Contains("tell me more"))
            {
                string lastTopic = memory.GetLastTopic();
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    memory.LogAction($"Question asked more on {lastTopic}");
                    string randomResponse = memory.GetRandomResponse(lastTopic);
                    if (!string.IsNullOrEmpty(sentimentResponse))
                    {
                        randomResponse = $"{sentimentResponse} {randomResponse}";
                    }
                    if (!string.IsNullOrEmpty(favTopic) && favTopic == lastTopic)
                    {
                        randomResponse += $" Always fun to chat about your favourite, {name}!";
                    }
                    messages.Add(new CybersecurityQuiz.ChatMessage(randomResponse));
                    messages.Add(new CybersecurityQuiz.ChatMessage("Anything else? (Type 'exit' to quit)\n"));
                    return messages;
                }
                else
                {
                    messages.Add(new CybersecurityQuiz.ChatMessage($"I don’t have a previous topic to expand on, {name}. What would you like to talk about?\n"));
                    return messages;
                }
            }

            //Handle activity log queries
            if (input.ToLower() == "show activity log" || input.ToLower() == "what have you done for me")
            {
                var actions = memory.GetActivityLog();
                if (actions.Count == 0)
                {
                    messages.Add(new CybersecurityQuiz.ChatMessage($"No recent actions, {name}."));
                }
                else
                {
                    memory.LogAction($"User viewed activity log");
                    messages.Add(new CybersecurityQuiz.ChatMessage($"Summary of recent actions:"));
                    //Display up to 5 actions by default
                    int displayCount = Math.Min(5, actions.Count);
                    for (int i = 0; i < displayCount; i++)
                    {
                        messages.Add(new CybersecurityQuiz.ChatMessage($"- {actions[i].Timestamp:yyyy-MM-dd HH:mm:ss} - {actions[i].Description}"));
                    }
                    if (actions.Count > 5)
                    {
                        messages.Add(new CybersecurityQuiz.ChatMessage($"Type 'expand' to see all {actions.Count} actions."));
                    }
                }
                messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                return messages;


            }

            //Handle request to show the full activity log
            if (input.ToLower() == "expand")
            {
                var actions = memory.GetActivityLog();
                memory.LogAction($"User viewed full activity log");
                messages.Add(new CybersecurityQuiz.ChatMessage($"Full activity log:"));
                foreach (var action in actions)
                {
                    messages.Add(new CybersecurityQuiz.ChatMessage($"- {action.Timestamp:yyyy-MM-dd HH:mm:ss} - {action.Description}"));
                }
                messages.Add(new CybersecurityQuiz.ChatMessage("Would you like to ask a question (or type 'exit' to quit)?\n"));
                return messages;
            }

            //Handle unrecognized input
            messages.Add(new CybersecurityQuiz.ChatMessage($"Sorry {name}, I’m not sure I understand. Can you try rephrasing?\n", "Red"));
            return messages;
        }

        //Detects sentiment words in the user's input
        private string? DetectSentiment(string input)
        {
            var sentiments = new Dictionary<string, string[]>
            {
                { "worried", new[] { "worried", "concerned", "anxious", "nervous" } },
                { "scared", new[] { "scared", "afraid", "terrified", "frightened" } },
                { "curious", new[] { "curious", "interested", "wonder", "intrigued" } },
                { "wondering", new[] { "wondering", "pondering", "thinking about", "musing" } },
                { "frustrated", new[] { "frustrated", "upset", "irritated", "angry" } },
                { "annoyed", new[] { "annoyed", "irritated", "bothered", "peeved" } },
                { "excited", new[] { "excited", "thrilled", "eager", "enthusiastic" } },
                { "happy", new[] { "happy", "glad", "joyful", "pleased" } }
            };

            input = input.ToLower();
            foreach (var sentiment in sentiments)
            {
                if (sentiment.Value.Any(word => input.Contains(word)))
                {
                    return sentiment.Key;
                }
            }
            return null;
        }
        //----------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//
}
//---------------------------------------End of Code-------------------------------------------------//