namespace HutchAgent.Config;

public class OpenIdOptions
{
  /// <summary>
  /// Fully qualified absolute base URL for API interactions with an OpenID Connect (OIDC) compliant
  /// Identity Provider (IdP), such as Keycloak.
  /// e.g. https://keycloak.tre.com/realms/hutch-dev
  /// </summary>
  public string OpenIdBaseUrl { get; set; } = string.Empty;

  /// <summary>
  /// Hutch Client ID for an OIDC IdP
  /// </summary>
  public string ClientId { get; set; } = string.Empty;

  /// <summary>
  /// Hutch Client Secret for an OIDC IdP
  /// </summary>
  public string ClientSecret { get; set; } = string.Empty;

  /// <summary>
  /// Username for a User in the IdP for Hutch to act on behalf of.
  /// </summary>
  ///  TODO in future it should be possible to omit this and use the Client Credentials flow?
  public string Username { get; set; } = string.Empty;

  /// <summary>
  /// Password for a User in the IdP for Hutch to act on behalf of.
  /// </summary>
  ///  TODO in future it should be possible to omit this and use the Client Credentials flow?
  public string Password { get; set; } = string.Empty;
}
