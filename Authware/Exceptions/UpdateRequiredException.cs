using Authware.Models;

namespace Authware.Exceptions;

/// <summary>
///     This is thrown when your application needs an update
/// </summary>
public class UpdateRequiredException : AuthwareException
{
    /// <summary>
    ///     Constructs an instance of the update required exception
    /// </summary>
    /// <param name="updateUrl">The url containing the link to the updater</param>
    /// <param name="errorResponse">The error response</param>
    public UpdateRequiredException(string updateUrl, ErrorResponse? errorResponse) : base(errorResponse)
    {
        UpdateUrl = updateUrl;
    }

    /// <summary>
    ///     This is the url for your updater
    /// </summary>
    public string UpdateUrl { get; }
}