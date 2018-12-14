using QuizApp.Extension;
using QuizApp.Model;
using static System.Text.Encoding;

namespace QuizApp.ViewModel
{
    public class TrivialVm
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public int Position { get; set; }
        public bool IsMultiple { get; private set; }

        /// <summary>
        /// Factory Method To Create a new TriviaVm from a Trivia Result
        /// </summary>
        /// <param name="result">The Trivia Result Argument to be Converted</param>
        /// <param name="index">The Question Number</param>
        /// <returns>new TriviaVm instance</returns>
        public static TrivialVm ConvertFrom(Result result, int index) => new TrivialVm(result, index);
        
        private TrivialVm(Result trivia, int index)
        {
            Category = EncoderExtension.Base64ToString(UTF8, trivia.Category);
            Type = EncoderExtension.Base64ToString(UTF8, trivia.Type);
            Difficulty = EncoderExtension.Base64ToString(UTF8, trivia.Difficulty).ToUpper();
            Question = EncoderExtension.Base64ToString(UTF8, trivia.Question);
            CorrectAnswer = EncoderExtension.Base64ToString(UTF8, trivia.CorrectAnswer);
            Position = index;
            var options = trivia.Options;
            if (Type.ToLower() == "boolean")
            {
                IsMultiple = false;
                OptionA = EncoderExtension.Base64ToString(UTF8, options[0]);
                OptionB = EncoderExtension.Base64ToString(UTF8, options[1]);
            }
            else
            {
                IsMultiple = true;
                OptionA = EncoderExtension.Base64ToString(UTF8, options[0]);
                OptionB = EncoderExtension.Base64ToString(UTF8, options[1]);
                OptionC = EncoderExtension.Base64ToString(UTF8, options[2]);
                OptionD = EncoderExtension.Base64ToString(UTF8, options[3]);
            }
        }
    }
}
