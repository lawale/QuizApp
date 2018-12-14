using System;
using System.Threading.Tasks;

namespace QuizApp.Service
{
    public interface INavigationService
    {
        Task DisplayAlert(string Title, string Message, string Cancel);

        Task<bool> DisplayAlert(string Title, string Message, string Accept, string Cancel);

        Task PopPage();

        Task PushPage(Type PageType);

    }
}
