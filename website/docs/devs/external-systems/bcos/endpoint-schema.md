---
sidebar_position: 2
---
# Endpoint Schema

This document contains more detail on the request and response schema for BC|OS REST API v2 endpoints used by Hutch.

This information was gathered during Hutch development and reflects the usage in the Hutch codebase today.

It should be used to supplement the BC|OS [OpenAPI spec](open-api).

# Tasks

## `/api/task/nextjob/{collectionId}`

### Response Codes

| Status Code | Content |
|-|-|
| `200` | OK |
| `204` | No tasks in the queue for this Collection |
| `401` | Unauthorised |
| `404` | Collection not found |

### Sample Response Body

#### `200`
```json
{
  "owner": "user1",
  "cohort": {
    "groups": [
      {
        "rules": [
          {
            "varname": "SEX",
            "type": "ALT",
            "oper": "=",
            "value": "1"
          }
        ],
        "rules_oper": "OR"
      }
    ],
    "groups_oper": "AND"
  },
  "collection": "RQ-CC-56ed86bc-999e-45e9-8efe-c4ca53b18479",
  "protocol_version": "v2",
  "char_salt": "8e012537-66a5-4da4-9375-93347ab8716d",
  "uuid": "816bbdf0-f0b0-4710-9b76-f6e5ea14513e"
}
```

## `/api/task/result/{uuid}/{collectionid}`

### Sample Request Body
  
```json
{
  "collection_id": "RQ-CC-3278e0f7-b22d-4806-a654-700de32e11cc",
  "status": "OK",
  "protocolVersion": "2",
  "queryResult": {
    "count": 765,
    "files": []
  }
}
```

### Response Codes

| Status Code | Content |
|-|-|
| `200` | Job Saved |
| `404` | UUID not found; Collection ID not found |
| `500` | Job submitting failed null |

### Sample Response Body

#### `200`
```json
// TODO
```


