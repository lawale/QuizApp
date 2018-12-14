using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using QuizApp.Model;

namespace QuizApp.Service
{
    public interface IRestService
    {
        Task<(bool isSuccess, string message)> RetrieveTriviaWithToken(string parameters);
        Task<(bool isSuccess, string message)> RetrieveTriviaWithoutToken(string parameters);
        Task<(bool isSuccess, string message)> GetSessionToken();
        Task<(bool isSuccess, string message)> ResetToken();
        string Token { get; }
        IList<Result> Questions { get; }
    }
}
