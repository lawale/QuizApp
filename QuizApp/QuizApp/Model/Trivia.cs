using System.Collections.Generic;
using QuizApp.Extension;

namespace QuizApp.Model
{
    public class Trivia
    {
        public int ResponseCode { get; set; }
        public IList<Result> Results { get; set; }
    }

    public class Result
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public string SelectedAnswer { get; set; }
        public IList<string> IncorrectAnswers { private get; set; }

        /// <summary>
        /// instantiate correct answer with other answerrs in a list.
        /// Shuffle list to randomize the position of Correct Answer
        /// </summary>
        public IList<string> Options
        {
            get
            {
                var list = new List<string>(IncorrectAnswers)
                {
                    CorrectAnswer
                };
                list.ShuffleList();
                return list;
            }
        }
    }
}
