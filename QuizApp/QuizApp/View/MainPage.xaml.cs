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
        private new MainPageVm BindingContext => base.BindingContext as MainPageVm;
        public MainPage()
        {
            InitializeComponent();
            category.ItemsSource = Categories.Keys.ToList();
            category.SelectedIndex = 0;
            difficulty.SelectedIndex = 0;
            type.SelectedIndex = 0;
            base.BindingContext = new MainPageVm();
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

        private void GenerateTrivia(object sender, EventArgs e)
        {
            NoQ = $"amount={(int)questions.Value}";
            Cat = category.SelectedIndex > 0 ? $"&category={Categories[(string)category.SelectedItem]}" : string.Empty;
            Diff = difficulty.SelectedIndex > 0 ? $"&difficulty={(string)difficulty.SelectedItem}".ToLower() : string.Empty;
            Type = type.SelectedIndex > 0 ? (type.SelectedIndex == 1 ? "&type=multiple" : "&type=boolean") : string.Empty;
            var parameter = $"{NoQ}{Cat}{Diff}{Type}";
            BindingContext.GenerateTrivia.Execute(parameter);
        }
        
    }
}
