using MauiApp1.Interfaces;
using System.Net;
using Microsoft.Maui.Networking;
using NetworkAccess = Microsoft.Maui.Networking.NetworkAccess;
using Newtonsoft.Json;

namespace MauiApp1.Services
{
    public class ApiManager : IApiManager
    {
        private string baseUrl = "";
        HttpClient _client = null;
        private readonly HttpClientHandler _clientHandler;
        private readonly CookieContainer _cookieContainer;

        public ApiManager()
        {
            _cookieContainer = new CookieContainer();
            _clientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            _client = new HttpClient(_clientHandler);
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<T> PostForResponseAsync<T>(string uriRequest, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            T response = default(T);

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == NetworkAccess.Internet)
            {
                var content = new FormUrlEncodedContent(parameters);
                HttpResponseMessage httpResponse = await _client.PostAsync(uriRequest, content, cancellationToken);


                if (httpResponse.IsSuccessStatusCode)
                {
                    var res = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<T>(res);
                }
            }
            else
            {
                throw new Exception();
            }

            return response;
        }
    }
}
