using System.ComponentModel.DataAnnotations;
using HutchAgent.Constants;

namespace HutchAgent.Models;

public class DatabaseConnectionDetails
{
  /// <summary>
  /// Database Server Hostname
  /// </summary>
  [Required]
  public string Hostname { get; set; } = string.Empty;

  /// <summary>
  /// Database Server Port. Defaults to PostgreSQL Default (5432)
  /// </summary>
  public int Port { get; set; } = 5432;

  /// <summary>
  /// Name of the Database to connect to
  /// </summary>
  [Required]
  public string Database { get; set; } = string.Empty;

  /// <summary>
  /// Username with access to the database
  /// </summary>
  [Required]
  public string Username { get; set; } = string.Empty;

  /// <summary>
  /// Password with access to the database
  /// </summary>
  [Required]
  public string Password { get; set; } = string.Empty;
}

public static class DatabaseConnectionDetailsExtensions
{
  /// <summary>
  /// <para>
  /// Get alternatives to `localhost` for the host machine of a given container engine.
  /// </para>
  /// <para>
  /// If the <see cref="DatabaseConnectionDetails" /> <see cref="Host"/> property is not `localhost`
  /// then no mapping occurs.
  /// </para>
  /// <para>
  /// If the container's localhost (not the host machine's) is desired,
  /// the loopback address `127.0.0.1` should be used instead to avoid mapping.</para>
  /// </summary>
  /// <param name="dataAccess">The <see cref="DatabaseConnectionDetails" /> object.</param>
  /// <param name="containerEngine">The target container engine.</param>
  /// <returns></returns>
  public static string GetContainerHost(this DatabaseConnectionDetails dataAccess, ContainerEngineType containerEngine)
  {
    return dataAccess.Hostname != "localhost"
      ? dataAccess.Hostname // if it's not localhost, it's irrelevant; use the configured value
      : containerEngine switch
      {
        ContainerEngineType.Podman => "host.containers.internal",
        ContainerEngineType.Docker or ContainerEngineType.Singularity => "172.17.0.1",
        _ => dataAccess.Hostname
      };
  }
}
