Keycloak is a popular Open ID Connect (OIDC) Identity Provider.

The TRE-FX stack uses OIDC Token auth for:
- The Workflow Executor (i.e. Hutch) to communicate with the TRE Controller API
- The Workflow Executor (i.e. Hutch) to communicate with the Intermediary Store (MinIO)

Hutch can bypass the need for OIDC Token auth when in Standalone mode (i.e. not communicating with a TRE Controller API) and/or if access credentials are passed directly for the Intermediary Store (e.g. at job submission) (or configured in the `StoreDefaults` config).

To use KeyCloak, some configuration is needed.

The `docker-compose` in the Hutch repo has an example of a keycloak instance suitable for development.

// TODO please-open.it

There is some futher configuration once this is running before it can be used.

## Further configuration

### Create a Realm

Minio in the sample `docker-compose` expects the realm to be called `hutch-dev`.

### Create a Client for Hutch

NOTE: You may wish to re-use the same client for Hutch and MinIO, since MinIO will only accept token from Hutch if they were requested using the MinIO Client Credentials.

General Settings:

- type: `OpenID Connect`
- client id: e.g. `hutch-agent`

Capability Config:

- Client authentication: `ON`
- Authentication Flow:
  - [x] Direct access grants
    - This is OIDC's "Resource Owner Password Credentials Grant" and is currently all Hutch supports, because MinIO and the TRE Controller API expect user tokens, not client ones.

Login Settings:

not sure what is needed here as we aren't doing an interactive user login flow...

- Root URL should be Hutch's configured URL e.g. for development `http://host.docker.internal:5209`
- Most other settings are URLs and for development can just be wildcarded with `*`

### Create (a) User(s)

Both MinIO and the TRE Controller API currently expect user tokens, so Hutch currently must have a user to get tokens for, which it does via the Password Grant Flow.

Additionally it's worth being aware that the TRE Controller expects an **access token** (so in future Hutch could use the Client Credentials flow), but Minio requires an **identity token**.

### Configure Hutch

After creating the client, the Credentials tab can be used to generate or view the client secret.

Hutch can then be configured with

- the keycloak URL e.g. `localhost:9090`
- its client id e.g. `hutch-agent`
- its client secret
- a username and password for a user to get a token on behalf of
