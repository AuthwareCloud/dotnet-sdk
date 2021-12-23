using System;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Authware.Exceptions;
using Authware.Models;
using Newtonsoft.Json;

namespace Authware
{
    internal class Requester
    {
        /// <summary>
        ///     This is the <see cref="HttpClient" /> the <see cref="Requester" /> uses to make HTTP Requests
        /// </summary>
        public readonly HttpClient Client;

        /// <summary>
        ///     This is for internal use,
        ///     it sets the HttpClient to be a base url for api.authware.org and adds certificate validation to prevent forgery
        /// </summary>
        internal Requester()
        {
            Client = new HttpClient(new HttpClientHandler
            {
                UseProxy = false,
                Proxy = null,
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) =>
                    certificate2.IssuerName.Name is "CN=Cloudflare Inc ECC CA-3, O=Cloudflare, Inc., C=US"
                        or "CN=R3, O=Let's Encrypt, C=US"
            })
            {
                BaseAddress = new Uri("https://api.authware.org")
            };
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Authware-App-Version",
                Assembly.GetEntryAssembly()?.GetName().Version.ToString());
        }

        /// <summary>
        ///     Makes a new instance of the Requester class which holds an <see cref="HttpClient" /> and wraps around it giving you
        ///     easier access make requests and parse the responses to C# models
        /// </summary>
        /// <param name="client">The <see cref="HttpClient" /> to use for making HTTP requests</param>
        public Requester(HttpClient client)
        {
            Client = client;
        }

        /// <summary>
        ///     Makes an http request using the <see cref="HttpClient" /> you passed into the constructor and automatically parses
        ///     the response to the class or struct specified
        ///     in the generic parameter to this class
        /// </summary>
        /// <param name="method">The HTTP method you want to make the request with</param>
        /// <param name="url">The URL you want to request data from</param>
        /// <param name="postData">Any data you want to post in the JSON body of the request</param>
        /// <typeparam name="T">The type you want to deserialize from JSON to</typeparam>
        /// <returns>The parsed class you specified in the generic parameter</returns>
        /// <exception cref="AuthwareException">
        ///     If the endpoint was an authware endpoint it will return this with the <see cref="ErrorResponse" />
        ///     parsed
        /// </exception>
        /// <exception cref="Exception">
        ///     Returns an exception if the API returned JSON either not able to be parsed back to the class specified
        ///     or there was an error with the request
        /// </exception>
        /// <remarks>
        ///     This class is meant to be used with the Authware wrapper it is only exposed for ease of use for users of the
        ///     wrapper.
        ///     It is discouraged to use this to make requests as the exceptions it throws does specify Authware issues
        /// </remarks>
        public async Task<T> Request<T>(HttpMethod method, string url, object postData)
        {
            using var request = new HttpRequestMessage(method, url);
            request.Headers.TryAddWithoutValidation("X-Request-DateTime",
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            if (postData != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8,
                    "application/json");

            using var response = await Client.SendAsync(request).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<T>(content);

            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                throw new AuthwareException(errorResponse);
            }
            catch (NullReferenceException e)
            {
                throw new Exception(
                    "A non success status code was returned from the Authware API. " +
                    "While attempting to parse an error response from the Authware API another exception occured", e);
            }
            catch (JsonReaderException e)
            {
                throw new Exception(
                    "A non success status code was returned from the Authware API. " +
                    "While attempting to parse an error response from the Authware API another exception occured", e);
            }
        }
    }
}