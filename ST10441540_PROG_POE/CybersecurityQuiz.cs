//ST10441540 - Brandon Less
//PROG6221 POE Part 3
//Group 2

//Reference:
//          ChatGPT:    https://chatgpt.com/share/685e848c-1288-8009-b998-d82cc3c4fd56

using System;
using System.Collections.Generic;

namespace ST10441540_PROG_POE
{
    //---------------------------------------------------------------------------------------------------//
    //A class to manage a cybersecurity quiz, including questions, scoring, and user interaction
    public class CybersecurityQuiz
    {
        //Declaration
        //List to store quiz questions
        private readonly List<QuizQuestion> quizQuestions;
        //Tracks the user's current score
        private int quizScore;
        //Tracks the index of the current question
        private int currentQuestionIndex;
        //Indicates whether the quiz is active
        private bool isActive;

        //---------------------------------------------------------------------------------------//
        //Nested class to represent a single quiz question
        public class QuizQuestion
        {
            //The question text
            public string Question { get; set; }
            //Array of answer options 
            public string[]? Options { get; set; }
            //The correct answer
            public string CorrectAnswer { get; set; }
            //Feedback provided after answering
            public string Feedback { get; set; }
            //Indicates if the question is true/false 
            public bool IsTrueFalse => Options == null;

            //Constructor for a quiz question
            public QuizQuestion(string question, string[]? options, string correctAnswer, string feedback)
            {
                Question = question;
                Options = options;
                CorrectAnswer = correctAnswer;
                Feedback = feedback;
            }
        }
        //----------------------------------------------------------------------------------------------------------------//
        //Nested class to represent a chat message with text and color
        public class ChatMessage
        {
            //The message text to display
            public string Text { get; set; }
            //Colour name for the message 
            public string Color { get; set; }

            //Constructor
            public ChatMessage(string text, string color = "Black")
            {
                Text = text;
                Color = color;
            }
        }
        //-----------------------------------------------------------------------------------------------------//
        // Constructor for the CybersecurityQuiz class
        public CybersecurityQuiz()
        {
            //Initialize the list of quiz questions with predefined questions
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion("1) What should you do if you receive an email asking for your password?",
                    new[] { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it\n" },
                    "C", "Reporting phishing emails helps prevent scams and protects others."),
                new QuizQuestion("2) True or False: Using the same password for all your accounts is a good practice.\n",
                    null, "False", "Using the same password increases the risk of multiple accounts being compromised if one is hacked."),
                new QuizQuestion("3) Which of these is a common sign of a phishing email?",
                    new[] { "A) A friendly greeting", "B) An urgent request for personal information", "C) A company logo", "D) A long email signature\n" },
                    "B", "Phishing emails often create urgency to trick you into sharing sensitive information."),
                new QuizQuestion("4) True or False: It’s safe to click on links in emails as long as the sender’s name looks familiar.\n",
                    null, "False", "Attackers can spoof sender names. Always verify the email address and be cautious with links."),
                new QuizQuestion("5) What’s the best way to create a strong password?",
                    new[] { "A) Use your birthdate", "B) Use a mix of letters, numbers, and symbols", "C) Use a single word", "D) Use your pet’s name\n" },
                    "B", "A strong password is complex and unique, making it harder to guess or crack."),
                new QuizQuestion("6) True or False: HTTPS in a website’s URL means it’s always safe to enter your personal information.\n",
                    null, "False", "HTTPS indicates encryption, but the website could still be malicious. Verify its legitimacy."),
                new QuizQuestion("7) What should you do if you suspect a social engineering attack?",
                    new[] { "A) Share your details to verify your identity", "B) Hang up or stop communication immediately", "C) Ask for more details to confirm", "D) Follow their instructions carefully\n" },
                    "B", "Social engineering attacks manipulate you into sharing information. Stop contact and report it."),
                new QuizQuestion("8) True or False: Two-factor authentication (2FA) adds an extra layer of security to your accounts.\n",
                    null, "True", "2FA requires a second form of verification, making it harder for attackers to access your accounts."),
                new QuizQuestion("9) Which of these is a safe browsing habit?",
                    new[] { "A) Clicking on pop-up ads", "B) Using public Wi-Fi for online banking", "C) Keeping your browser updated", "D) Entering details on unsecured websites\n" },
                    "C", "Updates fix security vulnerabilities, keeping your browsing safer."),
                new QuizQuestion("10) True or False: Sharing your password with a friend is safe if you trust them.\n",
                    null, "False", "Never share your password, even with trusted people, as it increases the risk of unauthorized access.")
            };
            //Initialize quiz state
            quizScore = 0;
            currentQuestionIndex = 0;
            isActive = false;
        }

        //Returns whether the quiz is currently active
        public bool IsActive() => isActive;

        //Starts the quiz by resetting score and question index
        public void StartQuiz() { isActive = true; quizScore = 0; currentQuestionIndex = 0; }

        //Ends the quiz and resets the question index
        public void EndQuiz() { isActive = false; currentQuestionIndex = 0; }

        //Returns the current quiz score
        public int GetQuizScore() => quizScore;

        //Increments the quiz score by 1
        public void IncrementScore() => quizScore++;

        //Returns the current quiz question
        public QuizQuestion GetCurrentQuestion() => quizQuestions[currentQuestionIndex];

        //Checks if there are more questions to answer
        public bool HasNextQuestion() => currentQuestionIndex < quizQuestions.Count - 1;

        //Moves to the next question in the quiz
        public void MoveToNextQuestion() => currentQuestionIndex++;

        //Returns the total number of questions in the quiz
        public int GetTotalQuestions() => quizQuestions.Count;

        //Checks if the quiz is completed
        public bool IsQuizCompleted()
        {
            return !isActive && currentQuestionIndex >= quizQuestions.Count;
        }

        //Processes the user's answer and returns a list of chat messages with feedback
        public List<ChatMessage> ProcessAnswer(string userAnswer)
        {
            var messages = new List<ChatMessage>();

            //Check if the quiz is active
            if (!isActive)
            {
                messages.Add(new ChatMessage("Quiz is not active. Type 'start quiz' to begin.\n", "Red"));
                return messages;
            }

            //Get the current question
            var currentQuestion = GetCurrentQuestion();
            //Normalize user input by trimming and converting to uppercase
            string? userAnswerUpper = userAnswer?.Trim().ToUpper();

            //Validate that an answer was provided
            if (string.IsNullOrEmpty(userAnswerUpper))
            {
                messages.Add(new ChatMessage("Please provide an answer (e.g., 'A' for multiple-choice or 'True'/'False').\n", "Red"));
                messages.Add(new ChatMessage(DisplayCurrentQuestion()));
                return messages;
            }

            bool isCorrect;
            //Handle true/false questions
            if (currentQuestion.IsTrueFalse)
            {
                //Validate that the answer is either "True" or "False"
                if (userAnswerUpper != "TRUE" && userAnswerUpper != "FALSE")
                {
                    messages.Add(new ChatMessage("Please answer 'True' or 'False'.\n", "Red"));
                    messages.Add(new ChatMessage(DisplayCurrentQuestion()));
                    return messages;
                }
                isCorrect = userAnswerUpper == currentQuestion.CorrectAnswer.ToUpper();
            }
            //Handle multiple-choice questions
            else
            {
                //Validate that the answer is a valid option (A, B, C, or D)
                if (!new[] { "A", "B", "C", "D" }.Contains(userAnswerUpper))
                {
                    messages.Add(new ChatMessage("Please select an option (A, B, C, or D).\n", "Red"));
                    messages.Add(new ChatMessage(DisplayCurrentQuestion()));
                    return messages;
                }
                isCorrect = userAnswerUpper == currentQuestion.CorrectAnswer.ToUpper();
            }

            //Provide feedback based on whether the answer is correct
            string feedbackWord = isCorrect ? "Correct! " : "Incorrect! ";
            string feedbackColor = isCorrect ? "Green" : "Red";
            messages.Add(new ChatMessage(feedbackWord, feedbackColor));
            messages.Add(new ChatMessage(currentQuestion.Feedback));

            //Increment score if the answer is correct
            if (isCorrect) IncrementScore();

            //Check if there are more questions
            if (HasNextQuestion())
            {
                //Move to the next question and display it
                MoveToNextQuestion();
                messages.Add(new ChatMessage("Next question:"));
                messages.Add(new ChatMessage(DisplayCurrentQuestion()));
            }
            else
            {
                //End the quiz and display the final score
                int finalScore = GetQuizScore();
                int totalQuestions = GetTotalQuestions();
                string scoreFeedback = finalScore >= 8
                    ? "Great job! You’re a cybersecurity pro!"
                    : "Keep learning to stay safe online!";
                EndQuiz();
                messages.Add(new ChatMessage($"\nQuiz finished! Your score: {finalScore}/{totalQuestions}"));
                messages.Add(new ChatMessage(scoreFeedback));
                messages.Add(new ChatMessage("Type 'start quiz' to play again or ask another question.\n"));
            }

            return messages;
        }

        //Returns the current question formatted for display
        public string DisplayCurrentQuestion()
        {
            var question = GetCurrentQuestion();
            if (question.IsTrueFalse)
                return $"{question.Question}\nAnswer with 'True' or 'False'.\n";
            else
                return $"{question.Question}\n{string.Join("\n", question.Options!)}\nAnswer with A, B, C, or D.\n";
        }
        //-------------------------------------------------------------------------------------------------------------------//
    }
    //End Class
    //-------------------------------------------------------------------------------------//

}
//---------------------------------------End of Code-------------------------------------------------//