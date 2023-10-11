you can connect minio to keycloak.

The `docker-compose` gives an example of this setup.

You'll also need to do some setup within Keycloak (detailed below) so that Minio can authenticate and have the right scopes/claims etc.

Note that this setup is using the **JWT Claim** approach detailed [here][Minio JWT Claim], *not* the **RolePolicy** approach from the same page.

## Create a Realm

If not already created, follow the instructions on creating a Realm in the Keycloak section.

It should match the realm name used in Minio's OpenID Config URL setting, e.g. `hutch-dev` for the development `docker-compose`. 

## Create a Client for Minio

General Settings:

- type: `OpenID Connect`
- client id: should match Minio config.
  - The development `docker-compose` uses `minio`

Capability Config:

- Authentication Flow:
  - [x] Standard flow
    - This is OIDC's "Authorization Code Flow" and is the only flow minio supports

Login Settings:

not sure what is needed here as we aren't doing an interactive user login flow...

- Root URL should be Minio's configured URL e.g. for development `http://host.docker.internal:9001`
- Most other settings are URLs and for development can just be wildcarded with `*`

## Create (a) User(s)

Minio's OIDC auth is wholly user-centric, so you will need users (at least one) in Keycloak.

## Grant Users Minio policy claims

Minio will only authenticate OIDC Users that have a `policy` claim containing one or more policies for Minio to apply to the users, as defined [here][Minio Policies].

To configure this in Keycloak you use "Mappers", which map from Keycloak data to token data.

There are a number of ways to configure Keycloak to do this, largely beyond the scope of this documentation.

What we will do for development purposes is configure a shared Client Scope to always add a default policy claim, rather than per user or role. Then we'll add that scope to the MinIO Client so MinIO can use it when authenticating.

We do it as a shared scope so that Hutch can use the same Client Scope as configured, since it also needs the policy claim to use with MinIO.

To do this:

1. Go to `Client scopes` in the left-hand nav
1. Create a new scope, called `minio_policies` or similar, and make its type `Default`.
1. On the `Mappers` tab, choose "Configure a new mapper" (not "Add predefined mapper")
1. Choose the type `Hardcoded claim`
1. Add a friendly name of your choice for the Mapper, e.g. `Minio Policies`
1. Token Claim Name should be `policy` to match Minio's default expectation
1. Claim value should be one or more [valid Minio Policies][Minio Policies]
  - `consoleAdmin` is a simple choice **for development only**
  - this value determines what rights within Minio the user(s) will be granted.
1. Turn on `Add to ID token` (required for Minio Auth) and `Add to access token` (since client applications like Hutch will use an access token to request Minio credentials).
1. Save

With the Client Scope created, you can add it to a client (e.g. Minio or Hutch):

1. Go to `Clients` in the left-hand nav and select the desired client.
1. On the `Client scopes` tab, choose `Add client scope`.
1. Select the Minio policies scope you created above.
1. Add it as `Default`.

[Minio Policies]: https://min.io/docs/minio/linux/administration/identity-access-management/policy-based-access-control.html#minio-policy
[Minio JWT Claim]: https://min.io/docs/minio/linux/administration/identity-access-management/oidc-access-management.html#json-web-token-claim
