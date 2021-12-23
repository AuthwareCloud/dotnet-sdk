﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authware.Exceptions;
using Authware.Models;

namespace Authware
{
    /// <summary>
    ///     The main Authware application class, this contains all of the functions that can be used to interact with the
    ///     Authware API, but for static implementations
    /// </summary>
    public static class AuthwareStatic
    {
        private static readonly AuthwareApplication Authware = new();

        /// <summary>
        ///     Initializes and checks the ID passed in against the Authware API to make sure the application is properly setup and
        ///     enabled
        /// </summary>
        /// <param name="applicationId">The ID of your application, this can be fetched on the manage application page</param>
        /// <returns>The application name, version and creation date represented by a <see cref="Application" /></returns>
        public static async Task<StaticResponse<Application>> InitializeApplicationAsync(string applicationId)
        {
            var (success, error, application) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.InitializeApplicationAsync(applicationId));

            return await HandleResponseAsync(success, error, application);
        }

        /// <summary>
        ///     Gets all application variables that the user has permission to get
        /// </summary>
        /// <param name="authenticated">
        ///     Whether you want to pull authentication required variables or not, an
        ///     <see cref="AuthwareException" /> will be thrown if this is true and no user is currently authenticated
        /// </param>
        /// <returns>An array of keys and values represented by a <see cref="Variable" /></returns>
        public static async Task<StaticResponse<Variable[]>> GrabApplicationVariablesAsync(bool authenticated = false)
        {
            var (success, error, variables) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.GrabApplicationVariablesAsync(authenticated));

            return await HandleResponseAsync(success, error, variables);
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
        public static async Task<StaticResponse<BaseResponse>> RegisterAsync(string username, string password,
            string email, string token)
        {
            var (success, error, response) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.RegisterAsync(username, password, email, token));

            return await HandleResponseAsync(success, error, response);
        }

        /// <summary>
        ///     Authenticates a user against the Authware API with the provided username and password and caches the Authware
        ///     authentication token if successful
        ///     If the user has logged in before it will check the cached Authware authentication token and if the token is invalid
        ///     it will authenticate with the username and password
        /// </summary>
        /// <param name="username">The username you want to authenticate with</param>
        /// <param name="password">The password you want to authenticate with</param>
        /// <returns>The authenticated users profile, represented as <see cref="Profile" /></returns>
        public static async Task<StaticResponse<Profile>> LoginAsync(string username, string password)
        {
            var (success, error, profile) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.LoginAsync(username, password));

            return await HandleResponseAsync(success, error, profile);
        }

        /// <summary>
        ///     Allows a user to change their email on your application to a new email
        /// </summary>
        /// <param name="password">The user's current password</param>
        /// <param name="email">The email the user wants to change their email to</param>
        /// <returns>A <see cref="BaseResponse" /> containing the code and the message returned from the authware api</returns>
        public static async Task<StaticResponse<BaseResponse>> ChangeEmailAsync(string password, string email)
        {
            var (success, error, response) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.ChangeEmailAsync(password, email));

            return await HandleResponseAsync(success, error, response);
        }

        /// <summary>
        ///     Allows a user to change their current password to the password specified in newPassword
        /// </summary>
        /// <param name="currentPassword">The user's current password</param>
        /// <param name="newPassword">The password the user wants to change their password to</param>
        /// <returns>A <see cref="BaseResponse" /> containing the code and the message returned from the authware api</returns>
        public static async Task<StaticResponse<BaseResponse>> ChangePasswordAsync(string currentPassword,
            string newPassword)
        {
            var (success, error, response) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.ChangePasswordAsync(currentPassword, newPassword));

            return await HandleResponseAsync(success, error, response);
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
        public static async Task<StaticResponse<ApiResponse>> ExecuteApiAsync(string apiId,
            Dictionary<string, object> parameters)
        {
            var (success, error, response) = await ExecuteAuthwareInstancedMethod(async () =>
                await Authware.ExecuteApiAsync(apiId, parameters));

            return await HandleResponseAsync(success, error, response);
        }

        /// <summary>
        /// Prepares and returns a response that works with <see cref="StaticResponse{T}"/>
        /// </summary>
        /// <param name="success">If the response was successful</param>
        /// <param name="errorResponse">The error response from the API, this can be null</param>
        /// <param name="response">The <see cref="TResponse"/> from the API</param>
        /// <typeparam name="TResponse">The type of response that is required</typeparam>
        /// <returns>The <see cref="StaticResponse{T}"/> wrapped for the <see cref="TResponse"/></returns>
        private static async Task<StaticResponse<TResponse>> HandleResponseAsync<TResponse>(bool success,
            BaseResponse errorResponse, TResponse response)
        {
            if (success)
                return new StaticResponse<TResponse>
                {
                    Code = 200,
                    Response = response
                };
            return new StaticResponse<TResponse>
            {
                Code = errorResponse.Code,
                Message = errorResponse.ToString()
            };
        }

        /// <summary>
        /// Executes an Authware instanced method
        /// </summary>
        /// <param name="func">The <see cref="Task"/> based <see cref="Func{TResult}"/> to execute</param>
        /// <typeparam name="TResponse">The type of response required</typeparam>
        /// <returns>The parsed response as a success bool, error response and <see cref="TResponse"/></returns>
        private static async Task<(bool, ErrorResponse, TResponse)> ExecuteAuthwareInstancedMethod<TResponse>(
            Func<Task<TResponse>> func)
        {
            try
            {
                return (true, null, await func());
            }
            catch (AuthwareException e)
            {
                return (false, e.ErrorResponse, default);
            }
            catch (Exception e)
            {
                return (false, new ErrorResponse
                {
                    Message = e.Message,
                    Code = 400
                }, default);
            }
        }
    }
}