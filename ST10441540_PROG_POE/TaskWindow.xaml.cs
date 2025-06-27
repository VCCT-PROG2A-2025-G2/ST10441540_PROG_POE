//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2




using ST10441540_PROG_POE;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ST10441540_PROG_POE
{
    //------------------------------------------------------------------------------------------------------//
    //A window class for managing tasks, including adding, deleting, and marking tasks as completed
    public partial class TaskWindow : Window
    {
        //Declaration
        private readonly ChatbotMemory memory;

        //Constructor
        public TaskWindow(ChatbotMemory memory)
        {
            //Initialize the window's UI components 
            InitializeComponent();
            //Ensure the memory parameter is not null, throw exception if it is
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            //Populate the task list UI with current tasks
            UpdateTaskList();
        }
        //---------------------------------------------------------------------------------//
        //Private method for the Add Task button click
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            //Get and trim input from the title and description text boxes
            string title = TaskTitleBox.Text.Trim();
            string description = TaskDescriptionBox.Text.Trim();

            //Validates that both title and description are provided
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(description))
            {
                //Add the task to the memory
                memory.AddTask(title, description);
                //Log the task addition to the activity log
                memory.LogAction($"Task added with description: {title}, {description} ");
                //Refresh the task list 
                UpdateTaskList();

                //Prompt user to set a reminder for the task
                MessageBoxResult result = MessageBox.Show(
                    "Task added successfully! Would you like to set a reminder?",
                    "Set Reminder",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                //If user chooses to set a reminder
                if (result == MessageBoxResult.Yes)
                {
                    //Create and show a ReminderPickerWindow dialog
                    var reminderDialog = new ReminderPickerWindow
                    {
                        Owner = this
                    };

                    //Check if the dialog was closed with a valid date selection
                    if (reminderDialog.ShowDialog() == true && reminderDialog.SelectedDate.HasValue)
                    {
                        //Get the selected reminder date
                        DateTime reminderDate = reminderDialog.SelectedDate.Value;
                        //Get the index of the newly added task
                        int taskIndex = memory.GetTasks().Count - 1;
                        //Attempt to set the reminder for the task
                        if (memory.SetReminder(taskIndex, reminderDate))
                        {
                            //Log the reminder setting to the activity log
                            memory.LogAction($"Reminder set for task '{title}' on {reminderDate:yyyy-MM-dd}");
                            //Display the user of successful reminder setting
                            MessageBox.Show($"Reminder set for {reminderDate:yyyy-MM-dd}.", "Reminder Set");
                        }
                        else
                        {
                            //Display error message
                            MessageBox.Show("Failed to set reminder. Please try again.", "Error");
                        }
                        //Refresh the task list UI to reflect any changes
                        UpdateTaskList();
                    }
                    else
                    {
                        //Notify user if no reminder was set
                        MessageBox.Show("No reminder was set.", "No Reminder");
                    }
                }

                //Clear input fields for the next task entry
                TaskTitleBox.Clear();
                TaskDescriptionBox.Clear();
                ReminderDatePicker.SelectedDate = null;
            }
            else
            {
                //Display error message
                MessageBox.Show("Please enter both a title and description.", "Missing Information");
            }
        }
        //-------------------------------------------------------------------------------------------------------//
        //Private method for the Delete Task button click
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if a task is selected in the ListBox
            if (TaskListBox.SelectedIndex >= 0)
            {
                //Get the title of the task to be deleted for logging
                string taskTitle = memory.GetTasks()[TaskListBox.SelectedIndex].Title;
                //Attempt to delete the selected task
                if (memory.DeleteTask(TaskListBox.SelectedIndex))
                {
                    //Log the task deletion to the activity log
                    memory.LogAction($"Task deleted: {taskTitle}");
                    //Refresh the task list UI after deletion
                    UpdateTaskList();
                }
                else
                {
                    //Display error message
                    MessageBox.Show("Failed to delete task. Please try again.", "Error");
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------//
        //Private method for the Mark Completed button click
        private void MarkCompletedButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if a task is selected in the ListBox
            if (TaskListBox.SelectedIndex >= 0)
            {
                //Get the title of the task to be marked completed for logging
                string taskTitle = memory.GetTasks()[TaskListBox.SelectedIndex].Title;
                //Attempt to mark the selected task as completed
                if (memory.MarkTaskCompleted(TaskListBox.SelectedIndex))
                {
                    //Log the task completion to the activity log
                    memory.LogAction($"Task marked as completed: {taskTitle}");
                    //Refresh the task list UI after marking as completed
                    UpdateTaskList();
                }
                else
                {
                    //Display error message
                    MessageBox.Show("Failed to mark task as completed. Please try again.", "Error");
                }
            }
        }
        //--------------------------------------------------------------------------------------------------//
        //Private method to refresh the task list in the UI
        private void UpdateTaskList()
        {
            //Clear the current ItemsSource to avoid duplication
            TaskListBox.ItemsSource = null;
            //Set the ItemsSource to the current list of tasks from memory
            TaskListBox.ItemsSource = memory.GetTasks();
        }
        //--------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//
}
//---------------------------------------End of Code-------------------------------------------------//