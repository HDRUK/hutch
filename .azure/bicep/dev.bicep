param location string = resourceGroup().location

param webSettings object = {}
param workerSettings object = {}

var env = 'dev'

var appHostnames = [
  'dev.hutch.com'
]

var aspName = 'nonprod-asp' // shared by all non prod apps
var apiBaseUrl = 'https://dev.hutch.com/api'

resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = {
  name: 'hutch-${env}-kv'
}

// Web App
module web 'web.bicep' = {
  name: 'hutch-web-${uniqueString(env)}'
  params: {
    location: location
    env: env
    aspName: aspName
    keyVaultName: kv.name
    appHostnames: appHostnames
    appSettings: webSettings
  }
}

// Worker Functions App
module worker 'worker.bicep' = {
  name: 'hutch-worker-${uniqueString(env)}'
  params: {
    location: location
    env: env
    keyVaultName: kv.name
    aspName: aspName
    webApiBaseUrl: apiBaseUrl
    appSettings: workerSettings
  }
}
