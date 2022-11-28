param keyVaultName string
param appInsightsName string
param appName string
param appSettings object = {}

param workerStorageConnectionStringSecret string

var secretRef = '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName='

var sharedSettings = {
  AzureWebJobsStorage: '${secretRef}${workerStorageConnectionStringSecret})'
  AZURE_STORAGE_CONNECTION_STRING: '${secretRef}azure-storage-connection-string)'
}

var fnSettings = {
  FUNCTIONS_EXTENSION_VERSION: '~4'
  FUNCTIONS_WORKER_RUNTIME: 'python'
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
  properties: union(fnSettings, appInsightsSettings, sharedSettings, appSettings)
}
