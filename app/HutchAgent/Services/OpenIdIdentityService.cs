using System.IdentityModel.Tokens.Jwt;
using HutchAgent.Config;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

  private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
  {
    var disco = await _http.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
    {
      Address = _openIdOptions.OpenIdBaseUrl,
      Policy = new DiscoveryPolicy
      {
        ValidateIssuerName =
          false, // Keycloak may have a different issuer name format // TODO probably should fix this for production vs development
      }
    });
    if (disco.IsError)
    {
      _logger.LogError("OIDC Discovery failed for the Identity Provider at {Address}", _openIdOptions.OpenIdBaseUrl);
      _logger.LogError("Attempted OIDC Discovery yielded the error: {Error}", disco.Error);
      throw new InvalidOperationException(disco.Error);
    }

    return disco;
  }

  /// <summary>
  /// Follow the OIDC Resource Owner Password Credentials Grant Flow to get identity and access tokens on behalf of a user
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
  /// <returns>The requested tokens</returns>
  public async Task<(string identity, string access, string refresh)> RequestUserTokens(OpenIdOptions options)
    => await RequestUserTokens(options.ClientId, options.ClientSecret, options.Username, options.Password);

  /// <summary>
  /// Follow the OIDC Resource Owner Password Credentials Grant Flow to get identity and access tokens on behalf of a user
  /// from the configured Identity Provider, using the provided user credentials.
  /// </summary>
  /// <param name="clientId">Client ID</param>
  /// <param name="secret">Client Secret</param>
  /// <param name="username">The User's Username</param>
  /// <param name="password">The User's Password</param>
  /// <returns>The requested tokens</returns>
  public async Task<(string identity, string access, string refresh)> RequestUserTokens(string clientId, string secret,
    string username,
    string password)
  {
    var disco = await GetDiscoveryDocument();

    // Make a password token request for a user
    var tokenResponse = await _http.RequestPasswordTokenAsync(new()
    {
      Address = disco.TokenEndpoint,
      ClientId = clientId,
      ClientSecret = secret,
      UserName = username,
      Password = password,
      Scope = "openid" // we may want an id_token as well as an access_token when acting as a user
    });

    if (tokenResponse.IsError)
    {
      _logger.LogError("Attempted OIDC Token Request failed: {Error}", tokenResponse.Error);
      throw new InvalidOperationException(tokenResponse.Error);
    }

    // TODO any claim validation Hutch cares about? somewhat depends on use case (e.g. Controller vs Store?)

    // return the tokens for use
    return (tokenResponse.IdentityToken, tokenResponse.AccessToken, tokenResponse.RefreshToken);
  }

  /// <summary>
  /// Follow the OIDC Client Credentials flow to get a token
  /// from the configured Identity Provider, using the provided client id and secret.
  /// </summary>
  /// <param name="options">
  /// <para>
  /// Options model from which Client ID and Client Secret only will be used.
  /// </para>
  /// <para>
  /// Note that this options object won't override the target IdP BaseURL used.
  /// </para>
  /// </param>
  /// <returns>An OIDC access token for the requesting client.</returns>
  public async Task<string> RequestClientAccessToken(OpenIdOptions options)
    => await RequestClientAccessToken(options.ClientId, options.ClientSecret);

  /// <summary>
  /// Follow the OIDC Client Credentials flow to get a token
  /// from the configured Identity Provider, using the provided client id and secret.
  /// </summary>
  /// <param name="clientId">Client ID</param>
  /// <param name="secret">Client Secret</param>
  /// <returns>An OIDC access token for the requesting client.</returns>
  public async Task<string> RequestClientAccessToken(string clientId, string? secret)
  {
    var disco = await GetDiscoveryDocument();

    // Make a password token request for a user
    var tokenRequest = new ClientCredentialsTokenRequest
    {
      Address = disco.TokenEndpoint,
      ClientId = clientId
    };
    if (!secret.IsNullOrEmpty()) tokenRequest.ClientSecret = secret;
    var tokenResponse = await _http.RequestClientCredentialsTokenAsync(tokenRequest);

    if (tokenResponse.IsError)
    {
      _logger.LogError("Attempted OIDC Token Request failed: {Error}", tokenResponse.Error);
      throw new InvalidOperationException(tokenResponse.Error);
    }

    // TODO any claim validation Hutch cares about? somewhat depends on use case (e.g. Controller vs Store?)

    // return the tokens for use
    return tokenResponse.AccessToken;
  }

  /// <summary>
  /// Refreshing access token
  /// </summary>
  /// <param name="options"></param>
  /// <param name="refreshToken"></param>
  /// <returns></returns>
  public async Task<(string access, string refresh)> RefreshAccessToken(OpenIdOptions options, string refreshToken) =>
    await RefreshAccessToken(options.ClientId, options.ClientSecret, refreshToken);

  /// <summary>
  /// Refreshing token using refresh token endpoint
  /// </summary>
  /// <param name="clientId"></param>
  /// <param name="secret"></param>
  /// <param name="refreshToken"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task<(string access, string refresh)> RefreshAccessToken(string clientId, string? secret,
    string refreshToken)
  {
    var disco = await GetDiscoveryDocument();

    var tokenRequest = new RefreshTokenRequest
    {
      Address = disco.TokenEndpoint,
      ClientId = clientId,
      RefreshToken = refreshToken,
    };
    if (!secret.IsNullOrEmpty()) tokenRequest.ClientSecret = secret;

    var tokenResponse = await _http.RequestRefreshTokenAsync(tokenRequest);

    if (!tokenResponse.IsError) return (tokenResponse.AccessToken, tokenResponse.RefreshToken);

    _logger.LogError("Attempted OIDC Token Request failed: {Error}", tokenResponse.Error);
    throw new InvalidOperationException(tokenResponse.Error);
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
