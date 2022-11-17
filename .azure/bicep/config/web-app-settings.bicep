param keyVaultName string
param appInsightsName string
param appName string
param appSettings object = {}

var secretRef = '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName='

var sharedSettings = {
  OutboundEmail__FromName: 'SargAssure'
  OutboundEmail__Provider: 'sendgrid'
  OutboundEmail__SendGridApiKey: '${secretRef}sendgrid-api-key)'
  OutboundEmail__FromAddress: 'noreply@sargassure.com'
  Planet__ApiKey: '${secretRef}planet-api-key)'
  Worker__ApiKey: '${secretRef}worker-api-key)'
  Worker__AoiAssetContainer: 'aoiassets'
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
