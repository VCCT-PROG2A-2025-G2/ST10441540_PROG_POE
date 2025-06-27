//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

using System;
using System.Windows;

namespace ST10441540_PROG_POE
{
    //-----------------------------------------------------------------------------------//
    //A window class for selecting a reminder date using a DatePicker control
    public partial class ReminderPickerWindow : Window
    {
        //Declaration
        public DateTime? SelectedDate { get; private set; }

        //Constructor
        public ReminderPickerWindow()
        {
            //Initialize the window's components
            InitializeComponent();
            //Set the default selected date in the DatePicker to today's date
            ReminderDatePicker.SelectedDate = DateTime.Today;
        }
        //----------------------------------------------------------------------------------------------------------------------//
        //Private method for the ok button click
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if a valid date is selected
            if (ReminderDatePicker.SelectedDate.HasValue)
            {
                //Store the selected date in the SelectedDate property
                SelectedDate = ReminderDatePicker.SelectedDate;
                //Indicate that the dialog was closed successfully
                DialogResult = true;
            }
            else
            {
                //Display error message
                MessageBox.Show("Please select a valid reminder date.", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------//
        //Private method for the Cancel button click
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Set DialogResult to false to indicate the dialog was cancelled
            DialogResult = false;
        }
        //----------------------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//
}
//---------------------------------------End of Code-------------------------------------------------//