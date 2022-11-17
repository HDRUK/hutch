param location string = resourceGroup().location

var env = 'dev'

var appHostnames = [
  'dev.sargassure.com'
]

var aspName = 'nonprod-asp' // shared by all non prod apps
var apiBaseUrl = 'https://dev.sargassure.com/api'

resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = {
  name: 'sargassure-${env}-kv'
}

// Web App
module web 'web.bicep' = {
  name: 'sargassure-web-${uniqueString(env)}'
  params: {
    location: location
    env: env
    aspName: aspName
    keyVaultName: kv.name
    appHostnames: appHostnames
    appSettings: {
      AspNetCore__Environment: env
    }
  }
}

// Worker Functions App
module worker 'worker.bicep' = {
  name: 'sargassure-worker-${uniqueString(env)}'
  params: {
    location: location
    env: env
    keyVaultName: kv.name
    aspName: aspName
    webApiBaseUrl: apiBaseUrl
  }
}
