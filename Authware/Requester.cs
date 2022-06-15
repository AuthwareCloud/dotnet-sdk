using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Authware.Exceptions;
using Authware.Models;
using Newtonsoft.Json;

namespace Authware;

internal class Requester
{
    /// <summary>
    ///     This is the <see cref="HttpClient" /> the <see cref="Requester" /> uses to make HTTP Requests
    /// </summary>
    internal readonly HttpClient Client;

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
            ServerCertificateCustomValidationCallback = (_, certificate2, _, _) => true
            // certificate2.IssuerName.Name!.Contains("CN=Cloudflare Inc ECC CA-3, O=Cloudflare, Inc., C=US") ||
            // certificate2.IssuerName.Name.Contains(", O=Let's Encrypt, C=US")
        })
        {
#if DEBUG
            BaseAddress = new Uri("http://localhost:44303/")
#else
            BaseAddress = new Uri("https://api.authware.org/")
#endif
        };
        Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Authware-App-Version",
            Assembly.GetEntryAssembly()?.GetName().Version.ToString());
        Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Authware-DotNet",
            Assembly.GetAssembly(typeof(AuthwareApplication)).GetName().Version.ToString()));
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
    internal async Task<T> Request<T>(HttpMethod method, string url, object? postData)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.TryAddWithoutValidation("X-Request-DateTime",
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        if (postData is not null)
            request.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8,
                "application/json");

        using var response = await Client.SendAsync(request).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(content)!;
        }
        catch (JsonReaderException e)
        {
            throw new Exception("There was an error when parsing the response from the Authware API. \n" +
                                "The code returned from the API was a success status code. \n" +
                                "API responses most likely changed and you need to update the the wrapper to fix this error. \n" +
                                "The response from the API was. \n" +
                                $"{content} \n", e);
        }

        try
        {
            if ((int) response.StatusCode == 429)
            {
                var retryAfter = response.Headers.RetryAfter.Delta!.Value;
                if (content.Contains("<")) throw new RateLimitException(null, retryAfter);

                throw new RateLimitException(JsonConvert.DeserializeObject<ErrorResponse>(content), retryAfter);
            }

            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
            if (errorResponse?.Code == BaseResponse.ResponseStatus.UpdateRequired)
                throw new UpdateRequiredException(response.Headers.GetValues("X-Authware-Updater-URL").First(),
                    errorResponse);
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