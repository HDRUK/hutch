param keyVaultName string
param appInsightsName string
param appName string
param appSettings object = {}

param workerStorageConnectionStringSecret string

var secretRef = '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName='

var sharedSettings = {
  AzureWebJobsStorage: '${secretRef}${workerStorageConnectionStringSecret})'
  // Log database settings
  LOG_DB_DRIVERNAME: '${secretRef}LOG_DB_DRIVERNAME)'
  LOG_DB_PORT: '${secretRef}LOG_DB_PORT)'
  LOG_DB_HOST: '${secretRef}LOG_DB_HOST)'
  LOG_DB_DATABASE: '${secretRef}LOG_DB_DATABASE)'
  LOG_DB_USERNAME: '${secretRef}LOG_DB_USERNAME)'
  LOG_DB_PASSWORD: '${secretRef}LOG_DB_PASSWORD)'
  // Data source database settings
  DATASOURCE_NAME: '${secretRef}DATASOURCE_NAME)'
  DATASOURCE_DB_DRIVERNAME: '${secretRef}DATASOURCE_DB_DRIVERNAME)'
  DATASOURCE_DB_PORT: '${secretRef}DATASOURCE_DB_PORT)'
  DATASOURCE_DB_HOST: '${secretRef}DATASOURCE_DB_HOST)'
  DATASOURCE_DB_USERNAME: '${secretRef}DATASOURCE_DB_USERNAME)'
  DATASOURCE_DB_PASSWORD: '${secretRef}DATASOURCE_DB_PASSWORD)'
  DATASOURCE_DB_SCHEMA: '${secretRef}DATASOURCE_DB_SCHEMA)'
  // Manager related settings
  MANAGER_URL: '${secretRef}MANAGER_URL)'
  MANAGER_VERIFY_SSL: '${secretRef}MANAGER_VERIFY_SSL)'
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
