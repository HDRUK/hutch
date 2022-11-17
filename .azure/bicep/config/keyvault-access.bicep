param keyVaultName string
param tenantId string
param objectId string

resource kv 'Microsoft.KeyVault/vaults@2020-04-01-preview' existing = {
  name: keyVaultName
}

// https://docs.microsoft.com/en-us/azure/templates/microsoft.keyvault/2019-09-01/vaults/accesspolicies
resource kvAccess 'Microsoft.KeyVault/vaults/accessPolicies@2020-04-01-preview' = {
  name: 'add'
  parent: kv
  properties: {
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: objectId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
  }
}
