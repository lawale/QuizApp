using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using QuizApp.Service;
using System.ComponentModel;
using QuizApp.View;

namespace QuizApp.ViewModel
{
    public class MainPageVm : INotifyPropertyChanged
    {
        public ICommand GetToken;
        public ICommand GenerateTrivia;
        public bool ParameterIsVisible { get; private set; } = true;
        public bool TokenIsVisible { get; set; } = false;
        public bool QuestionIsVisble { get; set; } = false;
        public bool TokenIsChecked { get; set; } = false;
        private IRestService restService = DependencyService.Get<IRestService>();
        private INavigationService navigationService = DependencyService.Get<INavigationService>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageVm()
        {
            GetToken = new Command(async () => await GetTokenMethod());
            GenerateTrivia = new Command<string>(async param => await GenerateTriviaMethod(param));
            
        }

        private async Task GetTokenMethod()
        {
            try
            {
                var response = await navigationService.DisplayAlert("Token", "If token does not exist or has expired, it will have to be regenerated. Are you sure you want to continue?.", "Yes", "No");
                if (response && string.IsNullOrEmpty(restService.Token))
                {
                    ParameterIsVisible = false;
                    TokenIsVisible = true;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenIsVisible)));

                    var (isSuccess, message) = await restService.GetSessionToken();

                    ParameterIsVisible = true;
                    TokenIsVisible = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenIsVisible)));

                    if (isSuccess)
                        await navigationService.DisplayAlert("Success", "Token Successfully generated", "OK");
                    else
                    {
                        var userResponse = await navigationService.DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");
                        if (userResponse)
                            await GetTokenMethod();
                        else
                        {
                            TokenIsChecked = false;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenIsChecked)));
                        }
                    }
                }
                else if (string.IsNullOrEmpty(restService.Token))
                {
                    TokenIsChecked = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenIsChecked)));
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                await navigationService.DisplayAlert("Error", "An error occured!", "Ok");
            }
        }

        private async Task GenerateTriviaMethod(string parameter)
        {
            ParameterIsVisible = false;
            QuestionIsVisble = true;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));
            try
            {
                if (TokenIsChecked)
                {
                    var (isSuccess, message) = await restService.RetrieveTriviaWithToken(parameter);
                    if (isSuccess)
                    {
                        await navigationService.PushPage(typeof(TriviaPage));
                        ParameterIsVisible = true;
                        QuestionIsVisble = false;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));

                    }
                    else
                    {
                        ParameterIsVisible = true;
                        QuestionIsVisble = false;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));

                        var userResponse = await navigationService.DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");

                        if (userResponse)
                            await GenerateTriviaMethod(parameter);
                        else
                        {
                            TokenIsChecked = false;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TokenIsChecked)));
                        }
                    }
                }
                else
                {
                    var (isSuccess, message) = await restService.RetrieveTriviaWithoutToken(parameter);

                    if (isSuccess)
                    {
                        await navigationService.PushPage(typeof(TriviaPage));
                        ParameterIsVisible = true;
                        QuestionIsVisble = false;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));
                    }
                    else
                    {
                        ParameterIsVisible = true;
                        QuestionIsVisble = false;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));
                        var userResponse = await navigationService.DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");
                        if (userResponse)
                            await GenerateTriviaMethod(parameter);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                ParameterIsVisible = true;
                QuestionIsVisble = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParameterIsVisible)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionIsVisble)));
                await navigationService.DisplayAlert("Error", "An error has occurred!", "Ok");
            }
        }
    }
}
