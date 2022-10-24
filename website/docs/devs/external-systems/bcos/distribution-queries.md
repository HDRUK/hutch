---
sidebar_position: 5
---
# Distribution Queries

## Get a task from BC|OS REST API

Endpoint: `/link_connector_api/task/nextjob/{collectionId}.b`

:::tip
Don't forget the `.b` after your collection ID.
:::

:::info Example
If your collection ID is `myCollection`:

`/link_connector_api/task/nextjob/myCollection.b`
:::

### Sample return value
```json
{
"owner" : "user1",
"code" : "GENERIC",
"analysis" : "DISTRIBUTION",
"uuid" : "77d85e8e-0f9d-4452-a554-8caf4c96682a",
"collection" : "myCollection"
}
```
