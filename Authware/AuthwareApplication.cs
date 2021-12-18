using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Authware.Exceptions;
using Authware.Models;

namespace Authware
{
    /// <summary>
    ///     The main Authware application class, this contains all of the functions that can be used to interact with the
    ///     Authware API
    /// </summary>
    public class AuthwareApplication
    {
        /// <summary>
        ///     This is used to facilitate HTTP requests for this class
        /// </summary>
        private readonly Requester _requester = new();

        /// <summary>
        ///     The ID of the current application
        /// </summary>
        private string _applicationId;

        /// <summary>
        ///     The path that the current users' authentication token is located
        /// </summary>
        private string _authTokenPath;

        /// <summary>
        ///     Initializes and checks the ID passed in against the Authware API to make sure the application is properly setup and
        ///     enabled
        /// </summary>
        /// <param name="applicationId">The ID of your application, this can be fetched on the manage application page</param>
        /// <returns>The application name, version and creation date represented by a <see cref="Application" /></returns>
        /// <exception cref="ArgumentNullException">Thrown if the application ID is null</exception>
        /// <exception cref="ArgumentException">Thrown if the application ID is not a valid GUID</exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the application does not exist under the provided ID
        /// </exception>
        public async Task<Application> InitializeApplicationAsync(string applicationId)
        {
            var _ = applicationId ?? throw new ArgumentNullException($"{nameof(applicationId)} can not be null");
            if (!Guid.TryParse(applicationId, out var _))
                throw new ArgumentException($"{applicationId} is invalid");

            _applicationId = applicationId;
            var applicationResponse = await _requester
                .Request<Application>(HttpMethod.Post, "/app", new {app_id = applicationId}).ConfigureAwait(false);
            _authTokenPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Authware",
                applicationId, "authtoken.bin");
            if (!Directory.Exists(Path.GetDirectoryName(_authTokenPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_authTokenPath) ?? string.Empty);

            return applicationResponse;
        }

        /// <summary>
        ///     Gets all application variables that the user has permission to get
        /// </summary>
        /// <param name="authenticated">
        ///     Whether you want to pull authentication required variables or not, an
        ///     <see cref="AuthwareException" /> will be thrown if this is true and no user is currently authenticated
        /// </param>
        /// <returns>An array of keys and values represented by a <see cref="Variable" /></returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application id is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the application is disabled or you attempted to fetch authenticated variables whilst not being
        ///     authenticated
        /// </exception>
        public async Task<Variable[]> GrabApplicationVariablesAsync(bool authenticated = false)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            if (authenticated)
            {
                var authenticatedVariables = await _requester
                    .Request<Variable[]>(HttpMethod.Get, "/user/variables", null)
                    .ConfigureAwait(false);
                return authenticatedVariables;
            }

            var variables = await _requester
                .Request<Variable[]>(HttpMethod.Post, "/user/variables", new {app_id = _applicationId})
                .ConfigureAwait(false);
            return variables;
        }

        /// <summary>
        ///     Allows a user to create an account on your application, provided if the username, password, email and license are
        ///     valid.
        /// </summary>
        /// <param name="username">The username the user will login with</param>
        /// <param name="password">The password the user should need to login with</param>
        /// <param name="email">The email the user will have assigned getting alerts and other things at</param>
        /// <param name="token">The license/token you want to register the user with</param>
        /// <returns>The base response containing the code and status message</returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application ID is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="ArgumentNullException">If any of the parameters are null this is what gets thrown</exception>
        /// <exception cref="ArgumentException">
        ///     This gets thrown if the license you want to use to register the user with is
        ///     invalid
        /// </exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the data provided is not acceptable by the Authware API, the license
        ///     was not valid or the application is disabled
        /// </exception>
        public async Task<BaseResponse> RegisterAsync(string username, string password, string email, string token)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            _ = username ?? throw new ArgumentNullException($"{nameof(username)} can not be null");
            _ = password ?? throw new ArgumentNullException($"{nameof(password)} can not be null");
            _ = email ?? throw new ArgumentNullException($"{nameof(email)} can not be null");
            _ = token ?? throw new ArgumentNullException($"{nameof(token)} can not be null");
            if (!Guid.TryParse(token, out var _))
                throw new ArgumentException($"{nameof(token)} is invalid");

            var response = await _requester
                .Request<BaseResponse>(HttpMethod.Post, "/user/register",
                    new {app_id = _applicationId, username, password, email_address = email, token})
                .ConfigureAwait(false);
            return response;
        }

        // I don't like how this method looks split it up maybe? possibly make a separate method for authenticating with token to get the profile? 
        /// <summary>
        ///     Authenticates a user against the Authware API with the provided username and password and caches the Authware
        ///     authentication token if successful
        ///     If the user has logged in before it will check the cached Authware authentication token and if the token is invalid
        ///     it will authenticate with the username and password
        /// </summary>
        /// <param name="username">The username you want to authenticate with</param>
        /// <param name="password">The password you want to authenticate with</param>
        /// <returns>The authenticated users profile, represented as <see cref="Profile" /></returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application ID is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="ArgumentNullException">If the username or password is null this exception is thrown</exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the data provided is not acceptable by the Authware API, the hardware ID did not match (if enabled), the
        ///     application version is out-of-date (if enabled) or the username and password are invalid
        /// </exception>
        public async Task<Profile> LoginAsync(string username, string password)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            _ = username ?? throw new ArgumentNullException($"{nameof(username)} can not be null");
            _ = password ?? throw new ArgumentNullException($"{nameof(password)} can not be null");
            _requester.Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Authware-Hardware-ID",
                Identifiers.GetIdentifier());
            if (File.Exists(_authTokenPath))
            {
                var authToken = File.ReadAllText(_authTokenPath);
                _requester.Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken);
                try
                {
                    return
                        await _requester.Request<Profile>(HttpMethod.Get, "user/profile", null).ConfigureAwait(false);
                }
                catch
                {
                    // ignored will be thrown below if it fails for whatever reason but still lets delete the bad auth token
                    File.Delete(_authTokenPath);
                }
            }

            var authResponse = await _requester
                .Request<AuthResponse>(HttpMethod.Post, "/user/auth", new {app_id = _applicationId, username, password})
                .ConfigureAwait(false);
            _requester.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authResponse.AuthToken);
            var profileResponse =
                await _requester.Request<Profile>(HttpMethod.Get, "user/profile", null).ConfigureAwait(false);
            File.WriteAllText(_authTokenPath, authResponse.AuthToken);
            return profileResponse;
        }

        /// <summary>
        ///     Allows a user to change their email on your application to a new email
        /// </summary>
        /// <param name="password">The user's current password</param>
        /// <param name="email">The email the user wants to change their email to</param>
        /// <returns>A <see cref="BaseResponse" /> containing the code and the message returned from the authware api</returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application id is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="ArgumentNullException">Throws if either password or email is null</exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the data provided is not acceptable by the Authware API, the hardware ID did not match (if enabled), the
        ///     application version is out-of-date (if enabled) or the password is invalid
        /// </exception>
        public async Task<BaseResponse> ChangeEmailAsync(string password, string email)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            _ = password ?? throw new ArgumentNullException($"{nameof(password)} can not be null");
            _ = email ?? throw new ArgumentNullException($"{nameof(email)} can not be null");

            var response = await _requester
                .Request<BaseResponse>(HttpMethod.Put, "/user/change-email",
                    new {password, new_email_address = email})
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        ///     Allows a user to change their current password to the password specified in newPassword
        /// </summary>
        /// <param name="currentPassword">The user's current password</param>
        /// <param name="newPassword">The password the user wants to change their password to</param>
        /// <returns>A <see cref="BaseResponse" /> containing the code and the message returned from the authware api</returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application id is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="ArgumentNullException">Throws if either currentPassword or newPassword is null</exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the data provided is not acceptable by the Authware API, the hardware ID did not match (if enabled), the
        ///     application version is out-of-date (if enabled) or the password is invalid
        /// </exception>
        public async Task<BaseResponse> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            _ = currentPassword ?? throw new ArgumentNullException($"{nameof(currentPassword)} can not be null");
            _ = newPassword ?? throw new ArgumentNullException($"{nameof(newPassword)} can not be null");

            var response = await _requester
                .Request<BaseResponse>(HttpMethod.Put, "/user/change-password",
                    new {old_password = currentPassword, password = newPassword, repeat_password = newPassword})
                .ConfigureAwait(false);
            return response;
        }

        /// <summary>
        ///     Executes a specific API under the current user
        /// </summary>
        /// <param name="apiId">The ID of the API to execute</param>
        /// <param name="parameters">The user-specified parameters to passthrough to the API</param>
        /// <returns>
        ///     The response given by your API, find the plaintext response under the 'DecodedResponse' property, the response
        ///     will be a status code if the 'Show API responses' setting is off
        /// </returns>
        /// <exception cref="Exception">
        ///     This gets thrown if the application id is null which would be if
        ///     <see cref="InitializeApplicationAsync" /> hasn't been called
        /// </exception>
        /// <exception cref="ArgumentNullException">Throws if the API ID is null</exception>
        /// <exception cref="AuthwareException">
        ///     Thrown if the data provided is not acceptable by the Authware API, the hardware ID did not match (if enabled), the
        ///     application version is out-of-date (if enabled), the API does not exist or the user does not have the required role
        ///     to execute it and if the API execution was not successful.
        /// </exception>
        public async Task<ApiResponse> ExecuteApiAsync(string apiId, Dictionary<string, object> parameters)
        {
            var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
            _ = apiId ?? throw new ArgumentNullException($"{nameof(apiId)} can not be null");

            var apiResponse =
                await _requester.Request<ApiResponse>(HttpMethod.Post, "/api/execute", new {api_id = apiId, parameters})
                    .ConfigureAwait(false);
            return apiResponse;
        }
    }
}