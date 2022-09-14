---
sidebar_position: 2
---
# Endpoint Schema

This document contains more detail on the request and response schema for BC|OS REST API v2 endpoints used by Hutch.

This information was gathered during Hutch development and reflects the usage in the Hutch codebase today.

It should be used to supplement the BC|OS [OpenAPI spec](open-api).

# Tasks

## `/api/task/nextjob/{collectionId}`

### Sample Response Body

```json
// TODO
```

## `/api/task/result/{uuid}/{collectionid}`

### Sample Request Body
  
  ```json
  {
    "collection_id": "RQ-CC-3278e0f7-b22d-4806-a654-700de32e11cc",
    "status": "OK"
    "protocolVersion": "2",
    "queryResult": {
    "count": 765,
    "files": []
    }
  }
  ```

### Response Codes

This table provides additional details, combined with those in the [OpenAPI spec](open-api).

|Status Code|Content|
|---|---|
|200|Job Saved|
|404|UUID not found; Collection ID not found|
|500|Job submitting failed null|

### Sample Response Bodies

#### 200
```json
// TODO
```


