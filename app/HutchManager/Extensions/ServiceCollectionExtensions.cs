using HutchManager.Config;
using HutchManager.Services;
using HutchManager.Services.Contracts;
using HutchManager.Services.EmailSender;
using HutchManager.Services.EmailServices;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HutchManager.Extensions
{
  public static class ServiceCollectionExtensions
  {
    /// <summary>
    /// Enum for Email Providers.
    /// </summary>
    enum EmailProviders
    {
      Local,
      SendGrid,
      SMTP
    }
    
    /// <summary>
    /// Enum for Job Queue Providers.
    /// </summary>
    enum JobQueueProviders
    {
      RabbitMq,
      AzureQueueStorage
    }
    
    /// <summary>
    /// Parse the desired Email Provider from configuration.
    /// </summary>
    /// <param name="c">The configuration object</param>
    /// <returns>The <c>enum</c> for the chosen Email Provider</returns>
    private static EmailProviders GetEmailProvider(IConfiguration c)
    {
      // TryParse defaults failures to the first enum value.
      Enum.TryParse<EmailProviders>(
        c["OutboundEmail:Provider"],
        ignoreCase: true,
        out var emailProvider);

      return emailProvider;
    }
    
    private static IServiceCollection ConfigureEmail(
      this IServiceCollection s,
      EmailProviders provider,
      IConfiguration c)
    {
      // set the appropriate configuration function
      Func<IConfiguration, IServiceCollection> ConfigureEmail =
        provider switch
        {
          EmailProviders.SendGrid => s.Configure<SendGridOptions>,
          //EmailProviders.SMTP => s.Configure<
          _ => s.Configure<LocalDiskEmailOptions>
        };

      // and execute it
      ConfigureEmail.Invoke(c.GetSection("OutboundEmail"));

      return s;
    }
    
    /// <summary>
    /// Determine which email sender the user wishes to use and adds it the service collection.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {
      var emailProvider = GetEmailProvider(c);

      s.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      return s.ConfigureEmail(emailProvider, c)
        .AddTransient<TokenIssuingService>()
        .AddTransient<RazorViewService>()
        .AddTransient<AccountEmailService>()
        .AddTransient(
          typeof(IEmailSender),
          emailProvider switch
          {
            EmailProviders.SendGrid => typeof(SendGridEmailSender),
            
            _ => typeof(LocalDiskEmailSender)
          });
    }

    /// <summary>
    /// Parse the desired Job Queue Provider from configuration.
    /// </summary>
    /// <param name="c">The configuration object</param>
    /// <returns>The <c>enum</c> for the chosen Job Queue Provider</returns>
    private static JobQueueProviders GetJobQueueProvider(IConfiguration c)
    {
      Enum.TryParse<JobQueueProviders>(
        c["JobQueue:Provider"],
        ignoreCase: true,
        out var jobQueueProvider);

      return jobQueueProvider;
    }
    
    private static IServiceCollection ConfigureJobQueue(
      this IServiceCollection s,
      JobQueueProviders provider,
      IConfiguration c)
    {
      // set the appropriate configuration function
      Func<IConfiguration, IServiceCollection> ConfigureEmail =
        provider switch
        {
          JobQueueProviders.RabbitMq => s.Configure<AzureJobQueueOptions>,
          JobQueueProviders.AzureQueueStorage => s.Configure<AzureJobQueueOptions>,
          _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };

      // and execute it
      ConfigureEmail.Invoke(c.GetSection("JobQueue"));

      return s;
    }
    
    /// <summary>
    /// Determine which job queue the user wishes to use and adds it the service collection.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IServiceCollection AddJobQueue(this IServiceCollection s, IConfiguration c)
    {
      var queueType = GetJobQueueProvider(c);

      return s.ConfigureJobQueue(queueType, c)
        .AddTransient(
          typeof(IJobQueueService),
          queueType switch
          {
            JobQueueProviders.AzureQueueStorage => typeof(AzureJobQueueService),
            _ => typeof(RabbitJobQueueService)
          });
    }
  }
}
