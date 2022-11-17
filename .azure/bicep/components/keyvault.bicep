param env string = 'dev'
param keyVaultName string = 'hutch-${env}-kv'
param tenantId string

param location string = resourceGroup().location

// https://docs.microsoft.com/en-us/azure/templates/microsoft.keyvault/vaults
resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
  }
}

output name string = kv.name
