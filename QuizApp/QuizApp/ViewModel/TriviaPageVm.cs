using QuizApp.Extension;
using System;
using System.Collections.Generic;
using QuizApp.Model;
using Xamarin.Forms;
using QuizApp.Service;
using System.Windows.Input;
using System.Threading.Tasks;

namespace QuizApp.ViewModel
{
    public class TriviaPageVm
    {
        public IList<TrivialVm> Questions { get; }
        public ICommand SelectAnswer { get; }
        public ICommand Submit { get; }
        private Dictionary<TrivialVm, string> Answers;

        public TriviaPageVm()
        {
            Answers = new Dictionary<TrivialVm, string>();
            Questions = DependencyService.Get<IRestService>().Questions.ChangeModel(TrivialVm.ConvertFrom);
            Submit = new Command(async () => await ComputeScore());
            SelectAnswer = new Command<(TrivialVm, string)>(AnswerSelected);
        }

        private async Task ComputeScore()
        {
            if (!await Application.Current.MainPage.DisplayAlert("Submit?", $"Are you sure you want to submit?", "Yes", "No"))
                return;
            int score = 0;
            foreach(var answer in Answers)
            {
                if (answer.Key.CorrectAnswer == answer.Value)
                    score++;
            }
            await Application.Current.MainPage.DisplayAlert("Score", $"You have scored {score}", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        
        private void AnswerSelected( (TrivialVm, string) input )
        {
            var (key, value) = input;
            if (Answers.ContainsKey(key))
                Answers[key] = value;
            else
                Answers.Add(key, value);
        }

    }
}
