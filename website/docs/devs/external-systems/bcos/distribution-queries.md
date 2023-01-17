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
The code field can be one of the following:
- DEMOGRAPHICS
- ICD-MAN
- GENERIC

## Uploading results for distribution queries
There are two endpoints for uploading distirbution query results:
1. `/link_connector_api/task/resultfile/{uuid}/{collectionId}`
2. `/link_connector_api/task/result/{uuid}/{collectionId}`

where `uuid` is the UUID that come with the distribution query and `collectionID` is the collection ID.

Use **endpoint 1.** is when you want to upload large files and/or sensitive data. **Endpoint 2.** is for when your data are small and/or not sensitive.

### Uploading large/sensitive data
There is a 2-step process for uploading large/sensitive data.

#### Step 1.
`POST` to **endpoint 1.**

The body of the `POST` request should take the following form:
```json
{
  "status": "ok",
  "protocolVersion": "v2",
  "uuid": "308e1f8d-520c-47fa-9dee-fb99cfd770aa",
  "queryResult": {
    "count": 0,
    "datasetsCount": 0,
    "files": [
      {
        "file_name": "{fileName}",
        "file_data": "QklPQkFOSwlDT0RFCURFU0NSSVBUSU9OCUNPVU5UCU1JTglR ...",
        "file_description": null,
        "file_size": 0,
        "file_type": "BCOS",
        "file_sensitive": false,
        "file_reference": ""
      }
    ]
  },
  "message": null,
  "collection_id": "myCollection"
}
```
The `file_name` parameter can be one of the following:
- code.distribution
  - used when the query code is **GENERIC**
- demographics.distribution
  - used when the query code is **DEMOGRAPHICS**
- icd_level1.distribution
  - used when the query code is **ICD-MAN**
- icd_level2.distribution
  - used when the query code is **ICD-MAN**

The choice depends on the type of distribution analysis being requested.

The `file_data` field needs to the body a file encoded as base-64.

#### Step 2.
`POST` to **endpoint 2.**

The body of the `POST` request should take the following form:
```json
{
  "status": "ok",
  "protocolVersion": "v2",
  "uuid": "308e1f8d-520c-47fa-9dee-fb99cfd770aa",
  "queryResult": {
    "count": 0,
    "datasetsCount": 0,
    "files": []
  },
  "message": null,
  "collection_id": "myCollection"
}
```
:::tip
The body of this `POST` request is essentially the same as in Step 1. but the `files` field is an empty array (`[]`).
:::

### Uploading large/sensitive data
There is a single step process for uploading small/not sensitive data.

:::caution Coming soon
We don't have information on this yet.
:::
