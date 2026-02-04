using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace Azure.Devops.Api.Legacy
{
    public abstract class RestRequester
    {
        private readonly RestClient _client;
        protected abstract string Url { get; }

        // Retry configuration
        private const int MAX_RETRIES = 3;
        private static readonly int[] TRANSIENT_HTTP_STATUS_CODES = {
            (int)HttpStatusCode.BadRequest,      // 400 - Not transient, but included for completeness
            (int)HttpStatusCode.Unauthorized,    // 401 - Not transient
            (int)HttpStatusCode.Forbidden,       // 403 - Not transient
            (int)HttpStatusCode.NotFound,        // 404 - Not transient
            (int)HttpStatusCode.TooManyRequests, // 429 - Rate limit (transient)
            (int)HttpStatusCode.BadGateway,      // 502 - Bad gateway (transient)
            (int)HttpStatusCode.ServiceUnavailable, // 503 - Service unavailable (transient)
            (int)HttpStatusCode.GatewayTimeout   // 504 - Gateway timeout (transient)
        };

        public RestRequester()
        {
            _client = new RestClient(Url);
            _client.Authenticator = new HttpBasicAuthenticator(string.Empty, Credentials.TOKEN);
        }

        protected async Task<T> Get<T>(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.GET);
            return await ExecuteWithRetry<T>(request);
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
            return await ExecuteWithRetry<T>(request);
        }

        private async Task<T> ExecuteWithRetry<T>(RestRequest request)
        {
            Exception lastException = null;

            for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
            {
                try
                {
                    var response = await _client.ExecuteAsync<T>(request);

                    // Check if response has a transient error status code
                    if (response.StatusCode != null &&
                        TRANSIENT_HTTP_STATUS_CODES.Contains((int)response.StatusCode) &&
                        attempt < MAX_RETRIES)
                    {
                        LogRetryAttempt(attempt, response.StatusCode.ToString(), response.ErrorMessage);
                        await WaitBeforeRetry(attempt);
                        continue;
                    }

                    // If we get here, the request was successful or we got a non-transient error
                    if (response.StatusCode == null || !TRANSIENT_HTTP_STATUS_CODES.Contains((int)response.StatusCode))
                    {
                        if (response.ErrorException != null)
                        {
                            throw response.ErrorException;
                        }
                        return response.Data;
                    }

                    // If we've reached max retries and still have a transient error, throw
                    throw new HttpRequestException($"Failed after {MAX_RETRIES} attempts with status code: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    // Check if this is a transient error that should be retried
                    if (IsTransientException(ex) && attempt < MAX_RETRIES)
                    {
                        LogRetryAttempt(attempt, "Exception", ex.Message);
                        await WaitBeforeRetry(attempt);
                        continue;
                    }

                    // If not a transient exception or we've reached max retries, rethrow
                    throw;
                }
            }

            // This should never be reached due to the throw above, but included for completeness
            throw lastException ?? new InvalidOperationException("Unexpected error in retry logic");
        }

        private bool IsTransientException(Exception ex)
        {
            // Check for HTTP status codes that indicate transient failures
            if (ex is HttpRequestException httpRequestException)
            {
                // Check if it's a specific HTTP status code
                if (httpRequestException.StatusCode.HasValue)
                {
                    return TRANSIENT_HTTP_STATUS_CODES.Contains((int)httpRequestException.StatusCode.Value);
                }

                // Check for network-related exceptions
                if (httpRequestException.Message.Contains("connection") ||
                    httpRequestException.Message.Contains("timeout") ||
                    httpRequestException.Message.Contains("failed"))
                {
                    return true;
                }
            }

            // Check for other common transient exceptions
            if (ex is TaskCanceledException ||
                ex is TimeoutException ||
                ex is System.Net.Sockets.SocketException)
            {
                return true;
            }

            return false;
        }

        private async Task WaitBeforeRetry(int attempt)
        {
            // Exponential backoff: 2^(attempt-1) seconds, with some jitter
            // Base delay: 1 second, then 2, 4 seconds
            int baseDelay = 1000; // 1 second in milliseconds
            int delay = baseDelay * (int)Math.Pow(2, attempt - 1);

            // Add random jitter (up to 50% of the delay) to avoid thundering herd
            var random = new Random();
            int jitter = random.Next(0, (int)(delay * 0.5));
            int totalDelay = delay + jitter;

            await Task.Delay(totalDelay);
        }

        private void LogRetryAttempt(int attempt, string statusCode, string message)
        {
            // In a real implementation, you would use a proper logging framework
            // For now, we'll use Console.WriteLine for demonstration
            Console.WriteLine($"Retry attempt {attempt}/{MAX_RETRIES}: Status={statusCode}, Error={message}");
        }

        protected async Task Delete<T>(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.DELETE);
            await ExecuteWithRetry(request);
        }
    }
}
