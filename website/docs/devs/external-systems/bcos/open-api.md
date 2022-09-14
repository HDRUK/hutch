---
sidebar_position: 3
---

# Open API (Swagger)

The best way to use the Open API spec is via the GUI on your instance of BC|OS:

    http(s)://<rquest-domain(:port)>/bcos-rest/swagger-ui.html#/

:::info
API Credentials may be required.
:::

### JSON Snapshot

This page provides a snapshot of the OpenAPI spec in JSON for where that may be useful.

:::caution
This page may be out of date compared to your running instance of BC|OS
:::

```json
{
  "swagger": "2.0",
  "info": {
    "description": "REST API for using BC|OS functionalities.",
    "title": "BC|OS REST API"
  },
  "host": "<rquest-host>",
  "basePath": "/bcos-rest",
  "tags": [
    { "name": "Collections", "description": "Fetching collections." },
    { "name": "Connector", "description": "Fetch for new job, submit result" },
    {
      "name": "Files",
      "description": "Upload files and use them later in availability queries etc."
    },
    { "name": "Results", "description": "Operations on jobs results" },
    {
      "name": "Tasks",
      "description": "Create tasks for rquest_backend, get statuses and results."
    }
  ],
  "paths": {
    "/api/collections/": {
      "get": {
        "tags": ["Collections"],
        "summary": "List active collections.",
        "operationId": "getActiveCollectionsUsingGET",
        "produces": ["application/json"],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
              "type": "array",
              "items": { "$ref": "#/definitions/GetCollectionResponse" }
            }
          },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/file/": {
      "post": {
        "tags": ["Files"],
        "summary": "Upload a file. Note that if a file with the same name exists it gets overridden.",
        "operationId": "saveFileUsingPOST",
        "consumes": ["multipart/form-data"],
        "produces": ["application/json"],
        "parameters": [
          {
            "name": "file",
            "in": "formData",
            "description": "file",
            "required": true,
            "type": "file"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": { "$ref": "#/definitions/FileSaveResponse" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/file/test": {
      "get": {
        "tags": ["Files"],
        "summary": "test",
        "operationId": "testUsingGET_1",
        "produces": ["*/*"],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/result/version": {
      "get": {
        "tags": ["Results"],
        "summary": "test",
        "operationId": "testUsingGET_3",
        "produces": ["*/*"],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/": {
      "post": {
        "tags": ["Tasks"],
        "summary": "Create a new job.",
        "operationId": "submitJobUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "in": "body",
            "name": "jobParameters",
            "description": "jobParameters",
            "required": true,
            "schema": { "$ref": "#/definitions/JobParameters" }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/JobId" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/nextjob/{collectionId}": {
      "get": {
        "tags": ["Tasks"],
        "summary": "Get next job.",
        "operationId": "getNextJobUsingGET",
        "produces": ["*/*", "application/octet-stream"],
        "parameters": [
          {
            "name": "collectionId",
            "in": "path",
            "description": "collectionId",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/ResponseEntity" }
          },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/queuestatus": {
      "post": {
        "tags": ["Tasks"],
        "summary": "Update LINK queue status",
        "operationId": "updateQueueStatusUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*"],
        "parameters": [
          {
            "in": "body",
            "name": "status",
            "description": "status",
            "required": true,
            "schema": { "type": "string" }
          }
        ],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/result/{uuid}/{collectionid}": {
      "post": {
        "tags": ["Tasks"],
        "summary": "Save job result",
        "operationId": "saveResultFileUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*"],
        "parameters": [
          {
            "name": "collectionid",
            "in": "path",
            "description": "collectionid",
            "required": true,
            "type": "string"
          },
          {
            "in": "body",
            "name": "result",
            "description": "result",
            "required": true,
            "schema": { "$ref": "#/definitions/Result" }
          },
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/resultfile/{uuid}/{collectionId}": {
      "post": {
        "tags": ["Tasks"],
        "summary": "Add job result file",
        "operationId": "addResultFileUsingPOST",
        "consumes": ["multipart/form-data"],
        "produces": ["*/*"],
        "parameters": [
          {
            "name": "collectionId",
            "in": "path",
            "description": "collectionId",
            "required": true,
            "type": "string"
          },
          {
            "name": "file",
            "in": "formData",
            "description": "file",
            "required": true,
            "type": "file"
          },
          {
            "name": "sensitive",
            "in": "query",
            "description": "sensitive",
            "required": true,
            "type": "boolean"
          },
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/results/{uuid}/{collectionId}": {
      "get": {
        "tags": ["Tasks"],
        "summary": "Get job result.",
        "operationId": "getJobResultUsingGET",
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "name": "collectionId",
            "in": "path",
            "description": "collectionId",
            "required": true,
            "type": "string"
          },
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": { "$ref": "#/definitions/Result" }
          },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/status/{uuid}": {
      "get": {
        "tags": ["Tasks"],
        "summary": "Get job info.",
        "operationId": "getJobStatusInfo1UsingGET",
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
              "type": "array",
              "items": { "$ref": "#/definitions/Pair«string,string»" }
            }
          },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/status/{uuid}/{collectionId}": {
      "get": {
        "tags": ["Tasks"],
        "summary": "Get job info.",
        "operationId": "getJobStatusInfo2UsingGET",
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "name": "collectionId",
            "in": "path",
            "description": "collectionId",
            "required": true,
            "type": "string"
          },
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "schema": {
              "type": "array",
              "items": { "$ref": "#/definitions/Pair«string,string»" }
            }
          },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/version": {
      "get": {
        "tags": ["Tasks"],
        "summary": "test",
        "operationId": "testUsingGET_2",
        "produces": ["*/*"],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/api/task/{uuid}/{collectionId}": {
      "delete": {
        "tags": ["Tasks"],
        "summary": "Cancel a job.",
        "operationId": "cancelJobUsingDELETE",
        "produces": ["*/*"],
        "parameters": [
          {
            "name": "collectionId",
            "in": "path",
            "description": "collectionId",
            "required": true,
            "type": "string"
          },
          {
            "name": "uuid",
            "in": "path",
            "description": "uuid",
            "required": true,
            "type": "string"
          }
        ],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "204": { "description": "No Content" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" }
        },
        "deprecated": false
      }
    },
    "/task/capi/cancel": {
      "post": {
        "tags": ["Connector"],
        "summary": "Get queue information",
        "operationId": "cancelJobUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "in": "body",
            "name": "collection",
            "description": "collection",
            "required": true,
            "schema": { "$ref": "#/definitions/Collection" }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/JsonNode" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/task/capi/query": {
      "post": {
        "tags": ["Connector"],
        "summary": "Fetch new job.",
        "operationId": "fetchNextJobUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "in": "body",
            "name": "collection",
            "description": "collection",
            "required": true,
            "schema": { "$ref": "#/definitions/Collection" }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/JsonNode" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/task/capi/queue": {
      "post": {
        "tags": ["Connector"],
        "summary": "Get queue information",
        "operationId": "getQueueInfoUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "in": "body",
            "name": "collection",
            "description": "collection",
            "required": true,
            "schema": { "$ref": "#/definitions/Collection" }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/JsonNode" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/task/capi/result": {
      "post": {
        "tags": ["Connector"],
        "summary": "Submit job result",
        "operationId": "submitJobResultUsingPOST",
        "consumes": ["application/json"],
        "produces": ["*/*", "application/json"],
        "parameters": [
          {
            "in": "body",
            "name": "collection",
            "description": "collection",
            "required": true,
            "schema": { "$ref": "#/definitions/Collection" }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "schema": { "$ref": "#/definitions/JsonNode" }
          },
          "201": { "description": "Created" },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    },
    "/task/capi/version": {
      "get": {
        "tags": ["Connector"],
        "summary": "test",
        "operationId": "testUsingGET",
        "produces": ["*/*"],
        "responses": {
          "200": { "description": "OK", "schema": { "type": "string" } },
          "401": { "description": "Unauthorized" },
          "403": { "description": "Forbidden" },
          "404": { "description": "Not Found" }
        },
        "deprecated": false
      }
    }
  },
  "definitions": {
    "Collection": {
      "type": "object",
      "properties": { "collection-id": { "type": "string" } },
      "title": "Collection"
    },
    "FileSaveResponse": {
      "type": "object",
      "properties": {
        "filename": { "type": "string" },
        "message": { "type": "string" },
        "timestamp": { "$ref": "#/definitions/Timestamp" }
      },
      "title": "FileSaveResponse"
    },
    "GetCollectionResponse": {
      "type": "object",
      "properties": {
        "externalId": { "type": "string" },
        "name": { "type": "string" },
        "protocol": { "type": "string", "enum": ["TCP_TUNNEL", "BCOS_REST"] },
        "rquestId": { "type": "string" }
      },
      "title": "GetCollectionResponse"
    },
    "JobId": {
      "type": "object",
      "properties": {
        "job-id": { "type": "string" },
        "job-uuid": { "type": "string" },
        "message": { "type": "string" }
      },
      "title": "JobId"
    },
    "JobParameters": {
      "type": "object",
      "required": ["application", "input"],
      "properties": {
        "application": { "type": "string", "example": "AVAILABILITY_QUERY" },
        "input": {
          "example": "\"input \": {  \"collections \": [ \"FI_DEMOCOL1 \"]}",
          "$ref": "#/definitions/JsonNode"
        }
      },
      "title": "JobParameters",
      "description": "Job parameters"
    },
    "JsonNode": { "type": "object", "title": "JsonNode" },
    "Pair«string,string»": {
      "type": "object",
      "properties": {
        "key": { "type": "string" },
        "left": { "type": "string" },
        "right": { "type": "string" },
        "value": { "type": "string" }
      },
      "title": "Pair«string,string»"
    },
    "QueryResult": {
      "type": "object",
      "properties": {
        "count": { "type": "integer", "format": "int64" },
        "files": {
          "type": "array",
          "items": { "$ref": "#/definitions/ResultFile" }
        }
      },
      "title": "QueryResult"
    },
    "ResponseEntity": {
      "type": "object",
      "properties": {
        "body": { "type": "object" },
        "statusCode": {
          "type": "string",
          "enum": [
            "100 CONTINUE",
            "101 SWITCHING_PROTOCOLS",
            "102 PROCESSING",
            "103 CHECKPOINT",
            "200 OK",
            "201 CREATED",
            "202 ACCEPTED",
            "203 NON_AUTHORITATIVE_INFORMATION",
            "204 NO_CONTENT",
            "205 RESET_CONTENT",
            "206 PARTIAL_CONTENT",
            "207 MULTI_STATUS",
            "208 ALREADY_REPORTED",
            "226 IM_USED",
            "300 MULTIPLE_CHOICES",
            "301 MOVED_PERMANENTLY",
            "302 FOUND",
            "302 MOVED_TEMPORARILY",
            "303 SEE_OTHER",
            "304 NOT_MODIFIED",
            "305 USE_PROXY",
            "307 TEMPORARY_REDIRECT",
            "308 PERMANENT_REDIRECT",
            "400 BAD_REQUEST",
            "401 UNAUTHORIZED",
            "402 PAYMENT_REQUIRED",
            "403 FORBIDDEN",
            "404 NOT_FOUND",
            "405 METHOD_NOT_ALLOWED",
            "406 NOT_ACCEPTABLE",
            "407 PROXY_AUTHENTICATION_REQUIRED",
            "408 REQUEST_TIMEOUT",
            "409 CONFLICT",
            "410 GONE",
            "411 LENGTH_REQUIRED",
            "412 PRECONDITION_FAILED",
            "413 PAYLOAD_TOO_LARGE",
            "413 REQUEST_ENTITY_TOO_LARGE",
            "414 URI_TOO_LONG",
            "414 REQUEST_URI_TOO_LONG",
            "415 UNSUPPORTED_MEDIA_TYPE",
            "416 REQUESTED_RANGE_NOT_SATISFIABLE",
            "417 EXPECTATION_FAILED",
            "418 I_AM_A_TEAPOT",
            "419 INSUFFICIENT_SPACE_ON_RESOURCE",
            "420 METHOD_FAILURE",
            "421 DESTINATION_LOCKED",
            "422 UNPROCESSABLE_ENTITY",
            "423 LOCKED",
            "424 FAILED_DEPENDENCY",
            "425 TOO_EARLY",
            "426 UPGRADE_REQUIRED",
            "428 PRECONDITION_REQUIRED",
            "429 TOO_MANY_REQUESTS",
            "431 REQUEST_HEADER_FIELDS_TOO_LARGE",
            "451 UNAVAILABLE_FOR_LEGAL_REASONS",
            "500 INTERNAL_SERVER_ERROR",
            "501 NOT_IMPLEMENTED",
            "502 BAD_GATEWAY",
            "503 SERVICE_UNAVAILABLE",
            "504 GATEWAY_TIMEOUT",
            "505 HTTP_VERSION_NOT_SUPPORTED",
            "506 VARIANT_ALSO_NEGOTIATES",
            "507 INSUFFICIENT_STORAGE",
            "508 LOOP_DETECTED",
            "509 BANDWIDTH_LIMIT_EXCEEDED",
            "510 NOT_EXTENDED",
            "511 NETWORK_AUTHENTICATION_REQUIRED"
          ]
        },
        "statusCodeValue": { "type": "integer", "format": "int32" }
      },
      "title": "ResponseEntity"
    },
    "Result": {
      "type": "object",
      "properties": {
        "collection_id": { "type": "string" },
        "protocolVersion": { "type": "string" },
        "queryResult": { "$ref": "#/definitions/QueryResult" },
        "status": { "type": "string" },
        "uuid": { "type": "string" }
      },
      "title": "Result"
    },
    "ResultFile": {
      "type": "object",
      "properties": {
        "file_data": { "type": "string" },
        "file_description": { "type": "string" },
        "file_name": { "type": "string" },
        "file_reference": { "type": "string" },
        "file_sensitive": { "type": "boolean" },
        "file_size": { "type": "number", "format": "double" },
        "file_type": { "type": "string", "enum": ["BCOS"] }
      },
      "title": "ResultFile"
    },
    "Timestamp": {
      "type": "object",
      "properties": {
        "date": { "type": "integer", "format": "int32" },
        "day": { "type": "integer", "format": "int32" },
        "hours": { "type": "integer", "format": "int32" },
        "minutes": { "type": "integer", "format": "int32" },
        "month": { "type": "integer", "format": "int32" },
        "nanos": { "type": "integer", "format": "int32" },
        "seconds": { "type": "integer", "format": "int32" },
        "time": { "type": "integer", "format": "int64" },
        "timezoneOffset": { "type": "integer", "format": "int32" },
        "year": { "type": "integer", "format": "int32" }
      },
      "title": "Timestamp"
    }
  }
}
```
