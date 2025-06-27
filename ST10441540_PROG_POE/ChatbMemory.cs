//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

//Reference:
//                      https://chatgpt.com/share/685e8830-1f24-8009-8f03-190b40536069

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10441540_PROG_POE
{
    //------------------------------------------------------------------------------------------------------------//
    //Class to manage the chatbot's memory, including user data, responses, tasks, and activity logging
    public class ChatbotMemory
    {
        //Declaration
        //Stores the user's name
        private string userName;
        //Stores the user's favourite topic
        private string favoriteTopic;
        //Tracks the last topic discussed
        private string lastTopic;
        //Tracks the last intent of the conversation
        private string lastIntent;
        //Dictionary mapping topics to lists of possible responses
        private readonly Dictionary<string, List<string>> responses;
        //Random number generator for selecting responses
        private readonly Random random;
        //Tracks the last used response for each topic to avoid repetition
        private Dictionary<string, string> lastUsedResponses;
        //List of tasks managed by the chatbot
        private List<Task> tasks;
        //List of activity log entries with timestamps and descriptions
        private List<(DateTime Timestamp, string Description)> activityLog;

        //------------------------------------------------------------------------------------------------------//
        //Nested class to represent a task with title, description, reminder, and completion status
        public class Task
        {
            //Task title
            public string Title { get; set; }
            //Task description
            public string Description { get; set; }
            //Optional reminder date for the task
            public DateTime? ReminderDate { get; set; }
            //Indicates whether the task is completed
            public bool IsCompleted { get; set; }
            //Checks if the reminder date is valid 
            public bool IsReminderValid => ReminderDate.HasValue && ReminderDate.Value != DateTime.MinValue;

            //Constructor
            public Task(string title, string description, DateTime? reminderDate = null)
            {
                Title = title;
                Description = description;
                ReminderDate = reminderDate;
                IsCompleted = false;
            }

            //Overrides ToString to provide a formatted string representation of the task
            public override string ToString()
            {
                string reminderText = ReminderDate.HasValue ? $"Reminder: {ReminderDate.Value:yyyy-MM-dd}" : "No reminder set";
                string status = IsCompleted ? "Completed" : "Not Completed";
                return $"{Title}\n{Description}\n{reminderText}\nStatus: {status}";
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------//

        //Constructor for the ChatbotMemory class
        public ChatbotMemory()
        {
            //Initialize fields with default values
            userName = "";
            favoriteTopic = "";
            lastTopic = "";
            lastIntent = "";
            tasks = new List<Task>();
            activityLog = new List<(DateTime, string)>();
            random = new Random();
            //Initialize the responses dictionary with predefined topic-response pairs
            responses = new Dictionary<string, List<string>>
            {
                { "password", new List<string>
                {
                    "Use strong, unique passwords for each account.",
                    "Avoid personal details in your passwords.",
                    "Mix letters, numbers, and symbols in passwords.",
                    "Enable two-factor authentication (2FA).",
                    "Never reuse passwords across accounts.",
                    "Keep your passwords private and secure."
                } },
                { "scam", new List<string>
                {
                    "Be cautious of emails asking for info.",
                    "Scammers may pose as trusted organizations.",
                    "Avoid urgent or threatening messages.",
                    "Verify the source of any offer.",
                    "Don’t share info if it seems too good."
                } },
                { "privacy", new List<string>
                {
                    "Review your account settings regularly.",
                    "Protect your privacy with updates.",
                    "Avoid unsecured websites for sharing.",
                    "Look for 'HTTPS' in the address bar.",
                    "Use a VPN for better privacy."
                } },
                { "phishing", new List<string>
                {
                    "Don’t click suspicious email links.",
                    "Always verify the sender first.",
                    "Check for 'HTTPS' and a padlock.",
                    "Beware of urgent messages."
                } },
                { "safe browsing", new List<string>
                {
                    "Keep your browser updated always.",
                    "Ensure security software is current.",
                    "Avoid unknown or unsecured sites.",
                    "Don’t enter information on risky pages.",
                    "Use unique passwords per website.",
                    "Enable pop-up blockers for safety."
                } }
            };
            //Initialize the dictionary to track last used responses
            lastUsedResponses = new Dictionary<string, string>();
        }

        //Sets the user's name with null checking
        public void SetUserName(string name) => userName = name ?? throw new ArgumentNullException(nameof(name));

        //Gets the user's name
        public string GetUserName() => userName;

        //Sets the user's favorite topic with null checking
        public void SetFavoriteTopic(string topic) => favoriteTopic = topic ?? throw new ArgumentNullException(nameof(topic));

        //Gets the user's favorite topic
        public string GetFavoriteTopic() => favoriteTopic;

        //Sets the last topic discussed, clearing last used response if the topic changes
        public void SetLastTopic(string topic)
        {
            if (lastTopic != topic && !string.IsNullOrEmpty(lastTopic))
            {
                lastUsedResponses.Remove(lastTopic);
            }
            lastTopic = topic ?? throw new ArgumentNullException(nameof(topic));
        }

        //Gets the last topic discussed
        public string GetLastTopic() => lastTopic;

        //Sets the last intent of the conversation with null checking
        public void SetLastIntent(string intent) => lastIntent = intent ?? throw new ArgumentNullException(nameof(intent));

        //Gets the last intent of the conversation
        public string GetLastIntent() => lastIntent;

        //Retrieves a random response for a given topic, avoiding repetition if possible
        public string GetRandomResponse(string topic)
        {
            //Validate topic input
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty.", nameof(topic));

            //Check if the topic exists in the responses dictionary
            if (!responses.ContainsKey(topic))
                return "I don’t have information on that topic yet.";

            var responseList = responses[topic];
            //Validate that the response list is not null or empty
            if (responseList == null || responseList.Count == 0)
                throw new InvalidOperationException($"No responses available for topic: {topic}");

            //Try to get the last used response for the topic
            lastUsedResponses.TryGetValue(topic, out string? lastResponse);
            //Filter out the last used response to avoid repetition
            var availableResponses = responseList.Where(r => r != lastResponse).ToList();

            //If no other responses are available, use the full list
            if (!availableResponses.Any())
            {
                availableResponses = responseList;
            }

            //Select a random response from the available responses
            string newResponse = availableResponses[random.Next(availableResponses.Count)];
            //Update the last used response for the topic
            lastUsedResponses[topic] = newResponse;
            return newResponse;
        }

        //Checks if a topic exists in the responses dictionary
        public bool HasTopic(string topic) => !string.IsNullOrEmpty(topic) && responses.ContainsKey(topic);

        //-------------------------------------------------------------------------------------------------------------//
        //Public method - adds a new task with title, description, and optional reminder date
        public void AddTask(string title, string description, DateTime? reminderDate = null)
        {
            //Validate task title
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Task title cannot be null or empty.", nameof(title));
            //Validate task description
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Task description cannot be null or empty.", nameof(description));

            //Add the new task to the tasks list
            tasks.Add(new Task(title, description, reminderDate));
        }
        //-------------------------------------------------------------------------------------------------------------//
        //Public boolean method - sets a reminder date for a task at the specified index
        public bool SetReminder(int index, DateTime reminderDate)
        {
            //Check if the index is valid
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index].ReminderDate = reminderDate;
                return true;
            }
            return false;
        }
        //----------------------------------------------------------------------------------------------------//
        //Returns the list of all tasks
        public List<Task> GetTasks()
        {
            return tasks;
        }
        //---------------------------------------------------------------------------------------//
        //Public boolean - deletes a task at the specified index
        public bool DeleteTask(int index)
        {
            //Check if the index is valid
            if (index >= 0 && index < tasks.Count)
            {
                tasks.RemoveAt(index);
                return true;
            }
            return false;
        }
        //---------------------------------------------------------------------------------------------------//
        //Public boolean method - marks a task as completed at the specified index
        public bool MarkTaskCompleted(int index)
        {
            //Check if the index is valid
            if (index >= 0 && index < tasks.Count)
            {
                tasks[index].IsCompleted = true;
                return true;
            }
            return false;
        }

        //---------------------------------------------------------------------------------------------------------//
        //Public method - logs an action with the current timestamp, limiting the log to 10 entries
        public void LogAction(string actionDescription)
        {
            //Insert the new action at the beginning of the log
            activityLog.Insert(0, (DateTime.Now, actionDescription));
            //Keep only the most recent 10 entries
            if (activityLog.Count > 10)
            {
                activityLog.RemoveRange(10, activityLog.Count - 10);
            }
        }
        //---------------------------------------------------------------------------------------------------------//
        //Returns the activity log
        public List<(DateTime Timestamp, string Description)> GetActivityLog()
        {
            return activityLog;
        }
        //-----------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//

}
//---------------------------------------End of Code-------------------------------------------------//