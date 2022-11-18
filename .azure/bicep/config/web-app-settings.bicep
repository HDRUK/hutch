param keyVaultName string
param appInsightsName string
param appName string
param appSettings object = {}

var secretRef = '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName='

var sharedSettings = {
  OutboundEmail__FromName: 'hutch'
  OutboundEmail__Provider: 'sendgrid'
  OutboundEmail__SendGridApiKey: '${secretRef}sendgrid-api-key)'
  OutboundEmail__FromAddress: 'noreply@hutch.com'
  // Environment for deployment (Development or Production)
  ASPNETCORE_ENVIRONMENT: '${secretRef}ASPNETCORE_ENVIRONMENT)'
  // Paths to SSL certificates
  ASPNETCORE_Kestrel__Certificates__Default__Path: '${secretRef}ASPNETCORE_Kestrel__Certificates__Default__Path)'
  ASPNETCORE_Kestrel__Certificates__Default__KeyPath: '${secretRef}ASPNETCORE_Kestrel__Certificates__Default__KeyPath)'
  // Connection string to database
  ASPNETCORE_ConnectionStrings__Default: '${secretRef}ASPNETCORE_ConnectionStrings__Default)'
  // Credentials for RabbitMQ queue
  ASPNETCORE_JobQueue__HostName: '${secretRef}ASPNETCORE_JobQueue__HostName)'
  ASPNETCORE_JobQueue__UserName: '${secretRef}ASPNETCORE_JobQueue__UserName)'
  ASPNETCORE_JobQueue__Password: '${secretRef}ASPNETCORE_JobQueue__Password)'
}

resource appinsights 'microsoft.insights/components@2020-02-02-preview' existing = {
  name: appInsightsName
}
var appInsightsSettings = {
  APPINSIGHTS_INSTRUMENTATIONKEY: appinsights.properties.InstrumentationKey
  ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
  XDT_MicrosoftApplicationInsights_Mode: 'recommended'
  DiagnosticServices_EXTENSION_VERSION: '~3'
  APPINSIGHTS_PROFILERFEATURE_VERSION: '1.0.0'
  APPINSIGHTS_SNAPSHOTFEATURE_VERSION: '1.0.0'
  InstrumentationEngine_EXTENSION_VERSION: '~1'
  SnapshotDebugger_EXTENSION_VERSION: '~1'
  XDT_MicrosoftApplicationInsights_BaseExtensions: '~1'
}

// https://docs.microsoft.com/en-us/azure/templates/microsoft.web/2020-09-01/sites/config-appsettings
resource settings 'Microsoft.Web/sites/config@2020-09-01' = {
  name: '${appName}/appsettings'
  properties: union(appInsightsSettings, sharedSettings, appSettings)
}
