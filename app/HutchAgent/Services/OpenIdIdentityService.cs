using System.IdentityModel.Tokens.Jwt;
using HutchAgent.Config;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Scriban.Parsing;

namespace HutchAgent.Services;

/// <summary>
/// Performs interactions with an Open ID Connect compliant IdentityProvider such as Keycloak.
/// </summary>
public class OpenIdIdentityService
{
  private readonly ILogger<OpenIdIdentityService> _logger;
  private readonly OpenIdOptions _openIdOptions;
  private readonly HttpClient _http;

  public OpenIdIdentityService(
    IOptions<OpenIdOptions> openIdOptions,
    IHttpClientFactory httpClientFactory,
    ILogger<OpenIdIdentityService> logger)
  {
    _logger = logger;
    _http = httpClientFactory.CreateClient();
    _openIdOptions = openIdOptions.Value;
  }

  /// <summary>
  /// Check that a JWT is valid and unexpired
  /// </summary>
  /// <param name="jwt">the token to check</param>
  /// <returns>Whether the token is valid and unexpired</returns>
  public bool IsTokenValid(string jwt)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(jwt)) return false;

      var token = new JwtSecurityToken(jwt);
      return IsTokenValid(token);
    }
    catch (Exception)
    {
      return false;
    }
  }

  /// <summary>
  /// Check that a JWT is valid and unexpired
  /// </summary>
  /// <param name="jwt">the token to check</param>
  /// <returns>Whether the token is valid and unexpired</returns>
  public bool IsTokenValid(JwtSecurityToken jwt)
  {
    var now = DateTimeOffset.UtcNow;

    // TODO in future we could check issuer, checksum etc?
    return jwt.ValidFrom >= now && jwt.ValidTo < now;
  }

  /// <summary>
  /// Follow the OIDC Authorization Code flow to get a token on behalf of a user
  /// from the configured Identity Provider, using the provided user credentials.
  /// </summary>
  /// <param name="options">
  /// <para>
  /// An options model to extract Client ID, Client Secret, Username and Password from.
  /// </para>
  /// <para>
  /// Note that this options object won't override the target IdP BaseURL used.
  /// </para>
  /// </param>
  /// <returns>The requested token</returns>
  public async Task<string> RequestUserAccessToken(OpenIdOptions options)
    => await RequestUserAccessToken(options.ClientId, options.ClientSecret, options.Username, options.Password);

  /// <summary>
  /// Follow the OIDC Authorization Code flow to get a token on behalf of a user
  /// from the configured Identity Provider, using the provided user credentials.
  /// </summary>
  /// <param name="clientId">Client ID</param>
  /// <param name="secret">Client Secret</param>
  /// <param name="username">The User's Username</param>
  /// <param name="password">The User's Password</param>
  /// <returns>The requested token</returns>
  public async Task<string> RequestUserAccessToken(string clientId, string secret, string username, string password)
  {
    var disco = await _http.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
    {
      Address = _openIdOptions.OpenIdBaseUrl,
      Policy = new DiscoveryPolicy
      {
        ValidateIssuerName = false, // Keycloak may have a different issuer name format
      }
    });
    if (disco.IsError)
    {
      _logger.LogError("OIDC Discovery failed for the Identity Provider at {Address}", _openIdOptions.OpenIdBaseUrl);
      _logger.LogError("Attempted OIDC Discovery yielded the error: {Error}", disco.Error);
      throw new InvalidOperationException(disco.Error);
    }

    // Make a password token request for a user
    var tokenResponse = await _http.RequestPasswordTokenAsync(new()
    {
      Address = disco.TokenEndpoint,
      ClientId = clientId,
      ClientSecret = secret,
      UserName = username,
      Password = password
    });

    if (tokenResponse.IsError)
    {
      _logger.LogError("Attempted OIDC Token Request failed: {Error}", tokenResponse.Error);
      throw new InvalidOperationException(tokenResponse.Error);
    }

    // TODO any claim validation Hutch cares about? somewhat depends on use case (e.g. Controller vs Store?)

    // return the access token for use
    return tokenResponse.AccessToken;
  }

  /// <summary>
  /// Follow the OIDC Client Credentials flow to get a token
  /// from the configured Identity Provider, using the provided client id and secret.
  /// </summary>
  /// <param name="clientId">Client ID</param>
  /// <param name="secret">Client Secret</param>
  /// <exception cref="NotImplementedException">Client Credentials flow currently not supported</exception>
  // TODO this is the "technically correct" thing for Hutch to do, but the other systems are expecting user tokens
  public Task RequestClientAccessToken(string clientId, string secret)
  {
    throw new NotImplementedException(
      "TRE Controller API and Intermediary Store don't support this yet, so neither does Hutch at this time.");
  }

  // JWT content validation example
  // var jwtHandler = new JwtSecurityTokenHandler();
  // var token = jwtHandler.ReadJwtToken(tokenResponse.AccessToken);
  // var groupClaims = token.Claims.Where(c => c.Type == "realm_access").Select(c => c.Value);
  // var roles = new TokenRoles()
  // {
  //   roles = new List<string>()
  // };
  // if (!string.IsNullOrWhiteSpace(requiredRole))
  // {
  //   if (groupClaims.Any())
  //   {
  //     roles = JsonConvert.DeserializeObject<TokenRoles>(groupClaims.First());
  //   }
  //
  //   if (!roles.roles.Any(gc => gc.Equals(requiredRole)))
  //   {
  //     Log.Information("{Function} User {Username} does not have correct role {AdminRole}",
  //       "GetTokenForUser", username, requiredRole);
  //     return "";
  //   }
  //
  //   Log.Information("{Function} Token found with correct role {AdminRole} for User {Username}",
  //     "GetTokenForUser", requiredRole, username);
  // }
  // else
  // {
  //   Log.Information("{Function} Token found for User {Username}, no role required",
  //     "GetTokenForUser", requiredRole, username);
  // }
}
