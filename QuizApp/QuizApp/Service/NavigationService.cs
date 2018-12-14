using System;
using System.Collections.Generic;
using System.Text;
using QuizApp.Service;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(NavigationService))]
namespace QuizApp.Service
{
    public class NavigationService : INavigationService
    {
        private Page MainPage => Application.Current.MainPage;

        public async Task DisplayAlert(string Title, string Message, string Cancel) =>
            await MainPage.DisplayAlert(Title, Message, Cancel);

        public async Task<bool> DisplayAlert(string Title, string Message, string Accept, string Cancel) => 
            await MainPage.DisplayAlert(Title, Message, Accept, Cancel);

        public async Task PopPage() => await MainPage.Navigation.PopAsync();

        public async Task PushPage(Type PageType) =>
            await MainPage.Navigation.PushAsync((Page)Activator.CreateInstance(PageType));
    }
}
