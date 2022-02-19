using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Authware.Exceptions;
using Authware.Models;

namespace Authware;

/// <summary>
///     The main Authware application class, this contains all of the functions that can be used to interact with the
///     Authware API
/// </summary>
public class AuthwareApplication
{
    /// <summary>
    ///     The function for getting a users hardware ID, this is optional, and if not set will be
    /// </summary>
    private Func<string> IdentifierFunction { get; } = Identifiers.GetIdentifier;

    /// <summary>
    ///     This is used to facilitate HTTP requests for this class
    /// </summary>
    private readonly Requester _requester = new();
    
    /// <summary>
    ///     Stores the information responded by <see cref="InitializeApplicationAsync" /> for easy access
    /// </summary>
    public Application? ApplicationInformation { get; private set; }

    /// <summary>
    ///     The ID of the current application
    /// </summary>
    private string? _applicationId;

    /// <summary>
    ///     The path that the current users' authentication token is located
    /// </summary>
    private string? _authTokenPath;

    /// <summary>
    ///     Constructs the application with a custom hardware ID system, this allows you to define what you want to use to get
    ///     a users' hardware ID. This can also allow you to enable hardware ID checking on non-Windows systems.
    /// </summary>
    /// <param name="identifierFunction">The function to fetch a hardware ID for a user</param>
    public AuthwareApplication(Func<string> identifierFunction)
    {
        IdentifierFunction = identifierFunction;
    }

    /// <summary>
    ///     Constructs the application class instance with default values
    /// </summary>
    public AuthwareApplication()
    {
    }
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
        if (ApplicationInformation is null)
        {
            var _ = applicationId ??
                    throw new ArgumentNullException(applicationId, $"{nameof(applicationId)} can not be null");
            if (!Guid.TryParse(applicationId, out var _))
                throw new ArgumentException($"{applicationId} is invalid");

            _applicationId = applicationId;
            var applicationResponse = await _requester
                .Request<Application>(HttpMethod.Post, "/app", new {app_id = applicationId})
                .ConfigureAwait(false);
            _authTokenPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Authware",
                applicationId, "authtoken.bin");
            if (!Directory.Exists(Path.GetDirectoryName(_authTokenPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_authTokenPath) ?? string.Empty);
            if (applicationResponse.CheckIdentifier)
                _requester.Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Authware-Hardware-ID",
                    IdentifierFunction());

            ApplicationInformation = applicationResponse;
        }

        return ApplicationInformation;
    }

    /// <summary>
    ///     Creates a new user variable with the specified key and value
    /// </summary>
    /// <param name="key">
    ///     The key of the variable to create
    /// </param>
    /// <param name="value">
    ///     The value of the variable to create
    /// </param>
    /// <param name="canEdit">
    ///     Should the users be able to edit this variable (This can be used to make readonly variables
    /// </param>
    /// <returns>A <see cref="UpdatedDataResponse{T}" /> which contains the newly created variable</returns>
    /// <exception cref="Exception">
    ///     This gets thrown if the application id is null which would be if
    ///     <see cref="InitializeApplicationAsync" /> hasn't been called
    /// </exception>
    /// <exception cref="ArgumentNullException">If any of the parameters are null this is what gets thrown</exception>
    /// <exception cref="AuthwareException">
    ///     Thrown if the application is disabled or you attempted to create a variable when the application has creating user
    ///     variables disabled
    /// </exception>
    public async Task<UpdatedDataResponse<UserVariable>> CreateUserVariableAsync(string key, string value,
        bool canEdit = true)
    { 
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = key ?? throw new ArgumentNullException(key, $"{nameof(key)} can not be null");
        _ = value ?? throw new ArgumentNullException(value, $"{nameof(value)} can not be null");

        var response = await _requester
            .Request<UpdatedDataResponse<UserVariable>>(HttpMethod.Post, "/user/variables",
                new {key, value, can_user_edit = canEdit})
            .ConfigureAwait(false);

        return response;
    }

    /// <summary>
    ///     Updates a user variable by the variables key
    /// </summary>
    /// <param name="key">
    ///     The key of the variable to update
    /// </param>
    /// <param name="newValue">
    ///     The new value of the variable
    /// </param>
    /// <returns>A <see cref="UpdatedDataResponse{T}" /> which contains the newly created variable</returns>
    /// <exception cref="Exception">
    ///     This gets thrown if the application id is null which would be if
    ///     <see cref="InitializeApplicationAsync" /> hasn't been called
    /// </exception>
    /// <exception cref="ArgumentNullException">If any of the parameters are null this is what gets thrown</exception>
    /// <exception cref="AuthwareException">
    ///     Thrown if the application is disabled or you attempted to modify a variable when you do not have permission to
    /// </exception>
    public async Task<UpdatedDataResponse<UserVariable>> UpdateUserVariableAsync(string key, string newValue)
    { 
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = key ?? throw new ArgumentNullException(key, $"{nameof(key)} can not be null");
        _ = newValue ?? throw new ArgumentNullException(newValue, $"{nameof(newValue)} can not be null");

        var response = await _requester
            .Request<UpdatedDataResponse<UserVariable>>(HttpMethod.Put, "/user/variables",
                new {key, value = newValue})
            .ConfigureAwait(false);

        return response;
    }

    /// <summary>
    ///     Deletes a user variable by the variables key
    /// </summary>
    /// <param name="key">
    ///     The key of the variable to delete
    /// </param>
    /// <returns>A <see cref="BaseResponse" /> that represents whether the response succeeded or not</returns>
    /// <exception cref="Exception">
    ///     This gets thrown if the application id is null which would be if
    ///     <see cref="InitializeApplicationAsync" /> hasn't been called
    /// </exception>
    /// <exception cref="ArgumentNullException">If any of the parameters are null this is what gets thrown</exception>
    /// <exception cref="AuthwareException">
    ///     Thrown if the application is disabled or you attempted to modify a variable when you do not have permission to
    /// </exception>
    public async Task<BaseResponse> DeleteUserVariableAsync(string key)
    { 
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = key ?? throw new ArgumentNullException(key, $"{nameof(key)} can not be null");

        var response = await _requester
            .Request<BaseResponse>(HttpMethod.Delete, "/user/variables",
                new {key})
            .ConfigureAwait(false);

        return response;
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
    /// <exception cref="ArgumentNullException">If any of the parameters are null this is what gets thrown</exception>
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
                .Request<Variable[]>(HttpMethod.Get, "/app/variables", null)
                .ConfigureAwait(false);
            return authenticatedVariables;
        }

        var variables = await _requester
            .Request<Variable[]>(HttpMethod.Post, "/app/variables", new {app_id = _applicationId})
            .ConfigureAwait(false);
        return variables;
    }

    /// <summary>
    ///     Logs the user out of your application, this deletes the cached auth token, this will not do anything if the user is
    ///     not signed in.
    /// </summary>
    /// <exception cref="Exception">
    ///     This gets thrown if the application ID is null which would be if
    ///     <see cref="InitializeApplicationAsync" /> hasn't been called
    /// </exception>
    public void Logout()
    {
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");

        // Try to delete the current auth token, this prevents issues if called when the user is not signed-in.
        if (File.Exists(_authTokenPath)) File.Delete(_authTokenPath!);

        _requester.Client.DefaultRequestHeaders.Authorization = null;
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
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = username ?? throw new ArgumentNullException(username, $"{nameof(username)} can not be null");
        _ = password ?? throw new ArgumentNullException(password, $"{nameof(password)} can not be null");
        _ = email ?? throw new ArgumentNullException(email, $"{nameof(email)} can not be null");
        _ = token ?? throw new ArgumentNullException(token, $"{nameof(token)} can not be null");
        if (!Guid.TryParse(token, out var _))
            throw new ArgumentException($"{nameof(token)} is invalid");

        var response = await _requester
            .Request<BaseResponse>(HttpMethod.Post, "/user/register",
                new
                {
                    app_id = _applicationId, username, password,
                    email_address = email, token
                })
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
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = username ?? throw new ArgumentNullException(username, $"{nameof(username)} can not be null");
        _ = password ?? throw new ArgumentNullException(password, $"{nameof(password)} can not be null");
        if (File.Exists(_authTokenPath))
        {
            var authToken = File.ReadAllText(_authTokenPath!);
            _requester.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);
            try
            {
                return await GetUserProfileAsync();
            }
            catch
            {
                // ignored will be thrown below if it fails for whatever reason but still lets delete the bad auth token
                File.Delete(_authTokenPath!);
                _requester.Client.DefaultRequestHeaders.Authorization = null;
            }
        }

        var authResponse = await _requester
            .Request<AuthResponse>(HttpMethod.Post, "/user/auth",
                new {app_id = _applicationId, username, password})
            .ConfigureAwait(false);
        _requester.Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authResponse.AuthToken);
        var profileResponse =
            await _requester.Request<Profile>(HttpMethod.Get, "user/profile", null).ConfigureAwait(false);
        File.WriteAllText(_authTokenPath!, authResponse.AuthToken);
        return profileResponse;
    }

    /// <summary>
    ///     Gets the currently authenticated users' profile
    /// </summary>
    /// <returns>The currently authenticated users' profile, represented as <see cref="Profile" /></returns>
    /// <exception cref="Exception">
    ///     This gets thrown if the application ID is null which would be if
    ///     <see cref="InitializeApplicationAsync" /> hasn't been called
    /// </exception>
    /// <exception cref="AuthwareException">
    ///     Thrown if no user is authenticated
    /// </exception>
    public async Task<Profile> GetUserProfileAsync()
    {
        var _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        return await _requester.Request<Profile>(HttpMethod.Get, "user/profile", null).ConfigureAwait(false);
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
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = password ?? throw new ArgumentNullException(password, $"{nameof(password)} can not be null");
        _ = email ?? throw new ArgumentNullException(email, $"{nameof(email)} can not be null");

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
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = currentPassword ??
            throw new ArgumentNullException(currentPassword, $"{nameof(currentPassword)} can not be null");
        _ = newPassword ?? throw new ArgumentNullException(newPassword, $"{nameof(newPassword)} can not be null");

        var response = await _requester
            .Request<BaseResponse>(HttpMethod.Put, "/user/change-password",
                new
                {
                    old_password = currentPassword, password = newPassword,
                    repeat_password = newPassword
                })
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
        _ = _applicationId ?? throw new Exception($"{nameof(_applicationId)} can not be null");
        _ = apiId ?? throw new ArgumentNullException(apiId, $"{nameof(apiId)} can not be null");

        var apiResponse =
            await _requester.Request<ApiResponse>(HttpMethod.Post, "/api/execute", new {api_id = apiId, parameters})
                .ConfigureAwait(false);
        return apiResponse;
    }
}