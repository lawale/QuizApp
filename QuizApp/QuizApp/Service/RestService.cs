using System;
using System.Collections.Generic;
using QuizApp.Model;
using Xamarin.Forms;
using QuizApp.Service;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

[assembly: Dependency(typeof(RestService))]
namespace QuizApp.Service
{
    public class RestService : IRestService
    {
        #region Private Properties
        private readonly string Domain = "https://opentdb.com/api.php?";

        private readonly string Encoding = "&encode=base64";

        private IDictionary<string, object> Properties => Application.Current.Properties;

        private readonly Uri RetrieveTokenUri;

        private readonly Uri ResetTokenUri;
        
        private readonly DefaultContractResolver contractResolver;

        private readonly JsonSerializerSettings settings;
        #endregion

        #region Constructor
        public RestService()
        {
            RetrieveTokenUri = new Uri("https://opentdb.com/api_token.php?command=request");
            ResetTokenUri = new Uri($"https://opentdb.com/api_token.php?command=reset&token{Token}");

            contractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
            settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
        }
        #endregion

        #region IRestService Implementation
        public async Task<(bool isSuccess, string message)> RetrieveTriviaWithoutToken(string parameters)
        {
            var url = new Uri($"{Domain}{parameters}{Encoding}");
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            {
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var trivia = JsonConvert.DeserializeObject<Trivia>(json, settings);
                        switch (trivia.ResponseCode)
                        {
                            case 0:
                                Questions = trivia.Results;
                                break;
                            case 1:
                                throw new HttpRequestException("Could not return Questions. Reduce number of questions requested.");
                            case 2:
                            case 3:
                            case 4:
                                throw new HttpRequestException("An error occured");
                        }
                        return (true, string.Empty);
                    }
                    else
                        throw new HttpRequestException("An error occured");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return (false, ex.Message);
                }
            }
        }

        public async Task<(bool isSuccess, string message)> RetrieveTriviaWithToken(string parameters)
        {
            var url = $"{Domain}{parameters}{Encoding}&token={Token}";
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(new Uri(url)))
            {
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var trivia = JsonConvert.DeserializeObject<Trivia>(json, settings);
                        switch (trivia.ResponseCode)
                        {
                            case 0:
                                Questions = trivia.Results;
                                break;
                            case 1:
                                throw new HttpRequestException("Could not return Questions. Please reduce number of questions requested.");
                            case 2:
                                throw new HttpRequestException("An error occured");
                            case 3:
                                var _userResponse = await Application.Current.MainPage.DisplayAlert("Token does not exist", "Would you like to regenerate your token?", "Yes", "No");
                                if (_userResponse)
                                {
                                    var (resetIsSuccess, resetMessage) = await GetSessionToken();
                                    if (resetIsSuccess)
                                    {
                                        var _userNewResponse = await Application.Current.MainPage.DisplayAlert("Token Generated", "Would you like to continue question generation?", "Yes", "No");
                                        if (_userNewResponse)
                                            await RetrieveTriviaWithToken(parameters);
                                        else
                                            throw new Exception("You chose not to continue question generation!");
                                    }
                                }
                                else
                                    throw new Exception("You chose not to regenerate token!");
                                break;

                            case 4:
                                var userResponse = await Application.Current.MainPage.DisplayAlert("Token expired", "Would you like to reset your token?", "Yes", "No");
                                if (userResponse)
                                {
                                    var (resetIsSuccess, resetMessage) = await ResetToken();
                                    if (resetIsSuccess)
                                    {
                                        var userNewResponse = await Application.Current.MainPage.DisplayAlert("Token Generated", "Would you like to continue question generation?", "Yes", "No");
                                        if (userNewResponse)
                                            await RetrieveTriviaWithToken(parameters);
                                        else
                                            throw new Exception("You chose not to continue question generation!");
                                    }
                                }
                                else
                                    throw new Exception("You chose not to reset token!");
                                break;
                        }
                        return (true, string.Empty);
                    }
                    else
                        throw new HttpRequestException("An error occured.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return (false, ex.Message);
                }
            }
        }

        public IList<Result> Questions { get; private set; }

        public async Task<(bool isSuccess, string message)> GetSessionToken()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(RetrieveTokenUri))
                {
                    if(response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var tokenClass = JsonConvert.DeserializeObject<TokenClass>(json, settings);
                        if (tokenClass.ResponseCode == 0)
                        {
                            if (Properties.ContainsKey("Token"))
                                Properties["Token"] = tokenClass.Token;
                            else
                                Properties.Add("Token", tokenClass.Token);
                            await Application.Current.SavePropertiesAsync();
                            Console.WriteLine(tokenClass.ResponseMessage);
                            return (true, string.Empty);
                        }
                        else
                            throw new HttpRequestException(tokenClass.ResponseMessage);
                    }
                    else
                    {
                        throw new HttpRequestException("An errror occured.");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, ex.Message);
            }
        }

        public string Token
        {
            get
            {
                if (Properties != null)
                    return Properties.ContainsKey("Token") ? Properties["Token"].ToString() : string.Empty;
                else
                    return string.Empty;
            }
        }

        public async Task<(bool isSuccess, string message)> ResetToken()
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(ResetTokenUri))
            {
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var tokenClass = JsonConvert.DeserializeObject<TokenClass>(json, settings);
                        if (tokenClass.ResponseCode == 0)
                        {
                            if (Properties.ContainsKey("Tokens"))
                                Properties["Token"] = tokenClass.Token;
                            else
                                Properties.Add("Token", tokenClass.Token);
                            await Application.Current.SavePropertiesAsync();
                            return (true, string.Empty);
                        }
                        else
                            throw new HttpRequestException("Could not reset token");
                    }
                    else
                        throw new HttpRequestException("An error occured.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return (false, ex.Message);
                }
            }
        }
        #endregion
    }
}