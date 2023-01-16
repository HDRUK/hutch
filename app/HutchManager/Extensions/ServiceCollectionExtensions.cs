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
    enum JobQueueProviders
    {
      RabbitMq,
      AzureQueueStorage
    }
    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {
      var emailProvider = c["OutboundEmail:Provider"] ?? string.Empty;

      var useSendGrid = emailProvider.Equals("sendgrid", StringComparison.InvariantCultureIgnoreCase);
      var useSmtp = emailProvider.Equals("smtp", StringComparison.InvariantCultureIgnoreCase);

      if (useSendGrid) s.Configure<SendGridOptions>(c.GetSection("OutboundEmail"));
      else if (useSmtp) s.Configure<SmtpOptions>(c.GetSection("OutboundEmail"));
      else s.Configure<LocalDiskEmailOptions>(c.GetSection("OutboundEmail"));

      s
        .AddTransient<TokenIssuingService>()
        .AddTransient<RazorViewService>()
        .AddTransient<AccountEmailService>()
        .TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      if (useSendGrid) s.AddTransient<IEmailSender, SendGridEmailSender>();
      else if (useSmtp) s.AddTransient<IEmailSender, SmtpEmailSender>();
      else s.AddTransient<IEmailSender, LocalDiskEmailSender>();

      return s;
    }

    private static JobQueueProviders GetJobQueueProvider(IConfiguration c)
    {
      Enum.TryParse<JobQueueProviders>(
        c["JobQueue:Provider"],
        ignoreCase: true,
        out var jobQueueProvider);

      return jobQueueProvider;
    }
    public static IServiceCollection AddJobQueue(this IServiceCollection s, IConfiguration c)
    {
      var queueType = GetJobQueueProvider(c);

      switch (queueType)
      {
        case JobQueueProviders.AzureQueueStorage:
          s.Configure<AzureJobQueueOptions>(c.GetSection("JobQueue"))
            .AddTransient<IJobQueueService, AzureJobQueueService>();
          break;
        case JobQueueProviders.RabbitMq:
          s.Configure<RabbitJobQueueOptions>(c.GetSection("JobQueue"))
            .AddTransient<IJobQueueService, RabbitJobQueueService>();
          break;
        default:
          throw new Exception($"'{queueType}' is not a valid queue service");
      }

      return s;
    }
  }
}
