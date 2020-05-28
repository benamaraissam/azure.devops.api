using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Devops.Api.Legacy
{
    public abstract class RestRequester
    {
        private readonly RestClient _client;
        protected abstract string Url { get; }
        public RestRequester()
        {
            _client = new RestClient(Url);
            _client.Authenticator = new HttpBasicAuthenticator(string.Empty, Credentials.TOKEN);
        }

        protected async Task<T> Get<T>(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.GET);
            var response = await _client.ExecuteAsync<T>(request);
            return response.Data;
        }


    }
}
