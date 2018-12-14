using System;
using Plugin.InputKit.Shared.Controls;
using QuizApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TriviaPage : ContentPage
    {
        private bool Finish = false;
        private new TriviaPageVm BindingContext => base.BindingContext as TriviaPageVm;
        public TriviaPage()
        {
            InitializeComponent();
            base.BindingContext = new TriviaPageVm();
        }

        private void AnswerSelected(object sender, EventArgs args)
        {
            var radio = sender as RadioButton;
            var param = radio.CommandParameter as TrivialVm;
            var answer = radio.Text as string;
            BindingContext.SelectAnswer.Execute((param, answer));
        }

        protected override bool OnBackButtonPressed()
        {
            Quit();
            return true;
        }

        private void Quit()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var response = await DisplayAlert("", "Do you want to stop the test?", "Yes", "No");
                if (response)
                {
                    BindingContext.Submit.Execute(null);
                    base.OnBackButtonPressed();
                }
            });
        }
    }
}