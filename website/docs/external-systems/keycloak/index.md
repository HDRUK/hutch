Keycloak is a popular Open ID Connect (OIDC) Identity Provider.

The TRE-FX stack uses OIDC Token auth for:
- The Workflow Executor (i.e. Hutch) to communicate with the TRE Controller API
- The Workflow Executor (i.e. Hutch) to communicate with the Intermediary Store (MinIO)

Hutch can bypass the need for OIDC Token auth when in Standalone mode (i.e. not communicating with a TRE Controller API) and/or if access credentials are passed directly for the Intermediary Store (e.g. at job submission) (or configured in the `StoreDefaults` config).

To use KeyCloak, some configuration is needed.

The `docker-compose` in the Hutch repo has an example of a keycloak instance suitable for development.

There is some futher configuration once this is running before it can be used.

## Further configuration

### Create a Realm

Minio in the sample `docker-compose` expects the realm to be called `hutch-dev`.

### Create a Client for Hutch

General Settings:

- type: `OpenID Connect`
- client id: e.g. `hutch-agent`

Capability Config:

- Client authentication: `ON`
- Authentication Flow:
  - [x] Service accounts roles
    - Hutch will use the Client Credentials flow as it doesn't act on a user's behalf

Login Settings:

not sure what is needed here as we aren't doing an interactive user login flow...

- Root URL should be Hutch's configured URL e.g. for development `http://host.docker.internal:5209`
- Most other settings are URLs and for development can just be wildcarded with `*`

### Configure Hutch

After creating the client, the Credentials tab can be used to generate or view the client secret.

Hutch can then be configured with

- the keycloak URL e.g. `localhost:9090`
- its client id e.g. `hutch-agent`
- its client secret
