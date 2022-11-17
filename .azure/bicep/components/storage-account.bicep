param location string = resourceGroup().location
param baseAccountName string
param uniqueStringSource string = resourceGroup().id
param keyVaultName string = ''
param tags object = {}

// https://docs.microsoft.com/en-us/azure/templates/microsoft.storage/2019-06-01/storageaccounts
resource storage 'Microsoft.Storage/storageAccounts@2021-01-01' = {
  name: '${baseAccountName}${uniqueString(uniqueStringSource)}'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
  }
  tags: union({
    Source: 'Bicep'
  }, tags)
}

var connectionString = 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'

// If a key vault name is specified, store a connection string for this storage account in it
resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = if (!empty(keyVaultName)) {
  name: keyVaultName
}
resource secret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = if (!empty(keyVaultName)) {
  name: 'storage-${storage.name}-connection-string'
  parent: kv
  properties: {
    value: connectionString
  }
}

output name string = storage.name
output connectionStringSecret string = empty(keyVaultName) ? '' : secret.name
