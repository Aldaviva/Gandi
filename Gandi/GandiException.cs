using Unfucked.HTTP.Exceptions;

namespace Gandi;

/// <summary>
/// An error occurred while communicating with the Gandi REST API. This can be caused by an application error (trying to query a DNS record that does not exist), authentication (passing an invalid or
/// expired API key or personal access token) or authorization (passing an API key or personal access token for the wrong user or organization, or one that doesn't have access to the domain or
/// permission to modify domain DNS records) (where the exception class will be <see cref="AuthException"/>), or a network I/O error like a DNS or TCP problem while connecting to the API HTTPS server.
/// </summary>
/// <param name="message">Description of the problem</param>
/// <param name="cause">Underlying issue, usually a subclass of <see cref="HttpException"/> (such as <see cref="NotAuthorizedException"/> or <see cref="ForbiddenException"/>).</param>
public class GandiException(string message, Exception cause): ApplicationException(message, cause) {

    /// <summary>
    /// An authentication or authorization error occurred while communicating with the Gandi REST API. This can be caused by passing an invalid or expired API key or personal access token, passing an
    /// API key or personal access token for the wrong user or organization, or passing an API key or personal access token that doesn't have access to the domain or permission to modify domain DNS
    /// records.
    /// </summary>
    /// <param name="message">Description of the problem</param>
    /// <param name="cause">Underlying issue, usually a subclass of <see cref="HttpException"/> (such as <see cref="NotAuthorizedException"/> or <see cref="ForbiddenException"/>).</param>
    public class AuthException(string message, Exception cause): GandiException(message, cause);

}