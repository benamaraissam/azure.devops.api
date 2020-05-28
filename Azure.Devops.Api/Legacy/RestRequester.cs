using Newtonsoft.Json;
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
            return await Execute<T>(request);
        }

        protected async Task<T> Insert<T>(string endpoint, object obj)
        {
            var request = new RestRequest(endpoint, Method.PUT);
            return await InsertOrUpdate<T>(obj, request);
        }

        protected async Task<T> Put<T>(string endpoint, object obj)
        {
            var request = new RestRequest(endpoint, Method.PUT);
            return await InsertOrUpdate<T>(obj, request);
        }

        private async Task<T> InsertOrUpdate<T>(object obj, RestRequest request)
        {
            request.AddParameter("application/json", JsonConvert.SerializeObject(obj), ParameterType.RequestBody);
            return await Execute<T>(request);
        }

        private async Task<T> Execute<T>(RestRequest request)
        {
            var response = await _client.ExecuteAsync<T>(request);
            return response.Data;
        }

        protected async Task Delete<T>(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.DELETE);
            await _client.ExecuteAsync(request);
        }



    }
}
