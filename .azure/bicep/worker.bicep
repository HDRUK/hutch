@allowed([
  'dev'
  'uat'
])
param env string = 'dev'

param appSettings object = {}

param keyVaultName string
param aspName string

param webApiBaseUrl string

param location string = resourceGroup().location

var appName = '${env}-hutch-worker'
var logAnalyticsWorkspaceName = 'hutch-la-ws'

// create backing storage account
module workerStorage 'components/storage-account.bicep' = {
  name: 'workerstorage-${uniqueString(appName)}'
  params: {
    location: location
    baseAccountName: 'worker'
    uniqueStringSource: appName
    keyVaultName: keyVaultName
    tags: {
      Environment: env
    }
  }
}

// ideally we would create an app service plan
// but while we are restricted to a single RG
// we can't create a linux consumption plan
// in the same RG as ANY OTHER ASP! WTF!
// module asp 'components/app-service-plan.bicep' = {
//   name: 'asp-${uniqueString(appName)}'
//   params: {
//     aspName: aspName
//     sku: 'Y1' // consumption
//     tags: {
//       Environment: env
//     }
//   }
// }

// create the app
module app 'components/functions-app.bicep' = {
  name: 'appService-${uniqueString(appName)}'
  params: {
    location: location
    aspName: aspName
    appName: appName
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    tags: {
      Environment: env
    }
  }
}

// grant it keyvault access
module apiKvAccess 'config/keyvault-access.bicep' = {
  name: 'kvAccess-${uniqueString(appName)}'
  params: {
    keyVaultName: keyVaultName
    tenantId: app.outputs.tenantId
    objectId: app.outputs.principalId
  }
}

// now that keyvault is accessible, add settings (some of which are KeyVault linked)
module workerAppSettings 'config/worker-app-settings.bicep' = {
  name: 'appSettings-${uniqueString(appName)}'
  params: {
    appName: app.outputs.name
    keyVaultName: keyVaultName
    appInsightsName: app.outputs.appInsightsName
    workerStorageConnectionStringSecret: workerStorage.outputs.connectionStringSecret
    appSettings: union(appSettings, {
      ManagerApiUrl: webApiBaseUrl
    })
  }
}
