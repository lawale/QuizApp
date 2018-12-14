using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Plugin.InputKit.Shared.Controls;
using System.Threading.Tasks;
using Xamarin.Forms;
using QuizApp.Service;
using QuizApp.ViewModel;

namespace QuizApp.View
{
    public partial class MainPage : ContentPage
    {
        private string NoQ;
        private string Cat;
        private string Diff;
        private string Type;
        private IRestService restService = DependencyService.Get<IRestService>();
        private INavigationService navigationService = DependencyService.Get<INavigationService>();
        private bool UseToken => box.IsChecked;

        public MainPage()
        {
            InitializeComponent();
            category.ItemsSource = Categories.Keys.ToList();
            category.SelectedIndex = 0;
            difficulty.SelectedIndex = 0;
            type.SelectedIndex = 0;
        }

        private static Dictionary<string, int> Categories;
        static MainPage()
        {
            Categories = new Dictionary<string, int>
            {
                { "Any Category", 0 },
                { "General Knowledge", 9 },
                { "Entertainment: Books", 10 },
                { "Entertainment: Film", 11 },
                { "Entertainment: Music", 12 },
                { "Entertainment: Musicals & Theatres", 13 },
                { "Entertainment: Television", 14 },
                { "Entertainment: Video Games", 15 },
                { "Entertainment: Board Games", 16 },
                { "Science & Nature", 17 },
                { "Science: Computers", 18 },
                { "Science: Mathematics", 19 },
                { "Mythology", 20 },
                { "Sports", 21 },
                { "Geography", 22 },
                { "History", 23 },
                { "Politics", 24 },
                { "Art", 25 },
                { "Celebrities", 26 },
                { "Animals", 27 },
                { "Vehicles", 28 },
                { "Entertainment: Comics", 29 },
                { "Science: Gadgets", 30 },
                { "Entertainment: Japanese Anime & Manga", 31 },
                { "Entertainment: Cartoon & Animations", 32 }
            };
        }

        private async void GenerateTrivia(object sender, EventArgs e)
        {
            NoQ = $"amount={(int)questions.Value}";
            Cat = category.SelectedIndex > 0 ? $"&category={Categories[(string)category.SelectedItem]}" : string.Empty;
            Diff = difficulty.SelectedIndex > 0 ? $"&difficulty={(string)difficulty.SelectedItem}".ToLower() : string.Empty;
            Type = type.SelectedIndex > 0 ? (type.SelectedIndex == 1 ? "&type=multiple" : "&type=boolean") : string.Empty;
            var parameter = $"{NoQ}{Cat}{Diff}{Type}";
            parameters.IsVisible = false;
            question.IsVisible = true;
            try
            {
                if (UseToken)
                {
                    var (isSuccess, message) = await restService.RetrieveTriviaWithToken(parameter);
                    if (isSuccess)
                    {
                        await navigationService.PushPage(typeof(TriviaPage));
                        question.IsVisible = false;
                        parameters.IsVisible = true;
                    }
                    else
                    {
                        question.IsVisible = false;
                        parameters.IsVisible = true;
                        var userResponse = await DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");

                        if (userResponse)
                            TokenEvent(box, new EventArgs());
                        else
                            box.IsChecked = false;
                    }
                }
                else
                {
                    var (isSuccess, message) = await restService.RetrieveTriviaWithoutToken(parameter);

                    if (isSuccess)
                    {
                        await navigationService.PushPage(typeof(TriviaPage));
                        question.IsVisible = false;
                        parameters.IsVisible = true;
                    }
                    else
                    {
                        question.IsVisible = false;
                        parameters.IsVisible = true;
                        var userResponse = await DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");
                        if (userResponse)
                            GenerateTrivia(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                question.IsVisible = false;
                parameters.IsVisible = true;
                await DisplayAlert("Error", "An error has occurred!", "Ok");
            }
        }

        private async void TokenEvent(object sender, EventArgs e)
        {
            if (box.IsChecked)
            {
                try
                {
                    var response = await DisplayAlert("Token", "If token does not exist or has expired, it will have to be regenerated. Are you sure you want to continue?.", "Yes", "No");
                    if (response && string.IsNullOrEmpty(restService.Token))
                    {
                        parameters.IsVisible = false;
                        token.IsVisible = true;
                        var (isSuccess, message) = await restService.GetSessionToken();
                        token.IsVisible = false;
                        parameters.IsVisible = true;
                        if (isSuccess)
                            await DisplayAlert("Success", "Token Successfully generated", "OK");
                        else
                        {
                            var userResponse = await DisplayAlert("Error", $"{message}. Do you want to retry", "Yes", "No");
                            if (userResponse)
                                TokenEvent(box, new EventArgs());
                            else
                                box.IsChecked = false;
                        }
                    }
                    else if (string.IsNullOrEmpty(restService.Token))
                        box.IsChecked = false;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    await DisplayAlert("Error", "An error occured!", "Ok");
                }
            }
        }
    }
}
